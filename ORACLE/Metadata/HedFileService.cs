namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Service für .HED-Datei-Operationen: Suchen, Kopieren, Lesen und Schreiben.
    /// 
    /// .HED-Dateien enthalten Metadaten im INI-ähnlichen Format:
    ///   [KTM_NAMING]
    ///   bikeName = JP01
    ///   trackName = Maggiora
    ///   trackLength = 1650
    ///   riderName = Jorge Prado
    /// </summary>
    public class HedFileService
    {
        /// <summary>
        /// Sucht eine .HED-Datei im Rider- und Run-Ordner.
        /// Falls im Rider-Ordner gefunden, wird die Datei in den Run-Ordner kopiert.
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner</param>
        /// <param name="riderPath">Pfad zum Rider-Ordner (eine Ebene über dem Run)</param>
        /// <returns>Pfad zur .HED-Datei im Run-Ordner, oder null wenn keine gefunden</returns>
        public string? FindAndPrepareHedFile(string runPath, string riderPath)
        {
            // 1. Suche im Rider-Ordner (primäre Quelle)
            var hedFile = FindFirstHedFile(riderPath);
            if (hedFile != null)
            {
                Console.WriteLine($".HED-File existiert in Riderordner: {Path.GetFileName(hedFile)}");
                return CopyToRunFolder(hedFile, runPath);
            }

            // 2. Fallback: Suche im Run-Ordner selbst
            hedFile = FindFirstHedFile(runPath);
            if (hedFile != null)
            {
                Console.WriteLine($".HED-File existiert im Runordner: {Path.GetFileName(hedFile)}");
                return hedFile;
            }

            return null;
        }

        /// <summary>
        /// Liest die Kern-Metadaten (bikeName, trackName, trackLength, riderName) aus einer .HED-Datei.
        /// </summary>
        /// <param name="hedFilePath">Vollständiger Pfad zur .HED-Datei</param>
        /// <returns>RunMetadata mit den gelesenen Werten (leere Strings für fehlende Keys)</returns>
        public RunMetadata ReadMetadata(string hedFilePath)
        {
            var values = ParseHedFile(hedFilePath);

            var rawBikeName = values.GetValueOrDefault("bikeName", "");
            if (!BikeAlias.IsCanonical(rawBikeName))
                Console.WriteLine($"Warnung: bikeName \"{rawBikeName}\" aus .HED enthält Leerzeichen — normalisiere zu \"{BikeAlias.Normalize(rawBikeName)}\".");

            return new RunMetadata
            {
                BikeName = BikeAlias.Normalize(rawBikeName),
                TrackName = values.GetValueOrDefault("trackName", ""),
                TrackLength = values.GetValueOrDefault("trackLength", ""),
                RiderName = values.GetValueOrDefault("riderName", ""),
                EngineClass = values.GetValueOrDefault("engineClass", ""),
            };
        }

        /// <summary>
        /// Kanonischer .HED-Dateiname: <c>{SeasonName}{EventName}_{BikeName}{SessionName}.HED</c>
        /// (ohne Leerzeichen), z.B. <c>2026RT22_GER-Teutschenthal_AA01R1.HED</c>.
        /// </summary>
        public static string BuildHedFileName(RunMetadata metadata)
            => $"{metadata.SeasonName}{metadata.EventName}_{metadata.BikeName}{metadata.SessionName}.HED";

        /// <summary>
        /// Stellt sicher, dass in JEDEM Run-Ordner genau eine .HED-Datei mit dem kanonischen Namen
        /// (<see cref="BuildHedFileName"/>) und normalisiertem Inhalt liegt. Inhaltsquelle ist die erste
        /// vorhandene .HED (Run- oder Rider-Ordner), sonst eine aus den Metadaten generierte.
        /// Abweichend benannte .HED-Dateien im Run-Ordner werden entfernt. Non-fatal je Run-Ordner.
        /// </summary>
        public void EnsureHedInAllRuns(IReadOnlyList<string> runPaths, string riderPath, RunMetadata metadata)
        {
            if (runPaths.Count == 0) return;

            var canonicalName = BuildHedFileName(metadata);

            var source = runPaths.Select(FindFirstHedFile).FirstOrDefault(p => p != null)
                         ?? FindFirstHedFile(riderPath);
            var lines = source != null && File.Exists(source)
                ? NormalizeBikeNameInLines(File.ReadAllLines(source), metadata.BikeName)
                : BuildHedContent(metadata).Split('\n').Select(l => l.TrimEnd('\r')).ToArray();

            int count = 0;
            foreach (var runPath in runPaths)
            {
                try
                {
                    foreach (var old in Directory.EnumerateFiles(runPath, "*.HED", SearchOption.TopDirectoryOnly).ToList())
                        if (!string.Equals(Path.GetFileName(old), canonicalName, StringComparison.OrdinalIgnoreCase))
                            File.Delete(old);

                    File.WriteAllLines(Path.Combine(runPath, canonicalName), lines);
                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[hed] ⚠️ HED für {Path.GetFileName(runPath)} übersprungen: {ex.Message}");
                }
            }
            Console.WriteLine($"[hed] .HED in {count}/{runPaths.Count} Run-Ordner(n) sichergestellt: \"{canonicalName}\".");
        }

        /// <summary>Setzt die <c>bikeName</c>-Zeile im .HED-Inhalt auf den kanonischen Wert.</summary>
        private static string[] NormalizeBikeNameInLines(string[] lines, string canonicalBikeName)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                var eq = lines[i].IndexOf('=');
                if (eq > 0 && lines[i][..eq].Trim().Equals("bikeName", StringComparison.OrdinalIgnoreCase))
                    lines[i] = $"{lines[i][..eq].TrimEnd()} = {canonicalBikeName}";
            }
            return lines;
        }

        /// <summary>
        /// Parst eine .HED-Datei und gibt alle Key-Value-Paare zurück.
        /// Ignoriert Sektionsheader ([...]) und leere Zeilen.
        /// </summary>
        /// <param name="hedFilePath">Vollständiger Pfad zur .HED-Datei</param>
        /// <returns>Dictionary mit Key-Value-Paaren (case-insensitive Keys)</returns>
        public Dictionary<string, string> ParseHedFile(string hedFilePath)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in File.ReadLines(hedFilePath))
            {
                var trimmed = line.Trim();
                // Leere Zeilen und Sektionsheader ([KTM_NAMING]) überspringen
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('['))
                    continue;

                var idx = trimmed.IndexOf('=');
                // Ungültige Zeilen überspringen (kein '=', leerer Key oder leerer Value)
                if (idx <= 0 || idx == trimmed.Length - 1)
                    continue;

                var key = trimmed[..idx].Trim();
                var value = trimmed[(idx + 1)..].Trim();

                if (!string.IsNullOrEmpty(key))
                    values[key] = value;
            }

            return values;
        }

        /// <summary>
        /// Erzeugt eine .HED-Datei im Run-Ordner mit vollständigen Metadaten.
        /// Dateiname: <c>{SeasonName}{EventName}_{BikeName}{SessionName}.HED</c> (siehe <see cref="BuildHedFileName"/>).
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner</param>
        /// <param name="metadata">Vollständige Metadaten für die .HED-Datei</param>
        public void WriteHedFile(string runPath, RunMetadata metadata)
        {
            var hedFilePath = Path.Combine(runPath, BuildHedFileName(metadata));
            File.WriteAllText(hedFilePath, BuildHedContent(metadata));
            Console.WriteLine($"Neues .HED-File erstellt: {hedFilePath}");
        }

        /// <summary>Erzeugt den vollständigen .HED-Inhalt (INI-Format) aus den Metadaten.</summary>
        private static string BuildHedContent(RunMetadata metadata) =>
$@"[KTM_NAMING]
disciplineName = {metadata.DisciplineName}
seasonName = {metadata.SeasonName}
calendarweek = {metadata.CalendarWeek}
teamName = {metadata.TeamName}
riderName = {metadata.RiderName}
riderabbreviationName = {metadata.RiderAbbreviation}
bikeNumber = {metadata.BikeNumber}
bikeName = {metadata.BikeName}
engineClass = {metadata.EngineClass}
typeName = {metadata.TypeName}
countrycode = {metadata.CountryCode}
countryName = {metadata.CountryName}
trackName = {metadata.TrackName}
trackLength = {metadata.TrackLength}
eventName = {metadata.EventName}
sessionName = {metadata.SessionName}
runNumber = {metadata.RunNumber}";

        // --- Private Hilfsmethoden ---

        /// <summary>
        /// Sucht die erste .HED-Datei in einem Verzeichnis (keine Rekursion).
        /// </summary>
        private static string? FindFirstHedFile(string directory)
        {
            if (!Directory.Exists(directory))
                return null;

            return Directory.EnumerateFiles(directory, "*.HED", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
        }

        /// <summary>
        /// Kopiert eine .HED-Datei in den Run-Ordner, falls noch nicht vorhanden.
        /// </summary>
        /// <returns>Pfad zur Zieldatei, oder null bei Fehler</returns>
        private static string? CopyToRunFolder(string sourceHedFile, string runPath)
        {
            var fileName = Path.GetFileName(sourceHedFile);
            var destFile = Path.Combine(runPath, fileName);

            // Datei bereits vorhanden → direkt verwenden
            if (File.Exists(destFile))
            {
                Console.WriteLine($"Datei {fileName} ist bereits im Runordner vorhanden.");
                return destFile;
            }

            try
            {
                File.Copy(sourceHedFile, destFile);
                Console.WriteLine($"Datei {fileName} wurde in den Runordner kopiert.");
                return destFile;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Kopieren der Datei: {ex.Message}");
                return null;
            }
        }
    }
}
