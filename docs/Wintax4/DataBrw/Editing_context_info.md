## Edit Context Information

The Context information refer to the constants, meteo data, user record, offsets, gain etc.. those are associated to the ZTX files.

These information can be displayed or edited trough the Edit Lap Properties window opened by the commands of the Edit menu of the Data Browser and Edit Lap of WinTAX4.

  * If it is opened by Edit of the Data Browser, the information of the lap/laps selected are edited; if more laps are selected the possible changes are applied to all laps.

  * If it is opened by Edit of the Data Browser Edit Lap of WinTAX4 the loaded lap/laps are edited. If there are appends, the changes are applied only to the first lap.




The XML files containing the context information are automatically created during the acquisition and are linked both to race parameters and to the settings available in the [Acquisition Manager](../Acquisition_Manager/SESSION_STARTUP/AcqMng_Session_Startup.md). Manual change of the values must be carried out carefully because information on the lap structure is modified, even if sometimes it is better to redefine some values for instance the progressive number of laps.

The window of Edit Lap is represented in the following figure available in the Lap Header page.

The window is formed by the tree Lap Info gathering all the issues relating to the Context Information. The selection of one of the elements of the tree shows on the right of the window, the page of the issue chosen. In the figure the page selected is the Header.

###  Header

The Header page displays and changes the Abs, Run, Lap, TrackLap, LapTime and Lap Distance values. In addition a marker can be selected to be associated to one of the laps proposed by the combo. Some markers are automatically assigned to the data acquisition, however it could be better for analysis purposes to add some extra markers.

The marker proposed by the combo varies according to the licence; the most important ones are:

  * No Marker No marker

  * Lap Lap on track

  * Out Out lap (first in a run)

  * Box In lap (last in a run)

  * Test Test lap (vehicle stationary in pits)

  * EngineTest Engine test

  * Out \+ Box Out followed by In

  * Best Lap Best (fastest) lap

  * Trigger OUT Available only for telemetry data




Other markers:

  * Road

  * Special

  * Warm Up

  * Stop

  * Abort Out

  * Manual Trigger

  * Reset

  * Auto Limit

  * PitLane




A comment can be added or modified.

###  Run Header

In the Run Header page, the Header information is added at Run level. information are read only.

###  Session

The Session page displays and changes the basic information of the Session, i.e. Track, Session and Track Length. A comment can be added or modified.

###  Track

The Track page displays and changes the information of the track, i.e. Name and CRT related.

###  Driver

The Driver page displays and changes the information of the device, i.e. Driver, Device, Engine and Box Number. Comments linked to the driver or to the Run can be added or modified.

###  Constants

The Constants page displays, adds and modifies the values associated to constants that can be then used in post processing by WinTAX4 and displayed in the [Channel Browser](../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md) .

Two levels of constants are available in WinTAX4:

  * Lap Level: the constants of a lap are saved in the [Constants.xml](DataArchives_Full.md), file available inside a lap directory.



  * Run Level: the constants of a run are saved in the [RunConstants.xml](DataArchives_Full.md), file available inside a run directory.




The constants can then be associated both to a lap and to a run. If a lap has the same constants in both levels, the constant defined in the lap is taken as a reference.

The figure shows the editor page of the constants

The editor is formed by a three column list and by a series of commands that can be recalled from the toolbar options, the main menu and keyboard. In the list the first column contains the names of the constants, the second contains the lap values and the third contains the run values. According to the type of licence, the "Run" values column might be missing.

When the editor opens, the constants appear in alphabetical order, but the first to appear are Lap levels and the Run levels.

Each cell can be selected by clicking with the mouse or moving with the arrow keys on the keyboard.

Each cell can be modified by double clicking with the mouse or SPACE BAR on the selected cell. The user can exit from the edit of the cell through the ESC key (if no cell is edited, the ESC key allows exiting from the editor) or selecting with the mouse another area of the window or moving with the arrow keys on the keyboard.

The Options menu and the Options toolbar have the following commands

  * Add Item Adds a constant at the end of the list (also the Insert key on the keyboard can be used)

  * Remove Selected Item Removes the selected constant. The Delete key on the keyboard is used for the same purpose; however if the constant is not selected, but the lap value or the run value are selected, the Delete key does not remove the constant but eliminates the selected value.

  * Remove All Items Removes all constants from the list.

  * Move up Moves upwards in the list, by one position, the selected element. The position is not saved.

  * Move down Moves downwards in the list, by one position, the selected element. The position is not saved.

  * Sort by Name Sorts alphabetically all constants.




The constants can be brought from lap to run level and vice versa or to both levels by adding or removing a value in the corresponding cell of the list. When editing a lap a constant at run level is removed, this change is valid for all laps of the run.

###  Offset & Gain

The Offset & Gain page displays and modifies the offset and the gain values associated to the channels and that are introduced to make a pre-elaboration of the channel values. The offset values are algebraically summed to the channel values; the gain values are multiplied to the channel itself.

According to the configuration of [Setup/General](../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#OffsetGain) the offset and the gain applied to a channel can be the same as in context info, those configured in [Channel Parameters](../WTXSetup/WndChannelsParametersSetup/Channels_Parameters.md) or both. The principle is that offsets are always values that must be algebraically summed while gain values are multiplication factors.

As for constants, WinTAX4 manages two level of Offset & Gain: Lap Level and Run Level.

Offset and gain of a lap are saved on [Offsets.xml](DataArchives_Full.md) files available in a lap directory and on the [RunOffsets.xml](DataArchives_Full.md) file available in the Run. Offset and gain can therefore be associated both at lap and run levels. If a lap has offset and gain defined at both levels, the ones defined at lap level are used.

The figure shows the offset and gain editor page.

The editor is formed by a five column list and by a series of commands that can be recalled from the options toolbar, the main menu and the keyboard.

The list includes all channels of the lap; the channel cannot be manually added or deleted; when a channel has no Offset & Gain, i.e. when offset is 0 and gain is 1, the list shows the channel in grey; when the offset is not 0 or gain not 1, the background of the channel is in white.

When the editor opens, the channel are sorted as in the [Logging Table](../WTXWorkWindows/WndLoggingTable/WndLoggingTable.md); the cells with the names of the channels cannot be edited.

Each cell can be selected by clicking with the mouse or moving with the arrow keys on the keyboard.

Each cell, excluded those with the names of the channels, can be modified by double clicking with the mouse or with the SPACE BAR on the selected cell. The user can exit from cell editing mode with the ESC key (if no cell is edited, the ESC key will close the editor) or selecting with the mouse another area of the window or moving with the arrow key on the keyboard.

The Options menu and the Options toolbar have the following commands

  * Remove Selected Item Removes the offset and gain from the selected channel by setting the default values of offset = 0, gain = 1. The Delete key on the keyboard is used for the same purpose; if however just one value and not the name of the channel is selected, the Delete key adds the default value only in the selected cell.

  * Remove All Items Removes all constants from the list.

  * Move up Moves upwards in the list by one position the selected element. The position is not saved.

  * Move down Moves downwards in the list by one position the selected element. The position is not saved.

  * Sort by Name Sorts alphabetically all channels.

  * Sort by Configured Sorts alphabetically all channels dividing them into two groups: first those with offset and gain different from default than all the others.




###  User Record

The User Record page shows, adds and modifies values associated to alphanumeric constants that can be later used in post processing by WinTAX4, for instance in the [Data Header](../WTXSetup/WndSetupDataHeader/WTXSetupDataHeader.md#WndDataHeaders) .

As for constants and Offset & Gain, WinTAX4 manages two level of User Record: Lap Level and Run Level.

The user records associated to a lap are saved on the [UserRecords.xml](DataArchives_Full.md) files available in a lap directory and on a [RunUserRecords.xml](DataArchives_Full.md) file available at Run level. The user records can then be associated both at lap and at run level. If a lap has the same user record defined at both levels, the user record defined at lap level is used.

The figure shows the editor page of the user record.

The editor is formed by a three column list and by a series of commands that can be recalled by the options toolbar, the main menu and the keyboard.

When the editor opens, the user records appear in alphabetical order, but the first to appear are Lap levels and Run levels.

Each cell can be selected by clicking with the mouse or moving with the arrow keys on the keyboard.

Each cell can be modified by double clicking with the mouse or with the SPACE BAR on the selected cell. The user can exit from editing the cell through the ESC key (if no cell is edited, the ESC key quits the editor) or selecting with the mouse another area of the window or pressing the arrow keys on the keyboard. 

The Options menu and the Options toolbar have the following commands

  * Add Item Adds a user record at the end of the list (also the Insert key on the keyboard can be used)

  * Remove Selected Item Removes the user record selected. The Delete key on the keyboard is used for the same purpose; however if the user record is not selected but the lap value or the run value are, the Delete key does not remove the constant but eliminates the selected value.

  * Remove All Items Removes all user records from the list.

  * Move up Moves upwards in the list by one position the selected element. The position is not saved.

  * Move down Moves downwards in the list by one position the selected element. The position is not saved.

  * Sort by Name Sorts alphabetically all user records.




The user records can be brought from lap to run level or vice versa or to both levels by adding or removing a value from the corresponding cell of the list. When editing a lap, a user record is removed at run level, this change is valid for all the laps of the run.

Examples: [User Record In VCH](../Sample/UserRecordInVCH.md); [User Record In Lookups](../Sample/UserRecordInLookups.md)

###  Meteo

The Meteo page shows and changes meteo data that can later be used in post processing by WinTAX4, as meteo information is part of the Info list of the [Channel Browser](../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md). Meteo data is saved on the [Meteo.xml](DataArchives_Full.md) files available in the directories containing the laps.

The figure shows meteo data editor page 

The editor is formed by a two column list where the first column contains the names of the meteo data while the second contains the values.

Each cell can be selected by clicking with the mouse or moving with the arrow key on the keyboard.

Each cell can be modified by double clicking with the mouse or SPACE BAR on the selected cell. The user can exit from the edit of the cell through the ESC key (if no cell is edited, the ESC key allows exiting from the editor) or selecting with the mouse another area of the window or moving with the arrow keys on the keyboard. The names of the meteo data cannot be modified.

###  Menu

The menu of the window allows the access to the following commands, divided into sub menus:

#### File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | Enter | Applies the current settings of the window. |
| Cancel | Esc | Closes the window without applying the current settings. |
| Load |  | Opens and loads an xml file containing the previously saved values of constants, offset and gain and user record. |
| Save |  | Enabled with Constants, Offset & Gain and User Record. It allows saving on an xml file the data available in the list. |
| Update Lock |  | Manual Refresh of the lock state in case the editor opened in read-only mode due to simultaneous editing of the same information. When simultaneous editing is no longer available, the window is removed from the read-only mode. |
  
#### Load

The function is enabled with Constants, Offset & Gain and User Record. It allows to open and load an xml file containing the previously saved values of constants, offset and gain and user record. As the xml file does not include information about the lap or run levels, before loading it, a dialog window opens asking at which level the information must be added.

### 

### Toolbar

The toolbar of the window allows the access to the same commands of menu.

### File locking window

If the editor window is opened on context information that are currently locked, a warning message is displayed informing about the locking application and user name.

The operator is prompted either to continue in read-only mode or to exit the editor. In read-only mode, it is possible to manually refresh the locking status via the File/Update Lock command.

In case of deadlock problems caused by crashed applications, locked files can be freed by selecting the command Edit/Clear Lock in the DataBrowser. This command forces a removal of the lock file in selected run, and it is available only for users with Super level.

TelDataX also manages Context Information Locking: please refer to the reference document for further details.