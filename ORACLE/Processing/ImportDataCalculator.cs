using KtmDataHandler.DataStorage;
using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;
using Ktm.Racing.Domain.Telemetry;

namespace Ktm.Racing.Oracle
{
    public class ImportDataCalculator
    {
        // Reader/writer are freshly created per run (avoids state leaks after ReleaseObjects)
        private IMarelliDataAccessWrite ztxAccessWrite = null!;
        private MarelliDataAccess? ztxAccessRead;

        private List<string> channelNames = new();

        private readonly GpsSettings _gps;
        private GpsCleanResult? runGps; // cleaned GPS of the current run (sVehicle → lap distances)
        private string? currentRunPath; // Current run path
        private IDataset resultDataset = new Dataset(); // Global variable for calculated results

        /// <summary>Creates the calculator; <paramref name="gps"/> = GPS channels and cleaning (section <c>Gps</c>).</summary>
        public ImportDataCalculator(GpsSettings? gps = null) => _gps = gps ?? new GpsSettings();

        /// <summary>
        /// Main function: performs all calculation steps for a single run.
        /// Creates fresh reader/writer instances per call so that the method
        /// can safely be called several times in a row.
        /// </summary>
        /// <param name="runPath">Path to the run folder (basis for loading the data)</param>
        /// <param name="distanceOffsetMeters">Start offset for sVehicle (continuity across race runs).</param>
        /// <param name="preloaded">
        /// Already read run data (e.g. from the race detection); then the run is not loaded again.
        /// <c>null</c> = load the run here.
        /// </param>
        /// <returns>Run distance (without offset) and the GPS distance per lap.</returns>
        public RunCalcResult RunCalculationPipeline(string runPath, double distanceOffsetMeters = 0, RunGpsData? preloaded = null)
        {
            currentRunPath = runPath;
            resultDataset = new Dataset();
            channelNames = new();
            runGps = null;
            ztxAccessRead = null;

            // Fresh writer per run (avoids state leaks after ReleaseObjects); the reader only if data is actually read.
            ztxAccessWrite = KtmDataHandlerFactory.CreateWriter();

            double runDistance = 0.0;
            double[] lapDistances = [];
            try
            {
                // 1. Take over or load run data (throw errors so that the caller counts the run as failed)
                var data = preloaded ?? LoadDataSet(runPath);
                channelNames = data.ChannelNames.ToList();

                // 2. Execute calculation functions in order
                runDistance = SVehicleCalc(data, distanceOffsetMeters);

                // 3. Distance per lap (from the same data source) for LapDistance
                lapDistances = ComputeLapDistances(runPath, data);

                // 4. Save results (only if calculations produced results)
                if (resultDataset.NumElements() > 0)
                {
                    SaveResults(resultDataset);
                    Console.WriteLine($"{resultDataset.NumElements()} calculated channel(s) saved.");
                }
                else
                {
                    Console.WriteLine("No calculation results to save.");
                }
            }
            finally
            {
                ztxAccessRead?.ReleaseObjects();
                ztxAccessWrite.ReleaseObjects();
            }
            return new RunCalcResult { RunDistanceMeters = runDistance, LapDistancesMeters = lapDistances };
        }

        /// <summary>
        /// Loads the run and reads the channel list, lap boundaries and the GPS channels.
        /// </summary>
        /// <param name="runPath">Path to the run folder</param>
        /// <exception cref="InvalidDataException">The run cannot be loaded.</exception>
        private RunGpsData LoadDataSet(string runPath)
        {
            var reader = OpenReader(runPath);
            Console.WriteLine($"Dataset successfully loaded from: {runPath}");

            var names = reader.GetChannelsNames() ?? [];
            Console.WriteLine("Channel list extracted from dataset.");

            var latName = GpsSettings.Resolve(names, _gps.LatitudeChannel);
            var lonName = GpsSettings.Resolve(names, _gps.LongitudeChannel);
            double[]? time = null, lat = null, lon = null;
            if (latName is not null && lonName is not null)
            {
                var ds = reader.GetChannels([latName, lonName]);
                time = ds.GetElement(latName)?.SignalData?.Time as double[];
                lat = ds.GetElement(latName)?.SignalData?.Data as double[];
                lon = ds.GetElement(lonName)?.SignalData?.Data as double[];
            }

            return new RunGpsData
            {
                ChannelNames = names,
                LapMarks = reader.GetLapMarks() ?? [],
                Time = time,
                Latitude = lat,
                Longitude = lon,
            };
        }

        /// <summary>Loads the run once (lazily) for read access.</summary>
        /// <exception cref="InvalidDataException">The run cannot be loaded.</exception>
        private MarelliDataAccess OpenReader(string runPath)
        {
            if (ztxAccessRead is not null)
                return ztxAccessRead;

            ztxAccessRead = KtmDataHandlerFactory.CreateAccess();
            ztxAccessRead.SetDataLocation([runPath]);
            var result = ztxAccessRead.LoadRun();
            if (result <= 0)
                throw new InvalidDataException($"LoadRun failed (result {result}): {runPath}");
            return ztxAccessRead;
        }

        /// <summary>
        /// Calculates the cumulative distance from GPS coordinates and adds it as a new channel.
        /// </summary>
        /// <param name="data">Channel list and GPS channels of the run.</param>
        /// <param name="distanceOffset">Offset [m] that is added to every sVehicle sample.</param>
        /// <returns>The distance [m] covered in this run (without offset).</returns>
        private double SVehicleCalc(RunGpsData data, double distanceOffset)
        {
            Console.WriteLine("Calculating cumulative distance from GPS...");

            // Exact channel names from the configuration (case-insensitive, no substring fallback).
            string? latChannelName = GpsSettings.Resolve(data.ChannelNames, _gps.LatitudeChannel);
            string? lonChannelName = GpsSettings.Resolve(data.ChannelNames, _gps.LongitudeChannel);
            if (latChannelName is null || lonChannelName is null)
            {
                var missing = new List<string>();
                if (latChannelName is null) missing.Add(_gps.LatitudeChannel);
                if (lonChannelName is null) missing.Add(_gps.LongitudeChannel);
                Console.WriteLine($"Missing channels: {string.Join(", ", missing)}. Calculation aborted.");
                return 0.0;
            }

            var latitudes = data.Latitude;
            var longitudes = data.Longitude;
            var time = data.Time;
            if (latitudes == null || longitudes == null || time == null)
            {
                Console.WriteLine("Channel data is not in the expected format.");
                return 0.0;
            }

            if (latitudes.Length != longitudes.Length || latitudes.Length != time.Length || latitudes.Length < 2)
            {
                Console.WriteLine("Latitude, longitude and time arrays must have the same length and at least 2 points.");
                return 0.0;
            }

            // Clean GPS (like Lat_Valid/Lon_Valid: remove samples without fix, bridge dropouts; additionally
            // merge repeated fixes and remove GPS jumps) and map the distance onto the channel grid.
            var gps = GpsCleaner.Clean(new GpsTrace(time, latitudes, longitudes), _gps.ToCleanOptions());
            runGps = gps;
            if (!gps.HasFix)
                Console.WriteLine("[gps] No GPS fix in this run (lat/lon = 0) – sVehicle stays 0 m, no LapDistance is written.");
            else if (gps.InvalidSamples > 0 || gps.SpikeFixes > 0 || gps.JumpCount > 0)
                Console.WriteLine($"[gps] Cleaned: {gps.Summary}.");

            double[] distance = gps.DistanceAt(time);
            double runDistance = gps.TotalMeters;

            // Add offset (continuity across race runs), only then round (avoids error accumulation).
            for (int i = 0; i < distance.Length; i++)
                distance[i] = Math.Round(distance[i] + distanceOffset, 2);
            // Create new timeseries for sVehicle
            var sVehicle = new KtmDataHandler.Models.Channels.Timeseries();
            sVehicle.SignalData.Name = "sVehicle";
            sVehicle.SignalData.Unit = "m";
            sVehicle.SignalData.TimeData = (time, distance);

            // Add to calculation dataset
            resultDataset.Add(sVehicle);

            Console.WriteLine($"sVehicle calculation completed (offset {distanceOffset:F0} m, +{runDistance:F0} m).");
            return runDistance;
        }

        /// <summary>
        /// Saves the calculated results as importData.ztx in the run.
        /// </summary>
        /// <param name="dataSet">Dataset with results</param>
        /// <exception cref="IOException">The write API reports errors (sVehicle was not written completely).</exception>
        private void SaveResults(IDataset dataSet)
        {
            Console.WriteLine("Saving results...");
            if (currentRunPath == null)
                throw new InvalidOperationException("runPath is not set.");
            ztxAccessWrite.SetDataLocation([currentRunPath]);
            var errors = KtmWriteErrors.Describe(ztxAccessWrite.AddImportDataToRun(dataSet));
            if (errors.Count > 0)
                throw new IOException($"AddImportDataToRun reported errors: {string.Join("; ", errors.Take(3))}");
        }

        /// <summary>
        /// GPS distance [m] per lap (1-based). Primarily from the cleaned run distance at the lap boundaries – this way the
        /// sum of the laps is exactly the sVehicle distance, and dropouts/jumps at a lap boundary are handled as in the run.
        /// Without lap boundaries: per lap via the per-lap channel query, also cleaned. Without a GPS fix the distance is
        /// unknown (<see cref="double.NaN"/>) – then an existing LapDistance is removed instead of writing "0 m".
        /// </summary>
        private double[] ComputeLapDistances(string runPath, RunGpsData data)
        {
            int lapCount = XmlRunEditor.OrderedLapFolders(runPath).Count;
            var distances = Enumerable.Repeat(double.NaN, lapCount).ToArray();
            if (runGps is { HasFix: false })
                return distances;

            if (runGps is not null && data.LapMarks.Count == lapCount + 1)
            {
                var atMarks = runGps.DistanceAt(data.LapMarks);
                for (int i = 0; i < lapCount; i++)
                    distances[i] = Math.Max(0.0, atMarks[i + 1] - atMarks[i]);
                return distances;
            }

            string? latName = GpsSettings.Resolve(channelNames, _gps.LatitudeChannel);
            string? lonName = GpsSettings.Resolve(channelNames, _gps.LongitudeChannel);
            if (latName is null || lonName is null)
            {
                Console.WriteLine($"Lap distance skipped: GPS channels ({_gps.LatitudeChannel}/{_gps.LongitudeChannel}) not available.");
                return distances;
            }

            var reader = OpenReader(runPath);
            for (uint n = 1; n <= lapCount; n++)
            {
                try
                {
                    // The NLaps overload returns a tuple (per-lap array, combined).
                    var (perLap, combined) = reader.GetChannels(
                        new[] { latName, lonName }, new uint[] { n }, null, false);
                    var ds = combined ?? (perLap is { Length: > 0 } ? perLap[0] : null);
                    var lat = ds?.GetElement(latName)?.SignalData?.Data as double[];
                    var lon = ds?.GetElement(lonName)?.SignalData?.Data as double[];
                    var time = ds?.GetElement(latName)?.SignalData?.Time as double[];
                    var lapGps = lat is null || lon is null || time is null
                        ? null
                        : GpsCleaner.Clean(new GpsTrace(time, lat, lon), _gps.ToCleanOptions());
                    distances[n - 1] = lapGps is { HasFix: true } ? lapGps.TotalMeters : double.NaN;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lap distance for lap {n} failed: {ex.Message}");
                }
            }
            return distances;
        }
    }
}
