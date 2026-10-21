namespace Ktm.Racing.Oracle
{
    /// <summary>Helpers for WinTAX run folders (<c>Run_&lt;number&gt;</c>).</summary>
    public static class RunFolders
    {
        /// <summary>The run number of a run folder ("Run_1083" → 1083); <c>int.MaxValue</c> when it has none.</summary>
        public static int Number(string runPathOrName)
        {
            var name = Path.GetFileName(Path.TrimEndingDirectorySeparator(runPathOrName));
            var digits = new string(name.SkipWhile(c => !char.IsDigit(c)).TakeWhile(char.IsDigit).ToArray());
            return int.TryParse(digits, out var number) ? number : int.MaxValue;
        }

        /// <summary>Run folders in chronological (numeric) order: Run_999 before Run_1000.</summary>
        public static List<string> InOrder(IEnumerable<string> runPaths) =>
            runPaths.OrderBy(Number).ThenBy(p => p, StringComparer.OrdinalIgnoreCase).ToList();
    }
}
