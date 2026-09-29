namespace Ktm.Racing.Core
{
    /// <summary>Zusammenfassung der renn-bewussten Verarbeitung eines Rider-Ordners (Option C).</summary>
    public sealed class RaceStintResult
    {
        /// <summary>Anzahl der als Rennen erkannten und durchgehend verarbeiteten Runs.</summary>
        public int RaceRunCount { get; init; }

        /// <summary>Anzahl der eigenständig verarbeiteten Nicht-Renn-Runs.</summary>
        public int NonRaceRunCount { get; init; }

        /// <summary>Gesamtzahl der Laps über alle Renn-Runs (durchgehende Nummerierung).</summary>
        public int TotalRaceLaps { get; init; }

        /// <summary>Gesamte Renndistanz [m] über alle Renn-Runs (durchgehende sVehicle-Distanz).</summary>
        public double TotalRaceDistanceMeters { get; init; }

        /// <summary>Pfade der als Rennen verarbeiteten Runs (in chronologischer Reihenfolge).</summary>
        public IReadOnlyList<string> RaceRunPaths { get; init; } = [];
    }

    /// <summary>
    /// Verarbeitet einen Rider-/Session-Ordner renn-bewusst (Option C): Die als Rennen erkannten
    /// Runs werden mit <b>durchgehender</b> <c>sVehicle</c>-Distanz und <b>fortlaufender</b>
    /// Lap-Nummerierung verarbeitet, sodass ein durch Sturz/Abwürgen fragmentiertes Rennen als
    /// eine zusammenhängende Einheit erscheint. Die Original-Ordnerstruktur bleibt unverändert
    /// (Runs werden nicht zusammengeführt).
    ///
    /// <para>Nicht-Renn-Runs (Blips, Standgas, Rollen, Sichtung, Auslauf) werden eigenständig
    /// verarbeitet (Offset 0) — wie bei der klassischen Einzel-Run-Verarbeitung.</para>
    /// </summary>
    public class RaceStintProcessor
    {
        private readonly RaceSegmentDetector _detector;

        public RaceStintProcessor(RaceSegmentDetector detector)
        {
            ArgumentNullException.ThrowIfNull(detector);
            _detector = detector;
        }

        /// <summary>Factory mit Standard-Abhängigkeiten.</summary>
        public static RaceStintProcessor Create() => new(RaceSegmentDetector.Create());

        /// <summary>
        /// Erkennt die Renn-Runs im Rider-Ordner und verarbeitet sie mit Continuity.
        /// </summary>
        /// <param name="riderPath">Rider-/Session-Ordner mit den Run_*-Unterordnern.</param>
        /// <param name="metadata">Zuvor ermittelte Run-Metadaten (Bike, Track, Fahrer).</param>
        /// <param name="trackLengthMeters">Track-Länge aus KOMPASS für die relative Distanzschwelle (0 = nur absolut).</param>
        public RaceStintResult ProcessRider(string riderPath, RunMetadata metadata, double trackLengthMeters = 0)
        {
            var detection = _detector.DetectRaces(riderPath, trackLengthMeters);

            // Renn-Runs chronologisch (numerisch nach Run-Nummer, robust gegen unterschiedliche Stellenzahl).
            var raceRuns = detection.Runs
                .Where(r => r.Verdict == RaceRunVerdict.Race)
                .OrderBy(r => RunNumber(r.RunFolderName))
                .ToList();
            var nonRaceRuns = detection.Runs
                .Where(r => r.Verdict == RaceRunVerdict.NoRace)
                .ToList();

            Console.WriteLine($"[stint] {raceRuns.Count} Renn-Run(s), {nonRaceRuns.Count} Nicht-Renn-Run(s).");

            var xmlEditor = new XmlRunEditor();
            var calculator = new ImportDataCalculator();

            // --- Renn-Runs: durchgehende Distanz + fortlaufende Lap-Nummerierung ---
            double distanceOffset = 0.0;
            int lapOffset = 0;
            for (int i = 0; i < raceRuns.Count; i++)
            {
                var run = raceRuns[i];
                bool isLast = i == raceRuns.Count - 1;

                // Berechnung ZUERST (schreibt importData.ztx + RunLapHeader-Overlay und liefert die
                // per-Lap-Distanzen), danach die XML-Properties mit fortlaufender Lap-Nummerierung.
                var calc = calculator.RunCalculationPipeline(run.RunPath, distanceOffset);
                int laps = xmlEditor.SetRunProperties(
                    run.RunPath, metadata.BikeName, metadata.TrackName,
                    metadata.TrackLength, metadata.RiderName, lapOffset, isLast, calc.LapDistancesMeters);

                Console.WriteLine(
                    $"[stint] {run.RunFolderName}: Laps {lapOffset + 1}..{lapOffset + laps}, " +
                    $"sVehicle {distanceOffset:F0}..{distanceOffset + calc.RunDistanceMeters:F0} m");

                lapOffset += laps;
                distanceOffset += calc.RunDistanceMeters;
            }

            // --- Nicht-Renn-Runs: eigenständig (Offset 0), unverändertes Verhalten ---
            foreach (var run in nonRaceRuns)
            {
                Console.WriteLine($"[stint] {run.RunFolderName}: kein Rennen ({run.Reason}) — eigenständige Verarbeitung.");
                var calc = calculator.RunCalculationPipeline(run.RunPath);
                xmlEditor.SetRunProperties(
                    run.RunPath, metadata.BikeName, metadata.TrackName,
                    metadata.TrackLength, metadata.RiderName, 0, true, calc.LapDistancesMeters);
            }

            return new RaceStintResult
            {
                RaceRunCount = raceRuns.Count,
                NonRaceRunCount = nonRaceRuns.Count,
                TotalRaceLaps = lapOffset,
                TotalRaceDistanceMeters = distanceOffset,
                RaceRunPaths = raceRuns.Select(r => r.RunPath).ToList().AsReadOnly(),
            };
        }

        /// <summary>Extrahiert die numerische Run-Nummer aus dem Ordnernamen ("Run_1083" → 1083).</summary>
        private static int RunNumber(string folderName)
        {
            var digits = new string(folderName.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out int n) ? n : 0;
        }
    }
}
