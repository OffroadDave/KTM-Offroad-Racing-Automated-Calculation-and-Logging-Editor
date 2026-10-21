# Gültiger Start (VALID Start) – VCH-Definition

`KTM_ORR_ValidStart.vch` ist ein **Entwurf** für eine WinTAX-VCH-Bibliothek, die einen *gültigen* Gatterstart erkennt.
Die gleiche Logik ist in ORACLE implementiert (`ORACLE.exe detect-start <Pfad>`, `Detection/StartDetector.cs`) und
wurde damit an den Beispieldaten RT22_GER-Teutschenthal geprüft.

## Problem der bisherigen Definition (`KTM_ORR_MotoDB.vch`)

```
StartTrigger   = Time, wenn Gas >= Kupplungsweg UND Gang != N UND revlimitsource != 1 UND chavg(xClutch) != 0
TimestampStart = chmin(StartTrigger)
```

* Es wird **nur der Auslösemoment** geprüft – nicht, ob das Bike vorher am Gatter stand und ob es danach wirklich
  losgefahren ist. Ein abgebrochener Start, ein Abwürgen oder eine Launch-Aktivierung in der Box gelten als Start.
* `chmin` liefert immer den ersten Kandidaten; einen ungültigen ersten Kandidaten kann die VCH nicht überspringen.
* Ohne Kupplungsdaten (`chavg(xClutch) == 0`) gibt es nie einen Start.

## Neue Definition (vier Bedingungen)

| # | Bedingung | VCH-Kanal | Schwelle |
|---|-----------|-----------|----------|
| 1 | **Bereit am Gatter**: Stillstand, Gang eingelegt, Launch-Drehzahl | `CND_VS_Staged`, `tVS_StagedTime` | vGPS < 3 km/h, NGear ≥ 1, nEngine720 > 4000 rpm, ununterbrochen ≥ 1.5 s |
| 2 | **Auslösen aus der Bereitschaft** (wie bisher: Gas ≥ Kupplungsweg, Launch-Begrenzer `revlimitsource ≠ 1` noch aktiv) | `CND_VS_GateReady`, `StartTriggerValid`, `TimestampStartCandidate` | Bereitschaft höchstens 1 s zurück |
| 3 | **Vollgas** | `rThrottle_StartLaunch` | ≥ 50 % innerhalb 0.5 s |
| 4 | **Wirklich gestartet** | `vVehicleGPS_Start3s`, `sStart5s` | ≥ 30 km/h in 3 s und ≥ 20 m in 5 s |

Ergebnis: `bValidStart` (1/0) und `TimestampStartValid` (Zeitpunkt oder NoRx).

Beobachtung aus den Daten: Am Gatter ist der Launch-Begrenzer aktiv (`revlimitsource = 4`), er springt beim Loslassen
der Kupplung auf 1. Das Gas springt dabei innerhalb eines Samples von ~25 % auf 100 % – deshalb wird die
Gas-Schwelle **nach** dem Auslösen geprüft, nicht im Auslösemoment.

## Ergebnis an den Beispieldaten (`ORACLE.exe detect-start`)

| Session | Runs | gültige Starts | Zeit gleich bisherigem `StartTrigger` |
|---------|------|----------------|---------------------------------------|
| SP (Startübung) | 24 | 4 (8 600–8 800 rpm, LC an, 79–82 km/h in 3 s, ~80 m in 5 s) | ja (alle 4) |
| R1 / R2 (Rennen) | 10 / 11 | je 1 (Run_1083 / Run_1096, 66–68 km/h in 3 s) | ja |
| TP | 6 | 1 (Run_1055) | ja |
| WU, FP, BC | 8 / 8 / 39 | 0 | – (auch bisher kein Trigger) |

Die Auslösezeitpunkte sind identisch; neu ist die Absicherung (Bereitschaft, Vollgas, tatsächlicher Start) und dass
verworfene Kandidaten mit Grund ausgegeben werden.

## Einführung

1. `KTM_ORR_ValidStart.vch` in WinTAX laden (benötigt `vVehicleGpsGuarded` aus `KTM_ORR_General.vch` und `sVehicle`
   aus ORACLE) und an einigen Runs mit `ORACLE.exe detect-start <Session>` vergleichen.
2. Zu prüfen in WinTAX: `dim`-Variablen behalten ihren Wert zwischen Samples (wie in `tRevlimiterInRace`), und
   `diaggt(ch, Wert, Haltezeit)` hält 1 s nach.
3. Nach Freigabe in `KTM_ORR_MotoDB.vch`: `TimestampStart = TimestampStartValid` (alle `CND_*Start*`-Bedingungen und
   MotoDB-Kennzahlen bauen darauf auf) und die Bibliothek in Shared Resources (`Source/02_MotoDB/VCH`) aufnehmen.
4. Optional für MotoDB/Tableau: `bValidStart` als Filter (nur Runs mit gültigem Start in Start-Auswertungen).
