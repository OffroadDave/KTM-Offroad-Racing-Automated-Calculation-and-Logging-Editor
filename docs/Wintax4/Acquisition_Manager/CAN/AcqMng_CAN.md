## CAN Acquisition

Online CAN acquisition mode permits to capture and save data from a generic CAN controller without using a specific protocol. Configuring how the data will be downloaded from CAN, can be manually done in a dedicated environment or automatically, by importing and using a DBC format file. (standard vector) 

### CAN Commands

The Can commands are the same from toolbar and menu; in the picture below there is the CAN toolbar.

| Commands | Description |
| --- | --- |
| Activate CAN | Enabling/Disabling of the acquisition process. the button can assume one of the following status: Green: Process enabled and connection to the device established Red: Process enabled and connection failed Grey: Process not enabled |
| Log Message CAN | Shows/Hides the log window |
  
Enabling/Disabling of the acquisition process

###  Configuration of the acquisition process

The configuration environment allows to define the map of channels that can be acquired on the CAN line.

#### CAN Signals Database

The CAN channels map is composed by elements called signals; in this sections the user can configure the signals.

  * Name: Path and name of the configuration file.

  * New: Create a new file .dbw which contains the description of the geometry of the messages.

  * Save As: Save a copy of the loaded configuration file.

  * Import DBC: Imports a standard vector file (.dbc) and converts it in a .dbw file.




The Import DBC button opens a file browser; selecting and opening a file with the browser, the following window appears.

The window shows a report about import DBC process. The window is listing all of the valid signals (imported), the invalid signals (skipped) and the message contains the source from where the message came from. Above the list there is a short report of import operations.

The Cancel button aborts import; the Import button complete the operations and convert all signals in a .dbw file. 

  * Edit:




The Edit button opens the [CAN Signal Database Editor](AcqMng_CANEdit.md) where signals are configured.

### Acquisition Summary

  * Total: This is an information message regarding the number of messages and signals contained in the map.

  * Acquired: This is an information message regarding the number of messages and signals choose by the user for acquisition.




### Data Acquisition

  * CAN Device: refers to the adaptor models supported by WinTAX4: NetronicsCANdo, Peak, Vector (Deprecated), Vector XL.

NOTE: Vector is now deprecated. It is now available only on 32bit versions of WinTAX and can be used to maintain compatibility with older PCI devices. Vector will not be supported in the future.

  * Bitrate: the values admitted for bitrate are 5Kbit/s, 10Kbit/s, 20Kbit/s, 50Kbit/s, 100Kbit/s, 125Kbit/s, 250Kbit/s, 500Kbit/s, 800Kbit/s, 1Mbit/s

  * Don't save Data: If this flag is ON, acquisition process will not save dstData.ztx file.

  * Auto-limit (sec.): Defines the length in seconds of the lap; default is 120 seconds.




### Enable Trigger

  * Enable Trigger: This check enables the use of a trigger channel for saving data

  * Trigger Channel: Selects the trigger channel

  * Min: Minimum value for the trigger channel

  * Max: Maximum value for the trigger channel

  * Pre-Trigger (sec.): When the value of the channel enters in the trigger range, the process starts saving the data. If this value is non-zero, the data saving begins N seconds before.

  * Post-Trigger (sec.): When the value of the channel exits the trigger range, the process stops saving the data. If this value is non-zero, the data saving stops N seconds after.


