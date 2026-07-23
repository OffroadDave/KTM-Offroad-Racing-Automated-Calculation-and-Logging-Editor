### Configuration Window

The Trend Configuration window allows configuring the aspect of the graphic [Trend windows](TREWndOverview.md) and it is formed by the following pages: Options, Script.

The window has a menu, a toolbar and a pop-up menu that eases the access to the configuration and management commands of the window itself

####  Options Page

The Options page allows configuring the graphic aspect of the [Trend](TREWndOverview.md) windows and it is divided into two sections: Window and Graph.

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

#####  Window Options

It allows configuring the general settings of the windows. Each configurable value can be edited by double clicking with the mouse or by pressing the space bar on the selected element.

  * Custom color: allows selecting a customized color in the window settings

  * Window: sets the window background color.

  * Graph: sets the background color of the graphic area.

  * Border: sets the borders color of the graphic areas.

  * Grid: displays the setting to enable the grid common to all channels graphs, in the graphic area of the window. To modify the parameter, edit the associated configuration window.




  * Enable Custom Grid settings: Enables the grid display with the customized settings.

  *     1.        * Style: Sets the style of the grid

       *          * Simple: the grid is formed by lines drawn with the styles configured in Line Style settings.

         * Cross: the grid is formed by crosses indicating the intersection of the vertical and horizontal divisions.

       * Color: Color of the grid

       * Line style: Sets the style of the grid line (valid if the Style Simple is set)

       *          * Solid: continuous line

         * Dash: dashed line

         * Dot: dotted line

         * DashDot: dashed line alternated with 1 dot

         * DashDotDot: dashed line alternated with 2 dots

  * X

  *     1.        * Mode: Calculation mode of the horizontal divisions.

       *          * Auto, displays an automatic number of equidistant divisions

         * Off, no division is displayed

         * Fixed, displays a fixed number of equidistant divisions

         * Step, displays the divisions at fixed intervals equal to the Step value

         * Manual, displays the divisions in correspondence with the values on the X axis set by the user in the text box Values.

       * Step: Fixed step to calculate the horizontal divisions (a divisions for each Step), valid with Mode set.

       * Div.: The number of horizontal divisions to be displayed, valid with Mode set at Auto or Fixed.

       * Values: List of values on Z axis corresponding to the divisions, valid with Mode set at Manual. The list of values can be added directly in the text box, using as division the character ';'.

  * Y

  *     1.        * Mode: Calculation mode of the vertical divisions.

       *          * Auto, displays an automatic number of equidistant divisions

         * Off, no division is displayed

         * Fixed, displays a fixed number of equidistant divisions

       * Step: not used

       * Div.: Number of vertical divisions to be displayed, valid with Mode set at Auto or Fixed.

       * Values: not used



  * Mode: selects Trend mode. The x-axis is based to all laps of Session, Run, Selected (could be one or more lap) or Single Lap loaded.

  * Layout: enables to select the vertical arrangement mode of the channels graphs and of the corresponding Y scales for each area. See also: [Layout](TREWndFeatures.md#TREFeatureLayout) function.

  *     * Parallel: the graphic area is divided into rectangles of the same height and the graphs of the channels are arranged in vertical sequence so that they do not overlay.

    * Overlay: all graphs share the vertical area available in the graphic area and are overlapping.

    * Manual: each graph can be placed by the user in a portion of the vertical area; the graphs can be overlapping. The vertical coordinates of the rectangle reserved to the channel correspond to the settings of the Top and Bottom fields of the Graphs Options for each channel.

  * Lap Filter: Opens [Lap Filter configuration page](../Common/Common.md#CommonConfLapFilter).

  * X Axis Channel: With this combo the user can select whether to set the X-axis with an internal lap counter or with a custom channel.




#####  Graphs Options

It allows configuring the settings specific of the channel to be graphed. Each configurable value can be edited by double clicking with the mouse or by pressing the space bar on the selected element. Multiple selections are possible through the CTRL and SHIFT keys. 

  * Axis: This column is for information only.

  * Label: displays the name of the channel. The name of the channel can be edited and can become a math expression if the sing = comes first.

  * Display:

  *     * Color: Displays the color of the channel graphs or of the markers. To modify the setting, edit the channel by opening the Channel Properties page where fonts and style are configured.

    * Style: displays the style of the channels graphs. To modify the setting, edit the channel by opening the Channel Properties page where font and styles are configured.




  * Enable custom Font settings: The font section can be disabled through the check button.

  *     1.        * Family Font: Sets the font.

       * Font Dimension: Sets the font size.

       * Scalable: Enables the adapting of the font size in relations to the window size.

  * Line settings

  *     1.        * Style: Sets the style of the graphs

       *          * None: no line is drawn

         * Line: continuous line

         * Step: stepped line

         * Fill Down: continuous line with colored bottom area

         * Fill Up: continuous line with colored top area

         * Bordered: bordered continuous line

       * Weight: Sets the width of the line in pixel.

       * Line color: Sets the line color.

       * Border Color: Color for the line border

  * Marker Settings

  *     1.        * Style: Style of the markers, graphic elements used to represent the marker.

       *          * None : no markers are drawn

         * Dots : dot

         * Cross : cross

         * Rhomboid : rhomboid

         * Square : square

         * Arrow Down: arrow downwards

         * Arrow Up: arrow upwards

         * Vert Line: vertical line

         * Horz Line: horizontal line

       * Weight: Size (width) of the markers in pixel.

       * Marker color: Color of the markers.

  * Box Value: If checked, inserts a border in the value box of the channels.



  * Scale:

  *     * Local: If Local is selected, the scale is automatically calculated. If Local is not selected, the outer limits of the scale can be manually set.

    * Min: Sets the minimum value of the manual y scale

    * Max: Sets the maximum value of the manual y scale

    * Bottom (%): sets the bottom vertical arrangement of the graphs in the vertical area in manual mode.

    * Top (%): sets the top vertical arrangement of the graphs in the vertical area in manual mode.



  * Value: Configures the statistics to be calculated for the channel. The following calculation possibilities are available:



  *     * Min: Returns to the minimum value of the channel in the lap analyzed

    * Max: Returns to the maximum value of the channel in the lap analyzed

    * Avg: Returns to the average value of the channel in the lap analyzed

    * Start: Returns to the value of the channel at the beginning of the lap analyzed

    * End: Returns to the value of the channel at the end of the lap analyzed

    * Event: Returns to the value of the channel at the time when the event takes place. This is true when Avg. Time is set to 0. (If this value is higher than 0, the average value of the channel is calculated on the left, set in Avg. Time, at the time of the first occurrence.

    * Info: Available only for the Info, returns to the value of the Info associated with the lap analyzed

    * Occurrences: Available only for the events, returns to the number of occurrences of the event in the lap analyzed.

    * StDev: Returns to the standard deviation of the channel in the analyzed lap.

    * Change: Returns the difference between End and Start.

    * First: Returns the first valid value of the first 3 effective seconds of the lap.

    * Last: Returns the last valid value in the last 3 effective seconds of the lap.

  * Event: Sets the event for the calculation of the Event statistics;

  * Avg. Time: Sets the left area where the event statistics can be calculated. The value 0 presupposes the calculation in the same instant of the first occurrence of the event. A value higher than 0, calculates the average of the channel in a time interval ranging from the instant of the event minus the value set and the instant of the event itself. The value set is in cents of second, so if the left area has to be set to 1 second, 100 should be the value. If the lowest limit of the range has a negative value, during the calculation it is forced to 0.




####  Script

The Script page allows configuring the scripts of the events of the [Trend](TREWndOverview.md) window or of the application, in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

####  Menu

The menu of the [Trend](TREWndOverview.md) window enables the access to the following commands, divided in sub menus: 

#####  File Menu

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
  
#####  View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use. |
  
#####  Graph Menu

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
| Set As Default | Configures the channel settings in the Channel Parameters. |
| Edit... | Gives the possibility to edit the selected parameter (similar to double click). |
  
##### 

##### Related Issues

  * [Trend window](TREWndOverview.md)

  * [Elements of the window](TREWndItems.md)

  * [Functions](TREWndFeatures.md)

  * [Commands](TREWndCommands.md)


