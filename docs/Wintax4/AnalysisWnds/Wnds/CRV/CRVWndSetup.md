### Configuration Window

The Track Configuration window enables to set the look of the [Track windows](CRVWndOverview.md) and it is formed by the pages: Options, Marker Channels, Sections Report, Track Channels, Trajectory, Event and Script. The window has a menu, a toolbar and a pop-up menu that eases the access to the configuration and management commands of the window itself.

####  Options

The Options page is composed of the Window Options and of some check commands.

#####  Window Options

It allows configuring the general setting of the window. Each box can be edited by double clicking with the left button of the mouse or by pressing Space Bar.

  * Custom Color: enables the customized setting of the colors and of the font of the window.

  * Window: sets the background color of the window.

  * Track: sets the color of the track.

  * Track Width: sets the width of the track. Values ranging from 1 to 20.

  * Border Width: sets the color of the borders of the track.

  * Border: sets the width of the track border, Values ranging from 1 to 10; if border is not required, select OFF.

  * Markers Width: sets the width of the breakpoints. Values ranging from 1 to 10.

  * Cursor icon: Sets the icon identifying the vehicle on the track.

  * Color: sets the color of the vehicle.

  * Finish icon: Sets the icon identifying the finish line on the track.

  * Font: Opens the [Font configuration page](../Common/Common.md#CommonConfFont). 

  * Grid: If enabled, shows a grid configured with the following parameters, Color, Width and Height.

  * Color: Sets the custom color of the grid.

  * Width: Sets the X division of the grid. In GPS Track the values are in degree, in Standard track the values are in meters.

  * Height: Sets the Y division of the grid. In GPS Track the values are in degree, in Standard track the values are in meters.




In the Options Page there are also some check buttons:

  * Show Track Section: enable the visualization of Track Section labels; if Show Track Section and Show Split Time are enabled both, the windows shows the Track Sections. The user can move each label using the mouse. The section numeration mode is configured in [Track Editor](../../../Tools/TrackEditor/GenerateTrackSections.md).




  * Show Split Time: enable the visualization of Split Time labels; if Show Track Section and Show Split Time are enabled both, the windows shows the Track Sections. The user can move each label using the mouse.




  * Show Custom Track: enable the visualization of the custom elements




  * Show Metric Scales: enable the visualization of the vertical and horizontal scales.




#### 

####  Marker Channels Page

The Marker Channels page allows configuring the settings specific for each channel to be displayed in a graph. Each row identifies a configured channel, while the columns identify the fields to be configured. Each cell can be edited by double clicking with the left button of the mouse or by pressing the Space Bar. Multiple selections are possible through the CTRL and SHIFT buttons.

Show Markers: Enable the visualization of any valid configured channel markers

  * Label: Displays the name of the channel. The name of the channel can be edited and can become a math expression if a = sign comes first.

  * Mode: Allows choosing between the Value, Range and Smooth mode.




In Value mode the number of division is not set and derives from the ratio (Max - Min)/Step. This mode shows only the calculated value and not the whole range between two calculated values.

In Range mode the number of division is fixed and can be set in the Division field. What varies is the Step because the bounds are always Min & Max; in fact Step = (Max - Min)/Division.

In Smooth mode the color range is defined as a blend starting from blue up to red. The blue color represent begin values (little module of value), the red color represent end values (great module of value). The blending range is depending by minimum, maximum and division values.

  * Step: Step is enabled only in Value mode and represents the width of each division; its measurement units are the same as the channel ones.

  * Division: Division is enabled in Range and in Smooth mode and represents the number of ranges to be created to display the marker.

  * Local: Enable the local scale configuration.

  * Min: Minimum values to configure the markers, its measurement units are the same as the channel ones.

  * Max: Maximum values to configure the markers, its measurement units are the same as the channel ones.

  * Color Mode: Sets the sequence of colors for the range. To modify the settings open the Sectors Configuration window. This window configures the colors and values of the ranges for the window markers. The picture shows the possible modes to select the color: Manual, Gradients and Blend.




In every channel configuration values occurs in the same way: if Local is checked, the intervals are automatically recalculated from the minimum and the maximum values configured; if Local is unchecked, only the number of intervals and the corresponding color are created, as shown in picture below.

The range values, when enable for displaying, can also be configured manually; the values are then saved and stored, and are refreshed automatically if other information that affect calculation is changed. The colors can be manually selected also range by range. Every range could be switched off.

The opening mode of a window by default is Manual. This means that the series of colors automatically presented is linked to the custom mode.

  1.      * Manual: In Manual mode the predefined colors are created according to the background colors of the window. The sequence of colors repeats cyclically each ten colors.

     * Gradient: In Gradient mode the color to be used as a base must be selected; as soon as it is chosen a sequence of colors is created following the color set.

     * Blend: In Blend mode, the window takes two user-defined valid colors and creates a palette between the colors.




The Reverse button reverses the order of the colors.

  * Statistics: Adds some information in the legend for marker information.

  *     * None: No information added.

    * Time: Adds the total time during which a channel remains in the range of each marker.

    * Percent: Adds the total percentage time during which a channel remains in the range of each marker.



  * View: Selects the visualization mode of marker.

  *     * Marker: Display markers on the trajectory. The display of the marker, if enabled, overtops the display of the track sections.

    * Histogram: Display markers as histogram.




#### Sections Report

It allows configuring the settings specific for each channel to be displayed for the section report. Each cell can be edited by double clicking with the left button of the mouse or by pressing the Space Bar. Multiple selections are possible through to the CTRL and SHIFT buttons.

Show Sections Reports: Enable the visualization of section reports

  * Label: Displays the name of the channel. The name of the channel can be edited and can become a math expression if a = sign comes first.

  * Color: Sets the graphic color of the channel.

  * Format: shows the style with which the actual channel value will be displayed. To modify the setting, the corresponding configuration window must be opened to enable to configure the setting for the display format of the channel values.




In the left combo the numeric format can be selected, in the right combo the number of decimals are selected. Please find following a list of the possible formats

  1.      * Auto: the channel format is kept unchanged.

     * Dec: the decimal format allows max of 5 digits after the comma.

     * Numeric: the numeric format allows max 15 digits after the comma.

     * Scientific: the scientific format allows max 15 digits after the format; the result is written in exponential form.

     * Hex: hexadecimal format; decimals cannot be configured.

     * Bin: binary format; decimals cannot be configured.

     * ASCII: text format; decimals cannot be configured.



  * Unit: if enabled, the section report legend displays the unit of measurement configured in "Setup/Channel Parameters"

  * Type: The type of statistic to be displayed in the channel can be configured to be calculated based on the section report; the following possibilities are available:

  *     * Start: returns the channel value to the starting time of each section

    * End: returns the channel value to the final time of each section

    * Max: returns to the max channel value for each section

    * Min: returns to the minimum channel value for each section

    * Avg: returns the average value of the channel samples of each section

    * St Dev: returns the standard deviation of the channel samples of each section

  * Legend: If enabled, the bottom of the window shows the display remarks of the channel.

  * Show: It is used to configure the sections to be displayed in the track. By selecting the individual check boxes, the sections are either displayed or hidden. The buttons are used to quickly select all the sections or just corner or straight sections.




  * Additional In this section you can add N channels, dragging them on the question mark. The value of these channels will be displayed in the boxes by the sections to which they have been added. The Delete button is used to cancel the channels.




Enable custom settings: Enables the customized settings. If it is disabled, the configurations available on the Track Editor are displayed, while if it is enabled the configuration set in the window are displayed.

  * Track Section Numeration: sets the visualization mode of the map:

  *     * All: all events of the map are displayed,

    * Corner: only corners are displayed

    * Straight: only straights are displayed

    * Manual: the list of the sections is displayed (see picture) and each section can be visualized or not. Each section can also be customized both for the label to be shown and for the visualization color. Sections can be added or removed thanks to the Options commands. The Default Color button sets red and green alternated colors.



  * Default color: sets the default color of sections:

  * Options: the Manual Track Section Numeration is configured using the buttons Add Section and Remove Section.




#### Track Channels

It allows configuring one or more channels to be displayed in a graph. Each row identifies a configured channel. This channels will be displayed on track when "show track channel values" is checked. This function activates a popup that shows the values of channels calculated in current cursor position.

## 

#### Trajectory

In Trajectory the user can configure the trajectory so that it is easier to see.

The style of the line, the color and the weight can be set.

## 

#### Event

The Event page allows configuring the events connected to the [Track](CRVWndOverview.md) window.

Enable show events: Enables the display of the events.

View Mode: sets the display mode of the events.

  * All: all events are displayed.

  * Manual: only the events selected by the user are displayed in the list.




The list displays the customized events configured by the user.

Each event can be configured by using the buttons on the Options bar (to add, remove an event, to remove all events, to modify and move them inside the list).

#### Script

The Script page allows configuring the scripts connected to the events of [Track](CRVWndOverview.md) window or application, in VBScript or JScript. The script language is chosen in Setup/General. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

####  Menu

The menu of the [Track](CRVWndOverview.md) window allows the access to the following commands, divided into sub menus:

#####  File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the settings to the window. |
| Cancel |  | Closes the window without applying the settings. |
| Load |  | Opens a dialog window to select a configuration file to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file, to save the setting. |
  
#####  Edit Menu

These commands are enabled both in the Section of the Section Reports and in the Markers section. Cut or Copied channels in one of the two sections are pasted always in the same sections.

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies to clipboard the setting of the channels selected from the active section list, and remove them from the list. |
| Copy | Ctrl + C | Copies to clipboard the setting of the channels selected from the active section list. |
| Paste | Ctrl + V | Pastes from clipboard the setting of the channels. |
  
#####  View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use. |
  
##### 

#####  Graph Menu

These commands are enabled both in the Section of the Section Reports and in the Markers section and work on the section currently enabled. If no section has been selected, the Section Reports is selected by Default.

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Graph | Adds a new element to the list for the configuration of channels. |
| Remove Graph | Removes from the list the settings of the selected channels. |
| Move Up | Moves up by one position the elements selected from the list. |
| Move Down | Moves down by one position the elements selected from the list. |
  
### 

#####  Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
## 

####  Toolbar

The toolbar of the window allows the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode ensuring to keep visualized the configuration window while other application windows are being used. |
| Cancel | Closes the configuration window without applying the setting (Similar to the Cancel command of the File menu). |
| Apply | Applies the setting to the graphs window (Similar to the Apply command of the File menu). |
| Load | Similar to the al Load command of the File menu |
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
| Channel Browser | Shows the pop-up menu to select the page in the Channel Browser window. . |
  
####  Pop-up Menu

The pop-up menu of the window can be displayed by double clicking with the right button of the mouse on the Marker Channels, Sections Report and Track Channels pages.

The pop-up menu of the window allows the access to the following commands:

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
| Set As Default | Sets the configuration of the channels in the Channel Parameters. |
| Edit... | Gives the possibility to edit the selected parameter (Similar to double click). |
  
##### Related Issues

  * [Track Window](CRVWndOverview.md)

  * [Elements of the Window](CRVWndItems.md)

  * [Functions](CRVWndFeatures.md)

  * [Commands](CRVWndCommands.md)


