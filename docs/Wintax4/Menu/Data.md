## Data

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous | Page Up | Loads Previous Lap (only for first DataSet). |
| Next | Page Down | Loads Next Lap (only for first DataSet). |
| Unselect Last |  | Unloads last (higher ABS) lap from append. |
| Append Next |  | Loads next (higher ABS) lap in append. |
| Shift Up Comparison |  | Shifts all comparison laps upwards (previous ABS in the archive). |
| Shift Down Comparison |  | Shifts all comparison laps downwards (the next ABS in the archive). |
| Select Reference Lap | R | Opens the DataBrowser to load [Reference Lap](../DataBrw/Reference_Lap.md). This feature is available with some licenses only. |
| Remove Reference Lap |  | Unselect [Reference Lap](../DataBrw/Reference_Lap.md). This feature is available in some licenses only. |
| Select Best Lap | F | Chooses the fastest lap from the current selection. It requires more than one lap loaded in append mode. |
| Edit Best Lap Settings |  | Allows selecting which markers are being used in the selection of the best lap. [See more here](Data.md) |
| Enabling Dataset | Ctrl \+ "n" | Enables or disables the desired Dataset. |
| Switch to Working Dataset | Alt \+ "n" | Moves the desired Dataset in the first slot. |
| Manual Lap Definition |  | Performs the manual lap definition operations. |
| Confirm Operation | End | Finishes the manual lap definition and confirm the division of the lap. |
| GPS Split lap |  | Allows the separation of laps based on GPS coordinates |
| Edit Lap Properties... | Ctrl \+ F10 | Allows to change the context information of the lap (e.g. constants, user records, driver name, abs etc.) |
| Setup Editor | Ctrl \+ F10 | Opens [Setup Editor](../SetupEditor/Setup_Editor.md) environment. Depending on each specific licence, Setup Editor could replace Edit Lap Properties. |
| Playback |  | Opens the submenu which performs the playback operations that allows the Real-Time simulation with Post Processing data. |
| DataSet Telemetry to all Windows | T | Allows to switch from the Post-Processing mode to the Telemetry mode and vice versa all the windows in a layout. |
| DataSet Telemetry to Layout Viewer | Shift \+ T | Allows to switch from the Post-Processing mode to the Telemetry mode and vice versa all the windows in a layout. |
| Export BookMark Deep Link |  | Export a Deep Link from the current dataset and cursor position. More info here |
| Import BookMark Deep Link |  | Import a Deep Link from a file to jump directly to a specific moment in a saved dataset. More info here |
| Deep Link URL |  | Save or share these links for quick navigation or documentation. More info here |
  
#### Enabling Dataset

The command displays the following submenu:

Each command of the sub menu enables or disables the desired Dataset. Each dataset can be enabled or disabled with the following shortcuts: Alt + 1, Alt + 2, Alt + 3, Alt + 4, Alt + 5, Alt + 6, Alt + 7, Alt + 8, Alt + 9, Alt + 0.

####  Switch to working Dataset

Move the desired Dataset in the first slot (for this reason always grayed). The command displays the following submenu:

Each command of the sub menu move a Dataset.

Each dataset can be moved with the following keyboard shortcuts:

Ctrl \+ 1, Ctrl + 2, Ctrl + 3, Ctrl + 4, Ctrl + 5, Ctrl + 6, Ctrl + 7, Ctrl \+ 8, Ctrl + 9, Ctrl + 0

#### Manual Lap Definition

Allows splitting a Single Lap in two or more laps. The command is available only for Graph windows and is not available if an Append or a Comparison was loaded.

Selecting the command, on the active graph window the cursor changes. Now you can click on the window breaking the lap in the desired parts.

COMMANDS:

Move the cursor with the mouse or with the left and right arrow keys of your keyboard.

ENTER or LEFT CLICK ON THE MOUSE: to select the intervals (time or distance) on graph window.

END: to finish selection operation and confirm the splitting of the lap. At the end of the operation, WinTAX4 loads the first split lap.

ESC: abort split operation.

Important Note: The data will be changed permanently on the archive. Select the option "Keep a copy of the original lap in the archive" to keep a copy of the original file in the archive which will be renamed with the extension OLD

#### 

#### Split Laps with GPS

##### Configuring GPS channels:

This function allows the separation of data into individual laps based on GPS coordinates. It works by using two acquired channels, "Latitude" and "Longitude," and selecting a point on the graph or circuit that represents the "gps" finish line.

The channels used for recognizing Latitude and Longitude data are configured in the "General Setup" under the "Special Channels" tab in the Track Options section.

##### Configuration of the Start / Finish line

Using the dedicated command (available in the data menu) "Set GPS Start/Finish Line" you can choose a point on a GRA window or the Track window to set the finish line's position, which is then saved as a GPS location.

This operation generates a file in the System folder (GPS_FinishLine.xml) where the coordinates of the selected point are saved.

The lap division function searches the loaded data for coordinates closest to the configured one. To avoid identifying coordinates that do not represent the finish line, you can set a tolerance in meters, initially set to 20 meters and manually editable by editing the GPS_FinishLine.xml file (e.g., to exclude parts of the track that approach the finish line but are not the finish line).

Using the "Save GPS Start/Finish Line" command, you can save the file containing coordinates to any path with a specific name.

With the "Load GPS Start/Finish Line" command, you can load a previous coordinate configuration (which will update the GPS_FinishLine.xml file).

##### Manual division of laps:

The "Split Laps via GPS Start/Finish Line" command allows you to divide the loaded data based on the set GPS point.

After executing the command, the lap list in the Data Browser will be updated based on the laps that are detected.

The lap numbering after the split will follow the current Manual Lap Definition behaviour.

##### Automatically divide laps during Data Download

Activating the "GPS Split Laps" function in the Acquisition Manager under the Acquisition / RXTasks Setup menu, you can automatically divide the data into individual laps using GPS information during the download. Therefore, when downloading, for example, xx undivided laps, they can be divided according to the GPS rules described earlier.

#### Edit best lap settings:

This feature allows the definition of **Lap Markers** that should be considered when WinTAX evaluates the **Best Lap**.

By default, WinTAX uses a predefined set of markers, but if this function is enabled, _the selection can be customized based on user preferences._

##### **Best Lap selection logic** :

  * The lap with the **lowest time** is considered.

  * Only laps with **enabled markers** are taken into account. 




#### How to Use

  1. Open the **Data** menu and select **Edit Best Lap Setting**.
  2. A dialog will appear showing a list of all available Lap Markers.
  3. Use the checkboxes to select or deselect the markers to include in the Best Lap calculation.




You can use the following buttons:

  * **Select All** : Selects all available markers.
  * **Clear All** : Deselects all markers.
  * **Default** : Restores the default selection.



#### Playback

The playback feature allows the Real-Time simulation with Post Processing data. The simulation is performed on all the windows of the layout. When the simulation starts the cursor of the Graph window moves along the horizontal axis. All other windows display the current position of the cursor. The simulation can be done in a continuous loop and the cursor can move at different speeds.

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Start Play / Stop Play | F6 | Start and stop the playback function |
| Play Speed |  | Set the playback speed: x/32, x1/16, x1/8, x1/4, x1/2, x1, x2, x4, x8, x16, x32. |
| Rewind |  | Set the cursor to zero and restart the function of reproduction if already in play. |
| Loop |  | If loop is activated, playback restarts from zero when the simulation ends. |
  
#### Deep Links

The Deep Link feature allows you to save and share precise positions within a dataset using Bookmarks. Each Deep Link includes a time reference (in seconds), so that when reopened, WinTAX will position the cursor at the exact saved point in the PostProcessing window.

##### Export Bookmark Deep Link

This function allows saving a Deep Link from the current dataset and cursor position.

Steps:

  1. Open the **DataSet Panel** and right-click on the **Bookmarks** section, or use the toolbar / menu (**Tools** or **Data**).

  2. Select **Export DeepLink**.

  3. WinTAX will:

     * Copy the Bookmark and its time reference (in seconds) to the clipboard.

     * Add the Deep Link to the current Bookmark list.

     * Prompt to save the Deep Link as an XML file.




This allows you to share or store the exact position within the dataset for future access.

##### Import Bookmark Deep Link

This function lets you load a Deep Link from a previously saved file.

Steps:

  1. Open the **Import DeepLink** command from the same context menu or toolbar.

  2. Select the XML file containing one or more Deep Links.

  3. WinTAX will:

     * Add the Bookmark(s) to the current list.

     * Load the corresponding dataset.

     * Move the cursor to the exact saved time position in the PostProcessing window.




##### Using the deeplink:

In the Bookmarks section of the Dataset Panel, it is possible to double-click on entries marked with a star. This will automatically load the associated dataset and align the cursor to the position saved during the Deep Link creation.