using KtmDataHandler.DataStorage;
using KtmDataHandler.Generic.Data;
using KtmDataHandler.Marelli.Sysma;
using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;
using KtmDataHandler.Models.Symbols;
using KtmSharedUtils.ObjectUtils;
using Ktm.Racing.Domain.Telemetry;

namespace Ktm.Racing.Oracle
{
    public class ImportDataCalculator
    {
        // Reader/Writer werden pro Run frisch erzeugt (vermeidet State-Leaks nach ReleaseObjects)
        private IMarelliDataAccessWrite ztxAccessWrite = null!;
        private IMarelliDataAccessRead ztxAccessRead = null!;

        private List<string> channelNames = new();

        // Expose channel names read-only for other classes

        public IReadOnlyList<string> ChannelNames => channelNames.AsReadOnly();

        private readonly GpsSettings _gps;
        private GpsCleanResult? runGps; // aufbereitetes GPS des aktuellen Runs (sVehicle → Lap-Distanzen)
        private string? currentRunPath; // Aktueller Run-Pfad
        private IDataset resultDataset = new Dataset(); // Globale Variable für berechnete Ergebnisse

        /// <summary>Erstellt den Rechner; <paramref name="gps"/> = GPS-Kanäle und Aufbereitung (Abschnitt <c>Gps</c>).</summary>
        public ImportDataCalculator(GpsSettings? gps = null) => _gps = gps ?? new GpsSettings();

        /// <summary>
        /// Hauptfunktion: Führt alle Berechnungsschritte für einen einzelnen Run aus.
        /// Erzeugt frische Reader/Writer-Instanzen pro Aufruf, damit die Methode
        /// sicher mehrfach hintereinander aufgerufen werden kann.
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner (Basis für das Laden der Daten)</param>
        /// <param name="distanceOffsetMeters">Start-Offset für sVehicle (Continuity über Renn-Runs).</param>
        /// <returns>Run-Distanz (ohne Offset) und die GPS-Distanz je Lap.</returns>
        public RunCalcResult RunCalculationPipeline(string runPath, double distanceOffsetMeters = 0)
        {
            currentRunPath = runPath;
            resultDataset = new Dataset();
            channelNames = new();
            runGps = null;

            // Frische Reader/Writer-Instanzen pro Run (vermeidet State-Leaks nach ReleaseObjects)
            ztxAccessRead = KtmDataHandlerFactory.CreateAccess();
            ztxAccessWrite = KtmDataHandlerFactory.CreateWriter();

            double runDistance = 0.0;
            double[] lapDistances = [];
            try
            {
                // 1. Datensatz laden (Fehler werfen, damit der Aufrufer den Run als fehlgeschlagen zählt)
                LoadDataSet(runPath);

                // 2. Berechnungsfunktionen der Reihe nach ausführen
                runDistance = SVehicleCalc(distanceOffsetMeters);

                // 3. Distanz je Lap (aus derselben geladenen Datenquelle) für LapDistance
                lapDistances = ComputeLapDistances(runPath);

                // 4. Ergebnisse speichern (nur wenn Berechnungen Ergebnisse erzeugt haben)
                if (resultDataset.NumElements() > 0)
                {
                    SaveResults(resultDataset);
                    Console.WriteLine($"{resultDataset.NumElements()} berechnete(r) Channel(s) gespeichert.");
                }
                else
                {
                    Console.WriteLine("Keine Berechnungsergebnisse zum Speichern.");
                }
            }
            finally
            {
                ztxAccessRead.ReleaseObjects();
                ztxAccessWrite.ReleaseObjects();
            }
            return new RunCalcResult { RunDistanceMeters = runDistance, LapDistancesMeters = lapDistances };
        }

        /// <summary>
        /// Führt die Berechnungspipeline für mehrere Runs sequentiell aus.
        /// Jeder Run wird mit frischen Reader/Writer-Instanzen verarbeitet,
        /// um State-Leaks zwischen Runs zu vermeiden.
        /// </summary>
        /// <param name="runPaths">Liste der Run-Pfade</param>
        /// <returns>Anzahl erfolgreich verarbeiteter Runs</returns>
        public int RunCalculationPipelineForRuns(IReadOnlyList<string> runPaths)
        {
            if (runPaths == null || runPaths.Count == 0)
            {
                Console.WriteLine("[calc] Keine Runs für Berechnungen angegeben.");
                return 0;
            }

            Console.WriteLine($"[calc] Starte Berechnungspipeline für {runPaths.Count} Run(s)...");
            int successCount = 0;

            for (int idx = 0; idx < runPaths.Count; idx++)
            {
                var runPath = runPaths[idx];
                Console.WriteLine($"[calc] ── Run {idx + 1}/{runPaths.Count}: {Path.GetFileName(runPath)}");
                try
                {
                    RunCalculationPipeline(runPath);
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[calc] ❌ Fehler bei {Path.GetFileName(runPath)}: {ex.Message}");
                }
            }

            Console.WriteLine($"[calc] Abgeschlossen: {successCount}/{runPaths.Count} Run(s) erfolgreich.");
            return successCount;
        }

        /// <summary>
        /// Lädt den Datensatz aus dem angegebenen Run-Ordner.
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner</param>
        /// <exception cref="InvalidDataException">Der Run kann nicht geladen werden.</exception>
        private void LoadDataSet(string runPath)
        {
            ztxAccessRead.SetDataLocation([runPath]);
            var result = ztxAccessRead.LoadRun();
            if (result <= 0)
                throw new InvalidDataException($"LoadRun failed (result {result}): {runPath}");
            Console.WriteLine($"Datensatz erfolgreich geladen aus: {runPath}");

            var names = ztxAccessRead.GetChannelsNames();
            if (names != null)
                channelNames = names.ToList();

            Console.WriteLine("Channelliste aus Datensatz extrahiert.");
        }

        /// <summary>
        /// Berechnung der kumulierten Distanz aus GPS-Koordinaten und Hinzufügen als neuen Channel.
        /// </summary>
        /// <param name="distanceOffset">Offset [m], der auf jede sVehicle-Stützstelle addiert wird.</param>
        /// <returns>Die in diesem Run zurückgelegte Distanz [m] (ohne Offset).</returns>
        private double SVehicleCalc(double distanceOffset)
        {
            Console.WriteLine("Kumulierte Distanz aus GPS wird berechnet...");

            // Exakte Kanalnamen aus der Konfiguration (Groß/Klein egal, kein Teilstring-Fallback).
            string? latChannelName = GpsSettings.Resolve(channelNames, _gps.LatitudeChannel);
            string? lonChannelName = GpsSettings.Resolve(channelNames, _gps.LongitudeChannel);
            if (latChannelName is null || lonChannelName is null)
            {
                var missing = new List<string>();
                if (latChannelName is null) missing.Add(_gps.LatitudeChannel);
                if (lonChannelName is null) missing.Add(_gps.LongitudeChannel);
                Console.WriteLine($"Fehlende Kanäle: {string.Join(", ", missing)}. Berechnung wird abgebrochen.");
                return 0.0;
            }
            var necessaryChannels = new[] { latChannelName, lonChannelName };

            IDataset processingDataset = ztxAccessRead.GetChannels(necessaryChannels);
            var latitudes = processingDataset.GetElement(latChannelName)?.SignalData?.Data as double[];
            var longitudes = processingDataset.GetElement(lonChannelName)?.SignalData?.Data as double[];
            var time = processingDataset.GetElement(latChannelName)?.SignalData?.Time as double[];
            if (latitudes == null || longitudes == null || time == null)
            {
                Console.WriteLine("Channel-Daten sind nicht im erwarteten Format.");
                return 0.0;
            }

            if (latitudes.Length != longitudes.Length || latitudes.Length != time.Length || latitudes.Length < 2)
            {
                Console.WriteLine("Latitude, Longitude und Zeit-Arrays müssen gleiche Länge und mindestens 2 Punkte haben.");
                return 0.0;
            }

            // GPS aufbereiten (wie Lat_Valid/Lon_Valid: Samples ohne Fix entfernen, Aussetzer überbrücken; zusätzlich
            // wiederholte Fixes zusammenfassen und GPS-Sprünge entfernen) und die Distanz auf das Kanalraster legen.
            var gps = GpsCleaner.Clean(new GpsTrace(time, latitudes, longitudes), _gps.ToCleanOptions());
            runGps = gps;
            if (!gps.HasFix)
                Console.WriteLine("[gps] Kein GPS-Fix in diesem Run (Lat/Lon = 0) – sVehicle und LapDistance sind 0 m.");
            else if (gps.InvalidSamples > 0 || gps.SpikeFixes > 0 || gps.JumpCount > 0)
                Console.WriteLine($"[gps] Aufbereitet: {gps.Summary}.");

            double[] distance = gps.DistanceAt(time);
            double runDistance = gps.TotalMeters;

            // Offset addieren (Continuity über Renn-Runs), dann erst runden (vermeidet Fehlerakkumulation).
            for (int i = 0; i < distance.Length; i++)
                distance[i] = Math.Round(distance[i] + distanceOffset, 2);
            // Create new timeseries for sVehicle
            var sVehicle = new KtmDataHandler.Models.Channels.Timeseries();
            sVehicle.SignalData.Name = "sVehicle";
            sVehicle.SignalData.Unit = "m";
            sVehicle.SignalData.TimeData = (time, distance);

            // Add to calculation dataset
            resultDataset.Add(sVehicle);

            Console.WriteLine($"Berechnung von sVehicle abgeschlossen (Offset {distanceOffset:F0} m, +{runDistance:F0} m).");
            return runDistance;
        }

        /// <summary>
        /// Speichert die berechneten Ergebnisse als importData.ztx im Run.
        /// </summary>
        /// <param name="dataSet">Datensatz mit Ergebnissen</param>
        /// <exception cref="IOException">Die Write-API meldet Fehler (sVehicle wurde nicht vollständig geschrieben).</exception>
        private void SaveResults(IDataset dataSet)
        {
            Console.WriteLine("Speichere Ergebnisse...");
            if (currentRunPath == null)
                throw new InvalidOperationException("runPath is not set.");
            ztxAccessWrite.SetDataLocation([currentRunPath]);
            var errors = KtmWriteErrors.Describe(ztxAccessWrite.AddImportDataToRun(dataSet));
            if (errors.Count > 0)
                throw new IOException($"AddImportDataToRun reported errors: {string.Join("; ", errors.Take(3))}");
        }

        /// <summary>
        /// GPS-Distanz [m] je Lap (1-basiert). Primär aus der aufbereiteten Run-Distanz an den Lap-Grenzen – dadurch ist die
        /// Summe der Laps exakt die sVehicle-Distanz, und Aussetzer/Sprünge an einer Lap-Grenze werden wie im Run behandelt.
        /// Ohne Lap-Grenzen: je Lap über die per-Lap-Kanalabfrage, ebenfalls aufbereitet. Ohne GPS-Fix ist die Distanz
        /// unbekannt (<see cref="double.NaN"/>) – dann wird eine vorhandene LapDistance entfernt statt „0 m“ zu schreiben.
        /// </summary>
        private double[] ComputeLapDistances(string runPath)
        {
            int lapCount = XmlRunEditor.OrderedLapFolders(runPath).Count;
            var distances = Enumerable.Repeat(double.NaN, lapCount).ToArray();
            if (runGps is { HasFix: false })
                return distances;

            if (runGps is not null && (ztxAccessRead as MarelliDataAccess)?.GetLapMarks() is { } marks && marks.Length == lapCount + 1)
            {
                var atMarks = runGps.DistanceAt(marks);
                for (int i = 0; i < lapCount; i++)
                    distances[i] = Math.Max(0.0, atMarks[i + 1] - atMarks[i]);
                return distances;
            }

            string? latName = GpsSettings.Resolve(channelNames, _gps.LatitudeChannel);
            string? lonName = GpsSettings.Resolve(channelNames, _gps.LongitudeChannel);
            if (latName is null || lonName is null)
            {
                Console.WriteLine($"Lap-Distanz übersprungen: GPS-Kanäle ({_gps.LatitudeChannel}/{_gps.LongitudeChannel}) nicht verfügbar.");
                return distances;
            }

            for (uint n = 1; n <= lapCount; n++)
            {
                try
                {
                    // Der NLaps-Overload liefert ein Tuple (per-Lap-Array, kombiniert).
                    var (perLap, combined) = ztxAccessRead.GetChannels(
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
                    Console.WriteLine($"Lap-Distanz für Lap {n} fehlgeschlagen: {ex.Message}");
                }
            }
            return distances;
        }
    }
}