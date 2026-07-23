### Run Preview Configuration window

The Run Preview Configuration window allows configuring the aspect of the [Run Preview window](RunPreviewWndOverview.md); it is composed of only one page, Options. The window has also a menu , a toolbar and an integrated pop-up menu that ease the access to the configuration and management of the window itself.

#### Options Page

The Options page enables to configure the graphic aspect of the [Run Preview window](RunPreviewWndOverview.md) and it is divided into two sections: Window and Graphs .

### 

##### Window Options

It enables the configuration of the window layout. Each element of the grid can be edited by double clicking with the mouse or by pressing the space bar.

  * Custom color: enables the setting of the customized colors of the window.




If it is enabled, the colors set in the Window, Graph and Border columns of this section are used for the background of the window, the background and the border of the graphic area.

If it is disabled, the default (corresponding to the Color Settings section, in the [Default Appearance](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgDefaultAppearence) page of the [General Setup](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md) window) colors are used. (General Settings, general configuration of the WinTAX4 environment).

  * Window: sets background color of the window.

  * Graph: sets the background color of the graphic area.

  * Border: sets the window areas borders color.

  * Mode: Allows selection of report mode (Session, Run)

  * Layout: enables selection of the channel graphs vertical arrangement mode (and of the corresponding Y scales) (see also: [Layout](RunPreviewWndFeatures.md#RunPreviewFeatureLayout) function).

  *     * Parallel: the graphic area is divided into rectangles of the same height and the graphs of the channels are arranged in vertical sequence so that they do not overlay.

    * Overlay: all graphs share the vertical area available in the graphic area and are overlapping.

    * Manual: each graph can be placed by the user in a portion of the vertical area; the graphs can be overlapping. The vertical coordinates of the rectangle reserved to the channel corresponds to the settings of the Top and Bottom fields of the Graphs section.

  * X Axis Mode: Sets the X axis in the time scale (Time) or in the distance scale (Distance) (see also [X Scale](RunPreviewWndFeatures.md#RunPreviewFeatureXScale) mode function).

  *     * Time: the X axis displays the scale of times (in seconds).

    * Distance: the X axis displays the distance scale covered (in meters).

  * Time Scale: sets the display Time mode for the X scale.

  *     * Relative: The time instant displayed in the values box of the X axis, is a relative instant that depends on the length of the lap with initial instant equal to 0.

    * Absolute: the time instant displayed in the values box of the X axis is an absolute time (expressed in hh:mm:ss:mmm) that depends on the length of the lap with initial time equal to lap start time. Also, the absolute value of the current instant of the lap in displayed in a box by the side of the scale.

  * Grid: displays the setting to enable the grid common to all graphs of the channels, in the graphic area of the window. To modify the parameter, edit the associated configuration window.

  * Downsampling: enables downsampling; downsampling factor can be chosen in the combo box on the right.

  * Laptime: shows/hides lap times.

  * Show Events: Enable events displaying




  1.      1.         * Enable: enables the grid display with the customized settings.

        * Style: sets the grid style

        *           * Simple: the grid is formed by lines drawn with the styles configured in Line Style settings.

          * Cross: the grid is formed by crosses indicating the intersection of the vertical and horizontal divisions.

        * Color: grid color

        * Line style: sets the grid line style (valid if the Simple Style is set)

        *           * Solid: continuous line

          * Dash: dashed line

          * Dot: dotted line

          * DashDot: dashed line alternated with 1 dot

          * DashDotDot: dashed line alternated with 2 dots




X

  1.      1.         * Mode: calculation mode of the horizontal divisions

        *           * Auto, shows an automatic number of equidistant divisions.

          * Off, no division is displayed.

          * Fixed, shows a fixed number of equidistant divisions.

          * Step, shows the divisions at fixed intervals equal to the Step value.

          * Manual, shows the divisions in correspondence with the values on the X axis set by the user in the text box Values.

        * Step: fixed step to calculate the horizontal divisions (a division each Step), valid if Mode is set to Step

        * Div.: number of horizontal divisions to be displayed, valid if Mode is set to Auto or Fixed

        * Values: list of values on the X axis corresponding to the divisions, valid if Mode is set to Manual. The list of values can be directly added in the text box using as division the character ';'.




Y

  1.      1.         * Mode: calculation mode of the vertical divisions

        *           * Auto, shows an automatic number of equidistant divisions.

          * Off, no division is displayed

          * Fixed, shows a fixed number of equidistant divisions

        * Step: not used

        * Div: number of vertical divisions to be displayed, valid if Mode is set to Auto or Fixed

        * Values: not used




If the window grid is disabled, the Default Grid settings in the [Default Appearance](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgDefaultAppearence) of the [General Setup](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md) window (General Settings, general configuration of the WinTAX4 environment), are valid.

##### Graphs Section

Enables configuration of the settings specific for each channel of the window. Each line identifies a configured channel, while the columns identify the fields to be configured. Each element of the grid can be edited by double clicking with the mouse or by pressing the space bar. Multiple sections are possible through the CTRL and SHIFT keys

  * Label: shows the name of the channel. The channel name can be edited and can become a math expression if the sign = comes first.

  * Color: shows the channel color or graph markers. To modify the setting, edit the channel by opening the Channel Properties window where fonts and styles can be selected.



  * Style: shows the graph channel style. To modify the settings, edit the channel by opening the display Style page where fonts and styles can be configured.




Enable custom font settings: enable/disable the custom font

  1.      1.         1.            * Family Font: sets the type of font.

           * Font Dimension: sets the size of font.

           * Scalable: enables the adapting of the font size proportionally to the window size.




Line settings

  1.      1.         1.            * Style: sets the style of the graphs line

           *              * None: no line is drawn

             * Line: continuous line

             * Step: stepped line

             * Fill Down: continuous line with colored bottom area

             * Fill Up: continuous line with colored top area

             * Bordered: continuous line with border

           * Weight: sets the line width in pixel.

           * Line color: sets the line color.

           * Border Color: color for the line border




Marker Settings

  1.      1.         1.            * Style: marker style, graphic elements used to represent the marker.

           *              * None: no markers are drawn

             * Dots: dot

             * Cross: cross

             * Rhomboid: rhomboid

             * Square: square

             * Arrow Down: arrow downwards

             * Arrow Up: arrow upwards

             * Vert Line: vertical line

             * Horz Line: horizontal line

           * Weight: marker size (width) in pixel.

           * Marker color: markers color.




Multi Color

  1.      1.         1.            * Enable: enable the multicolor settings

           * Configure: open the multicolor configuration window




Division: number of colored bands. The value must be included between 1 and 4. Changing the value of division, the start-end-color grid redraws the number of rows configured. In the rows it is possible to manually configure the values or recalculate it

Auto: calculate steps values using the first row Start and last row End as fixed values.

Reverse: reverses the sequence of colors.

Color Mode: In the rows of the start-end-color grid it is always possible to manually configure the colors. Otherwise color can be sets with color mode combo.

  * If Manual is selected, the window generate a group of default colors.

  * If Gradation is selected, the button Color is used to choose the base color of gradation.

  * If Blend is selected, the buttons Start color and End color are used to choose the start and the end colors of blend.




Box Value

  1.      1.         1.            * Border: show the outline border of cursor value.




  * Format: displays the style to show the current channel value. To modify the setting, open the configuration window that enables to configure the setting for the display format of the channel values.




In the combo on the left the numeric format is selected, in the combo on the right, the number of decimals is selected. Please find to follow the list of possible formats

  *     1.        * Auto: the format is kept unchanged

       * Dec: the decimal format allows max 5 digits after the comma.

       * Numeric: the numeric format allows max 15 digits after the comma.

       * Scientific: the scientific format allows max 15 digits after the comma; the result is written in exponential form.

       * Hex: hexadecimal format; the decimals cannot be configured.

       * Bin: binary format; the decimals cannot be configured.

       * ASCII: text format; the decimals cannot be configured.

  * Unit: Enables to display the measurement unit of the channel.

  * Scale

  *     * Show: show/hide the scale Y. In Run preview is always enabled.

    * Auto: Enables to automatically display the Y scale.

    * Min: Sets the minimum value of the channel.

    * Max: Sets the maximum value of the channel.

    * Bottom (%): Sets the vertical arrangement of the graph in the vertical area (Bottom).

    * Top (%): Sets the vertical arrangement of the graph in the vertical area (Top).

  * Filter: Shows the settings on the filters applied to the channel. To modify the setting, open the configuration window.




The filters can be added, removed or moved through the buttons on the Options bar. The filters available are:

  1.      1.         * OFF: Channel values are displayed as logged

        * Run Average: Applies a moving average filter to the channel. Filter length is defined in milliseconds. If set to zero, the filter is not calculated.

        * FFT filters applies a combination of frequency domain filters to the channel. The frequency content of the signal in the range(s) defined by the cut-off frequency is set to zero and the data is reconstructed in the time-domain. The four types available are:

        *           * FFT Low Pass: maintains frequency content below the cut-off freq. Freq1

          * FFT High Pass: maintains frequency content above the cut-off freq. Freq1

          * FFT Band Pass: maintains frequency content in the range Freq1< frequency < Freq2

          * FFT Band Stop: eliminates frequency content in the range Freq1< frequency < Freq2

        * IIR Low Pass: Infinite Impulsive Response Low Pass filter is a recursive filter that maintains frequency content below the cut-off frequency

        * IIR High Pass: Infinite Impulsive Response High Pass filter is a recursive filter that maintains frequency content above the cut-off frequency



  * Grid: displays the setting to enable the grid specific of the rectangle of the graphic area reserved to the channel. The window enabling the grid and the configuration of the parameters is the following:




  1.      1.         * Enable Local Grid Settings: enables to display the grid with the customized settings.

        * Style: sets the style of the grid

        *           * Simple the grid is formed by continuous lines.

          * Cross: the grid is formed by crosses indicating the intersection of the vertical and horizontal divisions.

        * Color: color of the grid

        * Line style: sets the style of the grid line (valid if Style Simple is set)

        *           * Solid: continuous line

          * Dash: dashed line

          * Dot: dotted line

          * DashDot: dashed line alternated with 1 dot

          * DashDotDot: dashed line alternated with 2 dots




X

  1.      1.         * Mode: calculation mode of the horizontal divisions

        *           * Auto, displays an automatic number of equidistant divisions.

          * Off, no division is displayed

          * Fixed, displays a fixed number of equidistant divisions

          * Step, displays the divisions at fixed intervals equal to the Step value.

          * Manual, displays the division in correspondence with the values on the X axis set by the user in the info box Values .

        * Step: fixed step to calculate the horizontal divisions (a division for each Step), valid if Mode is set to Step

        * Div: number of horizontal divisions to be displayed, valid if Mode is set at Auto or Fixed

        * Values: list of values on the X axis corresponding to the divisions, valid if Mode is set at Custom. The list of values can be added directly in the text box, using as division the character ';'




Y

  1.      1.         * Mode: calculation mode of the vertical divisions

        *           * Auto, displays an automatic number of equidistant divisions.

          * Off, no division is displayed

          * Fixed, displays a fixed number of equidistant divisions

          * Step, displays the divisions at fixed intervals equal to the Step value.

          * Manual, displays the division in correspondence with the values on the Y axis set by the user in the info box Values .

        * Step: fixed step to calculate the vertical divisions (a divisions for each Step), valid if Mode is set to Step

        * Div: the number of vertical divisions to be displayed, valid with Mode set at Auto or Fixed

        * Values: list of values on Y axis corresponding to the divisions, valid with Mode set at Custom. The list of values can be added directly in the text box, using as division the character ';'.




####  Menu

The menu of the [Run Preview window](RunPreviewWndOverview.md) allows the access to the following commands, divided in sub menus:

##### File Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Apply | Applies the current settings of the Run Preview window |
| Cancel | Closes the window without applying the current settings to the configuration of the Run Preview window |
  
##### Edit Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Cut | Copies to clipboard the configurations of the channels selected in the list of the Graphs section and it removes them from the list of the Graphs section. |
| Copy | Copies to clipboard the configurations of the channels selected in the list of the Graphs section |
| Paste | Pastes the configurations of the channels available in clipboard adding them to the list of the Graphs section |
  
##### Menu Graph

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Graph | Adds a new element to the Graphs list of channels configurations |
| Remove Graph | Removes from the Graphs list the configurations of the selected channels |
| Move Up | Moves up by one position the selected elements in the Graphs list |
| Move Down | Moves down by one position the selected elements in the Graphs list |
  
####  Toolbar

The toolbar of the [Run Preview window](RunPreviewWndOverview.md) allows the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | This button is disabled. |
| Apply | Applies the current settings to the configuration of the graphic window (similar to the Apply of the File menu) |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel of the File menu) |
| Cut | Similar to the Cut command of the Edit menu |
| Copy | Similar to the Copy command of the Edit menu |
| Paste | Similar to the Paste command of the Edit menu |
| Add | Similar to the Add Graph command of the Graph menu |
| Remove | Similar to the Remove Graph command of the Graph menu |
| Move Up | Similar to the Move Up command of the Graph menu |
| Move Down | Similar to the Move Down command of the Graph menu |
| Channel Browser | Displays the pop-up menu to select the page in the Channel Browser window. |
  
####  Pop-up Menu

The pop-up menu of the [Run Preview window](RunPreviewWndOverview.md) can be displayed by clicking with the right button of the mouse on the Options page. The pop-up menu of the [Run Preview window](RunPreviewWndOverview.md) allows the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Cut | Similar to the Cut command of the Edit menu |
| Copy | Similar to the Cut command of the Edit menu |
| Paste | Similar to the Paste command of the Edit menu |
| Add Graph | Similar to the Add Graph command of the Graph menu |
| Remove Graph | Similar to the Remove Graph command of the Graph menu |
| Move Up | Similar to the Move Up command of the Graph menu |
| Move Down | Similar to the Move Down command of the Graph menu |
| Edit | Edit the selected element of the grid. |
  
##### Related Issues

  * [Run Preview window](RunPreviewWndOverview.md)

  * [Elements of the window](RunPreviewWndItems.md)

  * [Functions](RunPreviewWndFeatures.md)

  * [Commands](RunPreviewWndCommands.md)


