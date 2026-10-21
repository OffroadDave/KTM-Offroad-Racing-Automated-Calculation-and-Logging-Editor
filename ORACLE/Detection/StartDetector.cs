namespace Ktm.Racing.Oracle
{
    /// <summary>Schwellen der Start-Erkennung (identisch zur VCH <c>KTM_ORR_ValidStart.vch</c>).</summary>
    public sealed class StartDetectionOptions
    {
        /// <summary>Stillstand: GPS-Geschwindigkeit darunter [km/h].</summary>
        public double StagedMaxSpeedKph { get; init; } = 3;

        /// <summary>Am Gatter: Drehzahl darüber [rpm] (Leerlauf liegt deutlich darunter).</summary>
        public double StagedMinRpm { get; init; } = 4000;

        /// <summary>Mindestdauer [s], die das Bike vor dem Start ununterbrochen bereit am Gatter steht (Drehzahlaufbau ~2 s).</summary>
        public double StagedMinSeconds { get; init; } = 1.5;

        /// <summary>Der Start muss spätestens so lange [s] nach dem Bereitstehen erfolgen.</summary>
        public double StagedHoldSeconds { get; init; } = 1;

        /// <summary>Gas innerhalb von <see cref="LaunchThrottleSeconds"/> nach dem Auslösen mindestens [%].</summary>
        public double LaunchMinThrottle { get; init; } = 50;

        /// <summary>Zeitfenster [s] nach dem Auslösen für <see cref="LaunchMinThrottle"/>.</summary>
        public double LaunchThrottleSeconds { get; init; } = 0.5;

        /// <summary>Bestätigung: innerhalb dieser Zeit [s] nach dem Start …</summary>
        public double ConfirmSeconds { get; init; } = 3;

        /// <summary>… mindestens diese GPS-Geschwindigkeit [km/h] erreichen …</summary>
        public double ConfirmMinSpeedKph { get; init; } = 30;

        /// <summary>… und innerhalb von <see cref="DistanceSeconds"/> mindestens diese Strecke [m] zurücklegen.</summary>
        public double ConfirmMinDistanceMeters { get; init; } = 20;

        /// <summary>Zeitfenster [s] für die Streckenprüfung.</summary>
        public double DistanceSeconds { get; init; } = 5;
    }

    /// <summary>Auf ein gemeinsames Zeitraster gebrachte Kanäle eines Runs (gleiche Länge, Zeit in s).</summary>
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

    /// <summary>Ein erkannter Start.</summary>
    public sealed record DetectedStart(
        double TimeSeconds, bool Valid, string Reason, double RpmAtLaunch, double MaxSpeed3sKph,
        double Distance5sMeters, bool LaunchControl);

    /// <summary>
    /// Erkennt <b>gültige</b> Starts (Gatterstart) statt nur „erstes Mal Gas ≥ Kupplung“:
    /// <list type="number">
    ///   <item>Bereit am Gatter: Stillstand, Gang eingelegt, Drehzahl hoch – ununterbrochen ≥ 1,5 s (<see cref="StartDetectionOptions.StagedMinSeconds"/>).</item>
    ///   <item>Start: spätestens 1 s danach (<see cref="StartDetectionOptions.StagedHoldSeconds"/>), innerhalb 0,5 s Gas ≥ 50 % und
    ///   Gas ≥ Kupplungsweg, nicht im Begrenzer-Modus.</item>
    ///   <item>Bestätigung: innerhalb 3 s ≥ 30 km/h und innerhalb 5 s ≥ 20 m.</item>
    /// </list>
    /// Abgelehnte Kandidaten (z.B. Anfahren aus der Box) werden mit Grund gemeldet.
    /// </summary>
    public static class StartDetector
    {
        /// <summary>Alle Start-Kandidaten eines Runs (gültige und abgelehnte), zeitlich sortiert.</summary>
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

                // Auslösen wie bisher (Gas ≥ Kupplungsweg, Launch-Begrenzer noch aktiv), aber nur aus der Bereitschaft am Gatter.
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

                // Nächster Kandidat erst nach dem Bestätigungsfenster und erneutem Bereitstehen.
                var next = t + options.DistanceSeconds;
                while (i < n && s.Time[i] < next)
                    i++;
                stagedSince = double.NaN;
                lastReady = double.NegativeInfinity;
            }

            return result;
        }

        /// <summary>
        /// Bringt einen Kanal per Sample-and-Hold auf das Zeitraster <paramref name="grid"/> (Kanäle haben unterschiedliche
        /// Abtastraten). Vor dem ersten Sample wird der erste Wert verwendet.
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

        /// <summary>Gleichmäßiges Zeitraster von <paramref name="start"/> bis <paramref name="end"/>.</summary>
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
