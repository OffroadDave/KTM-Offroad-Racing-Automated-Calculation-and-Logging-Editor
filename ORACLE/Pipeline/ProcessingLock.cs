namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Exclusive lock of a device/run folder during processing (<c>oracle.lock</c>, held by the operating system).
    /// Prevents two ORACLE processes (e.g. parallel KRAKEN jobs) from working on the same folder at the same time. If
    /// ORACLE crashes, Windows releases the lock; the file is deleted on close.
    /// </summary>
    public sealed class ProcessingLock : IDisposable
    {
        /// <summary>File name of the lock in the folder.</summary>
        public const string FileName = "oracle.lock";

        private readonly FileStream? _stream;

        private ProcessingLock(FileStream? stream) => _stream = stream;

        /// <summary>
        /// Tries to acquire the lock; <c>null</c> if another process is processing the folder.
        /// Short delays (file is just being deleted by a terminated process) are waited out.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">The folder does not exist.</exception>
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
                    // Folder not writable: continue without lock (write errors will surface concretely later).
                    Console.WriteLine($"Warning: {folder} is not writable – processing without lock.");
                    return new ProcessingLock(null);
                }
            }
        }

        /// <inheritdoc />
        public void Dispose() => _stream?.Dispose();
    }
}
