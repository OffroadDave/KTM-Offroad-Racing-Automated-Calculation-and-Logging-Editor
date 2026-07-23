### Commands

The main commands available in the [Track](CRVWndOverview.md) window can be enabled through:

  * the Options menu in the main menu of the application

  * the buttons of the dedicated Toolbar ,

  * Pop-up menu that can be displayed by clicking with the right button of the mouse on the graphic area of the window, on the Info boxes or on the Y scales corresponding to the selected channels.

  * Keyboard shortcuts




#### Options Menu

The Options menu allows the access to the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Zoom |  | Opens the Zoom submenu |
| Track |  | Show/Hide the track calculated in Track Editor. |
| GE Map |  | Display or hide the map of Google Earth. |
| GE Calibration |  | Fine calibration of the satellite map of Google Earth. |
| Clear GE Calibration |  | It removes the calibration function from the track, restoring the original position and dimensions. |
| Marker |  | Displays configured markers; the display of the marker, if enabled, overtops the display of the track sections. |
| Select lap marker |  | This command appears only when the Histogram Marker is selected and a compare of laps is loaded. The command permits to select the desired lap. |
| Track Sections |  | Displays Track Sections as configured in Track Editor. |
| Split Time |  | Shows the value of the split time in each section, if sections are displayed. |
| Track Sections Report |  | Adds text boxes to the track to show the report in correspondence of the section. |
| Automatic Placement |  | Performs the automatic positioning of the available labels on the track window. |
| Manual Section Definition | Alt + M | Starts Manual Section Definition operations |
| Confirm Operations | End | Confirms Manual Section Definition operations |
| Track Channels |  | Displays the sub menu to manage track channels |
| Trajectory |  | Opens a menu from which you can select or de-select the trajectory of one or all the laps loaded. |
| Customization |  | Displays the customization submenu. |
| Ruler |  | Displays the Ruler submenu. |
| Track Editor | ALT + T | Opens the [Track Editor](../../../Tools/TrackEditor/Track_Editor.md) that allows creating or loading Track and Track Sections. |
| Stop Operations | Esc | Aborts the GE calibration operations. |
| Freehand Drawing |  | List of commands that are used to draw or write texts on the window. See [here](../../../Toolbars/ToolbarFreeHand.md) for a complete description of the commands. |
| Properties | E | Opens the interface to configure the window. |
| Switch to Telemetry/Post Processing | Ctrl \+ T | Allows switching from Post-Processing mode to Telemetry mode and vice versa. |
  
##### 

##### Zoom

The Options/Zoom command opens the following submenu.

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Zoom In | + | Zooms the graphic area in respect to both axis, displaying in better details the interval around the current position of the cursor. |
| Zoom Out | - | Operation opposite to the Zoom In on the graphic area of the window: it displays in smaller detail in respect to both axis of the interval around the current position of the cursor. |
| Minimum Zoom | Alt + | Shows the graphic area in relation to the whole range. |
| Zoom Box |  | Enable the Zoom Box mode to select the part of the graphic area to be zoomed using a rectangle created with the mouse right button. |
| Pan |  | The Pan mode allows shifting the displayed zoom area using the mouse with left click button pressed. |
  
##### GE Calibrations

Fine calibration of the satellite map of Google Earth. When you activate the command, the calibration symbols appear on the window, which are used to move the track in the four directions admitted (North, South, West and East)

They also allow you to extend or shorten the trajectory, modifying the shape factor, always compared to the cardinal axes (North, South, West and East). This function proves useful to accurately define the position of the track compared to the satellite map. It therefore allows you to correct any GPS or acquisition errors of the satellite map with great precision (for example, when the position of the satellite is not perpendicular to the zone of the track). To apply the required calibration, simply use the command again. The calibration symbols disappears and the modification are applied to the track.

##### Select Lap Marker

This command appears only when Histogram Marker is selected and a compare of laps is loaded. The command permits to select the desired lap.

##### Track Sections

Display of the Track Sections as configured in Track Editor.

##### Split Time

Shows the value of the split time in each section, if sections are displayed.

##### Track Sections Report

If there are channels configured as section report, text boxes are added to the track to show the report in correspondence of the section.

##### Manual Section Definition

Track Sections may be manually defined at any time using the Manual Sections definition commands from the graph window. The command opens the [Track Sections Editor Toolbar](../../../Toolbars/ToolbarTrackSectionEditor.md), if it was not already open. In the graph, use the mouse to insert a series of section splits at points on a distance plot. Use the following shortcuts to support the manual sections definition:

End: shortcut to confirm manual splits

Esc: shortcut to abort operations

Manual map definition is available on the track window in both modes, standard and GPS.

##### 

##### Track channels

Displays the sub menu to manage track channels

  * Show track channels values: Shows track channel popup attached to the vehicle icon on window.

  * Capture track channels values: Places a fixed track channel popup in the current cursor position.

  * Clear track channels values: Removes all track channels popup.




##### Trajectory

It opens a menu from which you can select or de-select the trajectory of one or all the laps loaded. The trajectory shows the real path of the vehicle in the lap involved; it may not coincide with the track when this has been calculated differently (based on other data).

  * Dataset trajectories: The Dataset lines allow to show/hide a trajectory for the specified lap.

  * Real Time Trajectory: Show/Hide real time trajectory.

  * Hide all trajectories: Hide all trajectories.

  * Show all trajectories: Show all trajectories




##### Customization

Display the customization submenu. The menu is active only if computation mode of the track is standard. Not enabled for GPS.

  * Custom elements: shows/hide custom elements

  * Insert New TextBox: Adds a text item anywhere on the track window. In addition to the text, also the font and the color can be modified. The item can be placed anywhere in the window with the mouse.

  * Edit Custom Element: If an element of the custom track is selected, the corresponding configuration window opens to allow parameters modifications..

  * Remove Custom Element: Removes the selected custom element.

  * Remove All Custom Element: Removes all the custom elements from the window.




Esc key cancels custom operations

##### Ruler

Display the ruler submenu to calculate distance and GPS coordinate on the track window.

With Ruler operation active some command will be disabled e.g. Insert Points or Remove Points, in order to improve usability.

###### Distance between two points on the ground

This command opens a window which permits to measure the distance between two points without restrictions. The points can be everywhere on the ground. The points on the ground are selected with the mouse.

###### Distance between two points on the track

This command opens a window which permits to measure the distance between two points on the track. The selected points can be only on track. The points on the track are selected with the mouse. The window shows two values: The Linear Distance corresponds to the distance in a straight line between the points; the Track Distance is the distance traveled on the track. As shown in the picture below, the ruler shows the value for each trajectory when more than one lap is loaded.

#### 

#### 

#### Toolbar

The toolbar enables the access to the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Load |  | Opens a window to select a configuration file. |
| Save |  | Saves the present window configuration on a file. |
| Properties | E | Opens the interface to configure the window. |
| Zoom In |  | Zooms the graphic area with respect to both axis, displaying in better details the interval around the current position of the cursor. |
| Zoom Out |  | Operations opposite to the Zoom In on the graphic area of the window: it displays in smaller detail with respect to both axis of the interval around the current position of the cursor. |
| Zoom Box |  | Enable the Zoom Box mode to select the part of the graphic area to be zoomed using a rectangle created with the right button of the mouse. |
| Minimum Zoom |  | Shows the graphic area in relation to the whole range. |
| Pan |  | Enables Pan mode to move the zoom area viewed with the mouse. It is enabled only if the track has been zoomed. The Pan can be activated also by pressing Ctrl and clicking with the left button of the mouse on the track area. |
| Track |  | Show/Hide the track calculated in Track Editor. |
| GE Map |  | Display or hide Google Earth map. |
| GE Calibration |  | Fine calibration of Google Earth satellite map. |
| Clear GE Calibration |  | It removes the calibration function from the track, restoring the original position and dimensions. |
| Marker |  | Displays configured markers; When "Marker" is enabled, "track sections" becomes disabled. |
| Select Lap Marker |  | This command appears only when Marker Histogram is selected and a compare of laps is loaded. The command permits to select the desired lap. |
| Track Sections |  | Displays Track Sections as configured in Track Editor. |
| Split Time |  | Shows the value of the split time in each section, if sections are displayed. |
| Track Sections Report |  | Adds text boxes to the track to show the report in correspondence of the section. |
| Manual Section Definition | Alt + M | Starts Manual Section Definition operations |
| Track Channels |  | Displays the sub menu to manage track channels |
| Trajectory |  | Opens a menu from which you can select or de-select the trajectory of one or all the laps loaded. |
| Events | Ctrl + Alt + E | Enables the events window. |
| Custom Elements |  | Displays the customization submenu. |
| Ruler |  | Displays the Ruler submenu. |
| Track Editor | Alt \+ T | Opens the [Track Editor](../../../Tools/TrackEditor/Track_Editor.md) that allows creating or loading Track and Track Sections. |
| Stop Operations |  | Aborts the GE calibration operations. |
| Switch to Telemetry/Post Processing | CTRL \+ T | Allows switching from Post-Processing mode to Telemetry mode and vice versa. |
  
#### Pop-up menu

By clicking with the right button of the mouse on the graphic area of the window, a pop-up menu is displayed. Popup menu is built dynamically and can have many commands. In the following picture there is an example of popup.

This section will describe only the commands that have not already been described previously.

| COMMAND | DESCRIPTION |
| --- | --- |
| Print Window to Clipboard | Copy the image of the window in the current state on the clipboard. |
| Window | Opens the [Window submenu](../../../Menu/MenuWindow.md#MenuWindow_submenu) |
| Freehand Drawing | List of commands that are used to draw or write texts on the window. See [here](../../../Toolbars/ToolbarFreeHand.md) for a complete description of the commands. |
| Events | Opens the Events sub menu. |
  
#### 

##### Events

Opens the sub menu linked to the events:

  * Show Events - Shows or hides the events on the window

  * View All Events/View Custom Events - switches between the display mode of all the events and the display mode of only the events chosen

  * Remove Events - is available if the mode is custom events and allows removing either an event or all the events from the window




#### 

#### Keyboard Shortcut

To see the complete list of shortcuts available for the graph window, click [here](../../../Shortcut/WindowsShortcuts.md#ShortcutTrack).

Below are listed those shortcuts that do not appear in any of the commands and of the toolbar buttons described before. These commands can be executed only from keyboard. All these commands are not customizable.

| SHORTCUT | DESCRIPTION |
| --- | --- |
| Left | Move current vehicle position forward |
| Right | Move current vehicle position back |
| Esc | Cancel custom circuit operation |
|  |  |
  
##### 

##### Related Issues

  * [Track window](CRVWndOverview.md)

  * [Elements of the window](CRVWndItems.md)

  * [Functions](CRVWndFeatures.md)

  * [Configuration](CRVWndSetup.md)


