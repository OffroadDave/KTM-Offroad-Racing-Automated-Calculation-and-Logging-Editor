namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Exit codes for ORACLE, compatible with KRAKEN EXE job evaluation.
    /// 
    /// KRAKEN checks the process return value:
    /// <list type="bullet">
    ///   <item>0 = success → start next job (e.g. MotoDB)</item>
    ///   <item>≠ 0 = error → handle pipeline depending on the code</item>
    /// </list>
    /// 
    /// <para><b>Convention:</b> codes 0–9 are reserved for ORACLE.
    /// Higher codes can be used by future tools in the same process.</para>
    /// </summary>
    public enum OracleExitCode
    {
        /// <summary>
        /// All steps completed successfully.
        /// KRAKEN reaction: start the next dependent job.
        /// </summary>
        Success = 0,

        /// <summary>
        /// General error (unexpected exception, API error).
        /// KRAKEN reaction: abort pipeline, check logs.
        /// </summary>
        GeneralError = 1,

        /// <summary>
        /// Invalid or missing command-line arguments.
        /// KRAKEN reaction: check job configuration (ExePath/Arguments).
        /// </summary>
        InvalidArguments = 2,

        /// <summary>
        /// Required data not found (path does not exist, no runs, no metadata).
        /// KRAKEN reaction: retry if appropriate — data may still be syncing.
        /// </summary>
        DataNotFound = 3,

        /// <summary>
        /// Processing partially successful (e.g. corrupt run folders skipped, details in status <c>discovery</c>).
        /// KRAKEN reaction: log a warning, continue (start MotoDB); repair the runs and catch up with --force.
        /// </summary>
        PartialSuccess = 4,

        /// <summary>
        /// Required resource locked (e.g. loadChannels.xlsx open in Excel).
        /// KRAKEN reaction: retry — the lock is usually transient; do NOT continue with stale inputs.
        /// </summary>
        ResourceLocked = 5,

        /// <summary>
        /// No unambiguous length is known for the track (KOMPASS, loadChannels CIRCUITS, .HED and
        /// open suggestions empty/ambiguous) and it could not be estimated from the GPS lap distances.
        /// MotoDB identifies the track via an exact length comparison and would fail.
        /// KRAKEN reaction: do NOT start MotoDB; enter the length in KODE, then run again.
        /// </summary>
        TrackLengthMissing = 6,

        /// <summary>
        /// Data present but unusable: corrupt run folder (e.g. lap folder without cableData.ztx,
        /// laps split multiple times) or invalid KOMPASS.json. A retry will not change this.
        /// KRAKEN reaction: do NOT retry; repair the data (WinTAX / KODE), then run again.
        /// </summary>
        InvalidData = 7,
    }

    /// <summary>Helper functions for <see cref="OracleExitCode"/>.</summary>
    public static class OracleExitCodes
    {
        // Order = priority: the most severe code determines the overall result of multiple partial runs.
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

        /// <summary>Combines the exit codes of multiple partial runs (e.g. per device folder).</summary>
        public static OracleExitCode Combine(IEnumerable<OracleExitCode> codes)
        {
            var list = codes.ToList();
            return list.Count == 0 ? OracleExitCode.DataNotFound : Priority.First(list.Contains);
        }
    }
}
