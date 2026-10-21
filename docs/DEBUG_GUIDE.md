# ORACLE & SENTINEL — Debug Guide

## Inhaltsverzeichnis

1. [Quick-Start: Visual Studio Debug-Profile](#1-quick-start-visual-studio-debug-profile)
2. [Architektur-Überblick für Debugging](#2-architektur-überblick-für-debugging)
3. [Testdaten vorbereiten](#3-testdaten-vorbereiten)
4. [Debugging nach Command](#4-debugging-nach-command)
5. [Breakpoint-Strategie](#5-breakpoint-strategie)
6. [Log-Analyse](#6-log-analyse)
7. [oracle.status.json lesen](#7-oraclestatusjson-lesen)
8. [Häufige Fehler & Lösungen](#8-häufige-fehler--lösungen)
9. [KtmDataHandler / Autofac Debugging](#9-ktmdatahandler--autofac-debugging)
10. [ImportDataCalculator Debugging](#10-importdatacalculator-debugging)
11. [Session-Merge Debugging](#11-session-merge-debugging)
12. [SENTINEL Debugging](#12-sentinel-debugging)
13. [Kommandozeilen-Debugging (ohne VS)](#13-kommandozeilen-debugging-ohne-vs)
14. [Checkliste bei neuen Berechnungen](#14-checkliste-bei-neuen-berechnungen)

---

## 1. Quick-Start: Visual Studio Debug-Profile

In `ORACLE\Properties\launchSettings.json` sind 4 Profile vordefiniert:

| Profil | Command | Pfad-Typ | Aufruf |
|--------|---------|----------|--------|
| **ORACLE - Process** | `process` | Run-Ordner | Metadata + Properties + Berechnungen |
| **ORACLE - Merge** | `merge` | Rider-Ordner | Session-Merge über alle Runs |
| **ORACLE - Validate** | `validate` | Run-Ordner | Dateistruktur-Prüfung |
| **ORACLE - Legacy** | *(kein Command)* | Run-Ordner | process + merge (KRAKEN-kompatibel) |

### Profil wechseln

```
Visual Studio Toolbar → Dropdown neben ▶ Start → Profil auswählen
```

### Eigene Testpfade eintragen

`launchSettings.json` anpassen — Pfade müssen in Anführungszeichen stehen:

```json
{
  "profiles": {
    "ORACLE - Process": {
      "commandName": "Project",
      "commandLineArgs": "process \"C:\\MXTrackData\\DEIN_PFAD\\Run_xxx\""
    }
  }
}
```

### --force Flag hinzufügen

Wenn `oracle.status.json` die erneute Verarbeitung blockiert:

```json
"commandLineArgs": "process \"C:\\...\\Run_xxx\" --force"
```

---

## 2. Architektur-Überblick für Debugging

```
Program.Main()                          ← Einstiegspunkt, Command-Router
  │
  ├── OracleLogger.Initialize()         ← Ab hier: Console = Dual (Screen + Datei)
  │
  ├── OraclePipeline.Create()           ← Factory: MarelliDataBrowse + OracleConfig
  │
  └── switch (command)
      │
      ├── "process" → ProcessRunAsync()
      │     ├── OracleStatus (Idempotenz)
      │     ├── ResolveMetadataAsync()    ← HED-Datei oder KOMPASS
      │     ├── DiscoverRuns()            ← IMarelliDataBrowse.FindRuns()
      │     ├── RunPropertyEditor         ← Pro Run: Properties setzen
      │     ├── ImportDataCalculator      ← Pro Run: Berechnungen (sVehicle)
      │     └── OracleStatus.Save()
      │
      ├── "merge" → MergeSession()
      │     ├── SessionMergeService       ← Runs laden + Channels mergen
      │     ├── SessionMergeExporter      ← Ergebnis als WinTAX4-Run speichern
      │     └── OracleStatus.Save()
      │
      ├── "validate" → ValidateRun()
      │     ├── Ordnerstruktur prüfen
      │     ├── cableData.ztx Dateien prüfen
      │     └── Daten ladbar prüfen
      │
      └── _ → HandleLegacyAsync()         ← process + merge sequentiell
```

### Datenfluss der Abhängigkeiten

```
OraclePipeline
  ├── _browse: IMarelliDataBrowse        ← Run-Discovery, Pfad-Validierung
  ├── _config: OracleConfig              ← Pfade, TeamMappings (aus oracle-config.json)
  │
  ├── KtmDataHandlerFactory
  │     └── Autofac Container            ← Cached (Singleton), löst alle DI-Ketten auf
  │         ├── CreateReader()            → IMarelliDataAccessRead
  │         ├── CreateWriter()            → IMarelliDataAccessWrite
  │         └── CreateAccess()            → MarelliDataAccess (Read+Write)
  │
  └── ImportDataCalculator
        ├── ztxAccessRead                 ← Frisch pro Run (via Factory)
        └── ztxAccessWrite                ← Frisch pro Run (via Factory)
```

---

## 3. Testdaten vorbereiten

### Minimale WinTAX4-Ordnerstruktur

```
C:\MXTrackData\Testdata\
  └── 01_KTM-FR\Wintax\2026\450\05_RT\RT12_ESP-Almonte\R1\
      └── AA01\                              ← Rider-Ordner (für merge)
          ├── Run_205\                       ← Run-Ordner (für process/validate)
          │   ├── Lap_001\
          │   │   └── cableData.ztx          ← Telemetriedaten
          │   ├── Lap_002\
          │   │   └── cableData.ztx
          │   └── ...
          ├── Run_206\
          │   ├── Lap_001\
          │   │   └── cableData.ztx
          │   └── ...
          └── _MergedSession\                ← Wird von merge erstellt
              └── Run_Merged\
                  └── Lap_001\
                      └── cableData.ztx
```

### Pflicht-Dateien für `process`

| Datei | Woher | Benötigt von |
|-------|-------|-------------|
| `cableData.ztx` | Logger/Download | KtmDataHandler (Laden, Lesen, Schreiben) |
| `.HED`-Datei ODER KOMPASS.json | Manuell / DB | `ResolveMetadataAsync` (Metadaten) |
| `oracle-config.json` | Ausgabeverzeichnis | `OracleConfig.Load()` |

### Testdaten-Checkliste

- [ ] `cableData.ztx` hat Größe > 0 Bytes (kein OneDrive-Platzhalter)
- [ ] Mindestens 1 `Lap_*`-Ordner vorhanden
- [ ] Für sVehicle-Berechnung: Channels `Lat_dez` und `Lon_dez` müssen im Datensatz existieren
- [ ] `oracle-config.json` liegt im Build-Ausgabeverzeichnis (wird automatisch kopiert)

---

## 4. Debugging nach Command

### 4.1 `process` debuggen

**Ziel:** Vollständige Pipeline für einen Run durchlaufen.

1. Profil **"ORACLE - Process"** auswählen
2. Breakpoints setzen (siehe [Breakpoint-Strategie](#5-breakpoint-strategie))
3. F5 drücken

**Erwarteter Ablauf:**
```
ORACLE gestartet. Log-Datei: ...
Argumente: process C:\...\Run_205
[process] Run Path: C:\...\Run_205
  → Metadaten (HED oder KOMPASS)
  → X gültige(r) Run(s) gefunden
  → Properties gesetzt
  → [calc] Starte Berechnungspipeline für X Run(s)...
  → [calc] ── Run 1/X: Run_205
  → Berechnung von sVehicle erfolgreich abgeschlossen.
  → 1 berechnete(r) Channel(s) gespeichert.
  → [calc] Abgeschlossen: X/X Run(s) erfolgreich.
[process] Abgeschlossen.
ORACLE beendet. Exit-Code: 0 (Success)
```

**Idempotenz-Sperre:** Wenn `oracle.status.json` im Run-Ordner existiert und `ExitCode == 0`:
```
[process] Run bereits erfolgreich verarbeitet (oracle.status.json). Überspringe.
```
→ Lösung: `--force` Flag hinzufügen oder `oracle.status.json` manuell löschen.

### 4.2 `merge` debuggen

**Ziel:** Alle Runs eines Riders zusammenführen.

1. Profil **"ORACLE - Merge"** auswählen
2. Breakpoint in `SessionMergeService.MergeSession()` und `SessionMergeExporter.Export()`
3. F5 drücken

**Erwarteter Ablauf:**
```
[merge] Rider Path: C:\...\AA01
[merge] Session-Merge abgeschlossen: Session="R1", Rider="AA01", X/Y Run(s), Z Channel(s).
```

**Merge-Status-Datei:** Wird in `{riderPath}\.oracle-merge\oracle.status.json` gespeichert.

### 4.3 `validate` debuggen

**Ziel:** Prüfen ob ein Run-Ordner gültige WinTAX4-Daten enthält.

1. Profil **"ORACLE - Validate"** auswählen
2. F5 drücken

**Erwarteter Ablauf:**
```
[validate] Run Path: C:\...\Run_205
  X Lap-Ordner gefunden.
  X gültige cableData.ztx Datei(en), 0 leer.
  Y Channel(s) verfügbar.
[validate] Alle Prüfungen bestanden.
ORACLE beendet. Exit-Code: 0 (Success)
```

### 4.4 `legacy` debuggen

**Ziel:** Rückwärtskompatiblen Modus testen (args[0] = Pfad, kein Command).

1. Profil **"ORACLE - Legacy"** auswählen
2. F5 drücken
3. Führt `process` + `merge` sequentiell aus

---

## 5. Breakpoint-Strategie

### Wichtigste Breakpoints nach Szenario

#### Pipeline-Einstieg & Routing
| Datei | Stelle | Warum |
|-------|--------|-------|
| `Program.cs:45` | `switch` Expression | Welcher Command wird ausgeführt? |
| `OraclePipeline.cs` | `ProcessRunAsync()` Anfang | Alle Eingabeparameter inspizieren |
| `OraclePipeline.cs` | `MergeSession()` Anfang | Rider-Pfad und Force-Flag prüfen |

#### Metadaten & Discovery
| Datei | Stelle | Warum |
|-------|--------|-------|
| `OraclePipeline.cs` | `ResolveMetadataAsync()` | HED vs. KOMPASS-Pfad verfolgen |
| `OraclePipeline.cs` | `DiscoverRuns()` | Welche Runs wurden gefunden? |
| `HedFileService.cs` | `FindAndPrepareHedFile()` | Wird .HED-Datei gefunden? |
| `KompassService.cs` | `LoadAsync()` | Wird KOMPASS-DB geladen? |

#### Berechnungen (ImportDataCalculator)
| Datei | Stelle | Warum |
|-------|--------|-------|
| `ImportDataCalculator.cs` | `RunCalculationPipelineForRuns()` | Multi-Run-Loop inspizieren |
| `ImportDataCalculator.cs` | `LoadDataSet()` nach `GetChannelsNames()` | Welche Channels sind vorhanden? |
| `ImportDataCalculator.cs` | `SVehicleCalc()` bei `missingChannels` | Fehlen `Lat_dez` / `Lon_dez`? |
| `ImportDataCalculator.cs` | `SVehicleCalc()` nach Haversine-Loop | `distance[]` Array inspizieren |
| `ImportDataCalculator.cs` | `InterpolateZerosMathNet()` bei `validIdx` | Wie viele Null-Werte wurden gefunden? |
| `ImportDataCalculator.cs` | `SaveResults()` | Was wird geschrieben? |

#### Session-Merge
| Datei | Stelle | Warum |
|-------|--------|-------|
| `SessionMergeService.cs` | `MergeSession()` | Welche Runs werden gemergt? |
| `SessionMergeService.cs` | `MergeChannelsAcrossRuns()` | Channel-Concat inspizieren |
| `SessionMergeExporter.cs` | `Export()` | Zielordner und Template-Datei |

### Conditional Breakpoints

Für Multi-Run-Debugging in `RunCalculationPipelineForRuns`:

```
Bedingung: Path.GetFileName(runPath) == "Run_206"
```
→ Stoppt nur beim gewünschten Run.

Für Channel-Debugging in `SVehicleCalc`:
```
Bedingung: missingChannels.Count > 0
```
→ Stoppt nur wenn Channels fehlen.

---

## 6. Log-Analyse

### Log-Datei finden

ORACLE schreibt automatisch in:
```
{Build-Ausgabeverzeichnis}\logs\oracle-{yyyyMMdd-HHmmss}.log
```

Typischer Pfad:
```
ORACLE\bin\x64\Debug\net8.0\logs\oracle-20250715-143022.log
```

### Log-Format

Jede Zeile hat einen Zeitstempel:
```
[14:30:22.001] ORACLE gestartet. Log-Datei: ...
[14:30:22.050] Argumente: process C:\...\Run_205
[14:30:22.100] [process] Run Path: C:\...\Run_205
[14:30:23.500] Datensatz erfolgreich geladen aus: C:\...\Run_205
[14:30:23.510] Channelliste aus Datensatz extrahiert.
[14:30:23.520] Kumulierte Distanz aus GPS wird berechnet...
```

### Timing-Analyse

Die Zeitstempel ermöglichen Performance-Analyse:
```
[14:30:22.100] [process] Run Path: ...          ← Start
[14:30:23.500] Datensatz erfolgreich geladen     ← LoadDataSet dauerte ~1.4s
[14:30:24.200] sVehicle erfolgreich              ← Berechnung dauerte ~0.7s
[14:30:24.800] Channel(s) gespeichert            ← Speichern dauerte ~0.6s
```

### Log-Datei schnell öffnen (PowerShell)

```powershell
# Letzte Log-Datei öffnen
Get-ChildItem "ORACLE\bin\x64\Debug\net8.0\logs\*.log" | Sort-Object LastWriteTime -Descending | Select-Object -First 1 | Invoke-Item

# Letzte Log-Datei im Terminal anzeigen
Get-ChildItem "ORACLE\bin\x64\Debug\net8.0\logs\*.log" | Sort-Object LastWriteTime -Descending | Select-Object -First 1 | Get-Content
```

---

## 7. oracle.status.json lesen

### Datei finden

| Command | Status-Datei Speicherort |
|---------|-------------------------|
| `process` | `{runPath}\oracle.status.json` |
| `merge` | `{riderPath}\.oracle-merge\oracle.status.json` |

### Beispiel einer erfolgreichen Verarbeitung

```json
{
  "version": "1.0",
  "path": "C:\\...\\Run_205",
  "processedAtUtc": "2025-07-15T12:30:22Z",
  "oracleVersion": "1.0.0.0",
  "steps": {
    "metadata": {
      "status": "success",
      "details": {
        "bikeName": "450 SX-F",
        "riderName": "AA01"
      }
    },
    "properties": { "status": "success" },
    "calculations": {
      "status": "success",
      "details": {
        "processedRuns": 1,
        "totalRuns": 1
      }
    }
  },
  "exitCode": 0
}
```

### Fehlgeschlagene Verarbeitung erkennen

```json
{
  "steps": {
    "metadata": { "status": "failed" }
  },
  "exitCode": 3
}
```

→ `exitCode != 0` = ORACLE wird den Run beim nächsten Mal **erneut verarbeiten** (kein Idempotenz-Block).

### Status schnell prüfen (PowerShell)

```powershell
# Status einer Run-Verarbeitung lesen
Get-Content "C:\MXTrackData\...\Run_205\oracle.status.json" | ConvertFrom-Json | Format-List

# Status aller Runs in einem Rider-Ordner
Get-ChildItem "C:\MXTrackData\...\AA01\Run_*\oracle.status.json" | ForEach-Object {
    $status = Get-Content $_.FullName | ConvertFrom-Json
    [PSCustomObject]@{
        Run = Split-Path (Split-Path $_.FullName) -Leaf
        ExitCode = $status.exitCode
        ProcessedAt = $status.processedAtUtc
        CalcStatus = $status.steps.calculations.status
    }
} | Format-Table
```

### Status-Datei löschen (für erneute Verarbeitung)

```powershell
# Einzelner Run
Remove-Item "C:\MXTrackData\...\Run_205\oracle.status.json"

# Alle Runs im Rider-Ordner
Get-ChildItem "C:\MXTrackData\...\AA01\Run_*\oracle.status.json" | Remove-Item

# Merge-Status
Remove-Item "C:\MXTrackData\...\AA01\.oracle-merge\oracle.status.json"
```

---

## 8. Häufige Fehler & Lösungen

### Exit-Code Referenz

| Code | Enum | Bedeutung | Typische Ursache |
|------|------|-----------|-----------------|
| `0` | `Success` | Alles OK | — |
| `1` | `GeneralError` | Unerwarteter Fehler | Exception in Pipeline |
| `2` | `InvalidArguments` | Argumente ungültig | Pfad fehlt, falscher Command |
| `3` | `DataNotFound` | Daten nicht gefunden | Pfad existiert nicht, kein WinTAX4-Run, keine Metadaten |
| `4` | `PartialSuccess` | Teilweise erfolgreich | Legacy: process OK aber merge fehlgeschlagen |
| `5` | `ResourceLocked` | Ressource gesperrt | `loadChannels.xlsx` in Excel geöffnet → Retry |
| `6` | `TrackLengthMissing` | Keine eindeutige Track-Länge | KOMPASS/CIRCUITS leer oder Länge doppelt belegt und zu wenige GPS-Runden (auch im Event) → später erneut bzw. in KODE pflegen |
| `7` | `InvalidData` | Ungültige Daten | Beschädigter Run-Ordner oder ungültige `KOMPASS.json` → reparieren, kein Retry |

### Fehlertabelle

| Fehlermeldung | Ursache | Lösung |
|---------------|---------|--------|
| `Datensatz konnte nicht geladen werden.` | `SetDataLocation` oder `LoadRun` gescheitert | Pfad prüfen, cableData.ztx vorhanden und > 0 Bytes? |
| `Fehlende Kanäle: Lat_dez, Lon_dez` | GPS-Channels nicht im Datensatz | Testdaten haben keine GPS-Daten → sVehicle wird übersprungen (kein Fehler) |
| `Keine gültigen Runs gefunden` | `IMarelliDataBrowse.FindRuns()` liefert nichts | Pfad ist kein gültiger WinTAX4-Run, Ordnerstruktur prüfen |
| `Metadaten konnten nicht ermittelt werden.` | Kein .HED-File und KOMPASS nicht erreichbar | .HED-Datei im Run- oder Rider-Ordner ablegen, oder `DatabaseDirectory` in `oracle-config.json` prüfen |
| `Datenbankverzeichnis nicht gefunden` | KOMPASS-Pfad in Config falsch | `oracle-config.json` → `DatabaseDirectory` anpassen |
| `Kein gültiger WinTAX4-Run` | `IsRunLocation()` gibt false zurück | Ordner hat keine `Lap_*`-Unterordner mit cableData.ztx |
| `Run bereits erfolgreich verarbeitet` | `oracle.status.json` blockiert | `--force` verwenden oder Status-Datei löschen |
| `System.NullReferenceException` in KtmDataHandler | `new MarelliDataAccess()` statt Factory | **Immer** `KtmDataHandlerFactory.CreateReader()` / `.CreateWriter()` verwenden |
| `Autofac.Core.DependencyResolutionException` | DI-Container kann Typ nicht auflösen | KtmDataHandler.dll und KtmSharedUtils.dll Version prüfen |
| `Channel-Daten sind nicht im erwarteten Format.` | `SignalData.Data` ist nicht `double[]` | Channel hat unerwarteten Datentyp → im Debugger inspizieren |

### NullReferenceException in KtmDataHandler

Die häufigste Fehlerquelle. **NIEMALS** direkt instanziieren:

```csharp
// ❌ FALSCH — alle Parameter sind optional (default null) → NullRef
var reader = new MarelliDataAccess();

// ✅ RICHTIG — Autofac löst alle 6 Abhängigkeiten auf
var reader = KtmDataHandlerFactory.CreateReader();
```

Auch `new MarelliDataBrowse()` ist sicher — diese Klasse hat keine DI-Abhängigkeiten.

---

## 9. KtmDataHandler / Autofac Debugging

### Autofac-Container inspizieren

Breakpoint in `KtmDataHandlerFactory.GetContainer()` (Zeile `_container = builder.Build()`):

```
Watch: _container.ComponentRegistry.Registrations.Count
```
→ Zeigt wie viele Typen registriert sind.

### Reader/Writer API-Ablauf

```csharp
// 1. Instanz erzeugen (via Autofac)
var reader = KtmDataHandlerFactory.CreateReader();

// 2. Pfad setzen (MUSS string[] sein!)
reader.SetDataLocation(new string[] { runPath });

// 3. Run laden
reader.LoadRun();

// 4. Channels abfragen
string[] channels = reader.GetChannelsNames();

// 5. Daten lesen
IDataset dataset = reader.GetChannels(new[] { "Lat_dez", "Lon_dez" });
ITimeseries signal = dataset.GetElement("Lat_dez");

// 6. Daten inspizieren
double[] values = signal.SignalData.Data as double[];
double[] time = signal.SignalData.Time as double[];

// 7. Aufräumen (MUSS aufgerufen werden!)
reader.ReleaseObjects();
```

### Häufige KtmDataHandler-Fallen

| Problem | Symptom | Lösung |
|---------|---------|--------|
| `SetDataLocation(string)` | Kompiliert nicht | Muss `string[]` sein: `new string[] { path }` |
| `ReleaseObjects()` vergessen | Speicherleck / Datei-Lock | Immer in `finally` Block |
| Reader nach `ReleaseObjects()` wiederverwenden | Undefined Behavior | Neue Instanz via Factory erstellen |
| `GetChannelsNames()` vor `LoadRun()` | Leeres Array oder null | Erst `LoadRun()`, dann Channels abfragen |

---

## 10. ImportDataCalculator Debugging

### Multi-Run Ablauf verfolgen

```
RunCalculationPipelineForRuns(validRuns)    ← Einstieg: Liste aller Runs
  │
  └── for each run:
        RunCalculationPipeline(runPath)     ← Pro Run: frische Reader/Writer
          ├── LoadDataSet()                 ← Channels laden
          ├── SVehicleCalc()                ← GPS → kumulierte Distanz
          │     ├── Channel-Verfügbarkeit prüfen
          │     ├── InterpolateZerosMathNet()  ← Null-Werte interpolieren
          │     ├── Haversine-Berechnung
          │     └── resultDataset.Add()
          └── SaveResults()                 ← Ergebnis in cableData.ztx schreiben
```

### Daten im Debugger inspizieren

Breakpoint in `SVehicleCalc()` nach dem Laden der Channel-Daten:

```
Watch-Fenster:
  latitudes.Length          → Anzahl Datenpunkte
  latitudes[0..5]          → Erste GPS-Werte (sollte ≠ 0 sein)
  longitudes[0..5]         → Erste GPS-Werte
  time[0]                  → Startzeitpunkt
  time[^1]                 → Endzeitpunkt (C# 12 Range)
```

### Interpolation debuggen

Breakpoint in `InterpolateZerosMathNet()`:

```
Watch-Fenster:
  validIdx.Count           → Anzahl gültiger (nicht-null) Werte
  n - validIdx.Count       → Anzahl zu interpolierender Nullen
  first                    → Index des ersten gültigen Werts
  last                     → Index des letzten gültigen Werts
```

**Warnung:** Wenn `validIdx.Count == 0` → Channel enthält nur Nullen, keine Interpolation möglich.

### Haversine-Ergebnis prüfen

Breakpoint nach dem Haversine-Loop:

```
Watch-Fenster:
  distance[^1]             → Gesamtdistanz in Metern
  segmentDistances.Max()   → Größter Einzelschritt (plausibel?)
  segmentDistances.Average() → Mittlerer Schritt
```

**Plausibilitätsprüfung:**
- Rundstrecke ~1.5–5 km pro Lap
- Segmentdistanzen typisch 0.1–50 m (je nach Abtastrate)
- Wenn `segmentDistances.Max() > 1000` → wahrscheinlich GPS-Sprung (Datenfehler)

### SaveResults debuggen

Breakpoint in `SaveResults()`:

```
Watch-Fenster:
  dataSet.NumElements()    → Anzahl berechneter Channels
  currentRunPath           → Ziel-Run-Pfad
```

---

## 11. Session-Merge Debugging

### Merge-Ablauf verfolgen

```
OraclePipeline.MergeSession(riderPath)
  │
  ├── SessionMergeService.MergeSession(riderPath)
  │     ├── DiscoverRuns()         → Runs im Rider-Ordner finden
  │     ├── LoadSingleRun()        → Pro Run: Channels laden + Zeitversatz prüfen
  │     └── MergeChannelsAcrossRuns()  → Channels per ITimeseries.Concat() zusammenführen
  │
  ├── MergedSessionData inspizieren:
  │     ├── .MergedRunCount        → Wie viele Runs erfolgreich?
  │     ├── .TotalRunsDiscovered   → Wie viele Runs entdeckt?
  │     ├── .ChannelNames          → Welche Channels im Ergebnis?
  │     └── .HasData               → Gibt es überhaupt Daten?
  │
  └── SessionMergeExporter.Export(mergedSession, riderPath)
        ├── CreateFolderStructure()   → _MergedSession/Run_Merged/Lap_001/
        ├── CopyTemplateFile()        → cableData.ztx aus erstem gültigen Lap kopieren
        └── WriteMergedData()         → AddImportDataToRun()
```

### Merge-Ergebnis prüfen

```powershell
# Prüfen ob Merge-Ordner erstellt wurde
Test-Path "C:\MXTrackData\...\AA01\_MergedSession\Run_Merged\Lap_001\cableData.ztx"

# Dateigröße des Merge-Ergebnisses
Get-Item "C:\MXTrackData\...\AA01\_MergedSession\Run_Merged\Lap_001\cableData.ztx" | Select-Object Name, Length
```

### Merge-Validierung

Häufige Probleme:
- **0 Runs gemergt:** Kein Run im Rider-Ordner hat ladbare cableData.ztx
- **Channels fehlen im Merge:** Nicht alle Runs haben dieselben Channels
- **Export-Ordner existiert bereits:** Vorheriger Merge wurde nicht gelöscht

---

## 12. SENTINEL Debugging

SENTINEL ist ein separates Projekt (reines .NET 8, keine KtmDataHandler-Abhängigkeit).

### Konfiguration

`sentinel-config.json`:
```json
{
  "WatchPaths": ["C:\\MXTrackData\\..."],
  "PollingIntervalSeconds": 30,
  "StabilityThreshold": 3,
  "MinimumFileSizeBytes": 1024
}
```

### Debug-Ablauf

1. SENTINEL-Projekt als Startprojekt setzen
2. F5 drücken → Polling-Loop startet
3. Breakpoint in `RunWatcher.RunScanCycle()`
4. Ctrl+C im Konsolenfenster → Sauberer Shutdown

### SENTINEL Breakpoints

| Datei | Stelle | Warum |
|-------|--------|-------|
| `RunWatcher.cs` | `DiscoverPendingRuns()` | Welche Runs ohne download.done existieren? |
| `RunSnapshot.cs` | `Create()` | Aktueller Snapshot: Dateien, Größen |
| `StabilityTracker.cs` | `IsStable()` | Stabilitätsschwelle erreicht? |
| `DownloadDoneWriter.cs` | Hauptmethode | download.done XML inspizieren |

---

## 13. Kommandozeilen-Debugging (ohne VS)

### Direkter Aufruf

```powershell
cd "ORACLE\bin\x64\Debug\net8.0"

# Process
.\ORACLE.exe process "C:\MXTrackData\...\Run_205"

# Process mit Force
.\ORACLE.exe process "C:\MXTrackData\...\Run_205" --force

# Merge
.\ORACLE.exe merge "C:\MXTrackData\...\AA01"

# Validate
.\ORACLE.exe validate "C:\MXTrackData\...\Run_205"

# Legacy-Modus (wie KRAKEN aufruft)
.\ORACLE.exe "C:\MXTrackData\...\Run_205"

# Exit-Code prüfen
.\ORACLE.exe process "C:\MXTrackData\...\Run_205"; Write-Host "Exit-Code: $LASTEXITCODE"
```

### KRAKEN-Aufruf simulieren

KRAKEN übergibt `%RUN_PATH%` als Argument:

```powershell
# KRAKEN-Simulation: validate → process → merge
$runPath = "C:\MXTrackData\...\Run_205"
$riderPath = Split-Path $runPath

.\ORACLE.exe validate $runPath
if ($LASTEXITCODE -eq 0) {
    .\ORACLE.exe process $runPath
    if ($LASTEXITCODE -eq 0) {
        .\ORACLE.exe merge $riderPath
    }
}
Write-Host "Finale: $LASTEXITCODE"
```

### Log-Datei live verfolgen

```powershell
# Log-Datei in Echtzeit anzeigen (wie tail -f)
$logDir = "ORACLE\bin\x64\Debug\net8.0\logs"
$latest = Get-ChildItem "$logDir\*.log" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
Get-Content $latest.FullName -Wait
```

---

## 14. Checkliste bei neuen Berechnungen

Wenn eine neue Berechnungsfunktion in `ImportDataCalculator` hinzugefügt wird:

### 1. Methode erstellen

```csharp
private void NeueBerechnung()
{
    Console.WriteLine("Neue Berechnung wird ausgeführt...");

    // Notwendige Channels definieren
    var necessaryChannels = new List<string> { "ChannelA", "ChannelB" };

    // Channel-Verfügbarkeit prüfen
    var missingChannels = necessaryChannels.Except(channelNames).ToList();
    if (missingChannels.Any())
    {
        Console.WriteLine($"Fehlende Kanäle: {string.Join(", ", missingChannels)}. Berechnung wird abgebrochen.");
        return;  // Nicht werfen — nächste Berechnung soll weiterlaufen
    }

    // Daten laden
    IDataset processingDataset = ztxAccessRead.GetChannels(necessaryChannels.ToArray());

    // ... Berechnung ...

    // Ergebnis als Timeseries hinzufügen
    var result = new KtmDataHandler.Models.Channels.Timeseries();
    result.SignalData.Name = "NeuerChannel";
    result.SignalData.Unit = "km/h";
    result.SignalData.TimeData = (time, values);
    resultDataset.Add(result);

    Console.WriteLine("Neue Berechnung erfolgreich abgeschlossen.");
}
```

### 2. In Pipeline einbinden

```csharp
// In RunCalculationPipeline(), nach SVehicleCalc():
SVehicleCalc();
NeueBerechnung();    // ← Hier hinzufügen
```

### 3. Debug-Checkliste

- [ ] Breakpoint in der neuen Methode gesetzt
- [ ] `channelNames` im Debugger geprüft — sind die benötigten Channels vorhanden?
- [ ] `SignalData.Data as double[]` prüfen — gibt null zurück wenn falscher Typ
- [ ] `resultDataset.NumElements()` nach der Berechnung prüfen — wurde der Channel hinzugefügt?
- [ ] Log-Datei prüfen — werden alle `Console.WriteLine` korrekt ausgegeben?
- [ ] `oracle.status.json` löschen oder `--force` verwenden für erneuten Test
- [ ] Multi-Run testen: Funktioniert die Berechnung für mehrere Runs hintereinander?

---

## Anhang: Nützliche Watch-Ausdrücke

| Ausdruck | Kontext | Zeigt |
|----------|---------|-------|
| `args` | `Program.Main` | Alle Kommandozeilen-Argumente |
| `runPath` | überall | Aktueller Pfad |
| `channelNames.Count` | `ImportDataCalculator` | Anzahl verfügbarer Channels |
| `channelNames` | `ImportDataCalculator` | Alle Channel-Namen (für Find-In-List) |
| `resultDataset.NumElements()` | `ImportDataCalculator` | Berechnete Channels |
| `metadata` | `ProcessRunAsync` | Bike, Track, Rider |
| `validRuns` | `ProcessRunAsync` | Entdeckte Run-Pfade |
| `mergedSession.ChannelNames` | `MergeSession` | Channels im Merge-Ergebnis |
| `status.Steps` | vor `Save()` | Aktueller Verarbeitungsstatus |
| `_browse` | `OraclePipeline` | Injizierte IMarelliDataBrowse |
| `_config` | `OraclePipeline` | Geladene Konfiguration |
