### Commands

The main commands available in the [Run Preview window](RunPreviewWndOverview.md) can be enabled through the Popup menu that can be displayed by clicking with the right button of the mouse in the graphic area of the window, on the Info boxes or on the Y scales of the selected channels. Some commands are also available with Keyboard shortcuts.

#### Popup menu

By clicking with the right button of the mouse on the graphic area of the window, the following popup menu is displayed. The menu is dynamic as it can be configured according to the licence granted, but also according to the situation in which it is created; for example the Pan command is displayed only if there is a zoom active or the Clear Selection command is displayed only if at least one channel is selected. In general all pop-up commands of the pop-up menu are the same as those on the toolbar, the Options menu or other WinTAX4 menus.

this pop-up menu allows the access to the following commands:

| DESCRIPTION | DESCRIPTION |
| --- | --- |
| Mode | Contains [Session](RunPreviewWndFeatures.md#RunPreview_ModeSession) and [Run](RunPreviewWndFeatures.md#RunPreview_ModeRun) function. Each mode is described in function section. |
| Downsampling | Toggles activation for Downsampling Factor |
| Zoom Out | Operation opposite to the Zoom In on the graphic area of the window: it displays in smaller detail in respect to the X axis of the interval around the current position of the cursor (see also the [Zoom](RunPreviewWndFeatures.md#RunPreviewFeatureZoom) functions) |
| Zoom In | Zooms the graphic area in respect to the X axis, displaying in better details the interval around the current position of the cursor (see also the [Zoom](RunPreviewWndFeatures.md#RunPreviewFeatureZoom) functions) |
| Minimum Zoom | Shows the graphic area in respect to the whole interval of the X axis (see also the [Zoom](RunPreviewWndFeatures.md#RunPreviewFeatureZoom) functions) |
| Maximum Zoom | Zooms the graphic area in respect to the X axis, showing in maximum details the interval around the current position of the cursor (see also the [Zoom](RunPreviewWndFeatures.md#RunPreviewFeatureZoom) functions) |
| Zoom Fix Range | Shows the graphic area in correspondence with a fixed range of the X axis (see also the [Zoom](RunPreviewWndFeatures.md#RunPreviewFeatureZoom) functions) |
| Shift Working Dataset | Choosing this command, the cursor on Run Preview is fixed at the current position. Then, with the left button of the mouse, it is possible to shift the cursor. When the left mouse button will be released, the shift is propagated to the windows of the layout. |
| Reset all shifts | Resets the shift restoring the initial situation. |
| Pan | Enables the Pan mode to shift the zoom area displayed (see also the [Zoom](RunPreviewWndFeatures.md#RunPreviewFeatureZoom) functions) |
| Clear Selection | Deselects the channels selected on the window |
| Graph Layout | Shows the sub menu to select the vertical arrangement mode of the channels (Parallel, Overlay, Manual). The selected mode is highlighted by a check mark |
| Time or Distance | Switch between Distance (space) mode and Time mode of the X scale |

  * Time: The X axis represents the time instants in seconds.
  * Distance: The X axis represents the distances covered in meters. The Distance channel must be configured in the Special Channels (see the [Special Channels](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgSpecialChannels) page of the [General Setup](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md) window).

The switch can also be done by the short cut X.  
View | Displays the submenu View to select the graphic elements of the window that can be shown or hidden.  
Events / Show events | shows or hides events  
Stop Operations (Esc) | Stops calculating. See Stop Operation paragraph.  
| Properties | Opens the interface to [configure the Run Preview window](RunPreviewWndOverview.md) |
  
By clicking with the right button of the mouse on an Info box or on the Y scale of the channel, the following pop-up menu is displayed. The menu is dynamic as it can be configured according to the licence granted, but also according to the situation in which it is created. In general all pop-up commands of the pop-up menu are the same as those on the toolbar, the Options menu or other WinTAX4 menus

this pop-up menu allows accessing the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Remove Graphs |  | Removes from the window the channels currently selected |
| Clear Selection |  | Deselects all selected channels in the window |
| Stop Operations | Esc | Stops calculating. See Stop Operation paragraph. |
| Properties |  | Opens the interface to [configure the Run Preview window](RunPreviewWndOverview.md) |
  
#### Keyboard Shortcut

To see the complete list of shortcuts available for the graph window, click [here](../../Shortcut/WindowsShortcuts.md#ShortcutRunPreview).

Below are listed those shortcuts that do not appear in any of the commands and of the toolbar buttons described before. These commands can be executed only from keyboard. All these commands are not customizable.

| SHORTCUT | DESCRIPTION |
| --- | --- |
| Up | Increases cursor step in current window |
| Down | Decreases cursor step in current window |
| Left | Moves the cursor left by one step |
| Right | Moves the cursor right by one step |
|  |  |
  
#### Zooming operations:

It's also possible to operate the zoom using the scroll wheel of the mouse:

| SHORTCUT | DESCRIPTION |
| --- | --- |
| Mouse wheel Scroll up / down | Moves zooming area left and right |
| Mouse wheel scroll up / down + CTRL Key | Enlarges and reduces the zooming area |
|  |  |
  
#### View

Displays the sub menu to select the graphic elements of the window that can be shown or hidden

  * Transparent Cursor Values shows cursor values in transparent mode on the left side of the graph box.

  * Cursor position, shows/hides the information box indicating the cursor position on the X axis

  * X Scale, shows/hides the X scale

  * Y Scale, shows/hides the Y scale

  * Zoom Bar X, shows/hides the horizontal Zoom Bar

  * Cursor Values, shows/hides the information boxes indicating the current values of the cursor

  * X Scale Unit, shows/hides the measurement unit of the X scale

  * Show All, shows all the graphic elements

  * Hide All, hides all the graphic elements

  * Lap time, shows/hides lap times.




#### Stop Operations

Stops calculating. When you choose this command appears the following message.

Choosing Yes, the run Preview windows interrupts the preview and clear the window.

##### Related Issues

  * [Run Preview window](RunPreviewWndOverview.md)

  * [Elements of the window](RunPreviewWndItems.md)

  * [Functions](RunPreviewWndFeatures.md)

  * [Configuration](RunPreviewWndSetup.md)


