# KODE ↔ ORACLE: KOMPASS-Datenbankstruktur

## Überblick

**KODE** (*KTM Offroad Database Editor*) ist eine WinForms-GUI zum Erstellen und Pflegen der **KOMPASS.json**-Datenbank. **ORACLE** liest diese Datenbank über `KompassService`, um Metadaten (Rider, Track, Tracklänge) für die Telemetrie-Verarbeitung aufzulösen.

```
KODE (GUI)                              ORACLE (CLI)
┌───────────────────────┐               ┌───────────────────────┐
│  MainKODE.cs          │               │  KompassService.cs    │
│  ├── TracksKODE.cs    │  KOMPASS.json │  ├── LoadAsync()      │
│  ├── RidersKODE.cs    │ ────────────► │  ├── BuildMetadata()  │
│  └── BuildKOMPASS     │               │  └── LookupTrack/     │
│      Database()       │               │      Rider()          │
└───────────────────────┘               └───────────────────────┘
         │                                        │
         ▼                                        ▼
  SaveFileDialog →                     oracle-config.json →
  KOMPASS.json                         DatabaseDirectory +
  + OneDrive Backup                    DatabaseFileName
```

## KOMPASS.json Schema

```json
{
  "MX": {
    "Tracks": [
      {
        "Country": "Österreich",
        "Countrycode": "AT",
        "Track": "RedBull",
        "Tracklength": "1650",
        "Latitude": "47.8167",
        "Longitude": "13.0333"
      }
    ],
    "Riders": [
      {
        "Rider": "Max Mustermann",
        "Riderabbreviation": "MM",
        "Season": "2026",
        "Team": "KTM Factory Racing"
      }
    ]
  },
  "SX": { "Tracks": [...], "Riders": [...] },
  "Enduro": { "Tracks": [...], "Riders": [...] },
  "Rally": { "Tracks": [...], "Riders": [...] }
}
```

### Top-Level: Disziplinen

| Key | Beschreibung |
|-----|-------------|
| `MX` | Motocross |
| `SX` | Supercross |
| `Enduro` | Enduro |
| `Rally` | Rally |

KODE definiert die Disziplinen in `MainKODE.disciplines`:
```csharp
public static readonly List<string> disciplines = new() { "MX", "SX", "Enduro", "Rally" };
```

### TrackInfo (KODE → KOMPASS → ORACLE)

| JSON-Property | KODE DataGridView Column | ORACLE C#-Klasse | Verwendung in ORACLE |
|---------------|-------------------------|-------------------|---------------------|
| `Country` | `Country` | `TrackInfo.Country` | `RunMetadata.CountryName` |
| `Countrycode` | `Countrycode` | `TrackInfo.Countrycode` | Lookup-Key (mit `metadata.CountryCode`) |
| `Track` | `Track` | `TrackInfo.Track` | Lookup-Key (mit `metadata.TrackName`) |
| `Tracklength` | `Tracklength` | `TrackInfo.trackLength` | `RunMetadata.TrackLength` |
| `Latitude` | `LatFinish` | `TrackInfo.Latitude` | GPS-Koordinate Ziel (für SVehicleCalc) |
| `Longitude` | `LongFinish` | `TrackInfo.Longitude` | GPS-Koordinate Ziel (für SVehicleCalc) |

### RiderInfo (KODE → KOMPASS → ORACLE)

| JSON-Property | KODE DataGridView Column | ORACLE C#-Klasse | Verwendung in ORACLE |
|---------------|-------------------------|-------------------|---------------------|
| `Rider` | `Rider` | `RiderInfo.Rider` | `RunMetadata.RiderName` |
| `Riderabbreviation` | `Riderabbreviation` | `RiderInfo.Riderabbreviation` | Lookup-Key (mit `metadata.RiderAbbreviation`) |
| `Season` | `Season` | `RiderInfo.Season` | Lookup-Key (mit `metadata.SeasonName`) |
| `Team` | `Team` | `RiderInfo.Team` | Lookup-Key (mit `metadata.TeamName`) |

## Lookup-Logik in ORACLE

`KompassService.LookupTrackAndRider()` verwendet einen 3-stufigen Abgleich:

### Track-Lookup
```
Eingabe:   metadata.CountryCode (aus EventName, z.B. "AT")
           metadata.TrackName   (aus EventName, z.B. "RedBull")
Suche:     KOMPASS[discipline].Tracks.FirstOrDefault(t =>
             t.Countrycode == "AT" && t.Track == "RedBull")
Ergebnis:  metadata.TrackLength = "1650"
           metadata.CountryName = "Österreich"
```

### Rider-Lookup
```
Eingabe:   metadata.RiderAbbreviation (aus DeviceName, z.B. "MM")
           metadata.SeasonName        (aus Pfad-Index, z.B. "2026")
           metadata.TeamName          (aus TeamMappings, z.B. "KTM Factory Racing")
Suche:     KOMPASS[discipline].Riders.FirstOrDefault(r =>
             r.Riderabbreviation == "MM" &&
             r.Season == "2026" &&
             r.Team == "KTM Factory Racing")
Ergebnis:  metadata.RiderName = "Max Mustermann"
```

## Dateipfade

| Kontext | Pfad |
|---------|------|
| KODE Speicherort 1 | Vom Benutzer gewählt (SaveFileDialog) |
| KODE Speicherort 2 (Backup) | `C:\Users\{user}\OneDrive - PIERER Mobility AG\Datenbank\KOMPASS.json` |
| KODE Speicherort 3 (ORACLE) | `C:\KTM Tools\KTM Offroad Racing Shared Resources\SoftwareDB\KOMPASS.json` *(auto, falls Verzeichnis existiert)* |
| ORACLE Lesepfad (Default) | `C:\KTM Tools\KTM Offroad Racing Shared Resources\SoftwareDB\KOMPASS.json` |
| ORACLE Config | `oracle-config.json` → `DatabaseDirectory` + `DatabaseFileName` |

## Datenqualitätssicherung

KODE validiert vor dem Speichern automatisch:
- **Pflichtfelder**: Track-Name, Countrycode, Fahrername, Abkürzung
- **Duplikate**: Tracks (Disziplin + Countrycode + Track) und Riders (Disziplin + Abkürzung + Season + Team)
- **Countrycode Auto-Fill**: Wird automatisch aus der Country-Auswahl gesetzt (verhindert Inkonsistenzen)
- **Dynamische ComboBoxes**: Season/Team-Werte werden aus bestehenden KOMPASS-Daten befüllt

## Solution-Struktur

KODE ist als **externes Referenzprojekt** in `ORACLE.sln` eingebunden (Solution Folder `External`).
Es gibt keine Code-Abhängigkeit (`<ProjectReference>`) — KODE dient rein als Kontext für die
KOMPASS-Datenbankstruktur.

```
ORACLE.sln
├── ORACLE\ORACLE.csproj        ← Postprocessing CLI
├── SENTINEL\SENTINEL.csproj    ← Download-Watcher
└── External\
    └── KODE\KODE.csproj        ← KOMPASS DB Editor (Referenz)
```
