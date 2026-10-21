namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Normalizes bike/rider aliases to the canonical form without spaces
    /// (e.g. <c>"AA 01"</c> → <c>"AA01"</c>).
    ///
    /// <para>A space in the device folder name is a known data error: otherwise it leads to
    /// an incorrect <c>RiderAbbreviation</c> (<c>"AA "</c> instead of <c>"AA"</c>) — causing the
    /// KOMPASS rider lookup to fail — and to spaces in the written
    /// <c>Name</c>/<c>Alias</c> properties, which cause problems downstream (MotoDB/Tableau).</para>
    /// </summary>
    public static class BikeAlias
    {
        /// <summary>Removes all whitespace characters from the alias (<c>"AA 01"</c> → <c>"AA01"</c>).</summary>
        public static string Normalize(string? alias)
            => string.IsNullOrEmpty(alias)
                ? string.Empty
                : new string(alias.Where(c => !char.IsWhiteSpace(c)).ToArray());

        /// <summary>True if the alias is already canonical (empty or without whitespace).</summary>
        public static bool IsCanonical(string? alias)
            => string.IsNullOrEmpty(alias) || !alias.Any(char.IsWhiteSpace);
    }
}
