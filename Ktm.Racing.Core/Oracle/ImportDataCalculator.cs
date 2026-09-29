using KtmDataHandler.DataStorage;
using KtmDataHandler.Generic.Data;
using KtmDataHandler.Marelli.Sysma;
using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;
using KtmDataHandler.Models.Symbols;
using KtmSharedUtils.ObjectUtils;
using MathNet.Numerics.Interpolation;

namespace Ktm.Racing.Core
{
    public class ImportDataCalculator
    {
        // Reader/Writer werden pro Run frisch erzeugt (vermeidet State-Leaks nach ReleaseObjects)
        private IMarelliDataAccessWrite ztxAccessWrite = null!;
        private IMarelliDataAccessRead ztxAccessRead = null!;

        private List<string> channelNames = new();

        // Expose channel names read-only for other classes

        public IReadOnlyList<string> ChannelNames => channelNames.AsReadOnly();

        private const double EarthRadiusMeters = 6371000.0;
        private string? currentRunPath; // Aktueller Run-Pfad
        private IDataset resultDataset = new Dataset(); // Globale Variable für berechnete Ergebnisse

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

            // Frische Reader/Writer-Instanzen pro Run (vermeidet State-Leaks nach ReleaseObjects)
            ztxAccessRead = KtmDataHandlerFactory.CreateReader();
            ztxAccessWrite = KtmDataHandlerFactory.CreateWriter();

            double runDistance = 0.0;
            double[] lapDistances = [];
            try
            {
                // 1. Datensatz laden
                if (!LoadDataSet(runPath))
                {
                    Console.WriteLine("Datensatz konnte nicht geladen werden.");
                    return new RunCalcResult();
                }

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
        /// <returns>True bei Erfolg, false bei Fehler</returns>
        private bool LoadDataSet(string runPath)
        {
            try
            {
                // Setze den Datenpfad und lade den Run
                ztxAccessRead.SetDataLocation([runPath]);
                ztxAccessRead.LoadRun();
                Console.WriteLine($"Datensatz erfolgreich geladen aus: {runPath}");

                // Channel-Namen abfragen und speichern
                var names = ztxAccessRead.GetChannelsNames();
                if (names != null)
                    channelNames = names.ToList();

                Console.WriteLine("Channelliste aus Datensatz extrahiert.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden des Datensatzes: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Berechnung der kumulierten Distanz aus GPS-Koordinaten und Hinzufügen als neuen Channel.
        /// </summary>
        /// <param name="distanceOffset">Offset [m], der auf jede sVehicle-Stützstelle addiert wird.</param>
        /// <returns>Die in diesem Run zurückgelegte Distanz [m] (ohne Offset).</returns>
        private double SVehicleCalc(double distanceOffset)
        {
            Console.WriteLine("Kumulierte Distanz aus GPS wird berechnet...");

            // Setze die für diese Berechnung notwendigen Kanäle
            string LatChannelName = "Lat_dez";
            string LonChannelName = "Lon_dez";
            var necessaryChannels = new List<string> { LatChannelName, LonChannelName };  

            // Prüfe, ob alle notwendigen Kanäle vorhanden sind
            var missingChannels = necessaryChannels.Except(channelNames).ToList();
            if (missingChannels.Any())
            {
                Console.WriteLine($"Fehlende Kanäle: {string.Join(", ", missingChannels)}. Berechnung wird abgebrochen.");
                return 0.0;
            }
            // Daten vom Datensatz abrufen 
            IDataset processingDataset = ztxAccessRead.GetChannels(necessaryChannels.ToArray());
            Console.WriteLine($"Notwendige Kanäle: {string.Join(", ", necessaryChannels)} wurden erfolgreich geladen.");

            // Extract latitude and longitude signals
            ITimeseries latitudeSignal = processingDataset.GetElement(LatChannelName);
            ITimeseries longitudeSignal = processingDataset.GetElement(LonChannelName);

            if (latitudeSignal == null || longitudeSignal == null)
            {
                Console.WriteLine("Latitude oder Longitude Channel haben ungültige Werte.");
                return 0.0;
            }

            // Extract data arrays
            var latitudes = latitudeSignal.SignalData.Data as double[];
            var longitudes = longitudeSignal.SignalData.Data as double[];
            var time = latitudeSignal.SignalData.Time as double[];

            if (latitudes == null || longitudes == null || time == null)
            {
                Console.WriteLine("Channel-Daten sind nicht im erwarteten Format.");
                return 0.0;
            }

            // Interpolieren von Nullen mit Math.NET Numerics
            // Die Zeitreihe wird als X-Achse für die Interpolation verwendet
            latitudes = InterpolateZerosMathNet(latitudes, time, "Latitude");
            longitudes = InterpolateZerosMathNet(longitudes, time, "Longitude");

            if (latitudes.Length != longitudes.Length || latitudes.Length != time.Length || latitudes.Length < 2)
            {
                Console.WriteLine("Latitude, Longitude und Zeit-Arrays müssen gleiche Länge und mindestens 2 Punkte haben.");
                return 0.0;
            }

            // Convert to radians
            double[] latRad = latitudes.Select(x => Math.PI / 180.0 * x).ToArray();
            double[] lonRad = longitudes.Select(x => Math.PI / 180.0 * x).ToArray();

            // Calculate segment distances using Haversine formula
            double[] segmentDistances = new double[latRad.Length - 1];
            for (int i = 0; i < segmentDistances.Length; i++)
            {
                double dLat = latRad[i + 1] - latRad[i];
                double dLon = lonRad[i + 1] - lonRad[i];
                double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                           Math.Cos(latRad[i]) * Math.Cos(latRad[i + 1]) *
                           Math.Pow(Math.Sin(dLon / 2), 2);
                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                segmentDistances[i] = EarthRadiusMeters * c;
            }

            // Cumulative distance
            double[] distance = new double[latRad.Length];
            distance[0] = 0;
            for (int i = 1; i < distance.Length; i++)
                distance[i] = distance[i - 1] + segmentDistances[i - 1];

            // Eigene Renndistanz dieses Runs (vor Offset) für die stint-übergreifende Akkumulation.
            double runDistance = distance[^1];

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
        /// Speichert die berechneten Ergebnisse.
        /// </summary>
        /// <param name="dataSet">Datensatz mit Ergebnissen</param>
        private void SaveResults(IDataset dataSet)
        {
            Console.WriteLine("Speichere Ergebnisse...");
            if (currentRunPath == null)
            {
                Console.WriteLine("Fehler: runPath ist nicht gesetzt.");
                return;
            }
            ztxAccessWrite.SetDataLocation([currentRunPath]);
            ztxAccessWrite.AddImportDataToRun(dataSet);
        }

        /// <summary>
        /// Berechnet die GPS-Distanz [m] je Lap (1-basiert) über die per-Lap-Kanalabfrage
        /// <c>GetChannels(channels, NLaps, ...)</c> auf dem bereits geladenen Reader.
        /// </summary>
        private double[] ComputeLapDistances(string runPath)
        {
            int lapCount = Directory.GetDirectories(runPath, "Lap_*", SearchOption.TopDirectoryOnly).Length;
            var distances = new double[lapCount];

            for (uint n = 1; n <= lapCount; n++)
            {
                try
                {
                    // Der NLaps-Overload liefert ein Tuple (per-Lap-Array, kombiniert).
                    var (perLap, combined) = ztxAccessRead.GetChannels(
                        new[] { "Lat_dez", "Lon_dez" }, new uint[] { n }, null, false);
                    var ds = combined ?? (perLap is { Length: > 0 } ? perLap[0] : null);
                    var lat = ds?.GetElement("Lat_dez")?.SignalData?.Data as double[];
                    var lon = ds?.GetElement("Lon_dez")?.SignalData?.Data as double[];
                    distances[n - 1] = LapHaversine(lat, lon);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lap-Distanz für Lap {n} fehlgeschlagen: {ex.Message}");
                    distances[n - 1] = 0.0;
                }
            }
            return distances;
        }

        /// <summary>Kumulierte Haversine-Distanz über gültige GPS-Punkte (Nullpunkte werden übersprungen).</summary>
        private static double LapHaversine(double[]? lat, double[]? lon)
        {
            if (lat is null || lon is null) return 0.0;
            int n = Math.Min(lat.Length, lon.Length);
            double dist = 0.0;
            for (int i = 1; i < n; i++)
            {
                if (lat[i] == 0 || lon[i] == 0 || lat[i - 1] == 0 || lon[i - 1] == 0) continue;
                double la1 = lat[i - 1] * Math.PI / 180.0, la2 = lat[i] * Math.PI / 180.0;
                double dLat = la2 - la1, dLon = (lon[i] - lon[i - 1]) * Math.PI / 180.0;
                double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                         + Math.Cos(la1) * Math.Cos(la2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                dist += EarthRadiusMeters * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            }
            return dist;
        }

        /// <summary>
        /// Interpoliert Nullen in einem Array mit Math.NET Numerics.
        /// - Verwendet die Zeitreihe als X-Achse für die Interpolation.
        /// - Führende/trailing Nullen werden mit dem nächsten gültigen Wert ersetzt.
        /// - Innere Nullen werden linear interpoliert.
        /// </summary>
        private double[] InterpolateZerosMathNet(double[] data, double[] time, string name)
        {
            if (data == null || data.Length == 0 || time == null || time.Length != data.Length)
                return data ?? [];

            const double tol = 1e-9;
            int n = data.Length;
            var result = (double[])data.Clone();

            // Indizes der gültigen Werte (nicht Null)
            var validIdx = new List<int>();
            for (int i = 0; i < n; i++)
                if (Math.Abs(result[i]) > tol)
                    validIdx.Add(i);

            if (validIdx.Count == 0)
            {
                Console.WriteLine($"Warnung: {name} enthält nur Nullen; keine Interpolation möglich.");
                return result;
            }

            // Führende Nullen mit erstem gültigen Wert ersetzen
            int first = validIdx.First();
            for (int i = 0; i < first; i++)
                result[i] = result[first];

            // Abschließende Nullen mit letztem gültigen Wert ersetzen
            int last = validIdx.Last();
            for (int i = last + 1; i < n; i++)
                result[i] = result[last];

            // Interpolation mit Math.NET Numerics für innere Nullen
            // X: Zeitpunkte der gültigen Werte, Y: Werte
            double[] x = validIdx.Select(idx => time[idx]).ToArray();
            double[] y = validIdx.Select(idx => result[idx]).ToArray();

            // Lineare Interpolation erzeugen
            var interp = LinearSpline.InterpolateSorted(x, y);

            // Alle Nullstellen (außer führend/trailing) interpolieren
            for (int i = first + 1; i < last; i++)
            {
                if (Math.Abs(data[i]) <= tol)
                {
                    result[i] = interp.Interpolate(time[i]);
                }
            }

            int zerosInterpolated = result.Count(v => Math.Abs(v) > tol) - validIdx.Count;
            Console.WriteLine($"{name}: Interpolation mit Math.NET abgeschlossen. Ursprünglich gültige Werte: {validIdx.Count}, interpolierte Werte: {zerosInterpolated}, Länge: {n}.");

            return result;
        }
    }
}