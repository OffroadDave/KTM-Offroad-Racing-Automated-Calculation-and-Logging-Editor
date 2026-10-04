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
        /// Verarbeitung teilweise erfolgreich (z.B. Properties OK, aber Berechnung fehlgeschlagen).
        /// KRAKEN-Reaktion: Warnung loggen, ggf. weitermachen.
        /// </summary>
        PartialSuccess = 4,

        /// <summary>
        /// Benötigte Ressource gesperrt (z.B. loadChannels.xlsx offen in Excel).
        /// KRAKEN-Reaktion: Retry — die Sperre ist i.d.R. transient; NICHT mit veralteten Inputs weiterlaufen.
        /// </summary>
        ResourceLocked = 5,
    }
}
