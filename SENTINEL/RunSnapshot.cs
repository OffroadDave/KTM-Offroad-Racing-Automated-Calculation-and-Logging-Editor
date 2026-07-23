namespace SENTINEL
{
    /// <summary>
    /// Punkt-in-Zeit-Aufnahme des Dateisystem-Zustands eines Run-Ordners.
    /// 
    /// Wird bei jedem Polling-Zyklus für jeden Run ohne download.done erstellt.
    /// Zwei aufeinanderfolgende Snapshots werden verglichen um Stabilität zu erkennen:
    /// - Gleiche Dateianzahl + gleiche Gesamtgröße + keine 0-Byte-Dateien + keine Locks = stabil
    /// </summary>
    public sealed class RunSnapshot
    {
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

        /// <summary>True wenn mindestens eine Datei 0 Bytes groß ist (OneDrive-Platzhalter)</summary>
        public bool HasZeroByteFiles { get; init; }

        /// <summary>True wenn mindestens eine Datei nicht lesbar ist (gelockt durch Sync)</summary>
        public bool HasLockedFiles { get; init; }

        /// <summary>Dateien auf Run-Ebene (z.B. Session.xml)</summary>
        public List<FileSnapshot> RunFiles { get; init; } = [];

        /// <summary>Detailinfo pro Lap-Ordner</summary>
        public List<LapSnapshot> Laps { get; init; } = [];

        /// <summary>
        /// Strukturell gültig: Mindestens 1 Lap, keine 0-Byte-Dateien, keine Locks.
        /// Ein strukturell gültiger Snapshot kann für den Stabilitätsvergleich herangezogen werden.
        /// </summary>
        public bool IsStructurallyValid => LapCount > 0 && !HasZeroByteFiles && !HasLockedFiles;

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
        /// Scannt rekursiv alle Dateien, prüft Größen und File-Locks.
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
            bool hasLockedFiles = false;

            // --- Dateien auf Run-Ebene scannen (z.B. Session.xml, Car.xml) ---
            foreach (var file in runDir.GetFiles())
            {
                totalFileCount++;
                totalSize += file.Length;

                if (file.Length == 0)
                    hasZeroByteFiles = true;

                if (IsFileLocked(file.FullName))
                    hasLockedFiles = true;

                runFiles.Add(new FileSnapshot { FileName = file.Name, SizeBytes = file.Length });
            }

            // --- Lap_*-Ordner scannen ---
            var lapDirs = runDir.GetDirectories("Lap_*", SearchOption.TopDirectoryOnly)
                .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            foreach (var lapDir in lapDirs)
            {
                var lapFiles = new List<FileSnapshot>();
                long lapSize = 0;
                int lapFileCount = 0;

                foreach (var file in lapDir.GetFiles("*", SearchOption.AllDirectories))
                {
                    lapFileCount++;
                    lapSize += file.Length;
                    totalFileCount++;
                    totalSize += file.Length;

                    if (file.Length == 0)
                        hasZeroByteFiles = true;

                    if (file.Length < minimumFileSize && file.Extension.Equals(".ztx", StringComparison.OrdinalIgnoreCase))
                        hasZeroByteFiles = true;

                    if (IsFileLocked(file.FullName))
                        hasLockedFiles = true;

                    lapFiles.Add(new FileSnapshot { FileName = file.Name, SizeBytes = file.Length });
                }

                laps.Add(new LapSnapshot
                {
                    LapPath = lapDir.FullName,
                    LapName = lapDir.Name,
                    SizeBytes = lapSize,
                    SubfolderCount = lapDir.GetDirectories().Length,
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
                HasLockedFiles = hasLockedFiles,
                RunFiles = runFiles,
                Laps = laps,
            };
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
