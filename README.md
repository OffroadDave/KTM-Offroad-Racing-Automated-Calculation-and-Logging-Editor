# ORACLE — Nutzungsanleitung

**ORACLE** (KTM Offroad Racing Automated Calculation and Logging Editor) ist ein CLI-Tool zur
Nachbearbeitung von WinTAX4-/Marelli-Telemetrie aus Motocross-/Enduro-Runs. Es ermittelt Run-Metadaten,
erkennt Rennen, berechnet abgeleitete Kanäle (`sVehicle`, Lap-Distanz) und schreibt Run-/Lap-Properties
direkt in die WinTAX4-XML-Dateien — damit MotoDB sie anschließend sauber in die SQLite-DB einlesen kann.

> Architektur-Interna: siehe [`ORACLE/ARCHITECTURE.md`](ORACLE/ARCHITECTURE.md).
> Begleittool `SENTINEL` erzeugt `download.done`, wenn ein Run fertig synchronisiert ist (Trigger für KRAKEN).

---

## 1. Voraussetzungen

| Bereich | Benötigt für |
|---|---|
| **.NET 8 Runtime** | alle Commands |
| **KtmDataHandler-Suite** (`C:\KTM Tools\KTM Utils Software Suite`) + **HASP-Dongle** + **WinTAX/TelDataX-Runtime** | `process`, `validate`, `detect`, `--gps-split` (alles, was ZTX-Daten liest/schreibt) |
| **`KOMPASS.json`** (Stammdaten-DB) | Metadaten-Fallback, GPS-Ziellinie, Track-Untergrund |
| **`oracle-config.json`** (neben `ORACLE.exe`) | Pfade & Schwellenwerte |

> **`sync-motodb` benötigt KEINE native Suite und KEINEN Dongle** — es arbeitet nur mit `KOMPASS.json` + `loadChannels.xlsx` und läuft daher auf jeder Maschine.

---

## 2. Build & Start

```powershell
# Bauen
dotnet build "Ktm.Racing.slnx" -c Debug

# Veröffentlichen (eigenständige EXE für den Produktiveinsatz / KRAKEN)
dotnet publish "ORACLE/ORACLE.csproj" -c Release -o "C:\KTM Tools\ORACLE"

# Tests
dotnet test "Ktm.Racing.slnx" -c Debug
```

`ORACLE.exe` ohne Argumente gibt die Hilfe aus. `oracle-config.json` muss **im selben Ordner** wie die EXE liegen.

---

## 3. Run- vs. Rider-Pfad

Die WinTAX4-Ordnerstruktur ist: `Event / Session / Gerät(Rider) / Run_xxx / Lap_xxx`.

```
RT22_GER-Teutschenthal\        ← Event
└─ R1\                         ← Session
   └─ AA01\                    ← Gerät / Rider  ← "riderPath"
      ├─ Run_1078\             ← einzelner Run  ← "runPath"
      │  └─ Lap_00001102_00418\
      └─ Run_1079\
```

- **Rider-/Session-Pfad** (`…\R1\AA01`): ORACLE verarbeitet **alle** Runs renn-bewusst und führt ein
  fragmentiertes Rennen mit durchgehender Lap-Nummerierung & Distanz zusammen (Ordner bleiben unverändert).
- **Run-Pfad** (`…\Run_1079`): ORACLE verarbeitet genau diesen Run (KRAKEN-kompatibel, per-Run-Jobs).

`process` erkennt automatisch, welcher Fall vorliegt.

---

## 4. Commands

```text
ORACLE.exe process  <run|rider> [--force] [--gps-split]   Postprocessing (Metadaten, Calc, XML-Properties)
ORACLE.exe detect   <riderPath>                           Renn-Erkennung (read-only, verändert nichts)
ORACLE.exe validate <runPath>                             Datenintegritätsprüfung
ORACLE.exe sync-motodb [--kompass <p>] [--workbook <p>]   loadChannels.xlsx (CIRCUITS+HED) aus KOMPASS aktualisieren
ORACLE.exe <runPath>            [--force] [--gps-split]   Legacy-Modus (= process für einen Run)
```

### Flags

| Flag | Wirkung |
|---|---|
| `--force` | Idempotenz-Prüfung (`oracle.status.json`) ignorieren und erneut verarbeiten. |
| `--gps-split` | Runs anhand der KOMPASS-GPS-Ziellinie headless in Laps aufteilen (vor den Berechnungen). |
| `--kompass <Pfad>` | *(nur `sync-motodb`)* abweichende `KOMPASS.json`; sonst aus der Konfiguration. |
| `--workbook <Pfad>` | *(nur `sync-motodb`)* abweichende `loadChannels.xlsx`; sonst aus der Konfiguration. |

---

## 5. Typische Abläufe

```powershell
# Ganzen Rider-Ordner renn-bewusst verarbeiten (Normalfall am Track)
ORACLE.exe process "C:\MXTrackData\...\RT22_GER-Teutschenthal\R1\AA01"

# Einzelnen Run verarbeiten (so ruft KRAKEN ORACLE pro Run auf)
ORACLE.exe process "C:\MXTrackData\...\R1\AA01\Run_1079"

# Erneut verarbeiten, obwohl schon als erledigt markiert
ORACLE.exe process "C:\...\R1\AA01" --force

# Laps per GPS-Ziellinie neu setzen (benötigt KOMPASS Lat/Lon + Dongle/Runtime)
ORACLE.exe process "C:\...\R1\AA01" --gps-split

# Nur anschauen, welche Runs als Rennen erkannt würden (ändert nichts)
ORACLE.exe detect "C:\...\R1\AA01"

# Datenintegrität eines Runs prüfen
ORACLE.exe validate "C:\...\R1\AA01\Run_1079"

# MotoDB-Inputs aus KOMPASS aktualisieren (ohne Dongle, z.B. als KRAKEN-Vorstufe)
ORACLE.exe sync-motodb
```

---

## 6. Was `process` tut (in Reihenfolge)

1. **Idempotenz-Prüfung** — bereits erfolgreich verarbeitete Runs werden übersprungen (außer `--force`).
2. **Metadaten ermitteln** — aus der `.HED`-Datei im Run/Rider-Ordner, sonst aus `KOMPASS.json` (+ Pfad).
3. **`.HED` normalisieren** — in jedem Run eine kanonisch benannte `.HED` sicherstellen
   (`{Season}{Event}_{Bike}{Session}.HED`, ohne Leerzeichen, inkl. Sektion `[TRACK] surface = …`).
4. **(optional `--gps-split`)** — Runs anhand der GPS-Ziellinie in Laps aufteilen.
5. **Berechnen & schreiben** — `sVehicle` + per-Lap-Distanz; Lap/Alias/Marker/LapDistance, Track, Fahrer
   direkt in `LapHeader.xml` / `RunLapHeader.xml` / `Car.xml` / `Session.xml`.
6. **Status schreiben** — `oracle.status.json` im Geräteordner.

---

## 7. Exit-Codes (für KRAKEN)

| Code | Bedeutung | KRAKEN-Reaktion |
|---|---|---|
| `0` | Erfolg | Nächsten Job starten |
| `1` | Allgemeiner Fehler | Abbrechen, Logs prüfen |
| `2` | Ungültige Argumente | Job-Konfiguration prüfen |
| `3` | Daten nicht gefunden | **Retry** — Daten evtl. noch am Synchronisieren |
| `4` | Teilweise erfolgreich | Warnung loggen, ggf. weiter |
| `5` | Ressource gesperrt (z.B. `loadChannels.xlsx` offen in Excel) | **Retry** — Sperre ist i.d.R. transient |

---

## 8. Konfiguration — `oracle-config.json`

Fehlt die Datei, gelten die Standardwerte. JSON-Kommentare (`//`) sind erlaubt.

```jsonc
{
  // KOMPASS-Stammdaten (Metadaten-Fallback, GPS-Ziellinie, Untergrund)
  "DatabaseDirectory": "C:\\KTM Tools\\KTM Offroad Racing Shared Resources\\SoftwareDB",
  "DatabaseFileName": "KOMPASS.json",

  // Ziel für "sync-motodb"
  "MotoDbLoadChannelsPath": "C:\\KTM Tools\\KTM MotoDB\\loadChannels.xlsx",

  // Geräteordner "AA 01" -> "AA01" kanonisch umbenennen (bikeName ohne Leerzeichen)
  "NormalizeDeviceFolderName": true,

  // Renn-Erkennung (rein GPS/Gas-basiert; Drehzahl & GPS-Qualität nur als Guards)
  "RaceDetection": {
    "EngineOnMinRpm": 2000,
    "DistanceMinMeters": 1000,
    "TrackLengthFactor": 0.5,
    "SpeedMinKph": 25,
    "ThrottleMinPercent": 15,
    "ThrottleFallbackPercent": 20,
    "RpmRaceMin": 5000,
    "MinValidGpsSamples": 10
  },

  // GPS-Lap-Split (--gps-split)
  "GpsLapSplit": {
    "RadiusMeters": 25,          // Korridor um den Ziellinien-Punkt
    "MinLapSeconds": 20,         // Mindest-Rundenzeit (unterdrückt Doppel-Trigger)
    "IncludeTrailingPartialLap": true
  }
}
```

---

## 9. GPS-Split-Laps (`--gps-split`)

Teilt einen durchgehenden Run anhand einer **virtuellen GPS-Ziellinie** in Laps — headless, ohne WinTAX-GUI.

**Voraussetzungen:**
- Der Track muss in `KOMPASS.json` **`Latitude`/`Longitude`** gesetzt haben (die Ziellinie). Fehlt die
  Koordinate, wird der Split übersprungen (mit Log-Hinweis) — der Run wird normal weiterverarbeitet.
- Dongle + WinTAX/TelDataX-Runtime (der Schreibvorgang nutzt die native Write-API `SetChannelsSplitByLap`).
- Schwellen (`RadiusMeters`, `MinLapSeconds`) über `GpsLapSplit` in der Konfiguration.

**Empfehlung:** vor dem produktiven Einsatz erst auf einer **Kopie** eines Runs testen.

---

## 10. `sync-motodb` — MotoDB-Inputs aktuell halten

Regeneriert die KTM-eigenen Sheets **CIRCUITS** und **HED** in `loadChannels.xlsx` aus `KOMPASS.json`
(Upsert nach Track-Name; manuell gepflegte Spalten und MotoDB-Kanalsheets bleiben erhalten).

```powershell
ORACLE.exe sync-motodb
ORACLE.exe sync-motodb --workbook "D:\Test\loadChannels.xlsx"
```

Ideal als **KRAKEN-Vorstufe** vor `MotoDB.exe`, damit die Inputs vor jedem Lauf frisch sind. Ist das
Workbook gerade in Excel offen, liefert der Befehl **Exit-Code 5** → KRAKEN sollte es erneut versuchen,
statt mit veralteten Inputs weiterzulaufen.

---

## 11. Idempotenz

Nach erfolgreicher Verarbeitung schreibt ORACLE `oracle.status.json` in den **Geräteordner**
(Elternordner der Runs, z.B. `…\AA01`). Beim nächsten Aufruf werden bereits verarbeitete Runs
übersprungen (Exit-Code `0`). Mit `--force` wird die Prüfung ignoriert und erneut verarbeitet.

---

## 12. Logging

Jeder Lauf schreibt zusätzlich zur Konsole eine Logdatei: `{AppDir}/logs/oracle-{yyyyMMdd-HHmmss}.log`.
Praktisch, wenn ORACLE als KRAKEN-Job läuft und die Konsolenausgabe verloren geht.

---

## 13. Einordnung in die Toolchain

```
Logger am Bike ─► OneDrive/Workstation ─► SENTINEL (download.done)
                                               │
                                               ▼
                                            KRAKEN
                        ┌──────────────────────┼───────────────────────┐
                        ▼                       ▼                       ▼
             ORACLE.exe sync-motodb   ORACLE.exe process <run>     MotoDB.exe
             (CIRCUITS+HED aktuell)   (Metadaten/Calc/XML)         (SQLite-DB) ─► Tableau
```

---

## 14. Fehlerbehebung

| Symptom | Ursache / Lösung |
|---|---|
| `Metadaten konnten nicht ermittelt werden` | Keine `.HED` **und** `KOMPASS.json` nicht gefunden/leer → Pfade in `oracle-config.json` prüfen. |
| `[gps-split] Übersprungen: keine Ziellinie …` | Track in KOMPASS hat keine `Latitude`/`Longitude` → in KODE pflegen. |
| Exit-Code `5` bei `sync-motodb` | `loadChannels.xlsx` ist in Excel geöffnet → schließen und erneut ausführen. |
| `Access denied` beim Umbenennen eines Geräteordners | Ordner liegt in einem offenen VS-Code-Workspace o.ä. (Datei-Lock); non-fatal — oder `NormalizeDeviceFolderName: false`. |
| Run wird „übersprungen", obwohl gewünscht | `oracle.status.json` vorhanden → mit `--force` erzwingen. |
| Crash beim Laden nativer DLLs | KtmDataHandler-Suite/Dongle/WinTAX-Runtime fehlt → für reine Input-Syncs stattdessen `sync-motodb` (ohne native Abhängigkeit) nutzen. |
