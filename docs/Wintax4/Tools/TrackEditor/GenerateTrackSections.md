### Generate Track Sections

The circuit Track Sections and its related functions are an essential feature of driver and vehicle performance analysis on a race track. The Track Sections consists of a series of data points corresponding to the junctions between the straights and corners of the race track. This data points may be found automatically using either one of the two simple algorithms or manually with the mouse or keyboard input. Once the Track Sections has been defined it forms the basis for section-by-section comparisons of data recorded in different laps which gives a clear picture of precisely where and why time is lost or gained. Once defined for a given circuit, the map may be saved for future use allowing comparisons between different sessions. There are a few ways to generate track sections.

#### Track Sections Table

The Track Sections table permits to customize manually the sections of the track.

In the Sector table there are three main columns:

  * Sector:

  *     * Show: This checkbox shows and hides the corresponding section of track and graph preview windows. The effect of this check depends on the selected Type in the Track Section Numeration area. When "Show All" is enabled the "Show" check near each section is disabled. With Show Straight the check is enabled only on straight sections and with Show Corner only on corners; with Show Custom is enabled and can be changed on all the sections. 

    * Name: Displays the custom name of the section. Please note that when you modify the Track Sections Numeration setting (Show All, Straight, Corner, Custom), all the names of the sections will be erased and recalculated.

    * Color: Each sector can be configured with a color.

    * Type: The type of each sector can be Straight or Corner.

  * Distance (m): the start and the end point, in meters, of each sector. The end point is always editable and there are controls that prevent you to enter values that are not consistent.

  * Time (s): Time, in seconds, taken to travel each sector.




  * Shift All Track Section (m): allows to increase the length of all sections, except the last one, with a constant length using the Apply button. The total distance of the track will not change because the increase in the length of the sections is absorbed from the last section. The command is useful, for example, when moving beacon transmitter position. This value is expressed in meters.




#### Track Sections Numeration

WinTAX4 automatically names the sections defined either in manual or in auto mode generation. The automatic numeration is always intended as follows:

The Type of each sector is alternatively straight and corner. The default Name of each sector is: 1, 2, 3, ..., N for corners, 1-2, 2-3, ..., N - 0 for straights. The default color of the sectors is: red for corners, green for straights, black for not covered areas. Four predefined display filters are available

  * Show All: all sections are, by default, shown in each windows which supports the Track Sections object.

  * Show Straight: filter on straights type.

  * Show Corner: filter on straights type.

  * Show Custom: custom visualization. With this option, the User can change both the name of the sector and its type.




The chosen filter can be locally modified in each window.

Numeration mode: selects if the sectors are numerated sequentially (1,2,3,4,5,...) or distinguishing straights and corners (1, 1-2, 2, 2-3, 3, ...).

First Sector Type: selects if the first section is Straight or Corner.

#### Track Orientation

With this command it is possible to rotate in a different way the circuit, by setting a variable angle from 0 to 360 degrees.

#### Move Finish Line (m)

Move the finish line at any point of the circuit. The effect is permanent and will be seen on the track windows.

The function allows to move the finish line with the mouse and via edit box. The edit box allows to make a precise tuning of the position (shifting of points of the circuit). When you change values with edit box or with the slider, the sections are shifted accordingly even if their length remains unchanged. The values of the sections do not change, but their graphics are updated on the map with the new location of the finish line.

#### 

#### Auto Generate Track Sections

This window is used to specify the channels and their related thresholds which are used to define the Track Sections sections automatically based on either throttle position or lateral acceleration or No-Rx zones or conditions changes.

To save the trajectory, which is displayed on preview area, use the menu: File/Save Track As.

##### Auto Generate Track Sections by Lateral Acceleration

  * Lateral acceleration (g): Lateral acceleration channel

  * Threshold: Acceleration threshold which defines the start and end of a corner.

  * Min Time (s): Minimum allowed section duration in seconds. This parameter is used to filter out spikes in the acceleration channel which would result in spurious Track Sections divisions.

  * Auto by Lateral Acceleration: Calculates the sections and closes the window.




Track Sections splits using lateral acceleration

### 

##### Auto Generate Track Sections by Throttle

  * Throttle: Throttle channel

  * IN Limit: Throttle position used to define the start of a corner.

  * OUT Limit: Throttle position used to define the end of a corner.

  * Min Time (s): Minimum allowed section duration. This parameter is used to filter out spikes in the throttle channel which would result in spurious map divisions.

  * Auto by Throttle: Calculates the sections and closes the window.




##### 

##### Auto Generate Track Sections by No-Rx

Min Time (s): Minimum allowed section duration. This parameter is used to filter out spikes in the no-rx zones which would result in spurious Track Sections divisions.

Auto by No-Rx: Calculates the sections and closes the window.

##### 

##### Auto Generate Track Sections by condition

Min Time (s): Minimum allowed section duration. This parameter is used to filter out spikes in the no-rx zones which would result in spurious Track Sections divisions.

Auto by No-Rx: Calculates the sections and closes the window.

##### Manual Generate Track Sections

Track Sections may also be manually defined at any time using the New Sections command from the preview graph window or from the break points table with add/insert commands. When creating a new map the current break points table, if exists, is automatically emptied. Even the map displayed in the window is cleared.

To choose the preview window, use the combo at the top of the page. In the preview Graph or Track windows, use the mouse to insert a series of section splits at points on a distance plot. It is possible to select points in both windows. During the creation of new sections, the section table is updated immediately. To confirm the manual splits press the END key or click on Confirm Operation button after creating one or more divisions. Use ESC or click on Cancel Manual Sections to abort operations. 

On the track window preview use the mouse to select sections, to move the borders or the finish line.

In the section table the user can configure section using the commands show in the picture below.

The available commands are:

  * Add Section (+): add one section always in the last position

  * Insert Section (Insert): split in two parts the selected section

  * Remove Section (Delete): delete the selected section

  * Clear Track Section: remove all sections

  * Shift All Track Section: allows to add an offset in meters to all sections, useful for example when moving beacon transmitter position.




The length of every field can be graphically modified working directly on the trajectory through the mouse (see picture below). Moreover in the preview circuit window, it is possible to move the finish line by dragging the green arrow to the desired position.

To save the Track Sections, use the menu: File/Save Track Sections As..

##### Related Issues

  * [Track Editor](Track_Editor.md)

  * [Commands](Commands.md)

  * [Elements of the window](Elements.md)

  * [Track Setup](TrackSetup.md)

  * [Track Designer](TrackDesigner.md)

  * [Google Earth](Google_Earth.md)


