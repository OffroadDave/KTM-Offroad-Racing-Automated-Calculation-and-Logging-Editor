namespace Ktm.Racing.Oracle
{
    /// <summary>Prüfung der Kommandozeilen-Optionen: unbekannte Optionen werden abgelehnt statt ignoriert.</summary>
    public static class CommandLine
    {
        /// <summary>Erlaubte Optionen je Kommando: Schalter und Optionen mit Wert.</summary>
        public static (string[] Flags, string[] ValueOptions, int FirstOptionIndex) OptionsFor(string command) =>
            command.ToLowerInvariant() switch
            {
                "sync-motodb" => ([], ["--kompass", "--workbook"], 1),
                "process" => (["--force", "--gps-split"], [], 2),
                "split-laps" => (["--dry-run", "--undo"], [], 2),
                "validate" or "detect" or "detect-start" => ([], [], 2),
                _ => (["--force", "--gps-split"], [], 1), // Legacy: ORACLE.exe <runPath> [--force] [--gps-split]
            };

        /// <summary>Fehlermeldung zu ungültigen Argumenten, oder <c>null</c>, wenn alle Argumente zulässig sind.</summary>
        public static string? Validate(string[] args)
        {
            if (args.Length == 0)
                return null;

            var (flags, valueOptions, first) = OptionsFor(args[0]);
            for (var i = first; i < args.Length; i++)
            {
                var arg = args[i];
                if (flags.Contains(arg, StringComparer.OrdinalIgnoreCase))
                    continue;
                if (valueOptions.Contains(arg, StringComparer.OrdinalIgnoreCase))
                {
                    if (i + 1 >= args.Length || args[i + 1].StartsWith("--", StringComparison.Ordinal))
                        return $"Option {arg} erwartet einen Wert.";
                    i++;
                    continue;
                }

                return arg.StartsWith("--", StringComparison.Ordinal)
                    ? $"Unbekannte Option: {arg}"
                    : $"Unerwartetes Argument: {arg} (Pfade mit Leerzeichen in Anführungszeichen setzen).";
            }

            return null;
        }
    }
}
