namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Interprets the error tuples returned by the KtmDataHandler write API (<c>AddImportDataToRun</c>,
    /// <c>SetChannelsSplitByLap</c>): one entry per written unit with error counts in <c>Item1</c>/<c>Item3</c> and a message
    /// in <c>Item4</c> (<c>"No errors."</c> when fine). The native message may carry garbage after the text.
    /// </summary>
    public static class KtmWriteErrors
    {
        /// <summary>Human-readable descriptions of all entries that report an error (empty when everything was written).</summary>
        /// <param name="results">Return value of the write call (may be null).</param>
        /// <param name="unit">Name of one entry for the message, e.g. "lap" or "entry".</param>
        public static IReadOnlyList<string> Describe(IEnumerable<(uint, uint, uint, string)>? results, string unit = "entry") =>
            (results ?? [])
                .Select((r, i) => (r, i))
                .Where(x => x.r.Item1 != 0 || x.r.Item3 != 0)
                .Select(x => $"{unit} {x.i + 1}: {Clean(x.r.Item4)} ({x.r.Item3} channel(s))")
                .ToList();

        /// <summary>Keeps the printable part of a native message.</summary>
        public static string Clean(string? message)
        {
            if (string.IsNullOrEmpty(message))
                return "unknown error";
            var end = 0;
            while (end < message.Length && message[end] >= ' ' && message[end] < 0x7F)
                end++;
            var text = message[..end].Trim();
            return text.Length == 0 ? "unknown error" : text;
        }
    }
}
