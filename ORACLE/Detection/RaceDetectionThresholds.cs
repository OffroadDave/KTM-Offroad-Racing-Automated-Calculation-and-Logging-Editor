namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Schwellenwerte für die Renn-Erkennung (<see cref="RaceSegmentDetector"/>).
    /// Wird aus <c>oracle-config.json</c> (Abschnitt <c>RaceDetection</c>) geladen; sonst Defaults.
    ///
    /// <para>Die Erkennung ist bewusst <b>antriebsstrang-unabhängig</b>: Sie nutzt ausschließlich
    /// GPS (Distanz aus Lat/Lon) und Gasstellung; Drehzahl und GPS-Qualität dienen nur als Guards.
    /// <c>vRearnEng</c> (aus Drehzahl × Getriebeübersetzung berechnet) wird bewusst NICHT verwendet.</para>
    /// </summary>
    public sealed class RaceDetectionThresholds
    {
        /// <summary>Motor gilt als "an" ab dieser mittleren Drehzahl [rpm]. Darunter: kein Rennen.</summary>
        public double EngineOnMinRpm { get; set; } = 2000.0;

        /// <summary>Absolute Mindest-GPS-Distanz [m], damit ein Run als Rennen zählt.</summary>
        public double DistanceMinMeters { get; set; } = 1000.0;

        /// <summary>
        /// Faktor auf die Track-Länge (aus KOMPASS). Die effektive Mindestdistanz ist
        /// <c>max(DistanceMinMeters, TrackLengthFactor × TrackLen)</c>, sofern TrackLen bekannt ist.
        /// </summary>
        public double TrackLengthFactor { get; set; } = 0.5;

        /// <summary>Mindest-Durchschnittsgeschwindigkeit [kph] (GPS-Distanz / Laufzeit).</summary>
        public double SpeedMinKph { get; set; } = 25.0;

        /// <summary>Mindest-Durchschnittsgasstellung [%].</summary>
        public double ThrottleMinPercent { get; set; } = 15.0;

        /// <summary>Fallback bei GPS-Ausfall: Mindestgas [%] (höhere Schwelle, geringere Konfidenz).</summary>
        public double ThrottleFallbackPercent { get; set; } = 20.0;

        /// <summary>Fallback bei GPS-Ausfall: Mindest-Durchschnittsdrehzahl [rpm].</summary>
        public double RpmRaceMin { get; set; } = 5000.0;

        /// <summary>Mindestanzahl gültiger GPS-Punkte, damit GPS als nutzbar gilt.</summary>
        public int MinValidGpsSamples { get; set; } = 100;
    }
}
