## USB/FlashCard Acquisition

Process of data transfer from an external memory.

The data can be manually or automatically downloaded through, respectively, the Manual Download function (DLM USB/FlashCard) and the Auto Download (ADL USB/FlashCard) function.

The button "Activate USB/FlashCard" enables and disables the acquisition process. The button has a menu that allows the selection of a USB/FlashCard to be used for downloading data. 

| STATUS | DESCRIPTION |
| --- | --- |
|  | Process enabled and connection to the device established |
|  | Process enabled and connection failed |
|  | Process not enabled |
  
### Device Selection:

| STATUS | Description |
| --- | --- |
|  | Alias of the USB/FlashCard connected |
|  | USB/FlashCard connected |
|  | USB/FlashCard not connected |
|  | USB/FlashCard enabled for Auto Download (ADL) and Manual Download (DLM) |
|  | USB/FlashCard enabled only for Manual Download (DLM). |
| Logger | Alias configured: the device is displayed in bold |
| Logger | Alias not configured: the device is not displayed in bold |
  
### USB/FlashCard Commands

| ICON | Commands | Description |
| --- | --- | --- |
|  | Activate DVR | Starts/Stops the acquisition process. |
|  | ANY mode | WinTAX will connect to the first detected device in the drive list |
|  | Download Data from datalogger USB/FlashCard | This command downloads a summary list of laps available in the drive unit and shows the data in a table. |
|  | Create lap list from datalogger USB/FlashCard | Shows the "Download data" window, that allows selection of the laps to be downloaded. |
|  | Auto download USB/FlashCard | Enabling/Disabling an Auto Download session (ADL USB/FlashCard) |
|  | Download latest data USB/FlashCard | Downloads all laps from device once. |
|  | Log Messages USB/FlashCard | Shows/Hides the log window |
|  | Clear Data USB/FlashCard | Command for deleting data from the USB/FlashCard. |
|  | Format USB/FlashCard | Command for formatting the USB/FlashCard. |
  
### Configuration of the acquisition process

#### Acquisition Mode

The first thing to do is to configure the device acquisition interface. Concerning USB/FlashCard there are two choices: USB/FlashCard and Standard

  * Standard: the connection with the devices is started through FindDevs.

  * USB/FlashCard: the connection with the devices is started on the external memory, activate this option to enable the download from a USB/FlashCard.




####  USB/FlashCard Setup

  * Auto Download: Enabling/Disabling an Auto Download session

  * Reset auto download counter: reset the ABS counter of downloaded laps.




#### Manual Download Options

  * Update auto download counter after Manual download: when this option is enabled the auto download counter will be automatically updated. This will force the following Auto Download session to restart the download from the last ABS counter that was downloaded in manual download.

  * Check downloaded file on request laps list: If this option is enabled, a check-mark will be visualized near all laps that have already been downloaded by the PC.




#### USB/FlashCards

There is a list of the paths of the USB/FlashCard.

Options:

  * Add: Open the browse selection to add a new path.

  * Browse: Open the browse selection to change current path.

  * Remove: Remove the selected path.

  * Clear all: Remove all paths.




#### Lap information

The list of information columns that are visualized in the header of the "Download Window" can be configured in the "Lap Information" page.

### Start of the data download session

Set Device acquisition interface as USB/FlashCard.

Enable the Activate USB/FlashCard button. If in the above mentioned USB/FlashCards list, only one path has been configured, the process will automatically connect to this USB/FlashCard , otherwise a menu will open showing the list of the USB/FlashCards configured on the PC and the user will be able to choose which USB/FlashCard to be used. When the "Activate USB/FlashCard" button becomes green, the process is active. It is therefore possible to start an acquisition session from the USB/FlashCard displayed in the combo. Through the combo, the USB/FlashCard to which the user is currently connected can be modified.

###  Manual Acquisition from USB/FlashCard

To enable this type of download, the user must first of all manually select the lap(s) to be downloaded

The list of the laps is viewed by pressing the "Download Data ( )" button: this command opens a window containing the complete list of all laps acquired up to that moment by the central control unit.

After selecting one or more laps (via mouse or keyboard) and pressing the OK button, the data will be downloaded and saved on the local PC.

It is not possible to delete laps from the "Download Data" window. The delete operation is available only using the Clear Data command, which deletes ALL acquired data from the device.

Like the "Download Data" command, the "Check Downloaded Files USB/FlashCard" command allows to manually download (DLM) data.

The procedure for Manual Download (DLM) is the same as what is described for Download Data. This command allows viewing in the Download Data window, if the lap has already been downloaded from the PC.

In the figure the downloaded laps are checked in green. The download status is obtained by comparing the data available in the acquisition directory of the local PC and the data available on the USB/FlashCard.

###  Automatic Acquisition from USB/FlashCard

By enabling the Auto Download command, WinTAX4 starts the Auto Download (ADL) session: in this type of download, the application verifies if new data is available on the USB/FlashCard, it checks the list of laps and enables the download.

  * WinTAX4 waits until it has checked that a new lap is available.




If a new lap on the USB/FlashCard is available, it is detected by a file whose functions are similar to the [FindDevs](../MARVEL/AcqMng_MARVEL.md#FindDevs) tool.

Optional functions:

  * ###### [Device filtering](../FilterFindDevsDevices/FindDevs_Filter.md)


