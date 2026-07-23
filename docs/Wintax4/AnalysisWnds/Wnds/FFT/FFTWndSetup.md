### Configuration Window

The FFT Configuration window allows setting the [FFT Analysis](FFTWndOverview.md) window; it is formed by the Options and Script pages.

The window includes a menu, a toolbar and a pop-up menu that eases the access to the configuration and management commands of the window itself.

####  Options Page

The Options page allows configuring the graphic aspect of the [FFT Analysis](FFTWndOverview.md) windows and it is divided into 2 sections: Window and Graph. Each configurable value can be edited by double clicking with the mouse or by pressing the space bar on the selected element.

##### 

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

#####  Window Options

It allows configuring the general settings of the window.

  * Custom color: enables customized colors settings for the window.

  * Window: sets the background color of the window.

  * Graph: sets the graphic area background color.

  * Border: sets the borders color of the window areas.

  * Custom Font: if enabled, it uses the Font configured in the configuration setting instead of the font usually set by the parameters or by default.

  * Font: Opens the [Font configuration page](../Common/Common.md#CommonConfFont). 

  * Mode: Switch between Spectrogram and Frequency Analysis. The default setting is "Frequency Analysis"

  * X Axis Mode



  *     * Local: if it is checked, it automatically sets the frequencies range at 0 as min and max values are calculated according to the max frequency of all the channels available and according to the sampling theorem. If it is not clicked, the frequency range is configured in the Min. and Max. boxes.

    * Min: minimum value of the frequency range if Local is not enabled.

    * Max: maximum value of the frequency range if Local is not enabled.

    * Mode: the linear or logarithmic scale can be selected for the frequencies ranges on X Axis.

  * dB Scale

  *     * Local: if it is checked, it automatically sets the dB Scale range at min and max calculated values. If it is not clicked, the frequency range is configured in the Min. and Max. boxes. It is used in Spectrogram mode.

    * Min: minimum value of the frequency range if Local is not enabled; it is used in Spectrogram mode.

    * Max: maximum value of the frequency range if Local is not enabled; it is used in Spectrogram mode.

  * Grid: shows the setting to enable the grid common to all the channels graphs in the graphic area of the window. The parameter can be modified by editing the corresponding configuration window.




  * Enable Custom Grid settings: Enables the grid display with the customized settings.



  *     1.        * Style: Sets the style of the grid.

       *          * Simple: the grid is formed by lines drawn with the styles configured in Line Style settings.

         * Cross: the grid is displayed with crosses.

       * Color: color of the grid.

       * Line style: Sets the style of the grid line (valid if the Style Simple is enabled).

       *          * Solid: continuous line

         * Dash: dashed line

         * Dot: dotted line

         * DashDot: dashed line alternated with 1 dot

         * DashDotDot: dashed line alternated with 2 dots

  * X

  *     1.        * Mode: Calculation mode of the horizontal divisions.

       *          * Auto, displays an automatic number of equidistant divisions.

         * Off, no division is displayed

         * Fixed, displays a fixed number of equidistant divisions

         * Step, displays the divisions at fixed intervals equal to the Step value.

         * Manual, displays the divisions in correspondence with the values on the X axis set by the user in the text box Values.

       * Step: Fix step to calculate the horizontal divisions (a division for each Step), valid with Mode set at Step.

       * Div.: Quantity of horizontal division to be displayed, valid with Mode set at Auto or Fixed.

       * Values: List of values on the X axis corresponding to the divisions, valid with Mode set at Custom. The list of values can be directly added in the text box using ';' to separate the character.

  * Y

  *     1.        * Mode: Calculation mode of the vertical divisions.

       *          * Auto, displays an automatic number of equidistant divisions.

         * Off, no division is displayed

         * Fixed, displays a fixed number of equidistant divisions

       * Step: Fix step to calculate the vertical divisions (a division for each Step), valid with Mode set at Step.

       * Num. div.: Quantity of vertical division to be displayed, valid with Mode set at Auto or Fixed.

       * Values: List of values on the Y axis corresponding to the divisions, valid with Mode set at Custom. The list of values can be directly added in the text box using ';' to separate the character.




### 

#####  Graphs Section

It allows configuring the settings specific for each channel to be graphically displayed. Each line identifies a configured channel, while the columns identify the fields to be configured. Multiple selections are possible with the CTRL and SHIFT keys. In Spectrogram Mode only the first row is configurable.

  * Label: displays the name of the channel. The name of the channel can be edited and can become a math expression if the sign = comes first.

  * Color: Sets the graphic color of the channel.

  * Style: shows the style of the channel graph. To modify the setting, edit the channel by opening the Channel Properties page where fonts and styles can be configured.




  * Line settings section

  *     1.        * Style: sets the style of the graphs line

       *          * None: no line is drawn

         * Line: continuous line

         * Step: stepped line

         * Fill Down: continuous line with colored bottom area

         * Fill Up: continuous line with colored top area

         * Bordered: continuous line with border

       * Weight: sets the width of the line in pixel.

       * Line color: sets the line color.

       * Border Color: color for the line border

  * Marker Settings section

  *     1.        * Style: style of the markers, graphic elements used to represent the marker.

       *          * None: no markers are drawn

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

  * Time Range: If it is enabled, it limits the channel analysis to the time range identified by Init and Final.

  * Start Time (sec): Value of the minimum time range in seconds according to the frequency analysis that must be performed.

  * End Time (sec): Value of the maximum time range in seconds according to which the frequency analysis must be performed.

  * Frequency Analysis types: for each channel allows displaying PSD and/or Magnitude and/or Phase. On Spectrogram mode checks are disabled.

  * FFT Length: Length of the FFT filter. It coincides with the resulting quantity of points of the graphic.

  * Definition: Definition degree; to obtain a good graphic resolution and to highlight the pikes, uses a value at least greater or equal to 3.

  * Window: Window style applied to the analysis: it is used to reduce the "leakage" and discontinuity effects of the data on the range limits taken into account. The "windows" available are: Hamming, Hanning, Triangle, Blackman and Harris.

  * Scale: The linear or logarithmic scales can be selected for the values of the analysis of each channel for each representation (PSD, Magnitude and Phase). On Spectrogram mode selection is disabled.




#### 

####  Script

The Script page allows configuring the scripts connected to the events of the [FFT Analysis](FFTWndOverview.md) window or of the application, in VB Script or Jscript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

####  Menu

The [FFT Analysis](FFTWndOverview.md) window menu enables the access to the following commands, divided into sub menus:

#####  File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the present setting of the window. |
| Cancel |  | Closes the window without applying the present settings. |
| Load |  | Opens a dialog window to select a configuration file (.psd) to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file (.psd) on which the present settings can be saved. |
  
#####  Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies to clipboard the configurations of the channels selected in the list of the Graphs section and remove them from the list. |
| Copy | Ctrl + C | Copies to clipboard the configurations of the channels selected in the list of the Graphs section. |
| Paste | Ctrl + V | Pastes the configurations of the channels available in clipboard, adding them to the list of the Graphs section. |
  
#####  View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use. |
  
#####  Graph Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Graph | Adds a new element to the Graphs list of channel configuration. |
| Remove Graph | Removes from the Graphs list the settings of the selected channels. |
| Move Up | Moves up by one position the elements selected from the Graphs list. |
| Move Down | Moves down by one position the elements selected from the Graphs list. |
  
####  Toolbar

The window toolbar enables the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode that allows keeping displayed the configuration window while the application windows are being used. |
| Cancel | Closes the configuration window without applying the settings (Similar to the Cancel command of the File menu). |
| Apply | Applies the present settings to the configuration of the graphic window (Similar to the Apply command of the File menu). |
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
  
## 

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
| Edit... | Gives the possibility to edit the selected parameter (Similar to the double clicking). |
  
##### 

##### Related Issues

  * [FFT Analysis window](FFTWndOverview.md)

  * [Elements of the window](FFTWndItems.md)

  * [Functions](FFTWndFeatures.md)

  * [Commands](FFTWndCommands.md)


