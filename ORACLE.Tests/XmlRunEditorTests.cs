using System.Globalization;
using System.Xml.Linq;

namespace Ktm.Racing.Oracle.Tests;

public sealed class XmlRunEditorTests
{
    private static string MakeRun(TempDir tmp, string runName, params string[] lapFolders)
    {
        var run = tmp.CreateDir(runName);
        File.WriteAllText(Path.Combine(run, "Car.xml"), "<Car><Name>old</Name></Car>");
        File.WriteAllText(Path.Combine(run, "Session.xml"), "<Session><Track>old</Track></Session>");
        foreach (var lap in lapFolders)
        {
            var dir = Directory.CreateDirectory(Path.Combine(run, lap)).FullName;
            File.WriteAllText(Path.Combine(dir, "LapHeader.xml"), "<LapHeader><Lap>0</Lap></LapHeader>");
        }
        return run;
    }

    private static XElement LapRoot(string run, string lap) =>
        XDocument.Load(Path.Combine(run, lap, "LapHeader.xml")).Root!;

    [Fact]
    public void SetRunProperties_sets_lap_fields_box_marker_on_last_lap_and_run_from_folder()
    {
        using var tmp = new TempDir();
        var run = MakeRun(tmp, "Run_1079", "Lap_0001", "Lap_0002");

        var laps = new XmlRunEditor().SetRunProperties(
            run, alias: "AA 01", track: "Teutschenthal", trackLength: "1650", rider: "Test Rider",
            lapOffset: 0, isLastInStint: true, lapDistances: new[] { 15.37, 1600.5 });

        Assert.Equal(2, laps);

        var lap1 = LapRoot(run, "Lap_0001");
        Assert.Equal("1", lap1.Element("Lap")!.Value);
        Assert.Equal("1", lap1.Element("TrackLap")!.Value);
        Assert.Equal("AA01", lap1.Element("Alias")!.Value);
        Assert.Equal("1079", lap1.Element("Run")!.Value);
        Assert.Equal("Lap", lap1.Element("Marker")!.Value);
        Assert.Equal("15.37", lap1.Element("LapDistance")!.Value);

        var lap2 = LapRoot(run, "Lap_0002");
        Assert.Equal("2", lap2.Element("Lap")!.Value);
        Assert.Equal("Box", lap2.Element("Marker")!.Value);
        Assert.Equal("1600.5", lap2.Element("LapDistance")!.Value);

        var session = XDocument.Load(Path.Combine(run, "Session.xml")).Root!;
        Assert.Equal("Teutschenthal", session.Element("Track")!.Value);
        Assert.Equal("1650", session.Element("TrackLen")!.Value);

        var car = XDocument.Load(Path.Combine(run, "Car.xml")).Root!;
        Assert.Equal("AA01", car.Element("Name")!.Value);
        Assert.Equal("Test Rider", car.Element("DriverName")!.Value);
    }

    [Fact]
    public void SetRunProperties_lapOffset_produces_continuous_numbering()
    {
        using var tmp = new TempDir();
        var run = MakeRun(tmp, "Run_1084", "Lap_0001");

        new XmlRunEditor().SetRunProperties(run, "AA01", "T", "1000", "R", lapOffset: 5, isLastInStint: false);

        var lap = LapRoot(run, "Lap_0001");
        Assert.Equal("6", lap.Element("Lap")!.Value);
        Assert.Equal("Lap", lap.Element("Marker")!.Value);
    }

    [Fact]
    public void SetRunProperties_formats_LapDistance_with_invariant_culture()
    {
        using var tmp = new TempDir();
        var run = MakeRun(tmp, "Run_1", "Lap_0001");

        var previous = CultureInfo.CurrentCulture;
        try
        {
            // de-DE uses ',' as decimal separator; the written XML must still use '.'.
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            new XmlRunEditor().SetRunProperties(run, "AA01", "T", "1000", "R", lapDistances: new[] { 12.5 });
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }

        Assert.Equal("12.5", LapRoot(run, "Lap_0001").Element("LapDistance")!.Value);
    }

    [Fact]
    public void SetRunProperties_without_lapDistances_does_not_write_LapDistance()
    {
        using var tmp = new TempDir();
        var run = MakeRun(tmp, "Run_1", "Lap_0001");

        new XmlRunEditor().SetRunProperties(run, "AA01", "T", "1000", "R");

        Assert.Null(LapRoot(run, "Lap_0001").Element("LapDistance"));
    }

    [Fact]
    public void SetRunProperties_without_lapDistances_keeps_existing_LapDistance()
    {
        using var tmp = new TempDir();
        var run = MakeRun(tmp, "Run_1", "Lap_0001");
        File.WriteAllText(Path.Combine(run, "Lap_0001", "LapHeader.xml"), "<LapHeader><LapDistance>1500</LapDistance></LapHeader>");

        new XmlRunEditor().SetRunProperties(run, "AA01", "T", "1000", "R");

        Assert.Equal("1500", LapRoot(run, "Lap_0001").Element("LapDistance")!.Value);
    }

    [Fact]
    public void SetRunProperties_unknown_lapDistance_removes_stale_value_on_reprocessing()
    {
        using var tmp = new TempDir();
        var run = tmp.CreateDir("Run_1083");
        foreach (var lap in new[] { "Lap_0001", "Lap_0002" })
        {
            Directory.CreateDirectory(Path.Combine(run, lap));
            File.WriteAllText(Path.Combine(run, lap, "LapHeader.xml"), "<LapHeader><Lap>0</Lap><LapDistance>0</LapDistance></LapHeader>");
        }
        File.WriteAllText(Path.Combine(run, "RunLapHeader.xml"),
            "<RunLapHeader><Run><LapArray>" +
            "  <Lap><LapInfo><Path>Lap_0001\\cableData.ztx</Path></LapInfo>" +
            "    <Infos><Info><Name>LapDistance</Name><Data>0</Data><Infos/></Info></Infos></Lap>" +
            "  <Lap><LapInfo><Path>Lap_0002\\cableData.ztx</Path></LapInfo>" +
            "    <Infos><Info><Name>LapDistance</Name><Data>1234</Data><Infos/></Info></Infos></Lap>" +
            "</LapArray></Run></RunLapHeader>");

        new XmlRunEditor().SetRunProperties(run, "AA01", "T", "1000", "R",
            lapDistances: new[] { double.NaN, double.NaN });

        Assert.Null(LapRoot(run, "Lap_0001").Element("LapDistance"));
        Assert.Null(LapRoot(run, "Lap_0002").Element("LapDistance"));
        var doc = XDocument.Load(Path.Combine(run, "RunLapHeader.xml"));
        Assert.DoesNotContain(doc.Descendants("Info"), i => (string?)i.Element("Name") == "LapDistance");
        Assert.Equal(2, doc.Descendants("Info").Count(i => (string?)i.Element("Name") == "Lap"));
    }

    [Fact]
    public void SetRunProperties_updates_RunLapHeader_infos_and_normalizes_alias_in_paths()
    {
        using var tmp = new TempDir();
        var run = tmp.CreateDir("Run_1083");
        Directory.CreateDirectory(Path.Combine(run, "Lap_0001"));
        File.WriteAllText(Path.Combine(run, "Lap_0001", "LapHeader.xml"), "<LapHeader><Lap>0</Lap></LapHeader>");
        File.WriteAllText(Path.Combine(run, "Car.xml"), "<Car/>");
        File.WriteAllText(Path.Combine(run, "Session.xml"), "<Session/>");
        File.WriteAllText(Path.Combine(run, "RunLapHeader.xml"),
            "<RunLapHeader>" +
            "  <Inf><Path>C:\\data\\AA 01\\Run_1083\\</Path></Inf>" +
            "  <Run><LapArray><Lap>" +
            "    <LapInfo><Path>Lap_0001\\cableData.ztx</Path></LapInfo>" +
            "    <Infos></Infos>" +
            "  </Lap></LapArray></Run>" +
            "</RunLapHeader>");

        new XmlRunEditor().SetRunProperties(run, "AA 01", "T", "1000", "R",
            lapOffset: 0, isLastInStint: true, lapDistances: new[] { 42.0 });

        var doc = XDocument.Load(Path.Combine(run, "RunLapHeader.xml"));

        var infPath = doc.Descendants("Path").First().Value;
        Assert.Contains(@"\AA01\", infPath);
        Assert.DoesNotContain("AA 01", infPath);

        var infos = doc.Descendants("Infos").First(e => e.Elements("Info").Any());
        string Data(string name) => infos.Elements("Info").First(i => (string?)i.Element("Name") == name).Element("Data")!.Value;
        Assert.Equal("1", Data("Lap"));
        Assert.Equal("AA01", Data("Alias"));
        Assert.Equal("Box", Data("Marker"));
        Assert.Equal("42", Data("LapDistance"));
    }

    [Fact]
    public void SetRunProperties_tolerates_missing_xml_files()
    {
        using var tmp = new TempDir();
        var run = tmp.CreateDir("Run_1");
        Directory.CreateDirectory(Path.Combine(run, "Lap_0001")); // no LapHeader.xml, no Car/Session

        var ex = Record.Exception(() => new XmlRunEditor().SetRunProperties(run, "AA01", "T", "1000", "R"));

        Assert.Null(ex);
    }
}
