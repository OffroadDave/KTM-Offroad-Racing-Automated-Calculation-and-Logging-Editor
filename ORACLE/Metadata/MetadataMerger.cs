namespace Ktm.Racing.Oracle
{
    /// <summary>A field whose .HED value was replaced by KOMPASS/path (or kept with <c>PreferHedMetadata</c>).</summary>
    /// <param name="Field">.HED key, e.g. <c>riderName</c>.</param>
    /// <param name="HedValue">Value from the .HED.</param>
    /// <param name="KompassValue">Value from KOMPASS or the path.</param>
    public sealed record MetadataDifference(string Field, string HedValue, string KompassValue);

    /// <summary>
    /// Merges the metadata read from an existing .HED with the metadata derived from path + KOMPASS.
    /// <para><b>Master data wins:</b> fields that KOMPASS or the path provide unambiguously (rider, team, season,
    /// discipline, engine class, abbreviation, country name) replace differing .HED values – otherwise e.g. an old
    /// placeholder such as "Test Rider" would remain forever. Fields from the event name (track, country code, type, CW) are only
    /// filled in if they are missing in the .HED. With <c>preferHed</c> (deliberate manual entries, e.g. substitute riders) all
    /// existing .HED values are kept.</para>
    /// </summary>
    public static class MetadataMerger
    {
        /// <summary>Applies <paramref name="derived"/> to <paramref name="hed"/> and returns the differing fields.</summary>
        public static IReadOnlyList<MetadataDifference> Merge(RunMetadata hed, RunMetadata derived, bool preferHed)
        {
            ArgumentNullException.ThrowIfNull(hed);
            ArgumentNullException.ThrowIfNull(derived);
            var differences = new List<MetadataDifference>();

            string Authoritative(string field, string current, string candidate)
            {
                if (string.IsNullOrWhiteSpace(candidate))
                    return current;
                if (string.IsNullOrWhiteSpace(current))
                    return candidate.Trim();
                if (string.Equals(current.Trim(), candidate.Trim(), StringComparison.Ordinal))
                    return current;
                differences.Add(new MetadataDifference(field, current.Trim(), candidate.Trim()));
                return preferHed ? current : candidate.Trim();
            }

            static string Fill(string current, string candidate) =>
                string.IsNullOrWhiteSpace(current) ? candidate : current;

            // Rider and team only from a rider found in KOMPASS (derived.RiderName set).
            if (!string.IsNullOrWhiteSpace(derived.RiderName))
            {
                hed.RiderName = Authoritative("riderName", hed.RiderName, derived.RiderName);
                hed.TeamName = Authoritative("teamName", hed.TeamName, derived.TeamName);
            }
            else
            {
                hed.TeamName = Fill(hed.TeamName, derived.TeamName);
            }

            hed.SeasonName = Authoritative("seasonName", hed.SeasonName, derived.SeasonName);
            hed.DisciplineName = Authoritative("disciplineName", hed.DisciplineName, derived.DisciplineName);
            hed.EngineClass = Authoritative("engineClass", hed.EngineClass, derived.EngineClass);
            hed.RiderAbbreviation = Authoritative("riderabbreviationName", hed.RiderAbbreviation, derived.RiderAbbreviation);
            hed.BikeNumber = Authoritative("bikeNumber", hed.BikeNumber, derived.BikeNumber);
            hed.CountryName = Authoritative("countryName", hed.CountryName, derived.CountryName);

            hed.TypeName = Fill(hed.TypeName, derived.TypeName);
            hed.CalendarWeek = Fill(hed.CalendarWeek, derived.CalendarWeek);
            hed.CountryCode = Fill(hed.CountryCode, derived.CountryCode);
            hed.TrackName = Fill(hed.TrackName, derived.TrackName);
            hed.Surface = Fill(hed.Surface, derived.Surface);
            return differences;
        }
    }
}
