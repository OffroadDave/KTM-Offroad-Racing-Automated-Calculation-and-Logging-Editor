## General Setup

The General Setup window allows configuring the general aspects of the WinTAX4 environment and the settings used by default to display and manage the windows.

The window is formed by the following pages: General Options, Default Appearance, Layout Auto Save Options, Real Time, Special Channels, Video Options, Proxy, DataSets and Directories.

The window has also an integrated menu and a toolbar that ease the access to the configuration and management commands of the window itself.

### General Options page

The General Options page allows configuring the general aspects of the display and the functioning of the graphic windows, the behavior and the setting of the application, the calculation method for the Distance channel used to measure the distances covered.

#### Graphs Settings

  * Auto Connect: Enables automatic connection between the graphic cursors of all active graph windows. Once this function is active, all window cursors will be simultaneously displayed in the same position (time and space). Moving the cursor in the current window will also move the cursors in all other active windows.

  * Auto Connect Zoom: Automatically zooms all windows to the same amount.

  * Keep Zoom After Changing Layout: if it is enabled, it keeps unchanged the zoom level for the [Analysis windows](../../AnalysisWnds/AnalysisWndsOverview.md) displayed, changing the Layout [... see Layout functions ] loaded. If it is disabled, when the loaded Layout is [... see the Layout functions ] changed, the [Analysis windows](../../AnalysisWnds/AnalysisWndsOverview.md) will be displayed with the default zoom level (100%).

  * Keep Zoom After Changing Lap: if it is enabled, it keeps unchanged the zoom level for the [Analysis windows](../../AnalysisWnds/AnalysisWndsOverview.md) displayed, changing the Laps [... see Laps management] loaded. If it is disabled, when the Laps loaded are changed [... see Laps management ], the [Analysis windows](../../AnalysisWnds/AnalysisWndsOverview.md) will be displayed with the default zoom level (100%). If enabled, also the cursor position will be unchanged.

  * Keep in memory Graph Setup changes: Disable this flag if you don't want to be prompted to save a (changed) graph setup each time you change layout.

  * Distinguish Mark on VCH Labels: Marks math channel labels in graphs with.

  * Keep Shift After Changing Layout: Maintain current shift (ratio) when changing window layout.

  * Auto Align Y Scales: Aligns left-hand edges of graph windows. When horizontally tiled, all time-axes occupy the same space and the cursors in each window line up. This is useful for layouts which have graphs in Overlay mode.




  * Auto Reposition Popup Track: When enabled the Popup circuit will move when the cursor approaches.

  * Use table alias name: WinTAX4 uses aliases configured in the table instead of the real channel names.

  * Cursor Value in Transparent: If it is checked, the Graph Window shows the cursor value inside the graph area.

  * Double X Axis Unit: If it is checked, and "Cursor Value in Transparent" is checked too, the Graph Window shows both time cursor and space cursor values.

  * Variance Auto Scale Y Linked: (Default value is checked) If checked, ranges for the Y scale of the Variance, Variance Zoom and Variance Sector are calculated in "linked mode". If the flag is unchecked, the Y scales are calculated independently for each channel.

  * Invert mouse Wheel direction: This setting allows inverting the behaviour of graphs when using the mouse wheel to scroll or zoom. The setting is enabled by default. When enabled, (disabling the option inverts the behaviour indicate below) scrolling the mouse wheel FORWARDS / BACKWARDS causes the related functions to:

    * Scroll the graph to the RIGHT / LEFT if no other key is pressed.

    * Zooms the graph OUT / IN when CTRL is pressed.

    * Moves the cursor to the LEFT / RIGHT if the SHIFT key is pressed (on post processing GRA windows).

  * Switch PP-RT Globally on all layouts

  * Auto Save Graph Windows After Changing Layout: If a change is made to the layouts, WinTAX will save it automatically without showing a confirmation request for each layout that needs to be saved.




#### 

#### Downsampling mode

DownSampling: Graphical downsampling in graph windows. it is useful to graph high frequency channels in the whole race. All math expressions are calculated with the maximum precision.

  * OFF Disable downsampling and paint all samples in graphs

  * First Value Enable downsampling: use the first sample in the neighbourhood of the step of the channel.

  * Min Value Enable downsampling: use the sample corresponding to the minimum value in the neighbourhood of the step of the channel.

  * Max Value Enable downsampling: use the sample corresponding to the maximum value in the neighbourhood of the step of the channel.

  * Average Value Enable downsampling: use the sample corresponding to the average value in the neighbourhood of the step of the channel.

  * Line (Min/Max interpolation) Enable downsampling: use both the samples corresponding to the minimum and the maximum value in the neighbourhood of the step of the channel.




#### Zoom Fix

  * Time: sets the width (in seconds) of the horizontal zoom interval, used by the Zoom Fix Range function of the Graph windows, when the X axis is set in Time mode.

  * Distance: sets the width (in meters) of the horizontal zoom interval, used by the Zoom Fix Range function of the Graph windows, when the X axis is set in Distance mode.

  * Revolutions: sets the width (in revolutions) of the horizontal zoom interval, used by the Zoom Fix Range function of the Graph windows, when the X axis is set in Revolutions mode.




####  High Light

It sets the mode in which the selected channels are highlighted in the [Analysis windows](../../AnalysisWnds/AnalysisWndsOverview.md). Two modes are available:

  * Blink: for each selected channel, the graphic elements (curves, markers, scales) and the corresponding text boxes are displayed with a blinking effect.

  * Bold: for each selected channel, the graphic elements (curves, markers, scales) are drawn with a bolder width, the corresponding text boxes are highlighted by inverting the background and the text colors.




### Windows Update Priority

It sets the priority in which the [Analysis windows](../../AnalysisWnds/AnalysisWndsOverview.md) are calculated when opening layout or data. In particular, this option determines the order of calculation when there are reports embedded in layout. This settings is global, this means that it is the same for every Lap Report in every layout.

  * Layout order: With no priority set, the load follows the order of the layout.

  * Update Reports before Graphs: If Lap Reports are embedded in a layout, they will be calculated before other graph windows.

  * Update Graphs before Reports: If Lap Reports are embedded in a layout, they will be calculated after other graph windows.




#### Application

  * Ask Confirm Before Exit From the Application: If it is enabled, when the application closes, the user is asked for a confirmation through a window.

  * Ask confirmation before closing a Window: When enabled, a confirmation window appears when trying to close an "Analisys" window. The window is the closed only when the windows is confirmed.

  * Enable NoRX Percent: it enables the calculation of the NoRX percentage on the loaded data.

  * Single Channel List This option allows to group channels into a single list in Channel Browser.

  * Ask Saving Layout If the option is enabled, when the application closes or when a new layout is selected, a message will ask if the user wants to save the layout if the size of one or multiple windows has changed.. If the option is disabled, no confirmation message appears and the layout is not saved automatically upon closing the program or changing layout. The message is referred to changes that have been made to the disposition or size of analysis windows in the layouts. It DOES NOT refer to the content of analysis windows (channels or other).

  * Offset & Gain: selection of the source where the offset and gain values can be extracted

  *     * OFF: No offset and gain values are used

    * FileTable: Uses only the offset and gain values set in the context information.

    * Parameters Settings: Uses only the offset and gain values set in the [Channel Parameters](../WndChannelsParametersSetup/Channels_Parameters.md) .

    * Table+Parameters: Uses both parameters; the offsets are summed and the gains multiplied.

##### **How are Offset and Gain elaborations applied?**

It's possible to apply to the raw value of channels a further elaboration, based on the values of Gain and Offset: The calculation formula is the following: **_Value = ( RawValue + Offset ) * Gain_** ( WinTAX applies "Offset & Gain" values first adding the offset to the raw value and then the resulting value is multiplied by the gain. ) Note: The formula is hard coded in WinTAX and there are no settings that allow the user to change the order of the operations and/or the values. Offset & Gain are applied based on the selected settings configured in the General Setup (Off, File Table, Parameters Settings, Table + Parameters) every time a lap is loaded, a lap is loaded via OLE or settings of the General Setup are confirmed.  
---  
  
  * Measure System: enables to set the metrical measurement system adopted in the application.

  *     * Metric, standard metrical measurement system

    * British, British metrical measurement system

  * Script Language: enables to set the programming language used for the script

  *     * VBScript

    * JScript.

  * Time Mode: default of the display mode of the time instant in the application:

  *     * relative, the beginning of the lap is 0 seconds

    * absolute, the beginning of the lap is the hour of acquisition

  * Enum Setup: Defines the priority logic for selecting enum definitions associated with channels, both in real-time and post-processing: 

    *     * **Logger Data Enums only** : use only enum definitions coming from logger data. (Enums configured in the logging table)

    * **Parameters Enums only** : use only enum definitions defined in [Setup \- Channel Parameters](../WndChannelsParametersSetup/Channels_Parameters.md#Enum).

    * **Logger Data Priority** : enum definitions from logger data have priority; [Channel Parameters](../WndChannelsParametersSetup/Channels_Parameters.md#Enum) based definitions are used as fallback.

    * **Parameters Priority** : [Channel Parameters](../WndChannelsParametersSetup/Channels_Parameters.md#Enum) based enum definitions have priority; logger data definitions are used as fallback.

  * Clear Google Earth Location History Clears the cache memory of Google Earth.




#### Distance

  * Distance Interpolation: when enabled, this option displays the data in distance mode by performing a linear interpolation of channel values between the real measured distance channels. This is useful for channels which are logged at a higher rate than the distance channel. If the option is not enabled, channel values will be plotted all together only at the measured distance points. The effect is most noticeable at low speeds where the distance changes relatively little in a given time period.

  * Alignment (on comparison): this options is used when comparing laps in distance mode. It normalizes all laps in the comparison to the same lap length.

  *     * Skip Distance Alignment: don't align

    * Align to Maximum Distance: align (stretch) all laps to the longest in the selection

    * Align to Minimum Distance: align (compress) all laps to the shortest in the selection

    * Align to Average Distance: align to the mean lap length of the selection

    * Align to 0: align to a nominal lap length




Depending on the choices made in the previous list, it is possible to edit:

  1.      * Max difference (%)

     * Exclude TEST laps




Without distance interpolation

With distance interpolation

### Default Appearance page

The Default Appearance page allows configuring the default settings for the graphic aspect of the windows and of the application (colors of the graphic elements, fonts, grids), used when the customized settings are not used by the single windows.

Default: The button resets all fields to their default values.

Color Scheme: With this combo you can select a color scheme by default. The user can choose between Dark, Standard, Steel and White. If the user modify the colors, the combo shows the label "Modified" as shown in the picture below.

#### Color Settings

It shows and allows to set the colors used by default in the [Analysis windows](../../AnalysisWnds/AnalysisWndsOverview.md) where the customized colors are not set.

  * Window

  *     * Background: background color of the window.

  * Graphs

  *     * Background: background color of the graphic area of the window.

    * Border: border color of the graphic area of the window.

  * Track

  *     * Trace: color of the track used by default in the Track windows where no customized colors are set.

    * Border: border color of the track used by default in the Track windows where no customized colors are set.

    * Highlight zoom on track: enable the visualization of the zoom in track trace.




#### Font

It displays and enables to set the characteristics and the behavior of the font by default, used in the [Analysis windows](../../AnalysisWnds/AnalysisWndsOverview.md) where no customized font is set.

  * Family Font: sets the type of font.

  * Font Dimension: sets the size of the font.

  * Scalable: sets the adaptation of the size of the font in relation to the size of the window.




#### Cursor Values

This section allows configuring layout for items that display cursor values.

  * Border: set the default state of visualization of the cursor value border for new channels added in the graph windows.




#### Default Grid

It displays and enables to set the default characteristics of the grid displayed in the graphic area of the [Analysis windows](../../AnalysisWnds/AnalysisWndsOverview.md) . These settings are valid when the customized grid of the window is not enabled.

  * Default Grid: enables/disables the display of the grid.



  * [Style](javascript:TextPopup\(this\)): sets the style of the grid

    * Simple : the grid is formed by continuous lines.

    * Cross: the grid is formed by crosses indicating the intersection of the vertical and horizontal divisions.

  * Color : color of the grid

  * [Line style](javascript:TextPopup\(this\)): sets the style of the line of the grid (valid if Style Simple is set)

    * Solid: continuous line

    * Dash: dashed line

    * Dot: dotted line

    * DashDot: dashed line alternated to 1 dot

    * DashDotDot: dashed line alternated to 2 dots



  * X Div. Section

  * [Mode](javascript:TextPopup\(this\)): calculation mode of the horizontal divisions

    * Auto , shows an automatic number of equidistant divisions.

    * Off , no division is displayed

    * Fixed , shows a fixed number of equidistant divisions

    * Step , shows the divisions at fixed intervals equal to the Step value.

    * Custom , shows the divisions in correspondence with the values on the X axis set by the user in the text box. Values .

  * Step: fixed step to calculate the horizontal divisions (a division for each Step), valid with Mode set to Step

  * Num. div.: the number of horizontal divisions to be displayed, valid with Mode set at Auto or Fixed

  * Value : list of values on X axis corresponding to the divisions, valid with Mode set at Custom. The list of values can be added directly in the text box, using as division the character ';'.

  * Y Div. Section

  * [Mode](javascript:TextPopup\(this\)): calculation mode of the vertical divisions

    * Auto , shows an automatic number of equidistant divisions.

    * Off , no division is displayed

    * Fixed , shows a fixed number of equidistant divisions

  * Step: it cannot be configured at this level

  * Num. div.: the number of vertical divisions to be displayed, valid with Mode set at Auto or Fixed

  * Values: it cannot be configured at this level




#### Track Settings

Configuration of the display mode of the map section on the Graphs window.

  * Transparent: if it is checked, it displays the sections in transparent mode on the graphs; if FALSE they are displayed on a bar external to the graphic area.

  * Line Style: configuration of the type of division line between the different sections (dashed or continuous)

  * Color: configurable colors

  *     * Line: color of the division line

    * Background: background color of the boxes

    * Text: text color of the boxes




#### Finish Line Settings

Configuration of the display of the laps divisions in comparisons on the Graphs window.

  * Transparent: if it is checked, it displays the sections in transparent mode on the graphs; if FALSE they are displayed on a bar external to the graphic area.

  * Line Style: configuration of the type of division line between the different sections (dashed or continuous)

  * Color: configurable colors

  *     * Line: color of the division line

    * Background: background color of the boxes

    * Text: Text color of the boxes




#### Custom Splash Logo

It displays the path of the file used as image in presentation window (Splash) when the application starts. The button enables to open a browser window to select another file. If the path is empty, a default image is used.

### Layout Auto Save Options

The settings contained in this page allows the user to pre-configure the behaviour of WinTAX when changing various aspects of a layout:

### Available filters:

  * Number of Windows: This filter is triggered when the number of windows changes in the layout.

  * Type of Window: This filter is triggered when one or more windows of the layout have their window type changed.

  * Data Source Type: This filter is triggered when one or more windows of the layout are modified from Post processing to RealTime or vice-versa. NOTE: if the checkbox "Switch PP-RT globally on all layouts" is enabled, this filter is automatically disabled as the "Switch PP-RT globally on all layouts" prevails.

  * Size And Position: This filter is triggered whenever a window of the layout changes size or position. 

  * Analysis Window Changes: This filter is triggered whenever a setting of an analysis window of the layout is changed. NOTE: this filter automatically includes also the "Add / Remove channels" filter.

  * Add/Remove channels: This filter is triggered whenever one or more channels are added to an Analysis window. NOTE: this filter cannot be configured if the "Analysis windows changes" filter is different than "Do Nothing". NOTE: If a modification is detected, and the user chooses to save the whole window configuration is saved.




Each of the previously indicated filters can be configured with one of the following options and will be applied when the related filter is triggered: 

  * Do Nothing: WinTAX will never ask to save.

  * Ask To Save: WinTAX will ask the user to choose between saving and ignoring the triggered changes.

  * Auto Save: WinTAX will automatically save the related filter changes upon layout change or when the program is closed.




Also, the following settings can be enabled on this configuration page: 

  * Switch PP-RT Globally on all layouts: This option changes the behaviour of the following shortcuts: T: When disabled, T will switch from PP to RT and vice versa all windows of the active layouts (of all active layout viewers.) When enabled, T will switch from PP to RT and vice versa all layouts (of all active layout viewers.) 

  * SHIFT + T: When disabled "Shift + T" will switch from PP to RT and vice versa the selected layout of the selected layout viewer. When enabled, "Shift + T" will switch from PP to RT and vice versa all layouts of the selected layout viewer.




### Real Time Page

The Real Time page enables to configure the default settings [...] used when the customized settings are not enabled.

#### Show Laps default

It sets the display mode of the Laps in the Real Time (RT) windows.

  * Continuous: Track marker beacon event is shown as a vertical line in the RT graph window; new lap is shown as a continuation of the previous one.

  * Single Lap: Track marker event causes data traces to be reset to left-hand edge of RT graph window; only the current lap is shown




#### Real Time Graphs Window default

It sets the values to handle the scroll and the width of the X scale in the Real Time graphs.

  * Scroll(s): Default setting for RT windows. Data traces are scrolled in steps defined by this field.

  * Scroll(m): value in meters of the scroll on the Real Time windows.

  * X Width (s): Default setting for RT windows. Data traces are scrolled in steps defined by this field.

  * X Width (m): Default x-axis extents for RT graph windows.

  * Enable smoothly scroll: Enables scroll with a rate that ensure more fluidity.

  * Enable grid Scroll: Enables grid scrolling together with graphics. When this check is disabled, only X axis will scroll together with graphics, while grid will remain static on background and will not be aligned to X axis.




#### Post Processing View

  * Post Processing View: enables the customized selection of the Graph configuration to be used in the [Post Processing View](../../AnalysisWnds/Wnds/GRA/GRAWndFeatures.md#PostProcView)

  * Text box: path of the Graph configuration to be used in the Post Processing View




#### Label Events

  * Label Events: display of a label with the name of the event beside the marker of the occurrence

  * Hold Time (s): duration of the display of the label before changing it




#### Select Real Time Graphs Comparison

Selection of a lap to be used as a reference for the comparison during the Real Time acquisition.

  * No comparison

  * Best Lap of current Run

  * Best Lap of current Session

  * Previous Lap

  * Selected Lap




Show Last Valid Value: This check enables the visualization of the last valid value received in the stream: to be used with real-time streams Timeout: Enables the user to decide for how long the "Last valid value" is visualized (while receiving NORX data) before becoming stale and be visualized as NORX. When configured to "0" the timeout never expires and the last Valid Value is shown until a new valid value is received. The value indicated in this field is expressed in seconds.

### Special Channels Page

The special channels page allows you to configure the settings for the most common channels, used in various elaborations of WinTAX4, such as distance, speed, acceleration and throttle channels. On this page it is also possible to configure the options for the circuit calculation.

All configured channels may be logged channels or math channels obtained from other logged channels (such as “distance channel” obtained as the integral of the speed logged channel).

####  Special Channels

  * Distance (m): Distance channels in meters unit. This channel is used for distance-based analysis, circuit calculation, and circuit map sections. Use this option to specify a different channel from the default defined in the logging table.

  * Use default distance channel: This option allows the use of the default distance channels defined in the logging table (see configuration default distance channel in logging table)

  * Speed (km/h): Speed channel in kilometers per hour unit. This channel is used for circuit calculation (see Track editor).

  * Lateral Acceleration (g): Lateral acceleration channel in “g” unit, used For many elaborations, such as: “Track computation” or “Automatic definition of track sections” (see Track editor)

  * Longitudinal Acceleration (g): Longitudinal acceleration channel in “g” unit, used for many elaboration, such as “Track computation” (see Track editor)

  * Steer: Channel which shows the evolution of the angle of the steering wheel. it is used as default channel in Steering Wheel window.

  * Throttle: Engine throttle channel, used for many elaboration, such as: “Automatic definition of track sections” (see Track editor)

  * Gear: Gear channel, used for “Gear Change management” and other elaboration

  * Rpm: Channel which shows the revolutions per minute of the engine. It is used for Revolutions X Axis Mode in Graph window. 

  * Brake: The brake channel is used as default in the windows that use it.

  * Calculate Track Section with "Split channel": With this check enabled, global map is calculated using the special "split time" channel defined here.




#### 

#### Track Options

This section allows you to configure the settings for the track computation. The map obtained can be used for displaying the position of the vehicle during data analysis. The track computation can be performed in three main ways:

##### Standard

In standard mode the track is calculated with Speed, Lateral Acceleration and Distance channels. In this mode the vehicle is positioned only by the logged distance channel.

##### GPS(Degree)

The track is calculated with GPS Longitude and Latitude channels, one channel for GPS latitude and one channel for GPS longitude. In this case the value of a single coordinate is represented by a single channel expressed in degrees. 

  * Latitude (deg.): latitude coordinate channel in degrees unit.

  * Longitude (deg.): longitude coordinate channel in degrees unit




Note: This mode is preferable because requires the usage of only two channels for GPS coordinates.

By enabling the "Use distance from GPS coordinates" option, WinTAX will calculate the "distance" channel, using the Latitude and Longitude channels received from the GPS.

If the "Current Circuit" option is enabled, the GPS calculated distance will be used on the Calculated circuit from GPS. Otherwise it is always possible to select a custom circuit, by enabling the proper option.

##### GPS (Deg. / Min. / Sec.): 

The track is calculated with GPS Longitude and Latitude channels (three channels for GPS latitude and three channels for GPS longitude). In this case the values of a single coordinate are divided into three channels (one channel for degrees, one channel for minutes, and one channel for seconds). In this mode it is also required a GPS status channel that defines the GPS data status.

  * Latitude (deg.): latitude coordinate channel in degrees

  * Latitude (min.): latitude coordinate channel in minutes

  * Latitude (sec.): latitude coordinate channel in seconds

  * Longitude (deg.): longitude coordinate channel in degrees

  * Longitude (min.): longitude coordinate channel in minutes

  * Longitude (sec.): longitude coordinate channel in seconds

  * Status: GPS status channel

  * Enable OK/HS bit: enables check of GPS signal bit status (bit5 of Status channel). If bit5 is equal to zero WinTAX4 will not display GPS data.

  * Enable NSEW bits: enables check of the bits direction (north, south, east, and west).




Status channel bit mapping:

bit7: 1 = north, 0 = south

bit6: 1 = east, 0 = west

bit5: 1 = GPS data OK, 0 = GPS data HS

bit0: Precision 1 bit = 100m.

Note: This mode is used when the logger or the GPS module cannot handle the GPS data in only two channels.

##### Positioning Mode

This section allows configuring vehicle positioning mode on the track when one of the “GPS calculating mode” is selected.

Use distance channel from logged data: with this option the vehicle positioning is carried out through the configured distance channel. This channel may be a logged channel or a math channels obtained from other logged channels. In this case of vehicle positioning is required that the track has been calculated with distance information.

Use distance channel calculated from GPS speed logged data (km/h): with this option the vehicle positioning is carried out through the distance information, calculated from GPS Speed channel. In this case of vehicle positioning is required that the track has been calculated with distance information.

Use GPS coordinate: vehicle positioning is carried out using GPS longitude and latitude logged channels.

##### Trajectories

Plot Trajectories: This option enables the display of the trajectory of the vehicle on track windows.

Check if position is outside Track: This option allows checking if the current vehicle position is outside the track. If the vehicle is recognized to be outside the track its symbol will appear as a circle with a question mark inside.

Maximum deviation of position from track (m): Maximum distance to determine whether the vehicle is inside or outside the track.

### Video Options

The Video Options page enables to configure the video synchronization channel.

####  Video Logger Synchronization

The video can be synchronized to the data either manually or automatically so that graphs and video can all show the same point in time. You can also hear sound if audio is registered within the video component.

Synchronization Channel (PTS)

The Presentation time stamp (PTS) is a metadata field in an MPEG transport stream used to achieve synchronization of separate streams (such as Video, Audio and Data), PTS counter has a resolution of 90kHz.

The PTS is the reference time stamp for synching data and video frames in WinTAX4. The Marelli DVR video recorder sends the MPEG PTS values to the Data Logger. The Data Logger includes the PTS values in a logged channel. This channel must be configured in WinTAX4 for synching data and video frames.

Use PTS logged Channel: select the PTS channel from data or from VCH channels.

Automatic Synchronization: if the PTS channel is not available in the logging table, WinTAX4 can use time synchronization mode. In this case the video is always synchronized to the beginning of the data (the first video frame is positioned at time zero). You can also manually edit this offset (default =0). WinTAX4 has a function to unlock the video from the data: they can be independently moved, backward or forward, up to the required sync point. This new offset is saved within the configuration data.

### Proxy

The Proxy Authentication page enables to configure an Username and Password if required from the HTTP Proxy Server of your LAN.

### DataSets Page

The DataSets page enables to configure the settings of the fonts and of the graphic elements (lines of the curves and markers) used in the [Analysis windows](../../AnalysisWnds/AnalysisWndsOverview.md) and in the other elements of the application when the Compare Laps function is enabled [see Compare Laps function].

Default Settings: The button resets all fields to their default values.

Color Scheme: With this combo you can select a color scheme by default. The user can choose between High Contrast, Low Contrast and Standard. If the user modifies the colors, the combo box will show the label "Modified" as shown in the picture below.

Dataset Management: allows the user to select the behaviour that WinTAX will have when loading multiple datasets:

All windows: when loading multiple datasets, all datasets are visualized on all windows. The user can disable or enable one or more datasets, but this change will reflect all windows.

Single Window: When this option is active, the program still allows loading multiple datasets, and one or more datasets can be enabled and disabled, singularly on each analisys window.

Changing mode will require WinTAX to be restarted.

When "Single Windows" mode is enabled it is also possible to use the "[Dataset Panel](../../WTXWorkWindows/WndDataSet/DataSet.md)".

In the list, the column identifies the configurable fields, while rows correspond to the settings of the fields linked to the Laps loaded in Compare.

For each field it is possible to enable/disable the use of the configured settings (selecting/deselecting). The check box is placed beside the control configuration of the fields. When the setting of a field is disabled, the aspect of the corresponding graphic element is handled according to the settings in the configuration of the [Analysis window](../../AnalysisWnds/AnalysisWndsOverview.md).

  * Slot #: reference slot (max 10). If [Reference Lap](../../DataBrw/Reference_Lap.md) is licence-enabled the first row is dedicated to the configuration



  * Font: shows the active font used in the info text box of the Data Header toolbar and in the graphic windows.




  *     * Family Font: sets the type of font.

    * Font Dimension: sets the size of the font.

    * Scalable: enables the size adapting of the font in proportion to the size of the window.

  * Line

  *     * Style: sets the style of the line inside the graphs

      * None : no line is drawn

      * Line : continuous line

      * Step: stepped line

      * Fill Down : continuous line with colored bottom area

      * Fill Up: continuous line with colored upper area

      * Bordered: continuous line with border

    * Weight: sets width of the line in pixel.

    * Color 1: sets the color of the line.

    * Color 2: color of the border of the line

  * Marker

  *     * Style: Style of the markers, graphic elements used to display the marker.

      * None : No markers are drawn

      * Dots : dot

      * Cross : cross

      * Rhomboid : rhomboid

      * Square : square

      * Arrow Down: arrow downwards

      * Arrow Up: arrow upwards

      * Vert Line: vertical line

      * Horz Line: horizontal line

    * Weight: size (width) of the markers in pixel.

    * Color 1: color of the markers.

Independent color: When enabled, allows customization of the Line Border color and the Marker color separately from the Line color. When disabled, changing the Line – Color setting will also affect the Marker – Color and Line – Border color.




DataSet management:

### Directories Page

The Directories page enables to configure the paths of the working directories.

In the displayed list the first column shows the type of directory of reference (Folder Type), the second shows the complete path of the directory (Folder)set.

In each line of the list, the path of the Folder column can be modified directly in the text box opened by double clicking with the left button of the mouse or with the SPACE bar of the keyboard, or by opening a browser by clicking on the button [ button icon... ].

For some types of Folders (Data, Host, Import), it is possible to specify more than a reference directory.

Folder Types configurable:

  * Startup: sets the default directory used by the Data Browser

  * Acquisition: this is the directory where WinTAX4 saves downloaded files (Cable or Real time)

  * Log: where WinTAX4 saves log files (if enabled in the WinTAX4\system\LogFile.xml)

  * Data: defines the list of paths displayed in the Data Browser drop-down menu.

  * Host: the list of paths which can be enabled to be used by the AutoCopyToHost Rx Task.

  * Import: this is the path used by the AutoImport Rx Task

  * Export: Default directory for export functions

  * Work: folder where WinTAX4 saves temporary working files - do not change (Read Only path)

  * Lookup Files: This folder configures where WinTAX will search for lookup tables. Leave this setting empty to use the standard lookup folder used by WinTAX. Click on the 3 dots and select a folder to override the folder selection and use a custom folder. WinTAX will look in the selected folder and subfolders.




### Menu

The menu of the General Setup window enables the access to the following commands, divided into sub menus:

#### File

| COMMAND | DESCRIPTION |
| --- | --- |
| Apply | Applies the current settings and closes the window |
| Cancel | Closes the window without applying the current settings |
| Load | Opens a dialog window to select a configuration file Extended Comparison Settings (.ecs) to be loaded; it is enabled when the DataSets page is active |
| Save As | Opens a dialog window to select a configuration file Extended Comparison Settings (.ecs)on which the current settings for DataSets can be saved; it is enabled when the DataSets page is active |
  
#### Edit

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Item | It is enabled when the DataSets page or the Directories page are active; it adds a new item to the configuration list of the compared DataSets, or in the directories list for the folders where the multiple directories can be configured. |
| Remove Item | It is enabled when the DataSets page or the Directories page are active; it removes from configuration list of the colors for compared DataSets the selected items, or the directories selected in the list of the folders where multiple directories can be configured. |
| Move Up | It is enabled when the Directories page is active; it moves upwards by one position the directories selected in the list of the folders where the multiple directories can be selected. |
| Move Down | It is enabled when the Directories page is active; it moves downwards by one position the directories selected in the list of the folders where the multiple directories can be selected. |
  
#### View

| COMMAND | DESCRIPTION |
| --- | --- |
| Previous Page | It enables the page of the window previous to the one currently in use. |
| Next Page | It enables the page of the window next to the one currently in use. |
  
#### Help

| COMMAND | DESCRIPTION |
| --- | --- |
| Help | It shows the setup general help. |
  
###  Toolbar

The toolbar of the General Setup window enables the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Not available in this window |
| Cancel | Closes the window without applying the current settings (similar to the Cancel command of the File menu) |
| Apply | Applies the current settings and closes the window (similar to the Apply command of the File menu) |
| Load | Opens a dialog window to select a configuration file Extended Comparison Settings (.ecs)to be loaded; it is enabled when the DataSets page is active (similar to the Load command of the File menu) |
| Save As | Opens a dialog window to select a configuration file Extended Comparison Settings (.ecs), on which the current settings for DataSets can be saved; it is enabled when the DataSets page is active (similar to the Save As command of the File menu) |
| Previous Page | It enables the page of the window previous to the one currently in use (similar to the Previous Page command of the View menu) |
| Next Page | It enables the page of the window next to the one currently in use (similar to the Next Page command of the View menu) |
| Add Item | Similar to the Add Item command of the Edit menu |
| Remove Item | Similar to the Remove Item command of the Edit menu |
| Move Up | Similar to the Move Up command of the Edit menu |
| Move Down | Similar to the Move Down command of the Edit menu |
| Channel Browser | Shows the pop-up to select the page in the Channel Browser window. |