using KtmDataHandler.Marelli.Wintax;
using Ktm.Racing.Domain.Kompass;
using Ktm.Racing.Domain.Geo;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Service for accessing the KOMPASS database (JSON) via the shared
    /// Ktm.Racing.Domain library. Resolves track and rider metadata for the telemetry.
    /// </summary>
    public class KompassService
    {
        private readonly IKompassRepository _repository = new KompassRepository();
        private KompassDatabase _database = new();
        private IKompassLookup _lookup = new KompassLookup(new KompassDatabase());

        /// <summary><c>true</c> if the last <see cref="LoadAsync"/> failed on an existing but invalid file.</summary>
        public bool LastLoadInvalid { get; private set; }

        /// <summary>
        /// Loads the KOMPASS database from the specified JSON file.
        /// </summary>
        /// <param name="databaseFile">Full path to KOMPASS.json</param>
        /// <returns>True on success, false on error or empty database</returns>
        public async Task<bool> LoadAsync(string databaseFile)
        {
            LastLoadInvalid = false;
            if (!File.Exists(databaseFile))
            {
                Console.WriteLine($"Database file not found: {databaseFile}");
                return false;
            }

            // KODE may currently be writing the file: re-read a half-written JSON after a short pause
            // before treating it as invalid (exit 7, no retry by KRAKEN).
            Exception? lastError = null;
            for (var attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    _database = await _repository.LoadAsync(databaseFile).ConfigureAwait(false);
                    _lookup = new KompassLookup(_database);
                    lastError = null;
                    break;
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    if (attempt < 2)
                        await Task.Delay(500 * (attempt + 1)).ConfigureAwait(false);
                }
            }

            if (lastError is not null)
            {
                Console.WriteLine($"Error loading the database: {lastError.Message}");
                LastLoadInvalid = lastError is not (IOException or UnauthorizedAccessException);
                return false;
            }

            if (_database.DisciplineKeys.Count == 0)
            {
                Console.WriteLine("No disciplines found in the database or all disciplines are invalid.");
                LastLoadInvalid = true;
                return false;
            }

            Console.WriteLine($"KOMPASS loaded: {_database.DisciplineKeys.Count} discipline(s).");
            return true;
        }

        /// <summary>
        /// Extracts complete metadata from the path, the IMarelliDataBrowse API and the KOMPASS database.
        /// 
        /// Uses IMarelliDataBrowse for: EventName, SessionName, DeviceName (BikeName), RunName.
        /// Uses path parsing for KTM-specific parts: team, season, discipline.
        /// Uses KOMPASS for: track length, country, rider name.
        /// </summary>
        /// <param name="runPath">Path to the run folder</param>
        /// <param name="ztxBrowse">IMarelliDataBrowse instance for standardized path parsing</param>
        /// <param name="teamMappings">Mapping of team abbreviations to full names</param>
        /// <returns>Fully populated RunMetadata object</returns>
        public RunMetadata BuildMetadata(string runPath, IMarelliDataBrowse ztxBrowse,
            Dictionary<string, string> teamMappings)
        {
            var metadata = new RunMetadata();

            // --- Extract path metadata via the IMarelliDataBrowse API ---
            // The API parses the WinTAX4 folder structure: Event/Session/Device/Run_xxx
            metadata.EventName = ztxBrowse.GetEventName(runPath) ?? "";
            metadata.SessionName = ztxBrowse.GetSessionName(runPath) ?? "";
            metadata.BikeName = NormalizeBikeName(ztxBrowse.GetDeviceName(runPath) ?? "");
            metadata.RunNumber = ztxBrowse.GetRunName(runPath) ?? "";

            CompleteFromPathAndKompass(metadata, runPath, teamMappings);
            return metadata;
        }

        /// <summary>
        /// Completes metadata whose event/session/bike name is already set, from the path (team, season,
        /// discipline, engine class – by pattern, not by position) and from KOMPASS (track, rider).
        /// Without native dependency (testable).
        /// </summary>
        public void CompleteFromPathAndKompass(RunMetadata metadata, string runPath, IReadOnlyDictionary<string, string> teamMappings)
        {
            var path = PathMetadataParser.Parse(runPath, metadata.EventName, teamMappings, _database.DisciplineKeys);
            metadata.TeamName = path.TeamName;
            metadata.SeasonName = path.SeasonName;
            metadata.EngineClass = path.EngineClass;

            // --- Derive bike/rider abbreviation from BikeName ---
            // Format: letters + digits, "JP01" → RiderAbbreviation="JP", BikeNumber="01"; "AA101" → "AA" / "101".
            (metadata.RiderAbbreviation, metadata.BikeNumber) = SplitBikeName(metadata.BikeName);

            // --- Parse event name ---
            // Format: "RT01_AT-RedBull" → Type=Racetrack, CW=01, CC=AT, Track=RedBull
            ParseEventName(metadata);

            // Discipline: from the path, otherwise the discipline that contains the track (the path usually contains none).
            metadata.DisciplineName = !string.IsNullOrEmpty(path.DisciplineName)
                ? path.DisciplineName
                : FindDisciplineOfTrack(metadata.CountryCode, metadata.TrackName);

            if (string.IsNullOrEmpty(metadata.TeamName))
                Console.WriteLine("Warning: No team folder from TeamMappings found in the path – rider is searched without team.");
            if (string.IsNullOrEmpty(metadata.SeasonName))
                Console.WriteLine("Warning: No season (four-digit year) found in the path.");

            // --- Look up track and rider in the KOMPASS database ---
            LookupTrackAndRider(metadata);

            // Debug output of the extracted values
            LogMetadata(metadata);
        }

        /// <summary>
        /// Finds the finish line coordinate (track point) of a track across all disciplines. The name is
        /// compared spelling-tolerantly (<see cref="TrackNames.Same"/>: case, accents, punctuation); with
        /// multiple matches, the track with matching <paramref name="countryCode"/> wins. Returns <c>null</c> if
        /// the track is missing or has no valid lat/lon values. Requires a prior <see cref="LoadAsync"/>.
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
        /// Finds the surface of a track across all disciplines by track name.
        /// Returns <c>null</c> if the track is missing or has no surface set.
        /// Requires a prior <see cref="LoadAsync"/>.
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

        // --- Private helper methods ---

        /// <summary>
        /// Splits a bike name into rider abbreviation and bike number: leading letters + following digits
        /// ("AA01" → "AA"/"01", "AA101" → "AA"/"101", "AA1" → "AA"/"1"). Other formats fall back to the previous rule
        /// (last two characters = number).
        /// </summary>
        public static (string Abbreviation, string Number) SplitBikeName(string bikeName)
        {
            var match = System.Text.RegularExpressions.Regex.Match(bikeName ?? string.Empty, @"^([A-Za-z]+)(\d+)$");
            if (match.Success)
                return (match.Groups[1].Value, match.Groups[2].Value);
            if (string.IsNullOrEmpty(bikeName))
                return (string.Empty, string.Empty);
            return bikeName.Length > 2 ? (bikeName[..^2], bikeName[^2..]) : (string.Empty, bikeName);
        }

        /// <summary>
        /// Reads track and country code without side effects from the event folder in the path (e.g. <c>…\RT37_CHN-Shanghai\R1\AA01\Run_885</c>
        /// → Shanghai, CHN). <c>null</c> if there is no folder in the format <c>XXnn_CC-Track</c> in the path.
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
        /// Parses the EventName and extracts type, calendar week, country code and track.
        /// Format: "RT01_AT-RedBull" → Type=Racetrack, CW=01, CC=AT, Track=RedBull
        /// </summary>
        private static void ParseEventName(RunMetadata metadata)
        {
            var eventName = metadata.EventName;

            // Type code (first 2 characters): "RT" = Racetrack, "TT" = Testtrack
            string typeCode = eventName.Length >= 2 ? eventName[..2] : "";
            metadata.TypeName = typeCode switch
            {
                "RT" => "Racetrack",
                "TT" => "Testtrack",
                _ => "Unknown Type"
            };

            // Calendar week (characters 3-4)
            metadata.CalendarWeek = eventName.Length >= 4 ? eventName.Substring(2, 2) : "";

            // Extract country code and track name from the "XX_CC-Track" format
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
        /// Looks up track information (length, country, surface) and the rider in the KOMPASS database.
        /// Rider: exact (discipline, abbreviation, season, team); otherwise unambiguously via abbreviation + season (team then from KOMPASS).
        /// </summary>
        private void LookupTrackAndRider(RunMetadata metadata)
        {
            var track = _lookup.FindTrack(metadata.DisciplineName, metadata.CountryCode, metadata.TrackName);
            metadata.TrackLength = track?.TrackLength ?? "";
            metadata.CountryName = track?.Country ?? "";
            metadata.Surface = track?.Surface ?? "";
            if (track is null)
                Console.WriteLine($"Warning: Track \"{metadata.TrackName}\" ({metadata.CountryCode}) not found in KOMPASS ({metadata.DisciplineName}).");

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
                        Console.WriteLine($"Warning: Team from the path (\"{metadata.TeamName}\") does not match KOMPASS – using \"{rider.Team}\" for {rider.Name}.");
                    metadata.TeamName = rider.Team;
                }
            }

            metadata.RiderName = rider?.Name ?? "";
            if (rider is null)
                Console.WriteLine($"Warning: Rider \"{metadata.RiderAbbreviation}\" (season {metadata.SeasonName}, team \"{metadata.TeamName}\") not found in KOMPASS.");
        }

        /// <summary>
        /// The discipline that contains the track (country code + name); otherwise the default discipline MX or the only existing one.
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
        /// Normalizes the device/bike name (e.g. "AA 01" → "AA01") and warns about a non-canonical form.
        /// Prevents an incorrect RiderAbbreviation and spaces in the Name/Alias properties.
        /// </summary>
        private static string NormalizeBikeName(string rawDeviceName)
        {
            if (BikeAlias.IsCanonical(rawDeviceName))
                return rawDeviceName;

            var normalized = BikeAlias.Normalize(rawDeviceName);
            Console.WriteLine($"Warning: Device/bike name \"{rawDeviceName}\" contains spaces — normalizing to \"{normalized}\".");
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
