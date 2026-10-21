using Ktm.Racing.Oracle;

/// <summary>
/// ORACLE entry point — command-based router.
/// 
/// <para><b>Usage:</b></para>
/// <code>
/// ORACLE.exe process  &lt;runPath&gt;   [--force]  → metadata + properties + calculations
/// ORACLE.exe validate &lt;runPath&gt;              → file structure + loadability + channels
/// ORACLE.exe &lt;runPath&gt;            [--force]  → legacy (= process, backward compatible)
/// </code>
/// 
/// <para><b>Idempotency:</b> ORACLE creates <c>oracle.status.json</c> after successful
/// processing. On a repeated call the run is skipped (exit code 0).
/// With <c>--force</c> the check is ignored and the run is processed again.</para>
/// 
/// <para><b>KRAKEN integration:</b> each command can be configured as a separate EXE job:</para>
/// <code>
/// &lt;Arguments&gt;validate %RUN_PATH%&lt;/Arguments&gt;
/// &lt;Arguments&gt;process %RUN_PATH%&lt;/Arguments&gt;
/// </code>
/// </summary>
class Program
{
    static async Task<int> Main(string[] args)
    {
        // --- Initialize dual logging (console + file) ---
        using var logger = OracleLogger.Initialize();
        Console.WriteLine($"ORACLE started. Log file: {logger.LogFilePath}");
        Console.WriteLine($"Arguments: {string.Join(" ", args)}");

        if (args.Length == 0)
        {
            PrintUsage();
            return (int)OracleExitCode.InvalidArguments;
        }

        if (CommandLine.Validate(args) is { } argumentError)
        {
            Console.WriteLine($"Error: {argumentError}");
            Console.WriteLine("Run without arguments to show usage.");
            Console.WriteLine($"ORACLE finished. Exit code: {(int)OracleExitCode.InvalidArguments} ({OracleExitCode.InvalidArguments})");
            return (int)OracleExitCode.InvalidArguments;
        }

        OracleExitCode exitCode;

        try
        {
            // sync-motodb needs NO KtmDataHandler/native (no HASP dongle) → handle before creating the pipeline.
            if (args[0].Equals("sync-motodb", StringComparison.OrdinalIgnoreCase))
            {
                exitCode = MotoDbInputSync.Run(OracleConfig.Load(), GetOption(args, "--kompass"), GetOption(args, "--workbook"));
            }
            else
            {
                // Configuration and native API (dongle, runtime) inside the try: errors end up in the log with a defined exit code.
                var pipeline = OraclePipeline.Create();
                exitCode = args[0].ToLowerInvariant() switch
                {
                    "process" => await HandleProcessAsync(args, pipeline),
                    "validate" => HandleValidate(args, pipeline),
                    "detect" => HandleDetect(args, pipeline),
                    "detect-start" => args.Length < 2 ? MissingPath("detect-start <riderPath|runPath>") : pipeline.DetectStarts(args[1]),
                    "split-laps" => args.Length < 2
                        ? MissingPath("split-laps <event|session|device|run> [--dry-run] [--undo]")
                        : await pipeline.SplitLapsAsync(args[1], HasFlag(args, "--dry-run"), HasFlag(args, "--undo")),
                    _ => await HandleLegacyAsync(args, pipeline),
                };
            }
        }
        catch (InvalidDataException ex) when (ex.Message.StartsWith("oracle-config.json", StringComparison.Ordinal))
        {
            Console.WriteLine($"ORACLE: {ex.Message} – please fix oracle-config.json.");
            exitCode = OracleExitCode.InvalidData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ORACLE: Fatal error: {ex.Message}");
            // Print the inner exception chain: the actual cause (e.g. missing file) is often the innermost one.
            for (var inner = ex.InnerException; inner != null; inner = inner.InnerException)
                Console.WriteLine($"  --> {inner.GetType().Name}: {inner.Message}");
            Console.WriteLine(ex.StackTrace);
            exitCode = OracleExitCode.GeneralError;
        }

        Console.WriteLine($"ORACLE finished. Exit code: {(int)exitCode} ({exitCode})");
        return (int)exitCode;
    }

    /// <summary>
    /// Command: process — postprocessing pipeline for a run.
    /// </summary>
    private static async Task<OracleExitCode> HandleProcessAsync(string[] args, OraclePipeline pipeline)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: path missing.");
            Console.WriteLine("Usage: ORACLE.exe process <runPath> [--force] [--gps-split]");
            return OracleExitCode.InvalidArguments;
        }

        var force = args.Any(a => a.Equals("--force", StringComparison.OrdinalIgnoreCase));
        var gpsSplit = args.Any(a => a.Equals("--gps-split", StringComparison.OrdinalIgnoreCase));
        return await pipeline.ProcessPathAsync(args[1], force, gpsSplit);
    }

    /// <summary>
    /// Command: validate — data integrity check.
    /// </summary>
    private static OracleExitCode HandleValidate(string[] args, OraclePipeline pipeline)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: path missing.");
            Console.WriteLine("Usage: ORACLE.exe validate <runPath>");
            return OracleExitCode.InvalidArguments;
        }

        return pipeline.ValidateRun(args[1]);
    }

    /// <summary>
    /// Command: detect — race detection (read-only) across all runs of a folder.
    /// </summary>
    private static OracleExitCode HandleDetect(string[] args, OraclePipeline pipeline)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: path missing.");
            Console.WriteLine("Usage: ORACLE.exe detect <riderPath|runPath>");
            return OracleExitCode.InvalidArguments;
        }

        return pipeline.DetectRaces(args[1]);
    }

    /// <summary><c>true</c> if the flag <paramref name="flag"/> was passed.</summary>
    private static bool HasFlag(string[] args, string flag) => args.Any(a => a.Equals(flag, StringComparison.OrdinalIgnoreCase));

    /// <summary>Reads the value of a <c>--name value</c> argument; <c>null</c> if not present.</summary>
    private static string? GetOption(string[] args, string name)
    {
        for (int i = 0; i < args.Length - 1; i++)
            if (args[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                return args[i + 1];
        return null;
    }

    private static OracleExitCode MissingPath(string usage)
    {
        Console.WriteLine("Error: path missing.");
        Console.WriteLine($"Usage: ORACLE.exe {usage}");
        return OracleExitCode.InvalidArguments;
    }

    /// <summary>
    /// Legacy mode: if no known command is passed, args[0] is interpreted as
    /// runPath and process is executed.
    /// Ensures backward compatibility with the existing KRAKEN configuration:
    /// <c>ORACLE.exe %RUN_PATH%</c>
    /// </summary>
    private static async Task<OracleExitCode> HandleLegacyAsync(string[] args, OraclePipeline pipeline)
    {
        var runPath = args[0];
        var force = args.Any(a => a.Equals("--force", StringComparison.OrdinalIgnoreCase));
        var gpsSplit = args.Any(a => a.Equals("--gps-split", StringComparison.OrdinalIgnoreCase));
        Console.WriteLine($"[legacy] No command recognized — using legacy mode for: {runPath}");

        // --- Process (like "process": incl. device folder normalization and run repair) ---
        var processResult = await pipeline.ProcessPathAsync(runPath, force, gpsSplit);


        return processResult;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("ORACLE — KTM Offroad Racing Postprocessing");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  ORACLE.exe process  <run|rider> [--force] [--gps-split]  Postprocessing (rider folder: race-aware, continuous)");
        Console.WriteLine("  ORACLE.exe validate <runPath>                            Data integrity check");
        Console.WriteLine("  ORACLE.exe detect   <riderPath>                          Race detection (read-only)");
        Console.WriteLine("  ORACLE.exe detect-start <riderPath|runPath>              Detect valid starts (read-only)");
        Console.WriteLine("  ORACLE.exe split-laps <event|session|device|run> [--dry-run] [--undo]");
        Console.WriteLine("                                                           Split runs into laps at the GPS finish line (dry run/undo)");
        Console.WriteLine("  ORACLE.exe sync-motodb [--kompass <KOMPASS.json>] [--workbook <loadChannels.xlsx>]  MotoDB inputs (CIRCUITS+HED) from KOMPASS");
        Console.WriteLine("  ORACLE.exe <runPath>            [--force] [--gps-split]  Legacy mode (process)");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --force      Ignore idempotency check, process again");
        Console.WriteLine("  --gps-split  Split runs into laps at the GPS finish line before processing (like split-laps)");
        Console.WriteLine("  --dry-run    split-laps: only show what would be split");
        Console.WriteLine("  --undo       split-laps: restore the original laps of the last split");
        Console.WriteLine();
        Console.WriteLine("Exit codes:");
        Console.WriteLine("  0  Success");
        Console.WriteLine("  1  General error");
        Console.WriteLine("  2  Invalid arguments");
        Console.WriteLine("  3  Data not found (retry)");
        Console.WriteLine("  4  Partially successful (e.g. corrupt run folders skipped)");
        Console.WriteLine("  5  Resource locked, e.g. loadChannels.xlsx open in Excel (retry)");
        Console.WriteLine("  6  Track length missing – do not start MotoDB (retry later / enter length in KODE)");
        Console.WriteLine("  7  Invalid data (corrupt run, invalid KOMPASS.json) – no retry");
    }
}
