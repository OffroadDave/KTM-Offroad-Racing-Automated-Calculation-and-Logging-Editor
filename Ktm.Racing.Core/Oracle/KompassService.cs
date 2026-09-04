using System.Text.Json;
using System.Text.Json.Serialization;
using KtmDataHandler.Marelli.Wintax;

namespace Ktm.Racing.Core
{
    /// <summary>
    /// Service für den Zugriff auf die KOMPASS-Datenbank (JSON).
    /// Enthält Disziplinen mit zugehörigen Track- und Rider-Informationen.
    /// 
    /// Die KOMPASS-Datenbank hat folgende Struktur:
    /// {
    ///   "MX": { "Tracks": [...], "Riders": [...] },
    ///   "EN": { "Tracks": [...], "Riders": [...] }
    /// }
    /// </summary>
    public class KompassService
    {
        private readonly Dictionary<string, DisciplineData> _disciplines
            = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Lädt die KOMPASS-Datenbank aus der angegebenen JSON-Datei.
        /// </summary>
        /// <param name="databaseFile">Vollständiger Pfad zur KOMPASS.json</param>
        /// <returns>True bei Erfolg, false bei Fehler oder leerer Datenbank</returns>
        public async Task<bool> LoadAsync(string databaseFile)
        {
            if (!File.Exists(databaseFile))
            {
                Console.WriteLine($"Datenbankdatei nicht gefunden: {databaseFile}");
                return false;
            }

            try
            {
                using var fs = File.OpenRead(databaseFile);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                };

                using var doc = await JsonDocument.ParseAsync(fs);
                _disciplines.Clear();

                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    try
                    {
                        var data = prop.Value.Deserialize<DisciplineData>(options) ?? new DisciplineData();
                        _disciplines[prop.Name] = data;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warnung: Disziplin '{prop.Name}' konnte nicht geladen werden: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Datenbank: {ex.Message}");
                return false;
            }

            if (_disciplines.Count == 0)
            {
                Console.WriteLine("Keine Disziplinen in der Datenbank gefunden oder alle Disziplinen fehlerhaft.");
                return false;
            }

            Console.WriteLine($"KOMPASS geladen: {_disciplines.Count} Disziplin(en).");
            return true;
        }

        /// <summary>
        /// Extrahiert vollständige Metadaten aus Pfad, IMarelliDataBrowse API und KOMPASS-Datenbank.
        /// 
        /// Verwendet IMarelliDataBrowse für: EventName, SessionName, DeviceName (BikeName), RunName.
        /// Verwendet Pfad-Parsing für KTM-spezifische Teile: Team, Season, Discipline.
        /// Verwendet KOMPASS für: Track-Länge, Land, Rider-Name.
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner</param>
        /// <param name="ztxBrowse">IMarelliDataBrowse-Instanz für standardisiertes Pfad-Parsing</param>
        /// <param name="teamMappings">Mapping von Team-Abkürzungen zu vollständigen Namen</param>
        /// <returns>Vollständig befülltes RunMetadata-Objekt</returns>
        public RunMetadata BuildMetadata(string runPath, IMarelliDataBrowse ztxBrowse,
            Dictionary<string, string> teamMappings)
        {
            var metadata = new RunMetadata();

            // --- Pfad-Metadaten über IMarelliDataBrowse API extrahieren ---
            // Die API parst die WinTAX4-Ordnerstruktur: Event/Session/Device/Run_xxx
            metadata.EventName = ztxBrowse.GetEventName(runPath) ?? "";
            metadata.SessionName = ztxBrowse.GetSessionName(runPath) ?? "";
            metadata.BikeName = ztxBrowse.GetDeviceName(runPath) ?? "";
            metadata.RunNumber = ztxBrowse.GetRunName(runPath) ?? "";

            // --- KTM-spezifische Pfadteile extrahieren ---
            // Team, Season und Discipline sind KTM-eigene Ordnerebenen oberhalb
            // der Standard-WinTAX4-Struktur und werden daher per Index extrahiert.
            var pathParts = runPath.Split(
                [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
                StringSplitOptions.RemoveEmptyEntries);

            LogPathParts(pathParts);

            // Team-Name über konfigurierbare Mappings auflösen
            string teamAbbreviation = GetPathPart(pathParts, 10, "UnknownTeam");
            metadata.TeamName = teamMappings.GetValueOrDefault(teamAbbreviation, teamAbbreviation);

            metadata.SeasonName = GetPathPart(pathParts, 8, "UnknownSeason");
            metadata.DisciplineName = GetPathPart(pathParts, 7, "UnknownDiscipline");

            // --- Bike-/Rider-Abkürzung aus BikeName ableiten ---
            // Format: "JP01" → RiderAbbreviation="JP", BikeNumber="01"
            metadata.RiderAbbreviation = metadata.BikeName.Length > 2
                ? metadata.BikeName[..^2] : "";
            metadata.BikeNumber = metadata.BikeName.Length >= 2
                ? metadata.BikeName[^2..] : metadata.BikeName;

            // --- Event-Name parsen ---
            // Format: "RT01_AT-RedBull" → Type=Racetrack, CW=01, CC=AT, Track=RedBull
            ParseEventName(metadata);

            // --- Track und Rider aus KOMPASS-Datenbank nachschlagen ---
            LookupTrackAndRider(metadata);

            // Debug-Ausgabe der extrahierten Werte
            LogMetadata(metadata);

            return metadata;
        }

        // --- Private Hilfsmethoden ---

        /// <summary>
        /// Parst den EventName und extrahiert Typ, Kalenderwoche, Ländercode und Track.
        /// Format: "RT01_AT-RedBull" → Type=Racetrack, CW=01, CC=AT, Track=RedBull
        /// </summary>
        private static void ParseEventName(RunMetadata metadata)
        {
            var eventName = metadata.EventName;

            // Typ-Code (erste 2 Zeichen): "RT" = Racetrack, "TT" = Testtrack
            string typeCode = eventName.Length >= 2 ? eventName[..2] : "";
            metadata.TypeName = typeCode switch
            {
                "RT" => "Racetrack",
                "TT" => "Testtrack",
                _ => "Unknown Type"
            };

            // Kalenderwoche (Zeichen 3-4)
            metadata.CalendarWeek = eventName.Length >= 4 ? eventName.Substring(2, 2) : "";

            // Ländercode und Track-Name aus "XX_CC-Track" Format extrahieren
            int underscoreIdx = eventName.IndexOf('_');
            int dashIdx = eventName.IndexOf('-');
            metadata.CountryCode = (underscoreIdx != -1 && dashIdx > underscoreIdx)
                ? eventName.Substring(underscoreIdx + 1, dashIdx - underscoreIdx - 1)
                : "";
            metadata.TrackName = (dashIdx != -1 && dashIdx + 1 < eventName.Length)
                ? eventName[(dashIdx + 1)..]
                : "";
        }

        /// <summary>
        /// Sucht Track-Informationen (Länge, Land) und Rider-Name in der KOMPASS-Datenbank.
        /// Fallback-Disziplin: "MX" wenn die extrahierte Disziplin nicht gefunden wird.
        /// </summary>
        private void LookupTrackAndRider(RunMetadata metadata)
        {
            // Disziplin auflösen (Fallback auf "MX")
            var disciplineKey = _disciplines.ContainsKey(metadata.DisciplineName)
                ? metadata.DisciplineName : "MX";
            var discipline = _disciplines.GetValueOrDefault(disciplineKey);

            if (discipline == null)
                return;

            // Track nachschlagen über Ländercode + Track-Name
            var track = discipline.Tracks.FirstOrDefault(t =>
                string.Equals(t.Countrycode, metadata.CountryCode, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(t.Track, metadata.TrackName, StringComparison.OrdinalIgnoreCase));

            metadata.TrackLength = track?.TrackLength ?? "";
            metadata.CountryName = track?.Country ?? "";

            // Rider nachschlagen über Abkürzung + Season + Team
            var rider = discipline.Riders.FirstOrDefault(r =>
                string.Equals(r.Riderabbreviation, metadata.RiderAbbreviation, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(r.Season, metadata.SeasonName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(r.Team, metadata.TeamName, StringComparison.OrdinalIgnoreCase));

            metadata.RiderName = rider?.Rider ?? "";
        }

        /// <summary>
        /// Extrahiert einen Pfadteil relativ zum Ende des Pfad-Arrays.
        /// </summary>
        /// <param name="parts">Zerlegter Pfad</param>
        /// <param name="fromEnd">Position vom Ende (1 = letztes Element)</param>
        /// <param name="fallback">Fallback-Wert wenn Index außerhalb des Arrays</param>
        private static string GetPathPart(string[] parts, int fromEnd, string fallback)
        {
            return parts.Length >= fromEnd ? parts[^fromEnd] : fallback;
        }

        private static void LogPathParts(string[] pathParts)
        {
            Console.WriteLine("Pfadteile:");
            for (int i = 0; i < pathParts.Length; i++)
                Console.WriteLine($"[{i}]: {pathParts[i]}");
        }

        private static void LogMetadata(RunMetadata m)
        {
            Console.WriteLine($"discipline: {m.DisciplineName}");
            Console.WriteLine($"season: {m.SeasonName}");
            Console.WriteLine($"team: {m.TeamName}");
            Console.WriteLine($"riderName: {m.RiderName}");
            Console.WriteLine($"riderabbreviation: {m.RiderAbbreviation}");
            Console.WriteLine($"bikenumber: {m.BikeNumber}");
            Console.WriteLine($"bike: {m.BikeName}");
            Console.WriteLine($"type: {m.TypeName}");
            Console.WriteLine($"countrycode: {m.CountryCode}");
            Console.WriteLine($"track: {m.TrackName}");
            Console.WriteLine($"trackLength: {m.TrackLength}");
            Console.WriteLine($"calendarweek: {m.CalendarWeek}");
            Console.WriteLine($"event: {m.EventName}");
            Console.WriteLine($"session: {m.SessionName}");
            Console.WriteLine($"country: {m.CountryName}");
        }
    }

    // --- Datenmodelle für die KOMPASS-Datenbank ---

    /// <summary>
    /// Disziplin mit zugehörigen Tracks und Riders.
    /// Entspricht einem Top-Level-Eintrag in KOMPASS.json (z.B. "MX", "EN").
    /// </summary>
    public class DisciplineData
    {
        public List<TrackInfo> Tracks { get; set; } = [];
        public List<RiderInfo> Riders { get; set; } = [];
    }

    /// <summary>
    /// Track-Informationen aus der KOMPASS-Datenbank.
    /// </summary>
    public class TrackInfo
    {
        public string? Country { get; set; }
        public string? Countrycode { get; set; }
        public string? Track { get; set; }

        [JsonPropertyName("Tracklength")]
        public string? TrackLength { get; set; }

        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }

    /// <summary>
    /// Rider-Informationen aus der KOMPASS-Datenbank.
    /// </summary>
    public class RiderInfo
    {
        public string? Rider { get; set; }
        public string? Riderabbreviation { get; set; }
        public string? Season { get; set; }
        public string? Team { get; set; }
    }
}
