# Session-Merge Workflow & Debugging-Leitfaden

## Inhaltsverzeichnis

1. [Übersicht der Änderungen](#1-übersicht-der-änderungen)
2. [Architektur & Dateistruktur](#2-architektur--dateistruktur)
3. [WinTAX4-Ordnerstruktur](#3-wintax4-ordnerstruktur)
4. [Detaillierter Workflow](#4-detaillierter-workflow)
5. [Datenfluss-Diagramm](#5-datenfluss-diagramm)
6. [API-Aufrufkette](#6-api-aufrufkette)
7. [Edge-Cases & Fehlerbehandlung](#7-edge-cases--fehlerbehandlung)
8. [Debugging-Leitfaden](#8-debugging-leitfaden)
9. [Breakpoint-Checkliste](#9-breakpoint-checkliste)
10. [Konsolen-Ausgaben verstehen](#10-konsolen-ausgaben-verstehen)

---

## 1. Übersicht der Änderungen

### Neue Dateien

| Datei | Verantwortung |
|-------|---------------|
| `SessionMergeService.cs` | Kern-Service: Orchestrierung von Discovery → Loading → Merge |
| `SessionMergeValidator.cs` | Statische Validierungen für alle Eingangs- und Zwischenwerte |
| `MergedSessionData.cs` | Datenmodelle für das Merge-Ergebnis (`MergedSessionData`, `MergedRunInfo`, `RunMergeStatus`) |

### Geänderte Dateien

| Datei | Änderung |
|-------|----------|
| `Program.cs` (Zeilen 68–83) | Aufruf des `SessionMergeService` nach der Berechnungspipeline |

### Klassenbeziehungen

```
Program.Main()
  │
  ├── [bestehend] ImportDataCalculator.RunCalculationPipeline(runPath)
  │
  └── [NEU] SessionMergeService.MergeSession(riderPath)
                │
                ├── SessionMergeValidator  (statische Prüfungen)
                ├── KtmDataHandlerFactory  (Reader-Erzeugung pro Run)
                └── MergedSessionData      (Ergebnis-Datenmodell)
```

---

## 2. Architektur & Dateistruktur

### Design-Prinzipien

- **Service Pattern**: `SessionMergeService` kapselt die gesamte Merge-Logik
- **Validator Pattern**: `SessionMergeValidator` trennt Validierung von Geschäftslogik
- **Fail-Forward**: Ein fehlerhafter Run stoppt NICHT den Merge – er wird übersprungen und als `MergedRunInfo` mit Fehlerstatus protokolliert
- **Resource Safety**: Jeder Reader wird in `try/finally` mit `ReleaseObjects()` freigegeben
- **Defensive Cloning**: Erste Timeseries wird per `Clone()` kopiert, da `Concat()` in-place modifiziert

### Abhängigkeitsrichtung

```
Program.cs
    ↓
SessionMergeService
    ↓                    ↓                      ↓
SessionMergeValidator    KtmDataHandlerFactory   MergedSessionData
    ↓                    ↓
KtmDataHandler.dll APIs (IMarelliDataBrowse, IMarelliDataAccessRead, IDataset, ITimeseries)
```

---

## 3. WinTAX4-Ordnerstruktur

Das Verständnis der Ordnerhierarchie ist zentral für den Merge:

```
📁 RT12_ESP-Almonte/          ← Event
  📁 R1/                      ← Session (z.B. Race 1, FP1, QP)
    📁 AA01/                   ← Rider/Device (AA = Rider-Kürzel, 01 = Bike-Nr.)
      📁 Run_201/              ← Run (LEER – 0 Laps → Status: NoLaps)
      📁 Run_202/              ← Run (1 Lap → wird geladen)
      │  📁 Lap_001/
      │     📄 cableData.ztx   ← Telemetriedaten (Binärformat)
      📁 Run_203/              ← Run (1 Lap → wird geladen)
      │  📁 Lap_001/
      │     📄 cableData.ztx
      📁 Run_204/              ← Run (1 Lap → wird geladen)
      │  📁 Lap_001/
      │     📄 cableData.ztx
      📁 Run_205/              ← Run (21 Laps → wird geladen)
      │  📁 Lap_001/ ... Lap_021/
      │     📄 cableData.ztx
      📁 Run_206/              ← Run (LEER – 0 Laps → Status: NoLaps)
```

### Pfad-Hierarchie in der API

| API-Methode | Navigiert zu | Beispiel |
|-------------|-------------|----------|
| `GetDeviceName(runPath)` | Parent-Ordner des Runs | `AA01` |
| `GetSessionName(runPath)` | Grandparent-Ordner | `R1` |
| `GetEventName(runPath)` | Great-Grandparent | `RT12_ESP-Almonte` |
| `FindRuns(path)` | Sucht rekursiv nach `cableData.ztx` | Alle Run-Pfade |
| `IsRunLocation(path)` | Prüft: `Run_*`-Prefix + `Lap_*` + `cableData.ztx` | `true`/`false` |

---

## 4. Detaillierter Workflow

### Schritt-für-Schritt-Ablauf

```
┌─────────────────────────────────────────────────────────────────┐
│                    Program.Main()                               │
│                                                                 │
│  args[0] = "...\RT12_ESP-Almonte\R1\AA01\Run_205"               │
│                          │                                      │
│  [bestehende Pipeline]   │                                      │
│  ImportDataCalculator ───┘                                      │
│                                                                 │
│  riderPath = Path.Combine(runPath, "..") → "...\R1\AA01"        │
│                          │                                      │
│  SessionMergeService ────┘                                      │
│  .MergeSession(riderPath)                                       │
└──────────────┬──────────────────────────────────────────────────┘
               │
               ▼
```

### Phase 1 — Eingabe-Validierung (`SessionMergeValidator.ValidatePath`)

```
riderPath: "...\R1\AA01"
    │
    ├── Ist null/leer?      → ArgumentNullException
    ├── Verzeichnis existiert? → DirectoryNotFoundException
    └── ✅ Weiter
```

### Phase 2 — Run-Discovery (`IMarelliDataBrowse.FindRuns`)

```
FindRuns("...\R1\AA01")
    │
    ├── Sucht rekursiv nach "cableData.ztx"
    ├── Geht 2 Verzeichnisebenen hoch pro Fund → Run-Pfad
    │
    └── Ergebnis (unsortiert):
        ├── "...\Run_202"    (hat Lap_001/cableData.ztx)
        ├── "...\Run_203"    (hat Lap_001/cableData.ztx)
        ├── "...\Run_204"    (hat Lap_001/cableData.ztx)
        └── "...\Run_205"    (hat Lap_001..021/cableData.ztx)

⚠️ WICHTIG: Run_201 und Run_206 werden von FindRuns NICHT gefunden,
   weil sie keine cableData.ztx enthalten.
   Sie werden aber trotzdem nicht im Ergebnis fehlen — sie waren
   nie in der Discovery.
```

### Phase 3 — Sortierung

```
Sortierung nach Ordnername (StringComparer.OrdinalIgnoreCase):
    Run_202 → Run_203 → Run_204 → Run_205
    
Zweck: Chronologische Konsistenz für Concat (zeitliche Reihenfolge)
```

### Phase 4 — Metadaten-Extraktion (`ExtractSessionMetadata`)

```
Für den ERSTEN Run mit IsRunLocation = true:
    │
    ├── GetSessionName("...\Run_202") → "R1"       (Grandparent-Ordner)
    └── GetDeviceName("...\Run_202")  → "AA01"     (Parent-Ordner)

Fallback (wenn API fehlschlägt):
    ├── deviceFolder  = Path.GetFileName(Parent)     → "AA01"
    └── sessionFolder = Path.GetFileName(Grandparent) → "R1"
```

### Phase 5 — Run-Loading (`LoadSingleRun` × N)

Für **jeden** Run wird ein eigener Lade-Zyklus durchlaufen:

```
Für Run_202:
    │
    ├── 1. ValidateRunPath → IsRunLocation? ✅
    │
    ├── 2. reader = KtmDataHandlerFactory.CreateReader()
    │      (erzeugt neuen IMarelliDataAccessRead via Autofac DI)
    │
    ├── 3. reader.SetDataLocation(["...\Run_202"])
    │      (Setzt den Datenpfad im Reader)
    │
    ├── 4. reader.LoadRun()
    │      → Rückgabe: int (≥0 = Erfolg, -1 = Fehler)
    │      → Lädt ALLE Laps im Run automatisch
    │
    ├── 5. reader.GetChannelsNames()
    │      → string[] {"GPS_Lat", "GPS_Lon", "Speed", "RPM", ...}
    │      → Werden in allChannelNames (HashSet) eingefügt
    │
    ├── 6. reader.GetChannels(channelNames)
    │      → IDataset mit allen Channels als ITimeseries
    │      → Jede Timeseries enthält: Name, Data (double[]), Time (double[])
    │
    ├── 7. reader.GetLastSampleTime()
    │      → double, z.B. 125.43 Sekunden
    │
    ├── 8. Lap-Count zählen (Directory.GetDirectories("Lap_*"))
    │
    └── 9. reader.ReleaseObjects()  ← IMMER (finally-Block)

Ergebnis pro Run:
    ├── MergedRunInfo { Status=Success, LapCount=1, ChannelNames=[...], LastSampleTime=125.43 }
    └── IDataset (die geladenen Channel-Daten)
```

### Phase 6 — Channel-Merge (`MergeChannelsAcrossRuns`)

Dies ist der **Kern-Algorithmus** — die eigentliche Zusammenführung:

```
Eingabe:
    runDatasets = [(Run_202, Dataset₂), (Run_203, Dataset₃), (Run_204, Dataset₄), (Run_205, Dataset₅)]
    channelNames = Union aller Channels = ["GPS_Lat", "GPS_Lon", "Speed", "RPM", ...]

Algorithmus (Beispiel für Channel "Speed"):

    mergedChannels = {}  (leer)

    Run_202: dataset₂.GetElement("Speed") → timeseries₂
             mergedChannels hat "Speed" noch nicht
             → mergedChannels["Speed"] = timeseries₂.Clone()    ← DEFENSIVE KOPIE
                                                                    (weil Concat in-place!)

    Run_203: dataset₃.GetElement("Speed") → timeseries₃
             mergedChannels hat "Speed" bereits → accumulated
             → accumulated.Concat(timeseries₃)                  ← IN-PLACE APPEND
               [t₂_data...] + [t₃_data...] = [t₂_data...|t₃_data...]

    Run_204: dataset₄.GetElement("Speed") → timeseries₄
             → accumulated.Concat(timeseries₄)
               [t₂...|t₃...] + [t₄...] = [t₂...|t₃...|t₄...]

    Run_205: dataset₅.GetElement("Speed") → timeseries₅
             → accumulated.Concat(timeseries₅)
               [t₂...|t₃...|t₄...] + [t₅...] = [t₂...|t₃...|t₄...|t₅...]

    → mergedDataset.Add(mergedChannels["Speed"])

Dies wird für JEDEN Channel in channelNames wiederholt.
```

### Visuelles Concat-Schema

```
                    Time →
Run_202  ████████░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
Run_203  ░░░░░░░░████████░░░░░░░░░░░░░░░░░░░░░░
Run_204  ░░░░░░░░░░░░░░░░████████░░░░░░░░░░░░░░
Run_205  ░░░░░░░░░░░░░░░░░░░░░░░░████████████████████████

Merged   ████████████████████████████████████████████████████
         ↑ Clone  ↑ Concat  ↑ Concat  ↑ Concat (21 Laps)
```

### Phase 7 — Ergebnis-Zusammenbau

```
return new MergedSessionData
{
    SessionName       = "R1",
    RiderName         = "AA01",
    MergedDataset     = mergedDataset,        ← Konsolidiertes Dataset
    Runs              = [RunInfo₂₀₂, RunInfo₂₀₃, RunInfo₂₀₄, RunInfo₂₀₅],
    ChannelNames      = ["GPS_Lat", "GPS_Lon", "Speed", ...],
    TotalRunsDiscovered = 4,                  ← Nur die von FindRuns gefundenen
    MergedRunCount    = 4,                    ← Alle 4 erfolgreich
    HasData           = true                  ← MergedRunCount > 0
};
```

---

## 5. Datenfluss-Diagramm

```
Program.Main(args[0])
    │
    │  runPath = args[0]                         "...\R1\AA01\Run_205"
    │  riderPath = runPath + "\.."               "...\R1\AA01"
    │
    ▼
SessionMergeService(ztxBrowse)
    │
    │  Konstruktor: _browse = ztxBrowse (MarelliDataBrowse)
    │
    ▼
MergeSession(riderPath)
    │
    ├─► ValidatePath(riderPath)                  Pfad existiert? ✅
    │
    ├─► _browse.FindRuns(riderPath)              Rekursive Suche nach cableData.ztx
    │   └── string[] { Run_202, Run_203, Run_204, Run_205 }
    │
    ├─► .OrderBy(Ordnername)                     Sortierung: chronologisch
    │
    ├─► ExtractSessionMetadata()                 "R1", "AA01"
    │
    ├─► LoadSingleRun(Run_202, allChannelNames)
    │   ├── CreateReader()
    │   ├── SetDataLocation → LoadRun → GetChannelsNames → GetChannels
    │   ├── ReleaseObjects()
    │   └── return (MergedRunInfo, IDataset)
    │
    ├─► LoadSingleRun(Run_203, allChannelNames)  (gleicher Ablauf)
    ├─► LoadSingleRun(Run_204, allChannelNames)  (gleicher Ablauf)
    ├─► LoadSingleRun(Run_205, allChannelNames)  (gleicher Ablauf)
    │
    ├─► MergeChannelsAcrossRuns(runDatasets, channelNames)
    │   ├── Pro Channel: Clone(erster) + Concat(folgende)
    │   └── return IDataset (alle Channels verkettet)
    │
    └─► return MergedSessionData
            │
            ▼
        Program.Main() gibt Zusammenfassung aus
```

---

## 6. API-Aufrufkette

### Pro Run aufgerufene KtmDataHandler-APIs (in Reihenfolge)

| # | API-Aufruf | Rückgabe | Zweck |
|---|-----------|----------|-------|
| 1 | `KtmDataHandlerFactory.CreateReader()` | `IMarelliDataAccessRead` | Neuen Reader via Autofac DI |
| 2 | `reader.SetDataLocation(string[] { runPath })` | `void` | Datenpfad setzen |
| 3 | `reader.LoadRun()` | `int` (≥0 = OK, -1 = Fehler) | Alle Laps laden |
| 4 | `reader.GetChannelsNames()` | `string[]` | Verfügbare Channel-Namen |
| 5 | `reader.GetChannels(channelNames)` | `IDataset` | Alle Channels als Timeseries |
| 6 | `reader.GetLastSampleTime()` | `double` (Sekunden) | Letzte Sample-Zeit |
| 7 | `reader.ReleaseObjects()` | `void` | Ressourcen freigeben (IMMER) |

### Merge-spezifische APIs

| API | Typ | Verhalten |
|-----|-----|-----------|
| `ITimeseries.Clone()` | `object` (→ Cast zu `ITimeseries`) | Erzeugt tiefe Kopie |
| `ITimeseries.Concat(ITimeseries)` | `void` | **In-place!** Hängt Daten an das Ende an. Kein Time-Check. |
| `IDataset.GetElement(string)` | `ITimeseries` | Channel per Name aus Dataset holen |
| `IDataset.Add(ITimeseries)` | `void` | Channel zum Dataset hinzufügen |
| `IDataset.NumElements()` | `int` | Anzahl Channels im Dataset |

---

## 7. Edge-Cases & Fehlerbehandlung

### Übersicht der Status-Werte

| `RunMergeStatus` | Wann | Reaktion |
|-----------------|------|----------|
| `Success` | Run erfolgreich geladen + Channels vorhanden | In Merge einbezogen |
| `NoLaps` | `IsRunLocation = false`, aber Verzeichnis existiert | Übersprungen (häufig bei leeren Runs) |
| `NoChannels` | `GetChannelsNames()` gibt null/leer zurück ODER Dataset leer | Übersprungen |
| `LoadError` | `LoadRun()` gibt -1 zurück ODER Exception | Übersprungen, Fehler protokolliert |
| `InvalidPath` | Verzeichnis existiert nicht | Übersprungen |

### Szenario-Tabelle

| Szenario | Ergebnis |
|----------|----------|
| 0 Runs gefunden | `HasData = false`, leeres `MergedDataset` |
| 1 Run gefunden | Channels werden geklont (kein Concat), `MergedRunCount = 1` |
| Run ohne Laps (z.B. Run_201) | Von `FindRuns` gar nicht entdeckt (kein `cableData.ztx`) |
| Channel nur in 2 von 4 Runs | Wird nur aus diesen 2 Runs verkettet — kein Fehler |
| `LoadRun()` wirft Exception | `try/catch` → `LoadError`, Run übersprungen, Merge geht weiter |
| `GetChannels()` gibt null zurück | `ValidateDataset` schlägt fehl → `NoChannels` |
| Timeseries ist leer | `ValidateTimeseries` prüft `IsNullOrEmpty()` → übersprungen |
| `Concat()` wirft Exception | Wird vom `try/catch` in `LoadSingleRun` NICHT gefangen (passiert in `MergeChannelsAcrossRuns`) — hier wird auf API-Stabilität vertraut |

---

## 8. Debugging-Leitfaden

### Voraussetzungen

1. **Visual Studio 2022/2026** mit Debug-Konfiguration
2. **Testdaten**: Ein WinTAX4-Event-Ordner mit bekannter Struktur (z.B. `RT12_ESP-Almonte`)
3. **Launch-Profil**: `ORACLE.exe` mit Argument `<RunPath>`
   - Beispiel: `"C:\Testdaten\RT12_ESP-Almonte\R1\AA01\Run_205"`

### Debug-Konfiguration setzen

In **Visual Studio** → Projekt-Properties → Debug → Kommandozeilen-Argumente:

```
"C:\Testdaten\RT12_ESP-Almonte\R1\AA01\Run_205"
```

### Strategie: 3 Phasen debuggen

#### Phase A — Discovery verifizieren

**Ziel**: Werden die richtigen Runs gefunden?

1. **Breakpoint** auf `SessionMergeService.cs`, Zeile 103:
   ```csharp
   var discoveredRunPaths = _browse.FindRuns(riderPath);
   ```
2. **Nach Step-Over**: `discoveredRunPaths` im Watch-Fenster inspizieren
   - Erwartung: Array mit Pfaden zu Run_202, Run_203, Run_204, Run_205
   - Run_201 und Run_206 sollten NICHT enthalten sein (keine `cableData.ztx`)
3. **Breakpoint** auf Zeile 111-113 (Sortierung):
   ```csharp
   var sortedRunPaths = discoveredRunPaths
       .OrderBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase)
       .ToArray();
   ```
   - Prüfe: Sind die Runs alphabetisch/chronologisch sortiert?

#### Phase B — Einzelnen Run laden

**Ziel**: Werden die Daten korrekt aus `cableData.ztx` geladen?

1. **Breakpoint** auf `SessionMergeService.cs`, Zeile 261:
   ```csharp
   IMarelliDataAccessRead reader = KtmDataHandlerFactory.CreateReader();
   ```
2. **Step-Into** die folgenden Zeilen und prüfe:

   | Zeile | Prüfpunkt | Watch-Ausdruck |
   |-------|-----------|----------------|
   | 264 | `SetDataLocation` erfolgreich? | Keine Exception |
   | 265 | `LoadRun()` Rückgabewert | `loadResult` → muss ≥ 0 sein |
   | 279 | Channel-Namen | `channelNames` → z.B. `{"GPS_Lat", "GPS_Lon", ...}` |
   | 279 | Anzahl Channels | `channelNames.Length` |
   | 299 | Dataset geladen? | `dataset.NumElements()` → muss > 0 sein |
   | 314 | Letzte Sample-Zeit | `lastSampleTime` → z.B. 125.43 |

3. **Wichtig**: Prüfe im Watch-Fenster für ein beliebiges Channel:
   ```
   dataset.GetElement("GPS_Lat").SignalData.Data.Length    → Anzahl Samples
   dataset.GetElement("GPS_Lat").SignalData.Time.Length    → Anzahl Zeitstempel
   dataset.GetElement("GPS_Lat").SignalData.Time[0]       → Erster Zeitstempel
   ```

#### Phase C — Merge verifizieren

**Ziel**: Sind die verketteten Timeseries korrekt?

1. **Breakpoint** auf `SessionMergeService.cs`, Zeile 435 (Clone):
   ```csharp
   mergedChannels[channelName] = (ITimeseries)timeseries.Clone();
   ```
   - Prüfe: Wird nur beim ERSTEN Run pro Channel erreicht?

2. **Breakpoint** auf Zeile 442 (Concat):
   ```csharp
   accumulated.Concat(timeseries);
   ```
   - **Vor Concat** — Watch-Ausdruck:
     ```
     accumulated.SignalData.Data.Length     → Bisherige Datenpunkte
     timeseries.SignalData.Data.Length      → Neue Datenpunkte (werden angehängt)
     ```
   - **Nach Step-Over** — Prüfe:
     ```
     accumulated.SignalData.Data.Length     → Muss = alte + neue sein
     ```

3. **Breakpoint** auf Zeile 450 (finales Add):
   ```csharp
   mergedDataset.Add(kvp.Value);
   ```
   - Prüfe: `kvp.Key` = Channel-Name, `kvp.Value.SignalData.Data.Length` = Gesamtlänge

### Debug-Checkliste: Erwartete Werte (Beispiel RT12)

```
discoveredRunPaths.Length     = 4        (Run_202..205)
sortedRunPaths.Length         = 4
sessionName                  = "R1"
riderName                    = "AA01"

Run_202: LapCount=1,  channelNames.Length=N,  lastSampleTime > 0
Run_203: LapCount=1,  channelNames.Length=N,  lastSampleTime > 0
Run_204: LapCount=1,  channelNames.Length=N,  lastSampleTime > 0
Run_205: LapCount=21, channelNames.Length=N,  lastSampleTime > 0

mergedCount                  = 4
channelNameList.Count        = N  (Union aller Channels)
mergedDataset.NumElements()  = N

mergedSession.HasData        = true
```

---

## 9. Breakpoint-Checkliste

### Empfohlene Breakpoints (nach Priorität)

| Priorität | Datei | Zeile | Zweck |
|-----------|-------|-------|-------|
| 🔴 Hoch | `SessionMergeService.cs` | 103 | `FindRuns` — Discovery-Ergebnis prüfen |
| 🔴 Hoch | `SessionMergeService.cs` | 265 | `LoadRun()` — Ladevorgang pro Run |
| 🔴 Hoch | `SessionMergeService.cs` | 299 | `GetChannels()` — Dataset pro Run |
| 🟡 Mittel | `SessionMergeService.cs` | 435 | `Clone()` — Erstes Auftreten eines Channels |
| 🟡 Mittel | `SessionMergeService.cs` | 442 | `Concat()` — Verkettung prüfen |
| 🟡 Mittel | `SessionMergeService.cs` | 161 | Finales `MergedSessionData` inspizieren |
| 🟢 Niedrig | `SessionMergeValidator.cs` | 103 | `IsRunLocation` — Run-Validierung |
| 🟢 Niedrig | `SessionMergeValidator.cs` | 147 | `IsNullOrEmpty` — Timeseries-Validierung |
| 🟢 Niedrig | `Program.cs` | 71 | `MergeSession(riderPath)` — Einstiegspunkt |

### Conditional Breakpoints (für große Datensätze)

Wenn viele Runs/Channels vorhanden sind, nutze **bedingte Breakpoints**:

- **Nur bei bestimmtem Run stoppen** (Zeile 240):
  ```
  Bedingung: runFolderName == "Run_205"
  ```

- **Nur bei bestimmtem Channel stoppen** (Zeile 420):
  ```
  Bedingung: channelName == "GPS_Lat"
  ```

- **Nur bei Fehlern stoppen** (Zeile 335):
  ```
  Bedingung: true    (immer, da catch-Block nur bei Fehlern erreicht wird)
  ```

---

## 10. Konsolen-Ausgaben verstehen

### Erfolgreicher Ablauf

```
Run Path: C:\Testdaten\RT12_ESP-Almonte\R1\AA01\Run_205
Rider Path: C:\Testdaten\RT12_ESP-Almonte\R1\AA01
...
Session-Merge: Starte Zusammenführung für: C:\Testdaten\RT12_ESP-Almonte\R1\AA01
Session-Merge: 4 Run(s) entdeckt.
Session-Merge: Run_202 → OK (1 Lap(s), 47 Channel(s), LastT=125.43s)
Session-Merge: Run_203 → OK (1 Lap(s), 47 Channel(s), LastT=98.21s)
Session-Merge: Run_204 → OK (1 Lap(s), 47 Channel(s), LastT=110.55s)
Session-Merge: Run_205 → OK (21 Lap(s), 47 Channel(s), LastT=2847.12s)
Session-Merge: 4 von 4 Run(s) erfolgreich geladen. 47 Channel(s) insgesamt.
Session-Merge: Zusammenführung abgeschlossen. Ergebnis-Dataset enthält 47 Channel(s).
Session-Merge abgeschlossen: Session="R1", Rider="AA01", 4/4 Run(s) zusammengeführt, 47 Channel(s).
```

### Teilweiser Erfolg (Runs mit Problemen)

```
Session-Merge: Starte Zusammenführung für: C:\Testdaten\...\AA01
Session-Merge: 6 Run(s) entdeckt.
Session-Merge: Run_201 → NoLaps (Kein gültiger WinTAX4-Run ...)
Session-Merge: Run_202 → OK (1 Lap(s), 47 Channel(s), LastT=125.43s)
Session-Merge: Run_203 → Fehler: Access violation reading location ...
Session-Merge: Run_204 → OK (1 Lap(s), 47 Channel(s), LastT=110.55s)
Session-Merge: Run_205 → OK (21 Lap(s), 47 Channel(s), LastT=2847.12s)
Session-Merge: Run_206 → NoLaps (Kein gültiger WinTAX4-Run ...)
Session-Merge: 3 von 6 Run(s) erfolgreich geladen. 47 Channel(s) insgesamt.
```

### Keine Daten

```
Session-Merge: Starte Zusammenführung für: C:\Testdaten\...\AA01
Session-Merge: Keine Runs im angegebenen Verzeichnis gefunden.
Session-Merge: Keine Daten zum Zusammenführen gefunden.
```

### Ausgaben-Schlüssel

| Prefix | Bedeutung |
|--------|-----------|
| `→ OK` | Run erfolgreich geladen, Daten werden gemergt |
| `→ NoLaps` | Run existiert, aber keine Lap-Ordner mit Daten |
| `→ NoChannels` | Run geladen, aber keine Channels vorhanden |
| `→ Fehler:` | Exception beim Laden — Fehlermeldung folgt |
| `X von Y Run(s)` | X = erfolgreich gemergt, Y = total entdeckt |

---

## Anhang: Schnellreferenz für häufige Debug-Szenarien

### "FindRuns gibt 0 Runs zurück"

1. Prüfe: Existiert im Rider-Ordner mindestens ein `cableData.ztx` (rekursiv)?
2. Prüfe: Ist der `riderPath` korrekt? (`Path.Combine(runPath, "..")`)
3. Test im Immediate Window: `_browse.FindRuns(@"C:\...\AA01")`

### "LoadRun gibt -1 zurück"

1. Prüfe: Ist die `cableData.ztx` valide (nicht korrupt, nicht 0 Bytes)?
2. Prüfe: Sind die KtmDataHandler-DLLs korrekt geladen? (Output Window → Build)
3. Test: Denselben Run in WinTAX4 öffnen — funktioniert es dort?

### "GetChannels gibt leeres Dataset zurück"

1. Prüfe: `GetChannelsNames()` — gibt es überhaupt Channel-Namen?
2. Prüfe: `NumElements()` nach `GetChannels()` — wirklich 0?
3. Möglicherweise sind die Channel-Daten in der `cableData.ztx` leer (aufgezeichnet aber keine Samples)

### "Concat wirft Exception"

1. Prüfe die Quell-Timeseries: `timeseries.SignalData` nicht null?
2. Prüfe die Ziel-Timeseries: `accumulated.SignalData` nicht null?
3. Prüfe: Stimmen die Datentypen überein? (`DataType`-Property)

### "Merged Dataset hat weniger Channels als erwartet"

1. Prüfe `allChannelNames` (HashSet) — enthält es alle erwarteten Namen?
2. Prüfe: Werden Channels von `ValidateTimeseries` übersprungen? (Watch auf Console-Output)
3. Setze bedingten Breakpoint in `ValidateTimeseries` mit `return false`
