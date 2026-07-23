### Configuration Window

The Diagnostics Report Configuration window allows to configure the look of the Diagnostics Report graphic windows.

The window has the following pages: Options, Script .

The window also has a menu, a toolbar and a pop-up menu that ease the access to the configuration and management commands of the window itself.

####  Options

The Options page allows configuring the graphic aspect of the Diagnostics Report windows and it is divided into 2 sections. Each configurable value can be edited by double clicking with the left button of the mouse or pressing the SPACE bar on the selected element

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

#####  Window Options

It allows configuring the settings of the aspect of the window.

  * Custom color: Enables the setting of the window customized colors




If it is enabled, it is used for the background of the graphic area, the color set in the Window column of this section .

If it is disabled, it is used for the setting of the Color Settings, section in the Default Appearance page of the General Setup window (General Settings, general configuration of the WinTAX4 environment).

  * Window: Sets the window background color.

  * Border: Background color of the graphic item (not used).

  * Text color: Color of the text of the graphic items.




#####  Graphs Options

It allows configuring the settings specific of each channel of the window. Each line identifies a configured channel, while the fields to be configured correspond to the columns. Multiple selections are possible through the CTRL and SHIFT keys.

  * Label: shows the name of the channel. The name of the channel can be edited and can become a math expression if the sign = comes first.

  * Multiplexor Channel: Settings of the multiplexing channel.

  *     * Enable: Enables/disables the selection channel.

    * Name: Name of the selection channel.

    * Value: Selection value.

  * Channel 1, 2, 3, 4: The diagnostics channels. The field shows some information about the channel: Name (number of bits) type of display.




  1.      * Name

     *        * Channel: name of the channel in the table

       * Display Label: Alias name, used in numeric type.



  1.      * View Options



  1.      1.         * Invert Bits Order: If selected, reverses the order of the bits in channel display.

        * Bit Label: Allows selection of the type of information displayed by the bit. The label is disabled because it is valid only for Diagnostics with Vertical layout.

        * 3-state view: Option available only for Diagnostics: if selected, the diagnostic bits are no longer displayed with current and historical values but with a single value. Besides switching on and off, this can have an intermediate status (displayed with the color configured in the 3State Color value) representing the bit currently switched off, but that previously was switched on at least once.

        * Bit Text External File: Checking this item, the edit box and the button nearby are enabled, and can be used to select a text file.

        * Text list; This option enables a text read from the selected file to be displayed in the bit label. The text file used must be adequately formatted and must include a line for each bit text. Empty lines will also be imported and visualized in the bit label, but this will need to be configured as Bit Text. 



  1.      * Bit Selection



  1.      1.         * Bit Mask: Selection mask for the bits.

        * Type: display mode of the channel

        *           * Bitmap , typical bits display

          * Numeric, numeric display (decimal or hexadecimal) of the value of the bits mask

        * Bit List: list of the bits selected in the mask

        *           * #: bit index

          * Text: bit text, displayed in the label and in the tooltip

          * Logic: calculation logics, Positive or Negative

          * ON Color: color of the bit when the value is high

          * OFF Color: color of bit when the value is low

          * Latch Time: minimum time for which the bit is displayed with a high value. If 0 the bit stays high only for the time in which the value is effectively high. The configured value is considered in cents of seconds.

          * 3State Color: color of the intermediate status (current value low, historical value high).




####  Script

The Script page allows configuring scripts connected to the events of the Diagnostics Report window or of the application, in VBScript or JScript. The script language can be chosen in Setup/General. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

## 

#### Menu

The menu of the window allows the access to the following commands, divided into sub menus:

##### File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the current settings to the configuration of the graphic window. |
| Cancel |  | Closes the window without applying the current settings to the configuration of the graphic window. |
| Load |  | Opens a dialog window to select a configuration file for the Diagnostic Report (.dgs) window to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file for the Diagnostic Report (.dgs) window on which the current settings can saved. |
  
##### Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies to clipboard the configuration of the selected channels in the list of the Graphs section, and removes them from the list of the Graphs section. |
| Copy | Ctrl + C | Copies to clipboard the configuration of the selected channels in the list of the Graphs section. |
| Paste | Ctrl + V | Pastes the configuration of the channels available on clipboard, adding them to the list of the section Graphs. |
  
##### View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use. |
  
##### Graph Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Graph | Adds a new element to the Graphs list of channels configuration. |
| Remove Graph | Removes from the Graphs list the configurations of the selected channels. |
| Move Up | Moves up by one position the selected elements in the Graphs list. |
| Move Down | Moves down by one position the selected elements in the Graphs list. |
  
## 

#### Toolbar

The window toolbar enables the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode allowing to keep displayed the configuration window while the application window are in use |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel command of the File menu) |
| Apply | Applies the current settings to the configuration of the graphic window (similar to the Apply command of the File menu) |
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

#### Pop-up Menu

The pop-up menu of the window can be displayed by clicking with the right button of the mouse on the Options page..

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
| Restore Default | Modifies the channel configuration restoring the settings in the parameters. |
| Set As Default | Modifies the parameters configuration with the current settings of the channel. |
| Edit | Edits the selected cell. |
  
##### 

##### Related Issues

  * [Diagnostics Report window](DGSREPWndOverview.md)

  * [Elements of the window](DGSREPWndItems.md)

  * [Functions](DGSREPWndFeatures.md)

  * [Commands](DGSREPWndCommands.md)


