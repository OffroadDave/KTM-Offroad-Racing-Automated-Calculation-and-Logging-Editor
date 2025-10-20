using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KtmDataHandler.DataStorage;
using KtmDataHandler.Generic.Data;
using KtmDataHandler.Marelli.Sysma;
using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;
using KtmDataHandler.Models.Symbols;
using KtmSharedUtils.ObjectUtils;
using System.Configuration;
using System.IO;
// Math.NET Numerics für numerische Berechnungen
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;

namespace ORACLE
{
    public class ImportDataCalculator
    {
        // Define Objects 
        private IMarelliDataAccessWrite ztxAccessWrite = new MarelliDataAccess();
        private IMarelliDataAccessRead ztxAccessRead = new MarelliDataAccess();
        private IMarelliDataBrowse ztxBrowse = new MarelliDataBrowse();

        private List<string> channelNames = new();

        // Expose channel names read-only for other classes

        public IReadOnlyList<string> ChannelNames => channelNames.AsReadOnly();

        private const double EarthRadiusMeters = 6371000.0;
        private List<string> NecessaryChannels = new();
        private string? currentRunPath; // Aktueller Run-Pfad
        private IDataset resultDataset = new Dataset(); // Globale Variable für berechnete Ergebnisse

        /// <summary>
        /// Hauptfunktion: Führt alle Berechnungsschritte aus.
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner (Basis für das Laden der Daten)</param>
        public void RunCalculationPipeline(string runPath)
        {
            currentRunPath = runPath;

            // 1. Datensatz laden
            var dataSet = LoadDataSet(runPath);
            if (dataSet == null)
            {
                Console.WriteLine("Datensatz konnte nicht geladen werden.");
                return;
            }

            // 2. Berechnungsfunktionen der Reihe nach ausführen
            SVehicleCalc(dataSet); // Berechnung von sVehicle aus GPS Koordinaten
            //CalculateStep2(dataSet);
            //CalculateStep3(dataSet);
            // ... weitere Berechnungsschritte hier ergänzen

            // 3. Ergebnisse speichern
            SaveResults(resultDataset);
        }

        /// <summary>
        /// Lädt den Datensatz aus dem angegebenen Run-Ordner.
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner</param>
        /// <returns>Geladener Datensatz oder null bei Fehler</returns>
        private object? LoadDataSet(string runPath)
        {
            try
            {
                // Setze den Datenpfad und lade den Run
                ztxAccessRead.SetDataLocation(new string[] { runPath });
                ztxAccessRead.LoadRun();
                Console.WriteLine($"Datensatz erfolgreich geladen aus: {runPath}");

                // Channel-Namen abfragen und speichern
                var names = ztxAccessRead.GetChannelsNames();
                if (names != null)
                    channelNames = names.ToList();

                Console.WriteLine("Channelliste aus Datensatz extrahiert.");

                // TODO: Rückgabe des eigentlichen Datensatz-Objekts, falls verfügbar
                return ztxAccessRead; // oder ein spezifisches Objekt, falls vorhanden
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden des Datensatzes: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Berechnung der kumulierten Distanz aus GPS-Koordinaten und Hinzufügen als neuen Channel.
        /// </summary>
        /// <param name="dataSet">Das geladene Datensatzobjekt</param>
        private void SVehicleCalc(object dataSet)
        {
            Console.WriteLine("Kumulierte Distanz aus GPS wird berechnet...");

            // Setze die für diese Berechnung notwendigen Kanäle
            string LatChannelName = "Lat_dez";
            string LonChannelName = "Lon_dez";
            NecessaryChannels = new List<string> { LatChannelName, LonChannelName };  

            // Prüfe, ob alle notwendigen Kanäle vorhanden sind
            var missingChannels = NecessaryChannels.Except(channelNames).ToList();
            if (missingChannels.Any())
            {
                Console.WriteLine($"Fehlende Kanäle: {string.Join(", ", missingChannels)}. Berechnung wird abgebrochen.");
                return;
            }
            // Daten vom Datensatz abrufen 
            IDataset processingDataset = ztxAccessRead.GetChannels(NecessaryChannels.ToArray());

            // Extract latitude and longitude signals
            ITimeseries latitudeSignal = processingDataset.GetElement(LatChannelName);
            ITimeseries longitudeSignal = processingDataset.GetElement(LonChannelName);

            if (latitudeSignal == null || longitudeSignal == null)
            {
                Console.WriteLine("Latitude oder Longitude Channel haben ungültige Werte.");
                return;
            }

            // Extract data arrays
            var latitudes = latitudeSignal.SignalData.Data as double[];
            var longitudes = longitudeSignal.SignalData.Data as double[];
            var time = latitudeSignal.SignalData.Time as double[];

            if (latitudes == null || longitudes == null || time == null)
            {
                Console.WriteLine("Channel-Daten sind nicht im erwarteten Format.");
                return;
            }

            // Interpolieren von Nullen mit Math.NET Numerics
            // Die Zeitreihe wird als X-Achse für die Interpolation verwendet
            latitudes = InterpolateZerosMathNet(latitudes, time, "Latitude");
            longitudes = InterpolateZerosMathNet(longitudes, time, "Longitude");

            if (latitudes.Length != longitudes.Length || latitudes.Length != time.Length || latitudes.Length < 2)
            {
                Console.WriteLine("Latitude, Longitude und Zeit-Arrays müssen gleiche Länge und mindestens 2 Punkte haben.");
                return;
            }

            Console.WriteLine($"Notwendige Kanäle: {string.Join(", ", NecessaryChannels)}  Wurden erfolgreich geladen.");

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
                distance[i] = Math.Round(distance[i - 1] + segmentDistances[i - 1], 2);

            // Create new timeseries for sVehicle
            var sVehicle = new KtmDataHandler.Models.Channels.Timeseries();
            sVehicle.SignalData.Name = "sVehicle";
            sVehicle.SignalData.Unit = "m";
            sVehicle.SignalData.TimeData = (time, distance);

            // Add to calculation dataset
            resultDataset.Add(sVehicle);

            Console.WriteLine("Berechnung von sVehicle erfolgreich abgeschlossen.");
        }

        // Beispiel-Berechnungsfunktion 2
        private void CalculateStep2(object dataSet)
        {
            Console.WriteLine("Berechnungsschritt 2 wird ausgeführt...");
            // Beispiel: Setze die für diese Berechnung notwendigen Kanäle
            NecessaryChannels = new List<string> { "Speed", "Time" };
            // TODO: Implementiere Berechnung
        }

        // Beispiel-Berechnungsfunktion 3
        private void CalculateStep3(object dataSet)
        {
            Console.WriteLine("Berechnungsschritt 3 wird ausgeführt...");
            // Beispiel: Setze die für diese Berechnung notwendigen Kanäle
            NecessaryChannels = new List<string> { "RPM" };
            // TODO: Implementiere Berechnung
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
            ztxAccessWrite.SetDataLocation(new string[] { currentRunPath });
            ztxAccessWrite.AddImportDataToRun(dataSet);
            ztxAccessWrite.ReleaseObjects();
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
                return data;

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