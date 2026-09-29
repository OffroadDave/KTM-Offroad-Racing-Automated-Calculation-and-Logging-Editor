namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Datenmodell für alle extrahierten Metadaten eines WinTAX4-Runs.
    /// Wird entweder aus einer .HED-Datei oder aus dem Pfad + KOMPASS-Datenbank befüllt.
    /// 
    /// Kern-Metadaten (BikeName, TrackName, TrackLength, RiderName) werden immer gesetzt.
    /// Erweiterte Metadaten werden nur beim KOMPASS-Fallback befüllt und für die .HED-Generierung verwendet.
    /// </summary>
    public class RunMetadata
    {
        // --- Kern-Metadaten (für den XmlRunEditor benötigt) ---

        /// <summary>Bike/Fahrer-Alias, z.B. "JP01"</summary>
        public string BikeName { get; set; } = "";

        /// <summary>Track-Name, z.B. "Maggiora"</summary>
        public string TrackName { get; set; } = "";

        /// <summary>Track-Länge in Metern</summary>
        public string TrackLength { get; set; } = "";

        /// <summary>Vollständiger Fahrername, z.B. "Jorge Prado"</summary>
        public string RiderName { get; set; } = "";

        // --- Erweiterte Metadaten (bei KOMPASS-Fallback befüllt, für .HED-Generierung) ---

        /// <summary>Disziplin-Kürzel, z.B. "MX", "EN"</summary>
        public string DisciplineName { get; set; } = "";

        /// <summary>Saison, z.B. "2025"</summary>
        public string SeasonName { get; set; } = "";

        /// <summary>Vollständiger Teamname, z.B. "KTM Factory Racing"</summary>
        public string TeamName { get; set; } = "";

        /// <summary>Fahrer-Abkürzung ohne Bike-Nummer, z.B. "JP"</summary>
        public string RiderAbbreviation { get; set; } = "";

        /// <summary>Bike-Nummer, z.B. "01"</summary>
        public string BikeNumber { get; set; } = "";

        /// <summary>Event-Typ, z.B. "Racetrack", "Testtrack"</summary>
        public string TypeName { get; set; } = "";

        /// <summary>ISO-Ländercode, z.B. "AT", "IT"</summary>
        public string CountryCode { get; set; } = "";

        /// <summary>Vollständiger Ländername</summary>
        public string CountryName { get; set; } = "";

        /// <summary>Kalenderwoche, z.B. "01"</summary>
        public string CalendarWeek { get; set; } = "";

        /// <summary>Event-Name (Ordnername), z.B. "RT01_AT-RedBull"</summary>
        public string EventName { get; set; } = "";

        /// <summary>Session-Name (Ordnername), z.B. "FP1"</summary>
        public string SessionName { get; set; } = "";

        /// <summary>Run-Nummer oder Ordnername, z.B. "Run_001"</summary>
        public string RunNumber { get; set; } = "";
    }
}
