### Configuration Window

The configuration window is formed by the following pages: Options and Script.

The window also has a menu and a toolbar, that makes it easier to access the configuration and management commands of the window itself

#### Options

The following options are configurable on the window:

##### Window

| COMMAND | DESCRIPTION |
| --- | --- |
| Custom Font | Enables/disables custom Font option. |
| Font | Opens the [Font configuration page](../Common/Common.md#CommonConfFont). |
| Header Background | Window header color. |
| Header Foreground | Header font color. |
| Report Background | Report color. |
| Report Foreground | Report font color. |
| Mode | Selection of "Run Mode" or "Selection Mode" or " Session Mode" as starting report. |
| Track Sections | Sets map for report computation |
| Lap Filter | Opens the [Lap Filter configuration page](../Common/Common.md#CommonConfLapFilter). |
| Calculation Filter | See Lap Calculation Filter paragraph. |
| Grid | Color of the grid. |
| Active Text | Color of the loaded laps. |
| Ideal Time Background | Color of the cell which represents the best sector. |
| Lap Time Background | Background color of the Lap Time column. |
| Highlight Range | Flag which allows showing the sectors nearest to the best one. |
| Percentage | Percentage of the Highlight Range. |
| Background | Background: color of the highlighted cell |
  
#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

##### Graphs Options

Left columns of report can be configured by user adding laps information into window setup page.

#### Lap calculation filter

Displays the setting to enable the calculation filter. With this filter, which is configured in the same way to the [Lap Filter configuration](../Common/Common.md#CommonConfLapFilter), you determine which lap should be displayed but not calculated, as shown in the figure below where the lap with marker Out is not calculated.

#### 

#### Script

The Script page enabled to configure the scripts of the events of the window or of the application, in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

#### Menu

The menu of the window allows access to the following commands, divided into sub menus:

##### File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the current settings of the window. |
| Cancel |  | Closes the window without applying the current settings. |
  
##### View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use. |
  
## 

#### Toolbar

The toolbar of the window allows access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode allowing to keep displayed the configuration window while the application window is in use. |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel command of the File menu). |
| Apply | Applies the current settings to the configuration of the graphic window (similar to the Apply command of the File menu). |
| Previous page | Similar to the Previous page command of the View menu |
| Next page | Similar to the Next page command of the View menu |
  
##### 

##### Related Issues

  * [Section Time Report window](SECTRWndOverview.md)

  * [Elements of the window](SECTRWndItems.md)

  * [Commands](SECTRWndCommands.md)


