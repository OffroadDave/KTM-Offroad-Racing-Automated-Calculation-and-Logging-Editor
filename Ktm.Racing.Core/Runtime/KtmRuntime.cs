using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ktm.Racing.Core;

/// <summary>
/// Macht die KtmDataHandler-Laufzeitabhängigkeiten auffindbar, ohne dass die vielen
/// Companion-Dateien (log4net, Newtonsoft.Json, Microsoft.Bcl.AsyncInterfaces, die nativen
/// KtmSharedUtilsCpp/TelDataZTX/HASP-Lizenz-DLLs) ins Build-Output kopiert werden müssen.
///
/// Ursache: Referenziert werden nur KtmDataHandler.dll und KtmSharedUtils.dll; deren
/// Laufzeit-Abhängigkeiten liegen ausschließlich im "KTM Utils Software Suite"-Ordner.
/// Ohne diesen Bootstrap schlägt die Autofac-Aktivierung von MarelliDataAccess fehl
/// (z.B. MarelliDataXmlProcessingConstants) bzw. eine Companion-Assembly wird nicht gefunden.
/// </summary>
public static class KtmRuntime
{
    private static readonly object _lock = new();
    private static bool _initialized;

    /// <summary>
    /// Pfad zum KTM Utils Software Suite-Ordner. Überschreibbar via Umgebungsvariable
    /// KTM_UTILS_SUITE oder direkt vor dem ersten KtmDataHandler-Zugriff.
    /// </summary>
    public static string SuiteDirectory { get; set; } =
        Environment.GetEnvironmentVariable("KTM_UTILS_SUITE")
        ?? @"C:\KTM Tools\KTM Utils Software Suite";

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetDllDirectory(string lpPathName);

    // Läuft beim Laden des Core-Moduls (vor dem ersten Zugriff auf KtmDataHandler-Typen),
    // damit der Assembly-Resolver bereitsteht, bevor KtmDataHandler.dll aufgelöst wird.
#pragma warning disable CA2255 // ModuleInitializer ist hier bewusst gewählt (Integrations-Bootstrap), kein Source-Generator.
    [ModuleInitializer]
    internal static void ModuleInit() => EnsureInitialized();
#pragma warning restore CA2255

    /// <summary>
    /// Einmalig (thread-safe) die Assembly- und Native-DLL-Auflösung aus dem Suite-Ordner aktivieren.
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
                // Native DLLs (KtmSharedUtilsCpp, TelDataZTXx64, HASP-Stack) über OS-Loader auffindbar machen.
                SetDllDirectory(dir);
                var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                if (!path.Split(';').Any(p => string.Equals(p.TrimEnd('\\'), dir.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase)))
                    Environment.SetEnvironmentVariable("PATH", dir + ";" + path);

                // Verwaltete Companion-Assemblies bei Bedarf aus dem Suite-Ordner nachladen.
                AppDomain.CurrentDomain.AssemblyResolve += ResolveFromSuite;
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

            // Version bewusst ignorieren: KtmDataHandler fordert teils exakte Alt-Versionen an,
            // die Suite liefert die tatsächlich kompatible DLL.
            var asm = Assembly.LoadFrom(candidate);
            _cache[simpleName] = asm;
            return asm;
        }
    }
}
