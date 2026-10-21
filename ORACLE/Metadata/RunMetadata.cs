namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Data model for all extracted metadata of a WinTAX4 run.
    /// Populated either from a .HED file or from the path + KOMPASS database.
    /// 
    /// Core metadata (BikeName, TrackName, TrackLength, RiderName) is always set.
    /// Extended metadata is only populated in the KOMPASS fallback and used for .HED generation.
    /// </summary>
    public class RunMetadata
    {
        // --- Core metadata (required by the XmlRunEditor) ---

        /// <summary>Bike/rider alias, e.g. "JP01"</summary>
        public string BikeName { get; set; } = "";

        /// <summary>Track name, e.g. "Maggiora"</summary>
        public string TrackName { get; set; } = "";

        /// <summary>Track length in meters</summary>
        public string TrackLength { get; set; } = "";

        /// <summary>Track surface (e.g. Hardpack, Sand); written to the HED section [TRACK].</summary>
        public string Surface { get; set; } = "";

        /// <summary>Full rider name, e.g. "Jorge Prado"</summary>
        public string RiderName { get; set; } = "";

        // --- Extended metadata (populated in KOMPASS fallback, for .HED generation) ---

        /// <summary>Discipline abbreviation, e.g. "MX", "EN"</summary>
        public string DisciplineName { get; set; } = "";

        /// <summary>Season, e.g. "2025"</summary>
        public string SeasonName { get; set; } = "";

        /// <summary>Full team name, e.g. "KTM Factory Racing"</summary>
        public string TeamName { get; set; } = "";

        /// <summary>Rider abbreviation without bike number, e.g. "JP"</summary>
        public string RiderAbbreviation { get; set; } = "";

        /// <summary>Bike number, e.g. "01"</summary>
        public string BikeNumber { get; set; } = "";

        /// <summary>Event type, e.g. "Racetrack", "Testtrack"</summary>
        public string TypeName { get; set; } = "";

        /// <summary>Engine class/displacement from the path, e.g. "250" or "450"</summary>
        public string EngineClass { get; set; } = "";

        /// <summary>ISO country code, e.g. "AT", "IT"</summary>
        public string CountryCode { get; set; } = "";

        /// <summary>Full country name</summary>
        public string CountryName { get; set; } = "";

        /// <summary>Calendar week, e.g. "01"</summary>
        public string CalendarWeek { get; set; } = "";

        /// <summary>Event name (folder name), e.g. "RT01_AT-RedBull"</summary>
        public string EventName { get; set; } = "";

        /// <summary>Session name (folder name), e.g. "FP1"</summary>
        public string SessionName { get; set; } = "";

        /// <summary>Run number or folder name, e.g. "Run_001"</summary>
        public string RunNumber { get; set; } = "";
    }
}
