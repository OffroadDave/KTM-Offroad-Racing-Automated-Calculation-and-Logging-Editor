namespace Ktm.Racing.Oracle
{
    /// <summary>Validation of command-line options: unknown options are rejected instead of ignored.</summary>
    public static class CommandLine
    {
        /// <summary>Allowed options per command: flags and options with a value.</summary>
        public static (string[] Flags, string[] ValueOptions, int FirstOptionIndex) OptionsFor(string command) =>
            command.ToLowerInvariant() switch
            {
                "sync-motodb" => ([], ["--kompass", "--workbook"], 1),
                "process" => (["--force", "--gps-split"], [], 2),
                "split-laps" => (["--dry-run", "--undo"], [], 2),
                "weather" => (["--force"], [], 2),
                "validate" or "detect" or "detect-start" => ([], [], 2),
                _ => (["--force", "--gps-split"], [], 1), // Legacy: ORACLE.exe <runPath> [--force] [--gps-split]
            };

        /// <summary>Error message for invalid arguments, or <c>null</c> if all arguments are valid.</summary>
        public static string? Validate(string[] args)
        {
            if (args.Length == 0)
                return null;
            if (args[0].Equals("merge", StringComparison.OrdinalIgnoreCase))
                return "The 'merge' command has been removed.";

            var (flags, valueOptions, first) = OptionsFor(args[0]);
            for (var i = first; i < args.Length; i++)
            {
                var arg = args[i];
                if (flags.Contains(arg, StringComparer.OrdinalIgnoreCase))
                    continue;
                if (valueOptions.Contains(arg, StringComparer.OrdinalIgnoreCase))
                {
                    if (i + 1 >= args.Length || args[i + 1].StartsWith("--", StringComparison.Ordinal))
                        return $"Option {arg} expects a value.";
                    i++;
                    continue;
                }

                return arg.StartsWith("--", StringComparison.Ordinal)
                    ? $"Unknown option: {arg}"
                    : $"Unexpected argument: {arg} (enclose paths containing spaces in quotes).";
            }

            return null;
        }
    }
}
