### Functions

The [XY](XYWndOverview.md) window has the following functions; unless otherwise specified, the features listed are available in post processing and real time mode.

  * Cursor

  * Graph Layout

  * Channels selection

  * Elements display

  * Analysis

  * Graph Mode

  * Single channel visualization

  * Comparison




The following functions available only in post processing mode:

  * Selection of rectangles in the graphic area

  * Zoom

  * Multiple x-axis

  * Shift comparison

  * Connect cursor




#### Cursor

The cursor is identified by a horizontal and a vertical line in the graphic area. It allows scrolling all values in the range of the X scale and of the Y scales, updating the corresponding values of the channels in the info boxes.

The cursor can be moved in the graphic area moving the mouse and pressing the left button. 

If the window has more X axes, then the intersection between the lines is identified by a small circle for each cursor.

####  Graph Layout

The graph layout functions permits to modify the vertical arrangement of the graphs and Y scales corresponding to the channels configured on the Y axis.

The functions can be enabled through the Graph layout command on the Option menu, on the pop-up menu and on the toolbar, or by configuring the Layout field, in the Window Option section in the configuration window.

##### Parallel

The graphs and the Y scales corresponding to the channels are vertically arranged to equally divide the available space.

##### Overlay

The configured channels share the vertical area; the Y scales are horizontally listed side by side and the graphs overlap.

##### Manual

The graphs and the Y scales corresponding to the channels are vertically arranged on the basis of the user settings in the Top and Bottom fields that can be modified through the configuration window in the Graphs section or through mouse operations.

Clicking on a Y scale will make appear some small squares; using these squares you can resize or move the Y scale

#### Channels selection

A channel (Y scale, graphs and info boxes of a channel) can be selected by clicking with the left button of the mouse on the corresponding Y scale, or on the value box of the cursor channel. To carry out a multiple selection of channels, select the channels pressing the CTRL button. As an alternative, a channel can be selected with the TAB button of the keyboard. The Y scale, the graph and the cursor channel of the selected channels are highlighted.

To deselect a channel, click with the left button of the mouse on the Y scale or on the info boxes, or select the Clear Selection command on Options menu or on popup menu which deselects all channels.

#### Elements display

All the elements of the window can be shown or hidden by the view commands on the Options menu or on the pop-up menu.

#### Analysis

In XY Windows some numerical or statistical analysis are allowed: Best Fit, Reference Best Fit, Interpolation and Condition.

Best Fit: Best fit curve is a polynomial function of configurable degree that corresponds to the interpolation of the desired channel data. Curve fitting is the process of constructing a curve that has the best fit to a series of data points. For each channel on the Y axis, on which Best Fit has been activated, the cursor is displayed as an horizontal line, engaged on the vertical cursor intersection with the Best Fit curves. In configuration the user can set the style, the color and the width of the horizontal line and of the best fit line.

The interpolation degree used for Best Fit calculation is displayed in a text box inside channel cursor or in the legend, if enabled (like in the picture below). The sampling frequency of each channel configured on the Y axis can be modified.

The coefficients used for the Best Fit interpolation in the window can be displayed through the Best Fit Information command (menu Options / Best Fit Information, see picture below) or can also be shown on window if View Best Fit information in transparent global flag is enabled (see [XY Configuration](XYWndSetup.md))

On this window there are further commands, calling by right click on window:

  * Print: Prints the coefficients and formula window.

  * Copy Data to Clipboard: Copies to clipboard the coefficients values for each channel available taking also into account possible comparisons.

  * Open in Excel: Opens an Excel sheet containing coefficients values for each channel available taking also into account possible comparisons.

  * Export to Virtual Channel: Open a further window where the formula is adapted for Virtual channel using. There is a formula for each selected channel.




Reference Best Fit: The Reference Best Fit curve is a polynomial function of configurable degree that is constructed using coefficients that can be manually set in configuration. You can also configure the style, the color and the width of the curve. The interpolation degree used for Best Fit calculation is displayed in a text box inside channel cursor. The sampling frequency of each channel is configured on the Y axis can be modified.

Interpolation: With interpolation you can resample the channel choosing a new frequency from configuration. 

Conditions: If a condition is set, the points of the graph are displayed only if the condition is verified. Best Fit, Interpolation and Condition can be set through the configuration interface.

#### Graph Mode

The Graph Mode functions allow to modify the data display in the graphs window. The function is enabled by configuring Graph Mode field, in the Window Options section in the configuration window.

Three modes are available, Graph, Smooth, Percent, as displayed below:

##### Graph

The Graph mode shows all the couple of points (x, y) of all channels configured.

##### Smooth

The Smooth mode displays the graph using the density of values in small rectangles. Every rectangles contains a certain number of points which are calculated as percentage respect the whole channel. The window shows this percentage value with a variable color scale depending on the density and show the values in brackets near the cursor value.

The Smooth mode uses only the first X channel with its first Y channel.

##### Percent

The Percent mode displays the graph using the density of values in grid rectangles. Every rectangle contains a certain number of points which are calculated as percentage compared to the whole channel. The window shows this percentage with a text in each rectangle with value greater than zero.

The Percent mode is displayed only if a grid is configured because the calculated rectangles correspond to the grid rectangles.

The Percent mode uses only the first X channel with its first Y channel.

####  Zoom

The zoom allows displaying in detail some sections of the graphic area.

To zoom, use one of the following methods:

  * select a rectangle in the graphic areas and select the Zoom command from the pop-up menu

  * Zoom Out, Zoom In, Zoom Min, Zoom Max commands from the pop-up menu or from the toolbar.




To scroll the displayed area in details by zooming, use one of the following methods:

  * drag the zoom bars with the mouse, pressing the left button

  * enable the Pan mode of the commands of the pop-up menu or from the icon on the toolbar, and move the mouse pressing the left button.




####  Selection of rectangles in the graphic area

The rectangles in the graphic area can be selected by dragging the mouse while pressing the right button.

During the dragging, the selected rectangle is highlighted and the information of the X axis range and of the zoom percentage on the Y axis of the rectangle are displayed in a pop-up window.

####  Single channel visualization

The graphs of a channel configured on the y axis can be displayed in single mode using the arrow keys of the keyboard (right arrow, left arrow). The arrows show the channels in sequence in single mode and, after making the full circle, returning to the multiple configuration of departure.

####  Comparison

The window supports the display of multiple laps in comparison. In the picture below there are two laps in comparison. The values ​​displayed in the channel information area belong to the first lap enabled.

#### 

#### Multiple x-axis

The Window can be configured to have multiple x-axes, up to a maximum of four. For each x-axis, the user can configure more than one y-axes. X-axis will be displayed parallel on the bottom of the graph box.

If the XY is connected to another analysis window (e.g. a Graph window), the XY will show one cursor for each X-Y combination.

#### Shift Comparison

The XY window applies the shift comparison function set on any Graph window. When the shift is confirmed on any lap being compared, the XY window adapts to the new setting. There are two separate cases, based on whether the shift is on the first lap or on any of the other laps.

In the first case, the sampling interval on which the XY window is constructed is reduced because the shift shortens the lap, from a point of view of XY.

The second case is that the shift is applied on any of the other laps. The sampling interval on which the window is constructed this time does not change because the first lap has not changed. The trace of the lap is on the other hand changed in shift.

#### 

#### Connect cursor

The position of the cursor is automatically up-dated in all the displayed windows whenever the position of the cursor is changed in the current XY window in Graph mode. Some windows are always connected, while other are connected only if in [Setup/General](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgGeneralOptions) the auto connect cursor flag is set. To connect two XY windows, this option must be set. If the cursor is near to one of any points inside the XY window, the point under examination is highlighted and the cursors of the other windows are connected to the X-axis value identified from the intersection of X and Y.

### 

##### Related Issues

  * [XY window](XYWndOverview.md)

  * [Elements of the window](XYWndItems.md)

  * [Configuration](XYWndSetup.md)

  * [Commands](XYWndCommands.md)


