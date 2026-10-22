using System.Text.Json;
using Ktm.Racing.Domain.Telemetry;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Configuration for ORACLE.
    /// Loaded from oracle-config.json (fallback: defaults).
    /// 
    /// Contains configurable paths and team mappings
    /// that used to be constants in the code.
    /// </summary>
    public class OracleConfig
    {
        /// <summary>Directory of the KOMPASS database (installed Shared Resources)</summary>
        public string DatabaseDirectory { get; set; } = @"C:\KTM Tools\KTM Offroad Racing Shared Resources\DistributedTools\resources";

        /// <summary>File name of the KOMPASS database</summary>
        public string DatabaseFileName { get; set; } = "KOMPASS.json";

        /// <summary>Path to the MotoDB loadChannels.xlsx; its CIRCUITS/HED sheets are synchronized from KOMPASS (<c>sync-motodb</c>).</summary>
        public string MotoDbLoadChannelsPath { get; set; } = @"C:\KTM Tools\KTM MotoDB\loadChannels.xlsx";

        /// <summary>
        /// Mapping of team folder abbreviations to full team names.
        /// Used by the KOMPASS fallback when no .HED file is present.
        /// </summary>
        public Dictionary<string, string> TeamMappings { get; set; } = new(StringComparer.OrdinalIgnoreCase)
        {
            ["01_KTM-FR"] = "KTM Factory Racing",
            ["KTM-FR"] = "KTM Factory Racing",
            ["02_HQV"] = "Husqvarna Factory Racing",
            ["HQV"] = "Husqvarna Factory Racing",
            ["03_KTM-DC"] = "KTM De Carli Racing",
            ["KTM-DC"] = "KTM De Carli Racing",
        };

        /// <summary>Thresholds for race detection (<see cref="RaceSegmentDetector"/>).</summary>
        public RaceDetectionThresholds RaceDetection { get; set; } = new();

        /// <summary>GPS channels and their preprocessing – applies to sVehicle, lap distances, race detection and GPS lap split.</summary>
        public GpsSettings Gps { get; set; } = new();

        /// <summary>Thresholds for the GPS lap split (<c>--gps-split</c>).</summary>
        public GpsLapSplitSettings GpsLapSplit { get; set; } = new();

        /// <summary>Track length check before MotoDB (see <see cref="TrackLengthGate"/>).</summary>
        public TrackLengthSettings TrackLength { get; set; } = new();

        /// <summary>Weather during the run from a historical weather model (see <see cref="WeatherService"/>).</summary>
        public WeatherSettings Weather { get; set; } = new();

        /// <summary>
        /// If true, ORACLE canonically renames device folders with spaces in their name
        /// (e.g. <c>"AA 01"</c> → <c>"AA01"</c>) before a rider/session folder is processed.
        /// This keeps the bikeName consistently space-free in folders and XML.
        /// Non-fatal and guarded (only non-canonical names, only if the target folder is free).
        /// Set to false if downstream tools (e.g. KRAKEN) expect the original folder names.
        /// </summary>
        public bool NormalizeDeviceFolderName { get; set; } = true;

        /// <summary>
        /// If true (default), ORACLE moves orphaned, empty lap folders (leftovers of a WinTAX lap split, without
        /// cableData.ztx and not referenced in RunLapHeader.xml) to <c>Run_x\_ORACLE_removed</c> so that WinTAX
        /// recognizes the run as valid again. Nothing is deleted.
        /// </summary>
        public bool RepairOrphanLapFolders { get; set; } = true;

        /// <summary>
        /// If false (default), KOMPASS master data and the path (rider, team, season, discipline …) replace mismatching values
        /// of an existing .HED. Set to true if .HED files are deliberately maintained by hand (e.g. a substitute rider
        /// on someone else's bike) – mismatches are then only warned about.
        /// </summary>
        public bool PreferHedMetadata { get; set; }

        /// <summary>Full path to the KOMPASS database file</summary>
        public string DatabaseFilePath => Path.Combine(DatabaseDirectory, DatabaseFileName);

        /// <summary>
        /// Loads the configuration from oracle-config.json in the application directory.
        /// If the file is missing, defaults are used.
        /// </summary>
        /// <exception cref="InvalidDataException">The file exists but is invalid (ORACLE must not continue with defaults in that case).</exception>
        public static OracleConfig Load() => Load(Path.Combine(AppContext.BaseDirectory, "oracle-config.json"));

        /// <summary>Loads the configuration from <paramref name="configPath"/>; defaults if the file is missing.</summary>
        /// <exception cref="InvalidDataException">The file exists but is invalid.</exception>
        public static OracleConfig Load(string configPath)
        {
            if (!File.Exists(configPath))
            {
                Console.WriteLine("No oracle-config.json found, using default configuration.");
                return new OracleConfig();
            }

            try
            {
                var json = File.ReadAllText(configPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };

                var config = JsonSerializer.Deserialize<OracleConfig>(json, options)
                             ?? throw new InvalidDataException("oracle-config.json is empty.");

                // JSON deserialization creates a standard dictionary without a custom comparer.
                // Therefore recreate TeamMappings explicitly with a case-insensitive comparer.
                config.TeamMappings = new Dictionary<string, string>(
                    config.TeamMappings ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
                config.RaceDetection ??= new RaceDetectionThresholds();
                config.Gps ??= new GpsSettings();
                config.GpsLapSplit ??= new GpsLapSplitSettings();
                config.TrackLength ??= new TrackLengthSettings();
                config.Weather ??= new WeatherSettings();

                Console.WriteLine("Configuration loaded from oracle-config.json.");
                return config;
            }
            catch (Exception ex) when (ex is JsonException or NotSupportedException or ArgumentException)
            {
                throw new InvalidDataException($"oracle-config.json is invalid ({configPath}): {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// GPS channels and their preprocessing (<see cref="GpsCleaner"/>), section <c>Gps</c> in oracle-config.json. Corresponds to
    /// <c>Lat_Valid</c>/<c>Lon_Valid</c> from the WinTAX VCH (remove samples without fix, bridge dropouts); additionally,
    /// repeated fixes are merged and GPS jumps are removed.
    /// </summary>
    public sealed class GpsSettings
    {
        /// <summary>Exact name of the latitude channel (case-insensitive, no substring fallback).</summary>
        public string LatitudeChannel { get; set; } = "Lat_dez";

        /// <summary>Exact name of the longitude channel (case-insensitive, no substring fallback).</summary>
        public string LongitudeChannel { get; set; } = "Lon_dez";

        /// <summary>A step faster than this value [km/h] counts as a GPS jump, not as movement.</summary>
        public double MaxSpeedKph { get; set; } = 200.0;

        /// <summary>… and only from this length [m] upwards (smaller steps are timestamp noise).</summary>
        public double MinJumpMeters { get; set; } = 10.0;

        /// <summary>Fixes between two jumps spanning less than this time [s] are discarded as outliers.</summary>
        public double MinSegmentSeconds { get; set; } = 2.0;

        /// <summary>Preprocessing options.</summary>
        public GpsCleanOptions ToCleanOptions() => new()
        {
            MaxSpeedKph = MaxSpeedKph,
            MinJumpMeters = MinJumpMeters,
            MinSegmentSeconds = MinSegmentSeconds,
        };

        /// <summary>Resolves a channel exactly (case-insensitive) from the channel list; <c>null</c> if it is missing.</summary>
        public static string? Resolve(IEnumerable<string> names, string channel) => ChannelResolver.Find(names, channel);
    }

    /// <summary>
    /// Configurable values for the GPS lap split (<c>split-laps</c>, <c>process --gps-split</c>).
    /// Loaded from oracle-config.json (section <c>GpsLapSplit</c>); otherwise defaults. GPS channels: section <c>Gps</c>.
    /// </summary>
    public sealed class GpsLapSplitSettings
    {
        /// <summary>Minimum time [s] between two lap boundaries (suppresses double triggers and back-and-forth).</summary>
        public double MinLapSeconds { get; set; } = 20.0;

        /// <summary>Two GPS samples with a larger time gap [s] are not connected (gap).</summary>
        public double MaxSampleGapSeconds { get; set; } = 1.0;

        /// <summary>Minimum angle [°] between riding line and finish line; shallower (grazing) crossings do not count.</summary>
        public double MinCrossingAngleDegrees { get; set; } = 30.0;

        /// <summary>Slower crossings [km/h] (pushing, paddock) do not count.</summary>
        public double MinSpeedKph { get; set; } = 5.0;

        /// <summary>Crossings within this radius [m] around the KOMPASS point determine the position of the finish line.</summary>
        public double SnapRadiusMeters { get; set; } = 50.0;

        /// <summary>Half length [m] of the finish line on each side of the racing line.</summary>
        public double LineHalfWidthMeters { get; set; } = 12.0;

        /// <summary>An existing lap boundary lies "on the finish line" if a crossing occurs within this time [s] …</summary>
        public double BoundaryToleranceSeconds { get; set; } = 2.0;

        /// <summary>… or the GPS position at the boundary is closer than this distance [m] to the line.</summary>
        public double BoundaryDistanceMeters { get; set; } = 15.0;

        /// <summary>Free disk space required before writing, as a multiple of the run's lap data.</summary>
        public double FreeSpaceFactor { get; set; } = 1.2;

        /// <summary>
        /// Maximum permitted time shift of a channel caused by rewriting, in sample periods of the channel. The WinTAX API
        /// aligns every channel to its sample grid when writing (shift &lt; 1 period: 9 ms at 50/100 Hz, up to almost 1 s
        /// for 1 Hz channels); larger deviations indicate an error and abort (the run remains unchanged).
        /// </summary>
        public double MaxTimeShiftSamples { get; set; } = 1.0;

        /// <summary>Crossing detection options (jump thresholds from <paramref name="gps"/>).</summary>
        public GpsLapSplitOptions ToOptions(GpsSettings? gps = null)
        {
            gps ??= new GpsSettings();
            return new()
            {
                MinLapSeconds = MinLapSeconds,
                MaxSampleGapSeconds = MaxSampleGapSeconds,
                MinCrossingAngleDegrees = MinCrossingAngleDegrees,
                MinSpeedKph = MinSpeedKph,
                MaxSpeedKph = gps.MaxSpeedKph,
                MinJumpMeters = gps.MinJumpMeters,
            };
        }

        /// <summary>Options for placing the finish line.</summary>
        public FinishLineBuildOptions ToLineOptions() => new()
        {
            SnapRadiusMeters = SnapRadiusMeters,
            HalfWidthMeters = LineHalfWidthMeters,
        };

        /// <summary>Options for detecting "already split into laps".</summary>
        public LapSplitDetectorOptions ToDetectorOptions() => new()
        {
            BoundaryToleranceSeconds = BoundaryToleranceSeconds,
            BoundaryDistanceMeters = BoundaryDistanceMeters,
        };
    }

    /// <summary>
    /// Settings of the track length check (<see cref="TrackLengthGate"/>). Section <c>TrackLength</c>
    /// in oracle-config.json; otherwise defaults.
    /// </summary>
    public sealed class TrackLengthSettings
    {
        /// <summary>
        /// If true (default), <c>process</c> ends with <see cref="OracleExitCode.TrackLengthMissing"/> if no
        /// unambiguous length can be determined – KRAKEN then does not start MotoDB.
        /// </summary>
        public bool Enforce { get; set; } = true;

        /// <summary>
        /// Folder for length suggestions to KODE (subfolder <c>suggestions</c> of the team master).
        /// Environment variables are expanded; the master folder itself is never created.
        /// </summary>
        public string SuggestionsDirectory { get; set; } =
            @"%OneDriveCommercial%\Workstation - OffroadSync\DevProjects\KTM Internal Software Tools\resources\suggestions";

        /// <summary>Ignore the first and last lap (out/in lap) of each run.</summary>
        public bool ExcludeFirstAndLastLap { get; set; } = true;

        /// <summary>Shortest plausible lap distance [m].</summary>
        public double MinLapMeters { get; set; } = 300;

        /// <summary>Longest plausible lap distance [m].</summary>
        public double MaxLapMeters { get; set; } = 6000;

        /// <summary>Laps deviating from the median by more than this fraction count as outliers (0.07 = ±7 %).</summary>
        public double ClusterTolerance { get; set; } = 0.07;

        /// <summary>Minimum number of matching laps for an estimate.</summary>
        public int MinLaps { get; set; } = 3;

        /// <summary>Maximum spread (interquartile range / median) of the laps, 0.03 = 3 %.</summary>
        public double MaxRelativeSpread { get; set; } = 0.03;

        /// <summary>Rounding of the estimated length [m]; 1 = median in whole meters (default).</summary>
        public int RoundToMeters { get; set; } = 1;

        /// <summary>
        /// How long [s] ORACLE waits for a locked loadChannels.xlsx (e.g. open in Excel) before giving up with
        /// exit code 5. 0 = do not wait.
        /// </summary>
        public int LockWaitSeconds { get; set; } = 120;

        /// <summary>The expanded suggestions folder (empty if an environment variable is missing).</summary>
        public string ResolvedSuggestionsDirectory
        {
            get
            {
                var expanded = Environment.ExpandEnvironmentVariables(SuggestionsDirectory ?? string.Empty).Trim();
                return expanded.Contains('%') ? string.Empty : expanded;
            }
        }

        /// <summary>Converts the settings into the estimator's domain options.</summary>
        public TrackLengthEstimateOptions ToOptions() => new()
        {
            ExcludeFirstAndLastLap = ExcludeFirstAndLastLap,
            MinLapMeters = MinLapMeters,
            MaxLapMeters = MaxLapMeters,
            ClusterTolerance = ClusterTolerance,
            MinLaps = MinLaps,
            MaxRelativeSpread = MaxRelativeSpread,
            RoundToMeters = RoundToMeters,
        };
    }
}
