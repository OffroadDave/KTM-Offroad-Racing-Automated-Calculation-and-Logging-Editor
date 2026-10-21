namespace Ktm.Racing.Oracle
{
    /// <summary>Start detection thresholds (identical to the VCH <c>KTM_ORR_ValidStart.vch</c>).</summary>
    public sealed class StartDetectionOptions
    {
        /// <summary>Standstill: GPS speed below this [km/h].</summary>
        public double StagedMaxSpeedKph { get; init; } = 3;

        /// <summary>At the gate: RPM above this [rpm] (idle is well below).</summary>
        public double StagedMinRpm { get; init; } = 4000;

        /// <summary>Minimum duration [s] the bike must stand ready at the gate without interruption before the start (RPM build-up ~2 s).</summary>
        public double StagedMinSeconds { get; init; } = 1.5;

        /// <summary>The start must occur at most this long [s] after standing ready.</summary>
        public double StagedHoldSeconds { get; init; } = 1;

        /// <summary>Minimum throttle [%] within <see cref="LaunchThrottleSeconds"/> after the trigger.</summary>
        public double LaunchMinThrottle { get; init; } = 50;

        /// <summary>Time window [s] after the trigger for <see cref="LaunchMinThrottle"/>.</summary>
        public double LaunchThrottleSeconds { get; init; } = 0.5;

        /// <summary>Confirmation: within this time [s] after the start …</summary>
        public double ConfirmSeconds { get; init; } = 3;

        /// <summary>… reach at least this GPS speed [km/h] …</summary>
        public double ConfirmMinSpeedKph { get; init; } = 30;

        /// <summary>… and cover at least this distance [m] within <see cref="DistanceSeconds"/>.</summary>
        public double ConfirmMinDistanceMeters { get; init; } = 20;

        /// <summary>Time window [s] for the distance check.</summary>
        public double DistanceSeconds { get; init; } = 5;
    }

    /// <summary>Channels of a run resampled onto a common time grid (same length, time in s).</summary>
    public sealed class StartSignals
    {
        public required double[] Time { get; init; }
        public required double[] SpeedKph { get; init; }
        public required double[] Gear { get; init; }
        public required double[] Rpm { get; init; }
        public required double[] Throttle { get; init; }
        public double[]? Clutch { get; init; }
        public double[]? RevLimitSource { get; init; }
        public double[]? LaunchActive { get; init; }
    }

    /// <summary>A detected start.</summary>
    public sealed record DetectedStart(
        double TimeSeconds, bool Valid, string Reason, double RpmAtLaunch, double MaxSpeed3sKph,
        double Distance5sMeters, bool LaunchControl);

    /// <summary>
    /// Detects <b>valid</b> starts (gate starts) instead of just "first time throttle ≥ clutch":
    /// <list type="number">
    ///   <item>Ready at the gate: standstill, gear engaged, high RPM – uninterrupted for ≥ 1.5 s (<see cref="StartDetectionOptions.StagedMinSeconds"/>).</item>
    ///   <item>Start: at most 1 s later (<see cref="StartDetectionOptions.StagedHoldSeconds"/>), within 0.5 s throttle ≥ 50 % and
    ///   throttle ≥ clutch travel, not in limiter mode.</item>
    ///   <item>Confirmation: ≥ 30 km/h within 3 s and ≥ 20 m within 5 s.</item>
    /// </list>
    /// Rejected candidates (e.g. pulling away from the pit box) are reported with a reason.
    /// </summary>
    public static class StartDetector
    {
        /// <summary>All start candidates of a run (valid and rejected), sorted by time.</summary>
        public static IReadOnlyList<DetectedStart> Detect(StartSignals s, StartDetectionOptions? options = null)
        {
            options ??= new StartDetectionOptions();
            var n = s.Time.Length;
            var result = new List<DetectedStart>();
            if (n < 2)
                return result;

            double stagedSince = double.NaN, lastReady = double.NegativeInfinity;
            var i = 0;
            while (i < n)
            {
                var t = s.Time[i];
                var staged = s.SpeedKph[i] < options.StagedMaxSpeedKph && s.Gear[i] >= 1 && s.Rpm[i] > options.StagedMinRpm;
                if (staged)
                {
                    if (double.IsNaN(stagedSince))
                        stagedSince = t;
                    if (t - stagedSince >= options.StagedMinSeconds)
                        lastReady = t;
                }
                else
                {
                    stagedSince = double.NaN;
                }

                // Trigger as before (throttle ≥ clutch travel, launch limiter still active), but only from the ready state at the gate.
                var throttle = s.Throttle[i];
                var trigger = t - lastReady <= options.StagedHoldSeconds
                              && (s.Clutch is null ? throttle >= options.LaunchMinThrottle : throttle >= s.Clutch[i])
                              && (s.RevLimitSource is null || Math.Abs(s.RevLimitSource[i] - 1) > 0.5);
                if (!trigger)
                {
                    i++;
                    continue;
                }

                var launchThrottle = MaxInWindow(s.Time, s.Throttle, t, t + options.LaunchThrottleSeconds);
                var maxSpeed = MaxInWindow(s.Time, s.SpeedKph, t, t + options.ConfirmSeconds);
                var distance = DistanceInWindow(s.Time, s.SpeedKph, t, t + options.DistanceSeconds);
                var launchControl = s.LaunchActive is not null && MaxInWindow(s.Time, s.LaunchActive, t - options.StagedHoldSeconds, t) >= 0.5;
                var valid = launchThrottle >= options.LaunchMinThrottle
                            && maxSpeed >= options.ConfirmMinSpeedKph && distance >= options.ConfirmMinDistanceMeters;
                var reason = valid
                    ? "gate start confirmed"
                    : launchThrottle < options.LaunchMinThrottle
                        ? $"no full throttle (max {launchThrottle:0} % in {options.LaunchThrottleSeconds:0.0} s)"
                        : $"no launch (max {maxSpeed:0} km/h in {options.ConfirmSeconds:0} s, {distance:0} m in {options.DistanceSeconds:0} s)";
                result.Add(new DetectedStart(t, valid, reason, s.Rpm[i], maxSpeed, distance, launchControl));

                // Next candidate only after the confirmation window and standing ready again.
                var next = t + options.DistanceSeconds;
                while (i < n && s.Time[i] < next)
                    i++;
                stagedSince = double.NaN;
                lastReady = double.NegativeInfinity;
            }

            return result;
        }

        /// <summary>
        /// Resamples a channel onto the time grid <paramref name="grid"/> using sample-and-hold (channels have different
        /// sample rates). Before the first sample, the first value is used.
        /// </summary>
        public static double[] Resample(double[] time, double[] data, double[] grid)
        {
            var result = new double[grid.Length];
            if (time.Length == 0 || data.Length == 0)
                return result;
            var count = Math.Min(time.Length, data.Length);
            var j = 0;
            for (var k = 0; k < grid.Length; k++)
            {
                while (j + 1 < count && time[j + 1] <= grid[k])
                    j++;
                var v = data[j];
                result[k] = double.IsFinite(v) ? v : 0;
            }
            return result;
        }

        /// <summary>Uniform time grid from <paramref name="start"/> to <paramref name="end"/>.</summary>
        public static double[] Grid(double start, double end, double step)
        {
            var count = Math.Max(0, (int)Math.Floor((end - start) / step) + 1);
            var grid = new double[count];
            for (var k = 0; k < count; k++)
                grid[k] = start + k * step;
            return grid;
        }

        private static double MaxInWindow(double[] time, double[] data, double from, double to)
        {
            var max = 0.0;
            for (var k = 0; k < time.Length; k++)
            {
                if (time[k] < from) continue;
                if (time[k] > to) break;
                max = Math.Max(max, data[k]);
            }
            return max;
        }

        private static double DistanceInWindow(double[] time, double[] speedKph, double from, double to)
        {
            var meters = 0.0;
            for (var k = 1; k < time.Length; k++)
            {
                if (time[k] <= from) continue;
                if (time[k - 1] >= to) break;
                meters += speedKph[k - 1] / 3.6 * (time[k] - time[k - 1]);
            }
            return meters;
        }
    }
}
