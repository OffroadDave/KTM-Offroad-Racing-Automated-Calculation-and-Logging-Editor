### Window Configuration

This section describes the page thanks to which the user can configure some aspects of the [Numeric Table](NTBWndOverview.md) window.

The window contains the pages: Options and Script . The window moreover has a menu, a toolbar and pop-up menu that ease the access to the configuration and management commands of the window itself.

####  Options

The Options page enables configuration of the graphic aspect of the windows and it is divided into 2 sections.

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

#####  Window Options

It allows configuring the general window settings. Each element can be edited by double clicking with the left button of the mouse or by pressing the SPACE bar.

  * Custom Color: If checked, the window uses custom color: if unchecked the window uses the colors defined in Setup, General, Default Appearance.

  * Window: Sets the background color of the window.

  * Header: Sets the colors of the header.

    * Background: Sets the color of background

    * Text: Sets the color of text

  * Timestamps: Sets the colors of the Timestamps.

    * Background: Sets the color of background

    * Text: Sets the color of text

  * Values: Sets the colors of the Values.

    * Background: Sets the color of background

    * Text: Sets the color of text

  * Custom Font: Enables or Disables the custom font.

  * Font: Selection kind of font and size.

  * Unit: Enables or Disables the view of unit of measure.

  * Time Scale: Selects the time visualization mode

  *     * Relative

    * Absolute

  * Source: Selects the source of data

  * Frequency rate: Sets the values visualization frequency. You can choose within this values: 1Hz, 2Hz, 5Hz, 10Hz, 20Hz, 50Hz, 100Hz, 200Hz, 500Hz, 1KHz, 2KHz, 5KHz, 10KHz




#####  Graphs Options

This window does not allow any customization of the selected channel, it is only possible to include the channel itself.

  * Label : Displays the name of the channel. The name of the channel can be edited and can become a math expression if the sign = comes first.




#### Script

The Script page allows configuring the scripts connected to the events of the window or of the application, in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

####  Menu

The menu of the window allows access to the following commands, divided into sub menus:

#####  File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the present setting of the window. |
| Cancel |  | Closes the window without applying the present settings. |
| Load |  | Opens a dialog window to select a NTB configuration file to be loaded. |
| Save As |  | Opens a dialog window to select a NTB configuration file to be loaded (.ntb), on which the present settings must be saved. |
  
#####  Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies to clipboard the configurations of the channels selected in the Graph list section and after removes them from the list. |
| Copy | Ctrl + C | Copies to clipboard the configurations of the channels selected in the list of the Graphs section. |
| Paste | Ctrl + V | Pastes the configurations of the channels available in clipboard, adding them to the list of the Graphs section. |
  
#####  View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the window page previous to the one currently in use. |
| Next page | Ctrl \+ Tab | Enables the windows page next to the one currently in use. |
  
#####  Graph Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Graph | Adds a new element to the Graphs list of channel configuration. |
| Remove Graph | Removes from the Graphs list the settings of the selected channels. |
| Move Up | Move up by one position the elements selected in the Graphs list. |
| Move Down | Move down by one position the elements selected in the Graphs list. |
  
#####  Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
## 

####  Toolbar

The window toolbar allows the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode that allows keeping displayed the configuration window while the application windows are being used. |
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
  
## 

####  Pop-up Menu

The pop-up menu of the window can be displayed by right clicking the mouse on the Options page.

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
  
##### 

##### Related Issues

  * [Numeric Table window](NTBWndOverview.md)

  * [Elements of the window](NTBWndItems.md)

  * [Commands](NTBWndCommands.md)


