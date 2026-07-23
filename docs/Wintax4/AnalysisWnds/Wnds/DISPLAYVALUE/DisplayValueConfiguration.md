### Configuration Window

The Display Value Configuration window allows setting the look of the [Display Value windows](DisplayValueFeatures.md); it is formed by the Options and Script pages.

The window has a menu and a toolbar that ease the access to the configuration and management commands of the window itself.

####  Options Page

The Options page allows configuring the graphic aspect of the Display Value windows.

### 

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more about "Loading Priority" here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

#####  Window Options

It allows configuring the settings of the layout of the window. Each element, except checks, can be edited by double clicking with the left button of the mouse or with the SPACE bar.

  * Custom Color: allows the setting of the window background color.

  * Window: The color is used for the background of the graphic area; if Custom Color is disabled, the window uses as background the Window color set in Color Settings configuration, in the Default Appearance page of the General Setup window (General Settings, general configuration of the WinTAX4 environment).



  * Font: allows the settings of the window fonts.



  1.      * Label: Opens the [Font configuration page](../Common/Common.md#CommonConfFontLight). 



  1.      * Value: Opens the [Font configuration page](../Common/Common.md#CommonConfFontLight). 



  * Layout: this combo permits to choose the layout mode

    * Horizontal: In this case a single row is displayed and the channels are side by side

    * Vertical: In this case the channels are displayed inside a single column

    * Auto: In this case the window creates a grid with N rows and M columns based on the number of valid channels and on the window size.




#####  Graphs Options

allows configuring the settings of the channels. Each element, except checks, can be edited by double clicking with the left button of the mouse or with the SPACE bar.

  * Label: shows the name of the channel. The name of the channel can be edited and can become a math expression if the sign = comes first.



  * Color label: allows configuration of the label customized colors.

  * The color configured in the Text column of this section is used for the label.

  * The color configured in the Background column of this section is used for the background of the label area. By clicking the 3 dots button ( if available in the license ) it is possible to choose between solid, gradient, blend and manual type of color.



  * Color value: allows configuration of the value customized colors.

  * The color configured in the Text column of this section is used for the value.

  * The color configured in the Background column of this section is used for the background of the value area. By clicking the 3 dots button ( if available in the license ) it is possible to choose between solid, gradient, blend and manual type of color.



  * Condition: If the function is enabled in the license, this field accepts a condition channel. When the condition is true the selected colors are applied. If the condition is false or data is not valid, the color configured in the "Solid" color mode is applied. 

  * Display name: allows configuration of the label customized text.

  * Enabled: If checked, the window shows the configured text; if unchecked, the window shows the default channel Label.

  * Text: alternative text label that will be displayed in the window.



  * Unit: if checked displays the measure unit of the channel under the channel name



  * Trend: if checked displays an icon for the trend of the channel at the current sample for each dataset. 



  * Alarm: The alarm configuration range used will be as in Channels Parameters at the Alarm page. Every time a channel is inside the alarm range, it will be displayed with relevant colors.

  *   * Enabled: enables the visualization of the Alarms.

  * The color sets in the Text column of this section is used for the label if in alarm range. The default is black.

  * The color sets in the Background column of this section is used for the background of the label area if in alarm range. As default the background is red.



  * Out of range: Refers to the valid range values as set in Channels Parameters at the Alarm page. Every time a channel is outside the alarm range but inside valid range, it will be displayed with the associated colors. This option is not depending on the alarm.

  *   * Enabled: enables the visualization of the Out of Range.

  * The color configured for the Text column will be used on the label when values is out of range. The default is black.

  * The color sets in the Background column of this section is used for the background of the label area if out of range. As default the background is yellow.




####  Script

The Script page allows configuring scripts connected to the events of the Steering Wheel window or of an application, in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

####  Menu

The menu of the window allows the access to the following commands, divided into sub menu:

#####  File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the current settings of the window. |
| Cancel |  | Closes the window without applying the current settings. |
| Load |  | Opens a dialog window to select a configuration file to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file on which the current settings can saved |
  
#####  View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl + Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl + Tab | Enables the page of the window next to the one currently in use. |
  
#####  Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
### 

####  Toolbar

The window toolbar enables the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode allowing to keep visualized the configuration window while the application window are in use. |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel command of the File menu). |
| Apply | Applies the current settings to the configuration of the graphic window (similar to the Apply command of the File menu). |
| Load | Similar to the Load command of the File menu |
| Save As | Similar to the Save As command of the File menu |
| Previous page | Similar to the Previous page command of the View menu |
| Next page | Similar to the Next page command of the View menu |
| Channel Browser | visualizes the pop-up menu to select the page in the Channel Browser window. |
  
##### Related Issues

  * [Display Value window](DisplayValueFeatures.md)

  * [Elements of the window](DisplayValueWndItems.md)

  * [Commands](DisplayValueCommands.md)


