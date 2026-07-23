## PopUp Legend

The Popup Legend displays in a popup dockable window the channel values available on the graph at the current cursor position; the difference between the values and the values available at Datum can also be displayed.

The Popup Legend can be opened through the Options menu, or the popup menus displayed by clicking with the mouse on the graphic area or on the channels information area.

The window has the following features:

  * it is a dockable object

  * It allows to calculate the difference (time based and distance based) between the current position of the cursor and a fixed point (Set Datum) and the difference between one Set of Data and all other Set of Data currently loaded in a window.

  * it is linked to the Graph window currently focused




#### Standard Mode

The standard mode for the PopUp Legend is with no Datum enabled: the window shows for every channel and every DataSet the current position of the cursor (time and distance) and the cursor values for each valid channels.

#### Set Datum Mode

To activate the Set Datum mode, simply press the Set Datum button or click with the right button of the mouse in the graph window to open the popup menu. When Set Datum is active, the area between the datum and the cursor is grayed. With a double click on the window, the datum is repositioned. It's also possible to move the Datum cursor by left clicking and dragging the cursor, while pressing CTRL on the keyboard.

In Set Datum Mode, for each channel and for each Dataset the following values are shown:

Value: The value of the channel in the specified Dataset.

Datum: The value of the channel in the fixed datum.

Delta: The difference between the value of the channel in the current position and the value of the channel in the fixed datum.

Gradient: The "difference quotient" between channel value and fixed datum (it is the function difference divided by the point difference).

#### 

#### Delta Mode

The Delta mode allows to calculate the difference between the selected DataSet and the other ones. The user can select which Dataset will be the reference dataset, simply using the Delta button on the header; the reference DataSet will be highlighted on legend window. The referenced DataSet column will contain it is values, while the other columns will show the differences from the reference dataset.

#### Commands

The main commands available in the PopUp Legend window can be enabled through the Pop-up menu that can be displayed by clicking with the right button of the mouse in the graphic area of the window.

##### Pop-up menu

By clicking with the right button of the mouse on the graphic area of the window, the following pop-up menu is displayed. The commands of this menu will perform actions on both the Popup Legend and Graph window.

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Print Window to Clipboard |  | Copy the image of the window in the current state on the clipboard. |
| Remove Graphs | Delete | Removes from the window the channels currently selected. |
| Clear Selection |  | Clear channels selection in the current window. |
| Select All Channels | Ctrl \+ A | Selects all channels in the current window. |
| Show Graphs | Alt \+ Ctrl + S | Shows the graphs corresponding to the selected channels. |
| Hide Graphs | Alt \+ Ctrl + H | Hides the graphs corresponding to the selected channels |
| Search | Shift \+ - Shift \+ + | Displays the sub menu to select the method to search the absolute Minimum and Maximum points in the curve of the graph of the selected channel. To use the commands, a channel must be selected; the Minimum will place the cursor on the absolute minimum point in the part displayed of the graphs, the Maximum will place the cursor on the maximum point. |
| Set Datum | D | Set the [Datum cursor](../../AnalysisWnds/Wnds/GRA/GRAWndFeatures.md#GRAFeatureDatum) |
| Close Datum | Ctrl \+ D | Close the [Datum cursor](../../AnalysisWnds/Wnds/GRA/GRAWndFeatures.md#GRAFeatureDatum) |
| Positioning of Datum cursor | CTRL \+ Left button down | Allows the positioning of the Datum cursor on the graph |
| Changing the position of the Datum Cursor | CTRL \+ Left button pressed + Mouse move | Allows changing the position of the Datum cursor after it has been positioned |
| Graph Cursor |  | Displays the sub menu to select the graph cursor (single, multiple, vertical) Single. The window displays a cross cursor for the selected channel. Multiple. The window displays a cross cursor for each channel in the window. Vertical (default). The window displays a vertical cursor. |
| Copy Data to Clipboard | Ctrl \+ Shift + C | Copies to Windows clipboard the series of channels data configured in the window |
| Open in Excel | Shift \+ X | Opens an Excel sheet with data displayed in the PopUp legend |
| Open in Matlab |  | If you select a channel on the PopUp window, you can launch the Open command in Matlab, using the right arrow key or using the Options menu, which allows to open a Matlab Workspace window in which the required channel is loaded. On Matlab, the channel appears as an array of double values; with this array, you can carry out all the operations and analyses that Matlab allows you to use. If you repeat the Open in Matlab operation on a number of channels, they will be added to the list of channels in the workspace. |
| Properties | E | Opens the interface to configure the [Graph window](../../AnalysisWnds/Wnds/GRA/GRAWndOverview.md) |
  
### Keyboard Shortcut

Below find listed those shortcuts that do not appear in any of the commands and of the toolbar buttons described before. These commands can be executed only from keyboard. All these commands are not customizable.

| SHORTCUT | DESCRIPTION |
| --- | --- |
| Space | Opens the popup menu on selected channels. |
|  |  |