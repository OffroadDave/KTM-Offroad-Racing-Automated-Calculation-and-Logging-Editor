using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;

namespace Ktm.Racing.Core
{
    /// <summary>
    /// Exportiert die Ergebnisse eines Session-Merge als neuen WinTAX4-Run.
    /// 
    /// <para><b>Strategie:</b></para>
    /// <list type="number">
    ///   <item>Erstellt eine synthetische Run-Ordnerstruktur: <c>{riderPath}/_MergedSession/Run_Merged/Lap_001/</c></item>
    ///   <item>Kopiert eine vorhandene <c>cableData.ztx</c> als Template (WinTAX4 benötigt diese Datei)</item>
    ///   <item>Schreibt das gemergte Dataset via <see cref="IMarelliDataAccessWrite.AddImportDataToRun"/></item>
    /// </list>
    /// 
    /// <para><b>Warum dieses Vorgehen?</b></para>
    /// Die <see cref="IMarelliDataAccessWrite"/>-API hat keine <c>CreateNewRun</c>-Methode.
    /// <c>SetDataLocation</c> erwartet einen existierenden WinTAX4-Run mit mindestens einem
    /// <c>Lap_xxx/cableData.ztx</c>. Daher wird eine minimale Ordnerstruktur mit einer kopierten
    /// Template-Datei erstellt, und die gemergten Channels werden als ImportData hinzugefügt.
    /// 
    /// <para><b>Ausgabe-Ordner:</b></para>
    /// <code>
    /// {riderPath}/
    /// ├── Run_201/          ← Originale Runs
    /// ├── Run_202/
    /// └── _MergedSession/   ← Export-Ordner
    ///     └── Run_Merged/
    ///         └── Lap_001/
    ///             └── cableData.ztx  ← Kopie als Template
    ///             (+ ImportData-Ordner mit gemergten Channels)
    /// </code>
    /// </summary>
    public class SessionMergeExporter
    {
        /// <summary>Name des Export-Ordners im Rider-Verzeichnis</summary>
        public const string MergedSessionFolderName = "_MergedSession";

        /// <summary>Name des synthetischen Run-Ordners</summary>
        public const string MergedRunFolderName = "Run_Merged";

        /// <summary>
        /// Exportiert das Merge-Ergebnis als neuen WinTAX4-Run.
        /// </summary>
        /// <param name="mergedSession">Merge-Ergebnis mit konsolidiertem Dataset</param>
        /// <param name="riderPath">Pfad zum Rider-Ordner (Ziel für den Export)</param>
        /// <returns>Pfad zum exportierten Run-Ordner, oder null bei Fehler</returns>
        public string? Export(MergedSessionData mergedSession, string riderPath)
        {
            if (!mergedSession.HasData)
            {
                Console.WriteLine("[export] Keine Daten zum Exportieren vorhanden.");
                return null;
            }

            if (mergedSession.MergedDataset.NumElements() == 0)
            {
                Console.WriteLine("[export] Merged-Dataset enthält keine Channels.");
                return null;
            }

            // --- 1. Ordnerstruktur erstellen ---
            var mergedRunPath = CreateFolderStructure(riderPath);
            if (mergedRunPath is null)
                return null;

            // --- 2. cableData.ztx Template kopieren ---
            if (!CopyTemplateFile(mergedSession, mergedRunPath))
            {
                Console.WriteLine("[export] Keine cableData.ztx als Template gefunden. Export abgebrochen.");
                return null;
            }

            // --- 3. Merged Dataset via IMarelliDataAccessWrite schreiben ---
            if (!WriteMergedData(mergedSession.MergedDataset, mergedRunPath))
                return null;

            Console.WriteLine($"[export] Merge-Ergebnis erfolgreich exportiert: {mergedRunPath}");
            Console.WriteLine($"[export] {mergedSession.MergedDataset.NumElements()} Channel(s) geschrieben.");
            return mergedRunPath;
        }

        /// <summary>
        /// Gibt den erwarteten Ausgabepfad zurück (ohne ihn zu erstellen).
        /// </summary>
        public static string GetExpectedOutputPath(string riderPath)
        {
            return Path.Combine(riderPath, MergedSessionFolderName, MergedRunFolderName);
        }

        /// <summary>
        /// Erstellt die Ordnerstruktur _MergedSession/Run_Merged/Lap_001/.
        /// Löscht vorherige Merge-Ergebnisse falls vorhanden.
        /// </summary>
        private static string? CreateFolderStructure(string riderPath)
        {
            try
            {
                var mergedSessionPath = Path.Combine(riderPath, MergedSessionFolderName);
                var mergedRunPath = Path.Combine(mergedSessionPath, MergedRunFolderName);
                var mergedLapPath = Path.Combine(mergedRunPath, "Lap_001");

                // Vorherige Merge-Ergebnisse entfernen
                if (Directory.Exists(mergedRunPath))
                {
                    Console.WriteLine("[export] Entferne vorheriges Merge-Ergebnis...");
                    Directory.Delete(mergedRunPath, recursive: true);
                }

                Directory.CreateDirectory(mergedLapPath);
                Console.WriteLine($"[export] Ordnerstruktur erstellt: {mergedRunPath}");
                return mergedRunPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[export] Fehler beim Erstellen der Ordnerstruktur: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Kopiert die erste verfügbare cableData.ztx aus einem erfolgreichen Run als Template.
        /// WinTAX4 benötigt diese Datei als Grundlage — die Import-Daten werden separat gespeichert.
        /// </summary>
        private static bool CopyTemplateFile(MergedSessionData mergedSession, string mergedRunPath)
        {
            // Finde die erste cableData.ztx aus einem erfolgreich gemergten Run
            foreach (var runInfo in mergedSession.Runs)
            {
                if (runInfo.Status != RunMergeStatus.Success)
                    continue;

                var sourceLapDirs = Directory.GetDirectories(runInfo.RunPath, "Lap_*", SearchOption.TopDirectoryOnly);

                foreach (var lapDir in sourceLapDirs)
                {
                    var sourceZtx = Path.Combine(lapDir, "cableData.ztx");
                    if (File.Exists(sourceZtx) && new FileInfo(sourceZtx).Length > 0)
                    {
                        var targetZtx = Path.Combine(mergedRunPath, "Lap_001", "cableData.ztx");
                        File.Copy(sourceZtx, targetZtx, overwrite: true);
                        Console.WriteLine($"[export] Template cableData.ztx kopiert aus: {Path.GetFileName(lapDir)}");
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Schreibt das gemergte Dataset über die KtmDataHandler Write-API.
        /// 
        /// <para><b>API-Flow:</b></para>
        /// <list type="number">
        ///   <item><c>SetDataLocation</c> → Zeigt auf den synthetischen Run-Ordner</item>
        ///   <item><c>AddImportDataToRun</c> → Schreibt alle gemergten Channels als ImportData</item>
        ///   <item><c>ReleaseObjects</c> → Writer-Ressourcen freigeben</item>
        /// </list>
        /// </summary>
        private static bool WriteMergedData(IDataset mergedDataset, string mergedRunPath)
        {
            IMarelliDataAccessWrite writer = KtmDataHandlerFactory.CreateWriter();
            try
            {
                writer.SetDataLocation([mergedRunPath]);

                Console.WriteLine($"[export] Schreibe {mergedDataset.NumElements()} Channel(s) als ImportData...");
                // 1-Arg-Overload wie in ImportDataCalculator.SaveResults (bewährt); die 3-Arg-Variante
                // mit eigenem Temp-Ordner schlug beim synthetischen Merge-Run fehl.
                writer.AddImportDataToRun(mergedDataset);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[export] Fehler beim Schreiben der Merge-Daten: {ex}");
                return false;
            }
            finally
            {
                writer.ReleaseObjects();
            }
        }
    }
}
