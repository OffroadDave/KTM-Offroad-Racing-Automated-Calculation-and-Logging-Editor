## Rx Tasks

Rx Tasks are automatic processes which are carried out by [Acquisition Manager](../AcqMan.md) when new data is acquired directly from the logger, when downloading cable data or via telemetry.

These tasks are defined in the Rx Tasks environment, that can be reached from the Acquisition/Rx Tasks Setup menu or from the Rx Tasks Setup button on the toolbar.

For each task, select if the process is applied to cable or telemetry data or both.

In PRO versions, tasks are carried out in an order, which is configurable by the user, however the order should be set up with care as it may affect the resulting output (e.g. setting the Import Meteo task after the AutoRx causes the visualization of data in WinTAX4 without meteo information).

In TEAM versions, the order of the task is fixed, the user can only enable/disable a required task.

###  Auto RX: Automatic loading of data after saving lap

It updates the graph windows whenever a lap is downloaded. The Auto Rx process only detects new data in the WinTAX4 Data path as defined in Setup/General/Directories/Acquisition

This option may slow down the downloading process especially if the active windows contains many channels or complicated math channels. To speed up downloading, disable this option and use [Update Rx](../../Menu/Acquisition.md) command to refresh the screen with the latest lap at the end of downloading.

It is possible to change the default dataset used by the AutoRX function using the function ["Set Default AutoRX dataset"](../../Toolbars/ToolbarDataHeader.md#SetdefaultAutoRXDataset) in the Data Header toolbar. 

### Auto Rx Remote: Automatic loading of data from remote path after saving lap

It is similar to Auto Rx, when this task is active WinTAX4 will load and refresh the screen with data from a remote path.

Auto Rx Remote allows you to load data generated with TelDataZTX, if this function is enabled in the licence.

It is strongly suggested to select the "Session" level where data is going to be stored. Laps are searched up to 6 folder levels below the selected path.

Every time a lap is saved in the selected path, the lap will be automatically loaded in WinTAX. If the "Append Lap" is selected, the loaded lap will be appended. The user can select how many laps will be appended. Exceeding laps will be removed from the dataload.

It is required to select the Acquisition processs that is going to be used in the "Data Enable" area to "AutoRX remote" the laps. If no acquisition process is selected, data saved in the folder selected in the AutoRX remote folder will not be loaded.

Note: with Auto Rx Remote data is not copied from the remote location to the local PC.

### Automatically Associate AutoRX data to a DataSet:

In the "AutoRX" and "AutoRX remote" environment it is possible to associate a single device / stream to a specific Dataset where received data will be loaded.

##### Enabling "Multi AutoRX DataSet"

When this function is not enabled, the behaviour of the AutoRX function is as default. Data that is received when the AutoRX function is active is associated to the default Dataset. 

When "Enable Multi AutoRX Dataset" is activated:

  * the "select device" combo is disabled in AutoRX remote.
  * the protocol filter "data enable" is disabled in AutoRX and AutoRX Remote. 



For each Dataset (from 1 to 10) it will be possible to configure the following:

  * Dataset: represents the dataset that will be used to load acquired data.
  * Stream/Device: represents the device, as configured in acquisition manager, from which data will be acquired.
  * Acquisition: represents the type of data source that is used to acquire data.



The Streams/Device are referrering to the devices configured in the Acquisition manager settings.

Each device can only be assigned to a single dataset.

Acquired data is associated to a dataset only if all of the following conditions are true:

  * The dataset is enabled (as can be seen in Dataset 2 and 5 in the above screenshot)
  * Acquired data corresponds to the configured Device
  * Acquired data corresponds to one of the processes configured in the Acquisition filter



If the above condition are not met, data is saved and acquired but will not be loaded and will not be associated to a device.

##### Configuration Example (see above screenshot):

Case 1: Data acquired via Marvel protocol from "Device1", will be loaded on "Dataset 5"

Case 2: Data acquired via Marvel acquisition from "Device2" will not be loaded to any dataset because the acquisition protocol does not correspond to the one configured.

Case 3: Data acquired via TCP from Device3 is not loaded to any dataset because the Device name is not configured.

###  Auto Copy To Host: Automatic copying of downloaded files to remote path

It automatically copies acquired data to a series of user-defined paths. This is useful when data must be downloaded to a local path on one PC before copying them to a more generally accessible central archive on the network.

To allow the copy to a remote host of Fast Data Logging files (FDL), enable the flag Make also for FDL data.

Note. The Copy To Host of the FDL data is subordinated to the copy to host of the ztx data, therefore it cannot be enabled separately.

### 

### Auto Import Lap: Automatic copying and loading of data from remote path

This is the opposite process of Auto Copy To Host, it copies new data, which for instance is detected on a network path to the local PC in the Import directory.

If combined with Auto Rx Remote, this task allows importing new data onto a path on the local PC and then load it. This can be useful, for example, to display telemetry data lap-by-lap on the local PC without activating a telemetry acquisition session.

###  Export To Exp: Export laps to EXP Format files

It automatically exports the downloaded laps. The EXP file contains the archive information needed to easily import these laps. The Data Browser can later be configured to display this data just by manually pointing the unzipping directory of the EXP file or by using the Data Browser functions Import From Exp that automatically unzips the data in the path indicated by the user.

The filters:

  * Real time

  * TCP

  * MVL

  * USB/FlashCard

  * SEC




allow to export only the corresponding type of data. The Laps are exported to the directory Export defined in [General Setup/Directories](../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgDirectories). For each exported Lap, an EXP file will be created.

###  Export Channels To Exp: Export channels to Exp Format files

It has the same logic as Export To Exp, it allows exporting only a group of channels defined by the user.

The list of the channels is read by the configuration file *.ECH that must be configured by the user in the Browse .ECH file field.

The *.ECH file can be created only by Data Browser in the manual functions of Export Channels to ZTX or to EXP.

### Export Bin: Export laps to BIN Format files

It automatically exports in [BIN format](../../ExportFormats/binary_file_format.md) any new laps found in the Acquisition directory to the WinTAX4/Export directory. It specifies an *.ECH configuration file, which determines the channels to be exported and their related frequencies together with a time range.

###  Export Channels To ZTX: Export channels to ZTX files

It is similar to Export Channels To Exp, but the format of the exported file is different. A copy of the Lap acquired is created in the directory /Export. The copy created will be a directory (unzipped) as it contains only the data of the Lap. Further information of Run/Device/Session are not exported. To display the Lap from Data Browser, manually reconstruct the directory level needed. (obsolete function, Export Channels To Exp perform the same function)

In some licenses, management of Export Channels To Exp is extended to include more entries in the same task (max. 5). Check related flag in order to enable the single export during acquisition. Repeat the same destination will cause the overwriting of data.

### Convert Bin Remote: Convert remote laps to BIN format

It converts any new files from a selection of paths (typically network paths) and outputs data to another path defined by the user.

It can be used, for example, to automatically detect new data from different devices and export them in [BIN format](../../ExportFormats/binary_file_format.md) to any path.

###  Import Meteo: Import Meteorological data

Automatic task to associate meteo data from an external meteo station while on-board data is received.

The information available are:

| INFORMATION | DESCRIPTION | UNIT |
| --- | --- | --- |
| Temperature In | Meteo station internal temperature sensor | °C |
| Temperature Out | Meteo station external temperature sensor | °C |
| Wind Speed | Meteo station wind speed | km/h |
| Wind Direction | Meteo station wind direction | 180E -> 180W |
| Pressure | Meteo station internal barometric pressure sensor | bar |
| Humidity In | Meteo station internal humidity sensor | % rel hum |
| Humidity Out | Meteo station external humidity sensor | % rel hum |
| Rain | Meteo station rainfall sensor | mm |
  
From the configuration page it is possible to enable the acquisitions for which the task is activated. The path of a .met file is also needed to set the source of meteo data.

For the correct functioning of the "import meteo" task it is important that the devices used to produce the data (telemetry and meteo) have their times synchronized.

### Make Event Logs: Generate event log after saving lap

It creates an Event Log file in XML format based on the libraries of [Events](../../RuntimeAnalysis/Events/Events.md) currently loaded in WinTAX4. The log file created can be processed by external applications.

The filters:

  * Real time

  * TCP

  * MVL

  * USB/FlashCard

  * SEC




allow to create the Event Log only on the type of data configured.

### Auto Update Run: Automatic loading of data after closing run

It automatically replaces the current Real Time data with the "best" data.

"Best" data is what has been logged at the highest rate; it is supposed that Cable data is always at higher frequency than Real Time data.

This operation is transparent for the user that receives the highest rate when available.

See [Intelligent Data Loading](../Intelligent_Data_Loading/Intelligent_Data_loading.md) chapter for a detailed description of the functions.

## 

###  Export Channels to ZTX files on Auto RX Remote

This Rx Task mixes Auto Rx Remote and Export Channels To ZTX. it is active only when Auto Rx Remote and export channels from a selected .ech configuration are selected after saving laps on autoload data from remote directory.

### Run Application At Download End

This RX Task allows the user to run a "customizable" application when data download is completed. The path of the application and custom parameters can be configured in the "Tab" relative to the task. The task will not be executed at the end of the download of a single lap (Auto-RX), but when the whole data download has been completed. It will be executed only if acquired data is not "Real Time" (TCP, MARVEL, DVR, USB/FlashCard).

### Make Faster File: Convert laps in Faster File format

Option to automatically convert downloaded files in Faster File format, which is optimized to speed the data loading from the archive.

This task should be run before the "Auto Copy To Host" to ensure that files which are copied to a central archive path, for example, are already converted.

This task is available only with some licenses.

### Auto Import MDF / Matlab:

When enabled, this RxTask automatically imports `.MDF` or `.m` (Matlab) files placed into a specified directory. 

It works for both new files and, optionally, for files already in the folder.

Once activated, the system watches a folder for incoming files and processes them using predefined import configurations (IMD for MDF, or equivalent for Matlab). No user interaction is required during import.

##### Configuration Options

Import existing files: when enabled, imports all files already present in the input directory at activation time.

Overwrite existing output files: If enabled, output files will be overwritten if they already exist. 

If disabled, a suffix will be added to the RUN name of the imported file to avoid overwriting.

NOTE: If the flag "Overwrite Existing output files is disabled and the flag "Import Existing files" is enabled, and files are already existing in the output folder, WinTAX will overwrite those files.

Open Multi-Configuration: Opens a separate screen to define multiple configurations. Each configuration includes:

Config Number: Enables the configuration. All fields must be completed when the configuration is enabled.

Each configuration contains the following settings: 

Browse data folder: folder to monitor for .MDF or .m files.

Browse IMD file: full path to the import settings file (e.g., *.IMD).

Use Path Settings From: This setting lets you choose where the Track, Session, and Device values should come from:

\- “.imd file”:  Values are taken from the import configuration file.

\- “Custom”: Values are manually entered by the user in the Output path textboxes.

Output Path (Track, Session, Device): overrides the default output location for data to be saved. (Track / Session / Device fields)

Notes on Behaviors:

All pop-up messages during the import procedure are automatically suppressed. Logs messages are visualized in the main log window of WinTAX.

The RUN output directory is auto-generated using the configured path and file information.