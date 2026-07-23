### Functions

The [Track window](CRVWndOverview.md) has the following functions:

  * Connect Cursor

  * Zoom

  * Marker display

  * Marker compare

  * Track Sections display

  * Track Sections Report display

  * Customization

  * Track Channel Values

  * Integrate Google Earth Maps

  * Ruler




####  Connect Cursor

If Auto Connect check is enabled (in General options - Setup/General) the track scheme will be "connected" on the graphic window and viceversa. In fact when the cursor is moved on the [Graph](../GRA/GRAWndOverview.md) window, on the Track window the icon identifying the vehicle on the track will move accordingly. In the same way, by moving the vehicle with the mouse (or with the left and right arrows) the cursor on the other windows will move accordingly to the instant of time associated with the position of the vehicle on the circuit. This function that links time and space to work correctly needs the channel distance in the "Setup/Special Channels" to be properly configured.

The window is structured in a way that in case of zooming the connections between cursor and vehicle are always displayed indicating the vehicle in the middle of the window, this does not happen when the track is completely contained in the window.

In the case of append laps, the link between the vehicle on Track window and cursor position on graph window refers to the current lap.

In case of laps comparison, a vehicle for each lap compared is displayed on the track .

####  Zoom

The zoom on window track does not affect the zoom of the other windows. The are a few ways to zoom an area of the tracks : a) select a rectangle by using the right button of the mouse b) using the commands Zoom in menu or toolbar. When zooming, the window displays the vehicle in the middle as soon as a Connect Cursor situation occurs. When zooming, the user can go back to the initial situation through the [Minimum Zoom](CRVWndCommands.md#CRVToolbar_MinimumZoom) command. When zooming, the [Panning](CRVWndCommands.md#CRVToolbar_Pan) function can be used.

Instead, the zoom on the graph window affects the track window; in fact, when there is an active zoom on the graph windows, the track window shows the part of the circuit that is concerned with the zoom. If an append of laps is loaded and if the zoom includes two contiguous lap, the track is highlighted throughout the affected area unless the track overlaps.

####  Marker display

The Marker display shows as a channel is varying along the track using a colored representation. You can choose to display one of the markers available from the configuration in the [markers section](CRVWndSetup.md#CRVWndSetupPgMarkers). The markers display on the track can be enabled or disabled through the [Markers](CRVWndCommands.md#CRVMenuOptions_Marker) command, it overtops the Track Sections display if both are enabled.

In the picture there are two channels configured as Marker, SPEED and GEAR. Each channel includes an info chart reporting the ranges, the color associated and, if configured, the times or the percentage in which the channel has the values included in a certain range. The ranges are indicated on the track. In almost all corners for instance the magenta range appears, this indicates a SPEED value from 125 and 166.7.

In the picture above there are two Markers. By default the first Marker is displayed, the second one is hidden; in the picture the first one is hidden. There can however be some overlapping markers if the configured ranges do not cover the whole track, therefore combinations of different markers might occur. The gradation mode is useful in these cases to better identify the marker. In the picture the color mode is Manual.

The display of the marker can be disabled by clicking on the name in the ranges table. By clicking once again the display is enabled. In this way the Markers that are in a lower position in the arrangement can be display. For each marker the range can moreover be switched on/off by simply clicking on the corresponding colored square in the ranges table.

On the top of the channel list, there is a combo that permits to choose which markers show on the track.

In the picture below there is a Marker channel in Smooth mode.

Marker Histogram has the same function of Marker, except for the visualization, as shown in picture.

####  Marker Compare

If a comparison is turned on and the marker channel is displayed, the track window will show two colored tracks, side by side. The two tracks will show independent cursors indicating the relative position of the vehicles along the track and relevant values for marker channel.

Annotations for Run and Lap numbers are displayed with a line joining them to the colored trace.

Up to two laps in compare are supported. Use command “Select lap marker” to change overlapped laps

#### Track Sections display

This is a display mode that allows showing the Track Sections according to the options configured in the [Track Sections section](CRVWndSetup.md#CRVWndSetupPgSectionRep). The display of the Track Sections on the track can be enabled or disabled through the [Track Sections command](CRVWndCommands.md#CRVMenuOptions_TrackSections), the display is hidden if at the same time the Markers display is enabled.

The displayed sections are characterized by the color of the section and by the identification label. The labels can be moved in the favorite position by clicking on them and dragging them with the mouse. As an alternative the [Automatic Placement](CRVWndCommands.md#CRVMenuOptions_AutoPlacement) command can be selected to place the labels in a position calculated by WinTAX4. The labels are visible even if Track Section Reports is enabled.

#### Track Sections Report display

The display of the track is the same as the Track Sections display because the Track Sections are always highlighted and it has a priority always lower than the Markers display What changes are the highlighted labels that do not contain information on the Track Section, but contain the channel values of the Track Section, calculated according to a certain statistics.

Down on the left there is a small legend with the name of the channel, the statistics calculated and the icon associated to the statistics. This legend is useful to identify the various labels in the window. The labels can be moved in the favorite position by simply clicking on them and dragging them with the mouse. As an alternative the [Automatic Placement](CRVWndCommands.md#CRVMenuOptions_AutoPlacement) command can be selected to place the labels in a position calculated by WinTAX4. The labels are visible even if the Markers display and / or the Track Section display are enabled.

#### Customization

Customization allows adding text boxes, by setting markers and by dividing them into sections. it is enabled only when track is calculated in Standard mode (Speed/Acceleration), without GPS channels.

Selecting the command Insert New Text Box (Options / Customization / Insert New TextBox) appears the following dialog which permits to write a text and configure font and color.

In the picture below Custom Label 1, Custom Label 2 and Custom Label 3 are added with command Insert New Text Box.

####  Track Channel Values

The Track Channel Values mode allows showing, by popup, the channel values in the current cursor position and allows showing the values at the fixed desired points. There are no limits to the number of popup that can be opened. Every fixed popup can be removed by a mouse click on the small cross.

#### Integrate Google Earth maps

The track can be integrated with the satellite maps of Google Earth. To do so, you must have an active Internet connection to be able to access the Google archive. The chapter entitled Track Editor provides instructions on how to access such maps.

The GE map can be hidden using the GE Map button on the toolbar or in the Options menu. When the GE map is visible, you can zoom and turn it in the same way as when the map is not visible. The automatic scale function, which resizes the track so that it is always within the limits of the window, is not enabled when the GE map is enabled. For further information, see [Google Earth](../../../Tools/TrackEditor/Google_Earth.md).

#### Calculating distance between two point.

On the track it is possible to measure distances between two points on a circuit or on the ground using a tool called [Ruler](CRVWndCommands.md#CRVMenuOptions_Ruler). it is enabled only when track is calculated with GPS channels. 

##### Related Issues

  * [Track window](CRVWndOverview.md)

  * [Elements of the window](CRVWndItems.md)

  * [Configuration](CRVWndSetup.md)

  * [Commands](CRVWndCommands.md)


