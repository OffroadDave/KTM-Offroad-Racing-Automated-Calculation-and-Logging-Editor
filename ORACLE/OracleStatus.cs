using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ORACLE
{
    /// <summary>
    /// Status-Datei für Idempotenz: Wird nach erfolgreicher Verarbeitung im Run-Ordner
    /// als <c>oracle.status.json</c> gespeichert.
    /// 
    /// <para><b>Idempotenz-Logik:</b></para>
    /// <list type="bullet">
    ///   <item>Status existiert + ExitCode == 0 → Run bereits verarbeitet, überspringen</item>
    ///   <item>Status existiert + ExitCode != 0 → Vorherige Verarbeitung fehlgeschlagen, wiederholen</item>
    ///   <item>Status existiert nicht → Normal verarbeiten</item>
    ///   <item><c>--force</c> Flag → Immer verarbeiten, Status ignorieren</item>
    /// </list>
    /// 
    /// <para><b>Dateistruktur:</b></para>
    /// <code>
    /// Run_205/
    /// ├── Lap_001/
    /// ├── download.done          ← SENTINEL
    /// └── oracle.status.json     ← ORACLE (dieses Modell)
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

        /// <summary>Schema-Version der Status-Datei</summary>
        public string Version { get; set; } = "1.0";

        /// <summary>Verarbeiteter Run-/Rider-Pfad</summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>Zeitpunkt der Verarbeitung (UTC)</summary>
        public DateTime ProcessedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>ORACLE-Version (aus Assembly)</summary>
        public string OracleVersion { get; set; } = GetAssemblyVersion();

        /// <summary>Status der einzelnen Verarbeitungsschritte</summary>
        public Dictionary<string, StepStatus> Steps { get; set; } = [];

        /// <summary>Finaler Exit-Code</summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// Prüft ob der angegebene Pfad bereits erfolgreich verarbeitet wurde.
        /// </summary>
        /// <param name="targetPath">Run- oder Rider-Pfad</param>
        /// <returns>True wenn Status existiert und ExitCode == 0</returns>
        public static bool IsAlreadyProcessed(string targetPath)
        {
            var status = Load(targetPath);
            return status is not null && status.ExitCode == 0;
        }

        /// <summary>
        /// Lädt die Status-Datei aus dem angegebenen Verzeichnis.
        /// </summary>
        /// <returns>OracleStatus bei Erfolg, null wenn keine Status-Datei existiert oder Parse-Fehler</returns>
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
                Console.WriteLine($"Warnung: Status-Datei konnte nicht geladen werden: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Speichert die Status-Datei im angegebenen Verzeichnis.
        /// </summary>
        public void Save(string directoryPath)
        {
            var filePath = System.IO.Path.Combine(directoryPath, FileName);
            var json = JsonSerializer.Serialize(this, JsonOptions);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Setzt den Status eines Verarbeitungsschritts.
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
        /// Erstellt eine neue Status-Instanz für den angegebenen Pfad.
        /// </summary>
        public static OracleStatus Create(string targetPath)
        {
            return new OracleStatus
            {
                Path = targetPath,
                ProcessedAtUtc = DateTime.UtcNow,
            };
        }

        private static string GetAssemblyVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version?.ToString(3) ?? "0.0.0";
        }
    }

    /// <summary>
    /// Status eines einzelnen Verarbeitungsschritts in der Pipeline.
    /// </summary>
    public sealed class StepStatus
    {
        public const string Success = "success";
        public const string Failed = "failed";
        public const string Partial = "partial";
        public const string Skipped = "skipped";

        /// <summary>Status: "success", "failed", "partial", "skipped"</summary>
        public string Status { get; set; } = Success;

        /// <summary>Optionale Detailinformationen (z.B. Quelle, Channel-Liste, Anzahl)</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? Details { get; set; }
    }
}
