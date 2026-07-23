### Track Designer

Track Designer is a dedicated environment for editing the circuit, therefore can modify an existing circuit or create new ones. The editing of an existing circuit is done either by adding or deleting points or by moving the existing ones using the mouse or the arrow keys on the keyboard. In addition to the points of the circuit even the GE map is displayed in background if present. The points of the track are sensitive to the passage of the mouse.

#### Elements of the window

The window is composed by the following elements:

Menu

The menu is a list of commands to perform windows operations.

Toolbar

The toolbar contains a selection of the most frequently used commands.

Track Area

The track area is part of the window where a track is changed or created. The appearance is different according to the type of circuit in editing (Standard or GPS). In the window the user can identify the following information boxes:

  * Coordinate information




it is the box in the top left and contains the coordinates in degrees or in meters of the point where is located the mouse arrow.

The coordinates are in degrees in case of GPS track and represent the latitude and the longitude of the point. In case of Standard track the coordinates are in meters, starting from the finish line that is considered the origin of the track.

  * Points information




It is the box in the bottom left and contains the information about the selected point.

The Point Number is the sequence number starting to count from the finish line. The Distance from the start line are the meters that separate the current point from the finish line along the direction of travel. Point Latitude and Point Longitude are the latitude and the longitude of the selected point in GPS mode. Point X and Point Y are the coordinates of the point with respect to the finish line in Standard track.

  * Ruler




It is the box in the bottom right and contains the information about the map scale of the track.

  * Compass rose




It is the box in the top right and contains the north pointing compass rose when the track is GPS. Clicking on the compass arrow, the track is automatically rotated so the North is pointing to the top.

#### Editing an existing track.

Entering the Track Designer with a track that already exists, the window displays all the points of the track. The points are in green color. With the mouse the user can select one or more points and move them with the mouse or delete them with the appropriate command. The user can also insert a new point after the selected point using the appropriate command. The selected points are marked with red color. See Commands for further details.

###### Modifier Keys (ALT, Shift AND CTRL):

CTRL: To select a single point use the left click of the mouse on the desired point. To select more than one point press CTRL and select others points. Multiple selection can also be performed using the mouse left click while dragging and then moving the mouse to create a selection area. Repeating the clicking and selecting operation while the CTRL key is pressed allows selecting multiple group of points. SHIFT: Allows the selection of a range of points. It's possible to click on one point, and then while keeping the SHIFT key pressed it's possible to select another point and all the points between the first selected point and the last selected point will be part of the selection.

ALT: The ALT modifier key allows access to PAN mode of the map. While ALT is pressed, it's possible to click on the map and drag in any direction to scroll the map. It's also possible to get the same function clicking (if it is available) the central wheel of the mouse while dragging the map.

#### Create a new track

The track designer can create new circuits in GPS mode and Standard mode. In both cases, the points should be inserted, by clicking with the mouse, in a blank area.

##### Create a new Standard Track

This command opens the following window:

In this window the user must configure the track name and the size in meters of the coordinates width.

Name: Insert the name of the track

Area Width (m): This measure corresponds to a side of an hypothetical square within which the track is built.

OK: Confirm creation of the track.

Cancel: Abort creation operation.

After clicking OK, the graphic area will be blanked and the user can create the track using the mouse and the following commands: Insert Point from last point, Insert Point, Remove Points, Add Point from Interpolation Step, Fix Straight Line, Fix Straight Line Equidistant, Fix Curve, Select All, Clear Current Selection and Stop Operations.

The map obtained can be used as a normal map calculated from Track Editor.

##### Create a new GPS Track

This command opens the following window:

In the window user must configure the track name and the coordinate which define the track area. There are three way to do it:

Name: Insert the name of the track.

Define Area Manually: Select P1 as top left point and P2 as top right point that will be contained the map area. The coordinate points can be in Decimal Degree (e.g.. 44.530006) or in Sexagesimal Degree (e.g.. 44 31 48.03). It should be noted that a one degree of longitude at the equator measuring a little more than 11 km, the length of the degree decreases as you approach the poles. The average length of a degree of latitude is about 111 km and varies closer to the poles too. Be careful not to select a region too large otherwise behavior can be unpredictable.

Use current Google Earth Map: The option is active only if the current circuit has already configured a GE map. in this case the OK button erases all points of the current circuit but maintains the same GE map. 

Select new area with Google Earth: The command Browse Area opens the [Google Earth](Google_Earth.md) interface to select a region for building track. As for other modes, be careful not to select a region too large.

OK: Confirms the track creation.

Cancel: Aborts the track creation.

After clicking OK, the graphic area will be blanked (Define Area Manually) or filled with GE Map (Use current GE Map and Select new Area with GE) and the user can create the track using the mouse and the following commands: Insert Point, Insert point from last lap point, Remove Points, Add Point from Interpolation Step, Fix Straight Line, Fix Straight Line Equidistant, Fix Curve, Close Track, Insert finish line, Select All, Clear Current Selection and Stop Operations.

The map obtained can be used as a normal map calculated from Track Editor.

In the case of GPS circuits, to the left of the name of the track it is displayed the GPS label to distinguish GPS maps from standard ones.

#### Commands

In the menu there are all the commands available on the window. The toolbar is a selection of the menu commands; for this reason, this section describes only the menu commands.

##### Menu File 

The File menu contains the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Apply | Applies the current settings of the Track Designer and save the configurated track in the appropriate format. The command Apply overwrites the current track in the Track Editor. |
| Cancel | Closes the window without applying the current settings. If the user tries to go out with changes in progress, the window shows a warning message to ask data storage. |
  
##### Edit Menu

The Edit menu contains the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Create a new Standard Track |  | Creates a new track in Standard mode erasing the current track. See here for further details. |
| Create a new GPS Track |  | Creates a new track in GPS mode erasing the current track. See here for further details. |
| Insert Point | Insert | Adds an interpolated point in a clockwise direction, based on the last point inserted. (Please see description below for more information) |
| Insert Points from last lap point |  | When the command is activated, on every mouse click, the user will insert new points on the map. Operations will stop when user clicks on "Stop operations" or pushes the ESC key. |
| Remove Points | Delete | Removes the selected points.  During “Insert Points” operation, the command is disabled. |
| Add Points from Interpolation Step |  | Adds points based on interpolation step (meters) between two extreme points. User can enter the interpolation step in the relative pop-up windows. The command is active after you have selected at least two points (extreme points). |
| Fix Straight line |  | Moves the points of the trajectory on a straight line between the two extreme points. Some points can be removed if the displacement algorithm is not reflected. The command is active after you select at least two points. |
| Fix Straight line equidistant |  | Moves the points of the trajectory on a straight line equidistant between the two extreme points. The command is active after you select at least two points. |
| Fix Curve |  | Moves the trajectory points on a conic arc section using 5 points previously selected .The selection order of the five points is irrelevant but only the first five points selected are considered. The command is active after you select at least five basis points to create the arc of conic section. |
| Close Track |  | Close the path of the circuit.  The command is visible only for GPS circuit. |
| Insert finish line |  | Move the finish line at the selected point.  The command is visible only for GPS circuit. |
| Select All | Ctrl + A | Selects all the points in the area. |
| Clear current selection |  |  |
| Stop operations |  |  |
| Clear Current Selection |  | Removes the selections of the points. |
| Undo | CTRL + Z | Cancel last operation |
| Redo | CTRL + Y | Repeat the last undo operation |
| Stop Operations | Esc | Breaks the input mode and all Zoom Box and Pan operations. |
  
##### Insert Point

This function only works when one of the points on the map is selected. It is possible to activate this feature only using the right click menu item "Insert Point".

When right clicking on the map editor, the context menu shows an "Insert Point" menu item. When an existing point is selected on the map editor and the user clicks the Insert Point menu item, a new point is added between the selected point and the next point in a clockwise direction.

While the function is active, it is also possible to move the newly created point by selecting it and then left clicking and while holding, moving the point until it is in the desired position.

To quit the "Insert point" feature, it is possible to click on the context menu and choose "Insert Point" to disable the function or to click the corresponding icon in the menu bar.

##### View menu

The View menu contains the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Minimum Zoom |  | Shows the graphic area in relation to the whole range. |
| Zoom Box |  | Enable the Zoom Box mode to select the part of the graphic area to be zoomed using a rectangle created with the mouse right button. |
| Pan | ALT + Left click + Drag | Pan mode allows to shift the displayed zoom area dragging the map with the mouse while the left click button is pressed. |
| Zoom In |  | Zooms the graphic area with respect to both axis, displaying in better details the interval around the current position of the cursor. The Zoom In can also be performed with the mouse wheel. |
| Zoom Out |  | Operation opposite to the Zoom In on the graphic area of the window: it displays in smaller detail with respect to both axis of the interval around the current position of the cursor. The Zoom Out can also be performed with the mouse wheel. |
| Rotate -90 |  | Rotates the track of ninety degrees counterclockwise. |
| Rotate -1 | - | Rotates the track of ninety degrees clockwise. The Rotation can also be performed with the mouse wheel and pressing the CTRL key. |
| Rotate +1 | + | Rotates the track of one degree counterclockwise. The Rotation can also be performed with the mouse wheel and pressing the CTRL key. |
| Rotate +90 |  | Rotates the track of one degree clockwise. |
| Grid | G | Enables or disables the display of a grid in the window. See here for details. |
  
###### Grid

The Grid command opens the following submenu; the submenu depends on mode, Standard or GPS

In the first submenu, the Standard one, the user can switch off the grid or choose a distance value as step of the grid. The same in the GPS one, where the step is expressed in degrees fractions.

In the picture below there is a sample of a grid with step of 1/5000 degrees.

The GPS tracks display a solid red line corresponding to the grid lines with integer value degree (e.g.. 44, 45, 58, ...), and a blue dashed line corresponding to the grid lines at 1/60 of a degree.

The Standard tracks display two solid red lines, one vertical and one horizontal, corresponding to the finish line point of the track.

##### Tools menu

The Tools menu contains the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Ruler | Opens a tool for measuring distances. See below for details. |
  
###### Ruler

The Ruler command opens the following submenu

Distance between two points on the map

This command opens a window which permits to measure the distance between two points without restrictions. The points can be everywhere on the map as shown in the picture below. The points on the map are selected with the mouse.

Distance between two points on the track

This command opens a window which permits to measure the distance between two points on the track. The selected points can be only on track as shown in the picture below. The points on the track are selected with the mouse. The window shows two values: The "Linear Distance" corresponds to the distance in a straight line between the points; the Track Distance is the distance traveled on the track.

###### 

##### Help menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
##### Related Issues

  * [Track Editor](Track_Editor.md)

  * [Commands](Commands.md)

  * [Elements of the window](Elements.md)

  * [Track Setup](TrackSetup.md)

  * [Generate Track Section](GenerateTrackSections.md)

  * [Google Earth](Google_Earth.md)


