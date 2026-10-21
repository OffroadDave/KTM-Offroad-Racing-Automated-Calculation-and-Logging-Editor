namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Exit-Codes für ORACLE, kompatibel mit KRAKEN EXE-Job-Auswertung.
    /// 
    /// KRAKEN prüft den Rückgabewert des Prozesses:
    /// <list type="bullet">
    ///   <item>0 = Erfolg → nächsten Job starten (z.B. MotoDB)</item>
    ///   <item>≠ 0 = Fehler → Pipeline abhängig vom Code behandeln</item>
    /// </list>
    /// 
    /// <para><b>Konvention:</b> Codes 0–9 sind reserviert für ORACLE.
    /// Höhere Codes können für zukünftige Tools im gleichen Prozess genutzt werden.</para>
    /// </summary>
    public enum OracleExitCode
    {
        /// <summary>
        /// Alle Schritte erfolgreich abgeschlossen.
        /// KRAKEN-Reaktion: Nächsten abhängigen Job starten.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Allgemeiner Fehler (unerwartete Exception, API-Fehler).
        /// KRAKEN-Reaktion: Pipeline abbrechen, Logs prüfen.
        /// </summary>
        GeneralError = 1,

        /// <summary>
        /// Ungültige oder fehlende Kommandozeilen-Argumente.
        /// KRAKEN-Reaktion: Job-Konfiguration (ExePath/Arguments) prüfen.
        /// </summary>
        InvalidArguments = 2,

        /// <summary>
        /// Benötigte Daten nicht gefunden (Pfad existiert nicht, keine Runs, keine Metadaten).
        /// KRAKEN-Reaktion: Ggf. Retry — Daten könnten noch synchronisiert werden.
        /// </summary>
        DataNotFound = 3,

        /// <summary>
        /// Verarbeitung teilweise erfolgreich (z.B. beschädigte Run-Ordner übersprungen, Details im Status <c>discovery</c>).
        /// KRAKEN-Reaktion: Warnung loggen, weitermachen (MotoDB starten); Runs reparieren und mit --force nachholen.
        /// </summary>
        PartialSuccess = 4,

        /// <summary>
        /// Benötigte Ressource gesperrt (z.B. loadChannels.xlsx offen in Excel).
        /// KRAKEN-Reaktion: Retry — die Sperre ist i.d.R. transient; NICHT mit veralteten Inputs weiterlaufen.
        /// </summary>
        ResourceLocked = 5,

        /// <summary>
        /// Für den Track ist keine eindeutige Länge bekannt (KOMPASS, loadChannels CIRCUITS, .HED und
        /// offene Vorschläge leer/mehrdeutig) und sie konnte nicht aus den GPS-Rundendistanzen geschätzt werden.
        /// MotoDB identifiziert die Strecke über einen exakten Längenvergleich und würde fehlschlagen.
        /// KRAKEN-Reaktion: MotoDB NICHT starten; Länge in KODE eintragen, dann erneut ausführen.
        /// </summary>
        TrackLengthMissing = 6,

        /// <summary>
        /// Daten vorhanden, aber unbrauchbar: beschädigter Run-Ordner (z.B. Lap-Ordner ohne cableData.ztx,
        /// mehrfach gesplittete Laps) oder ungültige KOMPASS.json. Ein Retry ändert daran nichts.
        /// KRAKEN-Reaktion: NICHT wiederholen; Daten reparieren (WinTAX / KODE), dann erneut ausführen.
        /// </summary>
        InvalidData = 7,
    }

    /// <summary>Hilfsfunktionen für <see cref="OracleExitCode"/>.</summary>
    public static class OracleExitCodes
    {
        // Reihenfolge = Priorität: der gravierendste Code bestimmt das Gesamtergebnis mehrerer Teilverarbeitungen.
        private static readonly OracleExitCode[] Priority =
        [
            OracleExitCode.GeneralError,
            OracleExitCode.InvalidArguments,
            OracleExitCode.ResourceLocked,
            OracleExitCode.TrackLengthMissing,
            OracleExitCode.InvalidData,
            OracleExitCode.DataNotFound,
            OracleExitCode.PartialSuccess,
            OracleExitCode.Success,
        ];

        /// <summary>Fasst die Exit-Codes mehrerer Teilverarbeitungen (z.B. je Geräteordner) zusammen.</summary>
        public static OracleExitCode Combine(IEnumerable<OracleExitCode> codes)
        {
            var list = codes.ToList();
            return list.Count == 0 ? OracleExitCode.DataNotFound : Priority.First(list.Contains);
        }
    }
}
