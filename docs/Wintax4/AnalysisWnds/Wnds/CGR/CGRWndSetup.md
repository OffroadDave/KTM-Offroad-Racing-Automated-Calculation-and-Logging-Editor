### Configuration Window

The Gauge Configuration window allows setting the look of the Gauge windows; it is formed by the Options and Script pages.

The window moreover has a menu, a toolbar and a pop-up menu that ease the access to the configuration and management commands of the window itself.

####  Options

The Options page allows configuring the graphic aspect of the Gauge windows and it is divided into 2 sections.

### 

#####  Window Options

It allows configuring the settings of the aspect of the window. Each element can be edited by double clicking with the left button of the mouse or with the SPACE bar.

  * Custom color: allows the setting of the window background color.

  * Window: If enabled, the color set in the Window column of this section is used for the background of the graphic area. If disabled, the color set in the Default Appearance page of the General Setup window (General Settings, general configuration of the WinTAX4 environment) is used for the background of the graphic area.

  * Color: sets the gauge instruments default colors. All graphs configured in Graphs area inherits these settings as default. The following elements are customizable:

  *     * Indicator

    * Background

    * Min/Max Indicators

    * Scale

    * Text Background

    * Text




  * Custom Font: If checked the window use the local font; if unchecked the window use the font configured in Setup, General, Default Appearance.

  * Font: Opens the [Font configuration page](../Common/Common.md#CommonConfFontLight). 



  * Layout : allows selecting the arrangement mode of the graphs

  *     * Horizontal \- The window organizes all channels in a horizontal manner.

    * Vertical \- The window organizes all channels in a vertical manner.

    * Auto \- The window organizes all channels to fill the window space in the best possible way.




### 

#####  Graphs Options

It allows configuring the settings specific of each channel of the window. Each line identifies a configured channel, while the fields to be configured correspond to the columns. Each element can be edited by double clicking with the left button of the mouse or with the SPACE bar. Multiple selections are possible through the CTRL and SHIFT keys.

  * Label: shows the name of the channel. The name of the channel can be edited and can become a math expression if the sign = comes first.

  * Display

  *     * Color: local configuration of gauge instruments colors. the local setup overwrites the global one defined in Window area. The following elements are customizable:

    *       1.          * Indicator

         * Background

         * Min/Max Indicators

         * Scale

         * Text Background

         * Text




  1.      * Format: Visualizes the style to display the current value of the channel. To modify the setting, open the corresponding configuration window that allows configuring the settings for the visualization format of the channel values.




In the combo on the left the numeric format is selected, in the combo on the number of decimals. Please find to follow the list of the possible formats

  1.      1.         * Auto : The channel format is kept unchanged.

        * Dec : The decimal format allows max 5 digits after the comma.

        * Numeric : The numeric format allows max 15 digits after the comma.

        * Scientific : The scientific format allows max 15 digits after the comma; the result is written in exponential form.

        * Hex : Hexadecimal format; the decimals cannot be configured.

        * Bin : Binary format; the decimals cannot be configured.

        * Ascii : Text format; the decimals cannot be configured



  1.      * Type: selection of the indicator type, Hand or Solid Sector

     *        * Solid Sector: The graphic area is colored up to the current value.

       * Hand: The current value is indicated by a minute hand.

     * Angle: selection of gauge style. the following angles are the available styles of gauges.

     *        * 90° Left

       * 90° Right

       * 180°

       * 270°

       * 360°



  1.      * Indicator Size: option only related to Hand type of indicator. Small, Medium, Large are the available dimensions of the minute hand.



  *     * Label: The options are:

    *       * inside: allows displaying the channel name inside the instrument area.

      * outside: allows displaying the channel name outside the instrument area.

  * Scale:

  *     * Local: allows configuring the desired thresholds of channel. If checked, as default, the values are taken from Channel Parameters Setup, if available.

    * Min: configure the minimum value of local scale

    * Max: configure the maximum value of local scale

    * Factor: applies a factor to the scale; the default value is x1 and the admitted values are:

    *       * x1

      * x10

      * x100

      * x1000



  * Channel Value: enables the visualization of the boxes of the current value in the information area

  * Min/Max Values: enables the visualization of the boxes of the minimum and maximum values in the information area.

  * Min/Max Indicators: enables the visualization of the minimum and maximum markers in the graphic area.

  * Alarm: enables the visualization of the Alarms bar. To modify the setting, open the Properties window at the Alarm page. In this page, the alarm range and the colors used, can be customized. If the Local Alarm Range option is not selected, the alarm configuration used will be the one in Channels Parameters at the Alarm page.




#### 

####  Script

The Script page allows configuring scripts connected to the events of the Gauge window or of the application, in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

####  Menu

The menu of the window allows the access to the following commands, divided into sub menu:

#####  File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the current settings of the window. |
| Cancel |  | Closes the window without applying the current settings. |
| Load |  | Opens a dialog window to select a configuration file to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file on which the current settings can saved |
  
#####  Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies to clipboard the configuration of the selected channels in the list of the Graphs section, and removes them from the list. |
| Copy | Ctrl + C | Copies to clipboard the configuration of the selected channels in the list of the Graphs section, |
| Paste | Ctrl + V | Pastes the configuration of the channels available on clipboard, adding them to the list of the section Graphs. |
  
#####  View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl +Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use. |
  
#####  Menu Graph

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Graph | Adds a new element to the Graphs list of channels configuration. |
| Remove Graph | Removes from the Graphs list the configurations of the selected channels. |
| Move Up | Moves up by one position the selected elements in the Graphs list. |
| Move Down | Moves down by one position the selected elements in the Graphs list. |
  
#####  Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
## 

####  Toolbar

The window toolbar enables the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode allowing to keep visualized the configuration window while the application window is in use. |
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
| Channel Browser | Visualizes the pop-up menu to select the page in the Channel Browser window . |
  
####  Pop-up Menu

pop-up menu of the window that can be visualized by clicking with the right button of the mouse on the Options page.

The pop-up menu enables the access to the following commands:

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
| Set As Default | Configures the channel settings in the Channel Parameters. |
| Edit... | Gives the possibility to edit the selected parameter (similar to the double click) |
  
##### 

##### Related Issues

  * [Gauge window](CGRWndOverview.md)

  * [Elements of the window](CGRWndItems.md)

  * [Commands](CGRWndCommands.md)


