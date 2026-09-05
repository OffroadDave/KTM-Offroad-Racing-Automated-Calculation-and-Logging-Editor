using System.Text;
using Ktm.Racing.Core;
using Xunit;

namespace Ktm.Racing.Tests;

public class VchSettingsReaderTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), $"vch_{Guid.NewGuid():N}");

    public VchSettingsReaderTests() => Directory.CreateDirectory(_dir);

    private const string Sample =
@"<?xml version=""1.0""?>
<Library><VirtualChannels>
  <VCH><Name>TOil_max</Name><Properties>
    <Measure>°C</Measure><OutFormat>Dec</OutFormat><Decimals>0</Decimals><Freq>1 Hz</Freq>
  </Properties></VCH>
  <VCH><Name>OperatingHours</Name><Properties>
    <Measure>h</Measure><OutFormat>Dec</OutFormat><Decimals>1</Decimals><Freq>5 Hz</Freq>
  </Properties></VCH>
  <VCH><Name>NGearShifts</Name><Properties>
    <Measure/><OutFormat>Auto</OutFormat><Decimals>Auto</Decimals><Freq>Auto</Freq>
  </Properties></VCH>
</VirtualChannels></Library>";

    private string WriteVch(string fileName, string content, Encoding encoding)
    {
        var path = Path.Combine(_dir, fileName);
        File.WriteAllBytes(path, encoding.GetBytes(content));
        return path;
    }

    [Fact]
    public void Read_Latin1Encoded_ParsesUnitDecimalsFormatFrequency()
    {
        var path = WriteVch("lib.vch", Sample, Encoding.Latin1);

        var map = new VchSettingsReader().Read(new[] { path });

        Assert.Equal("°C", map["TOil_max"].Unit);
        Assert.Equal(0, map["TOil_max"].Decimals);
        Assert.Equal("Dec", map["TOil_max"].OutFormat);
        Assert.Equal("1 Hz", map["TOil_max"].Frequency);

        Assert.Equal("h", map["OperatingHours"].Unit);
        Assert.Equal(1, map["OperatingHours"].Decimals);

        Assert.Null(map["NGearShifts"].Decimals); // "Auto" -> null
        Assert.Equal("", map["NGearShifts"].Unit);
    }

    [Fact]
    public void Read_Utf8Encoded_AlsoParsesDegreeSign()
    {
        var path = WriteVch("lib_utf8.vch", Sample, new UTF8Encoding(false));

        var map = new VchSettingsReader().Read(new[] { path });

        Assert.Equal("°C", map["TOil_max"].Unit);
    }

    [Fact]
    public void Read_DuplicateChannelAcrossFiles_FirstWins()
    {
        var first = WriteVch("a.vch", Sample, Encoding.Latin1);
        var second = WriteVch("b.vch", Sample.Replace("<Decimals>0</Decimals>", "<Decimals>3</Decimals>"), Encoding.Latin1);

        var map = new VchSettingsReader().Read(new[] { first, second });

        Assert.Equal(0, map["TOil_max"].Decimals); // first file wins
    }

    [Fact]
    public void Read_MissingFile_IsIgnored()
        => Assert.Empty(new VchSettingsReader().Read(new[] { Path.Combine(_dir, "does_not_exist.vch") }));

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch { }
    }
}
