## TCP Acquisition

The TCP Data Acquisition allows the user to acquire data from a logger device by using the TCP protocol. Data can be manually or automatically downloaded using the Manual Download function (DLM) or the Auto Download (ADL) function. All commands and their data download procedures are described here.

##### TCP toolbar: list of the available commands:

##### 

Activate TCP: Starts/Stops the acquisition process. The icon on the button is a status indicator.

| STATUS | DESCRIPTION |
| --- | --- |
|  | Process enabled and connection with the device displayed in the combo box. |
|  | Process enabled and connection failed |
|  | Process not active |
  
### Device selection:

| STATUS | DESCRIPTION |
| --- | --- |
|  | Alias of the device connected |
|  | Device connected |
|  | Device not connected |
|  | Device enabled for Auto Download (ADL) and Manual Download (DLM) |
|  | Device enabled only for Manual Download (DLM). |
| Logger | Alias configured in the Devices list: the device is displayed in bold |
| Logger | Alias not configured in the Devices list: the device is not displayed in bold |
  
TCP Commands

| ICON | COMMANDS | DESCRIPTION |
| --- | --- | --- |
|  | Activate TCP | Start/Stop of the acquisition process. |
|  | Download data from data logger TCP | This command downloads a summary list of laps available in the data logger and shows the data in a table. (Manual Download session) |
|  | Auto Download TCP | Enabling /Disabling of an Auto Download session (ADL) |
|  | Download latest data TCP | Downloads all laps from device once. |
|  | Auto Download FDL | Enabling /Disabling of an Auto Download session (ADL) of the Fast Data Logging (FDL). (For further details about the [FDL](../FDL/AcqMng_FDL.md) download, please see the corresponding chapter). |
|  | Log Messages TCP | Shows/Hides the log window |
|  | Clear data TCP | Command to delete all data from the Logger. |
|  | ANY mode | WinTAX will connect to the first detected device on the network |
|  | Restart TCP | Command to restart the device (available only on menu) |
  
###  Configuration of the acquisition process

#### Enable connection via IP

When enabled, the device list will display an IP address. Configuring the IP address of a Data Logger in the Acquisition section disables the _FindDevs_ scan and allows a direct connection to the device at the specified IP. Only one IP address can be configured per device.

To enable usage of the IP address, the _Enable connection via IP_ flag must be enabled. Add a device (or, if a device is already available, enter the IP address in the _IP Address_ field). **Note:** This field is only available if the above-mentioned flag has been enabled.

In the list of acquisition devices, you will now see an entry with an IP icon. Select the desired device.

#### Auto Download

Auto Download: Enabling/Disabling of an Auto Download session

  * Force download in Trigger IN condition after time out: this option allows WinTAX4 to interrupt the Data Logger when it is in Trigger IN. During the WinTAX4 sessions it verifies that a new ABS is available and it checks the status of the trigger, when the conditions are checked, a warning message is displayed with two possible choices:

  *     * Skip: to inhibit the download of the lap(s)

    * Proceed to enable the data download. If the user makes no choice within the Time Out (configurable in the field near to the flag [s] ), WinTAX4 automatically proceeds with the Auto Download session.




This option may cause undesired stop-logging when used in the garage.

  * Force download in Trigger OUT condition after time out: like for Trigger IN, the trigger status enabling the condition is Trigger OUT.

  * Reset auto download counter: reset the ABS counter of downloaded laps.




####  Binding

  * WinTAX will list all available network cards in this combobox. If the default settings is selected ("Any") the acquisition process will connect to the logger using any card that is available on the system. If a card has been selected, instead, the process will connect using that card if it is connected and if it can reach the logger. If the card is not connected and binding has been selected for this card the connection will not be possible.




####  FDL

  * Auto Download FDL: Enabling/Disabling an Auto Download session (ADL FDL). 

  * Removal Time Out files: configuration of the timeout within which the user must answer before WinTAX4 automatically removes the FDL files. When an Auto Download (ADL FDL) session ends, if the "Remove FDL files after download" option is enabled, WinTAX4 displays a message asking the user whether to remove the downloaded files. If no answer is given before the timeout, WinTAX4 will automatically cancel the data and will continue the Auto Download session.




For further details about FDL, see [FDL](../FDL/AcqMng_FDL.md) chapter.

#### Manual Download Options

  * Update auto download counter after Manual Download: when this option is enabled the auto download counter will be automatically updated. This will force the following Auto Download session to restart the download from the last ABS counter that was downloaded in manual download.

  * Check downloaded file on request laps list: If this option is enabled, the interface will display if the lap has already been downloaded by the PC.

  * Remove FDL files after download: if this option is enabled, WinTAX4 displays a message asking the user whether to remove the downloaded files. If no answer is given before the timeout, WinTAX4 will automatically cancel the data and will continue the Auto Download session.




###  Start of a data downloading session

To start a data acquisition session (both Manual Download and Auto Download), just press the Activate TCP button

The combo will be automatically updated with the list of the devices connected to the network and the list of the devices configured in the ["Setup Acquisition" environment.](../SESSION_STARTUP/AcqMng_Session_Startup.md#AcqMng_SessionStartupSetupDevice)

When the Activate TCP button becomes green, the link with the device is active. It is now possible to start an acquisition session on that device (displayed as first line of the Combo Alias ). To modify the device to which the user is connected, just open the combo and select another one.

###  Manual Acquisition (DLM)

To enable this type of download, the user must manually select the lap(s) to be downloaded.

The lap list is viewed by pressing the "Download data ( )" button: this command opens a window containing the complete list of all laps acquired until that moment by the central control unit.

After selecting one or more laps (via mouse or keyboard) and pressing OK, the data will be downloaded and saved on the local PC.

Use the mouse right click to open a popup menu; it contains two commands for selecting an entire run:

  * Select RUN (shortcut key R)

  * Select last RUN (shortcut key L)




Check downloaded file on request laps list: If this option is enabled, it is possible to view if the lap has already been downloaded by the PC. The check is the same which is located in configuration options. In the figure the Laps already downloaded are checked in green. The download status is obtained by comparing the data available in the acquisition directory of the local PC and the data available on the acquirer.

It is not possible to delete laps from the "Download Data" window. The delete operation is available only using the Clear Data command, which deletes ALL acquired data from the device.

The columns available in the "Download Data" window can be configured in the Lap Selection page of the TCP Devices.

###  Automatic Acquisition (ADL)

When the Auto Download command is enabled, WinTAX4 starts the Auto Download session: in this type of download the application itself verifies if new data is available on the Data Logger, checking the list of the laps and enabling the Auto Download (ADL) function:

  * WinTAX4 waits until it detects the presence of a new lap (increase of the ABS counter).

  * The download does not begin while the trigger condition is not verified (Trigger = Trigger Out or Trigger IN).




The standard configuration of Auto Download ADL is:

  * If the user manually downloads a lap, it is not registered as an Auto Download (ADL) session. ( Update auto download counter after Manual download: not checked)

  * The Trigger condition is Trigger OUT ( Force download in Trigger IN condition after time out not checked)




This standard behavior can be changed in Configuration of the above mentioned Download options.

WinTAX4 will issue a warning if an Auto Download session is trying to download a set of data that has already been downloaded by any PC (local or remote) as it is shown in the following image:

Trigger is intended as the acquisition status of the Data Logger:

  1.      * Trigger IN: the Data Logger acquires and saves data on the local disk.

     * Trigger OUT: the Data Logger acquires but does not save data on the local disk.




The ABS counter change (acquisition of a new lap) and the trigger status of the acquirer are transmitted to WinTAX4 by the FindDevs.

FindDevs reads the broadcast packets sent by all devices connected to the network. These packets contain the information about the ABS and the trigger status. FindDevs interprets these information and makes them available for all application of the PC that requires them. The information are also available in the log window for debugging.

FindDevs is automatically activated when WinTAX4 starts.

Optional functions:

  * ###### [Device filtering](../FilterFindDevsDevices/FindDevs_Filter.md)


