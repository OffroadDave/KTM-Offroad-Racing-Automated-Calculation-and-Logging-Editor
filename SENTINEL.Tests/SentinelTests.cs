using System.Globalization;
using System.Xml.Linq;

namespace SENTINEL.Tests;

/// <summary>Builds a WinTAX-like run folder in a temp directory.</summary>
internal sealed class TempRun : IDisposable
{
    public string Root { get; } = Path.Combine(Path.GetTempPath(), "sentinel-tests-" + Guid.NewGuid().ToString("N"));
    public string RunPath { get; }

    public TempRun(string runName = "Run_1000")
    {
        RunPath = Path.Combine(Root, "Event", "BC", "AA 01", runName);
        Directory.CreateDirectory(RunPath);
    }

    public TempRun WithFile(string relative, int size)
    {
        var path = Path.Combine(RunPath, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, new byte[size]);
        return this;
    }

    public TempRun WithLap(string lapName, int size = 4096) => WithFile(Path.Combine(lapName, "cableData.ztx"), size);

    public TempRun WithHeader(params string[] lapNames)
    {
        var doc = new XDocument(new XElement("RunLapHeader",
            new XElement("Run", new XElement("LapArray",
                lapNames.Select((lap, i) => new XElement("Lap",
                    new XElement("LapInfo",
                        new XElement("Path", $"{lap}\\cableData.ztx"),
                        new XElement("LapIndex", i))))))));
        doc.Save(Path.Combine(RunPath, "RunLapHeader.xml"));
        return this;
    }

    public void Dispose()
    {
        try { Directory.Delete(Root, recursive: true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
    }
}

public class RunSnapshotTests
{
    [Fact]
    public void Complete_run_is_structurally_valid()
    {
        using var run = new TempRun().WithHeader("Lap_1", "Lap_2").WithLap("Lap_1").WithLap("Lap_2").WithFile("Session.xml", 100);

        var snapshot = RunSnapshot.Create(run.RunPath);

        Assert.True(snapshot.IsComplete);
        Assert.True(snapshot.IsStructurallyValid);
        Assert.Equal(2, snapshot.LapCount);
        Assert.Equal(4, snapshot.FileCount); // RunLapHeader.xml, Session.xml, 2x cableData.ztx
    }

    [Fact]
    public void Lap_referenced_in_header_but_missing_is_incomplete()
    {
        // B06: a paused sync looks "stable" by size; the header proves that a lap is still missing.
        using var run = new TempRun().WithHeader("Lap_1", "Lap_2", "Lap_3").WithLap("Lap_1").WithLap("Lap_2");

        var snapshot = RunSnapshot.Create(run.RunPath);

        Assert.False(snapshot.IsComplete);
        Assert.False(snapshot.IsStructurallyValid);
        Assert.Equal(["Lap_3\\cableData.ztx"], snapshot.MissingFiles);
    }

    [Fact]
    public void Missing_header_is_incomplete()
    {
        using var run = new TempRun().WithLap("Lap_1");

        var snapshot = RunSnapshot.Create(run.RunPath);

        Assert.Equal(["RunLapHeader.xml"], snapshot.MissingFiles);
        Assert.False(snapshot.IsStructurallyValid);
    }

    [Fact]
    public void Corrupt_header_is_incomplete()
    {
        using var run = new TempRun().WithLap("Lap_1");
        File.WriteAllText(Path.Combine(run.RunPath, "RunLapHeader.xml"), "<RunLapHeader><Run>");

        var snapshot = RunSnapshot.Create(run.RunPath);

        Assert.Equal(["RunLapHeader.xml"], snapshot.MissingFiles);
    }

    [Fact]
    public void Header_path_outside_run_counts_as_missing()
    {
        using var run = new TempRun().WithLap("Lap_1");
        new XDocument(new XElement("RunLapHeader", new XElement("LapInfo", new XElement("Path", "..\\..\\outside.ztx"))))
            .Save(Path.Combine(run.RunPath, "RunLapHeader.xml"));
        File.WriteAllBytes(Path.Combine(run.Root, "Event", "BC", "outside.ztx"), new byte[4096]);

        var snapshot = RunSnapshot.Create(run.RunPath);

        Assert.False(snapshot.IsComplete);
    }

    [Fact]
    public void Undersized_ztx_is_flagged_as_placeholder()
    {
        using var run = new TempRun().WithHeader("Lap_1").WithLap("Lap_1", size: 10);

        var snapshot = RunSnapshot.Create(run.RunPath, minimumFileSize: 1024);

        Assert.True(snapshot.HasZeroByteFiles);
        Assert.False(snapshot.IsStructurallyValid);
    }

    [Fact]
    public void Offline_attribute_is_detected_as_cloud_placeholder()
    {
        using var run = new TempRun().WithHeader("Lap_1").WithLap("Lap_1");
        var file = new FileInfo(Path.Combine(run.RunPath, "Lap_1", "cableData.ztx"));
        try
        {
            file.Attributes |= FileAttributes.Offline;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            return; // file system does not support the attribute
        }
        file.Refresh();
        if ((file.Attributes & FileAttributes.Offline) == 0)
            return;

        try
        {
            Assert.True(RunSnapshot.IsPlaceholder(file));
            var snapshot = RunSnapshot.Create(run.RunPath);
            Assert.True(snapshot.HasPlaceholderFiles);
            Assert.False(snapshot.IsStructurallyValid);
        }
        finally
        {
            file.Attributes &= ~FileAttributes.Offline;
        }
    }

    [Fact]
    public void Locked_file_is_detected()
    {
        using var run = new TempRun().WithHeader("Lap_1").WithLap("Lap_1");
        RunSnapshot snapshot;
        using (new FileStream(Path.Combine(run.RunPath, "Lap_1", "cableData.ztx"), FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
            snapshot = RunSnapshot.Create(run.RunPath);
        }

        Assert.True(snapshot.HasLockedFiles);
        Assert.False(snapshot.IsStructurallyValid);
    }

    [Fact]
    public void Missing_run_folder_gives_empty_snapshot()
    {
        var snapshot = RunSnapshot.Create(Path.Combine(Path.GetTempPath(), "does-not-exist-" + Guid.NewGuid().ToString("N")));

        Assert.Equal(0, snapshot.FileCount);
        Assert.False(snapshot.IsStructurallyValid);
    }
}

public class StabilityTrackerTests
{
    [Fact]
    public void Run_becomes_stable_after_threshold_unchanged_snapshots()
    {
        using var run = new TempRun().WithHeader("Lap_1").WithLap("Lap_1");
        var tracker = new StabilityTracker(requiredStableChecks: 2);

        tracker.RecordSnapshot(RunSnapshot.Create(run.RunPath));
        Assert.False(tracker.IsStable(run.RunPath));
        tracker.RecordSnapshot(RunSnapshot.Create(run.RunPath));
        Assert.False(tracker.IsStable(run.RunPath));
        tracker.RecordSnapshot(RunSnapshot.Create(run.RunPath));
        Assert.True(tracker.IsStable(run.RunPath));
    }

    [Fact]
    public void Growth_resets_the_counter()
    {
        using var run = new TempRun().WithHeader("Lap_1", "Lap_2").WithLap("Lap_1").WithLap("Lap_2");
        var tracker = new StabilityTracker(requiredStableChecks: 1);

        tracker.RecordSnapshot(RunSnapshot.Create(run.RunPath));
        run.WithLap("Lap_2", size: 8192);
        tracker.RecordSnapshot(RunSnapshot.Create(run.RunPath));
        Assert.False(tracker.IsStable(run.RunPath));
        tracker.RecordSnapshot(RunSnapshot.Create(run.RunPath));
        Assert.True(tracker.IsStable(run.RunPath));
    }

    [Fact]
    public void Incomplete_run_never_becomes_stable()
    {
        using var run = new TempRun().WithHeader("Lap_1", "Lap_2").WithLap("Lap_1");
        var tracker = new StabilityTracker(requiredStableChecks: 1);

        for (int i = 0; i < 5; i++)
            tracker.RecordSnapshot(RunSnapshot.Create(run.RunPath));

        Assert.False(tracker.IsStable(run.RunPath));
    }

    [Fact]
    public void Invalid_threshold_falls_back_to_three()
    {
        using var run = new TempRun().WithHeader("Lap_1").WithLap("Lap_1");
        var tracker = new StabilityTracker(requiredStableChecks: 0);

        for (int i = 0; i < 3; i++)
            tracker.RecordSnapshot(RunSnapshot.Create(run.RunPath));
        Assert.False(tracker.IsStable(run.RunPath));
        tracker.RecordSnapshot(RunSnapshot.Create(run.RunPath));
        Assert.True(tracker.IsStable(run.RunPath));
    }

    [Fact]
    public void RetainOnly_drops_states_of_vanished_runs()
    {
        var tracker = new StabilityTracker(3);
        tracker.RecordSnapshot(new RunSnapshot { RunPath = @"C:\a\Run_1" });
        tracker.RecordSnapshot(new RunSnapshot { RunPath = @"C:\a\Run_2" });

        var removed = tracker.RetainOnly([@"c:\A\RUN_1"]);

        Assert.Equal(1, removed);
        Assert.Equal(1, tracker.TrackedRunCount);
        Assert.NotNull(tracker.GetLatestSnapshot(@"C:\a\Run_1"));
    }
}

public class DownloadDoneWriterTests
{
    [Fact]
    public void Writes_schema_shaped_xml_atomically()
    {
        using var run = new TempRun().WithHeader("Lap_1").WithLap("Lap_1").WithFile("Session.xml", 100);
        var snapshot = RunSnapshot.Create(run.RunPath);

        var path = DownloadDoneWriter.Write(snapshot);

        Assert.True(DownloadDoneWriter.Exists(run.RunPath));
        Assert.False(File.Exists(path + ".tmp"));
        var done = XDocument.Load(path).Root!;
        Assert.Equal("Done", done.Name.LocalName);
        Assert.Equal("SENTINEL", done.Element("UserName")!.Value);
        Assert.True(DateTime.TryParseExact(done.Element("DateTime")!.Value, "yyyy-MM-dd'T'HH:mm:ss",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
        var runElement = done.Element("Run")!;
        Assert.Equal("Run_1000", runElement.Attribute("path")!.Value);
        Assert.Equal(snapshot.FileCount.ToString(CultureInfo.InvariantCulture), runElement.Attribute("files")!.Value);
        Assert.Single(runElement.Elements("Lap"));
    }

    [Fact]
    public void Overwrites_existing_file()
    {
        using var run = new TempRun().WithHeader("Lap_1").WithLap("Lap_1");
        File.WriteAllText(Path.Combine(run.RunPath, "download.done"), "old");

        var path = DownloadDoneWriter.Write(RunSnapshot.Create(run.RunPath));

        Assert.StartsWith("<?xml", File.ReadAllText(path));
    }

    [Fact]
    public void Timestamp_is_culture_invariant()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            // Finnish uses '.' as time separator; an unquoted ':' in a custom format would follow it.
            CultureInfo.CurrentCulture = new CultureInfo("fi-FI");
            Assert.Equal("2026-05-04T13:07:09", DownloadDoneWriter.FormatTimestamp(new DateTime(2026, 5, 4, 13, 7, 9)));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
}

public class SentinelConfigTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(30, 30)]
    [InlineData(100000, 3600)]
    public void Polling_interval_is_clamped(int configured, int expected)
    {
        var config = new SentinelConfig { PollingIntervalSeconds = configured };
        config.Normalize();
        Assert.Equal(expected, config.PollingIntervalSeconds);
    }

    [Fact]
    public void Invalid_values_are_corrected()
    {
        var config = new SentinelConfig
        {
            WatchPaths = [" C:\\Data ", "", "c:\\data", "D:\\Other"],
            StabilityThreshold = 0,
            MinimumFileSizeBytes = -1,
            MaxRunAgeDays = -3,
        };

        config.Normalize();

        Assert.Equal(["C:\\Data", "D:\\Other"], config.WatchPaths);
        Assert.Equal(3, config.StabilityThreshold);
        Assert.Equal(0, config.MinimumFileSizeBytes);
        Assert.Equal(0, config.MaxRunAgeDays);
    }

    [Fact]
    public void Load_reads_json_with_comments_and_normalizes()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, """
                {
                    // comment
                    "watchPaths": ["C:\\MXTrackData"],
                    "pollingIntervalSeconds": 0,
                    "maxRunAgeDays": 14,
                }
                """);

            var config = SentinelConfig.Load(path);

            Assert.Equal(["C:\\MXTrackData"], config.WatchPaths);
            Assert.Equal(SentinelConfig.MinPollingIntervalSeconds, config.PollingIntervalSeconds);
            Assert.Equal(14, config.MaxRunAgeDays);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Load_falls_back_to_defaults_for_missing_or_broken_file()
    {
        var missing = SentinelConfig.Load(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json"));
        Assert.Equal(30, missing.PollingIntervalSeconds);

        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "{ not json");
            var broken = SentinelConfig.Load(path);
            Assert.Equal(30, broken.PollingIntervalSeconds);
            Assert.Empty(broken.WatchPaths);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Shipped_config_file_is_valid()
    {
        var shipped = Path.Combine(AppContext.BaseDirectory, "sentinel-config.json");
        Assert.True(File.Exists(shipped), "sentinel-config.json must be copied to the output folder");

        var config = SentinelConfig.Load(shipped);

        Assert.NotEmpty(config.WatchPaths);
        Assert.Equal(30, config.PollingIntervalSeconds);
        Assert.Equal(3, config.StabilityThreshold);
        Assert.Equal(0, config.MaxRunAgeDays);
    }
}

public class RunWatcherTests
{
    [Fact]
    public void Scan_cycles_generate_download_done_once_stable()
    {
        using var run = new TempRun().WithHeader("Lap_1").WithLap("Lap_1");
        var watcher = new RunWatcher(new SentinelConfig { WatchPaths = [run.Root], StabilityThreshold = 1 });

        Assert.Equal(0, watcher.RunScanCycle());
        Assert.Equal(1, watcher.RunScanCycle());
        Assert.True(DownloadDoneWriter.Exists(run.RunPath));
        Assert.Equal(0, watcher.TrackedRunCount);
        Assert.Empty(watcher.DiscoverPendingRuns());
    }

    [Fact]
    public void Deleted_run_is_removed_from_tracking()
    {
        using var run = new TempRun().WithHeader("Lap_1", "Lap_2").WithLap("Lap_1");
        var watcher = new RunWatcher(new SentinelConfig { WatchPaths = [run.Root], StabilityThreshold = 1 });

        watcher.RunScanCycle();
        Assert.Equal(1, watcher.TrackedRunCount);

        Directory.Delete(run.RunPath, recursive: true);
        watcher.RunScanCycle();

        Assert.Equal(0, watcher.TrackedRunCount);
    }

    [Fact]
    public void Old_runs_are_skipped_when_max_age_is_set()
    {
        using var run = new TempRun().WithHeader("Lap_1").WithLap("Lap_1");
        var old = DateTime.UtcNow.AddDays(-30);
        Directory.SetLastWriteTimeUtc(Path.Combine(run.RunPath, "Lap_1"), old);
        Directory.SetLastWriteTimeUtc(run.RunPath, old);

        var withAge = new RunWatcher(new SentinelConfig { WatchPaths = [run.Root], MaxRunAgeDays = 7 });
        var withoutAge = new RunWatcher(new SentinelConfig { WatchPaths = [run.Root] });

        Assert.Empty(withAge.DiscoverPendingRuns());
        Assert.Single(withoutAge.DiscoverPendingRuns());
    }

    [Fact]
    public void Overlapping_watch_paths_report_a_run_once()
    {
        using var run = new TempRun().WithHeader("Lap_1").WithLap("Lap_1");
        var watcher = new RunWatcher(new SentinelConfig { WatchPaths = [run.Root, Path.Combine(run.Root, "Event")] });

        Assert.Single(watcher.DiscoverPendingRuns());
    }

    [Fact]
    public void Missing_watch_path_does_not_throw()
    {
        var watcher = new RunWatcher(new SentinelConfig { WatchPaths = [Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))] });

        Assert.Empty(watcher.DiscoverPendingRuns());
        Assert.Equal(0, watcher.RunScanCycle());
    }
}
