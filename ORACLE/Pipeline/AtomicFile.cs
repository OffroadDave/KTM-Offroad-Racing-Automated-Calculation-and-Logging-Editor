using System.Text;
using System.Xml.Linq;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Atomic file writes: the content goes to a temporary file in the same folder and replaces the target with one
    /// rename, so readers (WinTAX, KRAKEN, OneDrive) never see a half-written file and a failed write leaves the old file
    /// intact. Brief locks by virus scanners, indexers or OneDrive are retried.
    /// </summary>
    public static class AtomicFile
    {
        /// <summary>UTF-8 without byte order mark (the encoding of .HED files and ORACLE's JSON files).</summary>
        public static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        /// <summary>Writes <paramref name="content"/> to <paramref name="path"/> atomically.</summary>
        public static void WriteAllText(string path, string content, Encoding? encoding = null) =>
            Write(path, temp => File.WriteAllText(temp, content, encoding ?? Utf8NoBom));

        /// <summary>Writes <paramref name="lines"/> to <paramref name="path"/> atomically.</summary>
        public static void WriteAllLines(string path, IEnumerable<string> lines, Encoding? encoding = null) =>
            Write(path, temp => File.WriteAllLines(temp, lines, encoding ?? Utf8NoBom));

        /// <summary>Saves <paramref name="document"/> to <paramref name="path"/> atomically.</summary>
        public static void Save(XDocument document, string path)
        {
            ArgumentNullException.ThrowIfNull(document);
            Write(path, document.Save);
        }

        /// <summary>
        /// Runs a file-system operation, retrying briefly when another process (virus scanner, indexer, OneDrive) holds a
        /// freshly written file for a moment. Persistent errors are rethrown after about 1.5 s.
        /// </summary>
        public static void Retry(Action operation)
        {
            ArgumentNullException.ThrowIfNull(operation);
            for (var attempt = 0; ; attempt++)
            {
                try
                {
                    operation();
                    return;
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException && attempt < 4
                                           && ex is not DirectoryNotFoundException and not FileNotFoundException)
                {
                    Thread.Sleep(100 << attempt);
                }
            }
        }

        private static void Write(string path, Action<string> writeTemp)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            var temp = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(path))!, $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");
            try
            {
                writeTemp(temp);
                Retry(() => File.Move(temp, path, overwrite: true));
            }
            finally
            {
                if (File.Exists(temp))
                {
                    try { File.Delete(temp); }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { /* leftover temp file is harmless */ }
                }
            }
        }
    }
}
