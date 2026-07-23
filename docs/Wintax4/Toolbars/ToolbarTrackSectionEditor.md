## Track Sections Editor Toolbar

The Track Sections Editor Toolbar is a toolbar that can manually configure the track sections without using the [Track Editor](../Tools/TrackEditor/GenerateTrackSections.md). It is a self-completing and editable table for the definition of the sections of the track, auto-completed when the map is created outside of [Track Editor](../Tools/TrackEditor/GenerateTrackSections.md). The table is similar to the one available in the Track Editor and they share the same information.

The Track Sections consists of a series of datum points corresponding to the junctions between the straights and corners of the race track. These datum points may be found manually with the mouse or keyboard input. Once the Track Sections has been defined it forms the basis for section-by-section comparisons of data recorded in different laps which give a clear picture of precisely where and why time is lost or gained. Once defined for a given circuit, the map may be saved for future use allowing comparisons between different sessions. 

The table in the toolbar permits to customize manually the sections of the track.

In the Sector table there are two main columns:

  * Sector:

  *     * Show: Shows if the corresponding section is visible or hidden. The effect of this check, visible on track and graph windows, depends on the selected Type in the Track Section Numeration area in the Track Editor. With Show All the check has no effects. With Show Straight the check works only on straight sections and with Show Corner works only on corner; with Show Custom works on all the sections. The selection of the checks is immediately reflected on the windows which shows sections.

    * Name: The custom name of the section, if configured. Please note that the name change is reflected immediately on the corresponding label of the track. Double click to edit and to change the default name.

    * Color: Each sector can be configured with an own color.

    * Type: The type of each sector can be straight or corner.

  * Distance (m): the start and the end point, in meters, of each sectors. The end point is always editable and there are controls that prevent you to enter values that are not consistent.




There are two command buttons:

New Sections...: Clicking on the button, WinTAX4 opens a Manual section session. Using the cursor the user can divide in sections a graph in the Graph window or a track in a Track window. As shown in the pictures below, the cursor has a tooltip that shows the progressive number of the next section (S1, S2, S3, ... Sn). If the Manual Sections is open, the button New Sections becomes Confirm Operation. Clicking on this button the new sections are confirmed and loaded in the whole WinTAX4. The shortcut of Confirm Operations is END.

Stop Operation: This button is active only if a Manual sections session is open and interrupts the actual session. With Stop Operations WinTAX4 reload the previous sections. The shortcut of this command is ESC.

On the top of the toolbar there are two others buttons. The first one (pin symbol button), is used to dock and to undock the toolbar. The second one (cross symbol button), is used to close the toolbar.