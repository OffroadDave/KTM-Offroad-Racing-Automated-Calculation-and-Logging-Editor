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
| **KtmDataHandler-Suite** (`C:\KTM Tools\KTM Utils Software Suite`) + **HASP-Dongle** + **WinTAX/TelDataX-Runtime** | `process`, `validate`, `detect`, `split-laps`/`--gps-split` (alles, was ZTX-Daten liest/schreibt) |
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
  Enthält der Pfad **mehrere Geräte** (z.B. Session `BC` mit `AA01` und `AA02`), wird jedes Gerät
  eigenständig verarbeitet – mit eigenen Metadaten (Bike/Rider), eigener `.HED` und eigener `oracle.status.json`.
- **Run-Pfad** (`…\Run_1079`): ORACLE verarbeitet genau diesen Run (KRAKEN-kompatibel, per-Run-Jobs).

`process` erkennt automatisch, welcher Fall vorliegt. Vor der Verarbeitung **repariert** ORACLE Runs, die WinTAX wegen
verwaister Lap-Ordner verwirft (Reste eines Lap-Splits/Appends: ohne `cableData.ztx` und nicht in `RunLapHeader.xml`
referenziert) – diese Ordner werden nach `Run_x\_ORACLE_removed` verschoben (nichts wird gelöscht, `README.txt`
dokumentiert es; abschaltbar mit `RepairOrphanLapFolders: false`). **Leere Runs** (Logger ohne Aufzeichnung, keine
Lap-Ordner) erhalten nur die Metadaten (`.HED`, `Car.xml`, `Session.xml`) und gelten nicht als Fehler. Was danach
immer noch nicht verarbeitbar ist, wird **mit Ursache** im Log und im Status (`discovery.skippedRuns`) gemeldet; das
Ergebnis ist dann Exit-Code `4` (bzw. `7`, wenn kein Run verarbeitbar ist).

Ein Ordner wird während der Verarbeitung exklusiv gesperrt (`oracle.lock`); ein zweiter ORACLE-Prozess auf
demselben Ordner endet mit Exit-Code `5` (später erneut). Ein defekter Run bricht die übrigen Runs des Geräts
nicht ab (Status `stint.failedRuns`, Exit-Code `4`).

---

## 4. Commands

```text
ORACLE.exe process  <run|rider> [--force] [--gps-split]   Postprocessing (Metadaten, Calc, XML-Properties)
ORACLE.exe split-laps <event|session|device|run> [--dry-run] [--undo]   Runs per GPS-Ziellinie in Laps teilen (Abschnitt 9)
ORACLE.exe detect   <riderPath>                           Renn-Erkennung (read-only, verändert nichts)
ORACLE.exe detect-start <riderPath|runPath>               Gültige Starts erkennen (read-only, siehe docs/vch/ValidStart.md)
ORACLE.exe validate <runPath>                             Datenintegritätsprüfung
ORACLE.exe sync-motodb [--kompass <p>] [--workbook <p>]   loadChannels.xlsx (CIRCUITS+HED) aus KOMPASS aktualisieren
ORACLE.exe <runPath>            [--force] [--gps-split]   Legacy-Modus (= process für einen Run)
```

### Flags

| Flag | Wirkung |
|---|---|
| `--force` | Idempotenz-Prüfung (`oracle.status.json`) ignorieren und erneut verarbeiten. |
| `--gps-split` | Runs vor den Berechnungen per GPS-Ziellinie in Laps teilen (wie `split-laps`, Abschnitt 9). |
| `--dry-run` | *(nur `split-laps`)* nur anzeigen, welche Runs wie geteilt würden – schreibt nichts. |
| `--undo` | *(nur `split-laps`)* Original-Laps der letzten ORACLE-Teilung wiederherstellen. |
| `--kompass <Pfad>` | *(nur `sync-motodb`)* abweichende `KOMPASS.json`; sonst aus der Konfiguration. |
| `--workbook <Pfad>` | *(nur `sync-motodb`)* abweichende `loadChannels.xlsx`; sonst aus der Konfiguration. |

Unbekannte Optionen und überzählige Argumente (z.B. ein Pfad mit Leerzeichen ohne Anführungszeichen) werden
abgelehnt (Exit-Code `2`).

---

## 5. Typische Abläufe

```powershell
# Ganzen Rider-Ordner renn-bewusst verarbeiten (Normalfall am Track)
ORACLE.exe process "C:\MXTrackData\...\RT22_GER-Teutschenthal\R1\AA01"

# Einzelnen Run verarbeiten (so ruft KRAKEN ORACLE pro Run auf)
ORACLE.exe process "C:\MXTrackData\...\R1\AA01\Run_1079"

# Erneut verarbeiten, obwohl schon als erledigt markiert
ORACLE.exe process "C:\...\R1\AA01" --force

# Laps per GPS-Ziellinie setzen: erst Probelauf, dann teilen (benötigt KOMPASS Lat/Lon + Dongle/Runtime)
ORACLE.exe split-laps "C:\...\RT37_CHN-Shanghai" --dry-run
ORACLE.exe split-laps "C:\...\RT37_CHN-Shanghai"
ORACLE.exe process "C:\...\R1\AA01" --gps-split          # oder direkt in der Verarbeitung

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
   Team, Saison, Disziplin und Motorklasse werden am **Muster** des Pfads erkannt (Team = Schlüssel aus
   `TeamMappings`, Saison = vierstelliges Jahr, Motorklasse = 125…500; Disziplin = Pfadsegment mit einem
   KOMPASS-Disziplinnamen, sonst die Disziplin, die den Track enthält), unabhängig von der Ordnertiefe.
   Der Rider wird über Kürzel + Saison + Team gesucht (eindeutig auch ohne Team). **KOMPASS-Stammdaten gewinnen:**
   Weicht eine vorhandene `.HED` ab (z.B. `riderName = Test Rider`, KOMPASS kennt AA/2026 als *Andrea Adamo*), werden
   Rider, Team, Saison, Disziplin, Motorklasse, Kürzel und Ländername aus KOMPASS/Pfad übernommen und in `.HED` und
   `Car.xml` geschrieben (Log `[metadata] riderName: .HED "…" → "…"`). Track, Ländercode, Typ und KW aus dem
   Event-Namen werden nur ergänzt, wenn sie fehlen. Kennt KOMPASS den Rider nicht, bleibt der `.HED`-Wert. Für bewusst
   von Hand gepflegte `.HED`-Dateien (z.B. Ersatzfahrer): `PreferHedMetadata: true` – dann wird nur gewarnt. Ohne
   Rider/Track ist der Status-Schritt `metadata` = `partial`.
3. **`.HED` normalisieren** — in jedem Run eine kanonisch benannte `.HED` sicherstellen
   (`{Season}{Event}_{Bike}{Session}.HED`, ohne Leerzeichen, inkl. Sektion `[TRACK] surface = …`).
4. **(optional `--gps-split`)** — Runs per GPS-Ziellinie in Laps teilen (Abschnitt 9); bereits geteilte Runs bleiben unverändert.
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
| `4` | Teilweise erfolgreich (z.B. beschädigte `Run_*`-Ordner übersprungen, siehe Log/Status `discovery`) | Warnung loggen, MotoDB starten; Runs in WinTAX reparieren, dann `--force` |
| `5` | Ressource gesperrt (z.B. `loadChannels.xlsx` offen in Excel) | **Retry** — Sperre ist i.d.R. transient |
| `6` | Track-Länge fehlt (keine eindeutige Länge, auch keine Schätzung – auch nicht aus bereits verarbeiteten Runs des Events) | **MotoDB NICHT starten**; **später erneut** (nachdem längere Runs des Events verarbeitet sind) oder Länge in KODE eintragen |
| `7` | Ungültige Daten (beschädigter Run-Ordner, ungültige `KOMPASS.json`) | **Kein Retry** — Daten reparieren (WinTAX / KODE), dann erneut ausführen |

---

## 8. Konfiguration — `oracle-config.json`

Fehlt die Datei, gelten die Standardwerte. JSON-Kommentare (`//`) sind erlaubt.

```jsonc
{
  // KOMPASS-Stammdaten (Metadaten-Fallback, GPS-Ziellinie, Untergrund)
  "DatabaseDirectory": "C:\\KTM Tools\\KTM Offroad Racing Shared Resources\\DistributedTools\\resources",
  "DatabaseFileName": "KOMPASS.json",

  // Ziel für "sync-motodb"
  "MotoDbLoadChannelsPath": "C:\\KTM Tools\\KTM MotoDB\\loadChannels.xlsx",

  // Geräteordner "AA 01" -> "AA01" kanonisch umbenennen (bikeName ohne Leerzeichen)
  "NormalizeDeviceFolderName": true,

  // Verwaiste, leere Lap-Ordner (Lap-Split-Reste) nach Run_x\_ORACLE_removed verschieben (nichts wird gelöscht)
  "RepairOrphanLapFolders": true,

  // true = vorhandene .HED-Werte (Rider, Team, …) haben Vorrang vor KOMPASS (nur warnen); Default false = KOMPASS gewinnt
  "PreferHedMetadata": false,

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

  // GPS-Kanäle und Aufbereitung für alle Berechnungen (Abschnitt 9a)
  "Gps": {
    "LatitudeChannel": "Lat_dez",     // exakte Kanalnamen (Groß/Klein egal), kein Teilstring-Fallback
    "LongitudeChannel": "Lon_dez",
    "MaxSpeedKph": 200,               // schnellerer Schritt = GPS-Sprung …
    "MinJumpMeters": 10,              // … ab 10 m (kleinere Schritte sind Zeitstempel-Rauschen)
    "MinSegmentSeconds": 2            // Fragmente < 2 s zwischen zwei Sprüngen = Ausreißer
  },

  // GPS-Lap-Split (split-laps, --gps-split; Abschnitt 9)
  "GpsLapSplit": {
    "MinLapSeconds": 20,              // Mindestzeit zwischen zwei Lap-Grenzen (Doppel-Trigger, Hin-und-her)
    "MaxSampleGapSeconds": 1,         // GPS-Lücke: Samples mit größerem Abstand werden nicht verbunden
    "MinCrossingAngleDegrees": 30,    // schleifende Durchfahrten (flacher als 30° zur Linie) zählen nicht
    "MinSpeedKph": 5,                 // Schieben/Fahrerlager zählt nicht
    "SnapRadiusMeters": 50,           // Durchfahrten in diesem Umkreis um den KOMPASS-Punkt legen die Linie fest
    "LineHalfWidthMeters": 12,        // Linie reicht 12 m zu jeder Seite der Ideallinie (parallele Geraden bleiben außen)
    "BoundaryToleranceSeconds": 2,    // vorhandene Lap-Grenze gilt als "auf der Linie" bei Durchfahrt ±2 s …
    "BoundaryDistanceMeters": 15,     // … oder Position ≤ 15 m von der Linie
    "FreeSpaceFactor": 1.2,           // freier Speicher vor dem Schreiben: 1,2 × Lap-Daten des Runs
    "MaxTimeShiftSamples": 1          // max. Kanal-Versatz durch das Neuschreiben (in Abtastperioden), sonst Abbruch
  },

  // Track-Längen-Prüfung vor MotoDB (siehe Abschnitt 10a)
  "TrackLength": {
    "Enforce": true,             // ohne eindeutige Länge Exit-Code 6
    "SuggestionsDirectory": "%OneDriveCommercial%\\Workstation - OffroadSync\\DevProjects\\KTM Internal Software Tools\\resources\\suggestions",
    "ExcludeFirstAndLastLap": true,
    "MinLapMeters": 300,
    "MaxLapMeters": 6000,
    "ClusterTolerance": 0.07,    // Runden ±7 % um den Median zählen
    "MinLaps": 3,
    "MaxRelativeSpread": 0.03,   // max. 3 % Streuung (IQR/Median)
    "RoundToMeters": 1,          // Median in ganzen Metern
    "LockWaitSeconds": 120       // so lange auf eine gesperrte loadChannels.xlsx warten (dann Exit-Code 5)
  }
}
```

---

## 9. GPS-Split-Laps (`split-laps`, `--gps-split`)

Teilt Runs anhand einer **GPS-Ziellinie** in Laps — headless, ohne WinTAX-GUI, mit Sicherung und Rückgängig.

```powershell
ORACLE.exe split-laps "C:\...\RT37_CHN-Shanghai" --dry-run   # zeigt je Run: Erkennung + geplante Lap-Grenzen
ORACLE.exe split-laps "C:\...\RT37_CHN-Shanghai"             # teilt (Event-, Session-, Geräte- oder Run-Ordner)
ORACLE.exe split-laps "C:\...\R1\AA01\Run_885" --undo        # stellt die Original-Laps wieder her
```

**Ablauf je Track** (alle Runs unter dem Pfad, gruppiert nach Event-Ordner `RTnn_CC-Track`):

1. **Lesen** — Lap-Grenzen, Lap-Ordner (Reihenfolge aus `RunLapHeader.xml`, Akquisition = `Abs` aus `LapHeader.xml`)
   und GPS (`Lat_dez`/`Lon_dez`, exakte Namen) aller Runs.
2. **Ziellinie** — aus der KOMPASS-Koordinate des Tracks (Name schreibweisen-tolerant, Land bevorzugt). Der Punkt wird
   auf die Ideallinie gelegt (Median-Fußpunkt aller Durchfahrten im Umkreis von 50 m, nächstgelegene Gerade gewinnt) und
   die Linie quer zur Fahrachse gelegt (±12 m). Die **Fahrtrichtung spielt keine Rolle**. Eine Linie für alle Sessions;
   wird sie nur aus einer Session gebildet, weicht sie um < 0,2 m ab (Shanghai: ≤ 0,03 s). Liegt keine Durchfahrt in der
   Nähe, wird mit Hinweis übersprungen (`KOMPASS-Punkt … m entfernt – Lat/Lon prüfen`).
3. **Bereits geteilt?** — vor jedem Schreiben entscheidet die Erkennung je Run:

   | Befund | Entscheidung |
   |---|---|
   | eine durchgehende Lap | **teilen** |
   | Lap-Ordner aus einem WinTAX-/ORACLE-Split (`_Def_`/`_App_`) oder mehrere Laps derselben Akquisition | **nicht teilen** (bereits geteilt) |
   | nur Akquisitions-Grenzen, die meisten liegen auf der Ziellinie (Durchfahrt ±2 s oder ≤ 15 m) oder alle an einer Stelle (Bake) | **nicht teilen** (Logger-Runden) |
   | Akquisitions-Grenzen abseits der Linie (Logger hat mitten auf der Strecke neu begonnen) | **teilen** (Segmente werden zusammengeführt) |
   | keine GPS-Position an den Grenzen / gemischtes Bild | **nicht teilen**, Warnung |

   Damit ist der Split **idempotent**: ein zweiter Lauf (auch mit `--force`) erkennt die eigenen `_Def_`-Laps.
4. **Durchfahrten** — Schnitt jeder GPS-Strecke zwischen zwei Fixes mit der Linie, zeitlich interpoliert (≈ 0,02 s).
   Mehrfach geloggte Fixes (10-Hz-GPS in 50-Hz-Kanal) werden zusammengefasst. Ignoriert werden: schleifende Durchfahrten
   (< 30°), < 5 km/h, GPS-Lücken > 1 s / Sprünge > 60 m, eine zweite Durchfahrt innerhalb `MinLapSeconds`.
   Die Lap-Grenzen sind immer `[Run-Anfang, Durchfahrt 1, …, Durchfahrt n, Run-Ende]` — **keine Daten gehen verloren**.
5. **Schreiben (transaktional)** — Speicherplatz prüfen → alle Kanäle in `Run_x\_ORACLE_split_tmp` schreiben (WinTAX-API
   `SetChannelsSplitByLap` mit Lap-Startoffset) → zurücklesen und prüfen (Lap-Grenzen, Kanalzahl, Samples, Versatz) →
   Original-Laps + `RunLapHeader.xml` nach `Run_x\_ORACLE_original\<Zeitstempel>` **verschieben** → neue Laps als
   `<Original-Lap>_Def_000…` einsetzen, vollständige `LapHeader.xml` (Alias, Tabelle, Akquisition, Startzeit, Lap-Zeit) und
   `RunLapHeader.xml` schreiben, fehlende Lap-Dateien (`BitmappedChannels.xml`, `EnumChannels.xml`) übernehmen →
   Run erneut lesen. Jeder Fehler → **Rollback**, der Run bleibt unverändert; `_ORACLE_split_tmp` wird immer entfernt.
   `split-manifest.json` im Sicherungsordner protokolliert Lap-Grenzen, Ziellinie, Original- und neue Laps.

**Datentreue:** Werte bleiben erhalten (Shanghai: 99,85–100 % aller Samples identisch und in gleicher Reihenfolge). Die
WinTAX-API legt jedoch jeden Kanal beim Schreiben auf sein Abtastraster: Versatz < 1 Abtastperiode (GPS/100 Hz 9 ms,
1 kHz 0 ms, 1-Hz-Kanäle bis ~0,9 s) und je Lap-Grenze ±1 Sample. Größere Abweichungen brechen ab (`MaxTimeShiftSamples`).
Für Rundenzeiten, Distanzen und Lap-Statistiken ist das ohne Bedeutung; wer Original-Rohdaten braucht, findet sie unverändert
im Sicherungsordner (`--undo` stellt sie byte-identisch wieder her).

**Voraussetzungen:** KOMPASS-Track mit `Latitude`/`Longitude` (in KODE pflegen), Dongle + WinTAX/TelDataX-Runtime.
Das Kommando sperrt die Geräteordner (`oracle.lock`), parallele ORACLE-Jobs warten bzw. überspringen.
Bitte einen geteilten Run einmal in der WinTAX-GUI öffnen und prüfen, bevor der Split produktiv für alle Events läuft.

---

## 9a. GPS-Aufbereitung (`Lat_dez`/`Lon_dez`)

Alle GPS-Berechnungen (sVehicle, Lap-Distanzen → Track-Länge, Renn-Erkennung, Ziellinie und Durchfahrten) nutzen dieselbe
Aufbereitung (`GpsCleaner`, Abschnitt `Gps` der Konfiguration):

| Schritt | WinTAX-VCH `Lat_Valid`/`Lon_Valid` | ORACLE |
|---|---|---|
| Samples ohne Fix (`0`, NaN, außerhalb ±90/±180) entfernen | ✔ | ✔ |
| Über Aussetzer gerade verbinden | ✔ (`removenorx`) | ✔ |
| Wiederholte Fixes zusammenfassen (GPS ~10–25 Hz im 50-Hz-Kanal; Zeit = erstes Auftreten) | – | ✔ |
| GPS-Sprünge (≥ 10 m und > 200 km/h) nicht mitzählen, Fragmente < 2 s dazwischen verwerfen | – | ✔ |

Hintergrund: In 26 von 178 Runs (Shanghai, Teutschenthal) springt die Position kurz nach dem ersten Fix um 17–254 m.
Ohne Filter landen diese Sprünge in sVehicle, in der Distanz der ersten Lap und in der Renn-Erkennung. Ohne GPS-Fix sind
sVehicle 0 m, und es wird keine Lap-Distanz geschrieben (Log `[gps] No GPS fix …`); Aufbereitungen stehen im Log als `[gps] Cleaned: …`.
Die Rohkanäle werden nicht verändert; sVehicle liegt weiter auf dem Raster von `Lat_dez`. Lap-Distanzen ergeben sich aus
sVehicle an den Lap-Grenzen (Summe der Laps = Run-Distanz).

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

Schreibweisen-Varianten eines KOMPASS-Tracks (z.B. `Ernée` / `Ernee`) werden zu **einer** Zeile mit der
KOMPASS-Schreibweise zusammengeführt (alle gefüllten Zellen bleiben erhalten). Längen, die in KOMPASS bei
mehreren Tracks stehen, werden **nicht** geschrieben (MotoDB könnte die Strecken sonst nicht unterscheiden)
und als Warnung ausgegeben.

---

## 10a. Track-Länge vor MotoDB (`process`)

MotoDB erkennt eine Strecke, indem es `trackLength` aus der `.HED` **exakt** mit der Spalte `Length` im
Sheet CIRCUITS vergleicht. Deshalb braucht jeder Track eine Länge, und **zwei Tracks dürfen nie dieselbe
Länge haben**. `process` prüft das nach den Berechnungen (`TrackLengthGate`):

1. **KOMPASS** (bestätigte Länge) →
2. **CIRCUITS** (`loadChannels.xlsx`) →
3. **`.HED`** →
4. **offener Vorschlag** (bereits von einer anderen Maschine gemessen) →
5. **Schätzung aus den GPS-Rundendistanzen**: Out-/In-Lap und Ausreißer (Box, Abkürzung) werden verworfen,
   der Median von mind. 3 übereinstimmenden Runden in ganzen Metern (`RoundToMeters: 1`). Ist die Länge schon von einem
   anderen Track belegt, wird die nächste freie Länge (±1 m, ±2 m, …) verwendet.

Jede Quelle zählt nur, wenn **kein anderer Track** (KOMPASS oder CIRCUITS) dieselbe Länge hat.
Die verwendete Länge wird in `.HED`, `Session.xml` (`TrackLen`) und CIRCUITS geschrieben. Vorläufige Längen
(alles außer KOMPASS) landen zusätzlich als Vorschlag im Ordner `suggestions` des Team-Masters und werden
in KODE bestätigt (*More → Track length suggestions…*). Ohne eindeutige Länge endet `process` mit
**Exit-Code 6** — KRAKEN startet MotoDB dann nicht.

Reichen die Runden des verarbeiteten Runs nicht (z.B. kurzer Warm-up-Run im KRAKEN-Einzel-Run-Modus), nutzt
die Schätzung zusätzlich die `LapDistance`-Werte bereits verarbeiteter Runs **desselben Events** (alle Sessions
und Geräte). Ein erneuter Aufruf nach der Verarbeitung eines längeren Runs liefert daher die Länge.

Bereits verarbeitete Runs (Idempotenz-Skip) werden ebenfalls geprüft: Ist die Länge inzwischen bekannt
(z.B. in KODE bestätigt), werden `.HED`/`Session.xml`/CIRCUITS (und ein geänderter Untergrund in der `.HED`)
nachgezogen und die Korrektur im Status vermerkt (`trackLength.correctedFrom`, `correctedAtUtc`). Endete die
Verarbeitung mit Exit-Code `5`/`6` (nur die Länge fehlte), wird beim nächsten Aufruf **nur die Länge nachgeholt** –
aus den bereits geschriebenen Rundendistanzen der eigenen Runs und des Events, ohne Vollverarbeitung. Status-Schritt
`trackLength` in `oracle.status.json` (`lengthMeters`, `source`, `provisional`). Im Log erscheinen nur
Längenkonflikte, die den eigenen Track betreffen; Konflikte anderer Tracks meldet `sync-motodb`.

**In KODE abgelehnte Vorschläge** (`suggestions\rejected`) gelten nicht mehr: ORACLE ignoriert die abgelehnte Länge
auch dann, wenn sie bereits vorläufig in CIRCUITS oder `.HED` steht, und misst neu. Ergibt die Messung wieder die
abgelehnte Länge, endet `process` mit Exit-Code `6` (Länge in KODE eintragen).

**Gesperrte `loadChannels.xlsx`** (z.B. in Excel offen): ORACLE wartet bis `TrackLength.LockWaitSeconds` (Default
120 s) und versucht es erneut, bevor es mit Exit-Code `5` aufgibt. Schreibzugriffe paralleler ORACLE-Prozesse auf
das Workbook werden über einen systemweiten Mutex serialisiert.

---

## 11. Idempotenz

Nach erfolgreicher Verarbeitung schreibt ORACLE `oracle.status.json` in den **Geräteordner**
(Elternordner der Runs, z.B. `…\AA01`). Beim nächsten Aufruf werden bereits verarbeitete Runs
übersprungen (Exit-Code `0`, bzw. `4`, wenn beschädigte Run-Ordner weiterhin übersprungen werden).
Mit `--force` wird die Prüfung ignoriert und erneut verarbeitet.

Der Status enthält die **Verarbeitungsrevision** (`processingRevision`). Wurde ein Ordner mit einer älteren ORACLE-
Logik verarbeitet (z.B. vor der Metadaten-Korrektur oder der Run-Reparatur), wird er beim nächsten Aufruf
automatisch neu verarbeitet – ohne `--force`. Die Revision steht in `OracleStatus.CurrentProcessingRevision` und
wird erhöht, wenn eine Änderung bereits verarbeitete Daten verbessert.

---

## 12. Logging und Nachvollziehbarkeit

Jeder Lauf schreibt zusätzlich zur Konsole eine Logdatei: `{AppDir}/logs/oracle-{yyyyMMdd-HHmmss}-{PID}.log`.
Praktisch, wenn ORACLE als KRAKEN-Job läuft und die Konsolenausgabe verloren geht. Logdateien älter als 60 Tage
werden beim Start automatisch gelöscht. Runs ohne GPS-Fix erzeugen eine einzige Zeile `[gps] No GPS fix …`. Alle Konsolen- und Logmeldungen sind englisch.

Der verwendete **Datenstand** wird protokolliert (Log `[inputs]` und Status-Schritt `inputs`):
`KOMPASS.json` und `loadChannels.xlsx` mit Zeitstempel, Größe, SHA-256 (gekürzt) und – aus dem KODE-Sidecar
`KOMPASS.meta.json` – wer KOMPASS wann gespeichert hat. Welche Felder in KODE geändert wurden, steht im
gemeinsamen Änderungsprotokoll `KOMPASS.changes.log` neben dem Team-Master.

> **Deployment:** Auf verwalteten PCs kann die Anwendungssteuerung `.exe`-Dateien außerhalb freigegebener
> Ordner blockieren („Zugriff verweigert"). ORACLE daher im freigegebenen Installationsordner
> (z.B. `C:\KTM Tools\ORACLE`) betreiben; zum Testen alternativ `dotnet ORACLE.dll <Argumente>`.

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
| `Metadata could not be determined.` | Keine `.HED` **und** `KOMPASS.json` nicht gefunden/leer → Pfade in `oracle-config.json` prüfen. |
| `[gps-split] Skipped: no coordinate in KOMPASS …` | Track in KOMPASS hat keine `Latitude`/`Longitude` → in KODE pflegen. |
| `[gps-split] No finish line: No GPS pass within 50 m …` | KOMPASS-Punkt liegt neben der Strecke (Abstand steht im Log) → Koordinate in KODE korrigieren. |
| `[gps-split] … unclear lap structure …` | Run hat Akquisitions-Grenzen ohne GPS-Position → nicht automatisch geteilt; in WinTAX prüfen. |
| `[gps-split] ❌ … – run unchanged` | Schreiben/Prüfen fehlgeschlagen, Rollback erfolgt (Ursache im Log, z.B. Speicherplatz). |
| Exit-Code `5` bei `sync-motodb` | `loadChannels.xlsx` ist in Excel geöffnet → schließen und erneut ausführen. |
| Exit-Code `6` bei `process` | Keine eindeutige Track-Länge (Log `[trackLength]`) → Länge in KODE eintragen bzw. Vorschlag bestätigen, Konflikte gleicher Längen auflösen, erneut ausführen. |
| Exit-Code `4` / `7`, Log `[discovery] ⚠️ Run skipped …` | Beschädigter `Run_*`-Ordner (Ursache im Log, z.B. Lap-Ordner ohne `cableData.ztx`, mehrfach gesplittete Laps) → Lap-Split in WinTAX reparieren bzw. Run neu vom Logger laden, dann `--force`. |
| Exit-Code `7`, `KOMPASS is invalid …` | `KOMPASS.json` beschädigt → in KODE öffnen/speichern oder aus `backups` wiederherstellen. |
| `riderName` leer, Status `metadata: partial` | Rider (Kürzel/Saison/Team) fehlt in KOMPASS oder Team-Ordner fehlt in `TeamMappings` → in KODE bzw. `oracle-config.json` ergänzen. |
| `DriverName`/`riderName` falsch (alter Wert aus `.HED`) | Ab Revision 3 ersetzt KOMPASS abweichende `.HED`-Werte automatisch; der Ordner wird beim nächsten Aufruf neu verarbeitet. Bleibt der Wert, steht `PreferHedMetadata: true` in der Konfiguration oder KOMPASS kennt den Rider (Kürzel + Saison) nicht. |
| `Access denied` beim Umbenennen eines Geräteordners | Ordner liegt in einem offenen VS-Code-Workspace o.ä. (Datei-Lock); non-fatal — oder `NormalizeDeviceFolderName: false`. |
| Run wird „übersprungen", obwohl gewünscht | `oracle.status.json` vorhanden → mit `--force` erzwingen. |
| Crash beim Laden nativer DLLs | KtmDataHandler-Suite/Dongle/WinTAX-Runtime fehlt → für reine Input-Syncs stattdessen `sync-motodb` (ohne native Abhängigkeit) nutzen. |
