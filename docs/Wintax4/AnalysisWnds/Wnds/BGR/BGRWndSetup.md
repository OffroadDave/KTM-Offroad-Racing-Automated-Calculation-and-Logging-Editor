### Configuration Window

The Bargraph Configuration window allows configuring the graphic aspect of the Bargraph window: the window has the Options and Script pages.

The window also has a menu, a toolbar and a pop-up menu that ease the access to the configuration and management commands of the window itself.

####  Options

The Options page enables to configure the graphic aspect of the Bargraph windows and it is divided into two sections.

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

#####  Window Options

It allows configuring the graphic settings of the window. Each box can be edited by double clicking with the left button of the mouse or by pressing the SPACE Bar.

  * Custom color: allows the setting of the window background color.

  * Window: If enabled, the color set in the Window column of this section is used for the background of the graphic area. If disabled, the color set in the Default Appearance page of the General Setup window (General Settings, general configuration of the WinTAX4 environment) is used for the background of the graphic area.

  * Color: sets the gauge instruments default colors. All graphs configured in Graphs area inherits these settings as default.




The following elements are customizable: Indicator, Background, Min/Max Indicators, Scale, Text Background, Text

Inside the "Indicator" color cell there is a button that opens the following window:

The indicator color can be configured as Solid, Gradient and Blend.

In the following picture you can see the correspondence between colors and objects.

  * Custom font: If checked the window uses the local font; if unchecked the window uses the font configured in Setup, General, Default Appearance.



  * Font: Opens the [Font configuration page](../Common/Common.md#CommonConfFontLight). 



  * Layout : allows selecting the arrangement mode of the graphs

  *     * Horizontal

    * Vertical

    * Auto




#####  Graphs Options

It allows configuring the settings specific of each configured channel of the window. Each row identifies a configured channel, while the columns correspond to the fields to be configured. Each configurable value can be edited by double clicking with the mouse or by pressing the Space bar on the selected element. Multiple selections are handled through the CTRL and SHIFT keys

  * Label : Shows the name of the channel. The name of the channel can be edited and can become a math expression if the sign = comes first.

  * Display

  *     * Color: local configuration of gauge instruments colors. the local setup overwrites the global one defined in the Window area. The following elements are customizable:

    1.        1.           * Indicator

          * Background

          * Min/Max Indicators

          * Scale

          * Text Background

          * Text




  1.      * Format: shows the visualization style of the current value of the channel: to change the setting, open the corresponding configuration window that enables to configure the setting for the visualization format of the channel values.




In the combo on the left, the numeric format is selected, in the combo on the right the number of decimals is chosen. The possible formats are

  1.      1.         * Auto: The channel format will not be changed

        * Dec: The decimal format allows max 5 digits after the comma.

        * Numeric: The numeric format allows max 15 digits after the comma.

        * Scientific: The scientific format allows max 15 digits after the format; the result is written in exponential form.

        * Hex: Hexadecimal format; decimals cannot be configured.

        * Bin: Binary format; decimals cannot be configured.

        * Ascii: Text format; decimals cannot be configured.



  1.      * Type: selects the type of bar. Bar, Cursor, Cylindric Bar styles are available.

     * Arrange Mode: layout style. Horizontal or Vertical are available.



  * Scale:

  *     * Local: allows configuring the desired thresholds of the channel. If checked, as default, the values are taken from Channel Parameters Setup, if available. If unchecked the values are calculated from the maximum and the minimum values of the channels or taken from Channel Parameters Setup, if available. 

    * Min: configure the minimum value of local scale

    * Max: configure the maximum value of local scale

    * Factor: applies a factor to the scale; the default value is x1 and the admitted values are:

    *       * x1

      * x10

      * x100

      * x1000



  1.      * Position: sets the position of the references: Inside places the reference in the center of the graphs area, Outside will place it outside the graphic area.



  * Channel Value: enables the visualization of the current values in the information area

  * Min/Max Values: enables the visualization of the boxes of the minimum and maximum values in the information area.

  * Min/Max Indicators: Enables the visualization of the minimum and maximum markers in the graphic area.

  * Alarm: enables the visualization of the Alarms bar. To modify the setting, open the Properties window at the Alarm page. In this page, the alarm range and the colors used, can be customized. If the Local Alarm Range option is not selected, the alarm configuration used will be as in Channels Parameters at the Alarm page.




## 

####  Script

The Script page enables to configure the scripts connected to the events of the Bargraph window or of the application, in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

####  Menu

The menu of the window allows the access to the following commands, divided by sub menus:

#####  File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the current setting of the window. |
| Cancel |  | Closes the window without applying the current setting. |
| Load |  | Opens a dialog window to select a configuration file to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file to save the current setting. |
  
#####  Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies to clipboard the setting of the channels selected from the Graphs section list, and removes them from the list. |
| Copy | Ctrl + C | Copies to clipboard the setting of the channels selected from the Graphs section list. |
| Paste | Ctrl + V | Pastes the setting of the channels from the clipboard, adding them to the Graphs section list. |
  
#####  View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl + Shift \+ Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use. |
  
#####  Graph Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Graph | Adds a new element to the Graphs list for the configuration of channels. |
| Remove Graph | Removes from the Graphs list the setting of the selected channels. |
| Move Up | Moves up by one position the elements selected from the Graphs list. |
| Move Down | Moves down by one position the elements selected from the Graphs list. |
  
#####  Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
## 

####  Toolbar

The window toolbar allows the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode ensuring to keep displayed the configuration window while other application windows are in use. |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel command of the File menu). |
| Apply | Applies the present settings to the graphs window (similar to the Apply command of the File menu). |
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
| Channel Browser | Shows the pop-up menu to select the page in the Channel Browser window. |
  
## 

####  Pop-up Menu

This menu can be visualized by clicking with the right button of the mouse on the Options page.

The pop-up menu enables the access to the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl \+ X | Similar to the Cut command of the Edit menu |
| Copy | Ctrl \+ C | Similar to the Copy command of the Edit menu |
| Paste | Ctrl \+ V | Similar to the Paste command of the Edit menu |
| Add Graph |  | Similar to the Add Graph command of the Graph menu |
| Remove Graph |  | Similar to the Remove Graph command of the Graph menu |
| Move Up |  | Similar to the Move Up command of the Graph menu |
| Move Down |  | Similar to the Move Down command of the Graph menu |
| Restore Default |  | Sets in the channel the settings configured in the Channel Parameters. |
| Set As Default |  | Sets the configurations of the channel in the Channel Parameters. |
| Edit... |  | Gives the possibility to edit the selected parameter (similar to the double click). |
  
##### Related Issues

  * [Bargraph window](BGRWndOverview.md)

  * [Elements of the window](BGRWndItems.md)

  * [Commands](BGRWndCommands.md)


