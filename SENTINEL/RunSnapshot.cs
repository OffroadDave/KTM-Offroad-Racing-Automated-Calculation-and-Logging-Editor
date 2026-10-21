using System.Xml.Linq;

namespace SENTINEL
{
    /// <summary>
    /// Punkt-in-Zeit-Aufnahme des Dateisystem-Zustands eines Run-Ordners.
    /// 
    /// Wird bei jedem Polling-Zyklus für jeden Run ohne download.done erstellt.
    /// Zwei aufeinanderfolgende Snapshots werden verglichen um Stabilität zu erkennen:
    /// - Gleiche Dateianzahl + gleiche Gesamtgröße + vollständig + keine Platzhalter/0-Byte-Dateien + keine Locks = stabil
    /// </summary>
    public sealed class RunSnapshot
    {
        // Windows-Attribute von Cloud-Platzhaltern (OneDrive Files-On-Demand): Daten liegen noch nicht lokal.
        private const FileAttributes RecallOnOpen = (FileAttributes)0x00040000;
        private const FileAttributes RecallOnDataAccess = (FileAttributes)0x00400000;

        private static readonly EnumerationOptions Enumeration = new() { IgnoreInaccessible = true, RecurseSubdirectories = false };
        private static readonly EnumerationOptions RecursiveEnumeration = new() { IgnoreInaccessible = true, RecurseSubdirectories = true };

        /// <summary>Vollständiger Pfad zum Run-Ordner</summary>
        public required string RunPath { get; init; }

        /// <summary>Zeitstempel der Snapshot-Erstellung</summary>
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        /// <summary>Anzahl aller Dateien rekursiv im Run-Ordner</summary>
        public int FileCount { get; init; }

        /// <summary>Summe aller Dateigrößen in Bytes</summary>
        public long TotalSizeBytes { get; init; }

        /// <summary>Anzahl Lap_*-Unterordner</summary>
        public int LapCount { get; init; }

        /// <summary>True wenn mindestens eine Datei 0 Bytes groß ist bzw. eine ZTX-Datei unter der Mindestgröße liegt</summary>
        public bool HasZeroByteFiles { get; init; }

        /// <summary>True wenn mindestens eine Datei ein Cloud-Platzhalter ist (Inhalt noch nicht heruntergeladen)</summary>
        public bool HasPlaceholderFiles { get; init; }

        /// <summary>True wenn mindestens eine Datei nicht lesbar ist (gelockt durch Sync)</summary>
        public bool HasLockedFiles { get; init; }

        /// <summary>
        /// Lap-Dateien (relativ zum Run), die <c>RunLapHeader.xml</c> referenziert, die aber (noch) fehlen.
        /// Enthält <c>RunLapHeader.xml</c> selbst, wenn die Datei fehlt oder nicht lesbar ist.
        /// </summary>
        public List<string> MissingFiles { get; init; } = [];

        /// <summary>Dateien auf Run-Ebene (z.B. Session.xml)</summary>
        public List<FileSnapshot> RunFiles { get; init; } = [];

        /// <summary>Detailinfo pro Lap-Ordner</summary>
        public List<LapSnapshot> Laps { get; init; } = [];

        /// <summary>True wenn alle in RunLapHeader.xml referenzierten Laps mit Daten vorhanden sind.</summary>
        public bool IsComplete => MissingFiles.Count == 0;

        /// <summary>
        /// Strukturell gültig: Mindestens 1 Lap, vollständig laut RunLapHeader.xml, keine 0-Byte-Dateien, keine Platzhalter,
        /// keine Locks. Ein strukturell gültiger Snapshot kann für den Stabilitätsvergleich herangezogen werden.
        /// </summary>
        public bool IsStructurallyValid => LapCount > 0 && IsComplete && !HasZeroByteFiles && !HasPlaceholderFiles && !HasLockedFiles;

        /// <summary>
        /// Vergleicht diesen Snapshot mit einem vorherigen auf Stabilität.
        /// Stabil = gleiche Dateianzahl + gleiche Gesamtgröße + strukturell gültig.
        /// </summary>
        public bool IsStableComparedTo(RunSnapshot? previous)
        {
            if (previous is null) return false;

            return IsStructurallyValid
                && FileCount == previous.FileCount
                && TotalSizeBytes == previous.TotalSizeBytes;
        }

        /// <summary>
        /// Erstellt einen Snapshot des angegebenen Run-Ordners.
        /// Scannt rekursiv alle Dateien, prüft Größen, Platzhalter, File-Locks und die Vollständigkeit laut RunLapHeader.xml.
        /// </summary>
        /// <param name="runPath">Pfad zum Run_xxx-Ordner</param>
        /// <param name="minimumFileSize">Minimale Dateigröße in Bytes (unter diesem Wert = Platzhalter)</param>
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

                // Platzhalter nie öffnen: das Öffnen würde den Download auslösen.
                if (IsPlaceholder(file))
                    hasPlaceholders = true;
                else if (IsFileLocked(file.FullName))
                    hasLockedFiles = true;
            }

            // --- Dateien auf Run-Ebene scannen (z.B. Session.xml, Car.xml) ---
            foreach (var file in runDir.GetFiles("*", Enumeration))
            {
                Inspect(file);
                runFiles.Add(new FileSnapshot { FileName = file.Name, SizeBytes = file.Length });
            }

            // --- Lap_*-Ordner scannen ---
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
        /// Vergleicht <c>RunLapHeader.xml</c> mit den vorhandenen Dateien: Jede in <c>LapInfo/Path</c> referenzierte
        /// Datei (z.B. <c>Lap_…\cableData.ztx</c>) muss vorhanden sein. Ein reines Größen-Gleichbleiben genügt nicht, weil ein pausierter Sync
        /// (offline, Standby) ebenfalls „stabil“ aussieht.
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
                    // Pfade außerhalb des Run-Ordners (absolut oder mit ..) zählen als fehlend.
                    var full = Path.GetFullPath(Path.Combine(root, relative));
                    return !full.StartsWith(root, StringComparison.OrdinalIgnoreCase) || !File.Exists(full);
                })
                .ToList();
        }

        /// <summary>True für Cloud-Platzhalter (OneDrive Files-On-Demand), deren Inhalt noch nicht lokal vorliegt.</summary>
        internal static bool IsPlaceholder(FileInfo file)
        {
            var attributes = file.Attributes;
            return (attributes & (FileAttributes.Offline | RecallOnOpen | RecallOnDataAccess)) != 0;
        }

        /// <summary>
        /// Prüft ob eine Datei durch einen anderen Prozess (z.B. OneDrive) gesperrt ist.
        /// Versucht die Datei mit FileShare.Read zu öffnen — schlägt fehl wenn exklusiv gelockt.
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

    /// <summary>Snapshot einer einzelnen Datei (Name + Größe).</summary>
    public sealed class FileSnapshot
    {
        public required string FileName { get; init; }
        public long SizeBytes { get; init; }
    }

    /// <summary>Snapshot eines Lap-Ordners mit allen enthaltenen Dateien.</summary>
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
