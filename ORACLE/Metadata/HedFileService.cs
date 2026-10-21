namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Service für .HED-Datei-Operationen: Suchen, Kopieren, Lesen und Schreiben.
    /// 
    /// .HED-Dateien enthalten Metadaten im INI-ähnlichen Format:
    ///   [KTM_NAMING]
    ///   bikeName = JP01
    ///   trackName = Maggiora
    ///   trackLength = 1650
    ///   riderName = Jorge Prado
    /// </summary>
    public class HedFileService
    {
        /// <summary>
        /// Sucht eine .HED-Datei im Rider- und Run-Ordner.
        /// Falls im Rider-Ordner gefunden, wird die Datei in den Run-Ordner kopiert.
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner</param>
        /// <param name="riderPath">Pfad zum Rider-Ordner (eine Ebene über dem Run)</param>
        /// <returns>Pfad zur .HED-Datei im Run-Ordner, oder null wenn keine gefunden</returns>
        public string? FindAndPrepareHedFile(string runPath, string riderPath)
        {
            // 1. Suche im Rider-Ordner (primäre Quelle)
            var hedFile = FindFirstHedFile(riderPath);
            if (hedFile != null)
            {
                Console.WriteLine($".HED-File existiert in Riderordner: {Path.GetFileName(hedFile)}");
                return CopyToRunFolder(hedFile, runPath);
            }

            // 2. Fallback: Suche im Run-Ordner selbst
            hedFile = FindFirstHedFile(runPath);
            if (hedFile != null)
            {
                Console.WriteLine($".HED-File existiert im Runordner: {Path.GetFileName(hedFile)}");
                return hedFile;
            }

            return null;
        }

        /// <summary>
        /// Liest die Metadaten aus einer .HED-Datei: Fahrer-, Team-, Saison-, Disziplin- und Event-Stammdaten der Sektion
        /// <c>[KTM_NAMING]</c> sowie <c>surface</c>. Event, Session und Run kommen immer aus dem Pfad (eine aus dem Rider-Ordner
        /// kopierte .HED kann zu einem anderen Run gehören).
        /// </summary>
        /// <param name="hedFilePath">Vollständiger Pfad zur .HED-Datei</param>
        /// <returns>RunMetadata mit den gelesenen Werten (leere Strings für fehlende Keys)</returns>
        public RunMetadata ReadMetadata(string hedFilePath)
        {
            var values = ParseHedFile(hedFilePath);
            string Value(string key) => values.GetValueOrDefault(key, "");

            var rawBikeName = Value("bikeName");
            if (!BikeAlias.IsCanonical(rawBikeName))
                Console.WriteLine($"Warnung: bikeName \"{rawBikeName}\" aus .HED enthält Leerzeichen — normalisiere zu \"{BikeAlias.Normalize(rawBikeName)}\".");

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
        /// Kanonischer .HED-Dateiname: <c>{SeasonName}{EventName}_{BikeName}{SessionName}.HED</c>
        /// (ohne Leerzeichen), z.B. <c>2026RT22_GER-Teutschenthal_AA01R1.HED</c>.
        /// </summary>
        public static string BuildHedFileName(RunMetadata metadata)
            => $"{metadata.SeasonName}{metadata.EventName}_{metadata.BikeName}{metadata.SessionName}.HED";

        /// <summary>
        /// Stellt sicher, dass in JEDEM Run-Ordner genau eine .HED-Datei mit dem kanonischen Namen
        /// (<see cref="BuildHedFileName"/>) und normalisiertem Inhalt liegt. Inhaltsquelle ist die erste
        /// vorhandene .HED (Run- oder Rider-Ordner), sonst eine aus den Metadaten generierte.
        /// Abweichend benannte .HED-Dateien im Run-Ordner werden entfernt. Non-fatal je Run-Ordner.
        /// </summary>
        public void EnsureHedInAllRuns(IReadOnlyList<string> runPaths, string riderPath, RunMetadata metadata)
        {
            if (runPaths.Count == 0) return;

            var canonicalName = BuildHedFileName(metadata);

            // Quelle: bevorzugt die kanonische .HED eines Runs, sonst die neueste .HED der Runs, sonst die des Rider-Ordners.
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
                    // runNumber gehört zum einzelnen Run (Konvention: Ordnername, z.B. "Run_885").
                    var lines = SetValue((string[])template.Clone(), "runNumber", Path.GetFileName(Path.TrimEndingDirectorySeparator(runPath)));

                    // Erst die neue Datei vollständig schreiben, dann abweichend benannte .HED entfernen.
                    AtomicFile.WriteAllLines(Path.Combine(runPath, canonicalName), lines);
                    foreach (var old in Directory.EnumerateFiles(runPath, "*.HED", SearchOption.TopDirectoryOnly).ToList())
                        if (!string.Equals(Path.GetFileName(old), canonicalName, StringComparison.OrdinalIgnoreCase))
                            File.Delete(old);
                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[hed] ⚠️ HED für {Path.GetFileName(runPath)} übersprungen: {ex.Message}");
                }
            }
            Console.WriteLine($"[hed] .HED in {count}/{runPaths.Count} Run-Ordner(n) sichergestellt: \"{canonicalName}\".");
        }

        /// <summary>Setzt den Wert eines Schlüssels; fehlt er, wird er am Ende der ersten Sektion eingefügt.</summary>
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
        /// Übernimmt die bekannten (nicht leeren) Metadaten in vorhandene .HED-Zeilen, z.B. den kanonischen
        /// <c>bikeName</c> oder einen aus KOMPASS ermittelten <c>riderName</c>. Werte, die in den Metadaten leer sind,
        /// bleiben unverändert; fehlende Schlüssel werden nicht ergänzt (außer über die Track-Helfer).
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
        /// Setzt <c>surface</c> in allen .HED-Dateien eines Run-Ordners (siehe <see cref="EnsureTrackSurfaceLine"/>). Non-fatal.
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
                    Console.WriteLine($"[hed] ⚠️ surface in {Path.GetFileName(file)} nicht gesetzt: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Stellt sicher, dass der Track-Untergrund als <c>surface</c>-Zeile (unter einer <c>[TRACK]</c>-Sektion)
        /// im .HED-Inhalt vorhanden ist: aktualisiert eine vorhandene Zeile, sonst wird die Sektion angehängt.
        /// Leerer Untergrund lässt den Inhalt unverändert.
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

            // Vorhandene [TRACK]-Sektion ohne surface-Zeile: Zeile dort einfügen statt eine zweite Sektion anzuhängen.
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
        /// Setzt <c>trackLength</c> in allen .HED-Dateien eines Run-Ordners (siehe <see cref="EnsureTrackLengthLine"/>).
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
                    Console.WriteLine($"[hed] ⚠️ trackLength in {Path.GetFileName(file)} nicht gesetzt: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Setzt die <c>trackLength</c>-Zeile im .HED-Inhalt auf die geprüfte Länge (MotoDB vergleicht sie exakt mit
        /// CIRCUITS): aktualisiert eine vorhandene Zeile, sonst wird sie nach <c>trackName</c> eingefügt.
        /// Leere Länge lässt den Inhalt unverändert.
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
                // Ans Ende der ersten Sektion ([KTM_NAMING]), d.h. vor die nächste Sektionsüberschrift.
                var nextSection = list.FindIndex(l => l.TrimStart().StartsWith('['));
                if (nextSection >= 0)
                    nextSection = list.FindIndex(nextSection + 1, l => l.TrimStart().StartsWith('['));
                insertAt = nextSection >= 0 ? nextSection : list.Count;
            }
            list.Insert(insertAt, $"trackLength = {trackLength.Trim()}");
            return [.. list];
        }

        /// <summary>
        /// Parst eine .HED-Datei und gibt alle Key-Value-Paare zurück.
        /// Ignoriert Sektionsheader ([...]) und leere Zeilen.
        /// </summary>
        /// <param name="hedFilePath">Vollständiger Pfad zur .HED-Datei</param>
        /// <returns>Dictionary mit Key-Value-Paaren (case-insensitive Keys)</returns>
        public Dictionary<string, string> ParseHedFile(string hedFilePath)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in File.ReadLines(hedFilePath))
            {
                var trimmed = line.Trim();
                // Leere Zeilen und Sektionsheader ([KTM_NAMING]) überspringen
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('['))
                    continue;

                var idx = trimmed.IndexOf('=');
                // Ungültige Zeilen überspringen (kein '=', leerer Key oder leerer Value)
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
        /// Erzeugt eine .HED-Datei im Run-Ordner mit vollständigen Metadaten.
        /// Dateiname: <c>{SeasonName}{EventName}_{BikeName}{SessionName}.HED</c> (siehe <see cref="BuildHedFileName"/>).
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner</param>
        /// <param name="metadata">Vollständige Metadaten für die .HED-Datei</param>
        public void WriteHedFile(string runPath, RunMetadata metadata)
        {
            var hedFilePath = Path.Combine(runPath, BuildHedFileName(metadata));
            AtomicFile.WriteAllText(hedFilePath, BuildHedContent(metadata));
            Console.WriteLine($"Neues .HED-File erstellt: {hedFilePath}");
        }

        /// <summary>Erzeugt den vollständigen .HED-Inhalt (INI-Format) aus den Metadaten.</summary>
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

        // --- Private Hilfsmethoden ---

        /// <summary>
        /// Sucht die .HED-Datei eines Verzeichnisses (keine Rekursion); bei mehreren die zuletzt geänderte.
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
        /// Kopiert eine .HED-Datei in den Run-Ordner, falls noch nicht vorhanden.
        /// </summary>
        /// <returns>Pfad zur Zieldatei, oder null bei Fehler</returns>
        private static string? CopyToRunFolder(string sourceHedFile, string runPath)
        {
            var fileName = Path.GetFileName(sourceHedFile);
            var destFile = Path.Combine(runPath, fileName);

            // Datei bereits vorhanden → direkt verwenden
            if (File.Exists(destFile))
            {
                Console.WriteLine($"Datei {fileName} ist bereits im Runordner vorhanden.");
                return destFile;
            }

            try
            {
                File.Copy(sourceHedFile, destFile);
                Console.WriteLine($"Datei {fileName} wurde in den Runordner kopiert.");
                return destFile;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Kopieren der Datei: {ex.Message}");
                return null;
            }
        }
    }
}
