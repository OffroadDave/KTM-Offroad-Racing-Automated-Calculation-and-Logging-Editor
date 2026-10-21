namespace Ktm.Racing.Oracle.Tests;

public sealed class StartDetectorTests
{
    private const double Step = 0.01;

    /// <summary>Builds signals: optional staging phase (stationary, revs, launch limiter) then a launch at <paramref name="launchAt"/>.</summary>
    private static StartSignals Signals(double stagedFrom, double launchAt, double launchSpeedGain, double duration = 15, bool stagedRevs = true)
    {
        var time = StartDetector.Grid(0, duration, Step);
        var n = time.Length;
        double[] speed = new double[n], gear = new double[n], rpm = new double[n], throttle = new double[n], clutch = new double[n], rls = new double[n];
        for (var k = 0; k < n; k++)
        {
            var t = time[k];
            gear[k] = 2;
            if (t < launchAt)
            {
                var staged = t >= stagedFrom;
                speed[k] = 0;
                rpm[k] = staged && stagedRevs ? 8700 : 2300;
                throttle[k] = staged ? 25 : 0;
                clutch[k] = 45;
                rls[k] = staged ? 4 : 1;
            }
            else
            {
                speed[k] = Math.Min(90, (t - launchAt) * launchSpeedGain);
                rpm[k] = 9000;
                throttle[k] = t - launchAt < 0.02 ? 48 : 100;
                clutch[k] = 20;
                rls[k] = t - launchAt < 0.05 ? 4 : 1;
            }
        }
        return new StartSignals { Time = time, SpeedKph = speed, Gear = gear, Rpm = rpm, Throttle = throttle, Clutch = clutch, RevLimitSource = rls };
    }

    [Fact]
    public void Gate_start_after_staging_is_valid()
    {
        var start = Assert.Single(StartDetector.Detect(Signals(stagedFrom: 3, launchAt: 6, launchSpeedGain: 27)));

        Assert.True(start.Valid, start.Reason);
        Assert.InRange(start.TimeSeconds, 5.99, 6.02);
        Assert.InRange(start.MaxSpeed3sKph, 79, 82);
        Assert.InRange(start.Distance5sMeters, 60, 120);
    }

    [Fact]
    public void Launch_without_staging_is_no_candidate()
    {
        // Pit exit: rider pulls away without holding revs on the gate.
        Assert.Empty(StartDetector.Detect(Signals(stagedFrom: 5.8, launchAt: 6, launchSpeedGain: 27)));
        Assert.Empty(StartDetector.Detect(Signals(stagedFrom: 3, launchAt: 6, launchSpeedGain: 27, stagedRevs: false)));
    }

    [Fact]
    public void Aborted_start_is_rejected_with_reason()
    {
        var start = Assert.Single(StartDetector.Detect(Signals(stagedFrom: 3, launchAt: 6, launchSpeedGain: 2)));

        Assert.False(start.Valid);
        Assert.Contains("no launch", start.Reason);
    }

    [Fact]
    public void Resample_uses_sample_and_hold()
    {
        var grid = StartDetector.Grid(0, 1, 0.25);
        Assert.Equal([0, 0.25, 0.5, 0.75, 1.0], grid);
        Assert.Equal([1, 1, 2, 2, 3], StartDetector.Resample([0, 0.5, 1.0], [1, 2, 3], grid));
    }
}
