using Ktm.Racing.Domain.MotoDb;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Protects write access to <c>loadChannels.xlsx</c>: a system-wide mutex prevents parallel ORACLE jobs
    /// (KRAKEN) from reading-modifying-writing at the same time (lost updates), and a locked workbook (Excel) is
    /// retried up to a wait time instead of giving up immediately with exit code 5.
    /// </summary>
    public static class WorkbookGuard
    {
        private const string MutexName = @"Global\KTM.ORACLE.loadChannels";

        /// <summary>Interval between two attempts when the workbook is locked.</summary>
        public static TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Executes <paramref name="write"/> exclusively and retries it as long as the workbook is locked and
        /// <paramref name="maxWait"/> has not been exceeded.
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
                    Console.WriteLine($"[loadChannels] Workbook locked (e.g. open in Excel) – waiting up to {maxWait.TotalSeconds:0} s …");
                    announced = true;
                }
                Thread.Sleep(RetryInterval);
            }
        }

        /// <summary>Executes <paramref name="action"/> under the system-wide loadChannels mutex.</summary>
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
                    // Another ORACLE process crashed while holding the mutex – we take over.
                    acquired = true;
                }

                if (!acquired)
                    Console.WriteLine("[loadChannels] ⚠️ Another ORACLE process has been writing for a long time – continuing without mutex.");
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
