# ORACLE end-to-end test – RT22_GER-Teutschenthal (2026-10-12)

**Scope:** ORACLE (current working tree incl. track length gate), data exchange ORACLE ↔ KOMPASS ↔ KODE ↔ MotoDB
`loadChannels.xlsx`, example data `C:\MXTrackData\Testdata\01_KTM-FR\Wintax\2026\450\05_RT\RT22_GER-Teutschenthal`
(9 device folders, 118 runs, 468 MB).

**Method:** Isolated copy (`C:\MXTrackData\OracleE2E`, same relative folder depth), ORACLE Debug build with its own
`oracle-config.json` pointing to copies of the team-master KOMPASS, the real `loadChannels.xlsx` and a test suggestions
folder. KODE was driven headless through its real `MainKODEViewModel` (`KODE_DATA_DIR` isolated). Original data,
real KOMPASS master, real `loadChannels.xlsx` and the real suggestions folder were **not** modified (verified afterwards).
ORACLE was started with `dotnet ORACLE.dll` because application control blocks `.exe` files outside approved folders.

## 1. Results per area

| # | Area | Scenario | Result |
|---|------|----------|--------|
| 1 | CLI | no args / missing path / unknown command path | ✅ exit 2 / 2 / 3 |
| 2 | validate | valid run (262 channels) / session folder | ✅ exit 0 / exit 3 |
| 3 | detect | R1 (10 runs, read-only) | ✅ 2 race runs found, nothing written, 18.7 s |
| 4 | sync-motodb | KOMPASS → CIRCUITS/HED, idempotent second run | ✅ exit 0, second run 0 changes |
| 5 | sync-motodb | KOMPASS missing / invalid JSON / workbook missing / locked / open in Excel | ✅ exit 3 / 1 / 3 / 5 / 5 |
| 6 | process (run) | raw FP run, no GPS, no length known | ✅ exit 6 (MotoDB blocked) |
| 7 | process (device) | raw R2, 11 runs: folder rename, race detection, stint, XML, .HED | ✅ exit 0, 70 s |
| 8 | Track length | GPS estimate R2: 1560 m from 16 laps (spread 0.6 %), out-lap/in-lap/split-lap filtered | ✅ consistent in CIRCUITS, 11 × .HED, 11 × Session.xml, status, suggestion |
| 9 | Idempotency | rerun R2 | ✅ skipped in 2 s |
| 10 | Cross-run | FP run (was exit 6) after R2 measured the track | ✅ exit 0, 1560 m |
| 11 | Correction | already processed R1 (.HED 1650) / R2 (1560) when CIRCUITS says 1550 | ✅ corrected to 1550, exit 0 |
| 12 | KODE ← ORACLE/MotoDB | suggestions: ORACLE measurement with provenance + CIRCUITS values, conflicts (1000 m × 8, 1880 m × 2) | ✅ |
| 13 | KODE → KOMPASS → MotoDB | apply 1560 + Surface + Lat/Lon, save | ✅ backup → master → local copy (identical) → CIRCUITS 1560, accent variants merged, suggestion → `applied` |
| 14 | ORACLE ← KOMPASS | R1 after confirmation; new session QR | ✅ 1550 → 1560 (source KOMPASS, not provisional); QR .HED `surface = Hardpack` |
| 15 | --gps-split | finish line from KOMPASS without crossings | ✅ clean skip, exit 0 |
| 16 | Errors | KOMPASS invalid (raw run / processed run) | ✅ exit 3 / exit 0 with warning |
| 17 | Errors | KOMPASS folder missing | ✅ exit 3 |
| 18 | Errors | workbook locked during `process` | ✅ exit 5, **no partial writes**; after unlock consistent |
| 19 | Errors | KOMPASS length shared by two tracks | ✅ KOMPASS value rejected, unique CIRCUITS value used |
| 20 | Errors | suggestions folder unreachable | ✅ warning, exit 0, folder not created |
| 21 | Config | `Enforce=false` / `true` without any length | ✅ exit 0 / exit 6 |
| 22 | Convergence | after all error scenarios, rerun | ✅ KOMPASS = CIRCUITS = all .HED = 1560 |
| 23 | process (session, 2 devices) | BC (`AA 01` + `AA02`) | ❌ see C1 |
| 24 | Completeness | all run folders processed | ❌ see C2 |
| 25 | Metadata mapping | team/season/discipline/rider from path + KOMPASS | ❌ see H1 |

Stability: ~45 ORACLE invocations, no crash, every run produced a complete timestamped log.

## 2. Findings (prioritized)

### Critical

**C1 – Session folder with several devices: all devices get the metadata of the first device.**
`process …\BC` (devices `AA 01` and `AA02`) wrote `Car.xml Name = AA01` and `…_AA01BC.HED` into all 26 runs of
`AA02`. MotoDB would attribute AA02 data to the wrong bike/rider. The status is written only at session level, not per
device. *Cause:* `ProcessRiderAsync` resolves metadata once from `runs[0]` and applies it to all runs.
*Fix:* group runs by device folder and process each device separately (own metadata, .HED, status), or reject paths
that contain more than one device. *Impact:* KRAKEN per-run calls are not affected; manual/session-level calls are.

**C2 – Unrecognized run folders are skipped silently.**
`FP\Run_1038` (15 laps), `QR\Run_1065` (21 laps, the qualifying race) and `TP\Run_1054` (16 laps) – the main runs of
these sessions – are corrupt in the original data (lap folders split several times, `_Def_…_Def_…`, lap folders without
`cableData.ztx`). The WinTAX API does not list them, ORACLE processes the remaining short runs and reports exit 0
without any warning. *Fix:* compare `Run_*` folders with the runs found, log every skipped folder with the reason,
status `partial`, exit 4. *Data:* repair the three runs in WinTAX (or restore them from the logger download).

### High

**H1 – Path-based metadata uses fixed positions that do not match the folder layout.**
For `…\01_KTM-FR\Wintax\2026\450\05_RT\<Event>\<Session>\<Device>\Run_x` ORACLE derives
`team = Testdata` (position −10), `season = Wintax` (−8), `discipline = 2026` (−7). Consequences: rider lookup fails
although KOMPASS has *AA / 2026 / KTM Factory Racing* → `Car.xml DriverName` empty, .HED `riderName` empty, .HED file
name `WintaxRT22_…` instead of `2026RT22_…`, wrong `disciplineName/seasonName/teamName` in the .HED and in the
track length suggestion. The status still reports `metadata: success`. The production layout
(`C:\MXTrackData\<Team>\Wintax\<Season>\<Class>\…`) is affected the same way. *Fix:* recognize segments by pattern
(team mapping keys, 4-digit season, engine class, `NN_XX` event type) instead of positions; warn or mark the step
`partial` when the rider cannot be resolved.

**H2 – Real `loadChannels.xlsx`: all lengths in CIRCUITS are empty (changed 12:46).**
After the name cleanup (12:19, lengths present) the file was saved again at 12:46 without any length. If intended
(let ORACLE measure): every track is measured on first processing; until then KRAKEN per-run jobs of short runs end
with exit 6 (see M1). If not intended: restore column I from
`C:\KTM Tools\KTM MotoDB\backups\loadChannels-before-name-cleanup-20261012-121932.xlsx`, then confirm the unique values
in KODE.

### Medium

**M1 – KRAKEN per-run mode cannot estimate from short runs.** Runs without GPS laps (bike check, warm-up, blips) end
with exit 6 until a long run of the same track has been measured (then all later runs succeed via the open
suggestion/CIRCUITS value). *Option:* process long runs/rider folders first, or let the gate use the laps of all runs
of the event.

**M2 – Length corrections on already processed runs are not recorded in `oracle.status.json`.** The idempotent path
rewrites .HED/Session.xml/CIRCUITS (logged in the ORACLE log) but the status keeps the old length/source – audit gap.

**M3 – Audit trail of data transfers is incomplete.** KODE logs *that* it saved (plus backup and who/when in
`KOMPASS.meta.json`) but not *what* changed; ORACLE does not log which KOMPASS version (timestamp/author/hash) it used.

**M4 – KODE suggestion dialog hides a deviating MotoDB value.** For Teutschenthal ORACLE suggested 1560 m while
CIRCUITS had 1550 m; the dialog showed only the ORACLE value (*Current* = empty KOMPASS value).

### Low

- **L1** Invalid KOMPASS → exit 3 (*DataNotFound*, KRAKEN retries) – a distinct code would avoid endless retries.
- **L2** Warnings about unrelated tracks (1000 m × 8, 1880 m × 2) are repeated in every ORACLE run log.
- **L3** `gpsSplit` step status `success` although 0 runs were split.
- **L4** Unknown options (e.g. `--bogus`) are ignored silently.
- **L5** Usage text lists exit codes 0–4 only (5 *ResourceLocked* and 6 *TrackLengthMissing* missing).
- **L6** Idempotent path updates only `trackLength`, not `surface`, in already processed .HED files.
- **L7** KODE xUnit tests write into the real user log `%APPDATA%\KODE\logs` (no `KODE_DATA_DIR` in tests).
- **L8** Application control blocks `ORACLE.exe` outside approved folders – make sure the installed location is allowed.

### Observations (no defect)

- Runs with GPS = 0 produce `sVehicle = 0` with a warning – data property of the example data.
- `merge` is disabled (out of scope).
- During the test WinTAX4 was opened on the original SP session; it updated `<ModT>` in 24 `RunLapHeader.xml` files
  (not caused by ORACLE).

## 3. Performance

| Operation | Duration |
|-----------|----------|
| validate (1 run) | 2.8 s |
| detect (10 runs) | 18.7 s |
| sync-motodb | 2–4 s |
| process single run | ~6 s |
| process device folder (7 / 11 / 39 runs) | 64 s / 70 s / 159 s (≈ 4–9 s per run) |
| idempotency check incl. length check | ~2 s |
| KODE load / suggestions / save incl. MotoDB sync | 0.1 s / 2.1–2.5 s / 1.0 s |

## 4. Summary and readiness

The new track length chain works end to end and is robust: measurement from GPS laps, uniqueness rules, propagation
to CIRCUITS/.HED/Session.xml, suggestion to KODE, confirmation in KODE, adoption by ORACLE, correction of already
processed runs, lock handling without partial writes and convergence after errors. KODE ↔ KOMPASS ↔ MotoDB transfers
were complete and consistent (field mapping Tracklength, Surface, Latitude/Longitude, Country verified).

**Readiness: not ready for unattended production.** C1 (wrong bike attribution for multi-device session paths),
C2 (main runs silently missing) and H1 (rider/team/season mapping from the path) affect the correctness of the data
MotoDB receives. After fixing C1, C2 and H1 and clarifying H2, ORACLE can go to production; M1–M4 should follow.

## 5. Resolution (2026-10-12, same day)

All findings were fixed and re-verified on a fresh isolated copy of the example data (native WinTAX API).
ORACLE tests 57 → 82, KODE tests 129 → 133, all green.

| # | Fix | Verified with the example data |
|---|-----|--------------------------------|
| C1 | Paths with several devices are split per device folder (own metadata, .HED, status); exit codes combined by severity | BC: AA01 → `Car AA01`, `…_AA01BC.HED`; AA02 → `Car AA02`, `…_AA02BC.HED`; status per device |
| C2 | `RunFolderInspector` compares `Run_*` folders with the runs WinTAX finds and diagnoses the cause; status `discovery.skippedRuns`; exit 4 (some skipped) / 7 (none processable); idempotent rerun keeps reporting 4 | QR: Run_1065 reported ("6 of 21 lap folders without cableData.ztx; 20 split several times"), exit 4; Run_1065 directly → exit 7 |
| H1 | `PathMetadataParser`: team via `TeamMappings` key, season = 4-digit year, engine class, discipline = KOMPASS discipline in the path, else the discipline containing the track; rider by abbreviation + season (+ team); existing .HED values completed/corrected from KOMPASS; `metadata` = partial if rider/track unresolved | R2: rider *Andrea Adamo*, team *KTM Factory Racing*, season 2026, discipline MX, `2026RT22_GER-Teutschenthal_AA01R2.HED`, `Car.xml DriverName` set |
| H2 | **Open – user decision.** `loadChannels.xlsx` is open in Excel (since 12:37) and was edited by hand at 12:46; not touched. A restore tool for the 11 unique lengths (placeholders 1000 m/1880 m excluded) is prepared. | – |
| M1 | Gate falls back to `LapDistance` of runs of the same event already processed by ORACLE | FP runs without GPS: 1560 m from 31 laps incl. 17 processed runs, exit 0 (before: exit 6) |
| M2 | Idempotent length correction recorded in status (`correctedFrom`, `correctedAtUtc`) | R2: 1560 → 1565 recorded |
| M3 | ORACLE logs/stores the input versions (`[inputs]`, status `inputs`: timestamp, size, SHA-256, KODE author/time); KODE writes `KOMPASS.changes.log` (who/when/which fields) | `KOMPASS.json … sha256 3fff095f7eed, saved by reicheltd …` |
| M4 | KODE suggestion dialog: column *MotoDB (m)*, status *Differs* with explanation | unit test |
| L1 | Exit code 7 *InvalidData* for invalid KOMPASS (`process` and `sync-motodb`) and corrupt runs | exit 7 |
| L2 | Gate logs only conflicts of the own track (summary count for others) | – |
| L3 | `gpsSplit` = `skipped` when nothing was split | – |
| L4 | Unknown options / unexpected arguments rejected (exit 2), incl. unquoted paths with spaces | `--bogus`, `…\AA 01` unquoted → exit 2 |
| L5 | Usage text lists exit codes 0–7; README/DEBUG_GUIDE updated | – |
| L6 | Idempotent path also updates `surface` from KOMPASS | R2: surface "" → Hardpack |
| L7 | KODE tests use their own data folder (module initializer) | real KODE log unchanged during tests |
| L8 | Deployment note in README (application control, `dotnet ORACLE.dll` for tests) | – |

**Readiness after the fixes:** ready for production once H2 is decided; the three corrupt runs of the example data
(FP Run_1038, QR Run_1065, TP Run_1054) must be repaired in WinTAX to be processed.
