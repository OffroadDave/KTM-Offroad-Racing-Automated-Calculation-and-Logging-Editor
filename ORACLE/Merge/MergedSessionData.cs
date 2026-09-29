using KtmDataHandler.Models.Channels;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Ergebnis der Session-Zusammenführung (Merged Session Data).
    /// 
    /// Enthält das konsolidierte <see cref="IDataset"/> mit allen Channels über alle Runs
    /// eines Riders innerhalb einer Session, zusammengeführt via <see cref="ITimeseries.Concat"/>.
    /// 
    /// <para><b>Hierarchie:</b> Event → Session → Rider/Device → Run₁..Runₙ → Lap₁..Lapₘ</para>
    /// <para><b>Beispiel:</b> RT12_ESP-Almonte / R1 / AA01 / Run_202..Run_205</para>
    /// 
    /// <para><b>Edge-Cases:</b></para>
    /// <list type="bullet">
    ///   <item>0 Runs → <see cref="HasData"/> = false, <see cref="MergedDataset"/> leer</item>
    ///   <item>Run mit 0 Laps → Status <see cref="RunMergeStatus.NoLaps"/>, wird übersprungen</item>
    ///   <item>Run ohne Channels → Status <see cref="RunMergeStatus.NoChannels"/>, wird übersprungen</item>
    /// </list>
    /// </summary>
    public sealed class MergedSessionData
    {
        /// <summary>Session-Name aus der WinTAX4-Ordnerstruktur (z.B. "R1", "FP1")</summary>
        public string SessionName { get; init; } = "";

        /// <summary>Rider/Device-Name aus der WinTAX4-Ordnerstruktur (z.B. "AA01", "JP01")</summary>
        public string RiderName { get; init; } = "";

        /// <summary>
        /// Konsolidiertes Dataset mit allen gemergten Channels.
        /// Jeder Channel enthält die verketteten Zeitreihen aller Runs in chronologischer Reihenfolge.
        /// Leeres Dataset wenn keine Runs erfolgreich verarbeitet wurden.
        /// </summary>
        public IDataset MergedDataset { get; init; } = new Dataset();

        /// <summary>
        /// Detailinfo pro verarbeitetem Run, sortiert nach Run-Reihenfolge.
        /// Enthält auch übersprungene und fehlerhafte Runs mit entsprechendem Status.
        /// </summary>
        public IReadOnlyList<MergedRunInfo> Runs { get; init; } = [];

        /// <summary>
        /// Union aller Channel-Namen über alle erfolgreich geladenen Runs.
        /// Channels, die nur in einem Subset der Runs vorkommen, sind ebenfalls enthalten.
        /// </summary>
        public IReadOnlyList<string> ChannelNames { get; init; } = [];

        /// <summary>Gesamtanzahl entdeckter Runs (inkl. übersprungener und fehlerhafter)</summary>
        public int TotalRunsDiscovered { get; init; }

        /// <summary>Anzahl erfolgreich gemergter Runs (Status = Success)</summary>
        public int MergedRunCount { get; init; }

        /// <summary>True wenn mindestens ein Run erfolgreich gemergt wurde</summary>
        public bool HasData => MergedRunCount > 0;
    }

    /// <summary>
    /// Verarbeitungsergebnis eines einzelnen Runs innerhalb der Session-Zusammenführung.
    /// 
    /// Wird für jeden entdeckten Run erzeugt, unabhängig ob der Run erfolgreich 
    /// verarbeitet wurde oder nicht. Der <see cref="Status"/> gibt Auskunft über das Ergebnis.
    /// </summary>
    public sealed class MergedRunInfo
    {
        /// <summary>Vollständiger Pfad zum Run-Ordner</summary>
        public string RunPath { get; init; } = "";

        /// <summary>Run-Ordnername (z.B. "Run_202")</summary>
        public string RunFolderName { get; init; } = "";

        /// <summary>
        /// Anzahl Laps in diesem Run.
        /// Entspricht der Anzahl Lap_*-Unterordner mit cableData.ztx.
        /// 0 bei Runs ohne Daten (z.B. Run_201, Run_206 im Beispieldatensatz).
        /// </summary>
        public int LapCount { get; init; }

        /// <summary>
        /// Channel-Namen, die in diesem Run verfügbar waren.
        /// Leer bei Runs mit Status != Success.
        /// </summary>
        public IReadOnlyList<string> ChannelNames { get; init; } = [];

        /// <summary>
        /// Letzte Sample-Zeit in Sekunden (via <c>GetLastSampleTime()</c>).
        /// Wird für die korrekte zeitliche Einordnung beim Merge benötigt.
        /// 0.0 bei Runs mit Status != Success.
        /// </summary>
        public double LastSampleTime { get; init; }

        /// <summary>Verarbeitungsstatus dieses Runs</summary>
        public RunMergeStatus Status { get; init; }

        /// <summary>
        /// Fehlermeldung bei Status != Success.
        /// Null bei erfolgreich verarbeiteten Runs.
        /// </summary>
        public string? ErrorMessage { get; init; }
    }

    /// <summary>
    /// Status-Enum für die Run-Verarbeitung innerhalb des Session-Merge.
    /// Definiert alle möglichen Ergebnisse der Verarbeitung eines einzelnen Runs.
    /// </summary>
    public enum RunMergeStatus
    {
        /// <summary>Run erfolgreich geladen und in das Merge-Ergebnis integriert</summary>
        Success,

        /// <summary>Run enthält keine Laps / keine cableData.ztx (übersprungen)</summary>
        NoLaps,

        /// <summary>Run enthält keine Channels nach dem Laden (übersprungen)</summary>
        NoChannels,

        /// <summary>Fehler beim Laden des Runs über die KtmDataHandler-API</summary>
        LoadError,

        /// <summary>Run-Pfad existiert nicht oder ist kein gültiger WinTAX4-Run</summary>
        InvalidPath
    }
}
