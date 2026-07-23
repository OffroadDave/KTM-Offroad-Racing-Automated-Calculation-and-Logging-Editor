### Channel Configuration Window

The configuration window allows configuration of the [Channels windows](CHNWndOverview.md). The configuration window contains two pages: Options, Script.

Menu, toolbar and pop-up menu aid the user to configure and manage commands of window.

#### Options

The Options page allows configuring the graphical aspect of the window; it is composed by two sections: Window Options and Channel List.

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

##### Window Options

It allows configuring the graphical settings of the window. Each box can be edited by double clicking with the left button of the mouse or by pressing the shortcut Space Bar.

  * View: selects the display mode of the channels

  *     * All Channels: All channels in the logging table

    * Custom: The list configured by user in graphs sections or filled using drag & drop; in Custom View it is also possible to insert other kind of channels, not only logged.

    * List of groups: All channels of a group in the logging table

  * Custom Color: If checked, the window uses custom color: if unchecked the window uses the colors defined in Setup, General, Default Appearance.

  * Window: Sets the background color of the window.

  * Grid: Sets the background color of the grid.

  * Custom Font: If checked the window use the local font; if unchecked the window uses the font configured in Setup, General, Default Appearance.

  * Font: Opens the [Font configuration page](../Common/Common.md#CommonConfFont). 

  * Unit: Enables to display the measurement unit of the channel configured in [Channel Parameters](../../../WTXSetup/WndChannelsParametersSetup/Channels_Parameters.md).

  * Header: Enables to show the rows Time and Distance in the header of the window




##### Channel List

It allows configuring the specific setting for each channel to be displayed. Each row refers to a configured channel, while columns refer to the field to be configured. Each box can be edited by double clicking with the left button of the mouse or by pressing the Space Bar.

  * Label: displays the name of the channel. The name of the channel can be edited and can become a math expression if a "=" sign comes first.

  * Format: it displays the style for the display of the current channel value. To modify the setting, the corresponding configuration window must be opened to configure the setting for the display format of the channel values.




In the combo on the left side the numeric format can be selected, in the combo on the right the number of decimals is selected. Please find below a list of the possible formats

  1.      * Auto the channel format is kept unchanged.

     * Dec the decimal format allows a maximum of 5 digits after the comma.

     * Numeric the numeric format allows a maximum 15 digits after the comma.

     * Scientific the scientific format allows a maximum 15 digits after the format; the result is written in exponential form.

     * Hex hexadecimal format; decimals cannot be configured.

     * Bin binary format; decimals cannot be configured.

     * ASCII text format; decimals cannot be configured.




#### Script

The Script page enables to configure scripts linked to the events of the [Channels](CHNWndOverview.md) window or of the application, in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

#### Menu

The menu of the window allows access to the following commands, divided by sub menus:

##### File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the actual settings of the window |
| Cancel |  | Closes the window without applying the current settings |
| Load |  | Opens a dialog window to select a configuration file to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file (.chn), to save the current settings |
  
##### Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies to clipboard the setting of the channels selected from the Graphs section list, and removes them from the list. |
| Copy | Ctrl + C | Copies to clipboard the setting of the channels selected from the Graphs section list. |
| Paste | Ctrl + V | Pastes the setting of the channels in the clipboard, adding them to the Graphs section list. |
  
##### View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl + Shift + Tab | Enables the page of the window previous to the one currently in use |
| Next page | Ctrl + Tab | Enables the page of the window next to the one currently in use |
  
##### Graph Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Graph | Adds a new element to the Graphs list for the configuration of channels |
| Remove Graph | Removes from the Graphs list the setting of the selected channels |
| Move Up | Moves up by one position the elements selected from the Graphs list |
| Move Down | Moves down by one position the elements selected from the Graphs list |
  
#####  Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
## 

#### Toolbar

The toolbar enables access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the "Keep Visible" mode ensuring to keep displayed the configuration window while other application windows are being used |
| Cancel | Closes the configuration window without applying the setting |
| Apply | Applies the present setting to the graphs window |
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
  
#### Pop-up Menu

The pop-up menu of the window can be displayed by clicking with the right button of the mouse on the Options page.

The pop-up menu of the window enables the access to the following commands:

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
| Set As Default | Sets the configurations of the channel in the Channel Parameters. |
| Edit... | Gives the possibility to edit the selected parameter (similar to the double click). |
  
##### Related Issues

  * [Channels window](CHNWndOverview.md)

  * [Elements of the window](CHNWndItems.md)

  * [Functions](CHNWndFeatures.md)

  * [Commands](CHNWndCommands.md)


