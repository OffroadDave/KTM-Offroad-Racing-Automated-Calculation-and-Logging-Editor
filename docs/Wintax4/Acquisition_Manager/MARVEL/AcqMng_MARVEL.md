## MARVEL Acquisition

A Cable Data Acquisition from Marvel devices.

The data can be manually or automatically downloaded through, respectively, the Manual Download function (DLM) and the Auto Download (ADL) function.

All commands and their procedures to download data are described here.

##### Marvel bar: list of the available commands

Activate Marvel: Start/Stop the acquisition process. The icon of the button is a status indicator.

| Status | Description |
| --- | --- |
|  | Process enabled and connection with the device displayed in the combo |
|  | Process enabled and connection failed |
|  | Process not active |
  
### Device selection:

| Status of the commands | Description |
| --- | --- |
|  | Alias of the connected device |
|  | Device connected |
|  | Device not connected |
|  | Device enabled to download Auto Download (ADL) and Manual Download (DLM) |
|  | Device enabled only to Manual Download (DLM). |
| Logger | Alias configured in the Devices list: the device is displayed in bold |
| Logger | Alias not configured in the Devices list: the device is not displayed in bold |
  
### Marvel Commands

|  | Commands | Description |
| --- | --- | --- |
|  | Activate MVL | Start/Stop of the acquisition process. |
|  | ANY mode | WinTAX will connect to the first detected device on the network |
|  | Download data from data logger Marvel | This command downloads a summary list of laps available in the data logger and shows the data in a table. (Manual Download session) |
|  | Auto Download Marvel | Enabling /Disabling of an Auto Download (ADL) session |
|  | Download latest data Marvel | Downloads all laps from device once |
|  | Log Messages Marvel | Shows/Hides the log window |
|  | Clear Data Marvel | Command to delete all data from the Logger. |
|  | Restart Marvel | Command to restart the device (available only on menu) |
  
###  Configuration of the Download options

#### Enable connection via IP

When enabled, the device list will display an IP address. Configuring the IP address of a Data Logger in the Acquisition section disables the _FindDevs_ scan and allows a direct connection to the device at the specified IP. Only one IP address can be configured per device.

To enable usage of the IP address, the _Enable connection via IP_ flag must be enabled. Add a device (or, if a device is already available, enter the IP address in the _IP Address_ field). **Note:** This field is only available if the above-mentioned flag has been enabled.

In the list of acquisition devices, you will now see an entry with an IP icon. Select the desired device.

#### Auto Download

Enabling/Disabling of an Auto Download (ADL) session

  * Force download in Trigger IN condition after time out: this option allows WinTAX4 to interrupt the Data Logger when it is in Trigger IN. During the WinTAX4 sessions it verifies that a new ABS is available and it checks the status of the trigger, when the conditions are checked, a warning message is displayed with two possible choices:

  *     * Skip: to inhibit the download of the lap(s)

    * Proceed: to enable the data download. If the user makes no choice within the Time Out (configurable in the field near to the flag [s] ), WinTAX4 automatically proceeds with the Auto Download session.




This option may cause undesired stop-logging when used in the garage.

  * Force download in Trigger OUT condition after time out: like for Trigger IN, the trigger status enabling the condition is Trigger OUT.

  * Reset auto download counter: reset the ABS counter of downloaded laps.




###  Manual Download Options

  * Check downloaded file on request laps list: If this option is enabled, allows viewing if the lap has already been downloaded by the PC.




###  Start of a data downloading session

To start a data acquisition session (both Manual Download and Auto Download), just press the Activate Marvel button

The combo will be automatically up-dated with the list of the devices connected to the network and the list of the devices configured in the ["Setup Acquisition" environment.](../SESSION_STARTUP/AcqMng_Session_Startup.md#AcqMng_SessionStartupSetupDevice)

When the Activate Marvel button becomes green, the link with the device is active and it is possible to start an acquisition session on that device (displayed as first line of the Combo Alias ). To modify the device to which the user is connected, just open the combo and select another one.

####  Binding

  * WinTAX will list all available network cards in this "combobox". If the default settings is selected ("Any") the acquisition process will connect to the logger using any card that is available on the system. If a card has been selected, instead, the process will connect using that card if it is connected and if it can reach the logger. If the card is not connected and binding has been selected for this card the connection will not be possible.




###  Manual Acquisition (DLM)

To enable this type of download, the user first of all must manually select the lap(s) to be downloaded.

The list of the laps is viewed by pressing the "Download Data ( )" button : this command opens a window containing the complete list of all laps acquired until that moment by the central control unit.

Like the "Download Data" command, the "Check Downloaded Files" command allows to download data in Manual Download.

After selecting one or more laps (via mouse or keyboard) and pressing OK, the data will be downloaded and saved on the local PC.

It is not possible to delete laps from the "Download Data" window. The delete operation is available only using the Clear Data command, which deletes ALL acquired data from the device.

The columns available in the "Download Data" window can be configured in the "Lap Information" page of the TCP Devices.

The manual download procedure is the same as described for "Download Data".

This command allows viewing in the "Download Data" window, if the lap has already been downloaded from the PC.

In the figure the Laps already downloaded are checked in green. The download status is obtained by comparing the data available in the acquisition directory of the local PC and the data available on the acquirer.

###  Automatic Acquisition (ADL)

Enabling the Auto Download command, WinTAX4 starts the Auto Download session: in this type of download the application itself verifies if new data is available on the acquirer, checking the list of the laps and enabling the download.

Functioning of Auto Download :

  * WinTAX4 waits until it detects the presence of a new lap (increase of the ABS counter).

  * The download does not begin until the trigger condition is verified (Trigger = Trigger Out or Trigger IN).




The standard configuration to Auto Download:

  * The Trigger condition is Trigger OUT ( Trigger IN Warning not checked)




This standard behavior can be modified in Configuration of the above mentioned Download options.

WinTAX4 will issue a warning if an Auto Download session is trying to download a set of data already downloaded by any PC (local or remote) as is shown in the following figure:

Trigger is intended as the acquisition status of the Data Logger:

  1.      * Trigger IN: the Data Logger acquires and saves data on the inner disk.

     * Trigger OUT: the Data Logger acquires but does not save data on the inner disk.




The ABS counter change (acquisition of a new lap) and the trigger status of the acquirer are transmitted to WinTAX4 by the FindDevs.

FindDevs reads the broadcast packets sent by all devices connected to the network. These packets contain the information about the ABS and the trigger status. FindDevs interprets these information and makes them available for all application of the PC that requires them. The information are also available in the log window for debugging.

FindDevs is automatically activated by WinTAX4 at the start-up of the Acquisition Manager when needed. 

Optional functions:

  * ###### [Device filtering](../FilterFindDevsDevices/FindDevs_Filter.md)


