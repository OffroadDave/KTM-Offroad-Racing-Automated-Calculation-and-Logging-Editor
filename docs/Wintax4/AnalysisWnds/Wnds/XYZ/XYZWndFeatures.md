### Functions

The [XYZ](XYZWndOverview.md) window has the following functions:

  * Cursor

  * Display of the elements

  * Zoom

  * Selection of rectangles in the graphic area

  * Analysis

  * Connect Cursor




####  Cursor

The cursor is represented by a vertical and a horizontal line on the graphic area. It allows scrolling all values in the X and Y scale range, up-dating the corresponding channels values in the info boxes.

The cursor can be moved in the graphic area moving the mouse by pressing the left button; if the Best Fit analysis are not enabled, the cursor stays on the polynomial curve created.

####  Visualization of the elements

The elements of a window can be visualized or hidden enabling the View command on the Options menu or on the pop-up menu.

####  Zoom

The zoom enables to display detailed sections of the graphic area.

To zoom, please choose one of the following possibilities:

  * select a rectangle in the graphic area and select the Zoom command from the pop-up menu displayed by clicking the right button of the mouse .

  * Zoom Out, Zoom In, Zoom Min, Zoom Max commands from the pop-up menu or from the toolbar.




To scroll the displayed area in detail by zooming, please chose one of the following possibilities:

  * drag the zoom bars with the mouse, clicking the left button,

  * enable the Pan mode from the pop-up menu commands or form the icon on the toolbar, and move the mouse clicking the left button. Another way to enable the panning is to press the CTRL button and move the mouse clicking the left button.




####  Selection of rectangles in the graphic area

Rectangles in the graphic area can be selected by dragging the mouse and clicking the right button.

During this operation, the selected rectangle is displayed and the rectangle information of the X axis range and of the zoom percentage on the Y axis are displayed.

####  Analysis

Best Fit: Best fit curve is a polynomial function of configurable degree that corresponds to the interpolation of the desired channel data. Curve fitting is the process of constructing a curve that has the best fit to a series of data points. On the Y axis, the cursor is displayed as an horizontal line, engaged on the vertical cursor intersection with the Best Fit curves. In configuration the user can set the style, the color and the width of the horizontal line and of the best fit line.

The interpolation degree used for Best Fit is displayed in a text box below the name of the Y channel.

The coefficients used for the Best Fit interpolation can be displayed in the Best Fit coefficients window that can be displayed through the Best Fit Information command of the Options menu or of the toolbar.

Interpolation : the sampling frequency of each channel configured on the Y axis can be modified.

Condition : when a condition is present, only the points complying with this condition are displayed.

#### Connect cursor

The position of the cursor is automatically up-dated in all the displayed windows whenever the position of the cursor is changed in the current XYZ window in Graph mode. Some windows are always connected, while other are connected only if in [Setup/General](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgGeneralOptions) the auto connect cursor flag is set. To connect two XYZ windows, this option must be set. If the cursor moves closer a point on the XYZ window, the point is highlighted and the cursors of the other windows are connected to the X-axis value identified from the intersection of X and Y and from the color of Z.

### 

##### Related Issues

  * [XYZ window](XYZWndOverview.md)

  * [Elements of the window](XYZWndItems.md)

  * [Configuration](XYZWndSetup.md)

  * [Commands](XYZWndCommands.md)


