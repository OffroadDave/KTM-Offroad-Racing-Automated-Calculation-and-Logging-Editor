using System.Xml.Linq;

namespace SENTINEL
{
    /// <summary>
    /// Point-in-time capture of the file system state of a run folder.
    /// 
    /// Created on every polling cycle for each run without download.done.
    /// Two consecutive snapshots are compared to detect stability:
    /// - Same file count + same total size + complete + no placeholder/0-byte files + no locks = stable
    /// </summary>
    public sealed class RunSnapshot
    {
        // Windows attributes of cloud placeholders (OneDrive Files-On-Demand): data is not yet available locally.
        private const FileAttributes RecallOnOpen = (FileAttributes)0x00040000;
        private const FileAttributes RecallOnDataAccess = (FileAttributes)0x00400000;

        private static readonly EnumerationOptions Enumeration = new() { IgnoreInaccessible = true, RecurseSubdirectories = false };
        private static readonly EnumerationOptions RecursiveEnumeration = new() { IgnoreInaccessible = true, RecurseSubdirectories = true };

        /// <summary>Full path to the run folder</summary>
        public required string RunPath { get; init; }

        /// <summary>Timestamp of snapshot creation</summary>
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        /// <summary>Number of all files (recursive) in the run folder</summary>
        public int FileCount { get; init; }

        /// <summary>Sum of all file sizes in bytes</summary>
        public long TotalSizeBytes { get; init; }

        /// <summary>Number of Lap_* subfolders</summary>
        public int LapCount { get; init; }

        /// <summary>True if at least one file is 0 bytes or a ZTX file is below the minimum size</summary>
        public bool HasZeroByteFiles { get; init; }

        /// <summary>True if at least one file is a cloud placeholder (content not yet downloaded)</summary>
        public bool HasPlaceholderFiles { get; init; }

        /// <summary>True if at least one file is not readable (locked by sync)</summary>
        public bool HasLockedFiles { get; init; }

        /// <summary>
        /// Lap files (relative to the run) referenced by <c>RunLapHeader.xml</c> that are (still) missing.
        /// Contains <c>RunLapHeader.xml</c> itself if the file is missing or not readable.
        /// </summary>
        public List<string> MissingFiles { get; init; } = [];

        /// <summary>Files at run level (e.g. Session.xml)</summary>
        public List<FileSnapshot> RunFiles { get; init; } = [];

        /// <summary>Detail info per lap folder</summary>
        public List<LapSnapshot> Laps { get; init; } = [];

        /// <summary>True if all laps referenced in RunLapHeader.xml are present with data.</summary>
        public bool IsComplete => MissingFiles.Count == 0;

        /// <summary>
        /// Structurally valid: at least 1 lap, complete according to RunLapHeader.xml, no 0-byte files, no placeholders,
        /// no locks. A structurally valid snapshot can be used for the stability comparison.
        /// </summary>
        public bool IsStructurallyValid => LapCount > 0 && IsComplete && !HasZeroByteFiles && !HasPlaceholderFiles && !HasLockedFiles;

        /// <summary>
        /// Compares this snapshot with a previous one for stability.
        /// Stable = same file count + same total size + structurally valid.
        /// </summary>
        public bool IsStableComparedTo(RunSnapshot? previous)
        {
            if (previous is null) return false;

            return IsStructurallyValid
                && FileCount == previous.FileCount
                && TotalSizeBytes == previous.TotalSizeBytes;
        }

        /// <summary>
        /// Creates a snapshot of the given run folder.
        /// Recursively scans all files, checks sizes, placeholders, file locks and completeness according to RunLapHeader.xml.
        /// </summary>
        /// <param name="runPath">Path to the Run_xxx folder</param>
        /// <param name="minimumFileSize">Minimum file size in bytes (below this value = placeholder)</param>
        public static RunSnapshot Create(string runPath, long minimumFileSize = 1024)
        {
            var runDir = new DirectoryInfo(runPath);
            if (!runDir.Exists)
            {
                return new RunSnapshot
                {
                    RunPath = runPath,
                    FileCount = 0,
                    TotalSizeBytes = 0,
                };
            }

            var runFiles = new List<FileSnapshot>();
            var laps = new List<LapSnapshot>();
            int totalFileCount = 0;
            long totalSize = 0;
            bool hasZeroByteFiles = false;
            bool hasPlaceholders = false;
            bool hasLockedFiles = false;

            void Inspect(FileInfo file)
            {
                totalFileCount++;
                totalSize += file.Length;

                if (file.Length == 0)
                    hasZeroByteFiles = true;

                // Never open placeholders: opening would trigger the download.
                if (IsPlaceholder(file))
                    hasPlaceholders = true;
                else if (IsFileLocked(file.FullName))
                    hasLockedFiles = true;
            }

            // --- Scan files at run level (e.g. Session.xml, Car.xml) ---
            foreach (var file in runDir.GetFiles("*", Enumeration))
            {
                Inspect(file);
                runFiles.Add(new FileSnapshot { FileName = file.Name, SizeBytes = file.Length });
            }

            // --- Scan Lap_* folders ---
            var lapDirs = runDir.GetDirectories("Lap_*", Enumeration)
                .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            foreach (var lapDir in lapDirs)
            {
                var lapFiles = new List<FileSnapshot>();
                long lapSize = 0;
                int lapFileCount = 0;

                foreach (var file in lapDir.GetFiles("*", RecursiveEnumeration))
                {
                    lapFileCount++;
                    lapSize += file.Length;
                    Inspect(file);

                    if (file.Length < minimumFileSize && file.Extension.Equals(".ztx", StringComparison.OrdinalIgnoreCase))
                        hasZeroByteFiles = true;

                    lapFiles.Add(new FileSnapshot { FileName = file.Name, SizeBytes = file.Length });
                }

                laps.Add(new LapSnapshot
                {
                    LapPath = lapDir.FullName,
                    LapName = lapDir.Name,
                    SizeBytes = lapSize,
                    SubfolderCount = lapDir.GetDirectories("*", Enumeration).Length,
                    FileCount = lapFileCount,
                    Files = lapFiles,
                });
            }

            return new RunSnapshot
            {
                RunPath = runPath,
                FileCount = totalFileCount,
                TotalSizeBytes = totalSize,
                LapCount = lapDirs.Length,
                HasZeroByteFiles = hasZeroByteFiles,
                HasPlaceholderFiles = hasPlaceholders,
                HasLockedFiles = hasLockedFiles,
                MissingFiles = FindMissingFiles(runDir),
                RunFiles = runFiles,
                Laps = laps,
            };
        }

        /// <summary>
        /// Compares <c>RunLapHeader.xml</c> with the existing files: every file referenced in <c>LapInfo/Path</c>
        /// (e.g. <c>Lap_…\cableData.ztx</c>) must be present. Merely unchanged sizes are not enough, because a paused sync
        /// (offline, standby) also looks "stable".
        /// </summary>
        internal static List<string> FindMissingFiles(DirectoryInfo runDir)
        {
            var header = Path.Combine(runDir.FullName, "RunLapHeader.xml");
            if (!File.Exists(header) || IsPlaceholder(new FileInfo(header)))
                return ["RunLapHeader.xml"];

            List<string> referenced;
            try
            {
                referenced = XDocument.Load(header).Descendants("LapInfo")
                    .Select(info => (info.Element("Path")?.Value ?? string.Empty).Trim())
                    .Where(path => path.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (Exception ex) when (ex is IOException or System.Xml.XmlException or UnauthorizedAccessException)
            {
                return ["RunLapHeader.xml"];
            }

            var root = Path.GetFullPath(runDir.FullName).TrimEnd('\\') + "\\";
            return referenced
                .Where(relative =>
                {
                    // Paths outside the run folder (absolute or with ..) count as missing.
                    var full = Path.GetFullPath(Path.Combine(root, relative));
                    return !full.StartsWith(root, StringComparison.OrdinalIgnoreCase) || !File.Exists(full);
                })
                .ToList();
        }

        /// <summary>True for cloud placeholders (OneDrive Files-On-Demand) whose content is not yet available locally.</summary>
        internal static bool IsPlaceholder(FileInfo file)
        {
            var attributes = file.Attributes;
            return (attributes & (FileAttributes.Offline | RecallOnOpen | RecallOnDataAccess)) != 0;
        }

        /// <summary>
        /// Checks whether a file is locked by another process (e.g. OneDrive).
        /// Tries to open the file with FileShare.Read — fails if exclusively locked.
        /// </summary>
        private static bool IsFileLocked(string filePath)
        {
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return false;
            }
            catch (IOException)
            {
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
        }
    }

    /// <summary>Snapshot of a single file (name + size).</summary>
    public sealed class FileSnapshot
    {
        public required string FileName { get; init; }
        public long SizeBytes { get; init; }
    }

    /// <summary>Snapshot of a lap folder with all contained files.</summary>
    public sealed class LapSnapshot
    {
        public required string LapPath { get; init; }
        public required string LapName { get; init; }
        public long SizeBytes { get; init; }
        public int SubfolderCount { get; init; }
        public int FileCount { get; init; }
        public List<FileSnapshot> Files { get; init; } = [];
    }
}
