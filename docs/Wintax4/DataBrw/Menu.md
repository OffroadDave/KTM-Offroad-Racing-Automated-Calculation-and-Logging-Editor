## Commands

The Data Browser includes the Files, Edit, View, Tools menus and a toolbar by default. Here is a description of the commands.

### Files Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Open | Enter | Allows opening one or multiple laps loading them either in WinTAX4, or on the DataSet Panel or on the Bookmark according to the request. Read [Selection and loading of laps](SelectLoadLap.md) to see different ways of selecting them. |
| Open in Setup Editor | Ctrl \+ F10 | It opens the lap information context editor. See the paragraph: [Setup Editor](../SetupEditor/Setup_Editor.md) |
| Open in temp mode |  | Opens the lap or the selected laps in a TaxView window that cannot be modified. All changes that are done to a TaxView window will not be saved and the previous state of the window will be restored to its original state the next time the program is started. |
| Load Best Lap | Ctrl \+ B | Loads on WinTAX4 the Best Lap, i.e. the lap identified by the Data Browser as the fastest one. Not all laps can be selected to be the fastest one, but only those identified by the following [Lap Marker](Editing_context_info.md#Header): N or Marker, Lap, Best Lap. |
| Delete Lap | Delete | It deletes the selected laps. The file on disk is cancelled therefore a confirmation is required. The cancellation is extended to the directories and xml files associated to the lap. If all laps of the session are cancelled, the session itself will no longer be displayed in the list of sessions. |
| Remove Run | Ctrl \+ Delete | Removes all laps of the selected run. The file on disk are cancelled therefore a confirmation is required. The cancellation is extended to the directories and xml files associated to the laps. If all laps of the session are cancelled, the session itself will no longer be displayed in the list of sessions. |
| Rename Run |  | Change the Run Name of selected run. |
| Append Laps |  | This command enables to link two or more laps so that a single lap is obtained. A confirmation is required to carry out the operation. The resultant lap will have the Lap Time equal to the sum of the linked laps. If laps have different frequencies, situation is present only if the licence has the double frequencies management (Cable A and Cable B), all channels will be converted to higher frequency of the two tables with lowest common multiple logic. |
| Append Automatic Laps |  | This command appends all laps consecutive of the same kind in a unique lap, until a frequency change, changing of frequencies are present only if the licence has the double frequencies management (Cable A and Cable B). A confirmation is required to carry out the operation. The resultant lap will have the Lap Time equal to the sum of the linked laps. |
| Run Selection Mode | Ctrl \+ R | Enables/Disable run selection. The Run Selection mode permits to load the entire run in append even if only one lap is selected. |
| Enable Merge Import Data |  | Permits to merge [import data from ASCII](ImportAscii.md). |
| Enable Video Loading |  | Enables/Disables loading of video. |
| Browse Directory |  | It opens a browser to select the main directory where sessions can be searched and displayed. The selected directory becomes the directory selected also in the Select Archive combo and it is the one from which the Session list will display the sessions, if available. |
| Clear Browse Directory |  | It cancels all paths of the Select Archive combo with the exception of those configured on WinTAX4 in Setup/General/Directories/Data. |
| Add to Append |  | This command appends all currently selected laps to the first Data Set in compare. |
| Add to Comparison | Ctrl \+ Num Enter | It adds the selected laps to the existing list of comparisons displayed in the [Lap Info Bar](Header_Lap.md) |
| Lap Comparison |  | When this check is selected, the double click on a lap adds the lap to the comparison list displayed in the [Lap Info Bar](Header_Lap.md); if it is not selected, the double click loads the lap on WinTAX4. |
| Clear Comparison |  | It removes all laps from the [Lap Info Bar](Header_Lap.md) removing them also from WinTAX4 and from DataSet panel when available. |
| Auto Clear Laps Comparison |  | When checked, cancels the old laps comparison before a new lap selection. If unchecked, the new laps selection are added at the old laps comparison. |
| Add to Bookmark |  | It adds a lap to the [Bookmark](../WTXWorkWindows/WndDataSet/DataSet.md#BookmarkList) list of the DataSet panel |
| Add to Average Comparison | Alt + Num Enter | Adds the selected lap to the current average comparison |
| Distance Avg comparison mode |  | Selects the distance mode to calculate average. |
| Add to DataSet #N | Ctrl \+ 1  Ctrl + 2  ...  Ctrl + 9  Ctrl + 0 | It adds the selected laps to the comparison list displayed in the [Lap Info Bar](Header_Lap.md) but in the slot identified by the number of chosen dataset. |
| Load comparison to average Dataset | Alt \+ 1  Alt + 2  ... Alt + 9  Alt + 0 | Load the selected Dataset to average Comparison |
| Faster Archives |  |  |

  * Enable Faster Archives enables a faster search of the data if xml archives have been created.
  * Make Faster Archives It creates an xml file that contains the basic information about the sessions viewed and that allows a quicker display of the sessions if the Enable Faster Archives command is enabled. If the scanned directory is WinTAX4/WtxData, the WtxData.xml file is created in the directory itself. If the scanned directory is WinTAX4/ or higher level, the WtxData.xml file is also created in WinTAX4/WtxData directory. in fact the multi level search allows creating the caches of all major sub-directories.

When the data path is selected, the sessions will be loaded by the xml file; the icon in data combo associated to the path changes indicates the different loading. The xml file is updated with the modifications of the data structure.

  * The Esc command blocks the creation of Faster Archives; it is useful when the creation takes a long time.

  
Set Reference Lap |  | It sets the [Reference Lap](Reference_Lap.md) to make comparisons with a fixed reference lap.  
Remove Reference Lap |  | It removes the [Reference Lap](Reference_Lap.md)  
Enable automatic Scan | CTRL+E | Allows browsing laps and run folders without actually loading preview data. The same function can also be added on the main toolbars. On new install it will be available by default. If you are updating from a previous version you will need to add a custom button, by right clicking on the toolbar and choosing customize.  
Stop Scanning | Esc  | It blocks the scanning that the Data Browser carries out on the directory set to search the scans; it is useful when the scanning lasts for a long time. For further details see [Directory Tree](DataBrowserOptimization.md#StopBrowsing). The command Esc is also used to stop the creation of Faster Archives.  
File Info Version |  | When data has been acquired or created using a supported version, this function will visualize information on the version of WinTAX that has generated this data.  
| Exit |  | It closes the Data Browser without loading the data in WinTAX4 |
  
## 

###  Edit Menu

## 

| COMMAND | DESCRIPTION |
| --- | --- |
| Header | It opens Edit Lap Information on the Header page. See the paragraph: [Edit Context Information](Editing_context_info.md) |
| Run Header | It opens Edit Lap Information on the Run Header page. See the paragraph: [Edit Context Information](Editing_context_info.md) |
| Session | It opens Edit Lap Information on the Session page. See the paragraph: [Edit Context Information](Editing_context_info.md) |
| Track | It opens Edit Lap Information on the Track page. See the paragraph: [Edit Context Information](Editing_context_info.md) |
| Driver | It opens Edit Lap Information on the Driver page. See the paragraph: [Edit Context Information](Editing_context_info.md) |
| Constants | It opens Edit Lap Information on the Constants page. See the paragraph: [Edit Context Information](Editing_context_info.md) |
| Offset & Gain | It opens Edit Lap Information on the Offset & Gain page. See the paragraph: [Edit Context Information](Editing_context_info.md) |
| User Record | It opens Edit Lap Information on the User Record page. See the paragraph: [Edit Context Information](Editing_context_info.md) |
| Meteo | It opens Edit Lap Information on the Meteo page. See the paragraph: [Edit Context Information](Editing_context_info.md) |
| Edit Lap Properties | It opens the editor of context information the lap. See the paragraph: [Edit Context Information](Editing_context_info.md) |
| ClearLock | This command is available only for users with Super level; it forces the removal of a lock from a file that was left in a locked status. For further information about the locks see the paragraph: [File Locking](Editing_context_info.md#FileLocking) |
  
## 

###  View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Directory Tree | Ctrl \+ T | It displays the Directory Tree for the selection mode of the main directory alternated to the combo. For further information about the locks see [Directory Tree](DataBrowserOptimization.md) . |
| Track Preview | Ctrl \+ Q | Shows/Hides track preview window. |
| Arrange Session |  | Opens the Arrange Session submenu. |
| Refresh | F5 | It scans the sessions |
| Filters |  |  |

  * Session Filters Setup it opens or closes the panel to configure the session filters.
  * Clear All Filters All filters of the session are reset

For further details on the session filters see the paragraph: [Configuration of the Session Filters](Filters.md#Session).  
Lap Filters Setup |  | It opens the [configuration page of the Lap Filters](Filters.md#Lap).  
Hide Non-standard Laps |  | Hides spurious archive laps: duplicated (~), auto limit (@), trigger out (#). Hidden laps are not used at all in WinTAX4.  
Show Best Lap |  | Shows fastest lap of the whole session marking it with a “golden cup” icon  
Show Best Lap in Run |  | Shows fastest lap of each run of the session marking them with a “silver cup” icon  
Selection Panel |  | This command enables and disables the selection panel columns in the lap list.  
| Data Header |  | It opens the [configuration page of the Header](Options_Configuration.md). |
  
### 

#### Arrange Session

  * By ... It allows to alphabetically arrange the sessions according to the information configured in the [Header Session](Options_Configuration.md#Session).

  * Auto Arrange predefined arrangement of the sessions. The alphabetical arrangement is set according to the first column of the [Header Session](Options_Configuration.md#Session) .

  * Advanced Sort The following window opens where a customized arrangement can be set




### 

###  Tools Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Select Best Lap | It selects the best lap without loading it on WinTAX4. |
| Import | Opens the Import submenu. |
| Export | Opens the Export submenu |
| Make Faster Files | It creates for each selected channel a Faster File for each channel of the lap and it includes the .ZTX file of the lap. A Faster File contains all the samples of a single channel. The unzipping of the Faster File allows a quicker access to the data instead of a bigger .ZTX file. |
| Make Event Logs | It creates an [Event Log](../RuntimeAnalysis/Events/Events.md#MakeEventLog) in XML format based on the libraries of [Events](../RuntimeAnalysis/Events/Events.md) currently loaded in WinTAX4. The log file created can be processed by external applications. |
| Copy to Host | Copy to Host is one of the instruments that allows using WinTAX4 in network; the command enables to data downloaded via cable on a PC master, to be copied on other stations. In this way all stations are updated with the latest data. To configure the hosts where the files must be copied, go to WinTAX4 in Setup/General/Directories and configure at least one directory in host. |
| Customize Toolbars | It opens the configuration window of the toolbars |
| Customize Accelerators | It opens the configuration window of the accelerators; for the functioning, see the configuration window [of the accelerators](../WTXSetup/WndSetupAccelerators/WTXSetupAccelerators.md) of WinTAX4. |
  
####  Import

  * Import From Exp It allows selecting an EXP file and unfold it in the directory chosen by the user. For further details see the page: [Import from EXP](Import_from_EXP.md).

  * Import From ASCII The import from ASCII function is used to convert general text files into WinTAX4 data structures (therefore in native ZTX format). For further details see the page: [Import from ASCII](ImportAscii.md)

  * Import From Matlab WinTAX4 is able to import files from Matlab. For further details see the page: [Import from Matlab](ImportMatLab.md)

  * Import From Video It allows importing file video into RUN. For further details see the page: [Import from Video](Import_from_Video.md)

  * Import From MDF It allows importing files from MDF format (Measurement Data Format) to WinTAX telemetry data files. For further details see the page: Import from MDF.




#### Export

  * Export to BIN It creates a [BIN](../ExportFormats/binary_file_format.md) file containing all the channels selected. For further details see the page: [Export](Export.md)

  * Export to ASCII It creates an ASCII file (tab separated values, *.prn) containing all values of selected channels. For further details see the page: [Export](Export.md)

  * Export to CSV It creates a CSV file (comma separated values), containing all values of selected channels. For further details see the page: [Export](Export.md)

  * Export to XLS It creates a XLS file (Excel sheet) containing the values of the selected channels. For further details see the page: [Export](Export.md)

  * Export to ZTX It opens a directory selection window where the directory of the selected lap will be copied with all its content.




  * Export Channels to ZTX It creates a [BIN](../ExportFormats/binary_file_format.md) file containing all the channels selected. For further details see the page: [Export](Export.md)

  * Export to EXP It opens a path selection window of the file where it can be indicated where the EXP file will be created. The EXP file is a file compressed which contains not only all data of the lap, but also the path needed to recreate it in the structure Track / Session / Device / Run / Lap.

  * Export Channels to EXP It creates an EXP file containing all the channels selected. For further details see the page: [Export](Export.md)

  * Export to Matlab It creates a matlab file containing all the channels selected. For further details see the page: [Export](Export.md)




###  Toolbars

The Data Browser has by default just one toolbar with some predefined buttons, the Main toolbar. The Main toolbar can be modified, so buttons can be added or removed. New toolbars can also be created that will be customized.

To view the toolbars just click with the right button on the toolbars area and select which toolbar must be shown or hidden.

Each toolbar can be floating or can be docked to the main window; the toolbars can be docked on each of the four sides of the window. To dock a toolbar, drag it on the desired area of the window or double click on the header bar. In this latter case the toolbar will be placed on the last docked zone. Vice versa to make a docked toolbar floating, double click on the outer borders of the toolbar or drag it with the mouse outside the dock area.

The configurable toolbars have a downwards arrow on the right side. This arrow corresponds to a button that opens a menu through which some predefined commands can be added or removed from the toolbar.

Using the Customize menu from the right button on the toolbar (which is just like the command of the main menu Tools/Customize Toolbars ) it is possible to open [the configuration window of the toolbars](../WTXSetup/WndSetupToolbars/WTXSetupToolbars.md) through which all commands available on Data Browser can be added or removed from the toolbars.

For further information about the description of the functions of the single commands of the toolbar, please read the corresponding menus.

Only two combo boxes can be displayed on the toolbars: the Select Archive and Select Lap Filter combo boxes.

### Keyboard Shortcut

To see the complete list of shortcuts available for the Data Browser, click [here](../Shortcut/DataBrowserShortcut.md).

Below are listed those shortcuts that do not appear in any of the commands and of the toolbar buttons described before. These commands can be executed only from keyboard. All these commands are not customizable.

| SHORTCUT | DESCRIPTION |
| --- | --- |
| F1 | Display Data Browser Help |
| R | Select all the laps of a run |
|  |  |
  
###  Select Archive

The combo box is the starting point to create the list of the sessions. The directory that the user will choose here is scanned to search for available sessions. The combo box contains some default paths in WinTAX4 in [Setup/General/Directories/Data](../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgDirectories). Other 10 folder paths will be automatically saved and stored during usage of the Browse Directory command.

The scan starts as soon as the directory is selected. For the same operations you can also choose the [Directory Tree](DataBrowserOptimization.md). 

###  Select Lap Filter

The combo enables on the selected session one of the filter predefined by WinTAX4 or of those configured in the [configuration page of the Lap Filters](Filters.md#Lap) where the Lap Filters are further described. To avoid any filter on the session lap, select the All filter.

###  Pop-up Menu

Both the [Sessions List](Session_Laps.md#ListaSession) and the [Laps List](Session_Laps.md#ListaLap) allow the access to a pop-up menu with the right button of the mouse. All commands of the menu have already been described in the main Menus.

The only command not available in the menu is the Select Run menu on the lap list that allows to select the whole run of the selected lap.