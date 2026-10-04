namespace Ktm.Racing.Oracle.Tests;

public sealed class BikeAliasTests
{
    [Theory]
    [InlineData("AA 01", "AA01")]
    [InlineData("AA01", "AA01")]
    [InlineData("  AA  01  ", "AA01")]
    [InlineData("A\tB\n01", "AB01")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void Normalize_removes_all_whitespace(string? input, string expected)
        => Assert.Equal(expected, BikeAlias.Normalize(input));

    [Theory]
    [InlineData("AA01", true)]
    [InlineData("", true)]
    [InlineData(null, true)]
    [InlineData("AA 01", false)]
    [InlineData(" AA01", false)]
    [InlineData("AA01 ", false)]
    public void IsCanonical_detects_whitespace(string? input, bool expected)
        => Assert.Equal(expected, BikeAlias.IsCanonical(input));

    [Fact]
    public void Normalize_is_idempotent()
    {
        var once = BikeAlias.Normalize("AA 01");
        Assert.Equal(once, BikeAlias.Normalize(once));
        Assert.True(BikeAlias.IsCanonical(once));
    }
}
