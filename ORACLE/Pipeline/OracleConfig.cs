using System.Text.Json;
using Ktm.Racing.Domain.Telemetry;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Konfiguration für ORACLE.
    /// Wird aus oracle-config.json geladen (Fallback: Standardwerte).
    /// 
    /// Enthält konfigurierbare Pfade und Team-Mappings,
    /// die bisher als Konstanten im Code standen.
    /// </summary>
    public class OracleConfig
    {
        /// <summary>Verzeichnis der KOMPASS-Datenbank (installierte Shared Resources)</summary>
        public string DatabaseDirectory { get; set; } = @"C:\KTM Tools\KTM Offroad Racing Shared Resources\DistributedTools\resources";

        /// <summary>Dateiname der KOMPASS-Datenbank</summary>
        public string DatabaseFileName { get; set; } = "KOMPASS.json";

        /// <summary>Pfad zur MotoDB loadChannels.xlsx; dessen CIRCUITS/HED-Sheets werden aus KOMPASS synchronisiert (<c>sync-motodb</c>).</summary>
        public string MotoDbLoadChannelsPath { get; set; } = @"C:\KTM Tools\KTM MotoDB\loadChannels.xlsx";

        /// <summary>
        /// Mapping von Team-Ordner-Abkürzungen zu vollständigen Teamnamen.
        /// Wird beim KOMPASS-Fallback verwendet, wenn kein .HED-File vorhanden ist.
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

        /// <summary>Schwellenwerte für die Renn-Erkennung (<see cref="RaceSegmentDetector"/>).</summary>
        public RaceDetectionThresholds RaceDetection { get; set; } = new();

        /// <summary>GPS-Kanäle und deren Aufbereitung – gilt für sVehicle, Lap-Distanzen, Renn-Erkennung und GPS-Lap-Split.</summary>
        public GpsSettings Gps { get; set; } = new();

        /// <summary>Schwellenwerte für den GPS-Lap-Split (<c>--gps-split</c>).</summary>
        public GpsLapSplitSettings GpsLapSplit { get; set; } = new();

        /// <summary>Track-Längen-Prüfung vor MotoDB (siehe <see cref="TrackLengthGate"/>).</summary>
        public TrackLengthSettings TrackLength { get; set; } = new();

        /// <summary>
        /// Wenn true, benennt ORACLE Geräteordner mit Leerzeichen im Namen kanonisch um
        /// (z.B. <c>"AA 01"</c> → <c>"AA01"</c>), bevor ein Rider-/Session-Ordner verarbeitet wird.
        /// So ist der bikeName in Ordnern und XML durchgängig ohne Leerzeichen.
        /// Non-fatal und guarded (nur nicht-kanonische Namen, nur wenn der Zielordner frei ist).
        /// Auf false setzen, falls nachgelagerte Tools (z.B. KRAKEN) die Originalordnernamen erwarten.
        /// </summary>
        public bool NormalizeDeviceFolderName { get; set; } = true;

        /// <summary>
        /// Wenn true (Default), verschiebt ORACLE verwaiste, leere Lap-Ordner (Reste eines WinTAX-Lap-Splits, ohne
        /// cableData.ztx und nicht in RunLapHeader.xml referenziert) nach <c>Run_x\_ORACLE_removed</c>, damit WinTAX
        /// den Run wieder als gültig erkennt. Es wird nichts gelöscht.
        /// </summary>
        public bool RepairOrphanLapFolders { get; set; } = true;

        /// <summary>
        /// Wenn false (Default), ersetzen KOMPASS-Stammdaten und Pfad (Rider, Team, Saison, Disziplin …) abweichende Werte
        /// einer vorhandenen .HED. Auf true setzen, wenn .HED-Dateien bewusst von Hand gepflegt werden (z.B. Ersatzfahrer
        /// auf einem fremden Bike) – dann werden Abweichungen nur gewarnt.
        /// </summary>
        public bool PreferHedMetadata { get; set; }

        /// <summary>Vollständiger Pfad zur KOMPASS-Datenbankdatei</summary>
        public string DatabaseFilePath => Path.Combine(DatabaseDirectory, DatabaseFileName);

        /// <summary>
        /// Lädt die Konfiguration aus oracle-config.json im Anwendungsverzeichnis.
        /// Fehlt die Datei, werden Standardwerte verwendet.
        /// </summary>
        /// <exception cref="InvalidDataException">Die Datei existiert, ist aber ungültig (ORACLE soll dann nicht mit Standardwerten weiterlaufen).</exception>
        public static OracleConfig Load() => Load(Path.Combine(AppContext.BaseDirectory, "oracle-config.json"));

        /// <summary>Lädt die Konfiguration aus <paramref name="configPath"/>; ohne Datei Standardwerte.</summary>
        /// <exception cref="InvalidDataException">Die Datei existiert, ist aber ungültig.</exception>
        public static OracleConfig Load(string configPath)
        {
            if (!File.Exists(configPath))
            {
                Console.WriteLine("Keine oracle-config.json gefunden, verwende Standardkonfiguration.");
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

                // JSON-Deserialisierung erzeugt Standard-Dictionary ohne custom Comparer.
                // Daher TeamMappings explizit mit case-insensitivem Comparer neu erstellen.
                config.TeamMappings = new Dictionary<string, string>(
                    config.TeamMappings ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
                config.RaceDetection ??= new RaceDetectionThresholds();
                config.Gps ??= new GpsSettings();
                config.GpsLapSplit ??= new GpsLapSplitSettings();
                config.TrackLength ??= new TrackLengthSettings();

                Console.WriteLine("Konfiguration aus oracle-config.json geladen.");
                return config;
            }
            catch (Exception ex) when (ex is JsonException or NotSupportedException or ArgumentException)
            {
                throw new InvalidDataException($"oracle-config.json is invalid ({configPath}): {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// GPS-Kanäle und ihre Aufbereitung (<see cref="GpsCleaner"/>), Abschnitt <c>Gps</c> in oracle-config.json. Entspricht
    /// <c>Lat_Valid</c>/<c>Lon_Valid</c> aus der WinTAX-VCH (Samples ohne Fix entfernen, über Aussetzer verbinden), zusätzlich
    /// werden wiederholte Fixes zusammengefasst und GPS-Sprünge entfernt.
    /// </summary>
    public sealed class GpsSettings
    {
        /// <summary>Exakter Name des Breitengrad-Kanals (Groß/Klein egal, kein Teilstring-Fallback).</summary>
        public string LatitudeChannel { get; set; } = "Lat_dez";

        /// <summary>Exakter Name des Längengrad-Kanals (Groß/Klein egal, kein Teilstring-Fallback).</summary>
        public string LongitudeChannel { get; set; } = "Lon_dez";

        /// <summary>Ein Schritt schneller als dieser Wert [km/h] gilt als GPS-Sprung, nicht als Bewegung.</summary>
        public double MaxSpeedKph { get; set; } = 200.0;

        /// <summary>… und nur ab dieser Länge [m] (kleinere Schritte sind Zeitstempel-Rauschen).</summary>
        public double MinJumpMeters { get; set; } = 10.0;

        /// <summary>Fixes zwischen zwei Sprüngen, die kürzer als diese Zeit [s] sind, werden als Ausreißer verworfen.</summary>
        public double MinSegmentSeconds { get; set; } = 2.0;

        /// <summary>Optionen der Aufbereitung.</summary>
        public GpsCleanOptions ToCleanOptions() => new()
        {
            MaxSpeedKph = MaxSpeedKph,
            MinJumpMeters = MinJumpMeters,
            MinSegmentSeconds = MinSegmentSeconds,
        };

        /// <summary>Löst einen Kanal exakt (Groß/Klein egal) aus der Kanalliste auf; <c>null</c>, wenn er fehlt.</summary>
        public static string? Resolve(IEnumerable<string> names, string channel) =>
            names.FirstOrDefault(n => n.Equals(channel, StringComparison.Ordinal))
            ?? names.FirstOrDefault(n => n.Equals(channel, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Konfigurierbare Werte für den GPS-Lap-Split (<c>split-laps</c>, <c>process --gps-split</c>).
    /// Wird aus oracle-config.json (Abschnitt <c>GpsLapSplit</c>) geladen; sonst Defaults. GPS-Kanäle: Abschnitt <c>Gps</c>.
    /// </summary>
    public sealed class GpsLapSplitSettings
    {
        /// <summary>Mindestzeit [s] zwischen zwei Lap-Grenzen (unterdrückt Doppel-Trigger und Hin-und-her).</summary>
        public double MinLapSeconds { get; set; } = 20.0;

        /// <summary>Zwei GPS-Samples mit größerem Zeitabstand [s] werden nicht verbunden (Lücke).</summary>
        public double MaxSampleGapSeconds { get; set; } = 1.0;

        /// <summary>Minimaler Winkel [°] zwischen Fahrlinie und Ziellinie; flachere (schleifende) Durchfahrten zählen nicht.</summary>
        public double MinCrossingAngleDegrees { get; set; } = 30.0;

        /// <summary>Langsamere Durchfahrten [km/h] (Schieben, Fahrerlager) zählen nicht.</summary>
        public double MinSpeedKph { get; set; } = 5.0;

        /// <summary>Durchfahrten im Umkreis [m] um den KOMPASS-Punkt bestimmen die Lage der Ziellinie.</summary>
        public double SnapRadiusMeters { get; set; } = 50.0;

        /// <summary>Halbe Länge [m] der Ziellinie zu jeder Seite der Ideallinie.</summary>
        public double LineHalfWidthMeters { get; set; } = 12.0;

        /// <summary>Eine vorhandene Lap-Grenze liegt "auf der Ziellinie", wenn eine Durchfahrt innerhalb dieser Zeit [s] liegt …</summary>
        public double BoundaryToleranceSeconds { get; set; } = 2.0;

        /// <summary>… oder die GPS-Position an der Grenze näher als diese Distanz [m] an der Linie ist.</summary>
        public double BoundaryDistanceMeters { get; set; } = 15.0;

        /// <summary>Freier Speicher, der vor dem Schreiben vorhanden sein muss, als Vielfaches der Lap-Daten des Runs.</summary>
        public double FreeSpaceFactor { get; set; } = 1.2;

        /// <summary>
        /// Maximal zulässige Zeitverschiebung eines Kanals durch das Neuschreiben, in Abtastperioden des Kanals. Die WinTAX-API
        /// legt jeden Kanal beim Schreiben auf sein Abtastraster (Verschiebung &lt; 1 Periode: 9 ms bei 50/100 Hz, bis knapp 1 s
        /// bei 1-Hz-Kanälen); größere Abweichungen deuten auf einen Fehler und brechen ab (Run bleibt unverändert).
        /// </summary>
        public double MaxTimeShiftSamples { get; set; } = 1.0;

        /// <summary>Optionen der Durchfahrts-Erkennung (Sprung-Schwellen aus <paramref name="gps"/>).</summary>
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

        /// <summary>Optionen für die Platzierung der Ziellinie.</summary>
        public FinishLineBuildOptions ToLineOptions() => new()
        {
            SnapRadiusMeters = SnapRadiusMeters,
            HalfWidthMeters = LineHalfWidthMeters,
        };

        /// <summary>Optionen der Erkennung "bereits in Runden geteilt".</summary>
        public LapSplitDetectorOptions ToDetectorOptions() => new()
        {
            BoundaryToleranceSeconds = BoundaryToleranceSeconds,
            BoundaryDistanceMeters = BoundaryDistanceMeters,
        };
    }

    /// <summary>
    /// Einstellungen der Track-Längen-Prüfung (<see cref="TrackLengthGate"/>). Abschnitt <c>TrackLength</c>
    /// in oracle-config.json; sonst Defaults.
    /// </summary>
    public sealed class TrackLengthSettings
    {
        /// <summary>
        /// Wenn true (Default), endet <c>process</c> mit <see cref="OracleExitCode.TrackLengthMissing"/>, wenn keine
        /// eindeutige Länge ermittelt werden kann – KRAKEN startet MotoDB dann nicht.
        /// </summary>
        public bool Enforce { get; set; } = true;

        /// <summary>
        /// Ordner für Längen-Vorschläge an KODE (Unterordner <c>suggestions</c> des Team-Masters).
        /// Umgebungsvariablen werden expandiert; der Master-Ordner selbst wird nie angelegt.
        /// </summary>
        public string SuggestionsDirectory { get; set; } =
            @"%OneDriveCommercial%\Workstation - OffroadSync\DevProjects\KTM Internal Software Tools\resources\suggestions";

        /// <summary>Erste und letzte Runde (Out-/In-Lap) jedes Runs ignorieren.</summary>
        public bool ExcludeFirstAndLastLap { get; set; } = true;

        /// <summary>Kürzeste plausible Rundendistanz [m].</summary>
        public double MinLapMeters { get; set; } = 300;

        /// <summary>Längste plausible Rundendistanz [m].</summary>
        public double MaxLapMeters { get; set; } = 6000;

        /// <summary>Runden, die mehr als dieser Anteil vom Median abweichen, gelten als Ausreißer (0.07 = ±7 %).</summary>
        public double ClusterTolerance { get; set; } = 0.07;

        /// <summary>Mindestanzahl übereinstimmender Runden für eine Schätzung.</summary>
        public int MinLaps { get; set; } = 3;

        /// <summary>Maximale Streuung (Interquartilsabstand / Median) der Runden, 0.03 = 3 %.</summary>
        public double MaxRelativeSpread { get; set; } = 0.03;

        /// <summary>Rundung der geschätzten Länge [m]; 1 = Median in ganzen Metern (Default).</summary>
        public int RoundToMeters { get; set; } = 1;

        /// <summary>
        /// So lange [s] wartet ORACLE auf eine gesperrte loadChannels.xlsx (z.B. in Excel offen), bevor es mit
        /// Exit-Code 5 aufgibt. 0 = nicht warten.
        /// </summary>
        public int LockWaitSeconds { get; set; } = 120;

        /// <summary>Der expandierte Vorschlags-Ordner (leer, wenn eine Umgebungsvariable fehlt).</summary>
        public string ResolvedSuggestionsDirectory
        {
            get
            {
                var expanded = Environment.ExpandEnvironmentVariables(SuggestionsDirectory ?? string.Empty).Trim();
                return expanded.Contains('%') ? string.Empty : expanded;
            }
        }

        /// <summary>Konvertiert die Einstellungen in die Domänen-Optionen des Schätzers.</summary>
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
