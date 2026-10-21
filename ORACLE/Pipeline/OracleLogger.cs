namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Leichtgewichtiges Dual-Output-Logging: Schreibt gleichzeitig auf Console UND in eine Log-Datei.
    /// 
    /// <para><b>Funktionsweise:</b></para>
    /// Wird via <see cref="Console.SetOut"/> als Standard-Output gesetzt. Danach schreiben
    /// alle <c>Console.WriteLine</c>-Aufrufe im gesamten Programm automatisch auch in die Log-Datei —
    /// ohne Änderungen an bestehenden Services (OraclePipeline, ImportDataCalculator, etc.).
    /// 
    /// <para><b>Log-Datei:</b></para>
    /// <c>{AppDir}/logs/oracle-{yyyyMMdd-HHmmss}.log</c>
    /// 
    /// <para><b>Verwendung in Program.cs:</b></para>
    /// <code>
    /// using var logger = OracleLogger.Initialize();
    /// // Ab hier: Console.WriteLine → Console + Datei
    /// </code>
    /// 
    /// <para><b>KRAKEN-Kontext:</b></para>
    /// Wenn ORACLE als EXE-Job von KRAKEN gestartet wird, geht die Console-Ausgabe verloren.
    /// Die Log-Datei bewahrt alle Diagnose-Informationen für spätere Analyse.
    /// </summary>
    public sealed class OracleLogger : TextWriter, IDisposable
    {
        private readonly TextWriter _consoleWriter;
        private readonly StreamWriter _fileWriter;
        private bool _disposed;

        /// <summary>Pfad zur aktuellen Log-Datei</summary>
        public string LogFilePath { get; }

        public override System.Text.Encoding Encoding => _consoleWriter.Encoding;

        private OracleLogger(TextWriter consoleWriter, StreamWriter fileWriter, string logFilePath)
        {
            _consoleWriter = consoleWriter;
            _fileWriter = fileWriter;
            LogFilePath = logFilePath;
        }

        /// <summary>
        /// Initialisiert das Dual-Logging und setzt <see cref="Console.Out"/> um.
        /// Gibt eine <see cref="IDisposable"/>-Instanz zurück, die bei Programmende
        /// die Log-Datei schließt und Console.Out wiederherstellt.
        /// </summary>
        /// <returns>OracleLogger-Instanz (IDisposable)</returns>
        /// <summary>Logdateien älter als diese Anzahl Tage werden beim Start gelöscht (Hintergrundbetrieb ohne Wartung).</summary>
        public const int RetentionDays = 60;

        public static OracleLogger Initialize()
        {
            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            DeleteOldLogs(logDir, TimeSpan.FromDays(RetentionDays));

            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var logFilePath = Path.Combine(logDir, $"oracle-{timestamp}.log");

            var consoleWriter = Console.Out;
            var fileWriter = new StreamWriter(logFilePath, append: false, System.Text.Encoding.UTF8)
            {
                AutoFlush = true,
            };

            var logger = new OracleLogger(consoleWriter, fileWriter, logFilePath);
            Console.SetOut(logger);

            return logger;
        }

        /// <summary>Löscht <c>oracle-*.log</c> älter als <paramref name="maxAge"/>. Non-fatal.</summary>
        public static int DeleteOldLogs(string logDir, TimeSpan maxAge)
        {
            var deleted = 0;
            try
            {
                foreach (var file in Directory.EnumerateFiles(logDir, "oracle-*.log"))
                {
                    if (DateTime.Now - File.GetLastWriteTime(file) <= maxAge)
                        continue;
                    try { File.Delete(file); deleted++; }
                    catch (IOException) { /* in Benutzung – beim nächsten Start */ }
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Aufräumen ist optional.
            }
            return deleted;
        }

        public override void Write(char value)
        {
            _consoleWriter.Write(value);
            _fileWriter.Write(value);
        }

        public override void Write(string? value)
        {
            _consoleWriter.Write(value);
            _fileWriter.Write(value);
        }

        public override void WriteLine(string? value)
        {
            var line = $"[{DateTime.Now:HH:mm:ss.fff}] {value}";
            _consoleWriter.WriteLine(line);
            _fileWriter.WriteLine(line);
        }

        public override void WriteLine()
        {
            _consoleWriter.WriteLine();
            _fileWriter.WriteLine();
        }

        public override void Flush()
        {
            _consoleWriter.Flush();
            _fileWriter.Flush();
        }

        public override async Task FlushAsync()
        {
            await _consoleWriter.FlushAsync();
            await _fileWriter.FlushAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                Console.SetOut(_consoleWriter);
                _fileWriter.Flush();
                _fileWriter.Dispose();
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
