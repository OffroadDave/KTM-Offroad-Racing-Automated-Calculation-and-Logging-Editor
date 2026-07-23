### Channel Parameters Setup

The Setup / Channels Parameters menu is used to set up display parameters for individual logged, virtual channels and OLE channels (temporary data used by internal scripts or by external applications). The Parameters define some values as the default color axis extents, a scale factor and offset together with smoothing filters and a label for the units of the channel value. The resultant channel values are used in all graphical representations, virtual channels, circuit etc.

The Parameters configuration can be Global or Local, with each mode, one or more libraries can be loaded. The Global parameters are available for all users, the Local are available only for the current user, each user can have a personal parameter.

The first part of the dialog is the Find function and its filter options. Write in the box the name of the channel, WinTAX4 automatically filters the list with the matching names. Uses *XXX to find all channels which starts or contains XXX string.

Then the dialog contains the tabular list of the channels.

The information that can be associated to a channel are:

#### Label

Displays the channel name. A click on the column header permits to alphabetically sort the channels.

#### Display

  * ##### Color




Quickly configure the line color in Display Style

  * ##### Style




The style field allows to change some trace properties like the weight of the line, the style and the weight of the marker, the color of the line/marker, the font of the current channel etc.

In order to modify this setting, open the Display Style window on Channel Properties page.

Enable Custom Font settings: enables the local font configuration for the selected channels.

  * Family font: sets the character style.

  * Font dimension: sets the character size.

  * Scalable: The character size will adapt in relation to the windows size.




Line Settings

  1.      1.         * Style: sets the style of the graphs line

        *           * None: no line is drawn

          * Line: continuous line

          * Step: stepped line

          * Fill Down: continuous line with colored bottom area

          * Fill Up: continuous line with colored upper area

          * Bordered: continuous line with border

        * Weight: sets the width of the line in pixel.

        * Line color: sets the line color.

        * Border color: sets the border line color




Marker Settings

  1.      1.         * Style: style of the markers, graphic elements used to represent the marker.

        *           * None: No markers are drawn

          * Dots: dot

          * Cross: cross

          * Rhomboid: rhomboid

          * Square: square

          * Arrow Down: arrow downwards

          * Arrow Up: arrow upwards

          * Vert Line: vertical line

          * Horz Line: horizontal line

        * Weight: size (width) of the markers in pixel.

        * Marker color: color of the markers.




Box Value

  1.      1.         * Border: enables the outline border of cursor value.



  * ##### Unit




Label for engineering units to be displayed beyond the channel value in windows.

  * ##### Output Format




For numeric representation it is possible to choose the format of the numerical datum.

In order to modify this setting, open the Output Format page in Properties window.

Select the numeric format in the combo on the left, and select the number of decimals in the combo on the right. The list of allowed formats is:

  1.      * Auto: the format is kept unchanged

     * Dec: the decimal format allows max 5 digits after the comma

     * Numeric: the numeric format allows max 15 digits after the comma

     * Scientific: the scientific format allows max 15 digits after the comma; the result is written in exponential form

     * Hex: hexadecimal format; the decimals cannot be configured

     * Bin: binary format; the decimals cannot be configured.

     * ASCII: text format; the decimals cannot be configured



  * ##### Grid




Shows grid lines in the graph windows. Grid means Channel Grid, otherwise in [Setup/General](../WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgDefaultAppearence) the User can set a default Window's Grid.

In order to modify this setting, open the Properties window on Grid page.

Enable Local Grid settings

enables the grid visualization with the customized settings.

  1.      1.         * Style: sets the style of the grid

        *           * Simple: the grid is formed by continuous lines.

          * Cross: the grid is formed by crosses indicating the intersection of the vertical and horizontal divisions.

        * Color: color of the grid

        * Line style: sets the style of the line of the grid (valid if Style Simple is set)

        *           * Solid: continuous line

          * Dash: dashed line

          * Dot: dotted line

          * DashDot: dashed line alternated to 1 dot

          * DashDotDot: dashed line alternated to 2 dots




X

  1.      1.         * Mode: calculation mode of the horizontal divisions

        *           * Auto, displays an automatic number of equidistant divisions.

          * Off, no division is displayed

          * Fixed, displays a fixed number of equidistant divisions

          * Step, displays the divisions at fixed intervals equal to the Step value.

          * Manual, displays the divisions in correspondence with the values on the X axis set by the user in the text box. 

        * Step: fixed step to calculate the horizontal divisions (a division for each Step), valid if Mode is set

        * Div.: number of horizontal divisions to be displayed, valid if Mode is set at Auto or Fixed

        * Values: list of values on the X axis corresponding to the divisions, valid if Mode is set at Custom. The list of values can be added directly in the text box, using as division the character ';'




Y

  1.      1.         * Mode: calculation mode of the vertical divisions

        *           * Auto, displays an automatic number of equidistant divisions.

          * Off, no division is displayed

          * Fixed, displays a fixed number of equidistant divisions

          * Step, displays the divisions at fixed intervals equal to the Step value.

          * Manual, displays the divisions in correspondence with the values on the Y axis set by the user in the text box. 

        * Step: fixed step to calculate the vertical divisions (a division for each Step), valid if Mode is set

        * Div: number of vertical divisions to be displayed, valid if Mode is set at Auto or Fixed

        * Values: list of values on the Y axis corresponding to the divisions, valid if Mode is set at Custom. The list of values can be added directly in the text box, using as division the character ';'




  * ##### Enum




This function allows to add, remove or edit sets of values and associated text for individual channels. In WinTAX4 work windows, the text corresponding to the channel values will be displayed in its place; The Enum string is displayed whatever the Output Format (Auto, Scientific, Hexadecimal, etc). In order to modify this setting, open the Properties window on Enum Page.

It is also possible to paste enum values and strings from clipboard.

Values need to be pasted using text mode and formatted with a integer value as a first value separated from the string part with a tab, comma or semi-colon.

Using the "paste data from clipboard" button the values will be automatically formatted with the first integer in the value column and the string value in the second column. 

es: `0, Neutral `

`1, First`

`2, Second`

`3, Third`

`4, Fourth`

`5, Fifth`

`6, Sixth`

`7, Over`

## 

  * ##### Alias




An alias name is associated to the channel. it is possible to set up to 8 labels for one channel. In order to modify this setting, open the Properties window on Alias page.

This function enables to associate different names to a channel; reference to this channel names will be saved so there will be no need to manually reconfigure them again in the future.

Aliases can be configured in logged channels and in virtual channels (VCH).

Important: When dragging a channel to a "Analisys Window" some of ifs settings are also copied to the window. [Please read here for more information.](../../AnalysisWnds/Wnds/GRA/GRAWndSetup.md#REMARK:_DragAndDropChannelsFromChannelParameters)

##### How to enable the visualization of a custom name instead of the Logging name:

This feature allows channels to maintain consistent reference names across environments, even when their logging table names change.

To enable this function, activate the following flag in the “Channels Parameters” configuration:

When enabled, the channel name shown in environments will be taken from the “Label” column in “Channels Parameters,” overriding the logging table name, if it is associated to one or more aliases.

The original logging names will no longer appear in environments but are replaced as follows:

The Label name configured in the "Label" column will replace one of the configured Aliases. Original logged channel names configured as Aliases are replaced by the Label name in the following environments:

  * Channel Browser

  * Analysis Windows

  * Virtual Channels

  * Events

  * Conditions




NOTE: In the channel browser, the original logging table names remain visible in tooltips when hovering on the channel name.

###### Configuration example:

Two different loggers could have different channel names for the same type of channel.

Logged Channels:

  * LoggerA → EngineRevs

  * LoggerB → rpm




Parameters:

  * Label: Revs

  * Alias1: EngineRevs

  * Alias2: rpm




The "Revs" label will be consistently displayed across environments referencing the first available configured channels.

In configurations, the "Revs" label is suggested as if it was a logged channel (auto-complete).

Channels configured with this function are marked with an icon featuring the @ symbol.

##### Notes:

  * In the “Logging Table,” the original logging table names remain unchanged.

  * Multiple channels from the same acquisition table can share a single Label. Access through the alias label will point to the first channel in the Alias list.

  * Backward compatibility is ensured by allowing references to original names from the acquisition table.




#### 

#### Name

Allows the user to set the text that will be visualized on the screen in substitution of the channel name, in the following graphical analysis window: Data Preview, Graph and Graph Real Time, XY and XY Real Time, XYZ, Histogram, Channels, Numeric Table, Track, , Condition Light, Display Value, Symbols and Led, Trend, Alarms, FFT Analysis.

#### Scale

  * Show: global flag to show/hide as default the Y scale on the graphs. Used only on the graph windows in Overlay/Manual mode.

  * Setup Y: Setup of the Y scale. In order to modify this setting, open the Properties window on Scale page.




User can set the following parameters:

  *     * Display Scale Units: The values of the scale are divided for the selected value. A marker indicates the division factor: 10e2 (Hundred), 10e3 (Thousand), etc.

    * Scale Step: Range among the divisions of the Y scale, enabled only in manual scale

    * Show Label: Enabled only in Overlay/Manual mode; if checked, the scale shows the name of the label inside the ticks.

    * Show Unit: Enabled only in Overlay/Manual mode; if checked, the scale shows the measure unit inside the ticks.

    * Remove Decimals: If checked, the number of decimal of the values of the Y scale is set to 0.

  * Auto Scale: if it is selected, the outer limits of the Y scale of the channel are automatically calculated.

  * Min: Minimum of the Y scale in manual scale mode; before editing this fields, change "Setup Y" in the Auto Scale property.

  * Max: Maximum of the Y scale in manual scale mode; before editing this fields, change "Setup Y" in the Auto Scale property.

  * Group: allows the channel to be assigned to a group for Y scale auto scaling; before making any changes to this field make sure to deselect the Auto Scale option. See "Autoscale Groups of Channels" in Graph Analysis Windows for details.




#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

#### Offset

Offsets for each channel. The parameter values may be used in addition to the offsets of the table (configured by Sysma), independently or they may be ignored. This selection is made in the [Setup/General.](../WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgGeneralOptions)

#### Gain

Gains for each channel. The parameter values may be used in addition to the gains of the table (configured by Sysma), independently or they may be ignored. This selection is made in the [Setup/General.](../WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgGeneralOptions)

#### NOTE: Usage of offset and gain is applied with the following formula: 

**_Value = ( RawValue + Offset ) * Gain_**

( WinTAX applies "Offset & Gain" values first adding the offset to the raw value and then the resulting value is multiplied by the gain. )

Note: The formula is hard coded in WinTAX and there are no settings that allow the user to change the order of the operations and/or the values. 

Offset & Gain are applied based on the selected settings configured in the General Setup (Off, File Table, Parameters Settings, Table + Parameters) every time a lap is loaded, a lap is loaded via OLE or settings of the General Setup are confirmed.

#### Filter

Smoothing filters are used to remove unwanted noise or frequency content in a logged channel. When a filter is applied to a channel, the resulting values are used by the display and analysis functions (graphs, virtual channels, export and circuit map).

In order to modify this setting, open the Properties window on Filter page.

The filtering options are as follows:

  * Off: channel values are displayed as logged.

  * Run Average: applies a moving average filter to the channel. Filter length is defined in seconds. If set to zero, the filter is not calculated.

  * FFT: applies a combination of frequency domain filters to the channel. The frequency content of the signal in the range(s) defined by the cutoff frequency is set to zero and the data is reconstructed in the time-domain. The four types available are:

  *     * Low Pass \- maintains frequency content below the cutoff freq. Freq1

    * High Pass \- maintains frequency content above the cutoff freq. Freq1

    * Band Pass \- maintains frequency content in the range Freq1< frequency < Freq2

    * Band Stop \- eliminates frequency content in the range Freq1< frequency < Freq2

  * IIR: Infinite Impulsive Response filter of the channel. The two types available are:

  *     * Low Pass \- Infinite Impulsive Response Low Pass filter is a recursive filter that maintains frequency content below the cut-off frequency. Freq1

    * High Pass \- Infinite Impulsive Response Low Pass filter is a recursive filter that maintains frequency content above the cut-off frequency. Freq1




IMPORTANT: Filters can be customized also in the analysis windows (eg. GRA, XY...)

Filter can be configured to OFF or with a sequence of 1 or more filters for each channel. The filters are then applied, in the order that they have been sorted by the user in the configuration Windows.

If a channel has been configured with filters both in this window ("[Channel Parameter](Channels_Parameters.md)") and in the local configuration window (for example in a GRA window), the filters applied in the Channels parameters will be ignored for that window. 

#### 

####  Alarm

The Alarms function is used to search for anomalous conditions on any channel in the current lap. The function is most useful for runtime analysis on data received via telemetry while the race vehicle is out on the track.

Alarms are defined by specifying the acceptable operating range of the channel together with a second (larger) range which defines the range of valid values (this should be used to filter out noise spikes or failed sensor conditions).

In order to modify this setting, open the Properties window on the Alarm page.

  * Enable Alarm: enables/disables the calculation of the alarm status of a channel.




##### Alarm Limits

Allows the definition of the limits outside of which the channel will be considered in alarm.

  * Min: minimum threshold of the channel; it is the lower limit and alarm occurs if the value is below this limit.

  * Max: maximum threshold of the channel; it is the upper limit and alarm occurs if the value is above this limit.




Note: as well as numerical values, you can also enter strings corresponding to the Constants, also using drag&drop.

##### Valid Range (acceptable values)

Allows the definition of the range of values "valid" that each channel can assume. Values outside this range are considered invalid for the analysis and in this case the alarm will not be identified. Both the Alarm Limits should be included in Valid Range.

  * Min: minimum value of the validity range. 

  * Max: maximum values of the validity range. 




Note: as well as numerical values, you can also enter strings corresponding to the Constants, also using drag&drop.

##### Condition

Allows the definition of a condition for each individual alarm. The condition limits the alarm trigger only for intervals in which the condition is verified; if the condition is false the alarm will not be triggered.

The condition field is used only for managing the visualization of the global popup indicating the alarm statuses during Real Time acquisition. The condition field has no effects on the analysis windows.

  * Condition: Sets a condition which allows to switch on/off the alarm.




##### Display options

  * Show lower warning limit: allows to display lower limit of the alarm.

  * Show upper warning limit: allows to display upper limit of the alarm.

  * Style: style of Alarms limits: Line or Filled.

  * Line Style: needs Line as Style: Line or Dashed Line.

  * Color: color of Alarms limits.

  * Weight: needs Line as Style and Line as Line Style: weight of lines.




##### Real Time Options

  * Show Popup: enables alarms warning during real time analysis as a popup messages.

  * Alarms Warning popup message will be displayed only in alarm condition.

  * Alarms & Invalid Values Warning popup message will be displayed in alarm and in not-valid condition.

  * Show Event: alarms condition are displayed within Event Report window as events.

  * Alarms Warning popup message will be displayed only in alarm condition.

  * Alarms & Invalid Values Warning popup message will be displayed in alarm and in not-valid condition.

  * Debounce period: Minimum time (in milliseconds) between two consecutive alarms occurrences.

  * Delta time: Minimum time (in milliseconds) to validate the condition as an alarm.




For further information about the Alarms and their use, see also [Alarms Window](../../AnalysisWnds/Wnds/ALARMS/ALARMSWndOverview.md) and [Alarm Configuration](../../Menu/Tools.md#MenuTools_AlarmsSubmenu).

#### Alarm - Bitmask mode

The "**Bitmask" mode** provides an additional way to configure alarm conditions for channels, allowing alarms to be triggered using bitwise logic. 

##### How to Enable Bitmask Mode

  1. Open the Alarm properties window.

  2. In the "Alarm type" section, select Bitmask (the default for new alarms is "Range").

  3. When Bitmask is selected, the configuration panel will update to show the following options: 

     1. Mask Value HEX: input field Enter the mask (in HEX) to select which bits to check in the channel value. 

     2. Check Type Radio buttons (visible only in Bitmask mode): Choose the logic for alarm activation:

     * At least one bit active Alarm is triggered if at least one masked bit is active.

     * All bits are active (default) Alarm is triggered only if all masked bits are active.




Note: The alarm is triggered if "1 bit is active" or "at least 1bit is active", dependending of the selection made in the "Check Type" option.

Note: In Bitmask mode, the valid ranges cannot be configured.

####  Accelerator

Shortcut used to insert a channel (or a group of channels) on the graphs windows. In order to modify this setting, open the Properties window on Accelerator page.

### Gradients

By clicking the 3 dots button ( if available in the license ) it is possible also to choose a gradient type of color or select colors manually based on customizable values. The configuration of a Condition channel implies that the gradient color is applied only when the condition is true. If the condition is false, the color configured as solid is applied instead. 

#### 

#### 

#### Multiple Selection

The Multi selection is available by right clicking the mouse or using the space key on your keyboard.

For example the color of 3 channels can be changed at once in this way:

  * Select 3 channels with the standard key (mouse click + ctrl, shift \+ arrow up etc.)

  * Use mouse right click (or Space Bar, or Edit command) to open the color dialog in multi selection mode

  * Change the color

  * Exit from the Color pop-up with OK




#### List Sorting Options

The whole list can be organized in the following columns:

Label, Color, Style, Format, Show, Setup, Min, Max, Offset, Gain, Filter, Alarm, Alias, Hot Key, Unit, Grid, Enum.

Sort by Label in alphabetic order A -> Z

###  Menu

The Channels Parameters Setup menu allows accessing the following commands, divided into sub menu:

#### File Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Apply | Applies the current settings of the window |
| Cancel | Closes the window without applying the current settings |
| Load | Opens a dialog window to select a Channels Parameters configuration file to be loaded. |
| Save As | Opens a dialog window to select a Channels Parameters configuration file .xml), on which the current settings can be saved |
  
#### Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl \+ X | Copies to clipboard the configurations of the channels selected and removes them from the list. |
| Copy | Ctrl \+ C | Copies to clipboard the configurations of the channels selected from the list |
| Paste | Ctrl \+ V | Pastes the configurations of the channels available in clipboard adding them to the list |
  
####  Items Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Item | Adds a new element to the list |
| Remove Item | Removes from the list the selected channels |
| Move Up | Moves up by one position the elements selected in the list |
| Move Down | Moves down by one position the elements selected in the list |
| Switch to Global | Moves the Local parameter to Global to give it for all users |
| Switch to Local | Moves the Global parameter to Local, it will not be available to other users |
  
#### Tools Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Sort | See List Sorting Options |
| Import from CLL... | See [Import from CLL](Using_Parameters.md#Import_CLL) |
| Import from CLX | See [Import from CLX](Using_Parameters.md#Import_CLX) |
| Import from Data | See [Import from Data](Using_Parameters.md#Import_Data) |
| Import from VCH | See [Import from VCH](Using_Parameters.md#Import_VCH) |
| Reset all Windows to Default | Restores in all configurations the current settings of the parameters. See [Reset All Windows to Default](Using_Parameters.md#ResetAllWindowstoDefault) |
  
#### Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | It shows the setup channel parameters help. |
  
###  Toolbar

Channels Parameters Setup window toolbar allows accessing the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Button not enabled in this window |
| Apply | Similar to the Apply command of the File menu |
| Cancel | Similar to the Cancel command of the File menu |
| Load | Similar to the Load command of the File menu |
| Save As | Similar to the Save As command of the File menu |
| Import from CLL... | See [Import from CLL](Using_Parameters.md#Import_CLL) |
| Cut | Similar to the Cut command of the Edit menu |
| Copy | Similar to the Copy command of the Edit menu |
| Paste | Similar to the Paste command of the Edit menu |
| Add Channel | Similar to the Add Channel command of the Channels menu |
| Remove Channel | Similar to the Remove Channel command of the Channels menu |
| Move Up | Similar to the Move Up command of the Channels menu |
| Move Down | Similar to the Move Down command of the Channels menu |
| Sort | See List Sorting Options |
| Channel Browser | Displays the pop-up menu to select the page in the Channel Browser window. |
  
### Pop-up Menu

The Channels Parameters Setup window pop-up menu can be displayed by clicking on an item with the right button of the mouse.

The Channels Parameters Setup window pop-up menu allows accessing the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Cut | Similar to the Cut command of the Edit menu |
| Copy | Similar to the Copy command of the Edit menu |
| Paste | Similar to the Paste command of the Edit menu |
| Add Channel | Similar to the Add Channel command of the Channels menu |
| Remove Channel | Similar to the Remove Channel command of the Channels menu |
| Move Up | Similar to the Move Up command of the Channels menu |
| Move Down | Similar to the Move Down command of the Channels menu |
| Sort | See List Sorting Options |
| Edit | Enables to modify the selected box |
  
### Related Issues

  * [Using Parameters](Using_Parameters.md)

  * [Local Groups](LocalGroups.md)


