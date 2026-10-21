using KtmDataHandler.Marelli.Wintax;
using Ktm.Racing.Domain.Kompass;
using Ktm.Racing.Domain.Geo;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Service für den Zugriff auf die KOMPASS-Datenbank (JSON) über die geteilte
    /// Ktm.Racing.Domain-Bibliothek. Löst Track- und Rider-Metadaten für die Telemetrie auf.
    /// </summary>
    public class KompassService
    {
        private readonly IKompassRepository _repository = new KompassRepository();
        private KompassDatabase _database = new();
        private IKompassLookup _lookup = new KompassLookup(new KompassDatabase());

        /// <summary><c>true</c>, wenn die letzte <see cref="LoadAsync"/> an einer vorhandenen, aber ungültigen Datei scheiterte.</summary>
        public bool LastLoadInvalid { get; private set; }

        /// <summary>
        /// Lädt die KOMPASS-Datenbank aus der angegebenen JSON-Datei.
        /// </summary>
        /// <param name="databaseFile">Vollständiger Pfad zur KOMPASS.json</param>
        /// <returns>True bei Erfolg, false bei Fehler oder leerer Datenbank</returns>
        public async Task<bool> LoadAsync(string databaseFile)
        {
            LastLoadInvalid = false;
            if (!File.Exists(databaseFile))
            {
                Console.WriteLine($"Datenbankdatei nicht gefunden: {databaseFile}");
                return false;
            }

            try
            {
                _database = await _repository.LoadAsync(databaseFile).ConfigureAwait(false);
                _lookup = new KompassLookup(_database);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Datenbank: {ex.Message}");
                LastLoadInvalid = ex is not IOException;
                return false;
            }

            if (_database.DisciplineKeys.Count == 0)
            {
                Console.WriteLine("Keine Disziplinen in der Datenbank gefunden oder alle Disziplinen fehlerhaft.");
                LastLoadInvalid = true;
                return false;
            }

            Console.WriteLine($"KOMPASS geladen: {_database.DisciplineKeys.Count} Disziplin(en).");
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
            metadata.BikeName = NormalizeBikeName(ztxBrowse.GetDeviceName(runPath) ?? "");
            metadata.RunNumber = ztxBrowse.GetRunName(runPath) ?? "";

            CompleteFromPathAndKompass(metadata, runPath, teamMappings);
            return metadata;
        }

        /// <summary>
        /// Ergänzt Metadaten, deren Event-/Session-/Bike-Name bereits gesetzt sind, aus dem Pfad (Team, Saison,
        /// Disziplin, Motorklasse – per Muster, nicht per Position) und aus KOMPASS (Track, Rider).
        /// Ohne native Abhängigkeit (testbar).
        /// </summary>
        public void CompleteFromPathAndKompass(RunMetadata metadata, string runPath, IReadOnlyDictionary<string, string> teamMappings)
        {
            var path = PathMetadataParser.Parse(runPath, metadata.EventName, teamMappings, _database.DisciplineKeys);
            metadata.TeamName = path.TeamName;
            metadata.SeasonName = path.SeasonName;
            metadata.EngineClass = path.EngineClass;

            // --- Bike-/Rider-Abkürzung aus BikeName ableiten ---
            // Format: "JP01" → RiderAbbreviation="JP", BikeNumber="01"
            metadata.RiderAbbreviation = metadata.BikeName.Length > 2
                ? metadata.BikeName[..^2] : "";
            metadata.BikeNumber = metadata.BikeName.Length >= 2
                ? metadata.BikeName[^2..] : metadata.BikeName;

            // --- Event-Name parsen ---
            // Format: "RT01_AT-RedBull" → Type=Racetrack, CW=01, CC=AT, Track=RedBull
            ParseEventName(metadata);

            // Disziplin: aus dem Pfad, sonst die Disziplin, die den Track enthält (der Pfad enthält meist keine).
            metadata.DisciplineName = !string.IsNullOrEmpty(path.DisciplineName)
                ? path.DisciplineName
                : FindDisciplineOfTrack(metadata.CountryCode, metadata.TrackName);

            if (string.IsNullOrEmpty(metadata.TeamName))
                Console.WriteLine("Warnung: Kein Team-Ordner aus TeamMappings im Pfad gefunden – Rider wird ohne Team gesucht.");
            if (string.IsNullOrEmpty(metadata.SeasonName))
                Console.WriteLine("Warnung: Keine Saison (vierstelliges Jahr) im Pfad gefunden.");

            // --- Track und Rider aus KOMPASS-Datenbank nachschlagen ---
            LookupTrackAndRider(metadata);

            // Debug-Ausgabe der extrahierten Werte
            LogMetadata(metadata);
        }

        /// <summary>
        /// Sucht die Ziellinien-Koordinate (Track-Punkt) eines Tracks über alle Disziplinen hinweg. Der Name wird
        /// schreibweisen-tolerant verglichen (<see cref="TrackNames.Same"/>: Groß/Klein, Akzente, Satzzeichen); bei
        /// mehreren Treffern gewinnt der Track mit passendem <paramref name="countryCode"/>. Gibt <c>null</c> zurück, wenn
        /// der Track fehlt oder keine gültigen Lat/Lon-Werte hat. Erfordert vorheriges <see cref="LoadAsync"/>.
        /// </summary>
        public GeoCoordinate? FindFinishLineByTrack(string trackName, string? countryCode = null)
        {
            if (string.IsNullOrWhiteSpace(trackName))
                return null;

            var candidates = _database.DisciplineKeys
                .SelectMany(discipline => _database.Tracks(discipline))
                .Where(track => TrackNames.Same(track.Name, trackName) && track.Coordinate is not null)
                .ToList();

            var match = candidates.FirstOrDefault(track => !string.IsNullOrWhiteSpace(countryCode)
                            && string.Equals(track.CountryCode, countryCode, StringComparison.OrdinalIgnoreCase))
                        ?? candidates.FirstOrDefault();
            return match?.Coordinate;
        }

        /// <summary>
        /// Sucht den Untergrund (Surface) eines Tracks über alle Disziplinen hinweg anhand des Tracknamens.
        /// Gibt <c>null</c> zurück, wenn der Track fehlt oder keinen Untergrund gesetzt hat.
        /// Erfordert vorheriges <see cref="LoadAsync"/>.
        /// </summary>
        public string? FindTrackSurface(string trackName)
        {
            if (string.IsNullOrWhiteSpace(trackName))
                return null;

            foreach (var discipline in _database.DisciplineKeys)
                foreach (var track in _database.Tracks(discipline))
                    if (string.Equals(track.Name, trackName, StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(track.Surface))
                        return track.Surface;

            return null;
        }

        // --- Private Hilfsmethoden ---

        /// <summary>
        /// Liest Track und Ländercode ohne Seiteneffekte aus dem Event-Ordner im Pfad (z.B. <c>…\RT37_CHN-Shanghai\R1\AA01\Run_885</c>
        /// → Shanghai, CHN). <c>null</c>, wenn kein Ordner im Format <c>XXnn_CC-Track</c> im Pfad liegt.
        /// </summary>
        public static RunMetadata? ParseEventFromPath(string path)
        {
            for (var folder = Path.GetFullPath(path); !string.IsNullOrEmpty(folder); folder = Path.GetDirectoryName(folder))
            {
                var name = Path.GetFileName(Path.TrimEndingDirectorySeparator(folder));
                if (!System.Text.RegularExpressions.Regex.IsMatch(name, @"^[A-Za-z]{2}\d{2}_[^-_]+-.+$"))
                    continue;
                var metadata = new RunMetadata { EventName = name };
                ParseEventName(metadata);
                return metadata;
            }

            return null;
        }

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
        /// Sucht Track-Informationen (Länge, Land, Untergrund) und den Rider in der KOMPASS-Datenbank.
        /// Rider: exakt (Disziplin, Kürzel, Saison, Team); sonst eindeutig über Kürzel + Saison (Team dann aus KOMPASS).
        /// </summary>
        private void LookupTrackAndRider(RunMetadata metadata)
        {
            var track = _lookup.FindTrack(metadata.DisciplineName, metadata.CountryCode, metadata.TrackName);
            metadata.TrackLength = track?.TrackLength ?? "";
            metadata.CountryName = track?.Country ?? "";
            metadata.Surface = track?.Surface ?? "";
            if (track is null)
                Console.WriteLine($"Warnung: Track \"{metadata.TrackName}\" ({metadata.CountryCode}) nicht in KOMPASS ({metadata.DisciplineName}) gefunden.");

            var rider = _lookup.FindRider(metadata.DisciplineName, metadata.RiderAbbreviation, metadata.SeasonName, metadata.TeamName);
            if (rider is null && !string.IsNullOrEmpty(metadata.DisciplineName))
            {
                var candidates = _database.Riders(metadata.DisciplineName)
                    .Where(r => string.Equals(r.Abbreviation, metadata.RiderAbbreviation, StringComparison.OrdinalIgnoreCase)
                             && string.Equals(r.Season, metadata.SeasonName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                if (candidates.Count == 1)
                {
                    rider = candidates[0];
                    if (!string.Equals(metadata.TeamName, rider.Team, StringComparison.OrdinalIgnoreCase))
                        Console.WriteLine($"Warnung: Team aus dem Pfad (\"{metadata.TeamName}\") passt nicht zu KOMPASS – verwende \"{rider.Team}\" für {rider.Name}.");
                    metadata.TeamName = rider.Team;
                }
            }

            metadata.RiderName = rider?.Name ?? "";
            if (rider is null)
                Console.WriteLine($"Warnung: Rider \"{metadata.RiderAbbreviation}\" (Saison {metadata.SeasonName}, Team \"{metadata.TeamName}\") nicht in KOMPASS gefunden.");
        }

        /// <summary>
        /// Die Disziplin, die den Track enthält (Ländercode + Name); sonst die Standard-Disziplin MX bzw. die einzige vorhandene.
        /// </summary>
        private string FindDisciplineOfTrack(string countryCode, string trackName)
        {
            foreach (var discipline in _database.DisciplineKeys)
            {
                if (_database.Tracks(discipline).Any(t =>
                        string.Equals(t.CountryCode, countryCode, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(t.Name, trackName, StringComparison.OrdinalIgnoreCase)))
                    return discipline;
            }

            return _database.DisciplineKeys.Contains("MX") || _database.DisciplineKeys.Count == 0
                ? "MX"
                : _database.DisciplineKeys.First();
        }

        /// <summary>
        /// Normalisiert den Device-/Bike-Namen (z.B. "AA 01" → "AA01") und warnt bei nicht-kanonischer Form.
        /// Verhindert eine fehlerhafte RiderAbbreviation und Leerzeichen in den Name-/Alias-Properties.
        /// </summary>
        private static string NormalizeBikeName(string rawDeviceName)
        {
            if (BikeAlias.IsCanonical(rawDeviceName))
                return rawDeviceName;

            var normalized = BikeAlias.Normalize(rawDeviceName);
            Console.WriteLine($"Warnung: Device-/Bike-Name \"{rawDeviceName}\" enthält Leerzeichen — normalisiere zu \"{normalized}\".");
            return normalized;
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
}
