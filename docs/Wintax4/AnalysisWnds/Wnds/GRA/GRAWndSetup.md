### Configuration Window

The Graph Configuration window allows configuring the aspect of the [Graph](GRAWndOverview.md) windows; it is divided into four pages, Options, Event, Sections Report and Script. The window has also a menu, a toolbar and an integrated pop-up menu that ease the access to the configuration and management of the window itself.

#### Options Page

The Options page enables to configure the graphic aspect of the [Graph](GRAWndOverview.md) windows and it is divided into two sections: Window Options and Graphs Options .

#### 

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

##### Window Options

It enables to configure the settings of the aspect of the window. Each element of the grid can be edited by double clicking with the mouse or by pressing the space bar.

  * Custom color: enables the setting of the customized colors of the window.




If it is enabled, the colors set in the Window, Graph and Border columns of this section are used for the background of the window, the background and the border of the graphic area. If it is disabled, the default colors are used that correspond to the settings of the Color Settings section, in the [Default Appearance](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgDefaultAppearence) page of the [General Setup](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md) window (General Settings, general configuration of the WinTAX4 environment).

  * Window: sets background color of the window.

  * Graph: sets the background color of the graphic area.

  * Border: sets the color of the borders of the areas of the window.

  * Grid: displays the setting to enable the grid common to all graphs of the channels, in the graphic area of the window. To modify the parameter, edit the associated configuration window.




  *     1.        * Enable Custom Grid settings: enables the grid visualization with the customized settings.

       * Style: sets the style of the grid

       *          * Simple: the grid is formed by lines drawn with the styles configured in Line Style settings.

         * Cross: the grid is formed by crosses indicating the intersection of the vertical and horizontal divisions.

       * Color: color of the grid

       * Line style: sets the style of the line of the grid (valid if the Style Simple is set)

       *          * Solid: continuous line

         * Dash: dashed line

         * Dot: dotted line

         * DashDot: dashed line alternated with 1 dot

         * DashDotDot: dashed line alternated with 2 dots

  * X

  *     1.        * Mode: calculation mode of the horizontal divisions

       *          * Auto, shows an automatic number of equidistant divisions.

         * Off, no division is displayed

         * Fixed, shows a fixed number of equidistant divisions

         * Step, shows the divisions at fixed intervals equal to the Step value.

         * Manual, shows the divisions in correspondence with the values on the X axis set by the user in the text box Values.

       * Step: fixed step to calculate the horizontal divisions (a division each Step), valid if Mode is set to Step

       * Div: number of horizontal divisions to be displayed, valid if Mode is set to Auto or Fixed

       * Values: list of values on the X axis corresponding to the divisions, valid if Mode is set to Manual. The list of values can be directly added in the text box using as division the character ';'.

  * Y

  *     1.        * Mode: calculation mode of the vertical divisions

       *          * Auto, shows an automatic number of equidistant divisions.

         * Off, no division is displayed

         * Fixed, shows a fixed number of equidistant divisions

       * Step: not used.

       * Div: number of vertical divisions to be displayed, valid if Mode is set to Auto or Fixed

       * Values: not used.




If the window grid is disabled, the settings of the Default Grid in the [Default Appearance](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgDefaultAppearence) of the [General Setup](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md) window (General Settings, general configuration of the WinTAX4 environment) are valid.

  * Cursor Styles: Graph cursor selection

  *     * Vertical: the window displays a vertical cursor.

    * Horizontal: the window displays a vertical cursor and a horizontal cursor for each channel,

    * Intersection: the window display a cross cursor for each channel in the window and a vertical cursor to connect all cross cursors.

    * Multiple Cross: the window displays a cross cursor for each channel in the window.

    * Single Cross: the window displays a cross cursor for the selected channel.

  * Layout: enables to select the vertical arrangement mode of the channels graphs and of the corresponding Y scales for each area. See also: [Layout](GRAWndFeatures.md#GRAFeatureLayout) function.

  *     * Parallel: the graphic area is divided into rectangles of the same height and the graphs of the channels are arranged in vertical sequence so that they do not overlay.

    * Overlay: all graphs share the vertical area available in the graphic area and are overlapping.

    * Manual: each graphs can be placed by the user in a portion of the vertical area; the graphs can be overlapping. The vertical coordinates of the rectangle reserved to the channel correspond to the settings of the Top and Bottom fields of the Graphs Options for each channel.

The layout is associated with the following configuration window that permits to set layout for each area.




  * Y Scale text mode: Configure the orientation of the scale labels (Automatic, Vertical and Horizontal).



  * X Axis Mode: sets the X axis in the time scale (Time) or in the distance scale (Distance) or in Revolutions scale (Rpm). See also [X Scale](GRAWndFeatures.md#GRAFeatureXScale) for a detailed explanation of the modes..

    * Time:  In this mode the points in the graphs correspond to samples distributed based on the time of acquisition. The X axis displays the time scale in seconds.

    * Distance: the X axis displays the distance scale in meters.

    * Revolutions: the X axis displays the scale in engine revolutions unit.



  * Double X Axis: If checked, the window shows both Time X Axis and Distance X Axis; if the X Axis is configured as “Revolutions”, the second X Axis will be always Time.

  * Time Scale: sets the display Time mode for the X scale.

  *     * Relative: the time instant displayed in the values box of the X axis is a relative instant that depends on the length of the lap with initial instant equal to 0.

    * Absolute: the time instant displayed in the values box of the X axis is an absolute time (expressed in hh:mm:ss:mmm) that depends on the length of the lap with initial time equal to lap start time. Also, the absolute value of the current instant of the lap in displayed in a box by the side of the scale.

  * Real Time Settings: displays the Real Time for the window (Default or Custom). To modify the parameter, edit the associated configuration window.




  1.      1.         * Enable custom settings Real Time Options: enables customized Real Time settings.

        *           * X Width: width of the range for the X scale, in seconds or meters. (15000 meters is the maximum allowed size in distance mode, while 240 seconds is the maximum amount of seconds allowed in time mode)

          * X Scroll: offset value for the X axis in seconds and meters; this value is configurable only if the Enable smoothly scroll in Setup / General / Real Time is unchecked.

          * Show Laps: display mode of the Lap

          *             * Continuous: the graphs are displayed in continuous mode also when closing a lap.

            * Single Lap: when a finish line closes, the window resets and the channel starts back from 0.

          * Compare:

          *             * Real Time: The comparison is updated in real time.

            * Background: the comparison lap is shown in background.



  * Cursor value mode: sets the type of Cursor values. Transparent and Follow are available only in Post Processing.

  *     * Left: Cursor values are on the left side of the window

    * Right: Cursor values are on the right side of the window (default)

    * Transparent: Cursor values are visible in the left side of the graphic area with a transparent effect.

    * Follow graph cursor: Cursor values are visible as Transparent but in this mode they are always attached to the vertical cursor.




##### Graphs Options

Enables to configure the settings specific for each channel of the window. Each line identifies a configured channel, while the columns identify the fields to be configured. Each element of the grid can be edited by double clicking with the mouse or by pressing the space bar. Multiple sections are possible through the CTRL and SHIFT keys.

Between the header and the channels list there is a line with a checkbox as shown in the picture below.

In this line it is possible to enable or disable the area visualization. See Graph Areas for further details.

REMARK: 

When a channel is dragged into a window, the corresponding settings from Channel Parameters are copied into its Graph Options, if available.

The following options are considered: Color, Style, Unit, Format, Grid, Scale, Group, Filter, and Peak-to-Peak.

For most of these, the value in Graph Options takes priority. However, for Grid, Filter, and Scale, the value in Channel Parameters takes precedence if defined.

  * Label: shows the name of the channel. The name of the channel can be edited and can become a math expression if the sign = comes first.

  * Display

  *     * Color: shows the color of the channel or markers graph. To modify the setting, edit the channel by opening the Channel Properties window where fonts and styles can be configured.

    * Style: shows the style of the channel graph. To modify the setting, edit the channel by opening the Channel Properties page where fonts and styles can be configured.




Enable Custom Font settings: enables the local font configuration for the selected channels.

  1.      * Family font: sets the font.

     * Font dimension: sets the font size.

     * Scalable: enables the adapting of the font size in relations to the window size.




Line settings

  1.      1.         1.            * Style: sets the style of the graphs line

           *              * None: no line is drawn

             * Line: continuous line

             * Step: stepped line

             * Fill Down: continuous line with colored bottom area

             * Fill Up: continuous line with colored top area

             * Bordered: continuous line with border

           * Weight: sets the width of the line in pixels.

           * Line color: sets the line color.

           * Border color: color for the line border




Marker Settings section

  1.      1.         1.            * Style: style of the markers, graphic elements used to represent the marker.

           *              * None: no markers are drawn

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




### Multi color section

  1.      1.         1.            * Enable: enable multi color

           * Enable Custom Channel: This field allows to customize the channel that will be used to calculate the color values.

           * Configure: configure multi color channel; the button opens a window like shown in the picture below.




Division: number of colored bands. The value must be included between 1 and 4. When changing the value of steps, the start-end-color grid redraws the number of rows configured. In the rows it is possible to manually configure the values and the colors.

Auto: recalculate steps values using Start of the first row and End of the last row as fixed values.

Color Mode: In the rows of the start-end-color grid it is always possible to manually configure the colors. Otherwise color can be sets with color mode combo. If Custom is selected, the button generate a group of default colors. If Gradient is selected, the same button is used to choose the base color of gradient. If Blend is selected, the buttons Start color and End color are used to choose the start and the end colors of blend.

### Box Value

  1.      * Border: show the outline border of cursor value.




  * Unit: Enables to display the measurement unit of the channel.

  *     * Format: displays the style to show the current channel value. To modify the setting, open the configuration window that enables to configure the setting for the display format of the channel values.




In the combo on the left the numeric format is selected, in the combo on the right, the number of decimals is selected. Please find to follow the list of possible formats

  1.      1.         1.            * Auto: the format is kept unchanged

           * Dec: the decimal format allows max 5 digits after the comma.

           * Numeric: the numeric format allows max 15 digits after the comma.

           * Scientific: the scientific format allows max 15 digits after the comma; the result is written in exponential form.

           * Hex: hexadecimal format; the decimals cannot be configured.

           * Bin: binary format; the decimals cannot be configured.

           * Ascii: text format; the decimals cannot be configured.




  1.      * Grid: displays the setting to enable the grid specific of the rectangle of the graphic area reserved to the channel. The window enabling the grid and the configuration of the parameters is the following:




  1.      1.         * Enable Local Grid settings: enables to display the grid with the customized settings.

        *           * Style: sets the style of the grid

          *             * Simple: the grid is formed by continuous lines.

            * Cross: the grid is formed by crosses indicating the intersection of the vertical and horizontal divisions.

          * Color: color of the grid

          * Line style: sets the style of the grid line (valid if Style Simple is set)

          *             * Solid: continuous line

            * Dash: dashed line

            * Dot: dotted line

            * DashDot: dashed line alternated with 1 dot

            * DashDotDot: dashed line alternated with 2 dots




X

  1.      1.         1.            * Mode: calculation mode of the horizontal divisions

           *              * Auto, displays an automatic number of equidistant divisions.

             * Off, no division is displayed

             * Fixed, displays a fixed number of equidistant divisions

             * Step, displays the divisions at fixed intervals equal to the Step value.

             * Manual, displays the division in correspondence with the values on the X axis set by the user in the info box Values .

           * Step: fixed step to calculate the horizontal divisions (a division for each Step), valid if Mode is set to Step

           * Div.: number of horizontal divisions to be displayed, valid if Mode is set at Auto or Fixed

           * Values: list of values on the X axis corresponding to the divisions, valid if Mode is set at Custom. The list of values can be added directly in the text box, using as division the character ';'




Y

  1.      1.         1.            * Mode: calculation mode of the vertical divisions

           *              * Auto, displays an automatic number of equidistant divisions.

             * Off, no division is displayed

             * Fixed, displays a fixed number of equidistant divisions

             * Step, displays the divisions at fixed intervals equal to the Step value.

             * Manual, displays the division in correspondence with the values on the Y axis set by the user in the info box Values

           * Step: fixed step to calculate the vertical divisions (a divisions for each Step), valid if Mode is set to Step

           * Div: the number of vertical divisions to be displayed, valid with Mode set at Auto or Fixed

           * Values: list of values on Y axis corresponding to the divisions, valid with Mode set at Custom. The list of values can be added directly in the text box, using as division the character ';'.




  1.      * DataSet: displays the setting of the Dataset mode. 




The setting shows All when the channel is displayed for all, otherwise it shows the list of the numbers corresponding to the selected Datasets. By default they can all be displayed; to modify the selection, edit the configuration by opening the following window.

The configuration page depends on the licence enabled.

In the picture below there is an example of DataSet Page. You can simply choose from the various DataSets deciding which one to enable and which one not.

Here is another example of how the DataSet page can be structured.

The list always shows the maximum number of available Datasets.

The user can enable or disable the corresponding Dataset to be displayed.

Comparison mode allows the user to customize comparison settings of the General Setup in a more advanced way, but only in the local window. This enables Custom check comparison settings of the corresponding slot to be replaced with the channel properties; every single property (line style, line weight...) can be modified enabling the related check and editing new value.

When new channel is added, this setup inherits the actual Dataset configuration in General Setup.

  * Scale

  *     * Show: show or hide the Y scale in Manual or Overlay mode; it is not active in Parallel mode.

    * Local: enables the automatic display of the Y scale.

    * Min: sets the minimum value of the channel if Local is checked.

    * Max: sets the maximum value of the channel if Local is checked.

    * Bottom (%): sets the vertical arrangement of the graphs in the vertical area (Bottom).

    * Top (%): sets the vertical arrangement of the graphs in the vertical area (Top).

    * Group: assigns the channel to a group for Y scale auto scaling, if Local is not checked. See the Autoscale Groups of Channels function in Graph Analysis Windows for details.



  * Comparison mode: shows the comparison options for the selected channel. This allows user to specify which DataSet from the active selection are to be displayed together for the particular channel.

  * Auto: this button selects all the DataSet

  * Difference: If the Difference button is set, the graph will show the difference between the data in DataSet 1 and each of the other DataSet selected.

  * Average: if the Average button is set, the graph will show the average between the data in selected DataSets.




If the number of enabled DataSets into Comparison dialog is less than the active ones, they are highlighted by # character and its index (e.g. RPM avg#BC).

Note : If the licence grants [reference lap](../../../DataBrw/Reference_Lap.md) the first slot of the list will be the reference lap.

  * Reference Line: configures a horizontal line at some special values: Minimum, Maximum, Average and custom values, configured by user configured by the user through the setting of a threshold. In Real Time mode only custom lines are available.




Each value can be enabled or disabled by a check box and has the following settings:

  1.      1.         * Color: sets the line color.

        * Style: sets the style of the graphs line

        *           * Line: continuous line

          * Dashed Line: dashed line

          * Dot Line: dotted line

          * Dashed Dot Line: dashed line alternated with 1 dot

        * Weight: sets the width of the line in pixel.




For custom value there is the following setting more:

  * Threshold: sets the reference value of the line




  * Alarm Properties: configures the style of the alarms of this channels.




  1.      * Local Alarm Properties: enables the local configuration; if this options is set as off (e.g. when adding new channels), the configuration is taken from channel parameters.

     *        * Show upper warning limit: allows displaying the upper limit of the alarm.

       * Show lower warning limit: allows displaying the lower limit of the alarm.

       * Style: style of alarms limits: Line or Filled.

       * Line Style: needs Line as Style: Line or Dashed Line.

       * Color: color of Alarms limits.

       * Weight: weight of lines (needs Line as Style and Line as Line Style)



  * Condition: The condition sets a channel that is used to limit the calculation of the graph values for those in which the condition is true.

  * Filter: shows the configuration of filters that are going to be applied to the channels. To change the settings, open the configuration window.




The filters can be added, removed or sorted using the buttons on the Options area. The available filters are:

  1.      * OFF: Channel values are displayed as logged

     * Run Average: Applies a moving average filter to the channel. Filter length is defined in milliseconds. If set to zero, the filter is not calculated.

     * FFT filters applies a combination of frequency domain filters to the channel. The frequency content of the signal in the range(s) defined by the cut-off frequency is set to zero and the data is reconstructed in the time-domain. The four types available are:



  1.      *        * FFT Low Pass: maintains frequency content below the cut-off freq. Freq1

       * FFT High Pass: maintains frequency content above the cut-off freq. Freq1

       * FFT Band Pass: maintains frequency content in the range Freq1< frequency < Freq2

       * FFT Band Stop: eliminates frequency content in the range Freq1< frequency < Freq2

     * IIR Low Pass: Infinite Impulsive Response Low Pass filter is a recursive filter that maintains frequency content below the cut-off frequency

     * IIR High Pass: Infinite Impulsive Response High Pass filter is a recursive filter that maintains frequency content above the cut-off frequency.




IMPORTANT: Filters can be customized also in the [channel parameters window](../../../WTXSetup/WndChannelsParametersSetup/Channels_Parameters.md). 

Filter can be configured to OFF or with a sequence of 1 or more filters for each channel. The filters are then applied, in the order that they have been sorted by the user in the configuration Windows.

If a channel has been configured with filter both in the "[Channel Parameter](../../../WTXSetup/WndChannelsParametersSetup/Channels_Parameters.md)" window and in the local configuration window (this window), the applied filter are only the ones that have been configured in the local configuration window. 

#### Event Page

The Event page enables to configure the events of the [Graph](GRAWndOverview.md) window.

  * Enable show events: enabled to display the events.

  * View Mode: sets the display mode of the events.

  *     * All: all events are displayed.

    * Manual: only the events selected by the user are displayed.




The list shows the customized events configured by the user.

Each event in custom mode can be managed using the buttons on the Options bar to add, remove, modify and move the elements in the list. The button Add opens the event page in the Channel Browser and the user can drag events in the list.

#### Sections Report Page

The Sections Report page enables to configure the settings of the map. See the [Track Editor](../../../Tools/TrackEditor/Track_Editor.md) to learn how WinTAX4 manage and create the Track sections used in this configuration.

  * Enable custom settings: enables the customized settings.

  * Track Sections Numeration: Setting is linked to the possible display on the map according to the track editor. 

  *     * All: all events of the map are displayed,

    * Corner: only corners are displayed

    * Straight: only straights are displayed

    * Manual: the list of the sections is displayed and each section can be visualized or not. Each section can also be customized for the label to be shown. Sections can be added or removed thanks to the Options commands. The Default Color button sets red and green alternated colors.




Elements can be added or removed from the list through the buttons of the Options bar.

#### Script Page

The Script page enables to configure scripts linked to the events of the [Graph](GRAWndOverview.md) window or of the application, in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

####  Menu

The menu of the Graph Configuration window allows the access to the following commands, divided in sub menus:

##### File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the current settings of the graphic window |
| Cancel |  | Closes the window without applying the current settings to the configuration of the graphic window |
| Load |  | Opens a dialog window to select a configuration file for the Graphs (.gra) window to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file for a Graphs (.gra) window to save the current settings. |
  
#####  Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies to clipboard the configurations of the channels selected in the list of the Graphs section and it removes them from the list of the Graphs section. |
| Copy | Ctrl + C | Copies to clipboard the configurations of the channels selected in the list of the Graphs section |
| Paste | Ctrl + V | Pastes the configurations of the channels available in clipboard adding them to the list of the Graphs section |
  
##### View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use |
  
##### Graph Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Graph | Adds a new element to the Graphs list of channels configurations |
| Remove Graph | Removes from the Graphs list the configurations of the selected channels |
| Move Up | Moves up by one position the selected elements in the Graphs list . If Area title bar is selected, the command acts on the whole Area. |
| Move Down | Moves down by one position the selected elements in the Graphs list. If Area title bar is selected, the command acts on the whole Area. |
| Add Area | Adds a new Area. See graph area for further details. |
| Remove Area | Remove an existing Area. See graph area for further details. |
  
#### 

####  Toolbar

The toolbar of the Graph Configuration window allows the access to the following commands:

| DESCRIPTION | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode allowing to keep displayed the configuration window while the application window are in use |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel command of the File menu) |
| Apply | Applies the current settings to the configuration of the graphic window (similar to the Apply command of the File menu) |
| Load | Similar to the Load command of the File menu |
| Save As | Similar to the Save As command of the File menu |
| Previous page | Similar to the Previous page command of the View menu |
| Next page | Similar to the Next page command of the View menu |
| Cut | Similar to the Cut command of the Edit menu |
| Copy | Similar to the Copy command of the Edit menu |
| Paste | Similar to the Paste command of the Edit menu |
| Add | Similar to the Add Graph command of the Graph menu |
| Remove | Similar to the Remove Graph command of the Graph menu |
| Move Up | Similar to the Move Up command of the Graph menu |
| Move Down | Similar to the Move Down command of the Graph menu |
| Add Area | Similar to the Add Area command of the Graph menu |
| Remove Area | Similar to the Remove Area command of the Graph menu |
| Channel Browser | Displays the pop-up menu to select the page in the Channel Browser window. |
  
## 

####  Pop-up Menu

The pop-up menu of the Graph Configuration window can be displayed by clicking with the right button of the mouse on the Options page.

The pop-up menu of the Graph Configuration window allows the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Cut | Similar to the Cut command of the Edit menu |
| Copy | Similar to the Copy command of the Edit menu |
| Paste | Similar to the Paste command of the Edit menu |
| Add Graph | Similar to the Add Graph command of the Graph menu |
| Remove Graph | Similar to the Remove Graph command of the Graph menu |
| Move Up | Similar to the Move Up command of the Graph menu |
| Move Down | Similar to the Move Down command of the Graph menu |
| Restore Default | Sets in the channel the settings configured in the Channel Parameters |
| Set As Default | Configures the settings of the channel in the Channel Parameters |
  
### 

### Graph Areas

Areas are a further possibility to group the channels. Each area has a defined space configured on the graphics window.

You can divide in up to ten areas the graph window. You can add and remove areas with the Add Area and Remove Area commands. To add a channel to the desired area, just drag the channel from the channel browser to the desired area. The check box in the subtitle area permits to show or hide the corresponding area. The area name can be edited by double clicking with the mouse or by pressing the space bar. It is possible to change the order of areas in graph configuration via arrow keys (Move up and down). To use Move Up and Move down with areas, the header of one or more areas must be selected. You can also add a new area by Dragging & Dropping channels below the X axis of the graph window.

##### Related Issues

  * [Graph window](GRAWndOverview.md)

  * [Elements of the window](GRAWndItems.md)

  * [Functions](GRAWndFeatures.md)

  * [Commands](GRAWndCommands.md)


