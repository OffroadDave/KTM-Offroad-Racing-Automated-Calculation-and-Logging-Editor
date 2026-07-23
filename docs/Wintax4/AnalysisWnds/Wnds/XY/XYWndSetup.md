###  Configuration Window

The XY Configuration window enables to set the aspect of the graphic windows [XY](XYWndOverview.md); displays the pages: Options, Event, Script.

The window has moreover a menu, a toolbar and a popup menu that ease the access to the configuration and management commands of the window itself.

####  Options Page

The Options page enables the configuration of the graphic aspect of the windows; the page is divided into two sections: Window and Graphs. Each configurable value can be edited by double clicking with the mouse or by pressing the space bar on the selected element.

### 

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

#####  Window Options

It enables to configure the general settings of the window.

  * Custom Color: enables the setting of the window customized colors.

  * Window: sets the window background color.

  * Graph: sets the background color of the graphic area.

  * Border: sets the borders color of the graphic areas of the window.

  * Grid: displays the setting to enable the grid common to all channels graphs, in the graphic area of the window. To modify the parameter, edit the associated configuration window.




  1.      * Enable Custom Grid settings: enables the grid display with the customized settings.

     *        * Style: sets the style of the grid

       *          * Simple: the grid is formed by lines drawn with the styles configured in Line Style settings.

         * Cross: the grid is formed by crosses indicating the intersection of the vertical and horizontal divisions.

       * Color: color of the grid

       * Line style: sets the style of the grid line (valid if the Style Simple is set)

       *          * Solid: continuous line

         * Dash: dashed line

         * Dot: dotted line

         * DashDot: dashed line alternated with 1 dot

         * DashDotDot: dashed line alternated with 2 dots

     * X

     *        * Mode: calculation mode of the horizontal divisions

       *          * Auto, displays an automatic number of equidistant divisions.

         * Off, no division is displayed

         * Fixed, displays a fixed number of equidistant divisions

         * Step, displays the divisions at fixed intervals equal to the Step value.

         * Manual, displays the divisions in correspondence with the values on the X axis set by the user in the text box Values.

       * Step: at Step

       * Div: the number of horizontal divisions to be displayed, valid with Mode set at Auto or Fixed

       * Values: list of values on Z axis corresponding to the divisions, valid with Mode set at Manual. The list of values can be added directly in the text box, using as division the character ';'.

     * Y

     *        * Mode: calculation mode of the vertical divisions

       *          * Auto, displays an automatic number of equidistant divisions.

         * Off, no division is displayed

         * Fixed, displays a fixed number of equidistant divisions

       * Step: not used

       * Div: number of vertical divisions to be displayed, valid with Mode set at Auto or Fixed

       * Values: not used.




If the window grid is disabled, the window uses the setting of the Default Grid section, in the [Default Appearance](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgDefaultAppearence) page of the [General Setup](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md) window.

  * Layout: sets the vertical arrangement of the graphs; the effects of this choices are visible only in Graph Mode, since the other modes shows only one channel. 

  *     * Parallel: the channels graphs are vertically arranged proportionally to the number of channels on the Y axis. 

    * Overlay: the channels graphs share the whole graphic area and are overlapping.

    * Manual: the channels graphs are vertically arranged according to user's settings.

  * Y Scale text mode: Configure the orientation of the scale labels (Vertical or Horizontal).

  * Graph Mode: sets the display mode of the graphs.



  1.      * Graph: This mode displays the channels in graphical representation as in Graph window.

     * Smooth: This mode displays the graph using the density of values in small grid rectangles showing them with a variable color scale depending on the density. Smooth uses only the first X and Y channels.

     * Percent: This mode displays the graph using the density of values in small grid rectangles like Smooth, but shows the values instead of colored rectangles. Percent uses only the first X channel and its Y channel.



  * Color Mode: This option is active only in Smooth mode and is concerning to the coloring of the small rectangles.

  *     * If Manual is selected, the window will generate a group of default colors.

    * If Gradient is selected, the button Color is used to choose the base color of gradient.

    * If Blend is selected, the buttons Start color and End color are used to choose the start and the end colors of blend.




To modify the mode, edit the related configuration window.

Division: number of colored bands. The value must be included between 1 and 10. Changing the division value will also recalculate the number of cells in the color table. Every row of the table can be configured with it is own color..

Color Mode: In the rows of the color grid it is always possible to manually configure the colors. Otherwise color can be set with color mode combo.

  1.      1.         * If Manual is selected, the window generates a group of default colors.

        * If Gradient is selected, the Color button is used to choose the base color of gradient.

        * If Blend is selected, the buttons Start color and End color are used to choose the start and the end colors of blend.

Reverse: reverses the order of the colors.



  * Smooth Density (%): This option is active only in Smooth mode and regards the density of pixels displayed. The default value is 50%.



  * Scale Mode: sets the display mode of the Y scales

  *     * Normal: Y scales are displayed with the name of the channels at the top of each scale.

    * Legend: To save space, the name of the channels are removed from the top of each scale; these names are grouped into a legend displayed below the graph. If many channels are displayed, the labels of the legend are ordered by column.




#####  Graphs Options

It enables configuration of the settings specific for each channel to be displayed in the window. Each line identifies a configured channel, while the columns identifies the fields to be configured. Multiple selections are possible through the CTRL and SHIFT keys.

Below the channels grid, there is an "Add channels..." row, which permits to create groups of channels. The commands for adding channels act on the group, or on the groups, in which there is at least one selected channel. Since it is valid multiple selection, if more than one group is selected, a channel is added in each of them. The maximum number of groups permitted in XY window is 4: X1, X2, X3, X4.

Each group has the same schema and the same rules but they are independent of each other

  * Axis: identifies the X or Y axis on which the channel is configured. The number following the X (X1, X2, X3, X4) identifies the group of channels and is limited to 4; the number following the Y (Y1, Y2, ..., Yn, ...) identifies the number of the channels in its group and is unlimited. This field isn't editable.

  * Label: displays the name of the channel. The name of the channel can be edited and can become a math expression if the sign = comes first.

  * Color: sets the color of the channel graphs.

  * Style: displays the style of the channels graphs. To modify the setting, edit the channel by opening the Channel Properties page where font and styles are configurable.




  * Enable Custom Font settings: enables the local font configuration for the selected channels.

  *     1.        1.           * Family Font: sets the font.

          * Font Dimension: sets the font size.

          * Scalable: enables the adapting of the font size in relations to the window size.

  * Line settings

  *     1.        1.           * Style: sets the style of the graphs line

          *             * None: no line is drawn

            * Line: continuous line

            * Step: stepped line

            * Fill Down: continuous line with colored bottom area

            * Fill Up: continuous line with colored top area

            * Bordered: bordered continuous line

          * Weight: sets the width of the line in pixel.

          * Line color: sets the line color.

          * Border Color: color for the line border

  * Marker settings

  *     1.        1.           * Style: style of the markers, graphic elements used to represent the marker.

          *             * None: no markers are drawn

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



  * Box Value section

  *     1.        1.           * Border: show the border around the box of the cursor value.



  * Unit: enables to display the measurement unit of the channel which is configured in Channel Parameters.



  * Format: displays the style to display the current channel value. To modify the settings, open the configuration window.




In the combo on the left the numeric format is selected, in the combo on the right, the number of decimals is selected. Please find to follow the list of possible formats

  1.      * Auto: the format is kept unchanged

     * Dec: the decimal format allows max 5 digits after the comma.

     * Numeric: the numeric format allows max 15 digits after the comma

     * Scientific: the scientific format allows max 15 digits after the comma; the result is written in exponential form.

     * Hex: hexadecimal format; the decimals cannot be configured.

     * Bin: binary format; the decimals cannot be configured.

     * Ascii: text format; the decimals cannot be configured.



  * Grid: displays a checkbox that permits to enable a specific grid for the rectangle of the graphic area dedicated to the channel. The window allowing to enable the grid and configure its parameters is the following:




  1.      * Enable Local Grid settings: enables the grid visualization with the customized settings.

     *        * Style: sets the style of the grid

       *          * Simple: the grid is formed by continuous lines.

         * Cross: the grid is formed by crosses indicating the intersection of the vertical and horizontal divisions.

       * Color: color of the grid

       * Line style: sets the style of the line of the grid

       *          * Solid: continuous line

         * Dash: dashed line

         * Dot: dotted line

         * DashDot: dashed line alternated with 1 dot

         * DashDotDot: dashed line alternated with 2 dots



  * X



  *     1.        * Mode: calculation mode of the horizontal divisions

       *          * Auto, displays an automatic number of equidistant divisions.

         * Off, no division is displayed

         * Fixed, displays a fixed number of equidistant divisions

         * Step, displays the divisions at fixed intervals equal to the Step value.

         * Manual, displays the divisions in correspondence with the values on the X axis set by the user in the text box Values .

       * Step: fixed step to calculate the horizontal divisions (a division for each Step), valid if Mode is set at Step

       * Div: number of horizontal divisions to be displayed, valid if Mode is set at Auto or Fixed

       * Values: list of values on the X axis corresponding to the divisions, valid if Mode is set at Custom. The list of values can be added directly in the text box, using as division the character ";"

  * Y

  *     1.        * Mode: calculation mode of the vertical divisions

       *          * Auto, displays an automatic number of equidistant divisions.

         * Off, no division is displayed

         * Fixed, displays a fixed number of equidistant divisions

         * Step, displays the divisions at fixed intervals equal to the Step value.

         * Manual, displays the divisions in correspondence with the values on the Y axis set by the user in the text box Values .

       * Step: fixed step to calculate the vertical divisions (a division for each Step), valid if Mode is set at Step

       * Div: number of vertical divisions to be displayed, valid if Mode is set at Auto or Fixed

       * Values: list of values on the Y axis corresponding to the divisions, valid if Mode is set at Custom. The list of values can be added directly in the text box, using as division the character ";"



  * Dataset: in this page you can select one or more or all Datasets for comparison.




  * Scale



  1.      * Local: enables the automatic display of the Y scale.

     * Min: sets the minimum channel value.

     * Max: sets the maximum channel value.

     * Mode: switch between linear or logarithmic scale

     * Bottom (%): sets the vertical arrangement of the graphs in the vertical area (Bottom).

     * Top (%): sets the vertical arrangement of the graphs in the vertical area (Top).

     * Group: Allows the selection of a group id for each of the available channels, and for up to 10 groups. All channels in the same group will be displayed in the XY graph using the same scale. This is valid for both post processing and realtime. When a group ID is already assigned to at least one other channel, it is marked with the " * " character.



  * Filter: (available only in Post Processing mode) displays the settings on the filters applied to the channel. To modify the setting, open the configuration window.




The filers can be added, removed or moved through the buttons on the Options bar. The filters available are:

  1.      * OFF: channel values are displayed as logged

     * Run Average: applies a moving average filter to the channel. Filter length is defined in milliseconds. If set to zero, the filter is not calculated.

     * FFT filters applies a combination of frequency domain filters to the channel. The frequency content of the signal in the range(s) defined by the cutoff frequency is set to zero and the data is reconstructed in the time-domain. The four types available are:



  1.      *        * FFT Low Pass: maintains frequency content below the cutoff freq. Freq1

       * FFT High Pass: maintains frequency content above the cutoff freq. Freq1

       * FFT Band Pass: maintains frequency content in the range Freq1< frequency < Freq2

       * FFT Band Stop: eliminates frequency content in the range Freq1< frequency < Freq2

     * IIR Low Pass: Infinite Impulsive Response Low Pass filter is a recursive filter that maintains frequency content below the cut-off frequency

     * IIR High Pass: Infinite Impulsive Response High Pass filter is a recursive filter that maintains frequency content above the cut-off frequency




IMPORTANT: Filters can be customized also in the [channel parameters window](../../../WTXSetup/WndChannelsParametersSetup/Channels_Parameters.md). 

Filter can be configured to OFF or with a sequence of 1 or more filters for each channel. The filters are then applied, in the order that they have been sorted by the user in the configuration Windows.

If a channel has been configured with filter both in the "[Channel Parameter](../../../WTXSetup/WndChannelsParametersSetup/Channels_Parameters.md)" window and in the local configuration window (this window), the applied filter are only the ones that have been configured in the local configuration window. 

  * Analysis: displays the setting for advanced analysis applied to the channel. To modify the setting, open the properties window the Analysis page.




  1.      * Best Fit Settings

     *        * Order: degree of the polynomial function used to calculate the Best Fit curve. The Off value or one of the predefined values proposed in the combined box can be selected, to avoid setting the Best Fit curve. The maximum degree is 6.

       * Local calculation range: This flag, if it is enabled, permits to customize the y scale of the Best Fit curve with manual range. If the flag is unchecked (default), the range of y scale of the Best Fit curve will be automatically shown as the range of the y scale of the channel.

       * Min: minimum value of manual scale of the Best Fit curve

       * Max: maximum value of manual scale of the Best Fit curve

       * Color: color of the Best Fit curve. To modify the color open the window and select the color by clicking with the left button of the mouse.

       * Style: style for Best Fit line.

       * Weight: weight for Best Fit line.

     * Horizontal Cursor Line Settings

     *        * Show: enable horizontal cursor lines displaying.

       * Color: color for horizontal cursor lines.

       * Style: style for horizontal cursor lines.

       * Weight: weight for horizontal cursor lines.

     * Reference Best Fit

     *        * Order: degree of the polynomial function used to calculate the Reference Best Fit curve. The Off value or one of the predefined values proposed in the combined box can be selected, to avoid setting the Reference Best Fit curve. The maximum degree is 6.

       * Color: color of the Reference Best Fit curve. To modify the color, open the window to select the color by clicking with the left button of the mouse.

       * Style: style for Reference Best Fit line.

       * Weight: weight for Reference Best Fit line.

       * Coefficients B0, B1, B2, B3, B4, B5, B6: the Reference Best Fit is a manual constructed curve which could be read as an ideal Best Fit. In this section it is possible to insert the coefficients, also in scientific format, to calculate the curve.

     * Interpolation

     *        * Value: value of the customized sampling frequency. The Off value or one of the pre-defined frequencies proposed in the combined box (Range between 1 Hz and 10 Hz) can be selected, to keep the value set by default.

     * Condition




Adding a condition in the text box, on the graphs only the points complying with the condition itself will be displayed. With the button the conditions page opens on the channel browser to add the condition with the drag & drop.

  1.      * Information for all graphs

     *        * View Best Fit information in transparent: Selecting this flag, the Best Fit information are displayed within the graphics window like in picture below. The flag is global and applies to all channels in the window.




#### Event Page

The Event page allows configuring the events of the [XY](XYWndOverview.md) window.

Enable show events: must be enabled to display the events.

View Mode: sets the display mode of the events.

  * All: all events are displayed.

  * Manual: only the events selected by the user are displayed.




The list shows the customized events configured by the user.

Each event can be configured by using the Options bar button(to add, remove, modify, move in the list). The Add button opens the Channel Browser in the event page; then the user can drag the selected events in the list.

####  Script Page

The Script page enables to configure scripts of the events in the [XY](XYWndOverview.md) window or in the application, in Visual Basic environment. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

####  Menu

The menu of the XY Configuration window allows the access to the following commands, divided in sub menus:

##### File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the current settings of the window |
| Cancel |  | Closes the window without applying the current settings |
| Load |  | Opens a dialog window to select a XY configuration file to be loaded. |
| Save As |  | Opens a dialog window to select a XY configuration file .XY), on which the current settings can be saved |
  
##### Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies to clipboard the configurations of the channels selected in the list of the Graphs sections and removes them from the list of the Graphs section. |
| Copy | Ctrl + C | Copies to clipboard the configurations of the channels selected in the list of the Graphs |
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
| Move Up | Moves up by one position the selected elements in the Graphs list |
| Move Down | Moves down by one position the selected elements in the Graphs list |
  
##### Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
## 

####  Toolbar

The toolbar of the XY Configuration window allows the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode allowing to keep displaying the configuration window while the application window are in use |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel command of the File menu) |
| Apply | applies the current settings to the configuration of the graphic window (similar to the Apply command of the File menu) |
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
| Channel Browser | Displays the pop-up menu to select the page in the Channel Browser window. |
  
####  Pop-up Menu

The pop-up menu of the window can be displayed by clicking the right button of the mouse on the Options page.

The pop-up menu allows the access to the following commands:

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
| Set As Default | Sets the channel settings in the Channel Parameters. |
| Edit... | Gives the possibility to edit the selected parameter (similar to the double clicking). |
  
##### Related Issued

  * [XY window](XYWndOverview.md)

  * [Elements of the window](XYWndItems.md)

  * [Functions](XYWndFeatures.md)

  * [Commands](XYWndCommands.md)


