## Windows Scripting

In many of WinTAX4 windows are given the chance to set scripts. The Configuration of each windows allows to write them in the Script Page. 

The configuration allows you to assign scripts to global events (WinTAX4) and local events (the window). When one of these events occurs, the window in which the script is set, runs the script. The global events are named Application, instead the local are named Window.

Below there is a brief description of the Application and Window events. The full documentation is provided in the [Automation Interfaces](WinTAX4AutomationInterfaces.chm::/index.md) technical reference.

#### Application Events

OnCreate: The OnCreate method receives an event notice indicating that WinTAX4 application has been created.

OnExitApplication: The OnExitApplication method receives an event notice indicating that WinTAX4 is going to close. This event is fired after the user has confirmed to close WinTAX4 and immediately before to destroy all internal windows and objects.

OnLoadData: The OnLoadData method receives an event notice indicating that new data has been loaded in the current dataset.

OnSettingChange: The OnSettingChange method receives an event notice indicating that application settings are changed.

OnNewDataRealTime: The OnNewDataRealTime method receives an event notice indicating that new real time samples have been acquired.

OnNewLap: The OnNewLap method receives an event notice indicating that a new lap has been closed by an acquisition process.

OnAutoRX: The OnAutoRX method receives an event notice indicating that a new lap has been closed by an acquisition process and opened by WinTAX4.

OnAutoRXRemote: The OnAutoRXRemote method receives an event notice indicating that a new lap has been opened by WinTAX4 Auto RX Remote function.

OnAutoImportLap: The OnAutoImportLap method receives an event notice indicating that a new lap has been opened by WinTAX4 Auto Import Lap function.

OnAutoCopyToHost: The OnAutoCopyToHost method receives an event notice indicating that a 'Copy to Host' operation has been completed.

OnAutoAlarmFound: The OnAutoAlarmFound method receives an event notice indicating that an alarm was found during the 'Auto Search Alarms' operation.

OnEvent: The OnEvent method receives an event notice indicating that an event occurred.

OnNewLoggingTable: The OnNewLoggingTable method receives an event notice indicating that Real Time logging table is change.

OnBeginUpdate: The OnBeginUpdate method receives an event notice indicating that WinTAX4 is about to update windows ( e.g. with a new set of data or new setup ).

OnEndUpdate: The OnEndUpdate method receives an event notice indicating that WinTAX4 has finished to update windows ( e.g. with a new set of data or new setup ). **OnDownloadEnd** : The OnDownloadEnd method receives an event notice indicating that the download of data is completed.

#### Window Events

OnCreate: The OnCreate method receives an event notice indicating that current window has been created.

OnClose: The OnClose method receives an event notice indicating that current window has been closed.

OnChange: The OnChange method receives an event notice indicating that current window configuration has been changed.

OnCursorPosChange: The OnCursorPosChange method receives an event notice indicating that current window cursor has been moved.

OnZoomChange: The OnZoomChange method receives an event notice indicating that current window zoom has been changed.

OnSettingChange: The OnSettingChange method receives an event notice indicating that application settings are changed

OnDataRefreshStarted: The OnDataRefreshStarted method receives an event notice indicating that current window is about to calculate a new set of results.

OnDataRefreshFinished: The OnDataRefreshFinished method receives an event notice indicating that current window has just finished to calculate a new set of results.

OnLoadData: The OnLoadData method receives an event notice indicating that current window has just loaded new data to calculate a new set of results.

#### Configuration Script Page

The Script page enables to configure scripts linked to the events of the related window or of the application, in VBScript or JScript.

The section on the left displays the list of the functions available, grouped for Application and Window. The section on the right displays the code of the set functions: in this section the script can be edited.

####  Menu

The window configuration menu allows access to the following commands, divided in sub menus:

##### File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the current settings. |
| Cancel |  | Closes the window without applying the current settings. |
| Load |  | Opens a dialog window to select a configuration file for the window to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file for the window to save the current settings. |
  
##### Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl \+ X | Copies to clipboard the text selected in the editor and removes it from the script. |
| Copy | Ctrl \+ C | Copies to clipboard the text selected in the editor. |
| Paste | Ctrl \+ V | Pastes the text available in clipboard adding them to the script in the current position of the cursor. |
  
##### View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use |
  
##### 

####  Toolbar

The windows Configuration toolbar allows access to the following commands:

| DESCRIPTION | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode allowing to keep displayed the configuration window while the application window are in use |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel command of the File menu) |
| Apply | Applies the current settings to the configuration of the graphic window (similar to the Apply command of the File menu) |
| Load | Is equal to the Load command of the File menu |
| Save As | Is equal to the Save As command of the File menu |
| Previous page | Is equal to the Previous page command of the View menu |
| Next page | Is equal to the Next page command of the View menu |
| Cut | Is equal to the Cut command of the Edit menu |
| Copy | Is equal to the Copy command of the Edit menu |
| Paste | Is equal to the Paste command of the Edit menu |
  
## 

#### 