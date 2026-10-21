using Ktm.Racing.Domain.Kompass;
using Ktm.Racing.Domain.MotoDb;

namespace Ktm.Racing.Oracle;

/// <summary>
/// Synchronizes the MotoDB <c>loadChannels.xlsx</c> (sheets CIRCUITS + HED) from the KOMPASS master data DB.
/// Headless entry point for KRAKEN so that the MotoDB inputs are up to date before every run.
///
/// <para>Deliberately <b>without</b> KtmDataHandler/native dependency (only <c>Ktm.Racing.Domain</c> + ClosedXML):
/// the sync job needs no HASP dongle and no WinTAX runtime and therefore runs on any machine.
/// Reuses the tested <see cref="LoadChannelsWriter"/>; a locked workbook is non-fatal.</para>
/// </summary>
public static class MotoDbInputSync
{
    /// <summary>
    /// Performs the synchronization. Paths are optional and fall back to the configuration.
    /// </summary>
    /// <param name="config">ORACLE configuration (provides the default paths).</param>
    /// <param name="kompassPath">KOMPASS.json; <c>null</c> → <see cref="OracleConfig.DatabaseFilePath"/>.</param>
    /// <param name="workbookPath">loadChannels.xlsx; <c>null</c> → <see cref="OracleConfig.MotoDbLoadChannelsPath"/>.</param>
    public static OracleExitCode Run(OracleConfig config, string? kompassPath = null, string? workbookPath = null)
    {
        kompassPath ??= config.DatabaseFilePath;
        workbookPath ??= config.MotoDbLoadChannelsPath;

        Console.WriteLine($"[sync-motodb] KOMPASS:  {kompassPath}");
        Console.WriteLine($"[sync-motodb] Workbook: {workbookPath}");

        if (!File.Exists(kompassPath))
        {
            Console.WriteLine($"KOMPASS file not found: {kompassPath}");
            return OracleExitCode.DataNotFound;
        }
        if (!File.Exists(workbookPath))
        {
            Console.WriteLine($"loadChannels workbook not found: {workbookPath}");
            return OracleExitCode.DataNotFound;
        }

        KompassDatabase database;
        try
        {
            database = new KompassRepository().Load(kompassPath);
        }
        catch (Exception ex) when (ex is not IOException)
        {
            Console.WriteLine($"KOMPASS is invalid and cannot be read: {ex.Message}");
            return OracleExitCode.InvalidData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"KOMPASS could not be loaded: {ex.Message}");
            return OracleExitCode.GeneralError;
        }

        Console.WriteLine($"[sync-motodb] KOMPASS state:  {InputFingerprint.Describe(kompassPath)}");
        Console.WriteLine($"[sync-motodb] Workbook state: {InputFingerprint.Describe(workbookPath)}");

        var result = WorkbookGuard.WriteWithRetry(
            () => new LoadChannelsWriter().Sync(database, new LoadChannelsOptions { WorkbookPath = workbookPath }),
            TimeSpan.FromSeconds(Math.Max(0, config.TrackLength.LockWaitSeconds)));
        Console.WriteLine($"[sync-motodb] {result.Status}: {result.Message}");

        return result.Status switch
        {
            LoadChannelsSyncStatus.Updated => OracleExitCode.Success,
            LoadChannelsSyncStatus.Skipped => OracleExitCode.Success,                    // clean skip (non-fatal)
                        LoadChannelsSyncStatus.Locked => OracleExitCode.ResourceLocked,  // locked -> retry instead of using stale inputs
            _ => OracleExitCode.GeneralError,
        };
    }
}
