using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using Ktm.Racing.Domain.Geo;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Time series of a weather model at one location. Times are in the logger clock (local time of the track or UTC, see
    /// <see cref="WeatherSettings.LoggerClock"/>). Precipitation is the sum over the interval that <b>ends</b> at the time stamp.
    /// Missing values are <see cref="double.NaN"/>.
    /// </summary>
    public sealed class WeatherSeries
    {
        public required DateTime[] Time { get; init; }
        public required double[] TemperatureC { get; init; }
        public required double[] HumidityPct { get; init; }
        public required double[] PrecipitationMm { get; init; }
        public required double[] WindSpeedKmh { get; init; }
        public required double[] WindDirectionDeg { get; init; }
        public required double[] PressureHpa { get; init; }

        /// <summary>Hourly ground surface temperature (estimate of the track temperature).</summary>
        public DateTime[] HourlyTime { get; init; } = [];
        public double[] GroundTemperatureC { get; init; } = [];

        /// <summary>Grid point actually used by the model.</summary>
        public double GridLatitude { get; init; }
        public double GridLongitude { get; init; }
        public double ElevationMeters { get; init; }

        /// <summary>UTC offset of the series times (0 for UTC).</summary>
        public int UtcOffsetSeconds { get; init; }

        /// <summary>Model name as requested (<c>best_match</c> when not configured).</summary>
        public string Model { get; init; } = "best_match";

        /// <summary>Length of a precipitation interval (spacing of the time stamps, 15 min by default).</summary>
        public TimeSpan Step => Time.Length > 1 ? Time[1] - Time[0] : TimeSpan.FromMinutes(15);
    }

    /// <summary>Source of historical weather data (replaceable in tests).</summary>
    public interface IWeatherSource
    {
        /// <summary>Short name for logs and the .HED (e.g. "Open-Meteo").</summary>
        string Name { get; }

        /// <summary>Weather series covering [<paramref name="from"/>, <paramref name="to"/>] (logger clock) at <paramref name="location"/>.</summary>
        /// <exception cref="WeatherUnavailableException">No data (no connection, API error, no values).</exception>
        Task<WeatherSeries> GetAsync(GeoCoordinate location, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    }

    /// <summary>Weather data could not be obtained; the weather step is retried on the next invocation.</summary>
    public sealed class WeatherUnavailableException(string message, Exception? inner = null) : Exception(message, inner);

    /// <summary>
    /// Open-Meteo client: one request per session window with 15-minute values (temperature, humidity, precipitation, wind,
    /// surface pressure) and hourly ground temperature. With <c>timezone=auto</c> the API interprets the window in the local
    /// time of the coordinate including daylight saving time – matching the logger clock.
    /// </summary>
    public sealed class OpenMeteoWeatherSource(WeatherSettings settings) : IWeatherSource
    {
        private static readonly HttpClient Http = CreateClient();

        private const string Minutely = "temperature_2m,relative_humidity_2m,precipitation,wind_speed_10m,wind_direction_10m,surface_pressure";
        private const string Hourly = "soil_temperature_0cm";

        public string Name => "Open-Meteo";

        public async Task<WeatherSeries> GetAsync(GeoCoordinate location, DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            var apiKey = string.IsNullOrWhiteSpace(settings.ApiKeyEnvironmentVariable)
                ? null
                : Environment.GetEnvironmentVariable(settings.ApiKeyEnvironmentVariable);
            var url = BuildUrl(settings, location, from, to, apiKey);

            Exception? last = null;
            for (var attempt = 1; attempt <= 2; attempt++)
            {
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeout.CancelAfter(TimeSpan.FromSeconds(Math.Max(1, settings.TimeoutSeconds)));
                try
                {
                    using var response = await Http.GetAsync(url, timeout.Token).ConfigureAwait(false);
                    var body = await response.Content.ReadAsStringAsync(timeout.Token).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                        return Parse(body, string.IsNullOrWhiteSpace(settings.Model) ? "best_match" : settings.Model);

                    last = new WeatherUnavailableException($"HTTP {(int)response.StatusCode}: {ReadReason(body)}");
                    // Client errors (invalid request, rate limit, licence) do not improve with a retry.
                    if (response.StatusCode is not (HttpStatusCode.InternalServerError or HttpStatusCode.BadGateway
                        or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout))
                        break;
                }
                catch (HttpRequestException ex)
                {
                    last = new WeatherUnavailableException($"no connection to {new Uri(settings.BaseUrl).Host}: {ex.Message}", ex);
                }
                catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
                {
                    last = new WeatherUnavailableException($"no response within {settings.TimeoutSeconds} s", ex);
                }

                if (attempt < 2)
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken).ConfigureAwait(false);
            }

            throw last as WeatherUnavailableException ?? new WeatherUnavailableException("unknown error", last);
        }

        /// <summary>
        /// Request URL for the window: 15-minute values from the interval start before <paramref name="from"/> to one interval after
        /// <paramref name="to"/> (the precipitation of an interval is stamped at its end), hourly values covering the window.
        /// </summary>
        internal static string BuildUrl(WeatherSettings settings, GeoCoordinate location, DateTime from, DateTime to, string? apiKey)
        {
            var step = TimeSpan.FromMinutes(15);
            var start = Floor(from, step);
            var end = Floor(to, step) + step + step;
            var startHour = Floor(from, TimeSpan.FromHours(1));
            var endHour = Floor(to, TimeSpan.FromHours(1)) + TimeSpan.FromHours(1);

            var query = new StringBuilder();
            void Add(string name, string value) => query.Append(query.Length == 0 ? '?' : '&').Append(name).Append('=').Append(Uri.EscapeDataString(value));
            Add("latitude", location.Latitude.ToString("0.#####", CultureInfo.InvariantCulture));
            Add("longitude", location.Longitude.ToString("0.#####", CultureInfo.InvariantCulture));
            Add("minutely_15", Minutely);
            Add("hourly", Hourly);
            Add("start_minutely_15", Format(start));
            Add("end_minutely_15", Format(end));
            Add("start_hour", Format(startHour));
            Add("end_hour", Format(endHour));
            Add("timezone", settings.LoggerClockIsUtc ? "GMT" : "auto");
            if (!string.IsNullOrWhiteSpace(settings.Model))
                Add("models", settings.Model.Trim());
            if (!string.IsNullOrWhiteSpace(apiKey))
                Add("apikey", apiKey.Trim());
            return settings.BaseUrl.TrimEnd('?') + query;
        }

        /// <summary>Parses an Open-Meteo response.</summary>
        /// <exception cref="WeatherUnavailableException">Error response or no 15-minute values.</exception>
        internal static WeatherSeries Parse(string json, string model)
        {
            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(json);
            }
            catch (JsonException ex)
            {
                throw new WeatherUnavailableException($"invalid response: {ex.Message}", ex);
            }

            using (document)
            {
                var root = document.RootElement;
                if (root.TryGetProperty("error", out var error) && error.ValueKind == JsonValueKind.True)
                    throw new WeatherUnavailableException(ReadReason(json));
                if (!root.TryGetProperty("minutely_15", out var minutely) || !minutely.TryGetProperty("time", out var timeElement))
                    throw new WeatherUnavailableException("response without 15-minute values");

                var time = ReadTimes(timeElement);
                if (time.Length == 0)
                    throw new WeatherUnavailableException("response without 15-minute values");

                DateTime[] hourlyTime = [];
                double[] ground = [];
                if (root.TryGetProperty("hourly", out var hourly) && hourly.TryGetProperty("time", out var hourlyTimes))
                {
                    hourlyTime = ReadTimes(hourlyTimes);
                    ground = ReadValues(hourly, "soil_temperature_0cm", hourlyTime.Length);
                }

                return new WeatherSeries
                {
                    Time = time,
                    TemperatureC = ReadValues(minutely, "temperature_2m", time.Length),
                    HumidityPct = ReadValues(minutely, "relative_humidity_2m", time.Length),
                    PrecipitationMm = ReadValues(minutely, "precipitation", time.Length),
                    WindSpeedKmh = ReadValues(minutely, "wind_speed_10m", time.Length),
                    WindDirectionDeg = ReadValues(minutely, "wind_direction_10m", time.Length),
                    PressureHpa = ReadValues(minutely, "surface_pressure", time.Length),
                    HourlyTime = hourlyTime,
                    GroundTemperatureC = ground,
                    GridLatitude = Number(root, "latitude"),
                    GridLongitude = Number(root, "longitude"),
                    ElevationMeters = Number(root, "elevation"),
                    UtcOffsetSeconds = (int)(double.IsNaN(Number(root, "utc_offset_seconds")) ? 0 : Number(root, "utc_offset_seconds")),
                    Model = model,
                };
            }
        }

        private static DateTime[] ReadTimes(JsonElement array) =>
            array.ValueKind != JsonValueKind.Array
                ? []
                : array.EnumerateArray()
                    .Select(t => DateTime.ParseExact(t.GetString() ?? "", "yyyy-MM-dd'T'HH:mm", CultureInfo.InvariantCulture))
                    .ToArray();

        private static double[] ReadValues(JsonElement section, string name, int length)
        {
            var values = Enumerable.Repeat(double.NaN, length).ToArray();
            if (!section.TryGetProperty(name, out var array) || array.ValueKind != JsonValueKind.Array)
                return values;
            var i = 0;
            foreach (var item in array.EnumerateArray())
            {
                if (i >= length) break;
                values[i++] = item.ValueKind == JsonValueKind.Number ? item.GetDouble() : double.NaN;
            }
            return values;
        }

        private static double Number(JsonElement root, string name) =>
            root.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Number ? value.GetDouble() : double.NaN;

        private static string ReadReason(string body)
        {
            try
            {
                using var document = JsonDocument.Parse(body);
                if (document.RootElement.TryGetProperty("reason", out var reason))
                    return reason.GetString() ?? "error";
            }
            catch (JsonException)
            {
                // not JSON – fall through
            }
            return body.Length > 200 ? body[..200] : body;
        }

        private static DateTime Floor(DateTime value, TimeSpan step) => new(value.Ticks - value.Ticks % step.Ticks, value.Kind);

        private static string Format(DateTime value) => value.ToString("yyyy-MM-dd'T'HH:mm", CultureInfo.InvariantCulture);

        private static HttpClient CreateClient()
        {
            var client = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("ORACLE/1.0 (KTM Offroad Racing postprocessing)");
            return client;
        }
    }
}
