Report Manager is a tool for exporting data in binary format for [MPS4](../../ExternalComponents/MPS.md) or in TXT, CSV, XLS format for possible further use. The configuration environment can be opened from the main menu through Tools/Report Manager. The report manager is divided into three sections: MPS4 Export, Auto-Export and Options.

### MPS4 Export

The exported data for MPS4 can be:

  * Real time: Data is extracted from the real time stream during acquisition and exported

  * Post Processing: When a lap is closed (real time acquisition) or when a new lap is acquired (cable acquisition) the data is read




MPS4 Export is divided into three tabs, RT Channels , Report, Trend .

#### RT Channels

This tab allows configuring the channels to be used in real time exporting. For each channel it is also possible to set an alarm field.

## 

  * Name: Name of the channel

  * Alarm: Enables/disables the alarms management

  * Min/Max: Limits of the alarm status




The channels can be added, cancelled or their position can be modified using the "Options" toolbar buttons placed below the channels list. The channels configured in this page will be exported during the acquisition and read by the [MPS4](../../ExternalComponents/MPS.md) system, if in the Options page the export is enabled.

#### Report

Page to select the Lap Report tab configuration to be exported; it is used with the post processing exporting.

The Options toolbar includes three buttons: the first opens a browser to select the report; the second cancels the report, the third opens the configuration of the selected Lap Report..

The report configured in this page will be exported whenever the lap is closed (AutoRx) and read by the [MPS4](../../ExternalComponents/MPS.md) system, if in the Options page the export is enabled.

#### Trend

Page to select the Trend configurations to be exported; it is used with the post processing.

The Options toolbar includes five buttons: the first opens a browser to add a trend; the second allows to replace the selected report; the third cancels the report in the list; the fourth cancels them all; the fifth opens the configuration of the selected trend.

Up to 4 Trend configurations can be set.

The trends configured in this page will be exported whenever the lap is closed (AutoRx) and read by [MPS4](../../ExternalComponents/MPS.md) system, if in the Options page the export is enabled.

### Auto Export

In this page it's possible to configure folder location and file format for the report to be exported.

Report Configuration: Through the commands of the Options toolbar a report can be added, modified, cancelled and edited. The report available in the combo is the report that is exported when the lap is closed.

Output File: focusing on this text box only the selection file button is enabled allowing to select path and format of the output file TXT, CSV, XLS format.

If the format is xls, a configuration form of the output Excel file is displayed, as in the previous window.

  * External Sheet (dynamic cell copy) This option prevents WinTAX4 to reset the XLS file. If this option is set, WinTAX4 does not create a new XLS file when the report/session is reset, but it works always on an existing worksheet. Therefore, the export will fail if the XLS file is not found. In case of error, an error message is displayed in the History message window. The user must set this option when WinTAX4 is assumed to export information on an existing worksheet that already contains cell formatting or macros. In this case WinTAX4 will copy the single values to the corresponding cells by using Excel Automation. Only the cells corresponding to new report rows are overwritten.

  * Export to Sheet This option is enabled only when the External Sheet (dynamic cell copy) checkbox is set. If enabled, WinTAX4 will export values to the WorkSheet whose name is entered in the text field. If the WorkSheet is not found, an error message is displayed in the History message window and the export is aborted. If this option is not set, WinTAX4 will export values in the first WorkSheet.

  * Start from This option is enabled only when the External Sheet (dynamic cell copy) checkbox is set. If enabled, WinTAX4 will export values starting from Row and Column entered in the numeric fields (0 based indexes).

  * Run Macro This option is enabled only when the External Sheet (dynamic cell copy) checkbox is set. If enabled, WinTAX4 will call the indicated macros whose name are entered in the text field. Two macros can be set:




On Start This macro is run before any change in the WorkSheet.

On Finish This macro is run after the WorkSheet has been updated.

The macros must be declared in a macro module of the XLS file. The macros are run after each lap export, or when the session is cleared. In case of error, an error message is displayed in the History message window.

  * Auto Clear This option is enabled only when the External Sheet (dynamic cell copy) checkbox is set. If enabled, WinTAX4 will clear a number of Rows*Columns in the WorkSheet when a new session is started. Otherwise, a new session will simply start from the first row/column but old values are not cleared.




### Options

In this page the configurations chosen in the other two pages can be enabled.

  * Export lap to Pit List: enables/disables the post processing export (Report and Trend)

  * Export Real Time data: enables/disables the real time export (RT Channels)

  * Export Report: enables/disables the export of the reports in TXT, CSV, XLS.




### Report Manager multi-configuration

On selected licenses, it is possible to configure and export multiple reports based on:

  * Same incoming dataset RT/PP with a different reference Lap Report.
  * Same Lap Report with a different incoming dataset RT/PP.



The example below shows how to configure for 2 different devices or streams the same lap report that will be populated on a txt report for Car1 and Car 2.

### 

### Commands of the window

#### Menu

The window menu allows the access to the following commands, divided in sub menus:

##### File Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Apply | Applies the present settings of the window. |
| Cancel | Closes the window without applying the current settings. |
  
##### View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl \+ Shift + Tab | Enables the page of the window next to the one currently in use. |
  
##### Tools Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Update | Recalculates the lap report and the trends to be exported with the last downloaded lap. |
| Scan | Recalculates the lap report and the trends to be exported for the whole present session . |
| Clear | Clears the exported data. |
  
##### Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
#### 

#### Toolbar

The Automatic Tasks toolbar window allows access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Button not enabled in this window |
| Cancel | Similar to the Cancel command of the File menu |
| Apply | Similar to the Apply command of the File menu |
| Channel Browser | Visualizes the pop-up menu to select the page in the Channel Browser window. |
| Update | Similar to the Update command of the Tools menu |
| Scan | Similar to the Scan command of the Tools menu |
| Clear | Similar to the Clear command of the Tools menu |
  
##### Related Issues

  * [MPS](../../ExternalComponents/MPS.md)


