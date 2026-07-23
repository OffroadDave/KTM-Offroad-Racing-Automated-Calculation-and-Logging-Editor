## DataSet panel

The Dataset panel is used to control which DataSets are displayed within the various windows of a layout during a working session. It is an interactive control which is used both to see which DataSets are active in a particular window and to enable Datasets to one View/DataSet Panel.

The DataSet Panel is formed by the DataSet List section and by the Bookmark List section. The lists can be expanded or compressed through the button of the corresponding headers.

it is also possible to use the commands of the pop-up menu.

  * Expand all Expands both lists

  * Collapse all Compresses both lists

  * DataSets List Expands the list if is selected

  * Bookmark List Expands the list if is selected




### DataSets List

The DataSet List includes the Telemetry DataSet that was dragged on a Graph window in PostProcessing, changes it into a Real Time window. Below the Telemetry DataSet there is the Datasets list.

#### Telemetry DataSet

The Telemetry DataSet represents the Real Time stream coming from the Real Time acquisition process

This special DataSet can be used to select which windows are set to display Real Time or PostProcessing data (available for [Graph](../../AnalysisWnds/Wnds/GRA/GRAWndOverview.md) windows). it is located at the top of the Dataset List in the workspace panel. The Telemetry Dataset is connected to standard enable/disable commands and drag&drop into windows. (See the description of available commands in DataSets List)

The RETURN command ('Enable dataset to all windows') can be used as a short-cut to change a post processing layout to real time layout

The background color of the Telemetry Dataset allows you to know when it is connected to one (or more) window

#### 

#### Telemetry DataSet Multidevice

If the license has this function enabled, the tool bar is visualized like in the following screenshot:

In the image below, multiple "Real Time acquisition sources" have been configured and enabled in the acquisition manager. 

The green icon appears only on "Valid" and "Enabled" Datasets. If a Dataset is disabled or not valid, as can be seen below, the green icon does not appear.

If the "Enable Dataset to all windows" function is used, the selected Dataset is configured on all windows. In a similar way, the "Enable Dataset only to selected windows" configures only the selected window to visualize the selected Dataset.

If a Dataset is not valid, the first valid Dataset will be used instead.

In case any of the above mentioned functions are used (Enable Dataset to all windows and Enable Dataset only to selected windows), the related analysis windows are configured in "Real time" mode.

Every time an analysis window is selected, the visualized "Dataset" is highlighted in the Dataset list of the Dataset panel.

#### 

#### DataSets List

Below the Telemetry there is the header of the list, with headings that can be configured in the [DataSet Header Setup](../../WTXSetup/WndSetupDataHeader/WTXSetupDataHeader.md#WndDataSetHeaders) .

Then there is the 10 Slots list where each line corresponds to a possible DataSet loaded.

The clock icon means that in AutoRx the lap is loaded on that DataSet.

When the Dataset line is red (or brown if the list line is selected with the mouse), it means that the DataSet is active on the selected window.

When the Dataset line is white (or black if the list line is selected with the mouse), it means that the DataSet is not active on the selected window.

This means that the highlights on the list indicate which DataSets are active on the selected window.

In the following example on the [Graph](../../AnalysisWnds/Wnds/GRA/GRAWndOverview.md) window the DataSets available on slot 1 and 3 are displayed and compared. DataSet on slot 1 is also valid globally on all layout windows; in case of AutoRx, Slot 6 will receive the new lap.

The DataSet can be dragged & dropped:

  * on the DataSets List to change their position

  * on the Bookmark List to save the references

  * on the Analysis windows to enable them to that DataSet




#### Commands

Click with the right button of the mouse in the Datasets List to see the options.

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Enable DataSet to all Windows | Return | Displays the selected DataSet in all windows |
| Disable DataSet to all Windows | Backspace | Hides the selected DataSet in all windows |
| Enable DataSet only to selected Window | Ctrl \+ Return | Displays the selected DataSet in the window with the focus |
| Disable DataSet only to selected Window | Ctrl \+ Backspace | Hides the selected DataSet in the window with the focus |
| Load a DataSet into selected position | Ins | Allows to return to the Data Browser and select some data to load into the selected DataSet slot |
| Remove the selected DataSet | Del | Clear the Dataset from memory |
| Load a DataSet from a Bookmark into Selected Position |  | Allows to select a DataSet from Bookmark list to be loaded into the selected Dataset slot |
| Add to Bookmark |  | Adds the selected DataSet to Bookmark List |
| Set Default AutoRX Dataset |  | Defines which DataSet will be refreshed with the [AutoRx](../../Acquisition_Manager/Rx_tasks/Rx_tasks.md#AutoRx) function. |
  
## 

#### Bookmark List

The Bookmarks are references to favorite DataSet that can be loaded by double clicking on the wished bookmark or dragging it with the mouse in a slot.

Bookmarks can be added as follows:

  * Through drag & drop from DataSets List

  * Through the Add to Bookmark command from the pop-up menu opened by clicking with the right button of the mouse on DataSets List

  * Through the Add command from the pop-up menu opened by clicking with the right button of the mouse on Bookmark List.




Other commands of the window (with pop-up menu) are:

  * Remove: removes the bookmark cancelling the reference to the dataset

  * Rename: renames the bookmark.


