namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Exklusive Sperre eines Geräte-/Run-Ordners während der Verarbeitung (<c>oracle.lock</c>, vom Betriebssystem gehalten).
    /// Verhindert, dass zwei ORACLE-Prozesse (z.B. parallele KRAKEN-Jobs) denselben Ordner gleichzeitig bearbeiten. Stürzt
    /// ORACLE ab, gibt Windows die Sperre frei; die Datei wird beim Schließen gelöscht.
    /// </summary>
    public sealed class ProcessingLock : IDisposable
    {
        /// <summary>Dateiname der Sperre im Ordner.</summary>
        public const string FileName = "oracle.lock";

        private readonly FileStream? _stream;

        private ProcessingLock(FileStream? stream) => _stream = stream;

        /// <summary>
        /// Versucht die Sperre zu erhalten; <c>null</c>, wenn ein anderer Prozess den Ordner verarbeitet.
        /// Kurze Verzögerungen (Datei wird gerade von einem beendeten Prozess gelöscht) werden abgewartet.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">Der Ordner existiert nicht.</exception>
        public static ProcessingLock? TryAcquire(string folder)
        {
            if (!Directory.Exists(folder))
                throw new DirectoryNotFoundException($"Folder not found: {folder}");

            for (var attempt = 0; ; attempt++)
            {
                try
                {
                    var stream = new FileStream(Path.Combine(folder, FileName), FileMode.OpenOrCreate, FileAccess.ReadWrite,
                        FileShare.None, 1, FileOptions.DeleteOnClose);
                    using (var writer = new StreamWriter(stream, leaveOpen: true))
                        writer.Write($"{Environment.MachineName} pid {Environment.ProcessId} {DateTimeOffset.Now:O}");
                    return new ProcessingLock(stream);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException && attempt < 3)
                {
                    Thread.Sleep(200);
                }
                catch (IOException)
                {
                    return null;
                }
                catch (UnauthorizedAccessException)
                {
                    // Ordner nicht beschreibbar: ohne Sperre weiter (Schreibfehler zeigen sich später konkret).
                    Console.WriteLine($"Warnung: {folder} ist nicht beschreibbar – Verarbeitung ohne Sperre.");
                    return new ProcessingLock(null);
                }
            }
        }

        /// <inheritdoc />
        public void Dispose() => _stream?.Dispose();
    }
}
