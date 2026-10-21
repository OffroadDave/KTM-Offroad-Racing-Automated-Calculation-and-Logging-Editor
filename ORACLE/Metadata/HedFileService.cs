namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Service for .HED file operations: finding, copying, reading and writing.
    /// 
    /// .HED files contain metadata in an INI-like format:
    ///   [KTM_NAMING]
    ///   bikeName = JP01
    ///   trackName = Maggiora
    ///   trackLength = 1650
    ///   riderName = Jorge Prado
    /// </summary>
    public class HedFileService
    {
        /// <summary>
        /// Searches for a .HED file in the rider and run folders.
        /// If found in the rider folder, the file is copied to the run folder.
        /// </summary>
        /// <param name="runPath">Path to the run folder</param>
        /// <param name="riderPath">Path to the rider folder (one level above the run)</param>
        /// <returns>Path to the .HED file in the run folder, or null if none was found</returns>
        public string? FindAndPrepareHedFile(string runPath, string riderPath)
        {
            // 1. Search in the rider folder (primary source)
            var hedFile = FindFirstHedFile(riderPath);
            if (hedFile != null)
            {
                Console.WriteLine($".HED file exists in rider folder: {Path.GetFileName(hedFile)}");
                return CopyToRunFolder(hedFile, runPath);
            }

            // 2. Fallback: search in the run folder itself
            hedFile = FindFirstHedFile(runPath);
            if (hedFile != null)
            {
                Console.WriteLine($".HED file exists in run folder: {Path.GetFileName(hedFile)}");
                return hedFile;
            }

            return null;
        }

        /// <summary>
        /// Reads the metadata from a .HED file: rider, team, season, discipline and event master data of the section
        /// <c>[KTM_NAMING]</c> as well as <c>surface</c>. Event, session and run always come from the path (a .HED copied from the
        /// rider folder may belong to a different run).
        /// </summary>
        /// <param name="hedFilePath">Full path to the .HED file</param>
        /// <returns>RunMetadata with the values read (empty strings for missing keys)</returns>
        public RunMetadata ReadMetadata(string hedFilePath)
        {
            var values = ParseHedFile(hedFilePath);
            string Value(string key) => values.GetValueOrDefault(key, "");

            var rawBikeName = Value("bikeName");
            if (!BikeAlias.IsCanonical(rawBikeName))
                Console.WriteLine($"Warning: bikeName \"{rawBikeName}\" from .HED contains spaces — normalizing to \"{BikeAlias.Normalize(rawBikeName)}\".");

            return new RunMetadata
            {
                BikeName = BikeAlias.Normalize(rawBikeName),
                TrackName = Value("trackName"),
                TrackLength = Value("trackLength"),
                RiderName = Value("riderName"),
                RiderAbbreviation = Value("riderabbreviationName"),
                BikeNumber = Value("bikeNumber"),
                TeamName = Value("teamName"),
                SeasonName = Value("seasonName"),
                DisciplineName = Value("disciplineName"),
                EngineClass = Value("engineClass"),
                CalendarWeek = Value("calendarweek"),
                TypeName = Value("typeName"),
                CountryCode = Value("countrycode"),
                CountryName = Value("countryName"),
                Surface = Value("surface"),
            };
        }

        /// <summary>
        /// Canonical .HED file name: <c>{SeasonName}{EventName}_{BikeName}{SessionName}.HED</c>
        /// (without spaces), e.g. <c>2026RT22_GER-Teutschenthal_AA01R1.HED</c>.
        /// </summary>
        public static string BuildHedFileName(RunMetadata metadata)
            => $"{metadata.SeasonName}{metadata.EventName}_{metadata.BikeName}{metadata.SessionName}.HED";

        /// <summary>
        /// Ensures that EVERY run folder contains exactly one .HED file with the canonical name
        /// (<see cref="BuildHedFileName"/>) and normalized content. The content source is the first
        /// existing .HED (run or rider folder), otherwise one generated from the metadata.
        /// Differently named .HED files in the run folder are removed. Non-fatal per run folder.
        /// </summary>
        public void EnsureHedInAllRuns(IReadOnlyList<string> runPaths, string riderPath, RunMetadata metadata)
        {
            if (runPaths.Count == 0) return;

            var canonicalName = BuildHedFileName(metadata);

            // Source: preferably the canonical .HED of a run, otherwise the newest .HED of the runs, otherwise the rider folder's.
            var source = runPaths.Select(r => Path.Combine(r, canonicalName)).FirstOrDefault(File.Exists)
                         ?? runPaths.Select(FindFirstHedFile).OfType<string>().OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault()
                         ?? FindFirstHedFile(riderPath);
            var template = source != null && File.Exists(source)
                ? ApplyMetadataToLines(File.ReadAllLines(source), metadata)
                : BuildHedContent(metadata).Split('\n').Select(l => l.TrimEnd('\r')).ToArray();

            template = EnsureTrackSurfaceLine(template, metadata.Surface);
            template = EnsureTrackLengthLine(template, metadata.TrackLength);

            int count = 0;
            foreach (var runPath in runPaths)
            {
                try
                {
                    // runNumber belongs to the individual run (convention: folder name, e.g. "Run_885").
                    var lines = SetValue((string[])template.Clone(), "runNumber", Path.GetFileName(Path.TrimEndingDirectorySeparator(runPath)));

                    // First write the new file completely, then remove differently named .HED files.
                    AtomicFile.WriteAllLines(Path.Combine(runPath, canonicalName), lines);
                    foreach (var old in Directory.EnumerateFiles(runPath, "*.HED", SearchOption.TopDirectoryOnly).ToList())
                        if (!string.Equals(Path.GetFileName(old), canonicalName, StringComparison.OrdinalIgnoreCase))
                            File.Delete(old);
                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[hed] ⚠️ HED for {Path.GetFileName(runPath)} skipped: {ex.Message}");
                }
            }
            Console.WriteLine($"[hed] .HED ensured in {count}/{runPaths.Count} run folder(s): \"{canonicalName}\".");
        }

        /// <summary>Sets the value of a key; if it is missing, it is inserted at the end of the first section.</summary>
        internal static string[] SetValue(string[] lines, string key, string value)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                var eq = lines[i].IndexOf('=');
                if (eq > 0 && lines[i][..eq].Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    lines[i] = $"{lines[i][..eq].TrimEnd()} = {value}";
                    return lines;
                }
            }

            var list = lines.ToList();
            var firstSection = list.FindIndex(l => l.TrimStart().StartsWith('['));
            var nextSection = firstSection >= 0 ? list.FindIndex(firstSection + 1, l => l.TrimStart().StartsWith('[')) : -1;
            list.Insert(nextSection >= 0 ? nextSection : list.Count, $"{key} = {value}");
            return [.. list];
        }

        /// <summary>
        /// Applies the known (non-empty) metadata to existing .HED lines, e.g. the canonical
        /// <c>bikeName</c> or a <c>riderName</c> determined from KOMPASS. Values that are empty in the metadata
        /// remain unchanged; missing keys are not added (except via the track helpers).
        /// </summary>
        public static string[] ApplyMetadataToLines(string[] lines, RunMetadata metadata)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["bikeName"] = metadata.BikeName,
                ["bikeNumber"] = metadata.BikeNumber,
                ["riderName"] = metadata.RiderName,
                ["riderabbreviationName"] = metadata.RiderAbbreviation,
                ["teamName"] = metadata.TeamName,
                ["seasonName"] = metadata.SeasonName,
                ["disciplineName"] = metadata.DisciplineName,
                ["engineClass"] = metadata.EngineClass,
                ["calendarweek"] = metadata.CalendarWeek,
                ["typeName"] = metadata.TypeName,
                ["countrycode"] = metadata.CountryCode,
                ["countryName"] = metadata.CountryName,
                ["trackName"] = metadata.TrackName,
                ["eventName"] = metadata.EventName,
                ["sessionName"] = metadata.SessionName,
            };

            for (int i = 0; i < lines.Length; i++)
            {
                var eq = lines[i].IndexOf('=');
                if (eq <= 0) continue;
                var key = lines[i][..eq].Trim();
                if (values.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
                    lines[i] = $"{lines[i][..eq].TrimEnd()} = {value.Trim()}";
            }
            return lines;
        }

        /// <summary>
        /// Sets <c>surface</c> in all .HED files of a run folder (see <see cref="EnsureTrackSurfaceLine"/>). Non-fatal.
        /// </summary>
        public void SetTrackSurface(string runPath, string surface)
        {
            if (!Directory.Exists(runPath) || string.IsNullOrWhiteSpace(surface))
                return;
            foreach (var file in Directory.EnumerateFiles(runPath, "*.HED", SearchOption.TopDirectoryOnly).ToList())
            {
                try
                {
                    AtomicFile.WriteAllLines(file, EnsureTrackSurfaceLine(File.ReadAllLines(file), surface));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[hed] ⚠️ surface not set in {Path.GetFileName(file)}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Ensures that the track surface is present as a <c>surface</c> line (under a <c>[TRACK]</c> section)
        /// in the .HED content: updates an existing line, otherwise the section is appended.
        /// An empty surface leaves the content unchanged.
        /// </summary>
        private static string[] EnsureTrackSurfaceLine(string[] lines, string surface)
        {
            if (string.IsNullOrWhiteSpace(surface))
                return lines;

            for (int i = 0; i < lines.Length; i++)
            {
                var eq = lines[i].IndexOf('=');
                if (eq > 0 && lines[i][..eq].Trim().Equals("surface", StringComparison.OrdinalIgnoreCase))
                {
                    lines[i] = $"surface = {surface}";
                    return lines;
                }
            }

            // Existing [TRACK] section without surface line: insert the line there instead of appending a second section.
            var track = Array.FindIndex(lines, l => l.Trim().Equals("[TRACK]", StringComparison.OrdinalIgnoreCase));
            if (track >= 0)
            {
                var list = lines.ToList();
                list.Insert(track + 1, $"surface = {surface}");
                return [.. list];
            }

            return [.. lines, "[TRACK]", $"surface = {surface}"];
        }

        /// <summary>
        /// Sets <c>trackLength</c> in all .HED files of a run folder (see <see cref="EnsureTrackLengthLine"/>).
        /// Non-fatal.
        /// </summary>
        public void SetTrackLength(string runPath, string trackLength)
        {
            if (!Directory.Exists(runPath))
                return;
            foreach (var file in Directory.EnumerateFiles(runPath, "*.HED", SearchOption.TopDirectoryOnly).ToList())
            {
                try
                {
                    AtomicFile.WriteAllLines(file, EnsureTrackLengthLine(File.ReadAllLines(file), trackLength));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[hed] ⚠️ trackLength not set in {Path.GetFileName(file)}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Sets the <c>trackLength</c> line in the .HED content to the validated length (MotoDB compares it exactly with
        /// CIRCUITS): updates an existing line, otherwise it is inserted after <c>trackName</c>.
        /// An empty length leaves the content unchanged.
        /// </summary>
        public static string[] EnsureTrackLengthLine(string[] lines, string trackLength)
        {
            if (string.IsNullOrWhiteSpace(trackLength))
                return lines;

            int trackNameIndex = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                var eq = lines[i].IndexOf('=');
                if (eq <= 0) continue;
                var key = lines[i][..eq].Trim();
                if (key.Equals("trackLength", StringComparison.OrdinalIgnoreCase))
                {
                    lines[i] = $"{lines[i][..eq].TrimEnd()} = {trackLength.Trim()}";
                    return lines;
                }
                if (key.Equals("trackName", StringComparison.OrdinalIgnoreCase))
                    trackNameIndex = i;
            }

            var list = lines.ToList();
            var insertAt = trackNameIndex + 1;
            if (trackNameIndex < 0)
            {
                // At the end of the first section ([KTM_NAMING]), i.e. before the next section header.
                var nextSection = list.FindIndex(l => l.TrimStart().StartsWith('['));
                if (nextSection >= 0)
                    nextSection = list.FindIndex(nextSection + 1, l => l.TrimStart().StartsWith('['));
                insertAt = nextSection >= 0 ? nextSection : list.Count;
            }
            list.Insert(insertAt, $"trackLength = {trackLength.Trim()}");
            return [.. list];
        }

        /// <summary>
        /// Parses a .HED file and returns all key-value pairs.
        /// Ignores section headers ([...]) and empty lines.
        /// </summary>
        /// <param name="hedFilePath">Full path to the .HED file</param>
        /// <returns>Dictionary with key-value pairs (case-insensitive keys)</returns>
        public Dictionary<string, string> ParseHedFile(string hedFilePath)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in File.ReadLines(hedFilePath))
            {
                var trimmed = line.Trim();
                // Skip empty lines and section headers ([KTM_NAMING])
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('['))
                    continue;

                var idx = trimmed.IndexOf('=');
                // Skip invalid lines (no '=', empty key or empty value)
                if (idx <= 0 || idx == trimmed.Length - 1)
                    continue;

                var key = trimmed[..idx].Trim();
                var value = trimmed[(idx + 1)..].Trim();

                if (!string.IsNullOrEmpty(key))
                    values[key] = value;
            }

            return values;
        }

        /// <summary>
        /// Creates a .HED file in the run folder with complete metadata.
        /// File name: <c>{SeasonName}{EventName}_{BikeName}{SessionName}.HED</c> (see <see cref="BuildHedFileName"/>).
        /// </summary>
        /// <param name="runPath">Path to the run folder</param>
        /// <param name="metadata">Complete metadata for the .HED file</param>
        public void WriteHedFile(string runPath, RunMetadata metadata)
        {
            var hedFilePath = Path.Combine(runPath, BuildHedFileName(metadata));
            AtomicFile.WriteAllText(hedFilePath, BuildHedContent(metadata));
            Console.WriteLine($"New .HED file created: {hedFilePath}");
        }

        /// <summary>Builds the complete .HED content (INI format) from the metadata.</summary>
        private static string BuildHedContent(RunMetadata metadata) =>
$@"[KTM_NAMING]
disciplineName = {metadata.DisciplineName}
seasonName = {metadata.SeasonName}
calendarweek = {metadata.CalendarWeek}
teamName = {metadata.TeamName}
riderName = {metadata.RiderName}
riderabbreviationName = {metadata.RiderAbbreviation}
bikeNumber = {metadata.BikeNumber}
bikeName = {metadata.BikeName}
engineClass = {metadata.EngineClass}
typeName = {metadata.TypeName}
countrycode = {metadata.CountryCode}
countryName = {metadata.CountryName}
trackName = {metadata.TrackName}
trackLength = {metadata.TrackLength}
eventName = {metadata.EventName}
sessionName = {metadata.SessionName}
runNumber = {metadata.RunNumber}
[TRACK]
surface = {metadata.Surface}";

        // --- Private helper methods ---

        /// <summary>
        /// Finds the .HED file of a directory (no recursion); if there are several, the most recently modified one.
        /// </summary>
        private static string? FindFirstHedFile(string directory)
        {
            if (!Directory.Exists(directory))
                return null;

            return Directory.EnumerateFiles(directory, "*.HED", SearchOption.TopDirectoryOnly)
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .ThenBy(f => f, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();
        }

        /// <summary>
        /// Copies a .HED file to the run folder if it is not already there.
        /// </summary>
        /// <returns>Path to the target file, or null on error</returns>
        private static string? CopyToRunFolder(string sourceHedFile, string runPath)
        {
            var fileName = Path.GetFileName(sourceHedFile);
            var destFile = Path.Combine(runPath, fileName);

            // File already exists → use it directly
            if (File.Exists(destFile))
            {
                Console.WriteLine($"File {fileName} already exists in the run folder.");
                return destFile;
            }

            try
            {
                File.Copy(sourceHedFile, destFile);
                Console.WriteLine($"File {fileName} was copied to the run folder.");
                return destFile;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying the file: {ex.Message}");
                return null;
            }
        }
    }
}
