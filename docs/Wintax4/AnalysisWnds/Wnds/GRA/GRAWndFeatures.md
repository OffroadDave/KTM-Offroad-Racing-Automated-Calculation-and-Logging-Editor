### Functions

The [Graph](GRAWndOverview.md) window has the following functions.

  * Cursor

  * Step

  * Snap

  * Datum

  * Graph Cursor

  * Graph Cursor Values

  * Popup Legend

  * Max & Min

  * Max & Min Enhanced

  * Show absolute Max & Min on Graph

  * Show Min Max Peak to Peak

  * Graph Information

  * Layout

  * Display of the elements

  * Zoom

  * Rectangles selection in the graphic area

  * Channels selection

  * Shift of the channels graphs

  * Hide/Show Graphs

  * X Scale Mode

  * Multicolor Channel

  * Graph Areas

  * Variance

  * Reference Line

  * Autoscale Groups of Channels




The following functions are available only in Real Time mode

  * Post Processing View

  * Get Manual Range

  * Real time Comparison

  * Real time Analysis freeze




The [Graph](GRAWndOverview.md) windows has also a series of functions that can be performed interacting with the other windows displayed:

  * Auto Connect: the position of the cursor is automatically up-dated in all the displayed windows whenever the position of the cursor is in the current graphic window. Some windows are always connected, while others are connected only if in [Setup/General](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgGeneralOptions) the auto connect cursor flag is set. To connect two Graphs, this option must be set.

  * Auto Connect Zoom: the position of the zoom is automatically up-dated in all the displayed windows whenever the position of the zoom in the current graphic window. The zoom is connected only if in [Setup/General](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgGeneralOptions) the auto connect zoom flag is set.




#### Cursor

The cursor is the vertical line displayed in the graphic area that enables to scroll all values in the range of the X-axis, updating the corresponding values of the channels in the info boxes.

The cursor can be moved along the X-axis in one of following ways.

  * Moving the mouse in the graphic area while the left button is pressed.

  * through the arrow keys of the keyboard (right and left arrow key).

  * Manually setting the value of the X-axis in the X-axis window that is viewed by pressing the INS key of the keyboard.




In Real Time the cursor can be used only if the acquisition is stopped by freeze command.

#### Step

The Step setting allows configuring the fixed distance at which the cursor can move, when using the keyboard. The Step value can be set using the commands in Options menu and Graph toolbar and choosing from these values:

In Real Time the Step is valid only if the acquisition is stopped by freeze command.

#### Snap

The snap allows modifying the fixed distance at which the cursor can move, when the mouse is used.

To set the snap value select the snap button of the Toolbar or the command in Options menu and select a value from the list. Selecting OFF the function is disabled.

Selecting AUTO the value corresponds to the highest sampling frequency of all channels configured in the window.

In Real Time the Step is valid only if the acquisition is stopped by freeze command.

#### Datum

The Datum cursor is used as a reference to calculate the information displayed in the Pop-Up Legend window. When Set Datum is active, the cursor identifies a grayed area whose ends are used in Legend window.

In Real Time the datum is available only if the acquisition is stopped by freeze command.

The function, in a frozen Post Processing and Real Time situation, is enabled only when the Pop-Up Legend window is displayed.

#### Graph Cursor

The Graph Cursor can change the display of the cursor. The cursor has the following states:

  1. Vertical (default): the window displays a vertical cursor.

  2. Horizontal: the window displays a vertical cursor and a horizontal cursor for each channel.

  3. Intersection: the window displays a cross cursor for each channel in the window and a vertical cursor to connect all cross cursors.

  4. Multiple Cross: the window displays a cross cursor for each channel in the window.

  5. Single Cross: the window displays a cross cursor for the selected channel.




In Real Time the Graph Cursor can change only if the acquisition is stopped by freeze command; in this case the available cursors are Vertical, Single Cross and Multiple Cross.

#### Graph Cursor

The Graph windows can show the values of the channels in several mode: Left, Right, Transparent and Follow Graph Cursor. With Left the Cursor Values are on the left side of the window and with right on the right side. Transparent means that Cursor Values are visible inside the graph area, in the left side, with a transparent effect. In Follow Graph Cursor mode the values are visible as Transparent but they are always attached to the vertical cursor (see the picture below).

Transparent and Follow modes are available only in Post Processing. 

#### Popup Legend

The [Popup Legend](../../../WTXWorkWindows/WndPopupLegend/PopUp_Legend.md) function displays in a popup window the values of the channels available on the graph in the moment corresponding to the cursor position; the difference between the values and the values available at Datum can also be displayed.

The function can be enabled through the Options menu, or the popup menus displayed by clicking with the mouse on the graphic area or on the channels Info area.

The Popup Legend is not active in Real Time, not even if the acquisition is stopped by freeze command.

#### Max & Min

A window is opened to display the maximum and minimum values and other values of the channel available on the graph in the time range taken into account.

The window takes into account the time and space intervals in the Time and Distance basic cases.

In Real Time mode the Max & Min window displays the maximum and minimum values of the portion of channels visible in the window. If a zoom is active, the Max & Min refers only to the zooming area.

The right button on the Max & Min window enables the following commands

  * Print Prints the table of the maximum and minimum values

  * Export Exports the values in txt format

  * Copy Data to Clipboard Copies to the clipboard of Windows the values of the window

  * Open in Excel Opens an Excel file containing the Max & Min table

  * Go To: X value This command appears only if the right click is on columns Min X and Max X (underlined values) and move the cursor values at the corresponding time. It is also possible to left click the cells to obtain the same effect.




On the Max & Min table the following values are displayed

  * Time Start (Distance Start) It is the lower limit of the time (space) interval taken into account. In Post Processing the time value is displayed as minute, seconds and milliseconds even if the time exceeds one hour. In Real - Time the time value is displayed as seconds.

  * Time End (Distance End) It is the upper limit of the time (space) interval taken into account. In Post Processing the time value is displayed as minute, seconds and milliseconds even if the time exceeds one hour. In Real - Time the time value is displayed as seconds.

  * Name Name of the channel.

  * Min Minimum value of X in the interval taken into account.

  * Min X Time instant when X reaches its minimum value in the interval taken into account. The time value is displayed as minute, seconds and milliseconds even if the time exceeds one hour.

  * Max Maximum value of X in the interval taken into account.

  * Max X Time instant when X reaches its maximum value in the interval taken into account. The time value is displayed as minute, seconds and milliseconds even if the time exceeds one hour.

  * Avg Average values of the channel in the interval taken into account.

  * Avg (abs) Average of the absolute values of the channel in the interval taken into account.

  * StdDev Standard Deviation of the channel.

  * Resolution Minimum difference between the consecutive values of the channel samples.

  * Samples Number of the samples valid in the interval taken into account.




## 

#### Max & Min Enhanced

In some licenses, the Max & Min Enhanced is enabled. There are two more features than the standard version, the buttons row and the [Max Min Setup](../../../WTXSetup/MaxMinSetup/MaxMinSetup.md).

##### Buttons row

The buttons row on the top of the window has three commands: Selected, Lap, Loaded.

  * Selected: When the Selected button is pressed, the window has the same function as standard. When the windows has active zoom, the maximum and minimum values are calculated on active zoom.




There's also the special case where the window is accessed via right click during a zoom drag operation if Selected mode is active. 

In this case the Graph window does not change the current zoom but the Max & Min Enhanced will open on selected zoom. The Max & Min Enhanced cancels this value when on the window ​​other zooming operation will be made or when the window will be closed.

  * Lap: When the Lap button is pressed, the window shows maximum and minimum values on the entire lap (or append), even if there is an active zoom. If there is no zoom active, the window has the same features as Selected mode.

  * Loaded: If there are no laps appended, the window has the same features as Lap Button. In the case of append laps, the maximum and minimum values are calculated on the entire lap identified from cursor position.




##### Commands

The right button on the Max & Min window enables the following commands

  * Print Prints the table of the maximum and minimum values

  * Print Window to Clipboard: Copies the image of the current window in the Windows Clipboard.

  * Export to ASCII Exports the values in txt format

  * Copy Data to Clipboard Copies to the clipboard of Windows the values of the window

  * Open in Excel Opens an Excel file containing the Max & Min table

  * Properties: Opens the [Max Min Setup](../../../WTXSetup/MaxMinSetup/MaxMinSetup.md).

  * Go To: X value This command appears only if the right click is on columns Min X and Max X (underlined values) and move the cursor values at the corresponding time. It also be possible to left click the cells to obtain the same effect.




#### Show absolute Max & Min on Graph

This feature displays minimum and maximum values on the graph windows. See the picture below.

This function can be reached through the Options menu, Toolbar Graph, shortcut or through the pop-up menu displayed by clicking with the mouse on the graphic area. 

The function is not active in Real Time, not even if the acquisition is stopped by freeze command.

  1. To enable the function, the user needs to click on each Graph Window, to enable one of the calculation modes of the Min Max absolute range:



  * Selected

  * Lap

  * Loaded




#### Show Min Max Peak to Peak

The command “Show Min / Max peak to peak”, allows the visualization of the local minimum and maximum values for each channel graph of a GRAPH window.

To enable the command, the user needs to configure 2 settings:

  1. Right clicking on each Graph Window, the user needs to enable one of the calculation modes of the Min Max absolute range:



  * Selected: when the Selected mode is enabled, the maximum and minimum values are calculated on active zoom

  * Lap: when Lap mode is enabled, the window shows the maximum and minimum values of the entire lap (or append), even if there is an active zoom. With no zoom, visualized data is the same as if Selected mode is active.

  * Loaded: If there are no laps appended, visualized data is the same as when Lap Button is enabled. In the case of append laps, the maximum and minimum values are calculated on the entire lap identified from the cursor position.



  2. On each channel, the user needs to enable the setting “Show” in the “Peak to Peak” section. This setting by default is disabled.



On each single channel, it is also required to configure:

  * Threshhold percentage (Tolerance Y (%V))

  * Filter X (in seconds)




The X filter needs to be configured by the user, for each channel in every window and its value is represented in seconds. The default value is 0.02 seconds. The Y threshold is calculated based on the value configured by the user and is represented as a percentage. The default value is 10%.

This settings are saved automatically with the window when the configuration is confirmed. 

It is also possible to configure the default settings for a channel in the [Channel Parameters](../../../WTXSetup/WndChannelsParametersSetup/Channels_Parameters.md) screen. When adding a channel on any window, there will be no need to re-configure again the settings as they will appear with the default settings configured in channel parameters if it has been configured. 

##### Conditions for Peak detection:

Peaks are detected between potential minimums and maximums local values that follow the threshold conditions related to the changes of value and frequency filter on the time configured for the channel.

The meaning of “potential local minimum and maximums” is related to the points in which the “theoretical” derivative first has a null value: (Points where there is an inversion of the trend in the historical graph history, e.g.: For a set minimum, the preceding points have a decreasing trend and the following points have an increasing trend.

To appear as minimal “Peak”, the following condition must be respected:

  * In the time interval (X Axis), preceding and following the point, equal to the threshold of the configured X Filter.

  * The point must have the lowest value.

  * The tolerance threshold value (Y threshold) should not be exceeded between the point and other local maximum pontential points.



  * Outside of the time intervals (X axis), preceding and following the point, equal to the threshold of the configured X Filter

  * The tolerance value threshold must be exceeded on the values, between the point itself and any other point, not necessarily a maximum local point.




Likewise, for maximum peaks the following conditions must be respected:

  * In the time interval (X Axis), preceding and following the point, equal to the threshold of the configured X Filter.

  * The point must have the highest value.

  * The tolerance threshold value (Y threshold) should not be exceeded between the point and other local minimum pontential points. 

  * Outside of the time intervals (X axis), preceding and following the point, equal to the threshold of the configured X Filter

  * The tolerance value threshold must be exceeded on the values, between the point itself and any other point, not necessarily a minimum local point.




This percentage is applied to a Min-Max range, obtaining a Y comparison threshold.

The minimum - maximum range edges are calculated based on the 3 data selection modes for each Dataset in the compare:

  * Selected: Minimum and maximum absolutes of the current zoom level.

  * Lap: Minimum and maximum absolutes in the currently loaded lap.

  * Loaded: minimum and maximum absolutes between all loaded laps in Append, currently loaded for the compared Dataset.




The sequence of the peaks is calculated from the first sample available of all currently graphed, proceeding forward, with a mandatory alternation of minimum, maximum. (or maximum, minimum)

A maximum number of peaks for channel which is equal to 256.

When the maximum number of peaks has been reached, a label is visualized on the graph, near the last detected peak.

Note: In case the maximum number of peaks has been reached, it is suggested to increase the values configured for X filter and Y threshold.

Example: channels that have high frequency noises.

Alternatively, it is possible to modify the zoom level, to increase the detail level on the X axis, coherently with the X filter value.

For each channel it is suggested to apply a filter. (es. “FFT” or “Run Average”) to reduce noises on the signal and obtain better results from the function.

####  Graph Information

The Graph Information window displays in a window some information about the channel.

This window can be reached with the View Information command through the Options menu, the Toolbar Graph, or through the pop-up menu displayed by clicking with the mouse on the graphic area.

In Real Time the View Information is available only if the acquisition is stopped by freeze command.

For each channel the following information are displayed

  * T1(D1) T2(D2) Outer limits of the time (distance) interval taken into account

  * Name Name of the channel

  * Color Color of the graphs

  * Min Box Minimum value of the channel in the interval taken into account

  * Max Box Maximum value of the channel in the interval taken into account

  * Offset Offset of the obtained value according to the configuration set in [Setup/General](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgGeneralOptions)

  * Gain Gain of the channel obtained according to the configuration set in [Setup/General](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgGeneralOptions)

  * Shift (seconds/meters) Shift value of the graphs, in seconds or in meters according to the Time or Distance values of the X-axis




#### Layout

The Layout function enables to change the aspect of the graphic window and especially the vertical arrangement of the graphs and of the Y scales. The function is Available for each area in Post Processing and Real Time.

It can be enabled through the Graph Layout command of the Options menu, of the pop-up menu and of the button on the toolbar, selecting one of the suggested modes:

##### Parallel

The vertical space is divided for the number of channels configured; the graphs and the scales of the channels are vertically arranged without overlapping

##### Overlay

The configured channels share the vertical space; the Y scales are horizontally arranged side by side, the graphs overlay.

#####  Manual

The graphs and the Y scale corresponding to the channels are vertically arranged based on the settings of the user in the Top and Bottom fields that can be changed through the configuration window in the Graphs section.

To quickly configure the graphs, just select with the mouse the channel to highlight the selection squares allowing to drag the graph along the vertical axis or allowing to enlarge or reduce the space covered.

If more than one area is configured, it is possible to configure the layout for each area. 

#### 

#### Display of the elements

The items of the window can be displayed or hidden by enabling the View command on the Options menu or on the pop-up menu. Between Post Processing and Real Time there are some differences about the objects that will be displayed.

#### Zoom

The zoom enlarges the graphic area displaying in higher or smaller details the channels graphs. Zoom function is available in Post Processing and in Real time only if the acquisition is stopped by freeze command.

Two type of zoomings are available:

  * Horizontal Zoom: refers to an interval of the X-axis.

  * Zoom Box: corresponds to a rectangle of the graphic area.




There are different ways to zoom and to move the area displayed in details

  * manual selection of a zoom rectangle in the graphic area.




Select a rectangle of the graphic area with the mouse or the keyboard (function: Selection of rectangles in the graphic area ).

Select Zoom in the Pop-up menu displayed by clicking with the right button of the mouse or by pressing SHIFT key as indicated in the figure.

The square appearing in the figure was obtained by keeping CTRL pressed during the selection; if CTRL is not pressed the rectangle zooms only the X axis, leaving the Y axis unchanged.

  * Zoom In, Zoom Out, Zoom Min, Zoom Max commands (menu, toolbar, pop-up menu, [mouse wheel + CTRL](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#Invert_mouse_Wheel_direction)).




The Zoom In and Zoom Out, Zoom Min, Zoom Max commands can be enabled through the Options menu, the Toolbar Graph, the pop-up menu of the Graphs window. Zoom In and Zoom Out can be carried out also with the mouse scrolling the wheel while pressing the CTRL key.

  * Zoom In: Zooms horizontally the graphic area in relation to the X-axis; the interval around the current position of the cursor is also displayed.

  * Zoom Out: Opposite operation compared to Zoom In, the graphic area is displayed in lower detail with reference to the interval of the X-axis around the current position of the cursor.

  * Zoom Min: Zooms the graphic area in relation to the X-axis, in maximum details, with reference to the current position of the cursor.

  * Zoom Max: The graphic area is displayed in comparison with the whole extent of the X-axis; it corresponds to the minimum details (100%) and it is the default setting when the graphic windows are opened to be displayed.



  * Zoom Fix Range: The "Zoom Fix Range" can be enabled through the Toolbar Graph and the pop-up menu zooms horizontally the graphic area, displaying the interval around the current position of the cursor for a fixed interval whose value set in the Zoom Fix field (Time to Distance according to the mode of the X scale) of the [General Options](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgGeneralOptions) of the [General Setup](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md) window.



  * Zoom T1-T2 This function allows zooming horizontally the graphic area indicating directly the outer limits of the reference interval of the X-axis. Enable the T1-T2 mode using the command on the Toolbar Graph. When the Zoom T1-T2 mode is active, the cursor of the mouse is displayed as a hand lens. In this situation, it is possible to select the zoom outer limits with the mouse, by clicking with the left button on the graphic area or manually setting the T1 and T2 values in the X-Axis window that can be displayed by pressing the INS key. To exit from the T1-T2 mode without zooming, press the ESC key.



  * Zoom Undo, Zoom Redo When more zoom operations are carried out in sequence, it is possible to shift in the display sequence through the ZoomUndo and ZoomRedo commands available in the Toolbar Graph. The ZoomUndo icon displays in a pop-up menu the zoom operations carried out before the one in use while the ZoomRedo displays those coming after (if a previous zoom has been selected). In particular, see the following example.




The list in the pop-up menu shows that the following operations were carried out: Zoom In, shift of the zoom area, Zoom Fix, Zoom In and Zoom Out. Selecting the first "Zoom In X [11, 64]", the initial situation of the window (zoom at 100%) is displayed.

To shift the zooming area keeping the same level of detail, proceed as follows:

  * zoom bars




drag the zoom bars with the mouse, pressing the left button

  * arrow keys of the keyboard




use the arrow key of the keyboard, pressing the CTRL key

  * mouse




use the mouse wheel (for horizontal scroll)

  * Pan




The Pan mode allows shifting the displayed zoom area using the mouse or the keyboard.

Enable the Pan mode through the command available in the Toolbar Graph or in the pop-up menu. The cursor of the mouse is displayed as hand-shaped when it is moved in the graphic area of the window.

To shift the displayed zoom area, move the mouse pressing the left button, or use the arrow keys pressing the Ctrl key.

To exit the Pan mode, just release the left button of the mouse or press Esc.

## 

#### Rectangles selection in the graphic area

To select rectangles in the graphic area, proceed as follows:

  * use the arrow key of the keyboard, (Right Arrow, Left Arrow) pressing the Shift key; the selection corresponds to an interval on the X-axis

  * drag the mouse while pressing the right button and the Ctrl key




During this operation, the selected rectangle is highlighted and in a pop-up window, the information about the interval of the X-axis and about the zoom percentage on the Y-axis of the rectangle is displayed.

## 

#### Channel Selection

A channel can be selected (i.e. the Y scale, the graphs and the info boxes of a channel) by clicking with the left button of the mouse on the Y scale or on the info boxes of the channel. The channel is selected also clicking on the area of the transparent cursors. To multiple select channels, select the channels keeping the CTRL key pressed.

As an alternative select in sequence just one channel using the TAB.

On the Y scale, the graphs and the info boxes of the selected channels are highlighted.

To deselect a channel, click with the left button of the mouse on the Y scale or on the info boxes or deselect the Clear Selection command of the Options menu or of the pop-up menu.

Right clicking with the mouse on the Y scale or on the info boxes of the Info Channels area, the channel is automatically selected; if the channel has not been selected, other already selected channels are deselected.

The selected channel has two display modes that can be configured in [Setup/General.](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgGeneralOptions) In the highlight combo, two modes can be chosen: Bold that shows the channel in bold or Blink wich shows a blinking channel.

## 

#### Shift of the channel graphs

The Shift is an operation that allows translating one or all channels configured in the Graph window, along the horizontal axis distances (base time or base distance). The shift operation can act on graphs (Shift Graph) or on dataset (Shift Dataset).

The graphical shift allows moving the graphs along the horizontal axis: being a graph shift, this applies only on one window at a time.

The dataset shift has the same effect of translating all graphs along the horizontal axis, however in this case the data on which the graph is drawn is translated. This means that all other graph windows that may be present in the layout are automatically translated when the shift is confirmed on a window. This operation differs from Shift Graphs because the lap and not the graphs are dragged. The difference can be noted if the window displays the events; in Shift Lap the events follow the channels; in Shift Graphs the events are fixed.

It is important to note that if a layout is composed by more Graph windows, if the shift is performed on a window in base time, only the windows in base time are shifted and if the Shift operation is performed on a window in base distance, only the base distance window will be shifted.

Another important thing is that the XY windows are affected by Shift Dataset. No other window depends on it. 

No window depends on Shift Graph. The Shift Operations are available only in Post Processing.

WinTAX4 keeps track of the graph shift if a channel is hidden.

See [Shift Dataset](GRAWndCommands.md#GRAMenuOptions_ShiftDataset) and [Shift Graph](GRAWndCommands.md#GRAMenuOptions_ShiftGraph) for details about commands.

## 

#### Hide/Show Graphs

The Hide/Show Graphs function enables to hide/show the corners and the graphs elements of the selected channels.

The name of the channels of which the graph is hidden, are barred in the text box of the Info area.

The function can be enabled through the popup menu displayed by clicking with the mouse on the channels Info area. It is available in Post Processing and Real Time. 

## 

#### X Scale Mode

There are three different bases to show the X-axis: Time, Distance and Revolutions. The Revolutions mode is not available in Real Time.

Time: In Time operating mode, the points in the graphs correspond to samples distributed based on the time of acquisition; the frequency of samples in the graph is constant and correspond to the frequency of the channel. Therefore the Time mode displays on the X-axis scale the time in second of the laps loaded.

Distance: In Distance operating mode, the points in the graphs correspond to samples distributed based on the distance calculated from the base time samples and from a space channel which measures the distance travelled. In this mode the samples may not have a constant frequency since they are the result of a mathematical processing. The distance between the samples is constant only if speed is constant. Therefore the Distance mode displays on the X-axis the scale of the distances, in meters, covered of the Laps loaded. Configure the Distance channel in the [Special Channels](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgSpecialChannels) page of the [General Setup](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md) window.

Revolutions: In Revolutions operating mode, the points in the graphs correspond to samples distributed based on revolutions of the engine calculated from the base time samples and from an rpm channel which measures the revolutions per minute. In this mode the samples may not have a constant frequency since they are the result of a mathematical processing. The distance between the samples is constant only if rpm is constant. Therefore the Revolutions mode displays on the X-axis the scale of the revolutions made by the engine. Configure the Rpm channel in the [Special Channels](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgSpecialChannels) page of the [General Setup](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md) window.

The mode of the X scale can be changed through the configuration, or through the X-Axis command of the Options menu, of the pop-up menu or of the toolbar.

#### Multicolor channels

Opening the Channel Properties page in [Graph Configuration](GRAWndSetup.md#GRAWndSetupOptions_Graphs), in some licenses, enabling the multi color check, you can configure channels to be plotted with different colored bands. The function is available both in Post Processing and Real Time.

#### Graph Areas

Opening the [Graph Configuration](GRAWndSetup.md), you can find the [Graph Area commands](GRAWndSetup.md#GRASetupAreas). The controls handle the graphics area which can be divided into smaller horizontal strips. The maximum number of configurable areas is 10. The areas are separated by a divider with which you can resize them. Each area can be hidden, but at least one is always visible.

Areas are available both in post processing and Real Time.

#### Variance

In comparison mode, the graph windows can display additional channels that show the trend of the difference in time between the first two DataSets enabled. The channels are called Variance, Variance Zoom and Variance Sectors. The channels are displayed on the window with the command Variance from [Tools menu](../../../Menu/Tools.md) or via the shortcut Alt + V. The channels are displayed on the selected graph window or on the first graph window in the layout if another type of window is selected. 

  * Variance is the trend of the difference in time between the first two DataSets enabled calculated in the whole lap

  * Variance Zoom is the same of Variance but calculated on the active zoom.

  * Variance Sectors is the same of Variance but calculated sector by sector.




If the difference is positive it means that, if the traveled distance is the same, the first lap has a slower moving vehicle.

Variance is available only in Post Processing.

For scale calculation modes, [please consult this page](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#Variance_Auto_Scale_Y_Linked-desc). (Variance Auto Scale Y Linked)

##### 

#### Reference Line

This feature permits to plot horizontal lines for some special values of each channel as minimum, maximum, average and custom values too. In Real Time only custom lines are available because it is impossible to determine other values correctly during acquisition.

#### Autoscale Groups of Channels

This function automatically sets the Y Scale range for channel groups. Channels can be assigned to a group in Graph Configuration window , selecting an item from the Group field in the Scale section.

Default configuration for grouping channels can be set in Channels Parameters Setup window. Y Scale range is calculated considering minimum and maximum values for all channels belonging to the same group in all graphic Areas of a single Graph window. Calculation for different Graphs windows is independent.

#### Post Processing View

Enables to export on a Graph window in post processing mode the real time data displayed. The configuration used for the post processing window is the one of the starting real time Graph but it can be modified in the [Real Time](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgRealTime) page of Setup/General.

#### Get Manual Range

Automatically calculates the limits of the Y scale so that the channel is fully displayed and sets the manual mode of the Y scale (Auto Y off). The command is available only in Real Time via right click on selected channel.

#### Real time Comparison

Function enabling comparison of the real time stream of data with a reference lap.

#### 

#### Freeze Real Time Graphs

**Real-time visualization** can be temporarily paused, allowing users to analyze data in post-processing mode. The **Scroll** command enables navigation through frozen data, up to 180 seconds back from the current time.

This function applies to all real-time windows currently displayed in the layout. Frozen data is not saved by WinTAX4.

The command is available via the **Options** menu and the **Graph Toolbar**.

The following shortcuts can be used:

Shift + I: Freezes the layout in the selected window or layout viewer.

Ctrl \+ I: Freezes the single selected analisys window.

I: Freezes all analisys windows of all layout viewers and main windows.

##### Related Issues

  * [Graph window](GRAWndOverview.md)

  * [Elements of the window](GRAWndItems.md)

  * [Configuration](GRAWndSetup.md)

  * [Commands](GRAWndCommands.md)


