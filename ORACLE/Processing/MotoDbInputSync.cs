using Ktm.Racing.Domain.Kompass;
using Ktm.Racing.Domain.MotoDb;

namespace Ktm.Racing.Oracle;

/// <summary>
/// Synchronisiert die MotoDB-<c>loadChannels.xlsx</c> (Sheets CIRCUITS + HED) aus der KOMPASS-Stammdaten-DB.
/// Headless-Einstieg für KRAKEN, damit die MotoDB-Inputs vor jedem Lauf aktuell sind.
///
/// <para>Bewusst <b>ohne</b> KtmDataHandler/native Abhängigkeit (nur <c>Ktm.Racing.Domain</c> + ClosedXML):
/// der Sync-Job benötigt keinen HASP-Dongle und keine WinTAX-Runtime und läuft daher auf jeder Maschine.
/// Reuse des getesteten <see cref="LoadChannelsWriter"/>; ein gesperrtes Workbook ist non-fatal.</para>
/// </summary>
public static class MotoDbInputSync
{
    /// <summary>
    /// Führt die Synchronisierung aus. Pfade sind optional und fallen auf die Konfiguration zurück.
    /// </summary>
    /// <param name="config">ORACLE-Konfiguration (liefert die Default-Pfade).</param>
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
            Console.WriteLine($"KOMPASS-Datei nicht gefunden: {kompassPath}");
            return OracleExitCode.DataNotFound;
        }
        if (!File.Exists(workbookPath))
        {
            Console.WriteLine($"loadChannels-Workbook nicht gefunden: {workbookPath}");
            return OracleExitCode.DataNotFound;
        }

        KompassDatabase database;
        try
        {
            database = new KompassRepository().Load(kompassPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"KOMPASS konnte nicht geladen werden: {ex.Message}");
            return OracleExitCode.GeneralError;
        }

        var result = new LoadChannelsWriter().Sync(database, new LoadChannelsOptions { WorkbookPath = workbookPath });
        Console.WriteLine($"[sync-motodb] {result.Status}: {result.Message}");

        return result.Status switch
        {
            LoadChannelsSyncStatus.Updated => OracleExitCode.Success,
            LoadChannelsSyncStatus.Skipped => OracleExitCode.Success,        // sauberer Skip (non-fatal)
            LoadChannelsSyncStatus.Locked => OracleExitCode.ResourceLocked,  // gesperrt -> Retry statt veraltete Inputs zu verwenden
            _ => OracleExitCode.GeneralError,
        };
    }
}
