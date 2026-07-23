## Viewing Datasets

Windows that do not support comparison mode (e.g. lap reports, trend, circuit, channels) will show data only from the first Dataset in the list (the Base Dataset ).

Lap report windows show entries for all laps of the part of the Base Dataset archive (i.e. anything for a vehicle in a particular session).

All other windows can display data from any combination of Dataset .

If a window is associated with multiple Dataset they will be displayed in comparison (overlay) mode and will take the color settings defined in the Comparison Groups setup (each Dataset is equivalent to a comparison group).

The subset of laps from a displayed Dataset is selected via the [Interactive Data Header](../Toolbars/ToolbarDataHeader.md) . All windows displaying that Dataset will display the same subset of laps.

For individual channels within a [Graph](../AnalysisWnds/Wnds/GRA/GRAWndOverview.md) window it is possible to show/hide data from individual Dataset and to select the Auto Difference option.

In Virtual Channels (VCH) configurations, it is possible to use the [# operator](../RuntimeAnalysis/VCH/Functions/MatFunc_SpecialOperator.md); it is used to identify the Dataset on which the calculation of the channel will be made.

  1. Right click on the channel values and select Set comparison




  2. Enable the Dataset you want to draw

  3. Select the "Difference" button to view the difference between selected data and the Base Dataset




Auto connect (zoom, cursor, shift) functions apply to individual Datasets and any window that show them including comparisons.

Example:

Window_A shows Dataset #1

Window_B shows Datasets #2, #3.

Zoom in on window_B, window_A does not change.