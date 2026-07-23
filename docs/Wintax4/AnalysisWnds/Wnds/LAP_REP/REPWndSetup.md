### Lap Report Configuration

The Lap Report Configuration window allows configuring the aspect of the [Lap Report](REPWndOverview.md) window and it is composed of the following pages: Options, Script. The window has a menu, a toolbar and a pop-up menu that makes it easier to access the configuration and management commands of the window itself

####  Options Page

The Options page allows configuring the graphic aspect of the [Lap Report](REPWndOverview.md) windows and it is divided into 2 sections: Window and Graph.

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

#####  Window Options

It allows configuring the general settings of the windows. Each configurable value can be edited by double clicking with the mouse or by pressing the space bar on the selected element

  * Window: sets the window background color.

  * Boxes color

  *     * Background: sets the background color of the header rows and first column

    * Foreground: sets the text color of the header rows and first column



  * Mode: Selects report mode (Session, Run, Selected, Section, Single Lap)

  * Layout: Available only for Single Lap. Change the horizontal or vertical orientation.

  * Header: If this option is selected, the top of the window will show a header with some summary information about the session.

  * Lap Filter: displays the setting to enable the lap filters. The lap filters area is an additional filter on the lap, used by the calculation window. To modify the setting and filter the laps, change the settings of the edit window of the filters.




    * Enable: Enables the customized settings

    * Acquisition

    *       * Source: The laps are filtered on the basis of the data source: All no filter is applied, Cable filters taking into account only the cable data (files cableData.ztx, Real time filters taking into account only the data acquired in real time (files dstData.ztx and nbtData.ztx), NBT filters only the real time NBT data (files nbtData.ztx), Real time filters only the Real time data (files dstData.ztx). The Radio type keeps compatibility with WinTAX2 and WinTAX3 files and is relative to *.dtx file types.

    * Runs

    *       * All Runs: Selects all the runs

      * Runs: Filters to keep all runs specified in the list of the text box: The comma is used to list more runs or a tow dots to define an interval (Example: 1,2,3,10..12)

      * Last: Selects the last runs

    * Date / Hour

    *       * All Laps: By default and selects all laps of the run

      * Find all laps acquired:

      *         * Between: Filters only the laps acquired during the set interval

        * During the previous: Filters only the laps acquired during the last hours set.

    * Lap Markers:




The filter is carried out on the basis of the selected Markers. By default everything is selected. The Markers are information contained inside each lap and define the type of Lap. The two buttons on the options space bar are used to select or deselect all markers.

  * Alarms: 

    * Show: Enables to highlight the channels that have exceeded alarm thresholds during a lap.

  *     * Out of Range: background color for out of range values

    * Alarm: background color for alarm values



  * Statistics

  * Show: enables to display the statistics box at the bottom of the window.

  * Min: foreground color of the minimum value of a statistic between all laps.

  * Max: foreground color of the maximum value of a statistic between all laps.

  * Avg: foreground color of the average value of a statistic between all laps.

  * Level

    * Show: enables, in all the cells, the visualization of a style that allows having an indication of the percentage of the value respect to the maximum. The cell will then be partially colored and the level will correspond to the value of the cell compared to the maximum value.

    * Color: color of the percentage level

  * Median Corner




The Median Corner point (MED) is at the chord of the corner. It is used in section reports to display the corresponding values of other channels at the centre of the corner (e.g. steer angle, brake pressure etc.)

  * Channel: enter name of channel to be used to find Median Corner point (typically wheel speed or lateral acceleration).

  * Value: Median Corner point can be either at the Min or Max value of the channel specified (e.g. use Min with Wheel Speed or Max with Lateral Acceleration )




**N ote:** The _Median Corner_ option is only available if:

  * The report is in **Section** mode (_Mode_ field),

  * A **section map** is active and has been calculated ([see commands for defining sections here](../../../Tools/TrackEditor/GenerateTrackSections.md)).




#####  Graphs Options

It allows configuring the settings specific of the channel to be graphed. Each configurable value can be edited by double clicking with the mouse or by pressing the space bar on the selected element. Multiple selections are possible through the CTRL and SHIFT keys.

  * Label: Element to be displayed in report (channel, info etc.)

  * Custom font: Enables a custom font for this entry

  * Font: Opens the [Font configuration page](../Common/Common.md#CommonConfFont). 

  * Background: Cell background color

  * Foreground: Text color

  * Min: Shows minimum value of the selected element

  * Max: Shows maximum value of the selected element

  * Avg: Shows average (mean) value of the selected element

  * Start: Shows start value of the selected element

  * End: Shows end value of the selected element. NOTE: The "End" value is the last valid value in the last effective second of the lap. (e.g. If the channel has NoRX samples for more than a second at the end of the lap, the obtained value will be NoRX )

  * Change: Show the difference between End and Start for each lap. In mode Section shows the difference between End and Start for each section.

  * StdDev: Shows Standard Deviation value of the selected element

  * Median Corner: Shows medium value of the selected element for Section (Section report mode only)

  * Event: Enable the event search

  * Event Name: Specifies an event whose corresponding values will be displayed (not active in Section report mode)

  * Average Time: Specifies a time duration in 1/100s of a second over which the channel values are averaged when searching for events

  * Num Max Occurrences: Maximum number of event occurrences (columns) which will be displayed in each line of the report.

  * Condition Type: representation of how long a given condition was true.

  * Show Statistic: Enable the visualization of statistic values (min, max, avg) for each channel in the Run, Session or Selected Laps.

  * Export Decimals: This parameter acts on the number of decimals of the channel during exporting. If Auto is set, the number of decimals belonging to the channel is used, otherwise the number of decimals set is removed. It applies to all the export processes.

  * First: Returns the first valid value in the first 3 effective seconds of the lap.

  * Last: Returns the last valid value in the last 3 effective seconds of the lap.




####  Script Page

The Script page enables to configure scripts linked to the events of the [Lap Report](REPWndOverview.md) window or of the application, in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

####  Menu

The [Lap Report](REPWndOverview.md) window menu enables the access to the following commands, divided in sub menus:

##### File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the current settings of the window. |
| Cancel |  | Closes the window without applying the current settings. |
| Load |  | Opens a dialog window to select a XY configuration file to be loaded. |
| Save As |  | Opens a dialog window to select a XY configuration file .XY), on which the current settings can be saved. |
  
#####  Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies to clipboard the configurations of the channels selected in the list of the Graphs sections and removes them from the list of the Graphs section. |
| Copy | Ctrl + C | Copies to clipboard the configurations of the channels selected in the list of the Graphs. |
| Paste | Ctrl + V | Pastes the configurations of the channels available in clipboard adding them to the list of the Graphs section. |
  
##### View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl + Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl + Tab | Enables the page of the window next to the one currently in use. |
  
##### Graph Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Graph | Adds a new element to the Graphs list of channels configurations. |
| Remove Graph | Removes from the Graphs list the configurations of the selected channels. |
| Move Up | Moves up by one position the selected elements in the Graphs list. |
| Move Down | Moves down by one position the selected elements in the Graphs list. |
  
####  Toolbar

The toolbar of the window allows the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode allowing to keep displayed the configuration window while the application window are in use. |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel command of the File menu). |
| Apply | Applies the current settings to the configuration of the graphic window (similar to the Apply command of the File menu). |
| Load | Similar to the Load command of the File menu |
| Save As | Similar to the Save As command of the File menu |
| Previous page | Similar to the Previous page command of the View menu |
| Next page | Similar to the Next page command of the View menu |
| Cut | Similar to the Cut command of the Edit menu |
| Copy | Similar to the Copy command of the Edit menu |
| Paste | Similar to the Paste command of the Edit menu |
| Add Graph | Similar to the Add Graph command of the Graph menu |
| Remove Graph | Similar to the Remove Graph command of the Graph menu |
| Move Up | Similar to the Move Up command of the Graph menu |
| Move Down | Similar to the Move Down command of the Graph menu |
| Channel Browser | Displays the pop-up menu to select the page in the Channel Browser window.. |
  
####  Popup Menu

The pop-up menu of the window can be displayed by clicking with the right button of the mouse on the Options page.

The pop-up menu of the window gives access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Cut | Similar to the Cut command of the Edit menu |
| Copy | Similar to the Copy command of the Edit menu |
| Paste | Similar to the Paste command of the Edit menu |
| Add Graph | Similar to the Add Graph command of the Graph menu |
| Remove Graph | Similar to the Remove Graph command of the Graph menu |
| Move Up | Similar to the Move Up command of the Graph menu |
| Move Down | Similar to the Move Down command of the Graph menu |
| Restore Default | Sets in the channel the settings configured in the Channel Parameters. |
| Set As Default | Configures the channel settings in the Channel Parameters. |
| Edit... | Gives the possibility to edit the selected parameter (similar to double click). |
  
##### Related Issues

  * [Lap Report window](REPWndOverview.md)

  * [Elements of the window](REPWndItems.md)

  * [Functions](REPWndFeatures.md)

  * [Commands](REPWndCommands.md)


