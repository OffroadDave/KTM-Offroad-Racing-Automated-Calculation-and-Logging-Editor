namespace SENTINEL
{
    /// <summary>
    /// Verfolgt Snapshots pro Run über mehrere Polling-Zyklen und bestimmt,
    /// wann ein Run als "stabil" gilt (= Download abgeschlossen).
    /// 
    /// <para><b>Stabilitäts-Algorithmus:</b></para>
    /// <list type="number">
    ///   <item>Pro Run wird eine Historie von Snapshots geführt</item>
    ///   <item>Neuer Snapshot wird mit dem vorherigen verglichen (Dateianzahl + Gesamtgröße)</item>
    ///   <item>Sind beide gleich UND strukturell gültig → StableCount++</item>
    ///   <item>Änderung erkannt → StableCount wird auf 0 zurückgesetzt</item>
    ///   <item>StableCount ≥ Threshold → Run ist stabil</item>
    /// </list>
    /// </summary>
    public sealed class StabilityTracker
    {
        private readonly int _requiredStableChecks;
        private readonly Dictionary<string, RunTrackingState> _states = new(StringComparer.OrdinalIgnoreCase);

        public StabilityTracker(int requiredStableChecks)
        {
            _requiredStableChecks = requiredStableChecks > 0 ? requiredStableChecks : 3;
        }

        /// <summary>
        /// Zeichnet einen neuen Snapshot für einen Run auf und aktualisiert den Stabilitätszähler.
        /// </summary>
        public void RecordSnapshot(RunSnapshot snapshot)
        {
            if (!_states.TryGetValue(snapshot.RunPath, out var state))
            {
                state = new RunTrackingState();
                _states[snapshot.RunPath] = state;
            }

            if (snapshot.IsStableComparedTo(state.LastSnapshot))
            {
                state.ConsecutiveStableCount++;
            }
            else
            {
                state.ConsecutiveStableCount = 0;
            }

            state.LastSnapshot = snapshot;
        }

        /// <summary>
        /// Prüft ob ein Run die Stabilitätsschwelle erreicht hat.
        /// </summary>
        public bool IsStable(string runPath)
        {
            return _states.TryGetValue(runPath, out var state)
                && state.ConsecutiveStableCount >= _requiredStableChecks
                && state.LastSnapshot is not null
                && state.LastSnapshot.IsStructurallyValid;
        }

        /// <summary>
        /// Gibt den letzten Snapshot für einen Run zurück (für die download.done-Generierung).
        /// </summary>
        public RunSnapshot? GetLatestSnapshot(string runPath)
        {
            return _states.TryGetValue(runPath, out var state) ? state.LastSnapshot : null;
        }

        /// <summary>
        /// Entfernt den Tracking-State für einen Run (nach download.done-Generierung).
        /// </summary>
        public void Remove(string runPath)
        {
            _states.Remove(runPath);
        }

        /// <summary>
        /// Entfernt Tracking-States für Runs, die nicht mehr ausstehen (Ordner gelöscht, verschoben
        /// oder download.done von anderer Stelle erzeugt). Verhindert unbegrenztes Wachstum.
        /// </summary>
        /// <returns>Anzahl entfernter States</returns>
        public int RetainOnly(IEnumerable<string> activeRunPaths)
        {
            var active = new HashSet<string>(activeRunPaths, StringComparer.OrdinalIgnoreCase);
            var stale = _states.Keys.Where(path => !active.Contains(path)).ToList();
            foreach (var path in stale)
                _states.Remove(path);
            return stale.Count;
        }

        /// <summary>Anzahl aktuell getrackter Runs.</summary>
        public int TrackedRunCount => _states.Count;

        private sealed class RunTrackingState
        {
            public RunSnapshot? LastSnapshot { get; set; }
            public int ConsecutiveStableCount { get; set; }
        }
    }
}
