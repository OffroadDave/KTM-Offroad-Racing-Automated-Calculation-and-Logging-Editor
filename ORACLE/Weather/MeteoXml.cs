using System.Globalization;
using System.Text;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// WinTAX lap meteo record <c>Lap_*\Meteo.xml</c> (shown on the meteo page of "Edit Lap Information" and in the channel
    /// browser, available in VCH as <c>$METEOTEMPOUT</c>, <c>$METEOHUMOUT</c>, <c>$METEOBAR</c>, <c>$METEORAIN</c>,
    /// <c>$METEOWINDSP</c>, <c>$METEOWINDDIR</c>). Format as written by WinTAX:
    /// <code>
    /// &lt;?xml version="1.0"?&gt;
    /// &lt;Meteo&gt;&lt;TempIn/&gt;&lt;TempOut/&gt;&lt;WindSp/&gt;&lt;WindDir/&gt;&lt;Bar/&gt;&lt;HumIn/&gt;&lt;HumOut/&gt;&lt;Rain/&gt;&lt;/Meteo&gt;
    /// </code>
    /// The model has no separate "inside" sensor: <c>TempIn</c>/<c>HumIn</c> carry the same values as <c>TempOut</c>/<c>HumOut</c>.
    /// </summary>
    public static class MeteoXml
    {
        public const string FileName = "Meteo.xml";

        /// <summary>Meteo.xml content for a lap, or <c>null</c> when a required value is unknown.</summary>
        public static string? Build(WeatherSummary lap, WeatherSettings settings)
        {
            if (double.IsNaN(lap.AirTempC) || double.IsNaN(lap.HumidityPct) || double.IsNaN(lap.PressureHpa)
                || double.IsNaN(lap.WindSpeedKmh) || double.IsNaN(lap.RainMm))
                return null;

            var pressure = settings.MeteoPressureUnit.Equals("bar", StringComparison.OrdinalIgnoreCase)
                ? Format(lap.PressureHpa / 1000, "0.0000")
                : Format(lap.PressureHpa, "0.0");
            var wind = settings.MeteoWindSpeedUnit.Equals("ms", StringComparison.OrdinalIgnoreCase)
                ? Format(lap.WindSpeedKmh / 3.6, "0.0")
                : Format(lap.WindSpeedKmh, "0.0");
            // WinTAX documents the direction as "180E -> 180W": east positive, west negative.
            var direction = double.IsNaN(lap.WindDirectionDeg) ? 0
                : lap.WindDirectionDeg > 180 ? lap.WindDirectionDeg - 360 : lap.WindDirectionDeg;
            var temperature = Format(lap.AirTempC, "0.0");
            var humidity = Format(lap.HumidityPct, "0");

            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\"?>\r\n<Meteo>\r\n");
            Element(xml, "TempIn", temperature);
            Element(xml, "TempOut", temperature);
            Element(xml, "WindSp", wind);
            Element(xml, "WindDir", Format(direction, "0"));
            Element(xml, "Bar", pressure);
            Element(xml, "HumIn", humidity);
            Element(xml, "HumOut", humidity);
            Element(xml, "Rain", Format(lap.RainMm, "0.00"));
            xml.Append("</Meteo>\r\n");
            return xml.ToString();
        }

        /// <summary>Writes Meteo.xml into the lap folder (atomically); <c>false</c> when a required value is unknown.</summary>
        public static bool Write(string lapFolder, WeatherSummary lap, WeatherSettings settings)
        {
            var content = Build(lap, settings);
            if (content is null)
                return false;
            AtomicFile.WriteAllText(Path.Combine(lapFolder, FileName), content);
            return true;
        }

        private static void Element(StringBuilder xml, string name, string value) =>
            xml.Append('<').Append(name).Append('>').Append(value).Append("</").Append(name).Append(">\r\n");

        private static string Format(double value, string format)
        {
            var text = value.ToString(format, CultureInfo.InvariantCulture);
            return text == "-0" || text.StartsWith("-0.", StringComparison.Ordinal) && text.Trim('-', '0', '.').Length == 0 ? text.TrimStart('-') : text;
        }
    }
}
