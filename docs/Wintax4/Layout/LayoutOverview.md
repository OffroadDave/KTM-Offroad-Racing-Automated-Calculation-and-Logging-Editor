### Layout Overview

In WinTAX4 the configuration of the analysis windows can be saved so that it is possible to quickly reopen it at a later time. WinTAX4 offers the possibility to access the saved layouts and to reorganize them at will. The Layouts can be saved, opened, arranged through the commands available in the file menu and in the Layout toolbar. Moreover there is a bar containing some tabs that allow to quickly open the favorite Layouts.

By "saving the layout" it is intended that what is saved is the position of each open [analysis window](../AnalysisWnds/AnalysisWndsOverview.md); the position of the toolbars and of the possible working windows is not saved. The licenses granting access to the "[save in the layout](../Datasets/DataSetLayout.md)" feature, will allow to save also the active slots for each window.

### Menu File 

In the "File" menu the following commands regarding layout can be used: .

  * Load Layout: This command opens a selection window that searches for all layout previously saved in the User directory and allows to select one of them to be opened. The shortcut is Alt + Y.

  * Layout Wizard: Opens the layout wizard window which creates custom layouts.

  * Save Layout: Saves on the current layout file (.wly) the position of the windows currently open on WinTAX4 and the information about the slots in the Dataset. The shortcut is Y.

  * Save Layout As: Opens a window to select a layout file (.wly) to save the position of the windows currently open on WinTAX4 and the information of the slots of the Dataset. An existing layout can be overlapped or the name of a new layout can be written.




### Layout Toolbar

The visualization of the Layout Toolbar can be enable/disabled through the View/Toolbars/Layout command of the main menu.

For a detailed description of the toolbar, click [here](../Toolbars/ToolbarLayout.md).

Other elements can be added to the list also through the [Workspace](../WTXWorkWindows/WndWorkspace/WndWorkSpace.md) window. In this window just place the mouse on the element to be added, open the pop-up menu clicking the right button of the mouse and select the Add to layout bar command.

To cancel the elements added, always open the Layout Bar window.

### Layout Selection Tab

The "Layout Selection" Tab is displayed in the top part of the main window of WinTAX4 and it is composed by the same amount of the layouts available in the Layout Bar window list. Elements available in the list that are not layouts, are not displayed. So it is possible to have an easy access to the favorite layouts by simply clicking on the corresponding tab. 

The Layout Selection Tab is enabled when there is at least one layout loaded in the list; it can be displayed or hidden through the Layout Selection Tab command in the [View](../Menu/View.md) menu.

There are five navigation buttons on the left of the bar: 

  1. The cross removes all the window in the layout.
  2. The external arrows scroll the bar to the end respectively on the left-hand and right-hand sides
  3. The internal arrows scroll the bar on the left and on the right side with small steps.



Right Clicking the mouse button on the tabs of the Wintax4 toolbar shows a popup menu. It has the following commands: 

| **Command** | **Description** |
| --- | --- |
| Insert | Inserts a new layout before the selected one. |
| Rename... | Renames the selected layout. Can also be done by double clicking with the left mouse on the tabs in the Layout Bar toolbar. |
| Delete | Deletes the selected layout. |
| Tab Color... | Allows to put colors on the selected tab. |
  
The Layout Selection Tab is synchronized with the combo in [Layout Toolbar](../Toolbars/ToolbarLayout.md) so that they both display the current layout. If a layout has changed a message appears that asks you to confirm the changes.

Yes confirms the changes, No exits without saving, Cancel aborts the change layout. 

The last element on the right of the Layout Selection Tab is a tab with a plus sign. A click on the plus sign opens the Layout Wizard.

With this wizard you can select the preferred layout. An overview will be shown in the preview window while a description of the layout appears in the text box. Pressing the button Create, you can build, save and open a new layout with the desired structure. If "Add to Layout Selection Tab" is checked, the layout is inserted in the Layout Selection Tab. Pressing the Cancel button cancels creation of the layout.