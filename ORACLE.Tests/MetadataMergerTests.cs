namespace Ktm.Racing.Oracle.Tests;

public sealed class MetadataMergerTests
{
    private static RunMetadata Derived() => new()
    {
        RiderName = "Andrea Adamo",
        TeamName = "KTM Factory Racing",
        SeasonName = "2026",
        DisciplineName = "MX",
        EngineClass = "450",
        RiderAbbreviation = "AA",
        BikeNumber = "01",
        CountryName = "Germany",
        CountryCode = "GER",
        TrackName = "Teutschenthal",
        TypeName = "Racetrack",
        CalendarWeek = "22",
    };

    [Fact]
    public void Kompass_replaces_outdated_hed_values()
    {
        // Minimal manual .HED of R1: placeholder rider.
        var hed = new RunMetadata { BikeName = "AA01", TrackName = "Teutschenthal", TrackLength = "1565", RiderName = "Test Rider" };

        var differences = MetadataMerger.Merge(hed, Derived(), preferHed: false);

        Assert.Equal("Andrea Adamo", hed.RiderName);
        Assert.Equal("KTM Factory Racing", hed.TeamName);
        Assert.Equal(("2026", "MX", "450"), (hed.SeasonName, hed.DisciplineName, hed.EngineClass));
        Assert.Equal("1565", hed.TrackLength);
        var rider = Assert.Single(differences);
        Assert.Equal(("riderName", "Test Rider", "Andrea Adamo"), (rider.Field, rider.HedValue, rider.KompassValue));
    }

    [Fact]
    public void PreferHed_keeps_manual_values_but_reports_them()
    {
        var hed = new RunMetadata { RiderName = "Substitute Rider", SeasonName = "Wintax" };

        var differences = MetadataMerger.Merge(hed, Derived(), preferHed: true);

        Assert.Equal("Substitute Rider", hed.RiderName);
        Assert.Equal("Wintax", hed.SeasonName);
        Assert.Equal("KTM Factory Racing", hed.TeamName); // empty fields are still completed
        Assert.Equal(["riderName", "seasonName"], differences.Select(d => d.Field));
    }

    [Fact]
    public void Hed_values_stay_when_kompass_does_not_know_them()
    {
        var hed = new RunMetadata { RiderName = "Guest Rider", TeamName = "Guest Team", TrackName = "Teutschenthal Süd" };
        var derived = Derived();
        derived.RiderName = string.Empty; // rider not in KOMPASS
        derived.TeamName = "KTM Factory Racing";

        Assert.Empty(MetadataMerger.Merge(hed, derived, preferHed: false));
        Assert.Equal(("Guest Rider", "Guest Team"), (hed.RiderName, hed.TeamName));
        Assert.Equal("Teutschenthal Süd", hed.TrackName); // event-name fields are only filled, never replaced
    }
}
