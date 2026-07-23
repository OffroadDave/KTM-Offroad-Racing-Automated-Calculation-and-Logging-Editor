### Functions

The [Run Preview window](RunPreviewWndOverview.md) has the following functions:

  * Cursor

  * Layout

  * Zoom

  * Rectangles selection in the graphic area

  * Channels selection

  * X Scale Mode

  * Manual Range

  * Lap Selection

  * Mode

    * Session

    * Run




The [Run Preview window](RunPreviewWndOverview.md) has a function that is able to interact with the other windows displayed.

  * Auto Connect Zoom: the zoom position is automatically updated in all the displayed windows whenever the zoom position in the current "Run Preview window" is changed and vice versa. The zoom is bidirectionally connected only if in [Setup/General](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgGeneralOptions) the auto connect zoom flag is set. If this flag is off Run Preview Window can update the zoom of the other windows but not vice versa.




#### Cursor

The cursor is the vertical line displayed in the graphic area that enables to scroll all values in the range of the X-axis, updating the corresponding values of the channels in the info boxes.

The cursor can be moved along the X-axis in one of following ways.

  * Moving the mouse in the graphic area while the left button is pressed.

  * through the arrow keys of the keyboard (right and left arrow key).




The stepping of the cursor can be configured using shift + arrow keys up and down. The minimum value is 0.001s and the maximum 50s.

####  Layout

The Layout function enables to change the aspect of the graphic window and especially the vertical arrangement of the graphs and of the Y scales.

It can be enabled through the Graph Layout command in the Options menu, the popup menu and on the button on the toolbar, selecting one of the suggested modes:

| COMMAND | DESCRIPTION |
| --- | --- |
| Parallel | The vertical space is divided for the number of channels configured; the graphs and the scales of the channels are vertically arranged without overlapping |
| Overlay | The configured channels share the vertical space; the Y scales are horizontally arranged side by side, the graphs overlap. |
| Manual | The graphs and the Y scale corresponding to the channels are vertically arranged based on the settings of the user in the Top and Bottom fields that can be changed through the configuration window in the Graphs section. To quickly configure the graphs, just select with the mouse the channel to highlight the selection squares allowing to drag the graph along the vertical axis or allowing to enlarge or reduce the space covered. |
  
#### Zoom

The zoom doesn't enlarge the graphic area to display in higher or smaller details the channels graphs as in Graph Window; in Run Preview window the zoom identify a square rectangle that encloses the area which will be zoomed in the other windows. The only zooming available is Horizontal Zoom refers to an interval of the X-axis. 

There are different ways to zoom and to move the area displayed in details

  * manual selection of a zoom rectangle in the graphic area.




Select a rectangle of the graphic area with the mouse or the keyboard. Select Zoom in the Popup menu displayed by clicking with the right button of the mouse.

  * Zoom In, Zoom Out, Zoom Min, Zoom Max commands in pop-up menu.



  *     * Zoom In: Zooms horizontally the graphic area in relation to the X-axis; the interval around the current position of the cursor is also displayed.

    * Zoom Out: Opposite operation compared to Zoom In, the graphic area is displayed in lower detail with reference to the interval of the X-axis around the current position of the cursor.

    * Zoom Min: Zooms the graphic area in relation to the X-axis, in maximum details, with reference to the current position of the cursor.

    * Zoom Max: The graphic area is displayed in comparison with the whole extent of the X-axis; it corresponds to the minimum details (100%) and it is the default setting when the graphic windows are opened to be displayed.

  * Zoom Fix Range




The Zoom Fix Range can be enabled through the popup menu of the Zooms horizontally the graphic area, displaying the interval around the current position of the cursor for a fixed interval whose value set in the Zoom Fix field (Time to Distance according to the mode of the X scale) of the [General Options](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgGeneralOptions) of the [General Setup](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md) window.

  * Pan




The Pan mode allows to shift the displayed zoom area using the mouse or the keyboard. The Pan mode is automatically enabled when mouse is over the zoom area. The cursor of the mouse is displayed as hand-shaped. To shift the displayed zoom area, move the mouse pressing the left button. To exit the Pan mode, just release the left button of the mouse

#### 

#### Selection of rectangles in the graphic area

To select rectangles in the graphic area, proceed as follows: drag the mouse while pressing the right button and the Ctrl key.

During this operation, the selected rectangle is highlighted and in a pop-up window, the information about the interval of the X-axis and about the zoom percentage on the Y-axis of the rectangle is displayed.

#### Selection of the channels

A channel can be selected (i.e. the Y scale, the graphs and the info boxes of a channel) by clicking with the left button of the mouse on the Y scale or on the info boxes of the channel. The channel is selected also clicking on the area of the transparent cursors. To multiple select channels, select the channels keeping the CTRL key pressed.

The Y scale, the graphs and the info boxes of the selected channels are highlighted.

To deselect a channel, click with the left button of the mouse on the Y scale or on the info boxes or deselect the Clear Selection command of the Options menu or of the pop-up menu.

Clicking with the right button of the mouse on the Y scale or on the info boxes of the Info Channels area, the channel is automatically selected; if the channel was not selected, other already selected channels are deselected.

The selected channel has two display modes that can be configured in [Setup/General.](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgGeneralOptions) In the highlight combo, two modes can be chosen Bold that shows the channel in bold or the Blink mode shows the blinking channel.

#### X Scale Mode

The Time mode displays on the X-axis the scale, the times of the Laps loaded. The Distance mode displays on the X-axis the scale of the distances covered of the Laps loaded. To apply the Distance mode, set the Distance channel in the [Special Channels](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgSpecialChannels) page of the [General Setup](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md) window.

The mode of the X scale can be changed through the configuration, or through the X-Axis command of the Options menu, of the popup menu or of the toolbar.

#### Manual Range

Automatically calculates the limits of the Y scale so that the channel is fully displayed and sets the manual mode of the Y scale (Auto Y off).

#### Lap Selection

Clicking with the mouse on one of the sections of the Data Selection Bar, the correspondent lap is loaded on WinTAX4. The laps belonging to the current Dataset loaded are highlighted. One or more sections can be selected using the mouse: first select one lap and then, without release the left button, lead the mouse to pass over the required sections.

#### Mode

Session

The window displays all laps belonging to the same Session of the current selected lap.

Run

The window displays all laps belonging to the same Run of the current selected lap 

##### Related Issues

  * [Run Preview window](RunPreviewWndOverview.md)

  * [Elements of the window](RunPreviewWndItems.md)

  * [Configuration](RunPreviewWndSetup.md)

  * [Commands](RunPreviewWndCommands.md)


