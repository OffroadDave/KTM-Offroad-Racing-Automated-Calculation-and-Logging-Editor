### Commands

The main commands available in the [XY Window](XYWndOverview.md) can be enabled through

  * The Options menu in the main menu of the application

  * The buttons of the dedicated Toolbar

  * The pop-up menu that can be displayed by clicking with the right button of the mouse on the graphic area of the window

  * Keyboard shortcuts




#### Options Menu

The Options menu allows the access to the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Graph Layout | P O M | Displays the sub menu to select the vertical arrangement mode of the channels (Parallel, Overlay, Manual); the selected mode is highlighted with a check mark |
| Remove Graphs |  | Removes from the window the currently selected channels |
| Clear Selection |  | Deselect all the selected channels in the window |
| Select All Channels | Ctrl \+ A | Selects all channels in the window |
| View |  | Displays the View submenu |
| Best Fit Information |  | Shows the values of the coefficients |
| Switch to Telemetry Switch to Post Processing | Ctrl \+ T | Allows switching from the Post Processing mode to the Telemetry mode and vice versa. |
| Stop Operations | Esc | Exit from the Pan mode. |
| Freehand Drawing |  | List of commands that are used to draw or write texts on the window. See [here](../../../Toolbars/ToolbarFreeHand.md) for a complete description of the commands. |
| Properties | E | Opens the interface to configure the XY window |
  
#### 

#### View submenu

Displays the pop-up sub menu to select the graphic elements of the window that can be shown or hidden. The viewed elements are highlighted with a check mark on the left.

  * Show / Hide Channels, displays in a pop-up menu the list of the channels that can be viewed




selecting "All graphs" command, all the configured channels are displayed; selecting an element only the corresponding channel is displayed

  * Title, shows/hides the line which contains the header information

  * Legend, appears only in Legend Mode. Shows/hides the line which contains the legend information

  * Left Scales Y, shows/hides the scales of the Y channels on the left of the window

  * Right Scales Y, shows/hides the scales of the Y channels on the right of the window

  * Left Cursor Values, shows/hides the info boxes containing the current values of the cursor on the left of the window

  * Right Cursor Values, shows/hides the info boxes containing the current values of the cursor on the right of the window

  * Zoom Bar X, shows/hides the horizontal Zoom Bar

  * Zoom Bar Y, shows/hides the vertical Zoom Bar

  * X Scale, shows/hides the X scale

  * Show All, shows all graphic elements

  * Hide All, hides all graphic elements




##### Best Fit Information

If at least one channel is configured with Best Fit, it shows a summary containing the values of the coefficients used for the interpolation curve of each channel and the resulting polynomial formula. See [Analysis](XYWndFeatures.md#XYFeatureAnalysis) for further details.

#### 

#### Graph Toolbar

The toolbar allows the access to the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Load |  | Opens a window to select a configuration file corresponding to a XY window |
| Save |  | Saves the current window configuration on a file |
| Properties | E | See the description of the command in the Options Table. |
| Zoom In | + | Enlarges the graphic area in relation to the X axis, displaying in higher detail the range around the cursor position (see also the Zoom functions) |
| Zoom Out | - | Operation opposite to the Zoom In in the graphic area of the window: it shows in smaller detail in relation to the X axis of the range around the present position of the cursor (see also the Zoom functions) |
| Minimum Zoom | Ctrl \+ - | Shows the graphic area in relation to the whole range |
| Pan |  | Enables the Pan mode to move the displayed zoom area (see also the Zoom functions) |
| Zoom Undo |  | Rolls back the zoom level to the previous zooming situations. |
| Zoom Redo |  | It permits to restore a situation previous to the usage of the "Zoom Undo" command, cancelling the outcome of it. |
| Graph Layout |  | See the description of the command in the Options Table. |
| View |  | See the description of the command in the Options Table. |
| Show/Hide Y scales in All windows |  | Shows / Hides the Y scales in all windows that display them. |
| Show Events | Alt \+ Ctrl + E | Enables the display of the events configured in the window. |
| Best Fit Information |  | Shows the Best Fit coefficients window |
| Datasets |  | Shows in a sub menu the loaded Datasets list that can be selected (available only in Post Processing mode) |
| Stop Operations | Esc | Exit from the Pan mode. |
| Switch to Telemetry/Post Processing | Ctrl \+ T | See the description of the command in the Options Table. |
  
#### 

#### Pop-up Menu

By clicking with the right button of the mouse on the graphic area of the window the following pop-up menu is displayed:

This section will describe only the commands that have not already been described previously.

| COMMAND | DESCRIPTION |
| --- | --- |
| Print Window to Clipboard | Copy the image of the window in the current state on the clipboard. |
| Window | Opens the [Window submenu](../../../Menu/MenuWindow.md#MenuWindow_submenu) |
| Datasets | Shows the Dataset sub menu. |
| Events | Shows the Events submenu. |
  
##### Datasets

Shows in a sub menu the list of the Datasets loaded that can be displayed. Selecting the item Telemetry Dataset, the user can switch to the corresponding display mode

##### 

##### Events

Shows the Events submenu:

it is a dynamic menu and it can be modified according to the situation. In the example in the figure the following commands are listed:

  * Show Events Enables the display of the events configured in the window

  * View All Events ( View Custom Events ) shows all events (only the configured events) in the window.

  * Remove Events Opens a sub menu to select the event to be removed; all events can be removed. The removal is intended linked to the display of the event in the window and not to the cancellation of the event.




By clicking with the right button of the mouse on an Info box or on the Y scale of a channel, the following pop-up menu is displayed:

This section will describe only the commands that have not already been described previously.

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Hide Graphs | Alt + Ctrl + H | Hides trace of the selected channel and strikeout the channel name in order to indicate that the channel is not displayed but still available |
| Show Graphs | Alt + Ctrl + S | Shows the channel if hidden. |
  
#### 

#### Keyboard Shortcut

To see the complete list of shortcuts available for the xy window, click [here](../../../Shortcut/WindowsShortcuts.md#ShortcutXY).

Below are listed those shortcuts that do not appear in any of the commands and of the toolbar buttons described before. These commands can be executed only from keyboard. All these commands are not customizable.

| SHORTCUT | DESCRIPTION |
| --- | --- |
| Esc | Exit from the Pan mode (Stop Operations). |
| Left | View next channel in single mode |
| Right | View previous channel in single mode |
| Alt + Ctrl + N | Places cursor on next event occurrence |
| Alt + Ctrl + P | Places cursor on previous event occurrence |
| Tab | Selects next channel in current window |
| Del | Removes selected channels |
|  |  |
  
##### 

##### Related Issues

  * [XY window](XYWndOverview.md)

  * [Elements of the window](XYWndItems.md)

  * [Function](XYWndFeatures.md)

  * [Configuration](XYWndSetup.md)


