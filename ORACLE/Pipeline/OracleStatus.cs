using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ktm.Racing.Oracle
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

        // Assembly-Version einmal per Reflection ermitteln und cachen (konstant zur Laufzeit).
        private static readonly string CachedVersion = GetAssemblyVersion();

        /// <summary>Schema-Version der Status-Datei</summary>
        public string Version { get; set; } = "1.0";

        /// <summary>Verarbeiteter Run-/Rider-Pfad</summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>Zeitpunkt der Verarbeitung (UTC)</summary>
        public DateTime ProcessedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>ORACLE-Version (aus Assembly)</summary>
        public string OracleVersion { get; set; } = CachedVersion;

        /// <summary>Status der einzelnen Verarbeitungsschritte</summary>
        public Dictionary<string, StepStatus> Steps { get; set; } = [];

        /// <summary>Finaler Exit-Code</summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// Revision der Verarbeitungslogik, mit der der Status geschrieben wurde (0 = vor Einführung). Ist sie kleiner als
        /// <see cref="CurrentProcessingRevision"/>, wird der Ordner automatisch neu verarbeitet – so profitieren bereits
        /// verarbeitete Sessions von Korrekturen (z.B. Metadaten, Run-Reparatur), ohne <c>--force</c>.
        /// </summary>
        public int ProcessingRevision { get; set; }

        /// <summary>
        /// Aktuelle Revision der Verarbeitungslogik. Erhöhen, wenn eine Änderung bereits verarbeitete Daten verbessert.
        /// 2 = Metadaten per Muster, Geräte getrennt, Run-Reparatur, leere Runs, Längen-Median (2026-10-12).
        /// 3 = KOMPASS-Stammdaten ersetzen abweichende .HED-Werte (z.B. riderName-Platzhalter) (2026-10-19).
        /// </summary>
        public const int CurrentProcessingRevision = 3;

        /// <summary>Verarbeitungszustand eines Ordners laut Status-Datei.</summary>
        public enum State
        {
            /// <summary>Kein (lesbarer) Status oder fehlgeschlagen – vollständig verarbeiten.</summary>
            NotProcessed,

            /// <summary>Mit älterer Verarbeitungslogik verarbeitet – vollständig neu verarbeiten.</summary>
            Outdated,

            /// <summary>Verarbeitet; nur die Track-Länge fehlte noch (Exit 5/6) – nur die Länge nachholen.</summary>
            LengthPending,

            /// <summary>Erfolgreich verarbeitet (Exit 0, oder 4 nur wegen beschädigter Runs).</summary>
            Done,
        }

        /// <summary>Ermittelt den Verarbeitungszustand des Ordners.</summary>
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
        /// Prüft ob der angegebene Pfad bereits verarbeitet wurde (<see cref="State.Done"/> oder <see cref="State.LengthPending"/>).
        /// </summary>
        public static bool IsAlreadyProcessed(string targetPath)
        {
            var state = GetState(targetPath);
            if (state == State.Outdated)
                Console.WriteLine($"[status] Mit älterer ORACLE-Verarbeitungslogik verarbeitet (Revision < {CurrentProcessingRevision}) – wird neu verarbeitet.");
            return state is State.Done or State.LengthPending;
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
        /// Speichert die Status-Datei im angegebenen Verzeichnis. Non-fatal: Ist die Datei gesperrt
        /// (z.B. durch einen Editor/Watcher), wird eine Warnung geloggt statt zu werfen — eine
        /// erfolgreiche Verarbeitung soll nicht wegen eines Schreibfehlers als Fehler erscheinen.
        /// Ohne Status-Datei wird der Pfad beim nächsten Lauf erneut verarbeitet.
        /// </summary>
        public void Save(string directoryPath)
        {
            var filePath = System.IO.Path.Combine(directoryPath, FileName);
            try
            {
                var json = JsonSerializer.Serialize(this, JsonOptions);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warnung: Status-Datei konnte nicht gespeichert werden ({filePath}): {ex.Message}");
            }
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
