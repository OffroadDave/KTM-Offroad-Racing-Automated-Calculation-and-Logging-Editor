namespace Ktm.Racing.Oracle
{
    /// <summary>Summary of the race-aware processing of a rider folder (option C).</summary>
    public sealed class RaceStintResult
    {
        /// <summary>Number of runs detected as races and processed continuously.</summary>
        public int RaceRunCount { get; init; }

        /// <summary>Number of non-race runs processed independently.</summary>
        public int NonRaceRunCount { get; init; }

        /// <summary>Total number of laps across all race runs (continuous numbering).</summary>
        public int TotalRaceLaps { get; init; }

        /// <summary>Total race distance [m] across all race runs (continuous sVehicle distance).</summary>
        public double TotalRaceDistanceMeters { get; init; }

        /// <summary>Paths of the runs processed as races (in chronological order).</summary>
        public IReadOnlyList<string> RaceRunPaths { get; init; } = [];

        /// <summary>GPS distance per lap [m] for each processed run (basis of the track length estimate).</summary>
        public IReadOnlyList<IReadOnlyList<double>> LapDistancesPerRun { get; init; } = [];

        /// <summary>All processed run paths (race and non-race runs).</summary>
        public IReadOnlyList<string> ProcessedRunPaths { get; init; } = [];

        /// <summary>Runs whose processing failed (name: cause); the remaining runs were processed anyway.</summary>
        public IReadOnlyList<string> FailedRuns { get; init; } = [];
    }

    /// <summary>
    /// Processes a rider/session folder in a race-aware way (option C): the runs detected as races
    /// are processed with a <b>continuous</b> <c>sVehicle</c> distance and <b>sequential</b>
    /// lap numbering, so that a race fragmented by a crash/stall appears as
    /// one contiguous unit. The original folder structure remains unchanged
    /// (runs are not merged).
    ///
    /// <para>Non-race runs (blips, idling, rolling, sighting, cool-down) are processed
    /// independently (offset 0) — like in classic single-run processing.</para>
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
        /// Detects the race runs in the rider folder and processes them with continuity.
        /// </summary>
        /// <param name="riderPath">Rider/session folder containing the Run_* subfolders.</param>
        /// <param name="metadata">Previously determined run metadata (bike, track, rider).</param>
        /// <param name="trackLengthMeters">Track length from KOMPASS for the relative distance threshold (0 = absolute only).</param>
        public RaceStintResult ProcessRider(string riderPath, RunMetadata metadata, double trackLengthMeters = 0)
        {
            var detection = _detector.DetectRaces(riderPath, trackLengthMeters);

            // Race runs in chronological order (numerically by run number, robust against different digit counts).
            var raceRuns = detection.Runs
                .Where(r => r.Verdict == RaceRunVerdict.Race)
                .OrderBy(r => RunFolders.Number(r.RunFolderName))
                .ToList();
            var nonRaceRuns = detection.Runs
                .Where(r => r.Verdict == RaceRunVerdict.NoRace)
                .ToList();

            Console.WriteLine($"[stint] {raceRuns.Count} race run(s), {nonRaceRuns.Count} non-race run(s).");

            var xmlEditor = new XmlRunEditor();
            var calculator = new ImportDataCalculator(_detector.Gps);
            var lapDistancesPerRun = new List<IReadOnlyList<double>>();
            var processedRuns = new List<string>();
            var failedRuns = new List<string>();

            // --- Race runs: continuous distance + sequential lap numbering ---
            double distanceOffset = 0.0;
            int lapOffset = 0;
            for (int i = 0; i < raceRuns.Count; i++)
            {
                var run = raceRuns[i];
                bool isLast = i == raceRuns.Count - 1;

                // Calculation FIRST (writes importData.ztx + RunLapHeader overlay and returns the
                // per-lap distances), then the XML properties with sequential lap numbering.
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
                    // A defective run must not prevent processing of the device's remaining runs.
                    Console.WriteLine($"[stint] ⚠️ {run.RunFolderName}: processing failed – {ex.Message}");
                    failedRuns.Add($"{run.RunFolderName}: {ex.Message}");
                }
            }

            // --- Non-race runs: independent (offset 0), unchanged behavior ---
            foreach (var run in nonRaceRuns)
            {
                Console.WriteLine($"[stint] {run.RunFolderName}: no race ({run.Reason}) — independent processing.");
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
                    Console.WriteLine($"[stint] ⚠️ {run.RunFolderName}: processing failed – {ex.Message}");
                    failedRuns.Add($"{run.RunFolderName}: {ex.Message}");
                }
            }

            // --- Runs that race detection could not read: not processed, but never silently ---
            foreach (var run in detection.Runs.Where(r => r.Verdict == RaceRunVerdict.Error))
            {
                Console.WriteLine($"[stint] ⚠️ {run.RunFolderName}: not readable – {run.Reason}");
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
