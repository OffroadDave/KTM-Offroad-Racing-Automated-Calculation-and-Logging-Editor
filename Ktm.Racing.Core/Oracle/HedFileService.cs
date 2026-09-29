namespace Ktm.Racing.Core
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
                var fileName = Path.GetFileName(hedFile);
                Console.WriteLine($".HED-File existiert in Riderordner: {fileName}");
                return CopyToRunFolder(hedFile, runPath);
            }

            // 2. Fallback: Suche im Run-Ordner selbst
            hedFile = FindFirstHedFile(runPath);
            if (hedFile != null)
            {
                var fileName = Path.GetFileName(hedFile);
                Console.WriteLine($".HED-File existiert im Runordner: {fileName}");
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
            };
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
        /// Dateiname: "{SeasonName} {EventName} Specsheet {BikeName} {SessionName}.HED"
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner</param>
        /// <param name="metadata">Vollständige Metadaten für die .HED-Datei</param>
        public void WriteHedFile(string runPath, RunMetadata metadata)
        {
            // .HED-File im gewünschten Format erzeugen
            var content =
$@"[KTM_NAMING]
disciplineName = {metadata.DisciplineName}
seasonName = {metadata.SeasonName}
calendarweek = {metadata.CalendarWeek}
teamName = {metadata.TeamName}
riderName = {metadata.RiderName}
riderabbreviationName = {metadata.RiderAbbreviation}
bikeNumber = {metadata.BikeNumber}
bikeName = {metadata.BikeName}
typeName = {metadata.TypeName}
countrycode = {metadata.CountryCode}
countryName = {metadata.CountryName}
trackName = {metadata.TrackName}
trackLength = {metadata.TrackLength}
eventName = {metadata.EventName}
sessionName = {metadata.SessionName}
runNumber = {metadata.RunNumber}";

            var hedFileName = $"{metadata.SeasonName} {metadata.EventName} Specsheet {metadata.BikeName} {metadata.SessionName}.HED";
            var hedFilePath = Path.Combine(runPath, hedFileName);
            File.WriteAllText(hedFilePath, content);
            Console.WriteLine($"Neues .HED-File erstellt: {hedFilePath}");
        }

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
