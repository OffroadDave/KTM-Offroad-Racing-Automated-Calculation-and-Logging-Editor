### Commands

The main commands available in the [Lap Report](REPWndOverview.md) window can be enabled through:

  * The Options menu in the main menu of the application

  * the buttons of the dedicated Toolbar dedicated

  * Pop-up menu that can be displayed by clicking with the right button of the mouse on the graphic area of the window, on the Info boxes or on Y scales of the selected channels.




#### Options Menu

The Options menu for the [Lap Report](REPWndOverview.md) allows the access to the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Mode |  | [Session](REPWndFeatures.md#REPSession), [Run](REPWndFeatures.md#REPRun), [Selected](REPWndFeatures.md#REPSelected), [Section](REPWndFeatures.md#REPSection) or [Single Lap](REPWndFeatures.md#REPSingle) loaded. Each mode is described in function section. |
| Dataset |  | Dataset commands permit the connection between reports and datasets. With the selection of a dataset, it is possible to view different lap reports with different data loaded. The command works in all configurations except on Selected and Multisession. Only one dataset at a time can be selected. Opening one, automatically disables the previous. When Default is selected, the report works in the standard way and using data from the first dataset enabled. |
| Copy Data to Clipboard | Ctrl \+ Shift + C | Copies to Windows clipboard the reports displayed in the window |
| Open in Excel | Shift \+ X | Open data in Excel sheet |
| Export... |  | Opens the Save As dialog to export report as csv, txt (text files), xls (Microsoft Excel). |
| Properties | E | Opens the interface to [configure the window](REPWndSetup.md). |
| Refresh Windows |  | Recalculates manually the Lap Report window. |
| Stop Operations | Esc | It is enabled only during calculations and it is used to stop them in case it takes a long time. |
  
#### Toolbar

The toolbar of the [Lap Report](REPWndOverview.md) window allows the access to the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Load |  | Opens a window to select a configuration file. |
| Save |  | Saves the window present configuration on a file |
| Properties | E | Opens the interface to [configure the window](REPWndSetup.md). |
| Export |  | See the description of the command in the Options Table. |
| Session |  | Calculates report in all laps of current Session |
| Run |  | Calculates report in all laps of current Run |
| Selected |  | Calculates report in all laps of current laps selection |
| Section |  | Calculates statistics in sectors of the current map, if configured. |
| Single Lap |  | Calculates report to current displayed lap |
| Dataset |  | See the description of the command in the Options Table. |
| Refresh |  | Recalculates manually the Lap Report window. |
| Stop Operations | Esc | It is enabled only during the calculation and it used to stop it in case it takes a long time. |
  
#### Pop-up Menu

By clicking with the right button of the mouse on the graphic area of the window, the following popup menu is displayed

This section will describe only the commands that have not already been described previously.

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Window |  | Opens the [Window submenu](../../../Menu/MenuWindow.md#MenuWindow_submenu). |
| Print Window to Clipboard |  | Copy the image of the window in the current state on the clipboard. |
| Remove | Delete | Removes the selected channels from the graphs. |
| Edit Virtual Channel |  | Opens the VCH Editor (if the selected channel is a VCH) |
  
##### Related Issues

  * [Lap Report window](REPWndOverview.md)

  * [Elements of the window](REPWndItems.md)

  * [Functions](REPWndFeatures.md)

  * [Configuration](REPWndSetup.md)


