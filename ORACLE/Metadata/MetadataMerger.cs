namespace Ktm.Racing.Oracle
{
    /// <summary>Ein Feld, dessen .HED-Wert durch KOMPASS/Pfad ersetzt wurde (oder bei <c>PreferHedMetadata</c> behalten).</summary>
    /// <param name="Field">.HED-Schlüssel, z.B. <c>riderName</c>.</param>
    /// <param name="HedValue">Wert aus der .HED.</param>
    /// <param name="KompassValue">Wert aus KOMPASS bzw. dem Pfad.</param>
    public sealed record MetadataDifference(string Field, string HedValue, string KompassValue);

    /// <summary>
    /// Führt die aus einer vorhandenen .HED gelesenen Metadaten mit den aus Pfad + KOMPASS ermittelten zusammen.
    /// <para><b>Stammdaten gewinnen:</b> Felder, die KOMPASS bzw. der Pfad eindeutig liefern (Rider, Team, Saison,
    /// Disziplin, Motorklasse, Kürzel, Ländername), ersetzen abweichende .HED-Werte – sonst bliebe z.B. ein alter
    /// Platzhalter wie „Test Rider“ für immer stehen. Felder aus dem Event-Namen (Track, Ländercode, Typ, KW) werden nur
    /// ergänzt, wenn sie in der .HED fehlen. Mit <c>preferHed</c> (bewusste manuelle Einträge, z.B. Ersatzfahrer) bleiben
    /// alle vorhandenen .HED-Werte erhalten.</para>
    /// </summary>
    public static class MetadataMerger
    {
        /// <summary>Übernimmt <paramref name="derived"/> in <paramref name="hed"/> und liefert die abweichenden Felder.</summary>
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

            // Rider und Team nur aus einem in KOMPASS gefundenen Rider (derived.RiderName gesetzt).
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
