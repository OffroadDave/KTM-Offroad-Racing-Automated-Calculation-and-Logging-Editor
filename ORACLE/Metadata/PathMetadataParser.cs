using System.Text.RegularExpressions;

namespace Ktm.Racing.Oracle
{
    /// <summary>Aus dem Ordnerpfad eines Runs abgeleitete KTM-Metadaten.</summary>
    /// <param name="TeamName">Teamname (über TeamMappings aufgelöst) oder leer.</param>
    /// <param name="SeasonName">Saison (vierstelliges Jahr) oder leer.</param>
    /// <param name="DisciplineName">Disziplin, wenn ein Pfadsegment einer KOMPASS-Disziplin entspricht, sonst leer.</param>
    /// <param name="EngineClass">Motorklasse (z.B. 250/450) oder leer.</param>
    public sealed record PathMetadata(string TeamName, string SeasonName, string DisciplineName, string EngineClass);

    /// <summary>
    /// Erkennt Team, Saison, Disziplin und Motorklasse anhand ihres <b>Musters</b> im Pfad statt über feste
    /// Positionen – die Ordnerstruktur variiert (z.B. <c>…\01_KTM-FR\Wintax\2026\450\05_RT\&lt;Event&gt;\&lt;Session&gt;\&lt;Device&gt;\Run_x</c>).
    /// Gesucht wird nur oberhalb des Event-Ordners.
    /// </summary>
    public static class PathMetadataParser
    {
        private static readonly Regex Season = new(@"^(19|20)\d{2}$", RegexOptions.Compiled);
        private static readonly string[] EngineClasses = ["125", "150", "250", "300", "350", "450", "500"];

        /// <summary>Wertet den Pfad aus.</summary>
        /// <param name="runPath">Pfad des Run-Ordners (oder eines Ordners darunter/darüber).</param>
        /// <param name="eventName">Name des Event-Ordners (z.B. <c>RT22_GER-Teutschenthal</c>); leer = ganzer Pfad.</param>
        /// <param name="teamMappings">Team-Ordnername → Teamname.</param>
        /// <param name="disciplines">Bekannte Disziplinen (KOMPASS-Schlüssel, z.B. MX, SX).</param>
        public static PathMetadata Parse(string runPath, string eventName,
            IReadOnlyDictionary<string, string> teamMappings, IEnumerable<string> disciplines)
        {
            var parts = runPath.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries);
            var eventIndex = string.IsNullOrWhiteSpace(eventName)
                ? parts.Length
                : Array.FindLastIndex(parts, p => string.Equals(p, eventName, StringComparison.OrdinalIgnoreCase));
            var above = parts.Take(eventIndex < 0 ? parts.Length : eventIndex).ToArray();

            // Nächstes passendes Segment oberhalb des Events (von unten nach oben).
            string? Nearest(Func<string, bool> match) => above.Reverse().FirstOrDefault(match);

            var teamFolder = Nearest(p => teamMappings.ContainsKey(p));
            var team = teamFolder is null ? string.Empty : teamMappings[teamFolder];
            var season = Nearest(Season.IsMatch) ?? string.Empty;
            var known = disciplines.ToList();
            var discipline = Nearest(p => known.Contains(p, StringComparer.OrdinalIgnoreCase)) is { } d
                ? known.First(k => string.Equals(k, d, StringComparison.OrdinalIgnoreCase))
                : string.Empty;
            var engineClass = Nearest(p => EngineClasses.Contains(p)) ?? string.Empty;

            return new PathMetadata(team, season, discipline, engineClass);
        }
    }
}
