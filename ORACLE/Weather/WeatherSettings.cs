namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Weather during a run from a historical weather model (section <c>Weather</c> in oracle-config.json). ORACLE writes the
    /// values to the run's .HED (<c>[WEATHER]</c>, for MotoDB) and to <c>Lap_*\Meteo.xml</c> (WinTAX meteo page, <c>$METEO…</c>
    /// in VCH). Only the time of the run itself is evaluated – no forecast, no look-back before the session.
    /// </summary>
    public sealed class WeatherSettings
    {
        /// <summary>Fetch weather data during <c>process</c>. The <c>weather</c> command runs regardless.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Open-Meteo Historical Forecast API (archived high-resolution model runs, available shortly after the session;
        /// 15-minute values). Weather data by Open-Meteo.com, licence CC BY 4.0.
        /// </summary>
        public string BaseUrl { get; set; } = "https://historical-forecast-api.open-meteo.com/v1/forecast";

        /// <summary>Weather model (e.g. <c>icon_d2</c>, <c>ecmwf_ifs</c>); empty = <c>best_match</c> (best model for the location).</summary>
        public string Model { get; set; } = "";

        /// <summary>Environment variable holding an optional Open-Meteo API key (never stored in the configuration file).</summary>
        public string ApiKeyEnvironmentVariable { get; set; } = "ORACLE_OPENMETEO_APIKEY";

        /// <summary>Timeout per request [s].</summary>
        public int TimeoutSeconds { get; set; } = 20;

        /// <summary>
        /// Clock of the logger time stamps in LapHeader.xml (<c>STS</c>/<c>STMs</c>): <c>TrackLocal</c> (default – the logger runs on
        /// the local time of the track, verified with the GPS time at RT22) or <c>Utc</c>.
        /// </summary>
        public string LoggerClock { get; set; } = "TrackLocal";

        /// <summary>Precipitation during the run from which the track counts as <c>damp</c> [mm].</summary>
        public double DampRainMm { get; set; } = 0.1;

        /// <summary>Precipitation during the run from which the track counts as <c>wet</c> [mm].</summary>
        public double WetRainMm { get; set; } = 1.0;

        /// <summary>Precipitation rate in any 15-minute interval of the run from which the track counts as <c>wet</c> [mm/h].</summary>
        public double WetRateMmPerHour { get; set; } = 2.0;

        /// <summary>Write the <c>[WEATHER]</c> section into the run's .HED.</summary>
        public bool WriteHed { get; set; } = true;

        /// <summary>Write <c>Meteo.xml</c> into every lap folder.</summary>
        public bool WriteMeteoXml { get; set; } = true;

        /// <summary>Pressure unit of <c>&lt;Bar&gt;</c> in Meteo.xml: <c>mbar</c> (= hPa) or <c>bar</c>.</summary>
        public string MeteoPressureUnit { get; set; } = "mbar";

        /// <summary>Wind speed unit of <c>&lt;WindSp&gt;</c> in Meteo.xml: <c>kmh</c> or <c>ms</c>.</summary>
        public string MeteoWindSpeedUnit { get; set; } = "kmh";

        /// <summary><c>true</c> when the logger clock is UTC; otherwise the local time of the track.</summary>
        internal bool LoggerClockIsUtc => LoggerClock.Equals("Utc", StringComparison.OrdinalIgnoreCase);
    }
}
