### Configuration Window

The XYZ Configuration Window allows setting the look of the XYZ graphic windows; it has the following pages: Options, Event, Script .

The window also includes a menu, a toolbar and a pop-up menu that ease the access to the configuration and management commands of the window itself.

#### Options Page

The Options page allows configuring the graphic aspect of the XYZ windows and it is divided into two sections : Window and Graphs. Each configurable value can be edited by double clicking with the mouse or by pressing the space bar on the selected element.

##### 

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

##### Window Options

It allows configuring the general settings of the window.

  * Custom color: Enables the settings of the customized colors for the window.

  * Window: Sets the background color of the window.

  * Graph: Sets the background color of the graphic area.

  * Border: Sets the borders color of the window area.

  * Grid: displays the setting to enable the grid common to all the channels graphs in the graphic area of the window. The parameter can be modified by editing the corresponding configuration window.




  1.      * Enable Custom Grid settings: Enables the grid display with the customized settings. If the window grid is disabled, the Default Grid uses the [Default Appearance](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgDefaultAppearence) page of the [General Setup](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md) window (General Settings, general configuration of the WinTAX4 environment).

     *        * Style: Sets the style of the grid.

       *          * Simple:the grid is formed by lines drawn with the styles configured in Line Style settings.

         * Cross: the grid is formed by crosses indicating the intersection of the vertical and horizontal divisions.

       * Color: Color of the grid.

       * Line style: Sets the style of the grid line (valid if the simple style is enabled).

       *          * Solid: continuous line

         * Dash: dashed line

         * Dot: dotted line

         * DashDot: dashed line alternated with 1 dot

         * DashDotDot: dashed line alternated with 2 dots

     * X

     *        * Mode: Calculation mode of the horizontal divisions.

       *          * Auto, displays an automatic number of equidistant divisions.

         * Off, no division is displayed

         * Fixed, displays a fixed number of equidistant divisions

         * Step, displays the divisions at fixed intervals equal to the Step value.

         * Manual, shows the divisions in correspondence with the values on the X axis set by the user in the text box Values setting.

       * Step: Fixed step to calculate the horizontal divisions (a division for each Step), valid with Mode set at Step.

       * Div.: Quantity of horizontal division to be displayed, valid with Mode set at Auto or Fixed.

       * Values: List of values on the X axis corresponding to the divisions, valid with Mode set to Manual. The list of values can be directly added in the text box using ';' to separate the values.

     * Y

     *        * Mode: Calculation mode of the vertical divisions.

       *          * Auto, displays an automatic number of equidistant divisions.

         * Off, no division is displayed

         * Fixed, displays a fixed number of equidistant divisions

         * Step, displays the divisions at fixed intervals equal to the Step value.

         * Manual, shows the divisions in correspondence with the values on the Y axis set by the user in the Values setting.

       * Step: not used

       * Div.: Quantity of vertical division to be displayed, valid with Mode set to Auto or Fixed.

       * Values: not used.



  * Y Scale text mode: Configure the orientation of the scale labels (Vertical or Horizontal).

  * Graph Mode: sets the display mode of the graphs.

    * Graph: This mode displays the channels in graphical representation as in Graph window. (Dataset selection and comparison is supported on this window mode.)

    * Smooth: This mode displays the graph using the density of values in small grid rectangles showing them with a variable color scale depending on the density and Z channel. (Dataset selection is supported but compare is not supported.)

    * 3D: This mode displays the graph using a tridimensional representation where the third channel is the Z axis. In this mode the configuration window is limited as it appears in the following window. (Dataset selection is supported but comparison is not supported with this mode.)

    * Numeric Table: This mode allows the visualization of the values. Channels on axis X and Y are break-points while channel on the Z axis is the table numeric data (Dataset selection is supported but comparison is not supported with this mode.)




3D Settings: The XYZ window graph mode is configured as 3D the following TAB is available by double clicking on the 3D settings.

  * Use mouse wheel to zoom in/out: When enabled, allows the user to use the mouse wheel to enlarge and reduce the zoom level on the graph.

  * Show 3D info panel: Displays a box on the screen showing the horizontal and vertical angle of the graph 

  * Render mode: allows the user to select between orthographic and perspective view mode.

  * View axis label near the graph: allows to move label containing the name of the axis on the axis.



    * Table: Numeric Table: This view mode displays, in tabular form, the numerical distribution of the channel on the Z axis as arithmetic average: this value is calculated on break points represented by the channels on the X and Y. The configuration settings are limited as in the following image: 



  * Smooth Density (%): This option is active only in Smooth mode and is related to the density of pixels displayed. The default value is 50%.



  * Analysis: Sets the parameters for the advanced analysis of the graphs. The settings can be modified by changing the parameters on the configuration window. It is not just the channel to be analyzed like in [XY](../XY/XYWndOverview.md) but the whole window is analyzed; it anyhow refers to the channel set as Y.




  1.      * Best Fit Settings

     *        * Order: degree of the polynomial function used to calculate the Best Fit curve. The Off value or one of the predefined values proposed in the combined box can be selected, to avoid setting the Best Fit curve. The maximum degree is 6.

       * Color: color of the Best Fit curve. To modify the color, open the window to select the color by clicking with the left button of the mouse.

       * Style: style for Best Fit line.

       * Weight: weight for Best Fit line.

       * Single Components: If checked, the window displays a best fit curve for each cloud of Z point. in the picture below there are 




  1.      *      * Horizontal Cursor Line Settings

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




By adding in the text box a condition, the graphs will display only the points complying with the condition itself. The button opens the conditions page on the channel browser to add the condition with drag & drop.

  * Color Mode: Sets the parameters for the ranges and the colors of the Z axis.




If manual mode is selected, the window has the appearance of the picture below.

If mode Gradient or Blend are selected, the window has the appearance of the pictures below. The values in Start and End columns are configured manually or automatically. The manual configuration is active when the Z channel has local scale enabled.

  1.      * Division: it is a number into the range of 1-10; this value is the number of divisions of Z axis.



  1.      * Color Mode: In the rows of the start-end-color grid it is always possible to manually configure the colors. Otherwise color can be configured with this combo.

     * Reverse: Inverts the colors in the table.

     * Color / Start Color / End Color: Color selection in the different color modes.




##### Graphs Options

It enables to configure the settings specific for each channel to be graphically displayed. Each line identifies a configured channel, while the columns identify the fields to be configured. Multiple selections are possible with the CTRL and SHIFT buttons.

  * Axis: identifies the X, Y, Z axis on which the channel is configured. Max three channels can be configured, corresponding to X, Y, Z.

  * Label: displays the name of the channel. The name of the channel can be edited and can become a math expression if the sign = comes first.

  * Display

  *     * Color: displays the graphic color of the channel or of the markers. The setting can be modified editing the channel by opening the Channel Properties page where fonts and styles are configured.

    * Style: displays the graphic style of the channel. The setting can be modified editing the channel by opening the Channel Properties page where fonts and styles are configured.




  * Enable Custom Font settings: enables the local font configuration for the selected channels.

  *     1.        1.           * Family Font: sets the font.

          * Font Dimension: sets the font size.

          * Scalable: enables adaptive font size in relations to the window size.

  * Line settings

  *     1.        1.           * Style: sets the style of the graphs line

          *             * None : no line is drawn

            * Line : continuous line

            * Step : stepped line

            * Fill Down: continuous line with colored bottom area

            * Fill Up: continuous line with colored top area

            * Bordered : bordered continuous line

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



  1.      * Unit: Enables visualization of the measurement unit of the channel.

     * Format: shows the visualization style of the present channel value. The setting can be modified by opening the corresponding configuration window that allows configuring display format settings of the channel values.




In the combo on the left the numeric format is selected, in the combo on the right the quantity of decimals is displayed. Please find following the list of the available formats

  1.      1.         * Auto: The channel format is kept unchanged.

        * Dec: The decimal format allows max 5 digits after the comma.

        * Numeric: The numeric format allows max 15 digits after the comma.

        * Scientific: the scientific format allows max 15 digits after the format; the result is written in exponential form.

        * Hex: Hexadecimal format; the decimals cannot be configured.

        * Bin: Binary format; the decimals cannot be configured.

        * Ascii: Text format; the decimals cannot be configured



  1.      * Grid: displays enabled/disabled state of a grid specific for the rectangle of the graphic area dedicated to the channel. This window can be configured only for the Ychannel. The window enabling the grid and configuring the parameters is the following:




  1.      *        * Enable Local Grid settings: enables visualization of the grid with the customized settings.

       *          * Style: sets the style of the grid

         *            * Simple: the grid is formed by continuous lines.

           * Cross: the grid is formed by crosses indicating the intersection of the vertical and horizontal divisions.

         * Color: color of the grid

         * Line style: sets the style of the line of the grid

         *            * Solid: continuous line

           * Dash: dashed line

           * Dot: dotted line

           * DashDot: dashed line alternated with 1 dot

           * DashDotDot: dashed line alternated with 2 dots

     * X

     *        1.           * Mode: calculation mode of the horizontal divisions

          *             * Auto, displays an automatic number of equidistant divisions.

            * Off, no division is displayed

            * Fixed, displays a fixed number of equidistant divisions

            * Step, displays the divisions at fixed intervals equal to the Step value.

            * Manual, displays the divisions in correspondence with the values on the X axis set by the user in the text box Values .

          * Step: fixed step to calculate the horizontal divisions (a division for each Step), valid if Mode is set at Step

          * Div: number of horizontal divisions to be displayed, valid if Mode is set at Auto or Fixed

          * Values: list of values on the X axis corresponding to the divisions, valid if Mode is set at Custom. The list of values can be added directly in the text box, using as division the character ";"

     * Y



  1.      1.         * Mode: calculation mode of the vertical divisions

        *           * Auto, displays an automatic number of equidistant divisions.

          * Off, no division is displayed

          * Fixed, displays a fixed number of equidistant divisions

          * Step, displays the divisions at fixed intervals equal to the Step value.

          * Manual, displays the divisions in correspondence with the values on the Y axis set by the user in the text box Values .

        * Step: fixed step to calculate the vertical divisions (a division for each Step), valid if Mode is set at Step

        * Div: number of vertical divisions to be displayed, valid if Mode is set at Auto or Fixed

        * Values: list of values on the Y axis corresponding to the divisions, valid if Mode is set to Custom. The list of values can be added directly in the text box, using as division the character ";"



  * Scale



  1.      * Local: enables the automatic display of the Y scale.

     * Min: sets the minimum channel value.

     * Max: sets the maximum channel value.



  * Filter: displays the settings on the filters applied to the channels. The setting can be modified opening the configuration window.




Filters can be added, removed or moved through the buttons in the Options bar. The available filters are:

  1.      * OFF: Channel values are displayed as logged

     * Run Average: applies a moving average filter to the channel. Filter length is defined in milliseconds. If set to zero, the filter is not calculated.

     * FFT filters: applies a combination of frequency domain filters to the channel. The frequency content of the signal in the range(s) defined by the cutoff frequency is set to zero and the data is reconstructed in the time-domain. The four types available are:



  1.      *        * FFT Low Pass: maintains frequency content below the cutoff freq. Freq1

       * FFT High Pass: maintains frequency content above the cutoff freq. Freq1

       * FFT Band Pass: maintains frequency content in the range Freq1< frequency < Freq2

       * FFT Band Stop: eliminates frequency content in the range Freq1< frequency < Freq2

     * IIR Low Pass: Infinite Impulsive Response Low Pass filter is a recursive filter that maintains frequency content below the cut-off frequency.

     * IIR High Pass: Infinite Impulsive Response High Pass filter is a recursive filter that maintains frequency content above the cut-off frequency.




IMPORTANT: Filters can be customized also in the [channel parameters window](../../../WTXSetup/WndChannelsParametersSetup/Channels_Parameters.md). 

Filters can be configured to OFF or with a sequence of 1 or more filters for each channel. The filters are then applied, in the order that they have been sorted by the user in the configuration Windows.

If a channel has been configured with filter both in the "[Channel Parameter](../../../WTXSetup/WndChannelsParametersSetup/Channels_Parameters.md)" window and in the local configuration window (this window), the applied filter are only the ones that have been configured in the local configuration window. 

#### Event

The Event page enables to configure the events linked to the window.

Enable show events : Enables the display of the events.

View Mode: Sets the display mode of the events.

  * All: all events are displayed.

  * Manual: only the events selected by the user are displayed.




The list displays the customized events configured by the user.

Each event can be configured using the buttons on the Options bar (to add, remove an event, remove all events, modify and move items within the list).

#### Script

The Script page enables to configure scripts linked to the events of the [XYZ](XYZWndOverview.md) window or of the application, in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

#### Menu

The window menu allows the access to the following commands, divided into sub menus:

##### File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the present setting of the window. |
| Cancel |  | Closes the window without applying the present settings. |
| Load |  | Opens a dialog window to select a configuration file for the xyz window to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file for a xyz window to save the current settings. |
  
##### Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies to clipboard the configurations of the channels selected in the Graphs section list and removes them from the Graphs section. |
| Copy | Ctrl + C | Copies to clipboard the configurations of the channels selected in the Graphs section list. |
| Paste | Ctrl + V | Pastes the configurations of the channels available in clipboard, adding them to the Graphs section list. |
  
##### View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use. |
  
##### Graph Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Graph | Adds a new element to the Graphs list of channel configuration. |
| Remove Graph | Removes from the Graphs list the settings of the selected channels. |
| Move Up | Moves up by one position the elements selected from the Graphs list. |
| Move Down | Moves down by one position the elements selected from the Graphs list. |
  
##### Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
#### 

#### Toolbar

The toolbar of the XYZ Configuration window allows the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode that allows to keep displayed the configuration window while the application windows are being used. |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel command of the File menu). |
| Apply | Applies the present settings to the configuration of the graphic window (similar to the Apply command of the File menu). |
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
  
#### 

#### Pop-up Menu

The pop-up menu of the XYZ Configuration window can be displayed by clicking the right button of the mouse on the Options page.

The pop-up menu of the XYZ Configuration window allows access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Cut | Similar to the Cut command of the Edit menu |
| Copy | Similar to the Copy command of the Edit menu |
| Paste | Similar to the Paste command of the Edit menu |
| Add Graph | Similar to the Add Graph command of the Graph menu |
| Remove Graph | Similar to the Remove Graph command of the Graph menu |
| Move Up | Similar to the Move Up command of the Graph menu |
| Move Down | Similar to the Move Down command of the Graph menu |
| Restore Default | Copies in the channel the settings configured in the Channel Parameters. |
| Set As Default | Configures the settings of the channel in the Channel Parameters |
  
##### Related Issues

  * [XYZ window](XYZWndOverview.md)

  * [Elements of the window](XYZWndItems.md)

  * [Functions](XYZWndFeatures.md)

  * [Commands](XYZWndCommands.md)


