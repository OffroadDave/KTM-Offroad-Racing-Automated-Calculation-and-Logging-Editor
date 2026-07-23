### Commands

The main commands available in the [Graph](GRAWndOverview.md) window can be enabled through the

  * Options menu in the main menu of the application

  * The dedicated Toolbar buttons.

  * Area menu and area popup menu

  * Pop-up menu that can be displayed by clicking with the right button of the mouse in the graphic area of the window, on the Info boxes or on the Y scales of the selected channels.

  * Keyboard shortcuts




#### Options Menu

The pictures show two examples of menu options: the first refers to a window in Post Processing mode, the second to a window in Real Time mode.

The Options menu for the Graph and the Graph RT windows allows accessing the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Zoom |  | Displays the Zoom submenu. |
| Shift |  | Displays the Shift submenu |
| PopUp Legend | L | Displays the [PopUp Legend window](../../../WTXWorkWindows/WndPopupLegend/PopUp_Legend.md) |
| Set Datum | D | Set the [Datum cursor](GRAWndFeatures.md#GRAFeatureDatum) |
| Close Datum | Ctrl \+ D | Close the [Datum cursor](GRAWndFeatures.md#GRAFeatureDatum) |
| Max & Min | Ctrl \+ M | Displays or closes the window describing the [Max & Min](GRAWndFeatures.md#GRAFeatureMaxMin) values |
| Show absolute Min/Max on Graphs | Shift \+ M | Displays [Max & Min on Graph](GRAWndFeatures.md#GRAFeatureMaxMinOnGraph) |
| Show min/max peak to peak |  | Allows enabling the visualization of [Min/Max peak to peak](GRAWndFeatures.md#minMaxPeak2Peak). |
| Search | Shift \+ - Shift \+ + | Displays the Search submenu. |
| Graph Layout | P O M | Displays the sub menu to select the vertical arrangement of the channels (Parallel, Overlay, Manual); the mode selected is highlighted with a check mark |
| Y Scales Text mode |  | Displays the sub menu Y scales text mode |
| Graph Cursor |  | Displays the sub menu to select the graph cursor (single, multiple, vertical). |
| Cursor Value Mode |  | Displays the sub menu cursor value mode |
| Cursor Snap |  | Enables the Snap function to shift zooming area displayed. |
| Cursor Step |  | The Step allows setting the fixed distance at which the cursor can move, when the keyboard is used. Step values are in milliseconds. |
| Move cursor to... | Insert | Set cursor value position in the X-axis window. The value can be set in seconds or in meters depending if the Graph is in Time mode or Distance mode. |
| Graphs |  | Displays the Graphs submenu. |
| View |  | Displays the View submenu to select the graphic elements of the window that can be shown or hidden |
| View Information |  | Displays the [Graph Information window](GRAWndFeatures.md#GRAFeatureGraphInformation) which contains a summary of the current information about channels. |
| Time or Distance | X | Switch between Distance (space) mode and X-Scale time mode.. |
| Double X Axis |  | With this setting both axes, Time and Distance, are displayed. If Revolutions is configured, then the second axis is always Time. |
| Manual Section Definition | Alt \+ M | Starts Manual Section Definition operations |
| Confirm Operations | End | Confirms Manual Section Definition operations |
| Area | A | Displays the Area submenu for management of the window areas (show/hide, add, remove). |
| Move Previous | Alt \+ Left | If an append of laps is loaded, this command shifts the cursor in the corresponding position of the distance channel in the previous lap (homologous point). For example, if the position of the cursor is at 15 meters in a lap, the command moves the cursor 15 meters after the beginning of the previous lap. |
| Move Next | Alt \+ Right | If an append of laps is loaded, this command shifts the cursor in the corresponding position of the distance channel in the next lap (homologous point). For example, if the position of the cursor is 15 meters in a lap, the command moves the cursor 15 meters after the beginning of the next lap. |
| Switch to Telemetry/Post Processing | Ctrl \+ T | Allows switching from Post-Processing mode to Telemetry mode and vice versa. |
| DataSets |  | Shows in Datasets submenu the list of the datasets loaded that can be displayed. |
| Show Events | Ctrl \+ Alt + E | Enables to show the events configured in the window. |
| Open in Excel | Shift \+ X | Opens an Excel sheet displaying the series of data of the channels configured in the window |
| Open in Matlab |  | Opens a Workspace window of Matlab in which the required channel is loaded. |
| Stop Operations | Esc | Interrupts some operations on graph, as Manual Section Definition, Shifts, Zoom T1-T2. |
| Reset Min and Max value |  | Reset the minimum and the maximum value of all channels in the window during the acquisition. |
| Post Processing View | Q | The command opens a new graph window, called Post Processing View, which displays a snapshot of the RT window in the instant of the execution of this command. The command is available only in Time mode, in Distance mode is not enabled. |
| Freeze Real Time Graphs |  | Execute Freeze command (freeze / unfreeze displayed data). |
| Freehand Drawing |  | List of commands that are used to draw or write texts on the window. See [here](../../../Toolbars/ToolbarFreeHand.md) for a complete description of the commands. |
| Properties | E | Opens the interface to configure the [Graph window](GRAWndOverview.md) |
  
### 

##### Zoom

Displays the sub menu to work with zoom.

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Zoom In | + | Zooms the graphic area in respect to the X axis, displaying in better details the interval around the current position of the cursor (see also the [Zoom](GRAWndFeatures.md#GRAFeatureZoom) functions) |
| Zoom Out | - | Operation opposite to the Zoom In on the graphic area of the window: it displays in smaller detail in respect to the X axis of the interval around the current position of the cursor (see also the [Zoom](GRAWndFeatures.md#GRAFeatureZoom) functions). |
| Maximum Zoom | Ctrl \+ + | Displays the graphic area in respect to the whole interval of the X axis (see also the [Zoom](GRAWndFeatures.md#GRAFeatureZoom) functions). |
| Minimum Zoom | Ctrl \+ - | Zooms at maximum details in respect to the X axis, showing in maximum details the interval around the current position of the cursor (see also the [Zoom](GRAWndFeatures.md#GRAFeatureZoom) functions). |
| Zoom Box |  | Enable the Zoom Box mode to select the part of the graphic area to be zoomed (see also the [Zoom](GRAWndFeatures.md#GRAFeatureZoom) functions). |
| Zoom T1-T2 |  | Enable the T1-T2 mode to select the part of the graphic area to be zoomed in respect to the X axis (see also the [Zoom](GRAWndFeatures.md#GRAFeatureZoom) functions). |
| Zoom Fix Range |  | Displays the graphic area corresponding to a fixed interval of the X axis (see also the [Zoom](GRAWndFeatures.md#GRAFeatureZoom) functions) |
| Pan |  | The Pan mode allows shifting the displayed zoom area using the mouse or the keyboard (see also the [Zoom](GRAWndFeatures.md#GRAFeatureZoom) functions). |
| Zoom undo |  | Undo the last zoom operation. |
| Zoom redo |  | Redo the last zoom operation. |
  
##### Shift

Displays the Shift sub menu:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Shift a Dataset | Alt + S 1,2,3,4,5,6,7,8,9 Shift+R Enter Esc | Opens the Shift a Dataset submenu. For a description of the shift operations, see [here](GRAWndFeatures.md#GRAFeatureShiftGraphs). |
| Shift Graphs |  | Displays the Shift Graphs submenu. For a description of the shift operations, see [here](GRAWndFeatures.md#GRAFeatureShiftGraphs). |
| Reset All Shift | Alt \+ X | Resets the shift for each channel configured in the window restoring the initial situation. |
  
###### Shift a Dataset

Opens the Dataset submenu.

The first command, Activate shift mode, opens the shift operations. After selecting this command changes in Deactivate Shift Mode:

Choosing Deactivate shift mode the shift session will be closed. Otherwise, selecting any of the other commands, the corresponding Dataset will be activate for shift operations. To complete the shift, press Alt + S, or Enter, or choose Confirm Shift from pop-up menu or press the button of the shift on the toolbar, or choose Deactivate shift mode from options menu. With Esc, the shift is cancelled.

Shift+R,1,2,3,4,5,6,7,8,9,0: shortcuts to choose datasets. Selecting this item the cursor is displayed as hand-shaped and the lap can be dragged in the wished position with mouse or left and right arrow keys.

Enter: shortcut to confirm shift.

Esc: shortcut to cancel shift operations.

From toolbar the command displays in a pop-up menu the list of the Laps loaded; the shift function will be applied to the channel selected from this list.

###### Shift Graphs

Displays the Shift Graphs submenu with the following commands:

    * Shift a Graph: Selecting this item, the following list of the channel is displayed allowing to choose to which graph the shift should be applied. The shift function will be applied to the channel selected from this list. If more Laps are loaded, select a specific Lap from the next pop-up menu displayed.




Once the graphs are selected, the cursor is displayed as hand-shaped and the graph can be dragged in the desired position with mouse or left and right arrow keys. To complete the shift, press either Enter, or choose Confirm Shift from the pop-up menu or press the shift button on the toolbar. With Esc, the shift in use is cancelled.

    * Shift All Graphs Enables the shift on all the channels configured in the window. Selecting this item the cursor is displayed as hand-shaped and the graphs can be dragged in the wished position with mouse or left and right arrow keys. To complete the shift, please press Enter, or choose Confirm Shift from pop-up menu or press the button Shift on the toolbar. With Esc, the shift is cancelled.




##### Search

Displays the sub menu to select the method to search the absolute Minimum and Maximum points in the curve of the graph of the selected channel. To use the commands, a channel must be selected; the Minimum will place the cursor on the absolute minimum point in the part displayed of the graphs, the Maximum will place the cursor on the maximum point.

##### 

#####  Y Scales text mode

Configure the orientation of the scale labels (Automatic, Vertical, Horizontal).

##### Graph Cursor

Displays the sub menu to select the graph cursor (single, multiple, vertical):

  * Vertical (default) The window displays a vertical cursor.

  * Horizontal  The window displays the vertical cursor and a horizontal cursor for each channel in the window.

  * Intersection The window displays the vertical cursor and a short horizontal cursor for each channel in the window.

  * Multiple cross The window displays a cross cursor for each channel in the window.

  * Single cross The window displays a cross cursor for the selected channel.




##### 

##### Cursor Value Mode

Set the display mode of the cursor values.

Left: Cursor values are on the left side of the window

Right: Cursor values are on the right side of the window

Transparent: Cursor values are visible in the left side of the graphic area with a transparent effect.

Follows Graph Cursor: Cursor values are visible within the graphic area with a transparent effect as Transparent but in this mode cursor are always attached to the vertical cursor.

##### 

##### Cursor Snap

Enables the Snap function to shift zooming area displayed. The snap allows setting the fixed step at which the cursor can move, when the mouse is used.

##### Cursor Step

The Step allows setting the fixed step at which the cursor can move, when the keyboard is used. Step values are in milliseconds.

The settings of cursor step can be configured by Up and Down keys.

  * Up Increase cursor step in current window

  * Down Decrease cursor step in current window




##### Move cursor to...

Set cursor position value in the X-axis window. The value can be set in seconds or in meters depending if the Graph is in Time mode or Distance mode.

##### 

##### Graphs

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Remove Graphs | Delete | Removes from the window the channels currently selected. |
| Clear Selections |  | Clear channels selection in the current window. |
| Select Next Channel | Tab | Selects the next channel in the current window. |
| Select All Channels | Ctrl \+ A | Selects all channels in the current window. |
| Show Graphs | Alt \+ Ctrl + S | Shows the graphs corresponding to the selected channels. |
| Hide Graphs | Alt \+ Ctrl + H | Hides the graphs corresponding to the selected channels |
  
##### 

##### View

Displays the sub menu to select the graphic elements of the window that can be shown or hidden. The sub menu is a little bit different between Post Processing and Real Time, as shown in the picture below.

  * Cursor position shows/hides the information box indicating the cursor position on the X axis

  * X Scale shows/hides the X scale

  * Y Scales shows/hides the Y scales

  * Zoom Bar X, Zoom Bar Y shows/hides the vertical and horizontal Zoom Bars. Feature available only in Post Processing.

  * Cursor Values shows/hides the information boxes indicating the current values of the cursor.

  * X Scale Unit shows/hides the measurement unit of the X scale

  * Shift Value shows/hides the shift lap values. Feature available only in Post Processing.

  * Min Max Values shows/hide the box that displays the Maximum and Minimum values. Feature available only in Real Time.

  * Show All shows all the graphic elements

  * Hide All hides all the graphic elements




##### 

##### Time or distance

Switch between Distance (space) mode and Time mode of the X scale

  * Time: The X axis represents the time instants in seconds.

  * Distance: The X axis represents the distances covered in meters. The Distance channel must be configured in the Special Channels (see the [Special Channels](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgSpecialChannels) page of the [General Setup](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md) window).




##### 

##### Manual Section Definition

Track Sections may be manually defined at any time using the Manual Sections definition commands from the graph window. The command opens the [Track Sections Editor Toolbar](../../../Toolbars/ToolbarTrackSectionEditor.md), if it was not already open. In the graph, use the mouse to insert a series of section splits at points on a distance plot. Use the following shortcuts to support the manual sections definition:

End: shortcut to confirm manual splits

Esc: shortcut to abort operations

##### Area

Displays the sub menu to manage the Areas of the window that can be visualized, hidden, added and removed.

The "Show/Hide Area" command displays the list of Areas that can be visualized or hidden. 

If the area is visualized, the corresponding label is highlighted with a check mark. It's also possible to access this pop up menu with the "A" key on the keyboard while the focus is on the graph window.

##### Datasets

Shows in a sub menu the list of the Datasets loaded that can be displayed. Selecting the item Telemetry Dataset, the user can switch to the corresponding display mode

##### Open in Matlab

If you select a channel on a Graph window, you can launch the Open command in Matlab, using the right key or via the Options menu, which you will use to open a Workspace window of Matlab in which the required channel is loaded. On Matlab, the channel appears as an array of double values; with this array, you can carry out all the operations and analyses that Matlab allows you to use. If you repeat the Open in Matlab operation on a number of channels, they will be added to the list of channels in the workspace.

#### Toolbar Graph

The figure below shows an example of a toolbar; the toolbar is dynamic as it can be configured according to the licence granted, but also according to the situation in which it is created; for example the Pan command is displayed only if there is a zoom active or the Clear Selection command is displayed only if at least one channel is selected.

Post processing toolbar

Real time toolbar

The [Graph](GRAWndOverview.md) toolbar allows accessing the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Load |  | Opens a window to select a configuration file corresponding to a Graph window. |
| Save |  | Saves the current configuration of the window on a file. |
| Properties | E | Opens the interface to configure the [Graph Window](GRAWndOverview.md) |
| Zoom In | + | See the description of the command in the Zoom submenu. |
| Zoom Out | - | See the description of the command in the Zoom submenu. |
| Zoom Min | Ctrl \+ - | See the description of the command in the Zoom submenu. |
| Zoom Max | Ctrl \+ + | See the description of the command in the Zoom submenu. |
| Zoom Box |  | See the description of the command in the Zoom submenu. |
| Zoom T1-T2 |  | See the description of the command in the Zoom submenu. |
| Zoom Fix Range |  | See the description of the command in the Zoom submenu. |
| Pan |  | Enables the Pan mode to shift the zoom area displayed (see also the Zoom submenu). |
| Zoom Undo |  | Displays in a pop-up menu the list of the zooming operation carried out before the current display. Selecting an item from the list, the display of the window before the zooming is restored (see also the Zoom submenu). |
| Zoom Redo |  | Displays in a pop-up menu the list of the zooming operation carried out after the current display. Selecting an item from the list, the display of the window after the zooming is restored (see also the Zoom submenu). |
| Manual Section Definition | Alt \+ M | Starts Manual Section Definition operations |
| Show absolute Min/Max on Graph | Shift \+ M | Displays [Max & Min on Graph](GRAWndFeatures.md#GRAFeatureMaxMinOnGraph) |
| Graph Layout | P, O, M | Displays the sub menu to select the vertical arrangement of the channels (Parallel, Overlay, Manual); the mode selected is highlighted with a check mark |
| Time or Distance | X | Switch between Distance (space) mode and Time mode of the X scale. |
| Cursor Snap |  | Enables the Snap function to shift zooming area displayed. |
| Cursor Step |  | The Step allows setting the fixed step at which the cursor can move, when the keyboard is used. Step values are in milliseconds. |
| Shift Graphs |  | Displays the Shift submenu |
| View |  | Displays the View submenu to select the graphic elements of the window that can be shown or hidden |
| Show/Hide Y Scales in all windows |  | Shows / Hides the Y scales in all windows that can display it. |
| View Information |  | Displays the [Graph Information window](GRAWndFeatures.md#GRAFeatureGraphInformation) which contains a summary of the current information about channels. |
| Datasets |  | Shows in Datasets submenu the list of the datasets loaded that can be displayed. |
| Split Time |  | Shows the value of the split time in each sections, if sections are displayed. |
| Show Finish Line | Ctrl \+ End | Enables or disables the Finish Line flag. |
| Show Events | Ctrl \+ Alt + E | Enables to show the events configured in the window. |
| Next Event | Ctrl \+ Alt + N | Moves the cursor to the next event among the displayed ones. |
| Previous Event | Ctrl \+ Alt + P | Moves the cursor to the previous event among the displayed ones. |
| Post Processing View | Q | The command opens a new graph window, called Post Processing View, which displays a snapshot of the RT window in the instant of the execution of this command. The command is available only in Time mode, in Distance is not enable. |
| Freeze Real Time Graphs |  | Execute Freeze command (freeze / unfreeze displayed data). |
| Switch to Telemetry/Post Processing | Ctrl \+ T | Allows switching from Post-Processing mode to Telemetry mode and vice versa. |
  
##### 

##### Show Finish Line

When this flag is enabled, in case of more laps loaded (Append) the finish line is shown for each lap. For each lap the text refers to the Lap Marker (if available) or to the info Lap; the Lap Time is placed vertically along the divisions lines. The finish line can be transparent on the graphic area or outside the graphic area. The mode can be configured in [Setup/General](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgDefaultAppearence) .

The status Show/Hide of finish line is automatically saved by WinTAX4 and restored at the next opening.

### 

### Pop-up menu

By clicking with the right button of the mouse on the graphic area of the window, the following pop-up menu is displayed. The menu is dynamic as it can be configured according to the licence granted, but also according to the situation in which it is created; for example the Pan command is displayed only if there is a zoom active or the Clear Selection command is displayed only if at least one channel is selected. In general all pop-up commands of the pop-up menu are the same as those on the toolbar, the Options menu or other WinTAX4 menus.

This section will describe only the commands that have not already been described previously.

| COMMAND | DESCRIPTION |
| --- | --- |
| Print Window to Clipboard | Copy the image of the window in the current state on the clipboard. |
| Window | Opens the [Window submenu](../../../Menu/MenuWindow.md#MenuWindow_submenu). |
| Events | Shows Events submenu. |
  
##### 

##### Events

Shows all operations linked to the events;

The menu is dynamic and can be changed according to the situation. The example in the figure shows the following commands:

  * Show Events Enables the display of the events on the window

  * View All Events ( View Custom Events ) Shows all events (only the events configured) on the window.

  * Move right to next Moves the cursor to the next occurrence of an event

  * Move left to previous Moves the cursor to the previous occurrence of an event

  * Remove Events Opens a sub menu to select the event to be removed; all events can be removed. The removing is connected to the display of the event on the window and not to the cancellation of the event.




By clicking with the right button of the mouse on an Info box or on the Y scale of the channel, the following pop-up menu is displayed. The menu is dynamic as it can be configured according to the licence granted, but also according to the situation in which it is created. Most of all pop-up commands are the same as those on the toolbar, the Options menu or other.

This section will describe the commands that have not already been described previously.

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Copy Data to Clipboard | Ctrl \+ Shift + C | Copies to clipboard of Windows the series of data of the channels configured in the window |
| Set Comparison... |  | Opens an interface window for selecting datasets to compare and comparison mode. |
  
### 

##### Set Comparison...

Opens an interface window for selecting datasets to compare.

Click on each button to enable a comparison mode; the shortcuts are valid only if this window is open.

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Auto | U | default comparison |
| Difference | R | difference between selected data sets is calculated |
| Average | V | the plotted trace of a Channel represent the average of samples of compared laps. Average is automatically calculated in distance or in time base depending by the setting of x-axis. |
  
### 

### Keyboard Shortcut

To see the complete list of shortcuts available for the graph window, click [here](../../../Shortcut/WindowsShortcuts.md#ShortcutGraph).

Below are listed those shortcuts that do not appear in any of the commands and of the toolbar buttons described before. These commands can be executed only from keyboard. All these commands are not customizable.

| SHORTCUT | DESCRIPTION |
| --- | --- |
| Up | Increase cursor step in current window |
| Down | Decrease cursor step in current window |
| Left | Move the cursor left by one step |
| Right | Move the cursor right by one step |
| Ctrl + Left | Move the cursor left by step x 10 |
| Ctrl + Right | Move the cursor right by step x 10 |
| Ctrl + Left | Move current zoom left if there is a zoom active. |
| Ctrl + Right | Move current zoom right if there is a zoom active. |
| Shift + Left | Zoom Box selection left |
| Shift + Right | Zoom Box selection right |
| Ctrl + Up | Move Y Zoom up |
| Ctrl + Down | Move Y Zoom down |
|  |  |
  
##### Related Issues

  * [Graph window](GRAWndOverview.md)

  * [Elements of the window](GRAWndItems.md)

  * [Functions](GRAWndFeatures.md)

  * [Configuration](GRAWndCommands.md)


