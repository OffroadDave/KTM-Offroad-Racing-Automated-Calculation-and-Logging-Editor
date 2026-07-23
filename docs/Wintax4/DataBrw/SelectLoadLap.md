## Selecting and loading laps

To load and display the data on WinTAX4, select the session to be analyzed in [sessions list](Session_Laps.md#ListaSession) then switch to [lap list](Session_Laps.md#ListaLap) and here select the lap or laps to be analyzed. The groups of laps must not necessarily be contiguous.

### Single lap

A single lap can be selected with a click of the mouse or by scrolling through the lap list with the arrow keys.

To load the selected lap, double-click with the mouse or press the Enter key from keyboard or use the Open command from the File menu.

Note: if the flag Lap Comparison is set, the double click adds the lap to the comparison list.

The lap downloading process can be interrupted using the ESC key.

### Append (set of laps)

An Append is a link of two or more laps to obtain a Single Lap. The resultant lap will have the Lap Time equal to the sum of the linked laps.

If laps have different frequencies, all channels will be convert to higher frequency.

A group of laps can be selected as follows:

  * Left click and drag to select a set of contiguous laps

  * Shift+click to select a set of contiguous laps

  * Ctrl-click to select a set of non contiguous laps

  * R to select a whole Run (it is available also the Select Run command on the Popup menu of the lap list)




A groups of laps can be loaded.

  * Double clicking to load and view a single lap. If the flag Lap Comparison is set, the double click adds the lap to the comparison list.

  * Press Enter or the File/Open command to load and display the group of laps.

  * The lap downloading process can be interrupted using the ESC key




### Lap Comparison

The lap comparison is one of the basic characteristics WinTAX4 because it enables to compare various laps so to evaluate the performances of the vehicle and of the driver in different situations. The lap to be compared can be chosen through the DataBrowser, and through the [DataSet](../Datasets/Loading_Datasets.md) . Not only single laps but also groups of laps, the 'appends' can be compared. A special type of comparison is the [Reference Lap](Reference_Lap.md) .

As to the DataBrowser, the laps for the comparisons can be selected as follows:

  * Ctrl+Return to add gradually to the DataSet the lap or the selected laps.

  * Ctrl+ N to add to the Nth element of the DataSet the lap or the laps selected. Max 10 DataSet can be added; Ctrl+0 adds the dataset number 10.

  * If the Lap Comparison flag is set, the lap can be added to the comparison list by double clicking.




Once the comparison list has been created, it can be loaded by pressing Enter or the File/Open command

Users can append laps to the first Data Set in comparison with "Add to Append" command from the main File menu or from pop up menu.

User can add or remove single laps to first and second Data Set in comparison with mouse click on Selection Panel columns in Lap List. Command Selection Panel in main View menu switches visualization for Selection Panel columns.

For more information about DataSet, click [here](../Datasets/Loading_Datasets.md).

### Multisession mode

Allows to load all laps of more than one session.

In the Sessions List user can select two or more sessions through standard Windows multi selection controls (ctrl, shift), then confirm the selection by clicking Enter.

The loaded data is only usable in the Lap Report windows of the current layout.

Previously loaded Datasets are not removed. All other windows (e.g. Graph, XY) are not affected.

To restore the standard behavior user has to select again a single session from Data Browser.