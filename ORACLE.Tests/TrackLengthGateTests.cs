using ClosedXML.Excel;
using Ktm.Racing.Domain.Kompass;

namespace Ktm.Racing.Oracle.Tests;

public sealed class TrackLengthGateTests
{
    // Typical MX laps (first/last = out/in lap, one pit-lane lap).
    private static readonly IReadOnlyList<double>[] Laps =
    [
        [700, 1652, 1648, 1655, 1300, 1650, 900],
        [500, 1649, 1653, 200],
    ];

    [Fact]
    public void Kompass_length_wins_and_is_written_to_circuits_without_suggestion()
    {
        using var tmp = new TempDir();
        var gate = CreateGate(tmp, kompassLength: "1650", circuits: [("Lommel", 1850)]);

        var known = gate.ResolveKnown("Teutschenthal", hedLength: "1500");
        Assert.Equal(TrackLengthGateStatus.Ok, known.Status);
        Assert.Equal(TrackLengthSource.Kompass, known.Source);
        Assert.False(known.Provisional);

        var decision = gate.Complete(known, Metadata(), Laps, "run");
        Assert.Equal(1650, decision.LengthMeters);
        Assert.Equal(1650, CircuitLength(tmp, "Teutschenthal"));
        Assert.False(Directory.Exists(SuggestionsDir(tmp)));
    }

    [Fact]
    public void Shared_kompass_length_is_rejected_and_the_measured_length_is_made_unique()
    {
        using var tmp = new TempDir();
        // KOMPASS says 1000 m, but Lommel already uses 1000 m and Arco 1650 m (the measured value).
        var gate = CreateGate(tmp, kompassLength: "1000", circuits: [("Lommel", 1000), ("Arco", 1650)]);

        var known = gate.ResolveKnown("Teutschenthal", hedLength: "");
        Assert.Equal(TrackLengthGateStatus.Missing, known.Status);
        Assert.Contains(known.Warnings, w => w.Contains("1000 m") && w.Contains("Lommel"));

        var decision = gate.Complete(known, Metadata(), Laps, "run");

        Assert.Equal(TrackLengthGateStatus.Ok, decision.Status);
        Assert.Equal(TrackLengthSource.Estimate, decision.Source);
        Assert.True(decision.Provisional);
        Assert.Equal(1651, decision.LengthMeters); // 1650 is taken by Arco
        Assert.Equal(1651, CircuitLength(tmp, "Teutschenthal"));

        var suggestion = Assert.Single(new TrackLengthSuggestionStore(SuggestionsDir(tmp)).ReadPending());
        Assert.Equal("Teutschenthal", suggestion.TrackName);
        Assert.Equal(1651, suggestion.LengthMeters);
        Assert.Equal("ORACLE GPS", suggestion.Source);
        Assert.Equal(6, suggestion.LapCount);
    }

    [Fact]
    public void Pending_suggestion_is_reused_so_all_machines_use_the_same_length()
    {
        using var tmp = new TempDir();
        var first = CreateGate(tmp, kompassLength: "", circuits: []);
        first.Complete(first.ResolveKnown("Teutschenthal", ""), Metadata(), Laps, "run 1");

        // Another machine processes the same track later, with slightly different laps.
        var second = CreateGate(tmp, kompassLength: "", circuits: []);
        File.Delete(Path.Combine(tmp.Path, "loadChannels.xlsx"));
        var known = second.ResolveKnown("Teutschenthal", "");

        Assert.Equal(TrackLengthSource.Suggestion, known.Source);
        Assert.Equal(1651, known.LengthMeters); // median of the first machine's laps in whole meters
        second.Complete(known, Metadata(), [[0, 1700, 1702, 1698, 0]], "run 2");
        Assert.Single(new TrackLengthSuggestionStore(SuggestionsDir(tmp)).ReadPending());
    }

    [Fact]
    public void Circuits_length_is_used_when_kompass_is_empty()
    {
        using var tmp = new TempDir();
        var gate = CreateGate(tmp, kompassLength: "", circuits: [("Teutschenthal", 1720)]);

        var decision = gate.Complete(gate.ResolveKnown("Teutschenthal", ""), Metadata(), [], "run");

        Assert.Equal(TrackLengthSource.MotoDb, decision.Source);
        Assert.Equal(1720, decision.LengthMeters);
        Assert.False(Directory.Exists(SuggestionsDir(tmp))); // KODE reads CIRCUITS itself
    }

    [Fact]
    public void Missing_when_nothing_is_known_and_laps_are_insufficient()
    {
        using var tmp = new TempDir();
        var gate = CreateGate(tmp, kompassLength: "", circuits: []);

        var decision = gate.Complete(gate.ResolveKnown("Teutschenthal", ""), Metadata(), [[0, 1650, 0]], "run");

        Assert.Equal(TrackLengthGateStatus.Missing, decision.Status);
        Assert.Contains("Not enough", decision.Message);
        Assert.Null(CircuitLength(tmp, "Teutschenthal"));
    }

    [Fact]
    public void Locked_workbook_is_reported()
    {
        using var tmp = new TempDir();
        var gate = CreateGate(tmp, kompassLength: "1650", circuits: []);
        var known = gate.ResolveKnown("Teutschenthal", "");

        using (new FileStream(Path.Combine(tmp.Path, "loadChannels.xlsx"), FileMode.Open, FileAccess.Read, FileShare.None))
        {
            var decision = gate.Complete(known, Metadata(), [], "run");
            Assert.Equal(TrackLengthGateStatus.Locked, decision.Status);
        }
    }

    [Fact]
    public void Suggestions_directory_with_unknown_variable_resolves_to_empty()
    {
        var settings = new TrackLengthSettings { SuggestionsDirectory = @"%KTM_UNDEFINED_TEST_VARIABLE%\suggestions" };
        Assert.Equal(string.Empty, settings.ResolvedSuggestionsDirectory);
    }

    private static TrackLengthGate CreateGate(TempDir tmp, string kompassLength, (string Name, int Length)[] circuits)
    {
        var kompass = tmp.Write("KOMPASS.json",
            $$"""{ "MX": { "Tracks": [ { "Track": "Teutschenthal", "Country": "GERMANY", "Countrycode": "GER", "Tracklength": "{{kompassLength}}" } ] } }""");

        var workbook = Path.Combine(tmp.Path, "loadChannels.xlsx");
        if (!File.Exists(workbook))
        {
            using var wb = new XLWorkbook();
            var sheet = wb.Worksheets.Add("CIRCUITS");
            sheet.Cell(1, 1).Value = "Name";
            sheet.Cell(1, 9).Value = "Length";
            for (var i = 0; i < circuits.Length; i++)
            {
                sheet.Cell(i + 2, 1).Value = circuits[i].Name;
                sheet.Cell(i + 2, 9).Value = circuits[i].Length;
            }
            wb.SaveAs(workbook);
        }

        return new TrackLengthGate(kompass, workbook, new TrackLengthSettings { SuggestionsDirectory = SuggestionsDir(tmp), LockWaitSeconds = 0 });
    }

    private static string SuggestionsDir(TempDir tmp) => Path.Combine(tmp.Path, "suggestions");

    private static RunMetadata Metadata() => new() { TrackName = "Teutschenthal", DisciplineName = "MX", CountryCode = "GER" };

    private static int? CircuitLength(TempDir tmp, string name)
    {
        var path = Path.Combine(tmp.Path, "loadChannels.xlsx");
        if (!File.Exists(path))
            return null;
        using var wb = new XLWorkbook(path);
        var sheet = wb.Worksheet("CIRCUITS");
        for (var r = 2; r <= (sheet.LastRowUsed()?.RowNumber() ?? 1); r++)
        {
            if (sheet.Cell(r, 1).GetString() == name)
                return sheet.Cell(r, 9).IsEmpty() ? null : (int)sheet.Cell(r, 9).GetDouble();
        }
        return null;
    }
}
