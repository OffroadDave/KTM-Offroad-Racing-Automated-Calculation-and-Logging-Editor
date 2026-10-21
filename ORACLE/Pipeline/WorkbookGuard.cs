using Ktm.Racing.Domain.MotoDb;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Schützt Schreibzugriffe auf <c>loadChannels.xlsx</c>: ein systemweiter Mutex verhindert, dass parallele ORACLE-Jobs
    /// (KRAKEN) gleichzeitig lesen-ändern-schreiben (verlorene Updates), und ein gesperrtes Workbook (Excel) wird
    /// bis zu einer Wartezeit erneut versucht, statt sofort mit Exit-Code 5 aufzugeben.
    /// </summary>
    public static class WorkbookGuard
    {
        private const string MutexName = @"Global\KTM.ORACLE.loadChannels";

        /// <summary>Intervall zwischen zwei Versuchen bei gesperrtem Workbook.</summary>
        public static TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Führt <paramref name="write"/> exklusiv aus und wiederholt es, solange das Workbook gesperrt ist und
        /// <paramref name="maxWait"/> nicht überschritten ist.
        /// </summary>
        public static LoadChannelsSyncResult WriteWithRetry(Func<LoadChannelsSyncResult> write, TimeSpan maxWait)
        {
            ArgumentNullException.ThrowIfNull(write);
            var deadline = DateTime.UtcNow + maxWait;
            var announced = false;
            while (true)
            {
                var result = Exclusive(write, maxWait);
                if (result.Status != LoadChannelsSyncStatus.Locked || DateTime.UtcNow + RetryInterval > deadline)
                    return result;

                if (!announced)
                {
                    Console.WriteLine($"[loadChannels] Workbook gesperrt (z.B. in Excel offen) – warte bis zu {maxWait.TotalSeconds:0} s …");
                    announced = true;
                }
                Thread.Sleep(RetryInterval);
            }
        }

        /// <summary>Führt <paramref name="action"/> unter dem systemweiten loadChannels-Mutex aus.</summary>
        public static T Exclusive<T>(Func<T> action, TimeSpan timeout)
        {
            using var mutex = new Mutex(false, MutexName);
            var acquired = false;
            try
            {
                try
                {
                    acquired = mutex.WaitOne(timeout < TimeSpan.FromSeconds(30) ? TimeSpan.FromSeconds(30) : timeout);
                }
                catch (AbandonedMutexException)
                {
                    // Ein anderer ORACLE-Prozess ist abgestürzt, während er den Mutex hielt – wir übernehmen.
                    acquired = true;
                }

                if (!acquired)
                    Console.WriteLine("[loadChannels] ⚠️ Anderer ORACLE-Prozess schreibt seit langem – fahre ohne Mutex fort.");
                return action();
            }
            finally
            {
                if (acquired)
                    mutex.ReleaseMutex();
            }
        }
    }
}
