namespace Ktm.Racing.Oracle
{
    /// <summary>Weather over a time window (run or lap). Unknown values are <see cref="double.NaN"/>.</summary>
    public sealed record WeatherSummary
    {
        public DateTime Start { get; init; }
        public DateTime End { get; init; }
        public double AirTempC { get; init; } = double.NaN;
        public double AirTempStartC { get; init; } = double.NaN;
        public double AirTempEndC { get; init; } = double.NaN;
        public double GroundTempC { get; init; } = double.NaN;
        public double HumidityPct { get; init; } = double.NaN;
        public double PressureHpa { get; init; } = double.NaN;
        public double AirDensityKgM3 { get; init; } = double.NaN;
        public double WindSpeedKmh { get; init; } = double.NaN;
        public double WindDirectionDeg { get; init; } = double.NaN;
        public double RainMm { get; init; } = double.NaN;
        public double RainRateMaxMmPerHour { get; init; } = double.NaN;

        /// <summary><c>dry</c>, <c>damp</c> or <c>wet</c>; empty when the precipitation is unknown.</summary>
        public string TrackCondition { get; init; } = "";
    }

    /// <summary>Pure weather calculations on a <see cref="WeatherSeries"/> (no I/O).</summary>
    public static class WeatherCalculator
    {
        public const string Dry = "dry";
        public const string Damp = "damp";
        public const string Wet = "wet";

        /// <summary>Summarizes the weather over [<paramref name="start"/>, <paramref name="end"/>] (logger clock).</summary>
        public static WeatherSummary Summarize(WeatherSeries series, DateTime start, DateTime end, WeatherSettings settings)
        {
            if (end < start)
                (start, end) = (end, start);

            var temperature = Mean(series.Time, series.TemperatureC, start, end);
            var humidity = Mean(series.Time, series.HumidityPct, start, end);
            var pressure = Mean(series.Time, series.PressureHpa, start, end);
            var rain = RainSum(series.Time, series.PrecipitationMm, series.Step, start, end);
            var rate = MaxRate(series.Time, series.PrecipitationMm, series.Step, start, end);

            return new WeatherSummary
            {
                Start = start,
                End = end,
                AirTempC = temperature,
                AirTempStartC = ValueAt(series.Time, series.TemperatureC, start),
                AirTempEndC = ValueAt(series.Time, series.TemperatureC, end),
                GroundTempC = Mean(series.HourlyTime, series.GroundTemperatureC, start, end),
                HumidityPct = humidity,
                PressureHpa = pressure,
                AirDensityKgM3 = AirDensity(temperature, pressure, humidity),
                WindSpeedKmh = Mean(series.Time, series.WindSpeedKmh, start, end),
                WindDirectionDeg = MeanDirection(series.Time, series.WindDirectionDeg, start, end),
                RainMm = rain,
                RainRateMaxMmPerHour = rate,
                TrackCondition = Classify(rain, rate, settings),
            };
        }

        /// <summary>
        /// Track condition from the precipitation during the window: <c>wet</c> from <see cref="WeatherSettings.WetRainMm"/> or a
        /// rate of <see cref="WeatherSettings.WetRateMmPerHour"/>, <c>damp</c> from <see cref="WeatherSettings.DampRainMm"/>, else <c>dry</c>.
        /// </summary>
        public static string Classify(double rainMm, double rateMmPerHour, WeatherSettings settings)
        {
            if (double.IsNaN(rainMm))
                return "";
            if (rainMm >= settings.WetRainMm || (!double.IsNaN(rateMmPerHour) && rateMmPerHour >= settings.WetRateMmPerHour))
                return Wet;
            return rainMm >= settings.DampRainMm ? Damp : Dry;
        }

        /// <summary>Linear interpolation at <paramref name="at"/>; NaN outside the series (more than one step) or without valid neighbours.</summary>
        public static double ValueAt(DateTime[] time, double[] values, DateTime at)
        {
            var n = Math.Min(time.Length, values.Length);
            if (n == 0)
                return double.NaN;
            var tolerance = n > 1 ? time[1] - time[0] : TimeSpan.FromHours(1);

            int before = -1, after = -1;
            for (var i = 0; i < n; i++)
            {
                if (double.IsNaN(values[i])) continue;
                if (time[i] <= at) before = i;
                if (time[i] >= at) { after = i; break; }
            }

            if (before >= 0 && after >= 0)
            {
                if (before == after) return values[before];
                var fraction = (at - time[before]).TotalSeconds / (time[after] - time[before]).TotalSeconds;
                return values[before] + (values[after] - values[before]) * fraction;
            }
            if (before >= 0 && at - time[before] <= tolerance) return values[before];
            if (after >= 0 && time[after] - at <= tolerance) return values[after];
            return double.NaN;
        }

        /// <summary>Time-weighted mean of the interpolated series over [<paramref name="start"/>, <paramref name="end"/>] (trapezoid rule).</summary>
        public static double Mean(DateTime[] time, double[] values, DateTime start, DateTime end)
        {
            if (end <= start)
                return ValueAt(time, values, start);

            var points = new List<DateTime> { start };
            points.AddRange(time.Where(t => t > start && t < end));
            points.Add(end);

            double area = 0, covered = 0;
            for (var i = 1; i < points.Count; i++)
            {
                var a = ValueAt(time, values, points[i - 1]);
                var b = ValueAt(time, values, points[i]);
                if (double.IsNaN(a) || double.IsNaN(b)) continue;
                var seconds = (points[i] - points[i - 1]).TotalSeconds;
                area += (a + b) / 2 * seconds;
                covered += seconds;
            }
            return covered > 0 ? area / covered : double.NaN;
        }

        /// <summary>Mean direction [0, 360) as a vector mean (350° and 10° give 0°, not 180°).</summary>
        public static double MeanDirection(DateTime[] time, double[] degrees, DateTime start, DateTime end)
        {
            var sin = degrees.Select(d => Math.Sin(d * Math.PI / 180)).ToArray();
            var cos = degrees.Select(d => Math.Cos(d * Math.PI / 180)).ToArray();
            var s = Mean(time, sin, start, end);
            var c = Mean(time, cos, start, end);
            if (double.IsNaN(s) || double.IsNaN(c) || (Math.Abs(s) < 1e-9 && Math.Abs(c) < 1e-9))
                return double.NaN;
            var deg = Math.Atan2(s, c) * 180 / Math.PI;
            return deg < 0 ? deg + 360 : deg;
        }

        /// <summary>
        /// Precipitation within [<paramref name="start"/>, <paramref name="end"/>]: each value is the sum of the interval that ends at its
        /// time stamp and is counted proportionally to its overlap with the window. NaN when an overlapping interval is unknown.
        /// </summary>
        public static double RainSum(DateTime[] time, double[] precipitation, TimeSpan step, DateTime start, DateTime end)
        {
            double sum = 0;
            var any = false;
            for (var i = 0; i < Math.Min(time.Length, precipitation.Length); i++)
            {
                var overlap = Overlap(time[i] - step, time[i], start, end);
                if (overlap <= 0) continue;
                if (double.IsNaN(precipitation[i])) return double.NaN;
                sum += precipitation[i] * overlap / step.TotalSeconds;
                any = true;
            }
            return any ? sum : double.NaN;
        }

        /// <summary>Highest precipitation rate [mm/h] of the intervals overlapping the window.</summary>
        public static double MaxRate(DateTime[] time, double[] precipitation, TimeSpan step, DateTime start, DateTime end)
        {
            var max = double.NaN;
            for (var i = 0; i < Math.Min(time.Length, precipitation.Length); i++)
            {
                if (Overlap(time[i] - step, time[i], start, end) <= 0 || double.IsNaN(precipitation[i])) continue;
                var rate = precipitation[i] / step.TotalHours;
                max = double.IsNaN(max) ? rate : Math.Max(max, rate);
            }
            return max;
        }

        /// <summary>Saturation vapour pressure [hPa] (Magnus formula, over water).</summary>
        public static double SaturationVaporPressureHpa(double temperatureC) =>
            6.1078 * Math.Pow(10, 7.5 * temperatureC / (237.3 + temperatureC));

        /// <summary>Density of humid air [kg/m³] from temperature [°C], station pressure [hPa] and relative humidity [%].</summary>
        public static double AirDensity(double temperatureC, double pressureHpa, double humidityPct)
        {
            if (double.IsNaN(temperatureC) || double.IsNaN(pressureHpa))
                return double.NaN;
            var vapor = double.IsNaN(humidityPct) ? 0 : humidityPct / 100 * SaturationVaporPressureHpa(temperatureC);
            var kelvin = temperatureC + 273.15;
            return (pressureHpa - vapor) * 100 / (287.058 * kelvin) + vapor * 100 / (461.495 * kelvin);
        }

        /// <summary>Great-circle distance [km] (grid point of the model vs. track coordinate).</summary>
        public static double DistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0088;
            double ToRad(double d) => d * Math.PI / 180;
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                    + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return 2 * R * Math.Asin(Math.Min(1, Math.Sqrt(a)));
        }

        private static double Overlap(DateTime a0, DateTime a1, DateTime b0, DateTime b1)
        {
            var from = a0 > b0 ? a0 : b0;
            var to = a1 < b1 ? a1 : b1;
            return Math.Max(0, (to - from).TotalSeconds);
        }
    }
}
