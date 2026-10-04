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
                _database = await _repository.LoadAsync(databaseFile).ConfigureAwait(false);
                _lookup = new KompassLookup(_database);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Datenbank: {ex.Message}");
                return false;
            }

            if (_database.DisciplineKeys.Count == 0)
            {
                Console.WriteLine("Keine Disziplinen in der Datenbank gefunden oder alle Disziplinen fehlerhaft.");
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

            // Motorklasse (250/450) robust als eigenes Pfadsegment suchen (index-unabhängig).
            metadata.EngineClass = ExtractEngineClass(pathParts);

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

        /// <summary>
        /// Sucht die Ziellinien-Koordinate (Finish-Punkt) eines Tracks über alle Disziplinen hinweg
        /// anhand des Tracknamens. Gibt <c>null</c> zurück, wenn der Track fehlt oder keine gültigen
        /// Lat/Lon-Werte hat (in KOMPASS häufig leer). Erfordert vorheriges <see cref="LoadAsync"/>.
        /// </summary>
        public GeoCoordinate? FindFinishLineByTrack(string trackName)
        {
            if (string.IsNullOrWhiteSpace(trackName))
                return null;

            foreach (var discipline in _database.DisciplineKeys)
                foreach (var track in _database.Tracks(discipline))
                    if (string.Equals(track.Name, trackName, StringComparison.OrdinalIgnoreCase)
                        && track.Coordinate is { } coordinate)
                        return coordinate;

            return null;
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
            var track = _lookup.FindTrack(metadata.DisciplineName, metadata.CountryCode, metadata.TrackName);
            metadata.TrackLength = track?.TrackLength ?? "";
            metadata.CountryName = track?.Country ?? "";
            metadata.Surface = track?.Surface ?? "";

            var rider = _lookup.FindRider(metadata.DisciplineName, metadata.RiderAbbreviation, metadata.SeasonName, metadata.TeamName);
            metadata.RiderName = rider?.Name ?? "";
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

        /// <summary>Bekannte Motorklassen (Hubraum) als Pfadsegment.</summary>
        private static readonly string[] KnownEngineClasses = { "125", "150", "250", "300", "350", "450", "500" };

        /// <summary>Sucht die Motorklasse (z.B. "250"/"450") als eigenes Pfadsegment; leer wenn nicht gefunden.</summary>
        private static string ExtractEngineClass(string[] parts)
            => parts.FirstOrDefault(p => KnownEngineClasses.Contains(p)) ?? "";

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
}
