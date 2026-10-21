namespace Ktm.Racing.Oracle
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

        /// <summary>GPS-Distanz je Lap [m] für jeden verarbeiteten Run (Grundlage der Track-Längen-Schätzung).</summary>
        public IReadOnlyList<IReadOnlyList<double>> LapDistancesPerRun { get; init; } = [];

        /// <summary>Alle verarbeiteten Run-Pfade (Renn- und Nicht-Renn-Runs).</summary>
        public IReadOnlyList<string> ProcessedRunPaths { get; init; } = [];

        /// <summary>Runs, deren Verarbeitung fehlschlug (Name: Ursache); die übrigen Runs wurden trotzdem verarbeitet.</summary>
        public IReadOnlyList<string> FailedRuns { get; init; } = [];
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
                .OrderBy(r => RunFolders.Number(r.RunFolderName))
                .ToList();
            var nonRaceRuns = detection.Runs
                .Where(r => r.Verdict == RaceRunVerdict.NoRace)
                .ToList();

            Console.WriteLine($"[stint] {raceRuns.Count} Renn-Run(s), {nonRaceRuns.Count} Nicht-Renn-Run(s).");

            var xmlEditor = new XmlRunEditor();
            var calculator = new ImportDataCalculator(_detector.Gps);
            var lapDistancesPerRun = new List<IReadOnlyList<double>>();
            var processedRuns = new List<string>();
            var failedRuns = new List<string>();

            // --- Renn-Runs: durchgehende Distanz + fortlaufende Lap-Nummerierung ---
            double distanceOffset = 0.0;
            int lapOffset = 0;
            for (int i = 0; i < raceRuns.Count; i++)
            {
                var run = raceRuns[i];
                bool isLast = i == raceRuns.Count - 1;

                // Berechnung ZUERST (schreibt importData.ztx + RunLapHeader-Overlay und liefert die
                // per-Lap-Distanzen), danach die XML-Properties mit fortlaufender Lap-Nummerierung.
                try
                {
                    var calc = calculator.RunCalculationPipeline(run.RunPath, distanceOffset, run.Data);
                    int laps = xmlEditor.SetRunProperties(
                        run.RunPath, metadata.BikeName, metadata.TrackName,
                        metadata.TrackLength, metadata.RiderName, lapOffset, isLast, calc.LapDistancesMeters);
                    lapDistancesPerRun.Add(calc.LapDistancesMeters);
                    processedRuns.Add(run.RunPath);

                    Console.WriteLine(
                        $"[stint] {run.RunFolderName}: Laps {lapOffset + 1}..{lapOffset + laps}, " +
                        $"sVehicle {distanceOffset:F0}..{distanceOffset + calc.RunDistanceMeters:F0} m");

                    lapOffset += laps;
                    distanceOffset += calc.RunDistanceMeters;
                }
                catch (Exception ex)
                {
                    // Ein defekter Run darf die übrigen Runs des Geräts nicht verhindern.
                    Console.WriteLine($"[stint] ⚠️ {run.RunFolderName}: Verarbeitung fehlgeschlagen – {ex.Message}");
                    failedRuns.Add($"{run.RunFolderName}: {ex.Message}");
                }
            }

            // --- Nicht-Renn-Runs: eigenständig (Offset 0), unverändertes Verhalten ---
            foreach (var run in nonRaceRuns)
            {
                Console.WriteLine($"[stint] {run.RunFolderName}: kein Rennen ({run.Reason}) — eigenständige Verarbeitung.");
                try
                {
                    var calc = calculator.RunCalculationPipeline(run.RunPath, 0, run.Data);
                    xmlEditor.SetRunProperties(
                        run.RunPath, metadata.BikeName, metadata.TrackName,
                        metadata.TrackLength, metadata.RiderName, 0, true, calc.LapDistancesMeters);
                    lapDistancesPerRun.Add(calc.LapDistancesMeters);
                    processedRuns.Add(run.RunPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[stint] ⚠️ {run.RunFolderName}: Verarbeitung fehlgeschlagen – {ex.Message}");
                    failedRuns.Add($"{run.RunFolderName}: {ex.Message}");
                }
            }

            // --- Runs, die die Renn-Erkennung nicht lesen konnte: nicht verarbeitet, aber nie stillschweigend ---
            foreach (var run in detection.Runs.Where(r => r.Verdict == RaceRunVerdict.Error))
            {
                Console.WriteLine($"[stint] ⚠️ {run.RunFolderName}: nicht lesbar – {run.Reason}");
                failedRuns.Add($"{run.RunFolderName}: {run.Reason}");
            }

            return new RaceStintResult
            {
                RaceRunCount = raceRuns.Count,
                NonRaceRunCount = nonRaceRuns.Count,
                TotalRaceLaps = lapOffset,
                TotalRaceDistanceMeters = distanceOffset,
                RaceRunPaths = raceRuns.Select(r => r.RunPath).ToList().AsReadOnly(),
                LapDistancesPerRun = lapDistancesPerRun,
                ProcessedRunPaths = processedRuns,
                FailedRuns = failedRuns,
            };
        }

    }
}
