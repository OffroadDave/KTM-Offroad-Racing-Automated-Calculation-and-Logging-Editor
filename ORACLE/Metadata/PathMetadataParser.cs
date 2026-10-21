using System.Text.RegularExpressions;

namespace Ktm.Racing.Oracle
{
    /// <summary>KTM metadata derived from the folder path of a run.</summary>
    /// <param name="TeamName">Team name (resolved via TeamMappings) or empty.</param>
    /// <param name="SeasonName">Season (four-digit year) or empty.</param>
    /// <param name="DisciplineName">Discipline if a path segment matches a KOMPASS discipline, otherwise empty.</param>
    /// <param name="EngineClass">Engine class (e.g. 250/450) or empty.</param>
    public sealed record PathMetadata(string TeamName, string SeasonName, string DisciplineName, string EngineClass);

    /// <summary>
    /// Detects team, season, discipline and engine class by their <b>pattern</b> in the path instead of by fixed
    /// positions – the folder structure varies (e.g. <c>…\01_KTM-FR\Wintax\2026\450\05_RT\&lt;Event&gt;\&lt;Session&gt;\&lt;Device&gt;\Run_x</c>).
    /// Only the part above the event folder is searched.
    /// </summary>
    public static class PathMetadataParser
    {
        private static readonly Regex Season = new(@"^(19|20)\d{2}$", RegexOptions.Compiled);
        private static readonly string[] EngineClasses = ["125", "150", "250", "300", "350", "450", "500"];

        /// <summary>Evaluates the path.</summary>
        /// <param name="runPath">Path of the run folder (or of a folder below/above it).</param>
        /// <param name="eventName">Name of the event folder (e.g. <c>RT22_GER-Teutschenthal</c>); empty = entire path.</param>
        /// <param name="teamMappings">Team folder name → team name.</param>
        /// <param name="disciplines">Known disciplines (KOMPASS keys, e.g. MX, SX).</param>
        public static PathMetadata Parse(string runPath, string eventName,
            IReadOnlyDictionary<string, string> teamMappings, IEnumerable<string> disciplines)
        {
            var parts = runPath.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries);
            var eventIndex = string.IsNullOrWhiteSpace(eventName)
                ? parts.Length
                : Array.FindLastIndex(parts, p => string.Equals(p, eventName, StringComparison.OrdinalIgnoreCase));
            var above = parts.Take(eventIndex < 0 ? parts.Length : eventIndex).ToArray();

            // Nearest matching segment above the event (from bottom to top).
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
