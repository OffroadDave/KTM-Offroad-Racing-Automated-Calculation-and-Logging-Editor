namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Finds channels in a run's channel list by exact name (case-insensitive). Candidates are tried in order; there is no
    /// substring matching, so a similarly named channel (e.g. <c>Lat_Valid</c> for <c>Lat_dez</c>) is never picked by accident.
    /// </summary>
    public static class ChannelResolver
    {
        /// <summary>The first candidate present in <paramref name="names"/> (exact spelling preferred), or <c>null</c>.</summary>
        public static string? Find(IEnumerable<string> names, params string[] candidates)
        {
            ArgumentNullException.ThrowIfNull(names);
            var list = names as IReadOnlyCollection<string> ?? names.ToList();
            foreach (var candidate in candidates)
            {
                var match = list.FirstOrDefault(n => n.Equals(candidate, StringComparison.Ordinal))
                            ?? list.FirstOrDefault(n => n.Equals(candidate, StringComparison.OrdinalIgnoreCase));
                if (match is not null)
                    return match;
            }

            return null;
        }
    }
}
