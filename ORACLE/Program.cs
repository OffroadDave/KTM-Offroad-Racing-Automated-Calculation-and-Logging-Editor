using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using ORACLE; // Importiere die Service-Klasse für XML-Bearbeitung

class Program
{
    // Geladene, deserialisierte Daten: Key = "MX" / "EN" / ...
    private Dictionary<string, DisciplineData> _disciplines = new Dictionary<string, DisciplineData>(StringComparer.OrdinalIgnoreCase);
    private static string bikeName;
    private static string trackName;
    private static string trackLength;
    private static string riderName;

    static async Task Main(string[] args)
    {
        // --- Argumentprüfung ---
        if (args.Length == 0)
        {
            Console.WriteLine("Bitte geben Sie einen Dateipfad als Argument an.");
            return;
        }

        var runPath = args[0];
        Console.WriteLine($"Run Path: {runPath}");

        // --- Existenzprüfung Run-Ordner ---
        if (!Directory.Exists(runPath))
        {
            Console.WriteLine($"Der angegebene Run Path existiert nicht: {runPath}");
            return;
        }

        // --- Rider-Ordner bestimmen ---
        var riderPath = Path.GetFullPath(Path.Combine(runPath, ".."));
        Console.WriteLine($"Rider Path: {riderPath}");

        if (!Directory.Exists(riderPath))
        {
            Console.WriteLine($"Rider Path existiert nicht: {riderPath}");
            return;
        }

        // --- .HED-File suchen und ggf. kopieren ---
        var hedFile = Directory.EnumerateFiles(riderPath, "*.HED", SearchOption.TopDirectoryOnly).FirstOrDefault();
        string fileName = null;
        string hedFileToRead = null;
        var hedValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (hedFile != null)
        {
            fileName = Path.GetFileName(hedFile);
            Console.WriteLine($".HED-File existiert in Riderordner: {fileName}");

            var destFile = Path.Combine(runPath, fileName);
            if (File.Exists(destFile))
            {
                Console.WriteLine($"Datei {fileName} ist bereits im Runordner vorhanden.");
                hedFileToRead = destFile;
            }
            else
            {
                try
                {
                    File.Copy(hedFile, destFile);
                    Console.WriteLine($"Datei {fileName} wurde in den Runordner kopiert.");
                    hedFileToRead = destFile;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Kopieren der Datei: {ex.Message}");
                }
            }
        }
        else
        {
            // Falls kein .HED im Rider-Ordner, suche im Run-Ordner
            hedFile = Directory.EnumerateFiles(runPath, "*.HED", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (hedFile != null)
            {
                fileName = Path.GetFileName(hedFile);
                Console.WriteLine($".HED-File existiert im Runordner: {fileName}");
                hedFileToRead = hedFile;
            }
        }

        // --- .HED File einlesen und Werte übernehmen ---
        if (hedFileToRead != null)
        {
            foreach (var line in File.ReadLines(hedFileToRead))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("[")) continue;
                var idx = trimmed.IndexOf('=');
                if (idx <= 0 || idx == trimmed.Length - 1) continue;
                var key = trimmed.Substring(0, idx).Trim();
                var value = trimmed.Substring(idx + 1).Trim();
                if (!string.IsNullOrEmpty(key))
                    hedValues[key] = value;
            }

            // Hole die Werte aus dem Dictionary, falls vorhanden, sonst nutze die bisherigen Variablen
            bikeName = hedValues.TryGetValue("bikeName", out var bikeNameVal) ? bikeNameVal : bikeName;
            trackName = hedValues.TryGetValue("trackName", out var trackNameVal) ? trackNameVal : trackName;
            trackLength = hedValues.TryGetValue("trackLength", out var trackLengthVal) ? trackLengthVal : trackLength;
            riderName = hedValues.TryGetValue("riderName", out var riderNameVal) ? riderNameVal : riderName;
        }
        else
        {
            // --- Kein .HED-File gefunden: Extrahiere Werte aus Pfad und Datenbank ---
            Console.WriteLine("Kein .HED-File im Rider- oder Runordner gefunden.");
            Console.WriteLine("Lade KOMPASS...");

            string _databaseDirectory = @"C:\KTM Tools\KTM Offroad Racing Shared Resources\00_SoftwareDB";
            string _databaseFile = Path.Combine(_databaseDirectory, "KOMPASS.json");

            if (!Directory.Exists(_databaseDirectory))
            {
                Console.WriteLine($"Datenbankverzeichnis nicht gefunden: {_databaseDirectory}");
                return;
            }

            if (!File.Exists(_databaseFile))
            {
                Console.WriteLine($"Datenbankdatei nicht gefunden: {_databaseFile}");
                return;
            }

            Program prog = new Program();
            try
            {
                using FileStream fs = File.OpenRead(_databaseFile);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                };

                using var doc = await JsonDocument.ParseAsync(fs);
                var root = doc.RootElement;
                prog._disciplines.Clear();

                foreach (var disciplineProp in root.EnumerateObject())
                {
                    var key = disciplineProp.Name;
                    try
                    {
                        var data = disciplineProp.Value.Deserialize<DisciplineData>(options) ?? new DisciplineData();
                        prog._disciplines[key] = data;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warnung: Disziplin '{key}' konnte nicht geladen werden: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Datenbank: {ex.Message}");
                return;
            }

            if (prog._disciplines.Count == 0)
            {
                Console.WriteLine("Keine Disziplinen in der Datenbank gefunden oder alle Disziplinen fehlerhaft.");
                return;
            }

            var pathParts = runPath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine("Pfadteile:");
            for (int i = 0; i < pathParts.Length; i++)
                Console.WriteLine($"[{i}]: {pathParts[i]}");

            string teamNameAbbreviation = GetPathPart(pathParts, 10, "UnknownTeam");
            string teamName = teamNameAbbreviation switch
            {
                "01_KTM-FR" => "KTM Factory Racing",
                "02_HQV" => "Husqvarna Factory Racing",
                "03_KTM-DC" => "KTM De Carli Racing",
                _ => teamNameAbbreviation
            };
            string seasonName = GetPathPart(pathParts, 8, "UnknownSeason");
            string disciplineName = GetPathPart(pathParts, 7, "UnknownDiscipline");
            string eventName = GetPathPart(pathParts, 4, "UnknownEvent");
            string sessionName = GetPathPart(pathParts, 3, "UnknownSession");
            bikeName = GetPathPart(pathParts, 2, "UnknownBike");
            string run = GetPathPart(pathParts, 1, "UnknownRun");

            string riderabbreviationName = bikeName.Length > 2 ? bikeName[..^2] : "";
            string bikeNumber = bikeName.Length >= 2 ? bikeName[^2..] : bikeName;

            string type = eventName.Length >= 2 ? eventName[..2] : "";
            string calendarweek = eventName.Length >= 4 ? eventName.Substring(2, 2) : "";
            string typeName = type switch
            {
                "RT" => "Racetrack",
                "TT" => "Testtrack",
                _ => "Unknown Type"
            };

            int underscoreIndex = eventName.IndexOf('_');
            int dashIndex = eventName.IndexOf('-');
            string countrycode = (underscoreIndex != -1 && dashIndex > underscoreIndex)
                ? eventName.Substring(underscoreIndex + 1, dashIndex - underscoreIndex - 1)
                : "";
            trackName = (dashIndex != -1 && dashIndex + 1 < eventName.Length)
                ? eventName[(dashIndex + 1)..]
                : "";

            var disciplineKey = prog._disciplines.ContainsKey(disciplineName) ? disciplineName : "MX";
            var disciplineData = prog._disciplines.GetValueOrDefault(disciplineKey);

            TrackInfo foundTrack = null;
            if (disciplineData != null)
            {
                foundTrack = disciplineData.Tracks
                    .FirstOrDefault(t =>
                        string.Equals(t.Countrycode, countrycode, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(t.Track, trackName, StringComparison.OrdinalIgnoreCase));
            }
            trackLength = foundTrack?.trackLength ?? "";
            string country = foundTrack?.Country ?? "";

            RiderInfo foundRider = null;
            if (disciplineData != null)
            {
                foundRider = disciplineData.Riders
                    .FirstOrDefault(r =>
                        string.Equals(r.Riderabbreviation, riderabbreviationName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(r.Season, seasonName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(r.Team, teamName, StringComparison.OrdinalIgnoreCase));
            }
            riderName = foundRider?.Rider ?? "";

            // Debug-Ausgabe der extrahierten Werte
            Console.WriteLine($"discipline: {disciplineName}");
            Console.WriteLine($"season: {seasonName}");
            Console.WriteLine($"team: {teamName}");
            Console.WriteLine($"riderName: {riderName}");
            Console.WriteLine($"riderabbreviation: {riderabbreviationName}");
            Console.WriteLine($"bikenumber: {bikeNumber}");
            Console.WriteLine($"bike: {bikeName}");
            Console.WriteLine($"type: {typeName}");
            Console.WriteLine($"countrycode: {countrycode}");
            Console.WriteLine($"track: {trackName}");
            Console.WriteLine($"trackLength: {trackLength}");
            Console.WriteLine($"calendarweek: {calendarweek}");
            Console.WriteLine($"event: {eventName}");
            Console.WriteLine($"session: {sessionName}");
            Console.WriteLine($"country: {country}");

            // .HED-File im gewünschten Format erzeugen
            var hedFileContent =
$@"[KTM_NAMING]
disciplineName = {disciplineName}
seasonName = {seasonName}
calendarweek = {calendarweek}
teamName = {teamName}
riderName = {riderName}
riderabbreviationName = {riderabbreviationName}
bikeNumber = {bikeNumber}
bikeName = {bikeName}
typeName = {typeName}
countrycode = {countrycode}
countryName = {country}
trackName = {trackName}
trackLength = {trackLength}
eventName = {eventName}
sessionName = {sessionName}
runNumber = {run}";

            string hedFileName = $"{seasonName} {eventName} Specsheet {bikeName} {sessionName}.HED";
            var newHedFile = Path.Combine(runPath, hedFileName);
            File.WriteAllText(newHedFile, hedFileContent);
            Console.WriteLine($"Neues .HED-File erstellt: {newHedFile}");
        }

        // --- XML-Bearbeitung: Nur einmal am Ende ---
        // 1. Finde alle Runs
        var (validRuns, invalidRuns) = FindCableDataInFolders(runPath);

        // 2. XML-Dateien bearbeiten mit XmlRunEditor
        // Alias, Track, trackLength, Rider werden aus den extrahierten oder HED-Werten übergeben
        var xmlEditor = new XmlRunEditor();
        xmlEditor.EditXMLFiles(validRuns.ToArray(), bikeName, trackName, trackLength, riderName);
        // --- Ende XML-Bearbeitung ---

        // --- Berechnungspipeline ausführen ---
        var importCalculator = new ImportDataCalculator();
        importCalculator.RunCalculationPipeline(runPath);
    }

    // Hilfsmethode für Pfadextraktion
    private static string GetPathPart(string[] parts, int fromEnd, string fallback)
    {
        return parts.Length >= fromEnd ? parts[^fromEnd] : fallback;
    }

    // --- Hilfsmodelle für die JSON-Datenbank ---
    public class DatabaseRoot
    {
        [JsonExtensionData]
        public Dictionary<string, JsonElement> Disciplines { get; set; } = new();
    }

    public class DisciplineData
    {
        public List<TrackInfo> Tracks { get; set; } = new();
        public List<RiderInfo> Riders { get; set; } = new();
    }

    public class TrackInfo
    {
        public string? Country { get; set; }
        public string? Countrycode { get; set; }
        public string? Track { get; set; }
        public string? trackLength { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }

    public class RiderInfo
    {
        public string? Rider { get; set; }
        public string? Riderabbreviation { get; set; }
        public string? Season { get; set; }
        public string? Team { get; set; }
    }

    // === XML Edit Functions ===
    // Diese Methoden sind jetzt in XmlRunEditor ausgelagert und werden dort verwendet.

    public static (List<string> validRuns, List<string> invalidRuns) FindCableDataInFolders(string basePath)
    {
        if (string.IsNullOrWhiteSpace(basePath) || !Directory.Exists(basePath))
        {
            throw new ArgumentException("Invalid input. Please provide a valid base path string.");
        }

        // Get all folders recursively
        var folderList = Directory.GetDirectories(basePath, "*", SearchOption.AllDirectories).ToList();
        folderList.Add(basePath); // Include basePath itself

        var runParentMap = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        foreach (var folder in folderList)
        {
            var cableDataFile = Path.Combine(folder, "cableData.ztx");
            var runFolder = ExtractRunFolder(folder);
            if (!string.IsNullOrEmpty(runFolder))
            {
                if (File.Exists(cableDataFile))
                {
                    runParentMap[runFolder] = true;
                }
                else if (!runParentMap.ContainsKey(runFolder))
                {
                    runParentMap[runFolder] = false;
                }
            }
        }

        var validRuns = new List<string>();
        var invalidRuns = new List<string>();
        foreach (var kvp in runParentMap)
        {
            if (kvp.Value)
                validRuns.Add(kvp.Key);
            else
                invalidRuns.Add(kvp.Key);
        }
        return (validRuns, invalidRuns);
    }

    // Helper: Extracts the parent "Run_xxx" folder from a given path
    public static string ExtractRunFolder(string folder)
    {
        var parts = folder.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        foreach (var part in parts.Reverse())
        {
            if (part.StartsWith("Run_", StringComparison.OrdinalIgnoreCase))
                return Path.Combine(string.Join(Path.DirectorySeparatorChar.ToString(), parts.TakeWhile(p => p != part).Concat(new[] { part })));
        }
        return string.Empty;
    }
}
