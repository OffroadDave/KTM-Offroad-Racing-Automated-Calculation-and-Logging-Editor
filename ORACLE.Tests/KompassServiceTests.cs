namespace Ktm.Racing.Oracle.Tests;

// Exercises only the KOMPASS-backed lookups (no KtmDataHandler/native path is touched).
public sealed class KompassServiceTests
{
    private const string Json = """
    { "MX": { "Tracks": [
        { "Track": "Teutschenthal", "Countrycode": "GER", "Tracklength": "1650", "Latitude": "51.52", "Longitude": "11.80", "Surface": "Hardpack" },
        { "Track": "NoCoords", "Tracklength": "1000" }
    ] } }
    """;

    [Fact]
    public async Task FindFinishLineByTrack_returns_coordinate_when_present()
    {
        using var tmp = new TempDir();
        var service = new KompassService();
        Assert.True(await service.LoadAsync(tmp.Write("KOMPASS.json", Json)));

        var finish = service.FindFinishLineByTrack("teutschenthal"); // case-insensitive
        Assert.NotNull(finish);
        Assert.Equal(51.52, finish!.Value.Latitude, 3);
        Assert.Equal(11.80, finish.Value.Longitude, 3);
    }

    [Fact]
    public async Task FindFinishLineByTrack_returns_null_without_coordinates_or_unknown_track()
    {
        using var tmp = new TempDir();
        var service = new KompassService();
        await service.LoadAsync(tmp.Write("KOMPASS.json", Json));

        Assert.Null(service.FindFinishLineByTrack("NoCoords"));
        Assert.Null(service.FindFinishLineByTrack("DoesNotExist"));
    }

    [Fact]
    public async Task FindTrackSurface_returns_surface_or_null()
    {
        using var tmp = new TempDir();
        var service = new KompassService();
        await service.LoadAsync(tmp.Write("KOMPASS.json", Json));

        Assert.Equal("Hardpack", service.FindTrackSurface("Teutschenthal"));
        Assert.Null(service.FindTrackSurface("NoCoords"));
        Assert.Null(service.FindTrackSurface("DoesNotExist"));
    }
}
