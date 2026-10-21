namespace SENTINEL
{
    /// <summary>
    /// Tracks snapshots per run across multiple polling cycles and determines
    /// when a run is considered "stable" (= download completed).
    /// 
    /// <para><b>Stability algorithm:</b></para>
    /// <list type="number">
    ///   <item>A history of snapshots is kept per run</item>
    ///   <item>A new snapshot is compared with the previous one (file count + total size)</item>
    ///   <item>If both are equal AND structurally valid → StableCount++</item>
    ///   <item>Change detected → StableCount is reset to 0</item>
    ///   <item>StableCount ≥ threshold → run is stable</item>
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
        /// Records a new snapshot for a run and updates the stability counter.
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
        /// Checks whether a run has reached the stability threshold.
        /// </summary>
        public bool IsStable(string runPath)
        {
            return _states.TryGetValue(runPath, out var state)
                && state.ConsecutiveStableCount >= _requiredStableChecks
                && state.LastSnapshot is not null
                && state.LastSnapshot.IsStructurallyValid;
        }

        /// <summary>
        /// Returns the latest snapshot for a run (for download.done generation).
        /// </summary>
        public RunSnapshot? GetLatestSnapshot(string runPath)
        {
            return _states.TryGetValue(runPath, out var state) ? state.LastSnapshot : null;
        }

        /// <summary>
        /// Removes the tracking state for a run (after download.done generation).
        /// </summary>
        public void Remove(string runPath)
        {
            _states.Remove(runPath);
        }

        /// <summary>
        /// Removes tracking states for runs that are no longer pending (folder deleted, moved
        /// or download.done created elsewhere). Prevents unbounded growth.
        /// </summary>
        /// <returns>Number of removed states</returns>
        public int RetainOnly(IEnumerable<string> activeRunPaths)
        {
            var active = new HashSet<string>(activeRunPaths, StringComparer.OrdinalIgnoreCase);
            var stale = _states.Keys.Where(path => !active.Contains(path)).ToList();
            foreach (var path in stale)
                _states.Remove(path);
            return stale.Count;
        }

        /// <summary>Number of currently tracked runs.</summary>
        public int TrackedRunCount => _states.Count;

        private sealed class RunTrackingState
        {
            public RunSnapshot? LastSnapshot { get; set; }
            public int ConsecutiveStableCount { get; set; }
        }
    }
}
