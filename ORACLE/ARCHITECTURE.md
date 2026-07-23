# ORACLE – Architektur-Dokumentation

## Übersicht

**ORACLE** (KTM Offroad Racing Automated Calculation and Logging Editor) ist ein CLI-Tool
zur Verarbeitung von WinTAX4-Telemetriedaten aus Motocross-/Enduro-Runs.

Das Programm:
1. Extrahiert Run-Metadaten aus .HED-Dateien oder der KOMPASS-Datenbank
2. Setzt Run-/Lap-Properties über die KtmDataHandler-API
3. Berechnet abgeleitete Kanäle (z.B. GPS-Distanz) und speichert diese

## Datenfluss

```
Argument (runPath)
      │
      ▼
┌──────────────┐     ┌──────────────────┐
│  HED-Datei   │ ──► │ RunMetadata      │
│  (Rider/Run) │     │ (bikeName, etc.) │
└──────────────┘     └────────┬─────────┘
      │ nicht gefunden?       │
      ▼                       │
┌──────────────┐              │
│  KOMPASS DB  │ ──►          │
│  + Pfad API  │              │
└──────────────┘              │
                              ▼
               ┌──────────────────────────┐
               │  RunPropertyEditor        │
               │  (AddRunProperty/AddLap)  │
               └──────────────────────────┘
                              │
                              ▼
               ┌──────────────────────────┐
               │  ImportDataCalculator     │
               │  (sVehicle, GPS-Distanz)  │
               └──────────────────────────┘
```

## Dateistruktur

| Datei                     | Verantwortung                                                   |
|---------------------------|-----------------------------------------------------------------|
| `Program.cs`              | CLI-Einstiegspunkt, orchestriert den Programmablauf              |
| `OracleConfig.cs`         | Konfiguration aus oracle-config.json (Pfade, Team-Mappings)     |
| `RunMetadata.cs`          | Datenmodell für extrahierte Run-Metadaten                       |
| `HedFileService.cs`       | .HED-Datei suchen, kopieren, lesen, schreiben                   |
| `KompassService.cs`       | KOMPASS-Datenbank laden, Track/Rider nachschlagen, Datenmodelle |
| `RunPropertyEditor.cs`    | Run-/Lap-Properties via KtmDataHandler API setzen               |
| `ImportDataCalculator.cs` | Berechnungspipeline (GPS-Distanz → sVehicle)                    |
| `XmlRunEditor.cs`         | [Legacy Fallback] Direkte XML-Bearbeitung                       |
| `oracle-config.json`      | Konfigurationsdatei (Pfade, Team-Mappings)                      |

## Externe Abhängigkeiten

| Library                    | Verwendung                                                      |
|----------------------------|-----------------------------------------------------------------|
| **KtmDataHandler.dll**     | WinTAX4-Datenzugriff (MarelliDataAccess, MarelliDataBrowse)    |
| **KtmSharedUtils.dll**     | Shared Utilities (IPropertyList, IDataset)                      |
| **MathNet.Numerics 5.0.0** | Lineare Interpolation (LinearSpline.InterpolateSorted)          |

## KtmDataHandler API-Übersicht

### IMarelliDataAccessRead
| Methode                      | Beschreibung                          |
|------------------------------|---------------------------------------|
| `SetDataLocation(string[])`  | Run-Pfad setzen                       |
| `LoadRun()`                  | Daten laden                           |
| `GetChannelsNames()`         | Verfügbare Kanalnamen abfragen        |
| `GetChannels(string[])`      | Kanaldaten als IDataset laden         |
| `GetLapMarks()`              | Lap-Zeitstempel als double[] abfragen |

### IMarelliDataAccessWrite
| Methode                              | Beschreibung                       |
|--------------------------------------|------------------------------------|
| `AddRunProperty(name, value)`        | Run-Level Property setzen          |
| `AddLapProperty(nLap, name, value)`  | Lap-Level Property setzen          |
| `AddImportDataToRun(IDataset)`       | Berechnete Daten speichern         |
| `ReleaseObjects()`                   | COM-Ressourcen freigeben           |

### IMarelliDataBrowse
| Methode                  | Beschreibung                             |
|--------------------------|------------------------------------------|
| `IsRunLocation(path)`    | Prüft ob Pfad ein gültiger Run ist       |
| `FindRuns(path)`         | Findet alle Runs in einem Verzeichnis    |
| `GetEventName(path)`     | Event-Ordnername aus Pfad extrahieren    |
| `GetSessionName(path)`   | Session-Ordnername aus Pfad extrahieren  |
| `GetDeviceName(path)`    | Device/Bike-Ordnername extrahieren       |
| `GetRunName(path)`       | Run-Ordnername aus Pfad extrahieren      |
| `GetEventRootFolder(path)` | Event-Root-Ordner ermitteln            |

## Konfiguration

`oracle-config.json` enthält konfigurierbare Werte, die zuvor als Konstanten im Code standen:

```json
{
  "DatabaseDirectory": "C:\\KTM Tools\\...",
  "DatabaseFileName": "KOMPASS.json",
  "TeamMappings": {
    "01_KTM-FR": "KTM Factory Racing",
    "KTM-FR": "KTM Factory Racing"
  }
}
```

Falls die Datei fehlt, werden Standardwerte aus `OracleConfig.cs` verwendet.

## WinTAX4 Ordnerstruktur

```
.../Discipline/Season/.../Team/.../Event/Session/Device(Bike)/Run_xxx/
                                                                 ├── Lap_01/
                                                                 │   ├── cableData.ztx
                                                                 │   └── LapHeader.xml
                                                                 ├── Lap_02/
                                                                 │   ├── cableData.ztx
                                                                 │   └── LapHeader.xml
                                                                 ├── Session.xml
                                                                 └── Car.xml
```

- **Event/Session/Device/Run**: Standardmäßige WinTAX4-Ebenen → per `IMarelliDataBrowse` API auslesbar
- **Discipline/Season/Team**: KTM-spezifische Ebenen → per Pfad-Index extrahiert (`GetPathPart`)

## Legacy: XmlRunEditor

`XmlRunEditor.cs` wurde durch `RunPropertyEditor.cs` ersetzt:

| Alter Ansatz (XmlRunEditor)              | Neuer Ansatz (RunPropertyEditor)        |
|------------------------------------------|-----------------------------------------|
| Direkte XML-Manipulation                 | KtmDataHandler API                      |
| `Session.xml` → Track, TrackLen          | `AddRunProperty("Track", ...)` etc.     |
| `Car.xml` → Name, DriverName            | `AddRunProperty("Name", ...)` etc.      |
| `LapHeader.xml` → Lap, Alias, Run etc.  | `AddLapProperty(nLap, "Lap", ...)` etc. |

`XmlRunEditor` wird als Fallback-Implementierung beibehalten für Szenarien,
in denen die KtmDataHandler-API nicht verfügbar ist.
