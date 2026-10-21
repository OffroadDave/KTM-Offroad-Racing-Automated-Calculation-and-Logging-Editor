# Externe Tools & Abhängigkeiten von ORACLE – Notizen, Doku-Abgleich, Verbesserungen

Stand der Analyse: **21.10.2026**. Fundstellen (`Datei:Zeile`) beziehen sich auf den Code-Stand zum Analysezeitpunkt;
mehrere Dateien (u. a. `ImportDataCalculator.cs`, `GpsLapSplitService.cs`) wurden parallel bearbeitet – Zeilennummern können
sich daher verschieben. Alle Pfade relativ zum Repo-Root, außer `C:\…`-Pfade und `DOMAIN\…` (= `C:\Dev\Ktm.Racing.Domain\`).

Methodik: Code per grep; KTM-DLLs per `System.Reflection.Metadata`/`AssemblyLoadContext` inspiziert (Signaturen, Assembly-Referenzen,
Target-Framework, PE-Machine); CHMs mit `hh.exe -decompile` nach `%TEMP%\ext-notes\<name>` extrahiert; NuGet-Versionen über das
NuGet-MCP-Tool bzw. die NuGet-V3-API; Herstellerdoku über die offiziellen Webseiten. Es wurde **nicht** gebaut und kein Test
ausgeführt (parallel arbeitende Agenten).

> **Umsetzungsstand (nach den Review-PRs, Stand `docs/oracle-docs`):** Die Fundstellen unten beziehen sich auf den Baseline-Commit
> `1267f72`. Seitdem gilt:
>
> | Empfehlung | Status |
> |---|---|
> | §2.1.5 Nr. 1 – `LoadRun` ≤ 0 als Fehler | **umgesetzt** an allen Aufrufstellen (`ImportDataCalculator`, `validate`, `RaceSegmentDetector`, `StartAnalyzer`, `GpsLapSplitService`) |
> | §2.1.5 Nr. 2 und alle Verweise auf `SessionMerge*`/`SessionMergeExporter` | **hinfällig** – der Merge-Code wurde entfernt |
> | §2.1.5 Nr. 4 – Write-Tupel auswerten | **umgesetzt** in `Runtime\KtmWriteErrors.cs` (Annahme Item1/Item3 = Fehler, Item4 = Text); Bestätigung durch das KTM-Utils-Team steht aus |
> | §2.1.5 Nr. 6 – Cast `as MarelliDataAccess` | **umgesetzt** (`KtmDataHandlerFactory.CreateAccess`, kein Cast mehr) |
> | §2.1.5 Nr. 3, 5, 7–12 (Lösch-Semantik explizit, NoRx, `FindRuns`-Typfilter, leere Namen, Lizenzen, Instanzerzeugung, `PlatformTarget`, TelDataZTX-Kopie) | **offen** |
> | §2.5 Nr. 1 – direkte ClosedXML-Referenz | **entfernt** (kommt über `Ktm.Racing.Domain`) |
> | §2.6 Nr. 1, §2.7 – ConfigurationManager/MathNet kommentieren | **umgesetzt** (Kommentar im `ORACLE.csproj`) |
> | §2.8 – Microsoft.Vbe.Interop | **entfernt** |
> | §2.9 Nr. 1/2 – Testpakete | **umgesetzt**: xunit 2.9.3, xunit.runner.visualstudio 3.1.4, Microsoft.NET.Test.Sdk 17.14.1, coverlet.collector 6.0.4 (ORACLE.Tests, SENTINEL.Tests); Domain-Tests offen |
> | §2.10 – .NET 10 | **offen** (Support-Ende .NET 8: 10.11.2026) |
> | §2.11 – `download.done` | atomar geschrieben, Zeitstempel kulturunabhängig (SENTINEL); `download.done.xsd` weiterhin nicht im Repo |

---

## 1. Übersicht der gefundenen Abhängigkeiten

| Tool / Abhängigkeit | Version im Einsatz | Aktuelle Version (21.10.2026) | Verwendung in ORACLE (Fundstellen) | Quelle der Doku |
|---|---|---|---|---|
| **KtmDataHandler.dll** (managed, KTM) | 3.5.7.0 (Build 27.11.2024), TFM `.NETFramework 4.7.2`, PE `Amd64` | nicht ermittelbar (kein Feed/Changelog) | `ORACLE.csproj:27-30`; Factory `Runtime\KtmDataHandlerFactory.cs:40-109`; Reader/Writer in `Processing\ImportDataCalculator.cs`, `Processing\GpsLapSplitService.cs`, `Merge\SessionMergeService.cs`, `Merge\SessionMergeExporter.cs`, `Detection\RaceSegmentDetector.cs`, `Detection\StartAnalyzer.cs`, `Pipeline\OraclePipeline.cs`, `Metadata\KompassService.cs` (Details §2.1) | `KtmDataHandlerDocumentation.chm`, `docs\KtmDataHandlerDocumentation\*.md`, `C:\KTM Tools\KTM Utils Software Suite\KtmDataHandler.xml` |
| **KtmSharedUtils.dll** (managed, KTM) | 1.5.10.0, TFM `.NETFramework 4.7.2`, PE `Amd64` | nicht ermittelbar | `ORACLE.csproj:31-34`; `Runtime\KtmDataHandlerFactory.cs:4-5,43,57-67` (`IPropertyList`, `XMLFileProcessor<>`, `BinaryFileProcessor<>`, `JsonFileProcessor<>`); `Processing\ImportDataCalculator.cs:7` (`using KtmSharedUtils.ObjectUtils`) | `KtmSharedUtilsDocumentation.chm`, `docs\KtmSharedUtilsDocumentation`, `KtmSharedUtils.xml` |
| **TelDataZTXx64.dll** (nativ, Marelli) | 1.55.00 (18.07.2024) – Suite und Kopie `ORACLE\TelDataZTXx64.dll` sind byte-identisch (SHA256) | nicht ermittelbar | indirekt über KtmDataHandler; Kopie ins Output `ORACLE.csproj:41-43`; Suchpfad `Runtime\KtmRuntime.cs:67-70` | keine eigene API-Doku vorhanden; Format: `docs\Wintax4\DataBrw\DataArchives_Full.md` |
| **HASP / Sentinel LDK** (Dongle-Lizenz) | `haspvlib_109133.dll` 7.61.1, `hasp_net_windows.dll` 8.4, `hasp_rt.exe` (Suite) | nicht ermittelbar | indirekt (nur Kommentare: `Program.cs:48`, `Processing\MotoDbInputSync.cs:11`, `Runtime\KtmRuntime.cs:10,66`) | `docs\Wintax4\TroubleShooting\TroubleShootingDongle\USB_Protection_troubleshooting.md` |
| **WinTAX 4 Datenformat** (ZTX-Archiv) | – | `Wintax4.chm` vom 18.03.2026 | Run/Lap-Ordner, `cableData.ztx`, `importData.ztx`, `Car.xml`, `Session.xml`, `LapHeader.xml`, `RunLapHeader.xml`: `Processing\XmlRunEditor.cs:79,91,109,126,150`, `Processing\LapSplitStore.cs:84-85`, `Merge\SessionMergeExporter.cs:137-141` | `Wintax4.chm`, `docs\Wintax4` |
| WinTAX4 Automation (COM) | – | `WinTAX4AutomationInterfaces.chm` vom 02.04.2026 | **keine direkte Nutzung** (nur indirekt, falls KtmDataHandler mit `bLinktoApplication=true` an TelDataX/WinTAX koppelt) | `docs\WinTAX4AutomationInterfaces` |
| KtmSysmaAutomation / KtmVMSAutomation | 1.1.0.0 / 1.0.8.0 (Suite) | – | **keine Nutzung**; `Processing\ImportDataCalculator.cs:3` importiert nur den Namespace `KtmDataHandler.Marelli.Sysma` (Teil von KtmDataHandler) | `docs\KtmSysmaAutomationDocumentation`, `docs\KtmVMSAutomationDocumentation` |
| **Autofac** (NuGet) | 6.3.0 | 9.3.4 | `Runtime\KtmDataHandlerFactory.cs:1,40-83`; KtmDataHandler.dll referenziert selbst `Autofac 6.3.0.0` | docs.autofac.org |
| **ClosedXML** (NuGet) | 0.105.1 (ORACLE, ORACLE.Tests, Domain, Domain.Tests) | 0.105.1 (aktuell) | ORACLE-Code nutzt ClosedXML **nicht direkt**; Nutzung über `DOMAIN\src\Ktm.Racing.Domain\MotoDb\LoadChannelsWriter.cs:1,49,84,114,134,463`, aufgerufen von `Processing\MotoDbInputSync.cs:60-61`; Tests: `ORACLE.Tests\MotoDbInputSyncTests.cs:1`, `StabilityTests.cs:1`, `TrackLengthGateTests.cs:1` | docs.closedxml.io, GitHub-Releases |
| **MathNet.Numerics** (NuGet) | 5.0.0 | 5.0.0 (letzter Release 03.04.2022) | **keine Nutzung im ORACLE-Code**; aber `KtmSharedUtils.dll` referenziert `MathNet.Numerics 5.0.0.0` | nuget.org |
| **System.Configuration.ConfigurationManager** (NuGet) | 9.0.9 | 10.0.13 | **keine direkte Nutzung**; indirekt nötig für `log4net.dll` (Suite) → siehe §2.6 | nuget.org, learn.microsoft.com |
| log4net (Suite, nicht NuGet) | 2.0.12.0 (Build `.NET 4.5`) | 3.5.0 (NuGet) | indirekt über `LocalLogHelper`; ORACLE schaltet Logging ab: `Runtime\KtmDataHandlerFactory.cs:73-81` | – |
| **Microsoft.Vbe.Interop** (Office-PIA) | Office16 (HintPath `ORACLE.csproj:35-37`) | – | **keine Nutzung** (kein `Vbe` im Code); kopiert `Microsoft.Vbe.Interop.dll`, `office.dll`, `stdole.dll` nach `bin\Release\net8.0` | – |
| xunit | 2.5.3 | 2.9.3 (v2-Linie) / `xunit.v3` 4.0.2 | `ORACLE.Tests\ORACLE.Tests.csproj` | xunit.net |
| xunit.runner.visualstudio | 2.5.3 | 4.0.1 | `ORACLE.Tests\ORACLE.Tests.csproj` | xunit.net |
| Microsoft.NET.Test.Sdk | 17.8.0 | 18.11.0 | `ORACLE.Tests\ORACLE.Tests.csproj` | nuget.org |
| coverlet.collector | 6.0.0 | 10.1.0 | `ORACLE.Tests\ORACLE.Tests.csproj` | nuget.org |
| **.NET Runtime** | `net8.0` (ORACLE, ORACLE.Tests, SENTINEL, Domain); lokal 8.0.31 | .NET 10 LTS (lokal Runtime 10.0.13, SDK 10.0.402) | alle Projekte | dotnet.microsoft.com Support-Policy |
| KOMPASS.json (von KODE) | – | – | `Metadata\KompassService.cs:23`, `Pipeline\OracleConfig.cs:19`, `Pipeline\TrackLengthGate.cs:84` | nur `docs\KODE_KOMPASS_SCHEMA.md` (Repo) |
| MotoDB `loadChannels.xlsx` (Sheets CIRCUITS, HED) | – | – | `Pipeline\OracleConfig.cs:22`, `Processing\MotoDbInputSync.cs:7,60-69`, `DOMAIN\…\LoadChannelsWriter.cs` (`CircuitsSheet`/`HedSheet`) | keine externe Doku |
| `.HED`-Dateien | – | – | `Metadata\HedFileService.cs` (z. B. `:68-72`, `:100`, `:154`, `:198`) | keine externe Doku |
| KRAKEN (Job-Runner) / `download.done` | – | – | Exit-Codes `Pipeline\OracleExitCode.cs:15-66`; `download.done` von `SENTINEL\DownloadDoneWriter.cs:6-31` | keine externe Doku; `download.done.xsd` liegt **nicht** im Repo |
| WinTAX VCH-Dateien | `docs\vch\KTM_ORR_ValidStart.vch` | – | nachimplementiert: `Detection\StartDetector.cs:3` | `docs\vch\ValidStart.md`, `docs\Wintax4\RuntimeAnalysis\VCH*` |

### 1.1 Vollständigkeit der Markdown-Extrakte im Repo (vs. CHM)

| CHM | HTML-Seiten im CHM | `.md` im Repo | Ergebnis |
|---|---|---|---|
| KtmDataHandlerDocumentation.chm | 664 | 665 (+ `index.md`) | vollständig (alle 664 Seitennamen vorhanden) |
| KtmSharedUtilsDocumentation.chm | 473 | 474 (+ `index.md`) | vollständig |
| KtmSysmaAutomationDocumentation.chm | 136 | 137 (+ `index.md`) | vollständig |
| KtmVMSAutomationDocumentation.chm | 178 | 179 (+ `index.md`) | vollständig |
| Wintax4.chm (18.03.2026) | 365 `.htm` (356 eindeutige Namen) | 372 (360 eindeutige Namen) | alle CHM-Seiten vorhanden; **3 zusätzliche** md-Seiten (`acqmanager_dvrstartupwizard`, `acqmng_fdl`, `acquisition_manager_dvr`), die im aktuellen CHM fehlen → Extrakt stammt aus einem anderen CHM-Build. **Bilder fehlen** (CHM: 868 jpg, 144 png, 18 gif, 7 bmp; Repo: keine) |
| WinTAX4AutomationInterfaces.chm | 132 | 132 | vollständig |

Qualitätsmängel der Extrakte: Encoding-Artefakte (`Ã‚` in KtmDataHandler-Seiten, `â€œ` in Wintax4-Seiten) und Überschriften ohne
Trenner (z. B. `# MarelliDataAccessSetChannelsSplitByLap Method`). Inhaltlich (Signaturen/Summary) stimmen die geprüften Seiten mit
`KtmDataHandler.xml` überein. `KtmDataHandler.xml` (27.11.2024) gehört zum Build 3.5.7.0.

---

## 2. Tools im Detail

### 2.1 KtmDataHandler.dll 3.5.7.0 (+ TelDataZTXx64.dll 1.55.00)

**Version/Plattform (per Metadaten verifiziert):** TFM `.NETFramework,Version=v4.7.2`, PE-Machine `Amd64` (IL-only, aber nur x64
lauffähig). Assembly-Referenzen: `KtmSharedUtils 1.5.10.0`, `Autofac 6.3.0.0`, `Newtonsoft.Json 13.0.0.0`, `System.Drawing`,
`System`, `System.Xml`, `System.Core`, `Microsoft.CSharp`. Die Klasse `MarelliDataAccess` implementiert u. a.
`IMarelliDataAccessRead`, `IMarelliDataAccessWrite`, `IGettableLapMarks`, ist **nicht** `IDisposable`, hat aber einen Finalizer
(`KtmDataHandler.xml`, Member `MarelliDataAccess.Finalize`). Konstruktor: 6 optionale Interface-Parameter (Default `null`) –
deshalb die Autofac-Factory (`Runtime\KtmDataHandlerFactory.cs:9-18`).

#### 2.1.1 Von ORACLE genutzte APIs

Signaturen per Reflection aus der DLL; Semantik aus `KtmDataHandler.xml` bzw. den md-Seiten in `docs\KtmDataHandlerDocumentation`.

| API (Signatur) | Dokumentierte Semantik / Rückgabe | Fallstricke laut Doku/Analyse | Nutzung in ORACLE |
|---|---|---|---|
| `void SetDataLocation(string[] LocationURL, bool bLinktoApplication = true)` | „Set the data location“; `bLinktoApplication`: „true by default – set false if you don't want to link to the application, avoiding any TelDataX/WinTax connection“ (XML-Zeilen 2165-2171; md `1abce50e-…`) | Kein Rückgabewert, Fehler erst bei `LoadRun`. `GetChannels` arbeitet laut Doku „via TelDataX“ – ob Lesen/Schreiben mit `false` funktioniert, ist **nicht dokumentiert**. | immer Default `true`: `ImportDataCalculator.cs:127,216`, `GpsLapSplitService.cs:431,465,480,497`, `SessionMergeService.cs:264`, `SessionMergeExporter.cs:166`, `RaceSegmentDetector.cs:106`, `StartAnalyzer.cs:72`, `OraclePipeline.cs:418` |
| `int LoadRun()` (+ Überladungen mit `string[] propertiesToRead`, `MarelliOpenFilters`) | „Load a Run from location that has been set using SetDataLocation“; Rückgabe **-1 = Fehler beim Öffnen**, **≥ 0 = Anzahl geladener RUNS** (XML 2294-2312; md `3f3f0af1-…`) | `0` heißt „kein Run geladen“, ist laut Doku aber kein Fehlercode. Keine Exception-Semantik dokumentiert. | geprüft `< 0`: `GpsLapSplitService.cs:432,466,498`, `RaceSegmentDetector.cs:107-109`, `StartAnalyzer.cs:73`, `SessionMergeValidator.cs:192-194`, `ImportDataCalculator.cs:128-130`; **ignoriert**: `OraclePipeline.cs:419` |
| `string[] GetChannelsNames()` | „Get channel names“ (md `ac3253a7-…`) | Null-Fall nicht dokumentiert | überall mit Null-Guard, z. B. `GpsLapSplitService.cs:436`, `SessionMergeValidator` (`ValidateChannelNames`), `OraclePipeline.cs:421-425` |
| `IDataset GetChannels(string[] ChnNames, double tStart = -1, double tStop = -1, string[] Aliases = null, bool BRemoveNoRx = true)` | „Get channels via TelDataX“; `BRemoveNoRx`: „Remove NoRx samples“ (XML 2214-2224; md `a0fcee97-…`) | **Default entfernt NoRx-Samples** → Zeitvektoren lückenhaft/Kanäle evtl. unterschiedlich lang. Wird so gelesenes Material zurückgeschrieben, sind die NoRx-Samples im Ergebnis dauerhaft weg. | `ImportDataCalculator.cs:162`, `GpsLapSplitService.cs:442,470,504`, `SessionMergeService.cs:299`, `RaceSegmentDetector.cs:123`, `StartAnalyzer.cs:93`. Längenprüfung lat/lon vorhanden: `ImportDataCalculator.cs:172`, `GpsLapSplitService.cs:447` |
| `(IDataset[] perLap, IDataset combined) GetChannels(string[] ChannelList, uint[] NLaps, string[] Aliases = null, bool BRemoveNoRx = true)` | „Get the channels for the specified laps“; `NLaps`: „Lap numbers (**one based**)“; Rückgabe „per lap and cumulative“ (XML 2225-2236; md `d2cb3d2c-…`) | 1-basiert (im Gegensatz zu `SelectLap`, das 0-basiert ist, XML `SelectLap`) | `ImportDataCalculator.cs:256-257` mit `n = 1..lapCount` (korrekt 1-basiert), aber `BRemoveNoRx = false` – **abweichend** vom Run-Level-Aufruf `:162` (Default `true`) |
| `double[] GetLapMarks()` (aus `IGettableLapMarks`) | „Return the Lap Marks Array“ (XML 2183-2190; md `35d48a17-…`, `75ef8cf6-…`) | Einheit/Format nicht dokumentiert; aus dem Beispiel bei `SetChannelsSplitByLap` (`[0, 62, 120]` = 2 Laps) folgt: *n* Laps ↔ *n+1* Marks, Sekunden | `GpsLapSplitService.cs:435,501`; `ImportDataCalculator.cs:235` (über Cast auf `MarelliDataAccess`, prüft `marks.Length == lapCount + 1`) |
| `double GetLastSampleTime()` | „Get the last time sample of the loaded data“, „[sec]“ (XML 2206-2213; md `011b610e-…`) | – | `RaceSegmentDetector.cs:122`, `StartAnalyzer.cs:102`, `SessionMergeService.cs:314` |
| `void ReleaseObjects()` | „Force to release Marelli PC tools objects“ (XML 2178-2182; md `f3023450-…`, `244db264-…`) | Klasse ist nicht `IDisposable` → `using` nicht möglich; ohne Aufruf bleibt die Freigabe dem Finalizer überlassen | konsequent in `finally`: `ImportDataCalculator.cs:77-78`, `GpsLapSplitService.cs:454,486-487,509`, `SessionMergeService.cs:350`, `SessionMergeExporter.cs:182`, `RaceSegmentDetector.cs:168`, `StartAnalyzer.cs:125`, `OraclePipeline.cs:437` |
| `(uint, uint, uint, string)[] AddImportDataToRun(IDataset ds, string TempFolderPath = "", bool BDeletePreviousImportdata = true)` | Interface-Doku: „Add import channels to existing RUN; **existing importData.ztx files are removed**“; `BDeletePreviousImportdata`: „Delete previous importData; **if false, it is renamed**“; Rückgabe „Write errors“ (XML 2087-2093; md `a8ec2d5b-…`). Die Klassen-Doku (`7703d297-…`, XML 2433-2439) lässt den Lösch-Hinweis weg. | (1) **Löscht vorhandene `importData.ztx`** im Run – auch solche, die nicht von ORACLE stammen (WinTAX-Import-Assistenten erzeugen `ImportData.ztx`, siehe `docs\Wintax4\DataBrw\ImportAscii.md:213`, `ImportMatLab.md:137`, `Import_from_MDF.md:108`). (2) Fehler kommen als Tupel-Array, nicht als Exception. Alternative: `AppendImportDataToRun(...)` – Remark: „Before launching this method all the TelDataX instances previously open must have no VCH loaded“ (md `dec6445c-…`). `RemoveImportData()` existiert separat (md `726f06c9-…`). | `ImportDataCalculator.cs:216-219` (Fehler via `Runtime\KtmWriteErrors.cs` ausgewertet → `IOException`); `SessionMergeExporter.cs:171` (**Rückgabe ignoriert**; Kommentar `:169-170`: 3-Arg-Variante mit eigenem Temp-Ordner schlug fehl) |
| `(uint, uint, uint, string)[] SetChannelsSplitByLap(IDataset ds, double[] tLapMarks, ushort[] lapTSOMs = null)` | „Write channels splitted by laps in MArelli data format, given the LapMarks“; `tLapMarks`: „i.e. [ 0 , 62, 120 ] identifies 2 laps“; `lapTSOMs`: nur „LAP TSO“; Rückgabe „Write errors“ (XML 2102-2108; md `8f2253be-…`, `7656c168-…`) | `lapTSOMs` ist außer „LAP TSO“ **undokumentiert** (Einheit ms nur aus dem Parameternamen ableitbar). **Tupel-Elemente sind unbenannt und undokumentiert.** | `GpsLapSplitService.cs:476-481`: `lapTSOMs` = Millisekundenanteil jedes Lap-Starts (Kommentar `:476`: „without it the API rejects slow channels“ – empirisch, nicht dokumentiert); Ziel ist ein Temp-Ordner (`:480`). Fehlerinterpretation `Runtime\KtmWriteErrors.cs:13-18`: `Item1 != 0 \|\| Item3 != 0` = Fehler, `Item4` = Text (Annahme, nicht dokumentiert) |
| `string[] FindRuns(string initialUrl, RunType filterFolderToOpen = RunType.All, string[] yearToFilter = null, string[] circuitToFilter = null, string[] sessionToFilter = null, string[] deviceToFilter = null)` (`MarelliDataBrowse`) | „Find the run locations within a specific location“; Rückgabe „Full path of the RUNS“; Filter: Präfix `~` = herausfiltern, sonst hineinfiltern (XML 1114-1125; md `769a1bc4-…`). `RunType`: `All`, `BikeWarmUp`, `BikeRun` („no GRID“), `BikeGrid`, `None` | Default `All` liefert **auch Warm-up- und Grid-Runs** | immer Default: `RaceSegmentDetector.cs:70`, `StartAnalyzer.cs:24`, `SessionMergeService.cs:103`, `OraclePipeline.cs:312,542,1355` |
| `bool IsRunLocation(string runPath)` | „The location refers to an bike run“ (XML 1140-1146; md `92c873df-…`) | Kriterien **nicht dokumentiert** | `OraclePipeline.cs:110,368,522,1350`, `RaceSegmentDetector.cs:68`, `StartAnalyzer.cs:22`, `SessionMergeService.cs:189`, `SessionMergeValidator.cs:103` (Kommentar `:102` beschreibt Interna „Lap_*-Unterordner + cableData.ztx + Run_-Prefix“ – nicht durch Doku belegt) |
| `string GetEventName/GetSessionName/GetDeviceName/GetRunName(string runPath)` | „… based on the folder tree“; Rückgabe „**empty string invalid**“ (XML 1035-1069; md `cb685b63-…`, `8a2d4b00-…`, `b3602ed7-…`, `8c20dbed-…`) | Ungültig = leerer String, nicht `null` | `KompassService.cs:75-78`, `OraclePipeline.cs:872,1330-1334`, `SessionMergeService.cs:191-192` – jeweils `?? ""`, d. h. „ungültig“ und „fehlend“ fallen zusammen |
| `MarelliDataLicenses LicenseToUse { get; set; }`, `GetDataLicenses()` | „License to use to write ZTX“, „Get available license list“; Enum enthält `KTM_OFFROAD_EXPERT`, `KTM_OFFROAD_SAT` (XML 945-990, 2005-2010) | Default-Verhalten beim Schreiben nicht dokumentiert | **nicht genutzt** |

#### 2.1.2 Laufzeit-/Plattform-Befunde (verifiziert)

- **.NET-Framework-Assembly auf .NET 8:** KtmDataHandler/KtmSharedUtils sind `net472`-Builds. KtmDataHandler referenziert über die
  `System`-/`System.Drawing`-Fassaden u. a. `System.Diagnostics.EventLog` (Typen `EventLog`, `EventLogEntryType`) und
  `System.Drawing.ContentAlignment`. In .NET 8 leitet die `System.dll`-Fassade `EventLog` an `System.Diagnostics.EventLog` und die
  `System.Drawing.dll`-Fassade `ContentAlignment` an `System.Drawing.Common` weiter; **beide DLLs sind nicht Teil von
  `Microsoft.NETCore.App 8.0.31`**. `System.Diagnostics.EventLog.dll` liegt in `bin` nur, weil das Paket
  `System.Configuration.ConfigurationManager` es mitbringt; `System.Drawing.Common.dll` fehlt → Code-Pfade in KtmDataHandler, die
  `ContentAlignment` nutzen (vermutlich GUI-Pfade), würden mit `FileNotFoundException` scheitern.
- **Corrupted State Exceptions:** `KtmDataHandler.dll.config` (Suite) setzt `legacyCorruptedStateExceptionsPolicy enabled="true"`.
  Das wirkt nur unter .NET Framework im Host-Prozess. Unter .NET 8 ist die Behandlung von CSE nicht unterstützt
  (`HandleProcessCorruptedStateExceptionsAttribute` „is ignored“, Microsoft Learn). Eine Access Violation in `TelDataZTXx64.dll`
  beendet ORACLE daher sofort; der `catch` in `Program.cs:75-82` greift nicht, KRAKEN sieht einen Exit-Code außerhalb von
  `OracleExitCode` (z. B. `0xC0000005`).
- **DLL-Suchreihenfolge:** `KtmRuntime` ruft `SetDllDirectory(<Suite>)` auf (`Runtime\KtmRuntime.cs:67`). Laut Microsoft ist danach
  die Reihenfolge: 1. Verzeichnis der Anwendung, 2. `SetDllDirectory`-Pfad, … 6. `PATH`. Weil `ORACLE.csproj:41-43` eine Kopie von
  `TelDataZTXx64.dll` ins Output kopiert (verifiziert in `bin\Release\net8.0`), **gewinnt die Kopie gegenüber der Suite**. Aktuell
  identisch (1.55.00, gleicher SHA256); nach einem Suite-Update entstünde still ein Versionsmix KtmDataHandler(neu)/TelDataZTX(alt).
- **Managed-Abhängigkeiten aus der Suite** werden per `AssemblyResolve` und `Assembly.LoadFrom` geladen, Versionen ignoriert
  (`Runtime\KtmRuntime.cs:73,82-100`). Betroffen sind u. a. `log4net 2.0.12` (net45), `Newtonsoft.Json 13.0.0.0` (net45),
  `Microsoft.Bcl.AsyncInterfaces 1.0.0.0`.
- **Plattform:** PE-Machine `Amd64` bei beiden KTM-Assemblies; `ORACLE.csproj:8` erlaubt `AnyCPU;x64`. Der Kommentar
  `ORACLE.csproj:24` („KTM telemetry DLLs are AMD64 native“) ist ungenau: Die DLLs sind managed (IL-only), aber auf x64 festgelegt.
- **DI-Auflösung:** Aktuell hat jedes vom `MarelliDataAccess`-Konstruktor benötigte Interface (`ITelDataX`, `ITelDataZTX`,
  `ILocalLogHelper`, `IMarelliDataXmlProcessingConstants/UserRecords/Meteo`) genau **eine** öffentliche Implementierung
  (Reflection), und keine öffentliche konkrete Klasse in KtmDataHandler/KtmSharedUtils implementiert `IDisposable`.

#### 2.1.3 Best Practices (aus Doku und Analyse)

- Pro Run frische Instanzen + `ReleaseObjects()` in `finally` (wird in ORACLE durchgängig so gemacht).
- Rückgaben auswerten: `LoadRun` (−1/0), Write-Tupel von `AddImportDataToRun`/`SetChannelsSplitByLap`/`SetChannels`.
- Schreiben zuerst in einen Temp-Ordner und danach atomar tauschen (umgesetzt in `GpsLapSplitService.cs:480` + `Processing\LapSplitStore.cs`).
- `AppendImportDataToRun` nur, wenn keine TelDataX-Instanz mit geladenen VCHs offen ist (Remark der Doku).

#### 2.1.4 Deprecations / Breaking Changes

- Kein Changelog/Release-Notes für KtmDataHandler/KtmSharedUtils verfügbar → Breaking Changes nicht ermittelbar.
- Ziel-Framework `net472` ist ein Fremdkörper in einer .NET-8/10-Anwendung (siehe §2.1.2); jede neue Suite-Version muss gegen
  ORACLE erneut getestet werden.

#### 2.1.5 Verbesserungsmöglichkeiten für ORACLE

1. **`LoadRun`-Rückgabe überall prüfen und `0` als „kein Run geladen“ behandeln** – `OraclePipeline.cs:419` ignoriert den Wert
   komplett; `GpsLapSplitService.cs:432,466,498`, `RaceSegmentDetector.cs:107-109`, `StartAnalyzer.cs:73`,
   `SessionMergeValidator.cs:194` und `ImportDataCalculator.cs:129` akzeptieren `0`. Vor der Umstellung per Test klären, ob `0`
   in der Praxis auftritt.
2. **Write-Fehler im Merge-Export auswerten:** `SessionMergeExporter.cs:171` verwirft das Fehler-Tupel; analog zu
   `ImportDataCalculator.cs:217` über `KtmWriteErrors.Describe` prüfen und `WriteMergedData` (`:161`) dann `false` liefern lassen.
3. **Lösch-Semantik von `AddImportDataToRun` explizit machen:** In `ImportDataCalculator.cs:217` und `SessionMergeExporter.cs:171`
   `BDeletePreviousImportdata` als benanntes Argument setzen und dokumentieren, dass ORACLE `importData.ztx` „besitzt“. Falls
   Anwender eigene WinTAX-Importe (ImportData.ztx) in Runs ablegen, `BDeletePreviousImportdata: false` (Umbenennen statt Löschen)
   oder `AppendImportDataToRun` evaluieren.
4. **Tupel-Semantik absichern:** Die Annahme in `Runtime\KtmWriteErrors.cs:13-18` (Item1/Item3 = Fehlerzähler, Item4 = Text,
   `"No errors."`) ist nicht dokumentiert → beim KTM-Utils-Team bestätigen lassen und mit einem Integrationstest gegen echte
   Fehlerfälle (z. B. ungültige Frequenz) festnageln.
5. **NoRx-Verhalten bewusst wählen:** Split-Rewrite liest mit Default `BRemoveNoRx = true` (`GpsLapSplitService.cs:470`) und schreibt
   das Ergebnis per `SetChannelsSplitByLap` zurück (`:481`); dasselbe gilt für Merge (`SessionMergeService.cs:299` →
   `SessionMergeExporter.cs:171`). Damit gehen NoRx-Samples im Ergebnis verloren. Für 1:1-Umschreibungen `BRemoveNoRx: false`
   prüfen. Außerdem ist `ImportDataCalculator.cs:162` (true) vs. `:256-257` (false) inkonsistent.
6. **Cast entfernen:** `GetLapMarks()` ist über `IGettableLapMarks` Teil von `IMarelliDataAccessRead` (verifiziert);
   `(ztxAccessRead as MarelliDataAccess)?.GetLapMarks()` in `ImportDataCalculator.cs:235` kann direkt `ztxAccessRead.GetLapMarks()`
   sein. Damit ist auch `KtmDataHandlerFactory.CreateAccess()` nur noch nötig, wo wirklich Read+Write auf einer Instanz gebraucht wird.
7. **`FindRuns`-Typfilter prüfen:** Default `RunType.All` (`RaceSegmentDetector.cs:70`, `StartAnalyzer.cs:24`,
   `SessionMergeService.cs:103`, `OraclePipeline.cs:312,542,1355`) schließt Warm-up/Grid-Runs ein. Klären, ob diese Typen in
   Offroad-Daten vorkommen; falls ja, `RunType.BikeRun` übergeben oder `GetRunType` loggen.
8. **„Empty string invalid“ auswerten:** `?? ""` in `KompassService.cs:75-78` und `OraclePipeline.cs:872,1330-1334` macht ungültige
   Namen unsichtbar → `string.IsNullOrEmpty` explizit als „vom API nicht ermittelbar“ behandeln/loggen.
9. **Lizenz beim Schreiben sichtbar machen:** `LicenseToUse`/`GetDataLicenses()` werden nicht genutzt. Im `validate`-Pfad
   (`OraclePipeline.cs:415-437`) verfügbare Lizenzen loggen und klären, ob `KTM_OFFROAD_EXPERT`/`KTM_OFFROAD_SAT` explizit gesetzt
   werden sollte.
10. **Instanzerzeugung absichern:** `ImportDataCalculator.cs:48-49` und `GpsLapSplitService.cs:461-462` erzeugen zwei Instanzen vor
    dem `try`; wirft die zweite Erzeugung, wird die erste nicht freigegeben → Erzeugung in den `try` verlegen oder verschachteln.
11. **`PlatformTarget` auf x64 festlegen** (`ORACLE.csproj:8`) und den Kommentar `ORACLE.csproj:24` korrigieren (managed, x64-only).
12. **TelDataZTX-Kopie entfernen oder Version prüfen:** `ORACLE.csproj:41-43` + `ORACLE\TelDataZTXx64.dll` überschatten die Suite
    (DLL-Suchreihenfolge). Entweder Kopie entfernen (Suite ist via `KtmRuntime` auffindbar) oder beim Start `FileVersion` von Kopie
    und Suite vergleichen.
13. **Native Abstürze einplanen:** CSE sind unter .NET 8 nicht abfangbar → KRAKEN-Mapping für „unbekannte“ Exit-Codes (z. B.
    `0xC0000005`) als `GeneralError` dokumentieren (`Pipeline\OracleExitCode.cs:4-27`); optional riskante native Schritte in einen
    Kindprozess auslagern.
14. **Abhängigkeits-Smoke-Test:** Ein Test, der jede genutzte KtmDataHandler-API einmal gegen einen Mini-Run ausführt, würde
    fehlende Fassaden-Ziele (`System.Drawing.Common`, `System.Diagnostics.EventLog`) und Suite-Updates früh aufdecken. Zusätzlich
    beim KTM-Utils-Team einen `netstandard2.0`/`net8.0`-Build anfragen.
15. **`SetDataLocation(…, bLinktoApplication: false)`** nur nach Test erwägen (weniger Kopplung an TelDataX/WinTAX laut Doku); für
    Lesezugriffe „via TelDataX“ ist unklar, ob das funktioniert.

### 2.2 KtmSharedUtils.dll 1.5.10.0

- **Version:** 1.5.10.0, `net472`, `Amd64`. Referenziert `log4net 2.0.12.0`, `MathNet.Numerics 5.0.0.0`, `Newtonsoft.Json 13.0.0.0`,
  `System.IO.Compression.FileSystem`.
- **Relevante Funktionen:** DI-Abhängigkeiten von KtmDataHandler (`IPropertyList` & Co., generische `XMLFileProcessor<>`,
  `BinaryFileProcessor<>`, `JsonFileProcessor<>`) – registriert in `Runtime\KtmDataHandlerFactory.cs:57-67`.
  Log-Schalter `BLogEnable`, `BDeepLogEnable`, `BEnableTraceView` sind in `docs\KtmSharedUtilsDocumentation` dokumentiert
  (z. B. `11522bf4-…` `ILogHelper.BEnableTraceView`); ORACLE schaltet sie ab (`KtmDataHandlerFactory.cs:73-81`, Begründung:
  log4net-Feld bleibt beim parameterlosen Konstruktor `null`).
- **Best Practice / Verbesserung:**
  1. Die Registrierung „alle öffentlichen Typen, `AsImplementedInterfaces()`“ (`KtmDataHandlerFactory.cs:48-61`) ist bei
     Suite-Updates fragil (bei Autofac gewinnt die zuletzt registrierte Implementierung). Für die sechs Konstruktor-Abhängigkeiten
     von `MarelliDataAccess` explizite Registrierungen oder einen Start-Check „genau eine Implementierung“ ergänzen.
  2. Unbenutzte `using`-Direktiven in `ImportDataCalculator.cs:1-7` (z. B. `KtmDataHandler.Marelli.Sysma`) prüfen/entfernen.

### 2.3 WinTAX 4 Datenformat, HASP, VCH

- **Format (Doku `docs\Wintax4\DataBrw\DataArchives_Full.md`):** ZTX-Archiv mit sechs Verzeichnisebenen. RUN-Ordner enthält
  LAP-Ordner, `Session.xml`, `Car.xml`, `RunComments.xml`, „Lap Header Record file named **RunLapHeaders.xml**“, `RunOffset.xml`.
  LAP-Ordner enthält ZTX-Dateien (`dstData.ztx`, `cableData.ztx`, `nbtData.ztx`), `LapHeader.xml`, `Constants.xml`.
  Lap-Ordnernamen: `Lap_Abs_SN`, bei Kollision `Lap_Abs_SN~nnn`, Trigger-Out `Lap_Abs#SN~nnn`, Auto-Limit `…@…`.
  Import-Laps werden über `importData.ztx` identifiziert (`DataBrw\Filters.md:29,42`).
- **ORACLE:** schreibt XML direkt statt über die KtmDataHandler-Property-API (`Processing\XmlRunEditor.cs:9`, Dateien
  `:79,91,109,126,150`), `Processing\LapSplitStore.cs:84-85`.
- **HASP (Doku `USB_Protection_troubleshooting.md`):** H0007 „Sentinel Hasp Key not found“ (Dienst „Sentinel LDK License Manager“
  muss auf *Automatisch/Running* stehen), H0011/H0042 (License Manager zu alt), **H0027 „Terminal Services detected“** (Start in
  Remote-Desktop-Sitzung). Die Doku bezieht sich auf WinTAX; ob TelDataZTX identisch prüft, ist nicht dokumentiert.
- **Verbesserungen:**
  1. **Dateiname prüfen:** ORACLE nutzt `RunLapHeader.xml` (`XmlRunEditor.cs:150`, `LapSplitStore.cs:84`), die WinTAX-Doku
     nennt `RunLapHeaders.xml` (im CHM-Original identisch geschrieben). Gegen echte Runs verifizieren; falls beide Varianten
     vorkommen, beide behandeln.
  2. **Lap-Sortierung:** Ordnernamen mit `~nnn`, `#`, `@` sortieren ordinal nicht zwingend chronologisch (`Pipeline\LapHistory.cs:43,64`,
     `XmlRunEditor.OrderedLapFolders`). Reihenfolge besser aus `RunLapHeader(s).xml`/Lap-Marks ableiten oder per Test absichern.
  3. **HASP-Preflight:** Im `validate`-Pfad Dienststatus „Sentinel LDK License Manager“ und RDP-Sitzung (H0027) prüfen und mit
     klarer Meldung/Exit-Code abbrechen, statt erst in nativen Aufrufen zu scheitern.
  4. **ImportData-Kollision** mit WinTAX-Import-Assistenten: siehe §2.1.5 Nr. 3.

### 2.4 Autofac 6.3.0

- **Version:** 6.3.0 (aktuell 9.3.4). Die Suite liefert `Autofac.dll` 6.3.0 (`netstandard2.0`); **KtmDataHandler ist gegen Autofac
  6.3.0.0 kompiliert** und nutzt intern `ContainerBuilder`, `RegisterType`, `WithParameter`, `Build`,
  `ILifetimeScope.BeginLifetimeScope`, `Resolve` (Metadaten-Analyse).
- **Relevante Funktionen in ORACLE:** `RegisterAssemblyTypes(...).PublicOnly().AsImplementedInterfaces().AsSelf()`,
  `RegisterGeneric`, `Register(...).SingleInstance()`, `Resolve<T>()` (`KtmDataHandlerFactory.cs:40-109`).
- **Best Practices (docs.autofac.org):** nicht aus dem Root-Container auflösen, sondern pro Arbeitseinheit eine Lifetime-Scope;
  Komponenten, die `IDisposable` sind, werden sonst bis zum Container-Dispose gehalten. Da kein KTM-Typ `IDisposable` ist
  (verifiziert), entsteht aktuell kein Leak – die Regel ist hier Hygiene, kein Bug.
- **Breaking Changes:**
  - 6→7: Pflicht-Properties (`required`) werden automatisch injiziert; `RegisterGeneratedFactory` obsolet; `ILifetimeScope` um
    `BeginLoadContextLifetimeScope` erweitert; `net5.0` entfällt.
  - 7→8: `ResolveRequest` wird `readonly struct`; Shim `RequiresUnreferencedCodeAttribute` ab 8.4.0 `internal`; Targets `netstandard2.0`, `net8.0` u. a.
  - 8→9: keine öffentlichen API-Änderungen; `net6.0`/`net7.0` entfallen, `net10.0` kommt hinzu.
- **Verbesserungen:**
  1. Ein Upgrade ist **an KtmDataHandler gekoppelt** (dieselbe Autofac-Assembly wird von ORACLE und KtmDataHandler genutzt).
     Die von KtmDataHandler verwendeten APIs existieren laut Upgrade-Guides weiter; trotzdem nur mit Integrationstest
     (Smoke-Test aus §2.1.5 Nr. 14) auf 8.x/9.x heben. Ohne Test bei 6.3.0 bleiben.
  2. Explizite Registrierungen statt Assembly-Scan (siehe §2.2 Nr. 1).
  3. Optional: pro Run `BeginLifetimeScope()` statt Root-`Resolve` (`KtmDataHandlerFactory.cs:94,101,109`).

### 2.5 ClosedXML 0.105.1

- **Version:** 0.105.1 = aktuelle Version (NuGet). Vorabversion 0.x → Breaking Changes in Minor-Releases möglich (Release-Notes
  0.102.1–0.104.1: Wechsel auf OpenXML SDK 3, Parser-Austausch XLParser→ClosedParser, neue Formelberechnung; „Summary of breaking
  changes“ auf docs.closedxml.io).
- **Nutzung:** nur in `DOMAIN\src\Ktm.Racing.Domain\MotoDb\LoadChannelsWriter.cs` (Lesen mit `FileShare.ReadWrite | FileShare.Delete`
  `:113-115`; Schreiben über `SaveAtomic` mit Temp-Datei + `File.Move` `:454-474`). Gesperrte Workbook → `IOException` →
  `OracleExitCode.ResourceLocked` (`Processing\MotoDbInputSync.cs:69`).
- **Best Practices:** `using var workbook` (umgesetzt), Temp-Datei mit gültiger Endung `.xlsx` (umgesetzt, Kommentar `:457`).
- **Verbesserungen:**
  1. `ORACLE.csproj:19` – direkte ClosedXML-Referenz ist im ORACLE-Code ungenutzt (kommt transitiv über `Ktm.Racing.Domain`).
     Entfernen oder bewusst als Versions-Pin kommentieren.
  2. Version in ORACLE, ORACLE.Tests, Domain und Domain.Tests gemeinsam pflegen (z. B. Central Package Management mit
     `Directory.Packages.props`), weil 0.x-Updates brechen können.

### 2.6 System.Configuration.ConfigurationManager 9.0.9

- **Warum nötig (verifiziert):** `log4net.dll` 2.0.12 aus der Suite (Build `.NETFramework 4.5`) referenziert `System.Configuration
  4.0.0.0`. In .NET 8 leitet die Fassade `System.Configuration.dll` die Typen `ConfigurationManager`/`ConfigurationSection` an
  `System.Configuration.ConfigurationManager` weiter, die **nicht** im Shared Framework enthalten ist (`Microsoft.NETCore.App\8.0.31`
  geprüft). Zusätzlich bringt das Paket für `net8.0` `System.Diagnostics.EventLog` mit, das KtmDataHandler referenziert (§2.1.2).
  Weder KtmDataHandler noch KtmSharedUtils referenzieren das Paket direkt.
- **Version:** 9.0.9 (unterstützt `net8.0`), aktuell 10.0.13.
- **Verbesserungen:**
  1. In `ORACLE.csproj:21` einen Kommentar ergänzen („für log4net/KtmDataHandler, nicht entfernen“) – sonst wird es als
     ungenutzt entfernt.
  2. Major-Version an das Ziel-Framework koppeln (beim Wechsel auf `net10.0` → 10.0.x).

### 2.7 MathNet.Numerics 5.0.0

- **Version:** 5.0.0 = neueste Version (03.04.2022; kein neuerer Release).
- **Nutzung:** keine im ORACLE-Code. `KtmSharedUtils` referenziert `MathNet.Numerics 5.0.0.0`; das Paket legt den `net6.0`-Build
  in `bin`, die Suite enthält einen `net461`-Build (würde sonst über `KtmRuntime.ResolveFromSuite` geladen, `KtmRuntime.cs:96`).
- **Verbesserung:** Referenz in `ORACLE.csproj:20` mit Begründung kommentieren oder nach einem Smoke-Test entfernen.

### 2.8 Microsoft.Vbe.Interop

- **Nutzung:** keine. HintPath auf eine lokale Office-Installation (`ORACLE.csproj:35-37`) macht den Build maschinenabhängig und
  kopiert `Microsoft.Vbe.Interop.dll`, `office.dll`, `stdole.dll` ins Output (in `bin\Release\net8.0` verifiziert).
- **Verbesserung:** Referenz entfernen.

### 2.9 Testpakete (xunit 2.5.3, xunit.runner.visualstudio 2.5.3, Microsoft.NET.Test.Sdk 17.8.0, coverlet.collector 6.0.0)

- **Sicherheitsbefund:** `dotnet package list --vulnerable --include-transitive` für `ORACLE.Tests` meldet
  `System.Net.Http 4.3.0` (High, GHSA-7jgj-8wvc-jh57) und `System.Text.RegularExpressions 4.3.0` (High, GHSA-cmhx-cq75-c4mj).
  Kette: `xunit.assert`/`xunit.extensibility.core`/`xunit.extensibility.execution` 2.5.3 → `NETStandard.Library 1.6.1`.
  `xunit.assert` 2.9.3 hat für `net6.0`/`netstandard2.0` **keine** Abhängigkeit auf `NETStandard.Library` (NuGet-Katalog) →
  Update auf 2.9.3 beseitigt den Befund. ORACLE (Produktivprojekt) hat keine anfälligen Pakete.
- **xunit v3 (`xunit.v3` 4.0.2):** Testprojekte werden eigenständige Executables, Mindest-Runtime .NET 8 / .NET Framework 4.7.2,
  `async void`-Tests schlagen fehl, `IAsyncLifetime` erbt von `IAsyncDisposable` (Migration Guide xunit.net).
- **Verbesserungen:**
  1. Kurzfristig `xunit` → 2.9.3 (ORACLE.Tests und `DOMAIN\tests\Ktm.Racing.Domain.Tests`).
  2. `Microsoft.NET.Test.Sdk` (→ 18.11.0), `coverlet.collector` (→ 10.1.0) und `xunit.runner.visualstudio` (→ 4.0.1) aktualisieren;
     Kompatibilität des Runners 4.x mit xunit v2 vorher prüfen (nicht verifiziert).
  3. Migration auf xunit v3 zusammen mit dem .NET-10-Wechsel planen.

### 2.10 .NET 8 Runtime

- **Support-Ende:** .NET 8 (LTS) endet am **10.11.2026** – rund drei Wochen nach diesem Stand. Danach keine Sicherheitsupdates.
  .NET 10 (LTS) wird bis November 2028 unterstützt.
- **Verbesserung:** ORACLE, ORACLE.Tests, SENTINEL und Ktm.Racing.Domain auf `net10.0` heben (Runtime 10.0.13/SDK 10.0.402 lokal
  vorhanden). Wegen `net472`-KTM-DLLs danach den Smoke-Test aus §2.1.5 Nr. 14 ausführen; `System.Configuration.ConfigurationManager`
  auf 10.0.x anheben.

### 2.11 Datenaustausch (KOMPASS, MotoDB, HED, KRAKEN, VCH)

- **KOMPASS.json** (KODE): Schema nur im Repo (`docs\KODE_KOMPASS_SCHEMA.md`); ORACLE liest sie in `Metadata\KompassService.cs` und
  `Pipeline\TrackLengthGate.cs`, protokolliert Version/Hash über `Pipeline\InputFingerprint.cs:7-33`.
- **MotoDB loadChannels.xlsx:** Standardpfad `Pipeline\OracleConfig.cs:22`; Sheets `CIRCUITS`/`HED` (`LoadChannelsOptions`).
- **.HED:** Kanonischer Name `{Season}{Event}_{Bike}{Session}.HED` (`Metadata\HedFileService.cs:68-72`).
- **KRAKEN:** Exit-Codes 0–7 in `Pipeline\OracleExitCode.cs:15-66`; `download.done` wird von SENTINEL laut Kommentar „konform zum
  KRAKEN-XSD-Schema“ geschrieben (`SENTINEL\DownloadDoneWriter.cs:6-28`), die XSD liegt aber nicht im Repo.
- **VCH:** `KTM_ORR_ValidStart.vch` ist in `Detection\StartDetector.cs` nachgebaut (`:3`).
- **Verbesserung:** `download.done.xsd` ins Repo übernehmen und in einem SENTINEL-Test gegen die XSD validieren; Exit-Code-Mapping
  um native Abstürze ergänzen (§2.1.5 Nr. 13).

---

## 3. Nicht verfügbare Quellen / Connectors

- **KtmDataHandler/KtmSharedUtils/TelDataZTX:** Kein Changelog, keine Release-Notes, kein Paket-Feed → „aktuelle Version“ nicht
  ermittelbar. Semantik der Write-Tupel `(uint, uint, uint, string)` und des Parameters `lapTSOMs` ist in keiner Quelle beschrieben.
  Für die native `TelDataZTXx64.dll` existiert keine API-Doku.
- **HASP/Sentinel LDK:** Nur die WinTAX-Troubleshooting-Seite; keine SDK-/Runtime-Doku.
- **KRAKEN, KODE, MotoDB:** Keine externe Doku verfügbar; `download.done.xsd` nicht im Repo.
- **NuGet-MCP `fix_vulnerable_packages`:** Abbruch mit „Could not load file or assembly 'NuGet.Frameworks, Version=7.9.0.0'“
  (SDK 10.0.402) → ersatzweise `dotnet package list --vulnerable --include-transitive --no-restore`. `get_latest_package_version`
  funktionierte.
- **Websuche:** Zwei Suchanfragen (xunit v3, ClosedXML) scheiterten mit HTTP 504 (`github-mcp-server`). Stattdessen wurden die
  offiziellen Seiten direkt abgerufen. `https://docs.closedxml.io/en/latest/migrations/index.html` lieferte 404; die
  Breaking-Change-Zusammenfassung von ClosedXML wurde deshalb nur über die GitHub-Releases-Seite referenziert.
- **Build/Test:** Nicht ausgeführt (parallel arbeitende Agenten). Laufzeitaussagen beruhen auf Metadaten-Analyse und Doku.

---

## 4. Quellenverzeichnis

**Lokale Doku / Artefakte**

1. `C:\Dev\KTM Offroad Scripts\00_ExternalToolsDocumentation\KtmDataHandlerDocumentation.chm` (27.11.2024) → extrahiert nach `%TEMP%\ext-notes\KtmDataHandlerDocumentation`
2. `…\KtmSharedUtilsDocumentation.chm`, `…\KtmSysmaAutomationDocumentation.chm`, `…\KtmVMSAutomationDocumentation.chm` (27.11.2024)
3. `…\Wintax4.chm` (18.03.2026), `…\WinTAX4AutomationInterfaces.chm` (02.04.2026)
4. `C:\KTM Tools\KTM Utils Software Suite\KtmDataHandler.xml`, `KtmSharedUtils.xml`, `KtmDataHandler.dll.config`; DLL-Metadaten (`KtmDataHandler.dll`, `KtmSharedUtils.dll`, `log4net.dll`, `MathNet.Numerics.dll`, `Autofac.dll`, `TelDataZTXx64.dll`)
5. Repo-Extrakte: `docs\KtmDataHandlerDocumentation` – u. a. `1abce50e-a3bb-a28b-3f29-c2ea127847a6.md` (SetDataLocation), `3f3f0af1-5312-193c-45bd-8722fb925731.md` (LoadRun), `ac3253a7-a0c4-acd7-da87-b38d65259be7.md` (GetChannelsNames), `a0fcee97-f878-ab54-2d5d-3b2c7a322e41.md` / `d2cb3d2c-87f5-bf35-1aac-804c48b26459.md` (GetChannels), `35d48a17-65b5-bb3d-e87b-ca8d49d8b49c.md` / `75ef8cf6-05c2-764b-ca66-199381a6cfd9.md` (GetLapMarks), `011b610e-edb5-551b-b130-ffffd2a87793.md` (GetLastSampleTime), `f3023450-773f-f7e0-2994-6013c309fec8.md` / `244db264-35e0-c287-f137-98901c85bf02.md` (ReleaseObjects), `a8ec2d5b-7767-0617-01ad-57cbc0792420.md` / `7703d297-1e01-501e-d894-235be059be83.md` (AddImportDataToRun), `dec6445c-5e9c-c8c2-3eeb-124ef1dba88c.md` (AppendImportDataToRun), `726f06c9-d911-9dc6-3df9-e179310c83e5.md` (RemoveImportData), `8f2253be-32fc-6e26-233c-e5a8961992e4.md` / `7656c168-b1dd-2d99-4e5a-e09b77c64093.md` (SetChannelsSplitByLap), `769a1bc4-de6a-214d-2597-871907973eb3.md` (FindRuns), `92c873df-6e96-37cb-cc93-3cddd045c6f5.md` (IsRunLocation), `cb685b63-e1ca-0814-f1c6-7e6d985bb919.md` (GetEventName), `b4143402-4684-8b48-d51b-a8b7742e564a.md` (LicenseToUse)
6. `docs\KtmSharedUtilsDocumentation\11522bf4-a526-1cb0-7624-70ecb90582e8.md` (BEnableTraceView)
7. `docs\Wintax4\DataBrw\DataArchives_Full.md`, `DataBrw\Filters.md`, `DataBrw\ImportAscii.md`, `DataBrw\ImportMatLab.md`, `DataBrw\Import_from_MDF.md`, `TroubleShooting\TroubleShootingDongle\USB_Protection_troubleshooting.md`
8. `docs\WinTAX4AutomationInterfaces\*.md` (COM-Interfaces; von ORACLE nicht direkt genutzt)
9. `docs\KODE_KOMPASS_SCHEMA.md`, `docs\vch\KTM_ORR_ValidStart.vch`, `docs\vch\ValidStart.md`

**Online**

10. NuGet (MCP `get_latest_package_version` und NuGet-V3-Registration/Katalog): Autofac, ClosedXML, MathNet.Numerics, System.Configuration.ConfigurationManager, xunit, xunit.v3, xunit.assert, xunit.extensibility.core, xunit.runner.visualstudio, Microsoft.NET.Test.Sdk, coverlet.collector, log4net – https://www.nuget.org/ , https://api.nuget.org/v3/registration5-gz-semver2/
11. Autofac – Upgrading 6.x→7.x: https://docs.autofac.org/en/latest/whats-new/upgradingfrom6to7.html ; 7.x→8.x: https://docs.autofac.org/en/latest/whats-new/upgradingfrom7to8.html ; 8.x→9.x: https://docs.autofac.org/en/latest/whats-new/upgradingfrom8to9.html ; Lifetime/Disposal: https://docs.autofac.org/en/latest/lifetime/index.html
12. ClosedXML – Doku: https://docs.closedxml.io/en/latest/ ; Releases: https://github.com/ClosedXML/ClosedXML/releases
13. xUnit.net – Migrating from v2 to v3: https://xunit.net/docs/getting-started/v3/migration
14. .NET Support-Policy: https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core ; https://devblogs.microsoft.com/dotnet/dotnet-8-9-end-of-support/
15. `HandleProcessCorruptedStateExceptionsAttribute` (CSE unter .NET Core/5+ nicht unterstützt): https://learn.microsoft.com/en-us/dotnet/api/system.runtime.exceptionservices.handleprocesscorruptedstateexceptionsattribute
16. `SetDllDirectory` (DLL-Suchreihenfolge): https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-setdlldirectoryw
17. GitHub Advisories: https://github.com/advisories/GHSA-7jgj-8wvc-jh57 , https://github.com/advisories/GHSA-cmhx-cq75-c4mj
