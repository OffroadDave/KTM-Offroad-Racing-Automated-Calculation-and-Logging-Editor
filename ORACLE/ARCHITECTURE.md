# ORACLE – Architektur-Dokumentation

## Übersicht

**ORACLE** (KTM Offroad Racing Automated Calculation and Logging Editor) ist ein CLI-Tool
zur Nachbearbeitung von WinTAX4-Telemetriedaten aus Motocross-/Enduro-Runs. Seit der
Zusammenführung der früheren `Ktm.Racing.Core`-Bibliothek liegt die **gesamte Logik im
ORACLE-Projekt** (Namespace `Ktm.Racing.Oracle`).

Das Programm:
1. Extrahiert Run-Metadaten aus `.HED`-Dateien oder der KOMPASS-Datenbank (inkl. Alias-Normalisierung).
2. Erkennt, welche Runs tatsächlich ein **Rennen** sind (GPS-/Gas-basiert).
3. Berechnet abgeleitete Kanäle (`sVehicle`, per-Lap-Distanz) und schreibt Run-/Lap-Properties
   **direkt in die WinTAX4-XML-Dateien**.
4. Führt ein durch Sturz/Abwürgen fragmentiertes Rennen über mehrere Run-Ordner hinweg mit
   **durchgehender** Distanz- und Lap-Nummerierung zusammen (Original-Ordner bleiben unverändert).

## Commands

```
ORACLE.exe process  <run|rider> [--force]   Postprocessing; Rider-Ordner: renn-bewusst, durchgehend
ORACLE.exe detect   <riderPath>             Renn-Erkennung (read-only)
ORACLE.exe validate <runPath>               Datenintegritätsprüfung
ORACLE.exe <runPath>            [--force]   Legacy-Modus (= process für einen Run)
```

`process` routet automatisch: einzelner Run → per-Run-Verarbeitung (KRAKEN-kompatibel);
Rider-/Session-Ordner → renn-bewusste Stint-Verarbeitung (`RaceStintProcessor`).

## Datenfluss

```
Argument (run- oder riderPath)
      │
      ▼
┌──────────────┐   nicht gefunden   ┌──────────────┐
│  HED-Datei   │ ─────────────────► │  KOMPASS DB  │
│ (Rider/Run)  │                    │  + Pfad-API  │
└──────┬───────┘                    └──────┬───────┘
       │      RunMetadata (BikeAlias-Norm.)│
       └───────────────┬───────────────────┘
                       ▼
        ┌───────────────────────────┐
        │  RaceSegmentDetector      │  GPS-Distanz + Speed + Gas
        │  → Rennen ja/nein je Run  │  (Guards: ValidSat, nEngine720)
        └───────────────┬───────────┘
                        ▼
        ┌───────────────────────────┐
        │  RaceStintProcessor       │  ordnet Renn-Runs, akkumuliert Offsets
        └───────────────┬───────────┘
             ┌──────────┴────────────┐
             ▼                       ▼
  ┌────────────────────┐   ┌────────────────────────┐
  │ ImportDataCalc.    │   │ XmlRunEditor           │
  │ sVehicle + per-    │   │ direkte XML-Bearbeitung │
  │ Lap-Distanzen      │   │ (LapHeader/Car/Session) │
  └────────────────────┘   └────────────────────────┘
```

## Projektstruktur

Alles liegt im **ORACLE**-Projekt (`Ktm.Racing.Oracle`), thematisch gegliedert:

| Ordner        | Inhalt / Verantwortung                                                                 |
|---------------|-----------------------------------------------------------------------------------------|
| `Program.cs`  | CLI-Einstiegspunkt, Command-Routing                                                      |
| `Runtime/`    | `KtmRuntime` (native DLL-/Assembly-Bootstrap), `KtmDataHandlerFactory` (Autofac-DI)      |
| `Pipeline/`   | `OraclePipeline` (Orchestrierung), `OracleExitCode`, `OracleStatus` (Idempotenz), `OracleLogger`, `OracleConfig` |
| `Metadata/`   | `RunMetadata`, `HedFileService`, `KompassService`, `BikeAlias` (Alias-Normalisierung)    |
| `Processing/` | `ImportDataCalculator` (sVehicle + Lap-Distanz), `RunCalcResult`, `XmlRunEditor`         |
| `Detection/`  | `RaceSegmentDetector`, `RaceClassification`, `RaceDetectionThresholds`, `RaceStintProcessor` |
| `Weather/`    | `WeatherService` (Wetter je Session → `.HED` `[TRACK] trackCondition` + `[WEATHER]`, `Lap_*\Meteo.xml`), `OpenMeteoWeatherSource`, `WeatherCalculator`, `MeteoXml`, `WeatherSettings` |

## Renn-Erkennung & Continuity (Option C)

**Problem:** Bei jedem Motorstart erhöht der Logger den Zähler; ein durch Sturz/Abwürgen
unterbrochenes Rennen zerfällt so über mehrere `Run_xxx`-Ordner. Reine Zeitlücken trennen
Rennen nicht zuverlässig von Auslaufrunden (diese folgen mit Sekundenabstand).

**Erkennung** (`RaceSegmentDetector`, rein antriebsstrang-unabhängig):
- **Primär:** GPS-Distanz (aus `Lat_dez`/`Lon_dez`, aufbereitet durch `GpsCleaner`: ohne Aussetzer und GPS-Sprünge) + daraus abgeleitete Geschwindigkeit + mittlere Gasstellung (`rThrottle1`).
- **Guards:** `nEngine720` (Motor an?), `ValidSat` (GPS gültig?); bei GPS-Ausfall Fallback auf Gas + Drehzahl.
- `vRearnEng` wird bewusst NICHT verwendet (aus Drehzahl × Getriebeübersetzung berechnet, unzuverlässig).
- Schwellen konfigurierbar (`RaceDetection` in `oracle-config.json`).

**Continuity** (`RaceStintProcessor`): Die erkannten Renn-Runs werden chronologisch geordnet und mit
akkumulierten Offsets verarbeitet — `sVehicle` und `Lap`/`TrackLap` laufen **durchgehend** über die
Renn-Runs (z. B. Run A Laps 1–5, Run B Laps 6–21). Die Original-Ordnerstruktur bleibt unverändert.

## Property-Persistenz: direkte XML-Bearbeitung (`XmlRunEditor`)

Die KtmDataHandler-Property-API (`AddRunProperty`/`AddLapProperty`) persistiert in dieser Umgebung
**nicht** in die WinTAX4-XML-Dateien. Deshalb schreibt `XmlRunEditor` die Werte direkt:

| Datei              | Felder                                                                        |
|--------------------|-------------------------------------------------------------------------------|
| `LapHeader.xml`    | `Lap`, `TrackLap`, `Alias`, `Run`, `TrackRun`, `Marker`, `LapDistance` (primär) |
| `Car.xml`          | `Name`, `DriverName`                                                           |
| `Session.xml`      | `Track`, `TrackLen`                                                            |
| `RunLapHeader.xml` | `<Infos>` je Lap (sekundär, konsistent nachgezogen)                           |

`LapDistance` wird pro Lap aus den GPS-Koordinaten berechnet (`GetChannels` mit Lap-Filter → Haversine).
Der berechnete Kanal `sVehicle` wird als `importData.ztx` über `AddImportDataToRun` gespeichert.

### bikeName-Konsistenz (keine Leerzeichen)

Ein Geräteordner mit Leerzeichen (z. B. `AA 01`) ist ein bekannter Datenfehler und führt downstream
(MotoDB/Tableau, KOMPASS-Fahrersuche) zu Problemen. ORACLE stellt `bikeName` durchgängig ohne
Leerzeichen sicher (`AA 01` → `AA01`, siehe `BikeAlias`):

- **Werte:** `Alias`/`CarName`/`Name` in `LapHeader.xml`, `Car.xml`, `RunLapHeader.xml` werden im
  `XmlRunEditor` defensiv normalisiert; `RunMetadata.BikeName` wird bereits beim Einlesen (`HED`/KOMPASS) normalisiert.
- **Pfade:** Das Alias-Segment in `<Path>`-Feldern (v. a. `RunLapHeader > Inf > Path`) wird normalisiert.
- **Ordner:** Ist `NormalizeDeviceFolderName` aktiv (Default), benennt ORACLE nicht-kanonische
  Geräteordner **vor** der Verarbeitung um (`Directory.Move`). Guarded (nur nicht-kanonisch, nur wenn
  Ziel frei) und **non-fatal** – schlägt die Umbenennung fehl (z. B. Ordner in einem Editor/Watcher
  gesperrt), läuft die Verarbeitung mit dem Originalnamen weiter. Bei per-Run-Aufrufen (KRAKEN) wird
  der Elternordner **nicht** umbenannt.

### engineClass

`RunMetadata.EngineClass` wird beim KOMPASS-Fallback aus dem Pfad extrahiert (bekannte Klassen
`125/150/250/300/350/450/500`) und als `engineClass = …` in die generierte `.HED` geschrieben
(bzw. beim HED-Einlesen zurückgelesen).

## Externe Abhängigkeiten

| Bibliothek                 | Verwendung                                                     |
|----------------------------|----------------------------------------------------------------|
| **KtmDataHandler.dll**     | WinTAX4-Datenzugriff (MarelliDataAccess, MarelliDataBrowse)   |
| **KtmSharedUtils.dll**     | Shared Utilities (IPropertyList, IDataset)                    |
| **Ktm.Racing.Domain**      | Geteilte KOMPASS-/Rider-/Track-Modelle (eigenes Repo)         |
| **Autofac 6.3.0**          | DI-Container zum Auflösen der KtmDataHandler-Objekte          |
| **MathNet.Numerics 5.0.0** | Lineare Interpolation (GPS-Dropout-Glättung)                 |
| **ClosedXML 0.105.1**      | Excel-Reporting (perspektivisch)                              |

> Die nativen KTM-DLLs werden **nicht** ins Output kopiert; sie werden zur Laufzeit aus dem
> „KTM Utils Software Suite"-Ordner geladen (siehe `KtmRuntime`).

## KtmDataHandler API-Übersicht

### IMarelliDataAccessRead
| Methode                              | Beschreibung                          |
|--------------------------------------|---------------------------------------|
| `SetDataLocation(string[])`          | Run-Pfad setzen                       |
| `LoadRun()`                          | Daten laden                           |
| `GetChannelsNames()`                 | Verfügbare Kanalnamen abfragen        |
| `GetChannels(string[])`              | Kanaldaten als IDataset laden         |
| `GetChannels(string[], uint[], …)`   | Kanaldaten **je Lap** (1-basiert)     |
| `GetLastSampleTime()`                | Letzte Sample-Zeit [s]                |

### IMarelliDataAccessWrite
| Methode                          | Beschreibung                          |
|----------------------------------|---------------------------------------|
| `AddImportDataToRun(IDataset)`   | Berechnete Daten speichern (sVehicle) |
| `ReleaseObjects()`               | COM-Ressourcen freigeben              |

> Run-/Lap-Properties werden **nicht** über diese API gesetzt, sondern direkt in die XML (`XmlRunEditor`).

### IMarelliDataBrowse
| Methode                  | Beschreibung                             |
|--------------------------|------------------------------------------|
| `IsRunLocation(path)`    | Prüft ob Pfad ein gültiger Run ist       |
| `FindRuns(path)`         | Findet alle Runs in einem Verzeichnis    |
| `GetEventName(path)`     | Event-Ordnername aus Pfad extrahieren    |
| `GetSessionName(path)`   | Session-Ordnername aus Pfad extrahieren  |
| `GetDeviceName(path)`    | Device/Bike-Ordnername extrahieren       |
| `GetRunName(path)`       | Run-Ordnername aus Pfad extrahieren      |

## Konfiguration

`oracle-config.json` (neben der EXE) überschreibt die Defaults aus `OracleConfig.cs`:

```json
{
  "DatabaseDirectory": "C:\\KTM Tools\\...",
  "DatabaseFileName": "KOMPASS.json",
  "TeamMappings": { "01_KTM-FR": "KTM Factory Racing" },
  "NormalizeDeviceFolderName": true,
  "RaceDetection": {
    "EngineOnMinRpm": 2000,
    "DistanceMinMeters": 1000,
    "TrackLengthFactor": 0.5,
    "SpeedMinKph": 25,
    "ThrottleMinPercent": 15,
    "ThrottleFallbackPercent": 20,
    "RpmRaceMin": 5000,
    "MinValidGpsSamples": 100
  }
}
```

Falls die Datei fehlt, werden die Standardwerte aus `OracleConfig.cs` / `RaceDetectionThresholds.cs` verwendet.

## WinTAX4 Ordnerstruktur

```
.../Discipline/Season/.../Team/.../Event/Session/Device(Bike)/Run_xxx/
   ├── Lap_00001106_00418_Def_000/   (eine Lap-Akquisition; Ordner = eine Runde)
   │     ├── cableData.ztx           Telemetrie (Rohdaten)
   │     ├── importData.ztx          berechnete Kanäle (sVehicle) – von ORACLE
   │     └── LapHeader.xml           Lap-Properties (Lap, TrackLap, Alias, Marker, LapDistance)
   ├── Lap_00001106_00418_Def_001/
   ├── Session.xml                   Track, TrackLen
   ├── Car.xml                       Name, DriverName
   └── RunLapHeader.xml              aggregierte Lap-Infos
```

- **Event/Session/Device/Run**: Standard-WinTAX4-Ebenen → per `IMarelliDataBrowse` auslesbar.
- **Discipline/Season/Team**: KTM-spezifische Ebenen → per Pfad-Index extrahiert (`GetPathPart`).
- Der Lap-Ordnername kodiert den Zündzyklus-Zähler (`00001106`) und die Logger-Seriennummer (`00418`).

## Idempotenz

Nach erfolgreicher Verarbeitung schreibt ORACLE `oracle.status.json` (Run- bzw. Rider-Ebene). Ein
erneuter Aufruf überspringt bereits verarbeitete Pfade (Exit 0); `--force` erzwingt die Neuverarbeitung.
