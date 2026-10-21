using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ktm.Racing.Oracle;

/// <summary>
/// Makes the KtmDataHandler runtime dependencies discoverable without having to copy the many
/// companion files (log4net, Newtonsoft.Json, Microsoft.Bcl.AsyncInterfaces, the native
/// KtmSharedUtilsCpp/TelDataZTX/HASP license DLLs) into the build output.
///
/// Cause: only KtmDataHandler.dll and KtmSharedUtils.dll are referenced; their
/// runtime dependencies reside exclusively in the "KTM Utils Software Suite" folder.
/// Without this bootstrap, the Autofac activation of MarelliDataAccess fails
/// (e.g. MarelliDataXmlProcessingConstants) or a companion assembly is not found.
/// </summary>
public static class KtmRuntime
{
    private static readonly object _lock = new();
    // volatile: guarantees that the double-checked-lock read outside the lock sees the write.
    private static volatile bool _initialized;

    /// <summary>
    /// Path to the KTM Utils Software Suite folder. Selection order: environment variable KTM_UTILS_SUITE,
    /// then a "KtmSuite" directory shipped next to the app, otherwise the default installation path.
    /// Can also be set directly before the first KtmDataHandler access.
    /// </summary>
    public static string SuiteDirectory { get; set; } = ResolveDefaultSuiteDirectory();

    // Priority: KTM_UTILS_SUITE → "KtmSuite" folder next to the exe (shipped) → default installation path.
    private static string ResolveDefaultSuiteDirectory()
    {
        var env = Environment.GetEnvironmentVariable("KTM_UTILS_SUITE");
        if (!string.IsNullOrWhiteSpace(env)) return env;

        var bundled = Path.Combine(AppContext.BaseDirectory, "KtmSuite");
        if (Directory.Exists(bundled)) return bundled;

        return @"C:\KTM Tools\KTM Utils Software Suite";
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetDllDirectory(string lpPathName);

    // Runs when the core module is loaded (before the first access to KtmDataHandler types),
    // so that the assembly resolver is ready before KtmDataHandler.dll is resolved.
#pragma warning disable CA2255 // ModuleInitializer is chosen deliberately here (integration bootstrap), not a source generator.
    [ModuleInitializer]
    internal static void ModuleInit() => EnsureInitialized();
#pragma warning restore CA2255

    /// <summary>
    /// Activates the assembly and native DLL resolution from the suite folder once (thread-safe).
    /// </summary>
    public static void EnsureInitialized()
    {
        if (_initialized) return;
        lock (_lock)
        {
            if (_initialized) return;

            var dir = SuiteDirectory;
            if (Directory.Exists(dir))
            {
                // Make native DLLs (KtmSharedUtilsCpp, TelDataZTXx64, HASP stack) discoverable via the OS loader.
                if (!SetDllDirectory(dir))
                    Console.Error.WriteLine($"Warning: SetDllDirectory failed for {dir} (error {Marshal.GetLastWin32Error()}).");
                var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                if (!path.Split(';').Any(p => string.Equals(p.TrimEnd('\\'), dir.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase)))
                    Environment.SetEnvironmentVariable("PATH", dir + ";" + path);

                // Load managed companion assemblies from the suite folder on demand.
                AppDomain.CurrentDomain.AssemblyResolve += ResolveFromSuite;
            }
            else
            {
                // Report early and clearly: otherwise only a hard-to-interpret FileNotFoundException follows later.
                Console.Error.WriteLine($"Warning: KTM Utils Software Suite not found at {dir} (set KTM_UTILS_SUITE). " +
                                        "Commands that read WinTAX data will fail.");
            }

            _initialized = true;
        }
    }

    private static readonly Dictionary<string, Assembly> _cache = new(StringComparer.OrdinalIgnoreCase);

    private static Assembly? ResolveFromSuite(object? sender, ResolveEventArgs args)
    {
        var simpleName = new AssemblyName(args.Name).Name;
        if (string.IsNullOrEmpty(simpleName)) return null;

        lock (_cache)
        {
            if (_cache.TryGetValue(simpleName, out var cached)) return cached;

            var candidate = Path.Combine(SuiteDirectory, simpleName + ".dll");
            if (!File.Exists(candidate)) return null;

            // Deliberately ignore the version: KtmDataHandler sometimes requests exact old versions,
            // the suite provides the actually compatible DLL.
            var asm = Assembly.LoadFrom(candidate);
            _cache[simpleName] = asm;
            return asm;
        }
    }
}
