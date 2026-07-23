### Configuration Window 

The Diagnostics Configuration window allows configuring the aspect of the Diagnostics graphic windows. The window has the following pages, Options and Script .

The window has also a menu, a toolbar and a pop-up menu that ease the access to the configuration and management commands of the window itself.

####  Options

The Options page allows graphic aspect configuration on the Diagnostics window and it is divided into 2 sections.

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

#####  Window Options

Allows configuration of the window aspect. Each box can be edited by double clicking with the left button of the mouse or with the SPACE bar.

  * Custom color: enables the setting of the window customized colors. If it is enabled, the window will use the custom colors otherwise it will use colors configured in the Color Settings section of the Default Appearance page on the General Setup window.



  * Window: Window background color.

  * Border: Border color; it is used only in numeric type.

  * Text Color: Color of the text of the graphic items.

  * Layout: Horizontal and vertical arrangement of the diagnostics.




#####  Diagnostic Options

It enables configuration of the specific settings of each window channel. Each line identifies a configured channel, while the fields to be configured correspond to the columns. Each element can be edited by double clicking with the left button of the mouse or with the SPACE bar. Multiple selections are possible through the CTRL and SHIFT keys.

  * Label: Shows the name of the diagnostic.

  * Multiplexor Channel: Settings of the multiplexing channel.

  *     * Enable: Enables/disables the multiplexer channel.

    * Name: Name of the multiplexer channel.

    * Value: Multiplexer value.

  * Source Channels (Channel 1, Channel 2, Channel 3, Channel 4): The diagnostics channels. Each field shows some information of the channel: Name (number of bits) and type of visualization.




  1.      * Name

     *        * Channel: channel name

       * Display Label: Alias name, used in numeric type.



  1.      * View Options



  1.      1.         * Invert Bits Order: If selected, reverses the order of the bits in channel display.

        * Bit Label: Selection of the type of information displayed with the bit (valid only for Diagnostics with Vertical layout)

        * 3-state view: Option available only for Diagnostics; if it is selected, the diagnostics bits are no longer displayed with current value and historical value, but with a single value that besides the on and off switch can have an intermediate status (displayed with the color configured in the 3-State Color value) representing the bit currently switched off but that in the past it has switched on at least once.

        * Bit Text External File: Checking this item, editing is enabled and it is possible to select a text file and the columns becomes grey. This option enables to display in the bit label a text read from the selected .txt file. The text file used must be adequately formatted: It must include a line for each bit, and empty lines will be used as well. It requires the Bit Label configured on Bit Text. For example the following text shows a 32 bit text external file where bit 7 and bit 13 are not configured.

Text bit 0

        1. Text bit 1

        2. Text bit 2

        3. Text bit 3

        4. Text bit 4

        5. Text bit 5

        6. Text bit 6

        7.         8. Text bit 8

        9. Text bit 9

        10. Text bit 10

        11. Text bit 11

        12. Text bit 12

        13.         14. Text bit 14

        15. Text bit 15

        16. Text bit 16

        17. Text bit 17

        18. Text bit 18

        19. Text bit 19

        20. Text bit 20

        21. Text bit 21

        22. Text bit 22

        23. Text bit 23

        24. Text bit 24

        25. Text bit 25

        26. Text bit 26

        27. Text bit 27

        28. Text bit 28

        29. Text bit 29

        30. Text bit 30

        31. Text bit 31



  1.      * Bits Selection



  1.      1.         * Bit Mask: Selection mask for the bits.

        * Type: display mode of the channel

        *           * Bitmap , typical bits display

          * Numeric, numeric display (decimal or hexadecimal) of the value of the bits mask

          * **Bitfield : **This mode, available for channels that include a bitfield definition generated on SYSMA, displays the **name and status** of each individual bit. The Bitfield mode allows selecting between **bitfield** and **bitmap mask** visualization mode. Clicking on the bitfield configuration button shows the configuration that has been configured in sysma. The following configuration cannot be edited and can be visualized only for reference. To change the settings please use SYSMA.

        * Bit List: list of the bits selected in the mask

        *           * #: bit index

          * Text: bit text, displayed in the label and in the tooltip

          * Logic: calculation logics, Positive or Negative

          * ON Color: color of the bit when the value is high

          * OFF Color: color of bit when the value is low

          * Latch Time: minimum time for which the bit is displayed with a high value. If 0 the bit stays high only for the time in which the value is effectively high. The configured value is considered in hundredths of seconds.

          * 3State Color: color of the intermediate status (current value low, historical value high).




Note: Channels that have not been acquired, are displayed with the same color as the background. (The color of the background can be changed in the main settings.)

#### 

####  Script

The Script page allows configuring scripts connected to the events of the Diagnostics window or of the application, in VBScript or JScript. The choice of the script language can be done in Setup/General. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

#### Menu

The Diagnostics Configuration menu window allows access to the following commands, divided into sub menu:

##### File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the current settings to the configuration of the graphic window |
| Cancel |  | Closes the window without applying the current settings of the graphic window |
| Load |  | Opens a dialog window to select a configuration file for the Diagnostic (.dgs) window to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file for the Diagnostic (.dgs) window on which the current settings can be saved |
  
##### Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies to clipboard the configuration of the channels selected in the graph list section, Copied data will also be removed from the list in the graph list section. |
| Copy | Ctrl + C | Copies to clipboard the configuration of the selected channels in the list of the Graphs section |
| Paste | Ctrl + V | Pastes the configuration of the channels available on clipboard, adding them to the list of the section Graphs |
  
##### View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use |
  
##### Graph Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Graph | Adds a new element to the Graphs list of channels configuration |
| Remove Graph | Removes from the Graphs list the configurations of the selected channels |
| Move Up | Moves up by one position the selected elements in the Graphs list |
| Move Down | Moves down by one position the selected elements in the Graphs list |
  
##### Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
## 

#### Toolbar

The Diagnostics Configuration toolbar window allows access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode allowing to keep displaying the configuration window while the application window is in use |
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
| Channel Browser | displays the pop-up menu to select the page in the Channel Browser window. |
  
#### Pop-up Menu

The pop-up menu of the Diagnostics Configuration window can be displayed by clicking with the right button of the mouse on the Options page.

The pop-up menu of the Graph Configuration allows access to the following commands:

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

  * [Diagnostics window](DGSWndOverview.md)

  * [Elements of the window](DGSWndItems.md)

  * [Functions](DGSWndFeatures.md)

  * [Commands](DGSWndCommands.md)


