using Ktm.Racing.Oracle;

/// <summary>
/// ORACLE Entry-Point — Command-basierter Router.
/// 
/// <para><b>Verwendung:</b></para>
/// <code>
/// ORACLE.exe process  &lt;runPath&gt;   [--force]  → Metadata + Properties + Berechnungen
/// ORACLE.exe merge    &lt;riderPath&gt; [--force]  → Session-Merge über alle Runs
/// ORACLE.exe validate &lt;runPath&gt;              → Dateistruktur + Ladbarkeit + Channels
/// ORACLE.exe &lt;runPath&gt;            [--force]  → Legacy (= process + merge, rückwärtskompatibel)
/// </code>
/// 
/// <para><b>Idempotenz:</b> ORACLE erstellt <c>oracle.status.json</c> nach erfolgreicher
/// Verarbeitung. Bei erneutem Aufruf wird der Run übersprungen (Exit-Code 0).
/// Mit <c>--force</c> wird die Prüfung ignoriert und erneut verarbeitet.</para>
/// 
/// <para><b>KRAKEN-Integration:</b> Jeder Command kann als eigener EXE-Job konfiguriert werden:</para>
/// <code>
/// &lt;Arguments&gt;validate %RUN_PATH%&lt;/Arguments&gt;
/// &lt;Arguments&gt;process %RUN_PATH%&lt;/Arguments&gt;
/// &lt;Arguments&gt;merge %RIDER_PATH%&lt;/Arguments&gt;
/// </code>
/// </summary>
class Program
{
    static async Task<int> Main(string[] args)
    {
        // --- Dual-Logging initialisieren (Console + Datei) ---
        using var logger = OracleLogger.Initialize();
        Console.WriteLine($"ORACLE gestartet. Log-Datei: {logger.LogFilePath}");
        Console.WriteLine($"Argumente: {string.Join(" ", args)}");

        if (args.Length == 0)
        {
            PrintUsage();
            return (int)OracleExitCode.InvalidArguments;
        }

        if (CommandLine.Validate(args) is { } argumentError)
        {
            Console.WriteLine($"Fehler: {argumentError}");
            Console.WriteLine("Aufruf ohne Argumente zeigt die Verwendung.");
            Console.WriteLine($"ORACLE beendet. Exit-Code: {(int)OracleExitCode.InvalidArguments} ({OracleExitCode.InvalidArguments})");
            return (int)OracleExitCode.InvalidArguments;
        }

        // sync-motodb braucht KEIN KtmDataHandler/native (kein HASP-Dongle) → vor der Pipeline-Erzeugung behandeln.
        if (args[0].Equals("sync-motodb", StringComparison.OrdinalIgnoreCase))
        {
            var syncCode = MotoDbInputSync.Run(OracleConfig.Load(), GetOption(args, "--kompass"), GetOption(args, "--workbook"));
            Console.WriteLine($"ORACLE beendet. Exit-Code: {(int)syncCode} ({syncCode})");
            return (int)syncCode;
        }

        var pipeline = OraclePipeline.Create();
        OracleExitCode exitCode;

        try
        {
            exitCode = args[0].ToLowerInvariant() switch
            {
                "process" => await HandleProcessAsync(args, pipeline),
                // "merge" temporär deaktiviert – wird separat überarbeitet.
                // "merge" => HandleMerge(args, pipeline),
                "validate" => HandleValidate(args, pipeline),
                "detect" => HandleDetect(args, pipeline),
                "detect-start" => args.Length < 2 ? MissingPath("detect-start <riderPath|runPath>") : pipeline.DetectStarts(args[1]),
                "split-laps" => args.Length < 2
                    ? MissingPath("split-laps <event|session|device|run> [--dry-run] [--undo]")
                    : await pipeline.SplitLapsAsync(args[1], HasFlag(args, "--dry-run"), HasFlag(args, "--undo")),
                _ => await HandleLegacyAsync(args, pipeline),
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ORACLE: Schwerwiegender Fehler: {ex.Message}");
            // Inner-Exception-Kette ausgeben: die eigentliche Ursache (z.B. fehlende Datei) steckt oft ganz innen.
            for (var inner = ex.InnerException; inner != null; inner = inner.InnerException)
                Console.WriteLine($"  --> {inner.GetType().Name}: {inner.Message}");
            Console.WriteLine(ex.StackTrace);
            exitCode = OracleExitCode.GeneralError;
        }

        Console.WriteLine($"ORACLE beendet. Exit-Code: {(int)exitCode} ({exitCode})");
        return (int)exitCode;
    }

    /// <summary>
    /// Command: process — Postprocessing-Pipeline für einen Run.
    /// </summary>
    private static async Task<OracleExitCode> HandleProcessAsync(string[] args, OraclePipeline pipeline)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Fehler: Pfad fehlt.");
            Console.WriteLine("Verwendung: ORACLE.exe process <runPath> [--force] [--gps-split]");
            return OracleExitCode.InvalidArguments;
        }

        var force = args.Any(a => a.Equals("--force", StringComparison.OrdinalIgnoreCase));
        var gpsSplit = args.Any(a => a.Equals("--gps-split", StringComparison.OrdinalIgnoreCase));
        return await pipeline.ProcessPathAsync(args[1], force, gpsSplit);
    }

    /* merge temporär deaktiviert – wird separat überarbeitet.
    /// <summary>
    /// Command: merge — Session-Merge über alle Runs eines Riders.
    /// </summary>
    private static OracleExitCode HandleMerge(string[] args, OraclePipeline pipeline)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Fehler: Pfad fehlt.");
            Console.WriteLine("Verwendung: ORACLE.exe merge <riderPath> [--force]");
            return OracleExitCode.InvalidArguments;
        }

        var force = args.Any(a => a.Equals("--force", StringComparison.OrdinalIgnoreCase));
        return pipeline.MergeSession(args[1], force);
    }
    */

    /// <summary>
    /// Command: validate — Datenintegritätsprüfung.
    /// </summary>
    private static OracleExitCode HandleValidate(string[] args, OraclePipeline pipeline)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Fehler: Pfad fehlt.");
            Console.WriteLine("Verwendung: ORACLE.exe validate <runPath>");
            return OracleExitCode.InvalidArguments;
        }

        return pipeline.ValidateRun(args[1]);
    }

    /// <summary>
    /// Command: detect — Renn-Erkennung (read-only) über alle Runs eines Ordners.
    /// </summary>
    private static OracleExitCode HandleDetect(string[] args, OraclePipeline pipeline)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Fehler: Pfad fehlt.");
            Console.WriteLine("Verwendung: ORACLE.exe detect <riderPath|runPath>");
            return OracleExitCode.InvalidArguments;
        }

        return pipeline.DetectRaces(args[1]);
    }

    /// <summary><c>true</c>, wenn der Schalter <paramref name="flag"/> übergeben wurde.</summary>
    private static bool HasFlag(string[] args, string flag) => args.Any(a => a.Equals(flag, StringComparison.OrdinalIgnoreCase));

    /// <summary>Liest den Wert eines <c>--name value</c>-Arguments; <c>null</c> wenn nicht vorhanden.</summary>
    private static string? GetOption(string[] args, string name)
    {
        for (int i = 0; i < args.Length - 1; i++)
            if (args[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                return args[i + 1];
        return null;
    }

    private static OracleExitCode MissingPath(string usage)
    {
        Console.WriteLine("Fehler: Pfad fehlt.");
        Console.WriteLine($"Verwendung: ORACLE.exe {usage}");
        return OracleExitCode.InvalidArguments;
    }

    /// <summary>
    /// Legacy-Modus: Wenn kein bekannter Command übergeben wird, wird args[0] als
    /// runPath interpretiert und process + merge nacheinander ausgeführt.
    /// Gewährleistet Rückwärtskompatibilität mit bestehender KRAKEN-Konfiguration:
    /// <c>ORACLE.exe %RUN_PATH%</c>
    /// </summary>
    private static async Task<OracleExitCode> HandleLegacyAsync(string[] args, OraclePipeline pipeline)
    {
        var runPath = args[0];
        var force = args.Any(a => a.Equals("--force", StringComparison.OrdinalIgnoreCase));
        var gpsSplit = args.Any(a => a.Equals("--gps-split", StringComparison.OrdinalIgnoreCase));
        Console.WriteLine($"[legacy] Kein Command erkannt — verwende Legacy-Modus für: {runPath}");

        // --- Process (wie "process": inkl. Geräteordner-Normalisierung und Run-Reparatur) ---
        var processResult = await pipeline.ProcessPathAsync(runPath, force, gpsSplit);

        // --- Merge temporär deaktiviert – wird separat überarbeitet. ---
        // if (processResult != OracleExitCode.Success)
        // {
        //     return processResult;
        // }
        //
        // var riderPath = Path.GetFullPath(Path.Combine(runPath, ".."));
        // var mergeResult = pipeline.MergeSession(riderPath, force);
        //
        // if (mergeResult != OracleExitCode.Success)
        // {
        //     Console.WriteLine("[legacy] Session-Merge hatte Probleme, process war aber erfolgreich.");
        //     return OracleExitCode.PartialSuccess;
        // }

        return processResult;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("ORACLE — KTM Offroad Racing Postprocessing");
        Console.WriteLine();
        Console.WriteLine("Verwendung:");
        Console.WriteLine("  ORACLE.exe process  <run|rider> [--force] [--gps-split]  Postprocessing (Rider-Ordner: renn-bewusst, durchgehend)");
        // "merge" temporär deaktiviert – wird separat überarbeitet.
        // Console.WriteLine("  ORACLE.exe merge    <riderPath> [--force]  Session-Merge (alle Runs eines Riders)");
        Console.WriteLine("  ORACLE.exe validate <runPath>                            Datenintegritätsprüfung");
        Console.WriteLine("  ORACLE.exe detect   <riderPath>                          Renn-Erkennung (read-only)");
        Console.WriteLine("  ORACLE.exe detect-start <riderPath|runPath>              Gültige Starts erkennen (read-only)");
        Console.WriteLine("  ORACLE.exe split-laps <event|session|device|run> [--dry-run] [--undo]");
        Console.WriteLine("                                                           Runs per GPS-Ziellinie in Laps teilen (Probelauf/Rückgängig)");
        Console.WriteLine("  ORACLE.exe sync-motodb [--kompass <KOMPASS.json>] [--workbook <loadChannels.xlsx>]  MotoDB-Inputs (CIRCUITS+HED) aus KOMPASS");
        Console.WriteLine("  ORACLE.exe <runPath>            [--force] [--gps-split]  Legacy-Modus (process)");
        Console.WriteLine();
        Console.WriteLine("Optionen:");
        Console.WriteLine("  --force      Idempotenz-Prüfung ignorieren, erneut verarbeiten");
        Console.WriteLine("  --gps-split  Runs vor der Verarbeitung per GPS-Ziellinie in Laps teilen (wie split-laps)");
        Console.WriteLine("  --dry-run    split-laps: nur anzeigen, was geteilt würde");
        Console.WriteLine("  --undo       split-laps: Original-Laps der letzten Teilung wiederherstellen");
        Console.WriteLine();
        Console.WriteLine("Exit-Codes:");
        Console.WriteLine("  0  Erfolg");
        Console.WriteLine("  1  Allgemeiner Fehler");
        Console.WriteLine("  2  Ungültige Argumente");
        Console.WriteLine("  3  Daten nicht gefunden (Retry)");
        Console.WriteLine("  4  Teilweise erfolgreich (z.B. beschädigte Run-Ordner übersprungen)");
        Console.WriteLine("  5  Ressource gesperrt, z.B. loadChannels.xlsx in Excel offen (Retry)");
        Console.WriteLine("  6  Track-Länge fehlt – MotoDB nicht starten (später erneut / Länge in KODE)");
        Console.WriteLine("  7  Ungültige Daten (beschädigter Run, ungültige KOMPASS.json) – kein Retry");
    }
}
