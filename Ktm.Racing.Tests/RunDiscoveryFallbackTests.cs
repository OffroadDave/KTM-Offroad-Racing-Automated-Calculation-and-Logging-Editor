using Ktm.Racing.Core;
using Xunit;

namespace Ktm.Racing.Tests;

public class RunDiscoveryFallbackTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"disc_{Guid.NewGuid():N}");

    public RunDiscoveryFallbackTests() => Directory.CreateDirectory(_root);

    private string MakeRun(string name, bool withXml, string? ztxLapFolder)
    {
        var run = Path.Combine(_root, name);
        Directory.CreateDirectory(run);
        if (withXml)
        {
            File.WriteAllText(Path.Combine(run, "Session.xml"), "<Session/>");
            File.WriteAllText(Path.Combine(run, "Car.xml"), "<Car/>");
            File.WriteAllText(Path.Combine(run, "RunLapHeader.xml"), "<RunLapHeader/>");
        }
        if (ztxLapFolder is not null)
        {
            var lapDir = Path.Combine(run, ztxLapFolder);
            Directory.CreateDirectory(lapDir);
            File.WriteAllText(Path.Combine(lapDir, "cableData.ztx"), "x");
        }
        return run;
    }

    [Fact]
    public void ScanRunFolders_FindsRunsWithXmlAndCableData_IncludingReprocessedNested()
    {
        var clean = MakeRun("Run_1", withXml: true, ztxLapFolder: "Lap_00000001_00418");
        // Reprocessed like Run_1065: the .ztx sits in a suffixed variant folder.
        var reprocessed = MakeRun("Run_2", withXml: true, ztxLapFolder: "Lap_00000002_00418_Def_000_App_000_Def_000");
        MakeRun("Run_3", withXml: true, ztxLapFolder: null);              // no cableData.ztx -> excluded
        MakeRun("Run_4", withXml: false, ztxLapFolder: "Lap_00000004_00418"); // missing run XMLs -> excluded

        var found = RunDiscoveryService.ScanRunFolders(_root);

        Assert.Contains(clean, found);
        Assert.Contains(reprocessed, found);
        Assert.Equal(2, found.Count);
    }

    [Fact]
    public void ScanRunFolders_MissingFolder_ReturnsEmpty()
        => Assert.Empty(RunDiscoveryService.ScanRunFolders(Path.Combine(_root, "does_not_exist")));

    public void Dispose()
    {
        try { Directory.Delete(_root, recursive: true); } catch { }
    }
}
