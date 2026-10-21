namespace Ktm.Racing.Oracle.Tests;

public sealed class CleanupTests
{
    [Fact]
    public void Channels_are_found_by_exact_name_only()
    {
        string[] names = ["rThrottle1_Target", "RTHROTTLE1", "nEngine", "Lat_Valid"];

        Assert.Equal("RTHROTTLE1", ChannelResolver.Find(names, "rThrottle1", "rThrottle"));
        Assert.Equal("nEngine", ChannelResolver.Find(names, "nEngine720", "nEngine"));
        Assert.Null(ChannelResolver.Find(names, "Lat_dez"));
        Assert.Null(ChannelResolver.Find(names, "ValidSat"));
    }

    [Fact]
    public void Run_folders_are_ordered_numerically()
    {
        string[] runs = [@"C:\x\AA01\Run_1000", @"C:\x\AA01\Run_999", @"C:\x\AA01\Run_1001", @"C:\x\AA01\Other"];

        Assert.Equal([@"C:\x\AA01\Run_999", @"C:\x\AA01\Run_1000", @"C:\x\AA01\Run_1001", @"C:\x\AA01\Other"], RunFolders.InOrder(runs));
        Assert.Equal(1083, RunFolders.Number("Run_1083"));
        Assert.Equal(int.MaxValue, RunFolders.Number("Run_"));
    }

    [Fact]
    public void Removed_merge_command_is_reported()
    {
        Assert.Equal("Der Befehl 'merge' wurde entfernt.", CommandLine.Validate(["merge", @"C:\x"]));
    }
}
