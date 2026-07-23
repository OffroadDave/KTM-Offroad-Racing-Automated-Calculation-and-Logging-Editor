## Tools

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Track Editor | Alt \+ T | Opens the [Track Editor](../Tools/TrackEditor/Track_Editor.md) environment. |
| Calculate Track | Ctrl \+ O | Carries out the [Generate Track](../Tools/TrackEditor/GenerateTrackSections.md) command in track configuration on [Track Editor](../Tools/TrackEditor/Track_Editor.md) |
| Variance | Alt \+ V | Enables the [Variance](../AnalysisWnds/Wnds/GRA/GRAWndFeatures.md#GRAFeatureVariance) channel |
| Virtual Channels | V | Opens the [Virtual channels](../RuntimeAnalysis/VCH/VCH_libraries.md) editor |
| Lookups |  | Opens the [Lookups](../RuntimeAnalysis/VCH/Functions/VCH_functions.md#vch_Lookup1) editor |
| Conditions |  | Opens the [Conditions](../RuntimeAnalysis/Conditions/Conditions.md) editor |
| Events |  | Opens the Events submenu. |
| Scripts |  | Opens the [Scripts](../Scripting/Scripts_Env.md) editor |
| Alarms |  | Opens the Alarms submenu to manage Alarms. |
| Report Manager |  | Opens the [Report Manager](../Tools/Report_Manager/PitSystem.md) environment |
| Gear Change |  | Opens the Gear Change submenu. |
| Make Init File |  | Generates Init File |
| Delete Init File |  | Removes Init File |
| Run |  | Open the Run submenu. |
  
#### Events submenu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Configure... |  | Link to [Event](../RuntimeAnalysis/Events/Event_setup.md) editor |
| Default Settings... |  | Opens the configuration window of the default graphic settings and of the [Search Event](../WTXWorkWindows/WndSearchEvents/WndSearchEvents.md). Each new event inherits the configuration set in this page. The configuration window is similar to the [configuration page](../RuntimeAnalysis/Events/Event_setup.md) of each event. |
| Enable / Disable Events | Ctrl + E | Enables visualization of events in all windows. |
| Show Events | Alt + Ctrl \+ E | Enables visualization of events in the active window. |
| Jump to Auto Event | Alt + J | Shifts the cursor of the Graphs window to the first occurrence of the event configured as Search Event |
| First Occurrence | Alt + Up | Moves the cursor of the Graphs window to the first occurrence of the [Hot Event](../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#ChannelBrowserEvents). |
| Last Occurrence | Alt \+ Down | Moves the cursor of the Graphs window to the last occurrence of the [Hot Event](../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#ChannelBrowserEvents). |
| Previous Occurrence | Alt + P | Moves the cursor of the Graphs window to the first occurrence of the previous [Hot Event](../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#ChannelBrowserEvents). |
| Next Occurrence | Alt + N | Moves the cursor of the Graphs window to the first occurrence of the next [Hot Event](../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#ChannelBrowserEvents). |
| Move Left to Previous | Alt + Ctrl \+ P | Moves the cursor of the Graphs window to the first occurrence of the previous |
| Move Right to Next | Alt + Ctrl \+ N | Moves the cursor of the Graphs window to the first occurrence of the next |
| Next HotEvent | Ctrl + N | Sets as [Hot Event](../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#ChannelBrowserEvents) the next event in the list Events of the Channel Browser |
| Previous HotEvent | Ctrl \+ P | Sets as [Hot Event](../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#ChannelBrowserEvents) the previous event in the list Events of the Channel Browser |
  
#### 

####  Alarms

Auto search alarms setup for post processing and real time analysis.

##### Edit

Allows the configuration of the "Search Alarms" function.

The dialog has two sections, Post Processing and Real Time. 

Post Processing section

In this section the user can configure the different display mode of the[ Alarms Window](../AnalysisWnds/Wnds/ALARMS/ALARMSWndOverview.md). The Alarm Window performs automatic search of alarms whenever a new lap is loaded or when an [Auto Rx](../Acquisition_Manager/Rx_tasks/Rx_tasks.md#AutoRx) task is received.

Default: Alarms = OFF and All = OFF

The default mode, without check selected, updates Alarm Window only if window is already open. 

Alarms: Alarms = ON and All = OFF

If "Alarms" is the only check selected, the Alarm window is automatically displayed every time a channel exceeds the alarm configured thresholds. If the window was already open, it updates the values. If there are no alarms the window is automatically closed or not opened.

All: Alarms = ON and All = ON

If both "Alarms" and "All" checks are selected, the window is opened (or updated if already open) and shows all configured alarm channels even if the alarm has not been triggered..

Real Time section

In this section the user can enable alarms visualization during real time analysis using [Event Report window](../AnalysisWnds/Wnds/EVR/EVRWndOverview.md) and [Popup Alarms](../WTXWorkWindows/PopupAlarms/PopupAlarms.md).

Alarms = ON and Invalid Value = OFF

If Alarm check is enabled, during real time analysis, WinTAX4 shows alarms through a popup or into the Event Report window basing of [channel parameter](../WTXSetup/WndChannelsParametersSetup/Channels_Parameters.md#Alarm) configuration. 

Alarms = ON and Invalid Value = ON

If Invalid Value check is enabled, Popup Alarm and Event Report window highlight with yellow color the alarms outside Invalid Range. 

(see [channel parameter](../WTXSetup/WndChannelsParametersSetup/Channels_Parameters.md#Alarm) configuration).

##### View (Ctrl + W)

The Real time Alarms command opens the alarms list configured in [channel parameter](../WTXSetup/WndChannelsParametersSetup/Channels_Parameters.md#Alarm) setup. This list is referred only to Popup Alarms and Event Report window during real time analysis, and permits to hide or reactivate each single alarm without disabling it from configuration. it is related with the [Hide button](../WTXWorkWindows/PopupAlarms/PopupAlarms.md#HideAlarms) in Popup Alarms.

####  Gear Change

This feature allows User to search for Gear Changes occurrences into Graph windows.

Gear Change Event needs the Gear channel to be configured in General Setup\Special Channels before using it

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Next Gear Change | Alt \+ Ctrl + Right | Moves the cursor from the current position to the nearest next gear occurrence. |
| Previous Gear Change | Alt + Ctrl + Left | Moves the cursor from the current position to the nearest previous gear occurrence. |
| First Gear Change | Alt \+ Ctrl + Up | Moves the cursor to the first gear occurrence |
| Last Gear Change | Alt \+ Ctrl + Down | Moves the cursor to the last gear occurrence |
  
#### Init File

The Init File (WTXInitFile.xml) is a startup initialization configuration file and contains settings of General Setup, Acquisition Setup and RxTasks. The file is generated into WinTAX4\System directory by Make Init File command.

At startup, WinTAX4 automatically checks for the "WTXInitFile.xml"; every time the program is started, if the file is found, WinTAX4 starts using the configuration read from this file. Every modification made by the user will be saved with standard logic (in system xml files) when closing the application; the WTXInitFile.xml file is re-created only using the command Make Init File.

After the first startup using the file, user should delete it with "Delete Init File" command, in order to avoid undesired effects due to automatic searching of WTXInitFile.xml file at every WinTAX4 startup.

####  Run

This feature allows to execute some external application from WinTAX4.

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Calculator | Shift \+ A | Execute windows calculator |
| Notepad | Shift \+ O | Execute Notepad editor |
| Excel | Shift \+ E | Execute Microsoft Office Excel |
| Generic application | Shift \+ G | Open an application browser to select and execute the desired application. |