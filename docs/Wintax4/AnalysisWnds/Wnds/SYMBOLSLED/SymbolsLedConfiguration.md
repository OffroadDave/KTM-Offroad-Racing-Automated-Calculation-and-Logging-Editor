### Configuration Window

The Symbols Led Configuration window allows setting the look of the [Symbols Led windows](SymbolsLedFeatures.md); it is formed by the Options and Script pages.

The window has a menu, a toolbar and a pop-up menu that eases the access to the configuration and management commands of the window itself.

#### Options Page

The Options page allows configuring the graphic aspect of the Symbols Led windows and it is divided in two sections.

### 

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

##### Window Options

It allows configuring the settings of the window. Each element can be edited by double clicking with the left button of the mouse or with the SPACE bar.

  * Custom Color: allows the setting of the window background color.

  * Window: The color is used for the background of the graphic area; if Custom Color is disabled, the window uses as background the Window color set in Color Settings configuration, in the Default Appearance page of the General Setup window (General Settings, general configuration of the WinTAX4 environment).

  * Font: allows the settings of the window fonts.

    * Label: Opens the [Font configuration page](../Common/Common.md#CommonConfFontLight). 

    * Value: Opens the [Font configuration page](../Common/Common.md#CommonConfFontLight). 

  * Layout: this combo permits to choose the layout mode

  *     * Horizontal: In this case a single row is displayed and the channels are side by side

    * Vertical: In this case the channels are displayed inside a single column

    * Auto: In this case the window creates a grid with N rows and M columns based on the number of valid channels and based on the size of the window.



  * Size: allows configuring the size of a single bitmap or led cell. There are two choices: Normal (64x48, default) and Small (48x36).




##### 

##### Graphs Options

It allows configuring the settings of the channel message and the related conditions. Each element can be edited by double clicking with the left button of the mouse or with the SPACE bar.

  * Label: shows the name of the channel. Label can be edited and can become a math expression if the sign = comes first.

  * Display name: allows the setting of the label customized text.

  * Enabled: if checked, the window shows the Text configured; if unchecked ,the window display the channel Label.

  * Text: text alternative to Label to be displayed in the window.

  * Text color: color of text displayed by the window with no conditions applied.

  * Default Symbols: allows the setting of the channel with no conditions applied.

  * Type: selects the type of the output image of the channel, Led or Bitmap. The default is Led.

  * Led: selects the color of the Led with non conditions applied.

  * Bitmap: selects the bitmap related to the channel with non conditions applied. The default is a black bitmap.



  * Symbols back: color of background led displayed by the window with no conditions applied.

  * Label: permits to configure the label

  * Show: if checked, the window shows the label of the channel.

  * Text: selects the text color of the label of the channel.

  * Back: selects the background color of the label.



  * Value: permits to configure the label

  * Show: if checked, the window shows the values of the channel.

  * Text: selects the text color of the value of the channel.

  * Back: selects the background color of the value of the channel.



  * Alarm: The alarm configuration range used will be as in Channels Parameters at the Alarm page. Every time a channel is inside the alarm range, it will be displayed with relevant colors.

  *   * Enabled: enables the visualization of the Alarms.

  * The color sets in the Text column of this section is used for the label if in alarm range. The default is black.

  * The color sets in the Background column of this section is used for the background of the label area if in alarm range. As default the background is red.



  * Out of range: Refers to the valid range values as set in Channels Parameters at the Alarm page. Every time a channel is outside the alarm range but inside valid range, it will be displayed with relevant colors. This option is not depending on the alarm.

  *   * Enabled: enables the visualization of the Out of Range.

  * The color sets in the Text column of this section is used for the label if in out of range. The default is black.

  * The color sets in the Background column of this section is used for the background of the label area if out of range. As default the background is yellow.



  * Conditions: displays the status of the conditions (On or Off). To modify the conditions, edit the associated configuration window.

  * Hold Time: This field permits the configuration of the number of seconds that the user wants the condition to be maintained visible after the status has been changed to "ON". The range of values is from 0 to 5 seconds. The default value is 0. This function can be enabled only in Real Time mode.




The window allows configuring the settings of the conditions and the related bitmap. Each element, except checks, can be edited by double clicking with the left button of the mouse or with the SPACE bar.

Remarks: The maximum number of conditions allowed is 16. The conditions are read in sequence: if one is verified, the following ones are ignored; if it is not verified, the next will be examined. If neither condition is checked, the window displays the text configured in Channel section.

  * Enabled: if checked, the condition is included in window elaboration; if unchecked, the condition will be ignored.

  * Condition: logical operator

  * = equal operator, check whether the actual value of the channel is equal to the configured value.

  * <> different operator, check whether the actual value of the channel is different to the configured value.

  * > greater than operator, check whether the actual value of the channel is greater than the configured value.

  * < less than operator, check whether the actual value of the channel is less than the configured value.

  * >= greater than or equal operator, check whether the actual value of the channel is greater than or equal to the configured value.

  * <= less than or equal operator, check whether the actual value of the channel is less than or equal to the configured value.

  * Value: the value in this field will be compared together with the logical operator condition .

  * Background: background color displayed by the window if this condition is verified.



  * Bitmap: bitmap displayed by the window if this condition is verified. This options is active if the selected item is Bitmap.




or

  * Led: set the color of the Led if this condition is verified. This options is active if selected item is Led.




Condition can be added, removed or moved using the toolbar commands. 

Add: Adds a new empty condition

Remove: Deletes the selected conditions

Clear All: Removes all the conditions

Move Up: Moves up by one position the selected elements in the condition list.

Move Down: Moves down by one position the selected elements in the condition list.

#### Script Page

The Script page allows configuring scripts connected to the events of the Symbols Led window or of application, in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

#### Menu

The window menu allows the access to the following commands, divided into sub menu:

##### File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the current settings of the window. |
| Cancel |  | Closes the window without applying the current settings. |
| Load |  | Opens a dialog window to select a configuration file to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file on which the current settings can saved |
  
### 

##### Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl \+ X | Enabled only on Script page. Copies to clipboard the selected text and removes it from script edit box. |
| Copy | Ctrl \+ C | Enabled only on Script page. Copies to clipboard the selected text. |
| Paste | Ctrl \+ V | Enabled only on Script page. Pastes the text from the clipboard in the script edit box. |
  
### 

##### View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl + Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl + Tab | Enables the page of the window next to the one currently in use. |
  
### 

##### Graph Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Graph | Adds a new element to the conditions list |
| Remove Graph | Removes a condition from the list. |
| Move Up | Moves up by one position the elements selected from the conditions list |
| Move Down | Moves down by one position the elements selected from the conditions list |
  
### 

##### Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
### 

#### Pop-up Menu

The pop-up menu of the Symbols Led Configuration window can be displayed by clicking with the right button of the mouse on the Options page.

The pop-up menu allows the access to the following commands:

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
  
#### Toolbar

The window toolbar enables access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode ensuring to keep displayed the configuration window while other application windows are being used |
| Cancel | Closes the configuration window without applying the setting (similar to the Cancel command of the File menu) |
| Apply | Applies the present setting to the graphs window (similar to the Apply command of the File menu) |
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
| Channel Browser | Shows the pop-up menu to select the page in the Channel Browser window . |
  
##### Related Issues

  * [Symbols Led window](SymbolsLedFeatures.md)

  * [Elements of the window](SymbolsLedWndItems.md)

  * [Commands](SymbolsLedCommands.md)


