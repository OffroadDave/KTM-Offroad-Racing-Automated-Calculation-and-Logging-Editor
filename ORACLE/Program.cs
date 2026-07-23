using ORACLE;

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

        var pipeline = OraclePipeline.Create();
        OracleExitCode exitCode;

        try
        {
            exitCode = args[0].ToLowerInvariant() switch
            {
                "process" => await HandleProcessAsync(args, pipeline),
                "merge" => HandleMerge(args, pipeline),
                "validate" => HandleValidate(args, pipeline),
                _ => await HandleLegacyAsync(args, pipeline),
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ORACLE: Schwerwiegender Fehler: {ex.Message}");
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
            Console.WriteLine("Verwendung: ORACLE.exe process <runPath> [--force]");
            return OracleExitCode.InvalidArguments;
        }

        var force = args.Any(a => a.Equals("--force", StringComparison.OrdinalIgnoreCase));
        return await pipeline.ProcessRunAsync(args[1], force);
    }

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
    /// Legacy-Modus: Wenn kein bekannter Command übergeben wird, wird args[0] als
    /// runPath interpretiert und process + merge nacheinander ausgeführt.
    /// Gewährleistet Rückwärtskompatibilität mit bestehender KRAKEN-Konfiguration:
    /// <c>ORACLE.exe %RUN_PATH%</c>
    /// </summary>
    private static async Task<OracleExitCode> HandleLegacyAsync(string[] args, OraclePipeline pipeline)
    {
        var runPath = args[0];
        var force = args.Any(a => a.Equals("--force", StringComparison.OrdinalIgnoreCase));
        Console.WriteLine($"[legacy] Kein Command erkannt — verwende Legacy-Modus für: {runPath}");

        // --- Process ---
        var processResult = await pipeline.ProcessRunAsync(runPath, force);
        if (processResult != OracleExitCode.Success)
        {
            return processResult;
        }

        // --- Merge (Rider-Ordner = eine Ebene über Run) ---
        var riderPath = Path.GetFullPath(Path.Combine(runPath, ".."));
        var mergeResult = pipeline.MergeSession(riderPath, force);

        // Merge-Fehler sind nicht kritisch im Legacy-Modus
        if (mergeResult != OracleExitCode.Success)
        {
            Console.WriteLine("[legacy] Session-Merge hatte Probleme, process war aber erfolgreich.");
            return OracleExitCode.PartialSuccess;
        }

        return OracleExitCode.Success;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("ORACLE — KTM Offroad Racing Postprocessing");
        Console.WriteLine();
        Console.WriteLine("Verwendung:");
        Console.WriteLine("  ORACLE.exe process  <runPath>   [--force]  Postprocessing-Pipeline für einen Run");
        Console.WriteLine("  ORACLE.exe merge    <riderPath> [--force]  Session-Merge (alle Runs eines Riders)");
        Console.WriteLine("  ORACLE.exe validate <runPath>              Datenintegritätsprüfung");
        Console.WriteLine("  ORACLE.exe <runPath>            [--force]  Legacy-Modus (process + merge)");
        Console.WriteLine();
        Console.WriteLine("Optionen:");
        Console.WriteLine("  --force  Idempotenz-Prüfung ignorieren, erneut verarbeiten");
        Console.WriteLine();
        Console.WriteLine("Exit-Codes:");
        Console.WriteLine("  0  Erfolg");
        Console.WriteLine("  1  Allgemeiner Fehler");
        Console.WriteLine("  2  Ungültige Argumente");
        Console.WriteLine("  3  Daten nicht gefunden");
        Console.WriteLine("  4  Teilweise erfolgreich");
    }
}
