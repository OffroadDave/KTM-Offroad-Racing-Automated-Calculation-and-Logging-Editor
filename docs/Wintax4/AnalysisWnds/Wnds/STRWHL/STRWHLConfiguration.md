### Configuration Window

The Steering Wheel Configuration window allows setting the look of the [Steering Wheel windows](SteeringWheelFeatures.md); it is formed by the Options and Script pages.

The window moreover has a menu, a toolbar and a pop-up menu that eases the access to the configuration and management commands of the window itself.

####  Options

The Options page allows configuring the graphic aspect of the Steering Wheel windows and it is divided into 2 sections.

### 

#####  Window Options

allows configuring the settings of the windows layout. Each element, except checks, can be edited by double clicking with the left button of the mouse or with the SPACE bar.

  * Custom Color: allows the setting of the window customized colors.

  * If it is enabled, the color sets in the Window column of this section is used for the background of the graphic area.

  * If it is disabled, it is used the Color Settings configuration, in the Default Appearance page of the General Setup window (General Settings, general configuration of the WinTAX4 environment).



  * Window: set the background color .

  * Custom Font: allows the settings of the information area text font.

  * Font: When Custom font is selected, opens the [Font configuration page](../Common/Common.md#CommonConfFontLight). 




#####  Graphs Options

It allows configuring the settings specific of each channel of the window. Each line identifies a configured channel, while the fields to be configured correspond to the columns. Each element can be edited by double clicking with the left button of the mouse or with the SPACE bar. Multiple selections are possible through the CTRL and SHIFT keys.

  * Label: shows the name of the channel. The name of the channel can be edited and can become a math expression if the sign = comes first.

  * Color: local configuration of steering wheel instruments colors. The local setup overwrites the global one defined in Window area. The following elements are customizable:

  *     1.        * Steer

       * Steer Background

       * Scale

       * Text Background

       * Text




  * Scale: allows configuring the minimum and maximum thresholds of channel. As default the values are taken from Channel Parameters Setup

  *     * Local: allows configuring the desired thresholds of channel. If checked, as default, the values are taken from Channel Parameters Setup, if available. If unchecked the value is calculated from the maximum and the minimum values of the channels or taken from Channel Parameters Setup, if available.

    * Min: The minimum values of the channel

    * Max: The maximum values of the channel

    * Layout: This field can be clockwise or counterclockwise; it refers to the direction of rotation of the circular scale.




  * Show: allows showing/hiding some elements of the window

  *     * Label: If checked, shows the name of the channel

    * Value: If checked, shows the value of the channel in the current position




####  Script

The Script page allows configuring scripts connected to the events of the Steering Wheel window or of application, in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

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
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use. |
  
#####  Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
### 

####  Pop-up Menu

The pop-up menu of the Steering Wheel Configuration window can be displayed by clicking with the right button of the mouse on the Options page.

The pop-up menu allows access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Restore Default | Modifies the channel configuration restoring the settings in the parameters. |
| Set As Default | Modifies the parameters configuration with the current settings of the channel. |
| Edit | Edits the selected cell. |
  
####  Toolbar

The window toolbar enables access to the following commands:

| COMMAND | COMMAND |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode allowing to keep visualized the configuration window while the application window are in use. |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel command of the File menu). |
| Apply | Applies the current settings to the configuration of the graphic window (similar to the Apply command of the File menu). |
| Load | Similar to the Load command of the File menu |
| Save As | Similar to the Save As command of the File menu |
| Previous page | Similar to the Previous page command of the View menu |
| Next page | Similar to the Next page command of the View menu |
| Channel Browser | Visualizes the pop-up menu to select the page in the Channel Browser window |
  
##### Related Issues

  * [Steering Wheel window](SteeringWheelFeatures.md)

  * [Elements of the window](STRWHLWndItems.md)

  * [Commands](STRWHLCommands.md)


