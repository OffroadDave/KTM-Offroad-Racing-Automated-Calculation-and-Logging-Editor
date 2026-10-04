namespace Ktm.Racing.Oracle.Tests;

public sealed class OracleStatusTests
{
    [Fact]
    public void Save_then_Load_round_trips_core_fields()
    {
        using var tmp = new TempDir();
        var status = OracleStatus.Create(tmp.Path);
        status.SetStep("metadata", StepStatus.Success);
        status.SetStep("calculations", StepStatus.Partial, new Dictionary<string, object> { ["processedRuns"] = 2 });
        status.ExitCode = 0;
        status.Save(tmp.Path);

        var loaded = OracleStatus.Load(tmp.Path);

        Assert.NotNull(loaded);
        Assert.Equal(0, loaded!.ExitCode);
        Assert.Equal(StepStatus.Success, loaded.Steps["metadata"].Status);
        Assert.Equal(StepStatus.Partial, loaded.Steps["calculations"].Status);
    }

    [Fact]
    public void IsAlreadyProcessed_is_true_only_for_exit_code_zero()
    {
        using var tmp = new TempDir();
        var status = OracleStatus.Create(tmp.Path);

        status.ExitCode = 3;
        status.Save(tmp.Path);
        Assert.False(OracleStatus.IsAlreadyProcessed(tmp.Path));

        status.ExitCode = 0;
        status.Save(tmp.Path);
        Assert.True(OracleStatus.IsAlreadyProcessed(tmp.Path));
    }

    [Fact]
    public void Load_returns_null_for_missing_file()
    {
        using var tmp = new TempDir();
        Assert.Null(OracleStatus.Load(tmp.Path));
    }

    [Fact]
    public void Load_returns_null_for_corrupt_json()
    {
        using var tmp = new TempDir();
        File.WriteAllText(Path.Combine(tmp.Path, "oracle.status.json"), "{ not valid json ");
        Assert.Null(OracleStatus.Load(tmp.Path));
    }

    [Fact]
    public void Save_to_nonexistent_directory_is_non_fatal()
    {
        var missing = Path.Combine(Path.GetTempPath(), "oracle-tests", Guid.NewGuid().ToString("N"), "does-not-exist");
        var status = OracleStatus.Create(missing);

        var ex = Record.Exception(() => status.Save(missing));

        Assert.Null(ex); // Save swallows IO errors so a locked/missing path never masks success.
    }

    [Fact]
    public void OracleVersion_is_populated_and_stable_across_instances()
    {
        var a = OracleStatus.Create("x").OracleVersion;
        var b = OracleStatus.Create("y").OracleVersion;

        Assert.False(string.IsNullOrWhiteSpace(a));
        Assert.Equal(a, b);
    }
}
