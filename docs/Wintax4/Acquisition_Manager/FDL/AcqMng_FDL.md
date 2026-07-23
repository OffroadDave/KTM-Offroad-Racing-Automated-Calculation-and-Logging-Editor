## FDL Acquisition

Allows the acquisition through FDL (Fast Data Logging) Data through the FTP protocol from logger devices.

The data can be manually or automatically downloaded through, respectively, the Manual Download function (DLM FDL) and the Auto Download (ADL FDL) function.

Data is stored by WinTAX4 in the archive (Run level) in a standard MATLAB format (*.mat)

All commands and their procedures to download data are described in this chapter.

#####  TCP bar: list of the available commands:

Activate TCP: Start/Stop of the acquisition process. The icon of the button is like a status indicator. 

| STATUS | DESCRIPTION |
| --- | --- |
|  | Process enabled and connection to the device established |
|  | Process enabled and connection failed |
|  | Process not active |
  
###  Device Selection

| STATUS | DESCRIPTION |
| --- | --- |
|  | Alias of the connected device |
|  | Device connected |
|  | Device not connected |
|  | Device not enabled to download ADL and Manual Download (DLM) |
|  | Device enabled only to Manual Download (DLM). |
| Logger | Alias configured in the list of the Devices: the device is displayed in bold |
| Logger | Alias not configured: the device is not displayed in bold |
  
### FDL Commands

The commands are the same of TCP Acquisition

| ICON | COMMANDS | DESCRIPTION |
| --- | --- | --- |
|  | Download data from data logger TCP | This command downloads a list of laps available in the data logger and shows the data in a table. (Manual Download session) |
|  | Download latest data TCP | Similar to the Auto Download TCP but after downloading all laps, automatically turns off. |
|  | Auto Download FDL | Enabling /Disabling of a session ADL of the Fast Data Logging (FDL). |
|  | Log Messages TCP | Shows/Hides the log window |
|  | Clear data TCP | Command to cancel data from the Logger. |
|  | Restart TCP | Command to restart the device (available only on menu) |
  
## 

### Configuration of the Download options

#### Auto Download FDL

  * Auto Download FDL: Enabling/Disabling an Automatic Download session (ADL FDL)

  * Removal Time Out files: configuration of the timeout within which the user must answer before WinTAX4 automatically removes the FDL files. When an Automatic Download session (ADL FDL) ends, if the "Remove FDL files after download" option is enabled, WinTAX4 displays a message asking the user whether to remove the downloaded files. If no answer is given before the timeout, WinTAX4 will automatically cancel the data and will continue the Automatic Download session.




#### Manual Download FDL

  * Update Auto Download counter after Manual Download: If this option is enabled, when a session of manual download ends (DML FDL), WinTAX4 will ask to the user if the operation must be considered as an Automatic Download session (ADL FDL) and therefore it has to update its download status; answering "Yes" the next automatic download session will begin to download the lap after the last lap of the manual download (DLM FDL) session.



  * Remove FDL files after download: If this option is enabled, when data are downloaded, WinTAX4 asks the user whether to cancel the FDL file from the central control unit




###  Start a data downloading session

To start a data acquisition session (both Manual Download and Automatic Download), press the button: Activate TCP

The combo will be automatically updated with the list of the devices available in the net and with the list of the devices configured in the ["Setup Acquisition"](../SESSION_STARTUP/AcqMng_Session_Startup.md#AcqMng_SessionStartupSetupDevice) environment.

When the Activate TCP button becomes green, the link with the device is active. An acquisition session can therefore be started on this device (displayed as first line of the Combo Alias ). To modify the device to which the user is connected, just open the combo and select another one.

###  Manual Acquisition (DLM FDL)

To enable this type of download, first of all the user must manually select the FDL lap (s) to be downloaded.

The list of the FDL laps is viewed by pressing the Download data from data logger TCP button and selecting the second tab of the window. The window proposed contains therefore the complete list of all FDL laps acquired until that moment by the central control unit.

The list shows the files available on the logger and their formatting.

After selecting one or more files, press Download to begin the transfer of data; the.*.MAT files that are created, are saved in the archive in the RUN folder in a separate directory called FDL.

In the "Download Data" / "FDL" window it's also possible to directly remove the files from the logger through a menu available by right clicking or by pressing the Delete key of the keyboard.

### Automatic Acquisition (ADL FDL)

Enabling the Auto Download FDL command, WinTAX4 starts the automatic download session (ADL FDL): in this type of download the application itself checks if new data is available on the logger, checking the list of laps and enabling the download.

The behaviour of "automatic download session" (ADL FDL) is the same as TCP, therefore:

  * WinTAX4 waits until it checks if a new lap is available (increase of the ABS counter).

  * It does not download until the trigger condition is verified (Trigger = Trigger Out or Trigger).




Standard configuration to automatic download session:

  * If the user manually downloads a lap, it is not registered as an automatic download session. ( Update auto download counter after Manual download: not checked)

  * The Trigger condition is Trigger OUT ( Force download in Trigger IN condition after time out not checked)




The standard behavior can be modified in the configuration of the above mentioned [Download](../TCP/AcqMng_TCP.md#AcqMng_TCPSetup) options.

If the FDL autodownload is enabled simultaneously with the TCP one, the FDL download will follow the TCP one.

The concept of Trigger can be extended to the FDL acquisition:

  1.      * Trigger IN: the Data Logger acquires and saves data on the local disk.

     * Trigger OUT: the Data Logger acquires but does not save data on the local disk.




The checking of the change of the ABS counter (acquisition of a new lap) and the trigger status of the acquirer are transmitted to WinTAX4 by the FindDevs application.

FindDevs reads the broadcast packets sent by all devices connected to the network. These packets contain the information about the ABS and the trigger status. FindDevs interprets these information and makes them available for all application of the PC that requires them (first of all WinTAX4). The information are also available in the log window to debug.

FindDevs is automatically activated by WinTAX4 at the start-up of the Acquisition Manager when needed. 