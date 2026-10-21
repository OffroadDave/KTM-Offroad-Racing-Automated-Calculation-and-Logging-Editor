namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Lightweight dual-output logging: writes to the console AND a log file at the same time.
    /// 
    /// <para><b>How it works:</b></para>
    /// Set as standard output via <see cref="Console.SetOut"/>. After that,
    /// all <c>Console.WriteLine</c> calls throughout the program automatically also write to the log file —
    /// without changes to existing services (OraclePipeline, ImportDataCalculator, etc.).
    /// 
    /// <para><b>Log file:</b></para>
    /// <c>{AppDir}/logs/oracle-{yyyyMMdd-HHmmss}-{PID}.log</c>
    /// 
    /// <para><b>Usage in Program.cs:</b></para>
    /// <code>
    /// using var logger = OracleLogger.Initialize();
    /// // From here on: Console.WriteLine → console + file
    /// </code>
    /// 
    /// <para><b>KRAKEN context:</b></para>
    /// When ORACLE is started as an EXE job by KRAKEN, the console output is lost.
    /// The log file preserves all diagnostic information for later analysis.
    /// </summary>
    public sealed class OracleLogger : TextWriter, IDisposable
    {
        private readonly TextWriter _consoleWriter;
        private readonly StreamWriter _fileWriter;
        private bool _disposed;

        /// <summary>Path to the current log file</summary>
        public string LogFilePath { get; }

        public override System.Text.Encoding Encoding => _consoleWriter.Encoding;

        private OracleLogger(TextWriter consoleWriter, StreamWriter fileWriter, string logFilePath)
        {
            _consoleWriter = consoleWriter;
            _fileWriter = fileWriter;
            LogFilePath = logFilePath;
        }

        /// <summary>Log files older than this number of days are deleted at startup (unattended background operation).</summary>
        public const int RetentionDays = 60;

        /// <summary>
        /// Initializes dual logging and redirects <see cref="Console.Out"/>.
        /// Returns an <see cref="IDisposable"/> instance that, at program end,
        /// closes the log file and restores Console.Out.
        /// </summary>
        /// <returns>OracleLogger instance (IDisposable)</returns>
        public static OracleLogger Initialize()
        {
            try
            {
                return Initialize(Path.Combine(AppContext.BaseDirectory, "logs"));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Program folder not writable (e.g. under Program Files): fall back to the user temp folder instead of crashing.
                return Initialize(Path.Combine(Path.GetTempPath(), "ORACLE", "logs"));
            }
        }

        /// <summary>Initializes dual logging with a log file in <paramref name="logDir"/>.</summary>
        public static OracleLogger Initialize(string logDir)
        {
            Directory.CreateDirectory(logDir);
            DeleteOldLogs(logDir, TimeSpan.FromDays(RetentionDays));

            // Process ID in the name: two calls within the same second never write to the same file.
            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss", System.Globalization.CultureInfo.InvariantCulture);
            var logFilePath = Path.Combine(logDir, $"oracle-{timestamp}-{Environment.ProcessId}.log");

            var consoleWriter = Console.Out;
            var stream = new FileStream(logFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
            var fileWriter = new StreamWriter(stream, System.Text.Encoding.UTF8)
            {
                AutoFlush = true,
            };

            var logger = new OracleLogger(consoleWriter, fileWriter, logFilePath);
            Console.SetOut(logger);

            return logger;
        }

        /// <summary>Deletes <c>oracle-*.log</c> older than <paramref name="maxAge"/>. Non-fatal.</summary>
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
                    catch (IOException) { /* in use – on next start */ }
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Cleanup is optional.
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
