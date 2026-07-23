### Section Time Report Window Overview

The Section Time Report window enables to calculate the partial times of each lap. The analysis can be carried out both on the selected laps and on all the laps of a run and on all the laps of a session. The number of laps analyzed is limited to those identified by the following Lap Markers: NO MARKER, LAP, BEST LAP, BOX, OUT, OUT+BOX. This number can be further reduced through the local filters and manually disabled after the first load. The partial times are calculated on the basis of how the Track has been divided in Sections, this division is made using the [Track Editor](../../../Tools/TrackEditor/Track_Editor.md) or the [Manual Section Definition](../GRA/GRAWndCommands.md#GRAMenuOptions_ManualSectionDef) command on a Graph window.

Remarks: The Section Time Report opens in just one instance.

To understand the window, please bear in mind the following definitions:

  * Split Time is the coverage time of a single section of a lap.

  * Lap Time is the coverage time of the lap that is equal to the sum of all the split times of the lap.

  * Ideal Time is the sum of the best split times of each section. The best split time of a section corresponds to the lowest value of the coverage time of the section itself among all the selected laps; split times equal to 0 are not taken into account for the calculation of the ideal time.

  * Rolling Lap is the fastest lap formed by consecutive sections that do not necessarily begin from the first one. From the calculation of the Rolling Lap the split times equal to 0 are excluded.

  * Best Lap: is the fastest lap of lap list.

  * Selected Lap: it is the lap corresponding to the check in the column "Selected Lap"; the default selected lap is the Best Lap. 




##### Related Issues

  * [Analysis Window](../../AnalysisWndsOverview.md)

  * [Elements of the window](SECTRWndItems.md)

  * [Configuration](SECTRWndSetup.md)

  * [Commands](SECTRWndCommands.md)


