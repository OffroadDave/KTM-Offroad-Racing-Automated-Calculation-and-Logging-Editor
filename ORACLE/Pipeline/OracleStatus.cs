using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Status file for idempotency: saved in the run folder as <c>oracle.status.json</c>
    /// after successful processing.
    /// 
    /// <para><b>Idempotency logic:</b></para>
    /// <list type="bullet">
    ///   <item>Status exists + ExitCode == 0 → run already processed, skip</item>
    ///   <item>Status exists + ExitCode != 0 → previous processing failed, retry</item>
    ///   <item>Status does not exist → process normally</item>
    ///   <item><c>--force</c> flag → always process, ignore status</item>
    /// </list>
    /// 
    /// <para><b>File structure:</b></para>
    /// <code>
    /// Run_205/
    /// ├── Lap_001/
    /// ├── download.done          ← SENTINEL
    /// └── oracle.status.json     ← ORACLE (this model)
    /// </code>
    /// </summary>
    public sealed class OracleStatus
    {
        private const string FileName = "oracle.status.json";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        // Determine the assembly version once via reflection and cache it (constant at runtime).
        private static readonly string CachedVersion = GetAssemblyVersion();

        /// <summary>Schema version of the status file</summary>
        public string Version { get; set; } = "1.0";

        /// <summary>Processed run/rider path</summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>Time of processing (UTC)</summary>
        public DateTime ProcessedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>ORACLE version (from assembly)</summary>
        public string OracleVersion { get; set; } = CachedVersion;

        /// <summary>Status of the individual processing steps</summary>
        public Dictionary<string, StepStatus> Steps { get; set; } = [];

        /// <summary>Final exit code</summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// Revision of the processing logic that wrote the status (0 = before it was introduced). If it is lower than
        /// <see cref="CurrentProcessingRevision"/>, the folder is automatically reprocessed – so already
        /// processed sessions benefit from fixes (e.g. metadata, run repair) without <c>--force</c>.
        /// </summary>
        public int ProcessingRevision { get; set; }

        /// <summary>
        /// Current revision of the processing logic. Increase it when a change improves already processed data.
        /// 2 = metadata by pattern, devices separated, run repair, empty runs, length median (2026-10-12).
        /// 3 = KOMPASS master data replaces mismatching .HED values (e.g. riderName placeholder) (2026-10-19).
        /// </summary>
        public const int CurrentProcessingRevision = 3;

        /// <summary>Processing state of a folder according to the status file.</summary>
        public enum State
        {
            /// <summary>No (readable) status or failed – process fully.</summary>
            NotProcessed,

            /// <summary>Processed with older processing logic – fully reprocess.</summary>
            Outdated,

            /// <summary>Processed; only the track length was still missing (exit 5/6) – only catch up on the length.</summary>
            LengthPending,

            /// <summary>Successfully processed (exit 0, or 4 only because of corrupt runs).</summary>
            Done,
        }

        /// <summary>Determines the processing state of the folder.</summary>
        public static State GetState(string targetPath)
        {
            var status = Load(targetPath);
            if (status is null)
                return State.NotProcessed;
            if (status.ProcessingRevision < CurrentProcessingRevision)
                return State.Outdated;
            if (status.ExitCode == 0
                || (status.ExitCode == 4 && status.Steps.TryGetValue("discovery", out var discovery) && discovery.Status == StepStatus.Partial))
                return State.Done;
            if (status.ExitCode is 5 or 6 && (status.Steps.ContainsKey("stint") || status.Steps.ContainsKey("calculations")))
                return State.LengthPending;
            return State.NotProcessed;
        }

        /// <summary>
        /// Checks whether the specified path has already been processed (<see cref="State.Done"/> or <see cref="State.LengthPending"/>).
        /// </summary>
        public static bool IsAlreadyProcessed(string targetPath)
        {
            var state = GetState(targetPath);
            if (state == State.Outdated)
                Console.WriteLine($"[status] Processed with older ORACLE processing logic (revision < {CurrentProcessingRevision}) – reprocessing.");
            return state is State.Done or State.LengthPending;
        }

        /// <summary>
        /// Loads the status file from the specified directory.
        /// </summary>
        /// <returns>OracleStatus on success, null if no status file exists or on a parse error</returns>
        public static OracleStatus? Load(string directoryPath)
        {
            var filePath = System.IO.Path.Combine(directoryPath, FileName);
            if (!File.Exists(filePath))
                return null;

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<OracleStatus>(json, JsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: status file could not be loaded: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Saves the status file in the specified directory. Non-fatal: if the file is locked
        /// (e.g. by an editor/watcher), a warning is logged instead of throwing — a
        /// successful processing must not appear as a failure because of a write error.
        /// Without a status file, the path is reprocessed on the next run.
        /// </summary>
        public void Save(string directoryPath)
        {
            var filePath = System.IO.Path.Combine(directoryPath, FileName);
            try
            {
                var json = JsonSerializer.Serialize(this, JsonOptions);
                AtomicFile.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: status file could not be saved ({filePath}): {ex.Message}");
            }
        }

        /// <summary>
        /// Marks an already processed folder as outdated (e.g. after <c>split-laps</c> has changed the laps)
        /// so that the next <c>process</c> call fully reprocesses it without <c>--force</c>. Without a status file
        /// nothing happens. Throws if the invalidation cannot be saved (unlike <see cref="Save"/>).
        /// </summary>
        public static void MarkOutdated(string directoryPath, string reason)
        {
            if (Load(directoryPath) is not { } status)
                return;
            status.ProcessingRevision = 0;
            status.SetStep("invalidated", StepStatus.Skipped, new Dictionary<string, object>
            {
                ["reason"] = reason,
                ["atUtc"] = DateTime.UtcNow,
            });
            AtomicFile.WriteAllText(System.IO.Path.Combine(directoryPath, FileName), JsonSerializer.Serialize(status, JsonOptions));
        }

        /// <summary>
        /// Sets the status of a processing step.
        /// </summary>
        public void SetStep(string stepName, string status, Dictionary<string, object>? details = null)
        {
            Steps[stepName] = new StepStatus
            {
                Status = status,
                Details = details
            };
        }

        /// <summary>
        /// Creates a new status instance for the specified path.
        /// </summary>
        public static OracleStatus Create(string targetPath)
        {
            return new OracleStatus
            {
                Path = targetPath,
                ProcessedAtUtc = DateTime.UtcNow,
                ProcessingRevision = CurrentProcessingRevision,
            };
        }

        private static string GetAssemblyVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version?.ToString(3) ?? "0.0.0";
        }
    }

    /// <summary>
    /// Status of a single processing step in the pipeline.
    /// </summary>
    public sealed class StepStatus
    {
        public const string Success = "success";
        public const string Failed = "failed";
        public const string Partial = "partial";
        public const string Skipped = "skipped";

        /// <summary>Status: "success", "failed", "partial", "skipped"</summary>
        public string Status { get; set; } = Success;

        /// <summary>Optional details (e.g. source, channel list, count)</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? Details { get; set; }
    }
}
