namespace Ktm.Racing.Core;

/// <summary>
/// Konfiguration für den Race Report: Pfade zur Input-Excel und zu den VCH-Bibliotheken.
/// Standardwerte entsprechen der aktuellen KTM-Ordnerstruktur; alles ist überschreibbar.
/// </summary>
public sealed class RaceReportOptions
{
    /// <summary>Input-Excel mit den zu berechnenden Kanälen (LOAD=1 &amp; CALC=1).</summary>
    public string InputExcelPath { get; set; } =
        @"C:\Dev\KTM Offroad Racing Shared Resources\Source\02_MotoDB\Racereport_Inputs_MX.xlsx";

    /// <summary>Optionaler Blattname in der Input-Excel (null → automatische Wahl/erstes Blatt).</summary>
    public string? InputSheetName { get; set; }

    /// <summary>Ordner, der bei SetMathLibrary als vchFolderPath übergeben wird.</summary>
    public string VchFolder { get; set; } =
        @"C:\KTM Tools\KTM Offroad Racing Shared Resources\02_MotoDB\VCH";

    /// <summary>Die zu ladenden VCH-Bibliotheken (Reihenfolge: Conditions, MotoDB, General).</summary>
    public List<string> VchFiles { get; set; } = new()
    {
        @"C:\KTM Tools\KTM Offroad Racing Shared Resources\02_MotoDB\VCH\KTM_ORR_Conditions.vch",
        @"C:\KTM Tools\KTM Offroad Racing Shared Resources\02_MotoDB\VCH\KTM_ORR_MotoDB.vch",
        @"C:\KTM Tools\KTM Offroad Racing Shared Resources\01_General\VCH\KTM_ORR_General.vch",
    };

    /// <summary>Zusätzliche VCH-Bibliotheken – werden nur geladen, wenn die Datei existiert.</summary>
    public List<string> ExtraVchFiles { get; set; } = new()
    {
        @"C:\KTM Tools\KTM Offroad Racing Shared Resources\02_MotoDB\VCH\KTM_Performance_ORR.vch",
        @"C:\KTM Tools\KTM Offroad Racing Shared Resources\02_MotoDB\VCH\KTM_Conditions_ORR.vch",
    };

    /// <summary>Zielordner für neue Excel-Dateien (null → Desktop des Benutzers).</summary>
    public string? OutputFolder { get; set; }

    /// <summary>Effektive VCH-Liste: Basis-Dateien plus vorhandene Extra-Bibliotheken.</summary>
    public List<string> EffectiveVchFiles()
    {
        var list = VchFiles.Where(File.Exists).ToList();
        list.AddRange(ExtraVchFiles.Where(File.Exists));
        return list;
    }
}
