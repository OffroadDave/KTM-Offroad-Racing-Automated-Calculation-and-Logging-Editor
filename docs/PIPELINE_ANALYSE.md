# ORACLE Pipeline-Analyse: Schwachstellen, Architektur & Roadmap

## Inhaltsverzeichnis

1. [Ist-Zustand: Die komplette Pipeline](#1-ist-zustand-die-komplette-pipeline)
2. [Schwachstellenanalyse](#2-schwachstellenanalyse)
3. [Architektur-Bewertung: ORACLE als Monolith vs. Library](#3-architektur-bewertung-oracle-als-monolith-vs-library)
4. [Empfehlung: Tool-Landschaft](#4-empfehlung-tool-landschaft)
5. [Detailplan: download.done](#5-detailplan-downloaddone)
6. [Detailplan: ORACLE Refactoring](#6-detailplan-oracle-refactoring)
7. [Detailplan: Idempotenz & Robustheit](#7-detailplan-idempotenz--robustheit)
8. [Risikomatrix](#8-risikomatrix)

---

## 1. Ist-Zustand: Die komplette Pipeline

### Datenfluss End-to-End

```
╔═══════════════════════════════════════════════════════════════════════════╗
║                           STRECKE (Race/Test)                           ║
║  Fahrer fährt Session → Daten auf Marelli-Logger (cableData.ztx)        ║
╚════════════════════════════════╤══════════════════════════════════════════╝
                                 │
                                 ▼
╔═══════════════════════════════════════════════════════════════════════════╗
║                              BOX / PADDOCK                              ║
║  Mitarbeiter downloaded Daten vom Logger → Laptop                       ║
║  Ordnerstruktur: Event/Session/Rider/Run_xxx/Lap_xxx/cableData.ztx      ║
╚════════════════════════════════╤══════════════════════════════════════════╝
                                 │  OneDrive Sync
                                 ▼
╔═══════════════════════════════════════════════════════════════════════════╗
║                         ONEDRIVE (CLOUD)                                ║
║  Automatische Synchronisation → Firma                                   ║
╚════════════════════════════════╤══════════════════════════════════════════╝
                                 │  OneDrive Sync
                                 ▼
╔═══════════════════════════════════════════════════════════════════════════╗
║                    WORKSTATION (Lokal, Firma)                            ║
║                                                                         ║
║  ┌─────────────────────────────────────────────────────────────────┐     ║
║  │ OneDrive → Lokale Kopie                                        │     ║
║  │ C:\MXTrackData\...\Event\Session\Rider\Run_xxx\               │     ║
║  └──────────────────────────┬──────────────────────────────────────┘     ║
║                              │                                          ║
║  ┌───────────────────────────▼──────────────────────────────────────┐    ║
║  │ KRAKEN (Polling alle 5s, .NET 4.7.2)                            │    ║
║  │                                                                  │    ║
║  │  1. Scannt definierte Pfade nach download.done                   │    ║
║  │  2. Findet download.done → startet Job-Pipeline                  │    ║
║  │  3. Jobs: INIT → POPULATE → COMMIT → VCH → EXE → WTX_EXP       │    ║
║  │                                          │                       │    ║
║  │                                          ▼                       │    ║
║  │                                    ┌──────────┐                  │    ║
║  │                                    │ ORACLE   │ ◄── EXE-Job     │    ║
║  │                                    │ .exe     │                  │    ║
║  │                                    └────┬─────┘                  │    ║
║  │                                         │                        │    ║
║  │                                         ▼                        │    ║
║  │                                    ┌──────────┐                  │    ║
║  │                                    │ MotoDB   │ ◄── EXE-Job     │    ║
║  │                                    │ .exe     │     (DependsOn   │    ║
║  │                                    └────┬─────┘      ORACLE)     │    ║
║  │                                         │                        │    ║
║  └─────────────────────────────────────────┼────────────────────────┘    ║
║                                            │                            ║
║  ┌─────────────────────────────────────────▼────────────────────────┐    ║
║  │ SQL-Datenbank (MotoDB Output)                                    │    ║
║  └──────────────────────────┬───────────────────────────────────────┘    ║
║                              │                                          ║
║  ┌───────────────────────────▼──────────────────────────────────────┐    ║
║  │ Tableau (Visualisierung & Analyse)                               │    ║
║  └──────────────────────────────────────────────────────────────────┘    ║
╚═════════════════════════════════════════════════════════════════════════════╝
```

### KRAKEN Job-Konfiguration (aus JobList.xsd)

KRAKEN kennt folgende Job-Typen mit Abhängigkeiten:

```
INIT ──────────► POPULATE ──────────► COMMIT
                     │                    │
                     ▼                    ▼
                   VCH (Math-DLLs)      EXE (ORACLE.exe)
                                          │
                                          ▼
                                        EXE (MotoDB.exe)
                                          │
                                          ▼
                                       WTX_EXP (Export)
```

| Job-Typ | Funktion | Relevanz für ORACLE |
|---------|----------|-------------------|
| `INIT` | Kalkulationsordner initialisieren | Vorbereitung |
| `POPULATE` | Daten in Kalkulationsordner kopieren (mit Filter) | Datenbereitstellung |
| `COMMIT` | Ergebnisse zurück in Run schreiben | Ähnlich wie `SaveResults` |
| `VCH` | Virtual-Channel-Berechnungen (Marelli Math-DLLs) | Parallele Berechnung |
| `EXE` | Externes Programm ausführen (ExePath + Arguments) | **ORACLE wird hier aufgerufen** |
| `WTX_EXP` | WinTAX4-Export (ZTX, MAT, CSV, ...) | Nachverarbeitung |
| `WEATHER_STATION` | Wetterdaten einlesen | Optional |

### KRAKEN EXE-Job Schema für ORACLE

```xml
<EXE>
    <JobAlias>ORACLE Postprocessing</JobAlias>
    <JobVersion>1.0</JobVersion>
    <Compulsory>Yes</Compulsory>
    <Parameters>
        <ExePath>C:\KTM Tools\ORACLE\ORACLE.exe</ExePath>
        <Arguments>%RUN_PATH%</Arguments>   <!-- KRAKEN ersetzt Variable -->
    </Parameters>
    <Dependencies>
        <DependsOnJob>VCH</DependsOnJob>    <!-- Erst nach VCH-Berechnungen -->
    </Dependencies>
</EXE>
```

### download.done Schema (aus XSD)

```xml
<Done>
    <UserName>MaxMustermann</UserName>
    <ComputerName>PADDOCK-LAPTOP</ComputerName>
    <UserDomain>KTM</UserDomain>
    <DateTime>2025-06-15T14:30:00</DateTime>
    <LocalPath>C:\MXTrackData\...\Run_205</LocalPath>
    <RemotePath>\\onedrive\...\Run_205</RemotePath>
    <Run path="Run_205" size="524288" subfolders="21" files="2">
        <File size="1024">Session.xml</File>
        <Lap path="Lap_001" size="24576" subfolders="0" files="1">
            <File size="24576">cableData.ztx</File>
        </Lap>
        <Lap path="Lap_002" size="24576" subfolders="0" files="1">
            <File size="24576">cableData.ztx</File>
        </Lap>
        <!-- ... weitere Laps ... -->
    </Run>
</Done>
```

**Wichtige Felder:**
- `Run.path`, `Run.size`, `Run.subfolders`, `Run.files` → Integritätsprüfung
- `Lap.path`, `Lap.size` → Pro-Lap Validierung
- `File.size` → Dateigrößenvergleich nach Download

---

## 2. Schwachstellenanalyse

### 🔴 KRITISCH — Fehlende download.done

| # | Schwachstelle | Risiko | Auswirkung |
|---|--------------|--------|------------|
| K1 | **Keine automatische download.done-Erstellung** | Hoch | KRAKEN erkennt keine neuen Daten → komplette Pipeline blockiert |
| K2 | **Fehlende Kriterien für "Download abgeschlossen"** | Hoch | download.done könnte zu früh erstellt werden → ORACLE verarbeitet unvollständige Daten |
| K3 | **OneDrive-Sync hat kein "fertig"-Signal** | Hoch | Dateien könnten noch synchronisieren während ORACLE startet |

**Kern-Problem:** OneDrive synchronisiert Datei für Datei. Es gibt keinen atomaren "alles ist da"-Moment. Die `download.done` muss genau diesen Moment erkennen.

### 🟡 MITTEL — ORACLE Architektur

| # | Schwachstelle | Risiko | Auswirkung |
|---|--------------|--------|------------|
| M1 | **ORACLE kennt nur einen einzelnen `runPath`** | Mittel | Session-Merge berechnet `riderPath` aus `runPath + ".."` — fragil bei Pfadänderungen |
| M2 | **Kein Exit-Code-Management** | Mittel | KRAKEN-EXE-Job prüft Exit-Code — ORACLE gibt keinen definierten zurück |
| M3 | **Console.WriteLine als einziges Logging** | Mittel | Kein File-Logging → bei KRAKEN-Ausführung sind Logs verloren |
| M4 | **ImportDataCalculator arbeitet nur auf EINEM Run** | Mittel | Bei Aufruf über Rider-Pfad wird nur der erste Run berechnet |
| M5 | **RunPropertyEditor nutzt eine EINZIGE MarelliDataAccess-Instanz** für alle Runs in der Schleife | Mittel | Potentieller State-Leak zwischen Runs (SetDataLocation → ReleaseObjects Zyklus) |
| M6 | **Session-Merge ohne Persistenz** | Mittel | `MergedSessionData` wird erzeugt aber nirgends gespeichert → geht nach Programmende verloren |

### 🟢 NIEDRIG — Betriebsrisiken

| # | Schwachstelle | Risiko | Auswirkung |
|---|--------------|--------|------------|
| N1 | **Keine Idempotenz** | Niedrig | ORACLE zweimal auf denselben Run ausführen → Properties doppelt gesetzt, ImportData doppelt |
| N2 | **KRAKEN pollt alle 5 Sekunden** | Niedrig | CPU-Last gering, aber bei vielen Pfaden skaliert es schlecht |
| N3 | **KRAKEN ist .NET 4.7.2, ORACLE ist .NET 8** | Niedrig | Kein direktes Problem (EXE-Aufruf), aber DLL-Sharing (KtmDataHandler) hat Versionsrisiko |
| N4 | **Keine Fortschrittsanzeige** | Niedrig | Bei langen Sessions (50+ Laps) kein Feedback ob ORACLE noch arbeitet |

---

### Timing-Problem im Detail: OneDrive → download.done

```
Zeitachse:

t=0    Mitarbeiter startet Download vom Logger
t=30s  Erste Dateien auf OneDrive (kleine XMLs)
t=60s  OneDrive-Sync auf Workstation beginnt
t=90s  Kleine Dateien (Session.xml, LapHeader.xml) sind lokal
t=120s cableData.ztx Lap_001 ist lokal (24 KB)
t=150s cableData.ztx Lap_002 ist lokal
  ...
t=600s cableData.ztx Lap_021 ist lokal (Run_205 komplett)
t=601s ← Alle Dateien da, ABER:

Probleme:
  ❌ OneDrive hat KEIN "Sync complete"-Event
  ❌ Dateien können in beliebiger Reihenfolge ankommen
  ❌ Dateien können temporär 0 Bytes haben (Platzhalter)
  ❌ OneDrive sperrt Dateien während des Schreibens
```

---

## 3. Architektur-Bewertung: ORACLE als Monolith vs. Library

### Ist-Zustand: Monolithische EXE

```
ORACLE.exe
├── Program.cs           ← Main(): Steuert ALLES sequentiell
├── OracleConfig.cs      ← Konfiguration
├── HedFileService.cs    ← .HED lesen/schreiben
├── KompassService.cs    ← KOMPASS-DB abfragen
├── RunPropertyEditor.cs ← Run-Properties setzen
├── ImportDataCalculator.cs ← Berechnungen (sVehicle)
├── SessionMergeService.cs  ← NEU: Session-Merge
└── RunMetadata.cs       ← Datenmodell
```

**Problem:** `Program.Main()` mischt Orchestrierung, Validierung und Geschäftslogik. Der KRAKEN-EXE-Job kann nur die ganze Pipeline oder nichts ausführen.

### Empfehlung: Command-basierte Architektur

ORACLE sollte verschiedene **Modi/Commands** unterstützen, damit KRAKEN (und auch manuelle Nutzung) gezielt einzelne Schritte aufrufen können:

```
ORACLE.exe <command> [options]

Commands:
  process    Komplette Postprocessing-Pipeline für einen Run
  merge      Nur Session-Merge (alle Runs eines Riders)
  validate   download.done generieren/validieren
```

### Warum mehrere Commands statt mehrere EXEs?

| Kriterium | Mehrere EXEs | Eine EXE mit Commands |
|-----------|-------------|----------------------|
| Deployment | N Dateien deployen | 1 Datei |
| Shared Code | DLLs oder Copy/Paste | Alles in einer Assembly |
| Konfiguration | N Config-Dateien | 1 oracle-config.json |
| KtmDataHandler.dll | N Kopien oder shared Pfad | 1 Kopie |
| KRAKEN-Integration | N EXE-Jobs | N EXE-Jobs (gleiche EXE, andere Args) |
| Versionierung | Versions-Chaos | 1 Version |
| **Empfehlung** | ❌ | ✅ |

---

## 4. Empfehlung: Tool-Landschaft

### Neue Gesamtarchitektur

```
╔═══════════════════════════════════════════════════════════════╗
║  SENTINEL (NEUES TOOL — Lightweight .NET 8 Console)         ║
║                                                              ║
║  Aufgabe: OneDrive-Ordner überwachen,                       ║
║           download.done generieren wenn Sync abgeschlossen   ║
║                                                              ║
║  Läuft als Windows-Dienst oder Scheduled Task                ║
╚══════════════════════════╤════════════════════════════════════╝
                            │  Erzeugt download.done
                            ▼
╔═══════════════════════════════════════════════════════════════╗
║  KRAKEN (bestehend, unverändert)                             ║
║                                                              ║
║  Erkennt download.done → startet Job-Pipeline                ║
║  Job 1: ORACLE.exe process <runPath>                         ║
║  Job 2: ORACLE.exe merge <riderPath>                         ║
║  Job 3: MotoDB.exe <runPath>                                 ║
╚══════════════════════════╤════════════════════════════════════╝
                            │
                            ▼
╔═══════════════════════════════════════════════════════════════╗
║  ORACLE (refactored, Command-basiert)                        ║
║                                                              ║
║  oracle process <runPath>  → Properties + Calc + Save        ║
║  oracle merge <riderPath>  → Session-Merge + Export          ║
║  oracle validate <path>    → Datenintegrität prüfen          ║
╚══════════════════════════╤════════════════════════════════════╝
                            │
                            ▼
╔═══════════════════════════════════════════════════════════════╗
║  MotoDB (bestehend)                                          ║
║  SQL-Datenbank erstellen → Tableau                           ║
╚═══════════════════════════════════════════════════════════════╝
```

### Warum ein separates SENTINEL-Tool?

| Argument | Erklärung |
|----------|-----------|
| **Separation of Concerns** | "Ist der Download fertig?" ist eine völlig andere Frage als "Wie verarbeite ich die Daten?" |
| **Dauerlauf vs. Einmalausführung** | SENTINEL läuft permanent und überwacht. ORACLE wird einmalig pro Run gestartet und beendet sich. |
| **Unabhängiges Deployment** | SENTINEL-Änderungen erfordern keinen ORACLE-Rebuild |
| **Testbarkeit** | SENTINEL kann isoliert getestet werden (Mock-Ordner mit simuliertem Sync) |
| **KRAKEN-Kompatibilität** | KRAKEN braucht keine Änderung — es sieht nur die download.done |

---

## 5. Detailplan: download.done

### Kriterien für "Download abgeschlossen"

Das ist die **schwierigste Frage** in der Pipeline. Hier die robusten Kriterien:

#### Strategie: Stabilitätsprüfung (Size-Stable-Check)

```
SENTINEL Algorithmus:

1. Scanne Rider-Ordner nach Run_*-Verzeichnissen
2. Für jeden Run ohne download.done:
   a. Zähle alle Dateien rekursiv (*.ztx, *.xml)
   b. Summiere Dateigrößen
   c. Speichere als Snapshot: { timestamp, fileCount, totalSize }
3. Warte N Sekunden (z.B. 30s)
4. Wiederhole Scan
5. Vergleiche mit vorherigem Snapshot:
   ├── fileCount geändert?     → Noch nicht fertig, weiter warten
   ├── totalSize geändert?     → Noch nicht fertig, weiter warten
   ├── Dateien mit Size=0?     → OneDrive-Platzhalter, nicht fertig
   ├── Dateien gelockt?        → Noch am Schreiben, nicht fertig
   └── Alles stabil + keine 0-Byte-Dateien + keine Locks?
       → STABIL seit N Sekunden
6. Stabilitätsschwelle erreicht (z.B. 3 aufeinanderfolgende stabile Checks)?
   → download.done generieren
```

#### Warum nicht FileSystemWatcher?

| Ansatz | Problem |
|--------|---------|
| `FileSystemWatcher` | OneDrive-Sync erzeugt viele Events (Rename, Create, Change). Buffer-Overflow bei vielen Dateien. Unzuverlässig für "alles fertig"-Erkennung. |
| Polling + Stability | Einfach, robust, keine verpassten Events. Kostet minimal CPU. |

#### Zusätzliche Validierungskriterien

```
Pflicht-Checks:
  ✅ Mindestens 1 Lap_*-Ordner mit cableData.ztx vorhanden
  ✅ Keine Datei hat Size = 0 (OneDrive-Platzhalter)
  ✅ Keine Datei ist gelockt (FileShare.Read test)
  ✅ Stabilität über mindestens 2 Polling-Zyklen (z.B. 60s)

Optional-Checks:
  ⬜ cableData.ztx hat plausible Mindestgröße (> 1 KB)
  ⬜ Session.xml ist wohlgeformt (XML-Parse-Test)
  ⬜ Ordnerstruktur passt zu WinTAX4-Schema (Run_xxx/Lap_xxx)
```

#### Generierte download.done Struktur

```xml
<Done>
    <UserName>SENTINEL</UserName>
    <ComputerName>WORKSTATION-01</ComputerName>
    <UserDomain>KTM</UserDomain>
    <DateTime>2025-06-15T14:32:00</DateTime>
    <LocalPath>C:\MXTrackData\...\Run_205</LocalPath>
    <RemotePath></RemotePath>
    <Run path="Run_205" size="524288" subfolders="21" files="23">
        <File size="2048">Session.xml</File>
        <Lap path="Lap_001" size="24576" subfolders="0" files="1">
            <File size="24576">cableData.ztx</File>
        </Lap>
        <!-- ... -->
    </Run>
</Done>
```

---

## 6. Detailplan: ORACLE Refactoring

### Phase 1 — Exit-Codes & Logging (sofort)

ORACLE braucht definierte Exit-Codes für KRAKEN:

```
Exit-Code   Bedeutung                    KRAKEN-Reaktion
─────────   ──────────────────────────   ─────────────────
0           Erfolg                       → Nächsten Job starten
1           Fehler (allgemein)           → Pipeline abbrechen, Log prüfen
2           Ungültige Argumente          → Konfigurationsfehler
3           Daten nicht gefunden         → Warten / Retry
4           Partielle Verarbeitung       → Warnung, weitermachen
```

### Phase 2 — Command-Routing (kurzfristig)

```
ORACLE.exe process <runPath>
    → Metadaten laden
    → Properties setzen (NUR für diesen Run)
    → Berechnungen ausführen (NUR für diesen Run)
    → Ergebnisse speichern

ORACLE.exe merge <riderPath>
    → Alle Runs im Rider-Ordner zusammenführen
    → MergedSessionData als WinTAX4-Run speichern (via IMarelliDataAccessWrite)

ORACLE.exe validate <runPath>
    → cableData.ztx Integrität prüfen
    → Channels laden und prüfen ob Daten vorhanden
    → Exit-Code 0 = OK, 3 = Fehler
```

**KRAKEN-Konfiguration (3 EXE-Jobs in Reihenfolge):**

```xml
<EXE>
    <JobAlias>ORACLE Validate</JobAlias>
    <Parameters>
        <ExePath>ORACLE.exe</ExePath>
        <Arguments>validate %RUN_PATH%</Arguments>
    </Parameters>
    <Dependencies><DependsOnJob>INIT</DependsOnJob></Dependencies>
</EXE>

<EXE>
    <JobAlias>ORACLE Process</JobAlias>
    <Parameters>
        <ExePath>ORACLE.exe</ExePath>
        <Arguments>process %RUN_PATH%</Arguments>
    </Parameters>
    <Dependencies><DependsOnJob>ORACLE Validate</DependsOnJob></Dependencies>
</EXE>

<EXE>
    <JobAlias>ORACLE Merge</JobAlias>
    <Parameters>
        <ExePath>ORACLE.exe</ExePath>
        <Arguments>merge %RIDER_PATH%</Arguments>
    </Parameters>
    <Dependencies><DependsOnJob>ORACLE Process</DependsOnJob></Dependencies>
</EXE>
```

### Phase 3 — OraclePipeline-Klasse (mittelfristig)

Die aktuelle `Program.Main()` wird zu einer orchestrierbaren Klasse:

```
OraclePipeline
├── ProcessRun(runPath)           ← Aktueller Main()-Ablauf für 1 Run
│   ├── LoadConfig()
│   ├── ResolveMetadata()
│   ├── SetProperties()
│   ├── RunCalculations()
│   └── SaveResults()
│
├── MergeSession(riderPath)       ← SessionMergeService delegieren
│   ├── DiscoverRuns()
│   ├── MergeChannels()
│   └── ExportMergedRun()         ← NEU: via IMarelliDataAccessWrite
│
└── ValidateRun(runPath)          ← NEU: Integritätsprüfung
    ├── CheckFileStructure()
    ├── CheckDataLoadable()
    └── CheckChannelsPresent()
```

**Vorteile:**
- `Program.Main()` wird zum dünnen Argument-Parser
- Jede Methode ist einzeln testbar
- Zukünftige Tools können `OraclePipeline` als Library nutzen (wenn als NuGet verpackt)

### Konkreter Refactoring-Vorschlag für Program.cs

```
Vorher (aktuell):
  Main(args)
    ├── args[0] = runPath
    ├── Validierung
    ├── Config
    ├── Metadaten
    ├── Properties
    ├── Calculation
    └── Session-Merge

Nachher:
  Main(args)
    ├── Parse Command (process | merge | validate)
    ├── Parse Options (Pfad, Flags)
    └── switch(command)
        ├── "process" → pipeline.ProcessRun(runPath)
        ├── "merge"   → pipeline.MergeSession(riderPath)
        └── "validate"→ pipeline.ValidateRun(runPath)
    return exitCode
```

---

## 7. Detailplan: Idempotenz & Robustheit

### Problem: Doppelte Ausführung

Wenn KRAKEN ORACLE zweimal für denselben Run startet (z.B. durch Retry):

| Aktion | Aktuelles Verhalten | Problem |
|--------|-------------------|---------|
| `SetRunProperties` | Überschreibt → OK | Kein Problem (idempotent) |
| `AddImportDataToRun` | **Fügt hinzu** | ❌ Doppelte ImportData-Channels |
| `Session-Merge` | Erzeugt neues Dataset | Kein Problem (nicht persistiert) |

### Lösung: Status-Datei pro Run

```
Run_205/
├── Lap_001/
├── ...
├── cableData.ztx
├── download.done          ← SENTINEL hat erstellt
└── oracle.status.json     ← ORACLE erstellt nach Abschluss
```

```json
{
    "version": "1.0",
    "runPath": "C:\\...\\Run_205",
    "processedAt": "2025-06-15T14:35:00Z",
    "oracleVersion": "1.0.0",
    "steps": {
        "metadata": { "status": "success", "source": "hed" },
        "properties": { "status": "success" },
        "calculations": { "status": "success", "channels": ["sVehicle"] },
        "merge": { "status": "success", "mergedRuns": 4 }
    },
    "exitCode": 0
}
```

**Idempotenz-Logik:**
```
if (oracle.status.json existiert UND exitCode == 0):
    Console.WriteLine("Run bereits verarbeitet. Überspringe.")
    return 0

if (oracle.status.json existiert UND exitCode != 0):
    Console.WriteLine("Vorherige Verarbeitung fehlgeschlagen. Wiederhole.")
    // Weiter mit Verarbeitung
```

---

## 8. Risikomatrix

### Priorisiert nach Geschäftsauswirkung

```
                        Eintrittswahrscheinlichkeit
                     Niedrig         Mittel          Hoch
                  ┌──────────────┬──────────────┬──────────────┐
         Hoch     │              │ M6: Merge    │ K1: Keine    │
                  │              │ nicht        │ download.done│
   Aus-           │              │ persistiert  │              │
   wirkung        ├──────────────┼──────────────┼──────────────┤
         Mittel   │ N3: .NET     │ M2: Kein     │ K3: OneDrive │
                  │ Version      │ Exit-Code    │ Sync-Timing  │
                  │ Mismatch     │              │              │
                  ├──────────────┼──────────────┼──────────────┤
         Niedrig  │ M5: State    │ N1: Keine    │ M3: Nur      │
                  │ Leak         │ Idempotenz   │ Console-Log  │
                  │ Property-    │              │              │
                  │ Editor       │              │              │
                  └──────────────┴──────────────┴──────────────┘
```

### Empfohlene Reihenfolge der Umsetzung

| Priorität | Maßnahme | Aufwand | Begründung |
|-----------|----------|---------|------------|
| 🔴 1 | **Exit-Codes in ORACLE** | 1h | Ohne definierte Exit-Codes kann KRAKEN nicht auf Fehler reagieren |
| 🔴 2 | **SENTINEL Prototyp** (download.done Generator) | 4-8h | Pipeline ist ohne download.done komplett blockiert |
| 🟡 3 | **Command-Routing** (process/merge/validate) | 2-4h | Trennt Concerns, ermöglicht gezielte KRAKEN-Jobs |
| 🟡 4 | **oracle.status.json** (Idempotenz) | 2h | Verhindert doppelte Verarbeitung |
| 🟡 5 | **File-Logging** (zusätzlich zu Console) | 1-2h | Debugging bei KRAKEN-Ausführung |
| 🟡 6 | **Session-Merge Persistenz** (IMarelliDataAccessWrite) | 2-4h | Merge-Ergebnis geht sonst verloren |
| 🟢 7 | **OraclePipeline-Klasse** Refactoring | 4h | Clean Architecture, Testbarkeit |
| 🟢 8 | **ImportDataCalculator** auf alle Runs erweitern | 2h | Aktuell nur 1 Run |
