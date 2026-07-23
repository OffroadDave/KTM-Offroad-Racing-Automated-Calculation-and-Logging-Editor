## Setup

Before starting a working session, define the parameters used by WinTAX4 to organize in the archives the data acquired.

These settings are defined in the Setup Acquisition environment that can be reached from the Acquisition/Setup menu or from the Setup Acquisition button on the toolbar.

### 

##### Option A - Session configuration - Dynamic Names using Logger Channels:

This function allows using acquired channels to automatically define some of the fields of the Acquisition Setup screen.

When the "Logged Ch" flags are enabled, the user can enter a channel name in the related field. During data acquisition WinTAX will replace the channel name with the related data. The channels needs to be ENUM channels. The associated text of this ENUM channel will be read from the acquired data of the lap and copied at runtime in the corresponding field.

If a NON ENUM channel is used or if a corresponding string is not found in the ENUM channel, the information will be set to the numeric value of the channel.

This function is alternative and available optionally.

##### Option B - Session configuration - Dynamic Names using Logger Channels: 

This function allows using a combination of acquired channels and texts to automatically define some of the fields of the Acquisition Setup screen.

The channels indicated between the $ symbols, if corresponding to an ENUM channel, will be read at runtime and replaced with the runtime value during data acquisition.

This function is alternative and available optionally .

### 

### Commands of the window

| COMMAND | DESCRIPTION |
| --- | --- |
| Apply | Exit from Acquisition Manager Setup confirming the changes |
| Cancel | Exit from Acquisition Manager Setup aborting the changes |
| Load | Loading of context data (constant, user record, offset and gain) of a device |
| Save As | Saving of context data (constant, user record, offset and gain) of a device. |
| Add Device | Adds a new slot device to the configuration |
| Remove Device | Cancels of the slot device selecting |
| Move Up | Moves upwards by one position the slot device selected |
| Move Down | Moves downwards by one position the slot device selected |
| Publish All Session & Device Setup | Publishes on the WTS server the configurations of Session and Devices |
| Retrieve All Session & Device Setup | Reading from the WTS server the Session and Devices configurations |
  
###  Session

These settings are used to create the data archive; in particular the Track and Session information are the names of the root directories where data is saved. In the example below Track is set as Track_GP, while Session is set as race.

The Run information is the name of the directory which contains the laps, in the sample is Run_001.

The information Track Length and Comment are saved on the Session.xml file available in the Run directory and can be viewed in the list of the session of the Data Browser and in the Data Header of WinTAX4.

The second Track information refers to a configured circuit that can be related with all the laps in the Run.

The Publish All Session & Device Setup and the Retrieve All Session & Device Setup commands are enabled only when the Devices acquisition interface in the Options page is set as WTS.

###  Devices

The Devices configuration determines like for the Session section, where the data are saved in the archive.

To enable the Device Recognition, the Device Alias will have to be the same as the Alias sent to the Data Logger through Sysma.

The Device Recognition is an automatism of the acquisition process that allows saving the data downloaded in the directory corresponding to the Alias of the device the user is working on.

If no correspondence is found, the saving is forced in the first device (position inside the tree) of the list.

If the device is connected to the network, the Device Name can be automatically configured through the Search command; by pressing this button, WinTAX4 offers a list containing the Alias of the devices currently connected to the network: selecting one of them the field Device Name is automatically updated with its Alias. This function helps avoiding possible mistakes related to vehicle naming that might cause data being saved in unexpected positions.

Driver Name, Driver Comment, Engine Name and Box Number are other context information.

In the Setup Editor page (or in Offset Gain, Constant, User Record pages in some licenses) is possible to manually set (or loading the configuration from files) the context data of the device that will be saved on the Lap_xxxxxxxxx_xxxxx directory and that are available to the user during the post processing analysis.

In the tree list of the devices, the area and the icon on the left of the name of the alias indicate if the device is enabled for an Auto Download (ADL, area checked and yellow icon) or if it is enabled only for the Manual Download (DLM, area not checked and grey icon).

Some licenses are configured to receive data in the up mentioned fields via an apposite enum channel ( Option A - Dynamic names using Logger channels ) or be auto-filled with a combination of text and one or multiple channels. (Option B - Session and devices received via acquired channels.)

##### Option A - Device configuration - Dynamic Names using Logger Channels:

This function allows using acquired channels to automatically define some of the fields of the Acquisition Setup screen.

When the "Logged Ch" flags are enabled, the user can enter a channel name in the related field. During data acquisition WinTAX will replace the channel name with the related data. The channels needs to be ENUM channels. The associated text of this ENUM channel will be read from the acquired data of the lap and copied at runtime in the corresponding field.

If a NON ENUM channel is used or if a corresponding string is not found in the ENUM channel, the information will be set to the numeric value of the channel.

This function is alternative and available optionally.

##### Option B - Device configuration - Dynamic Names using Logger Channels:

This function allows using a combination of acquired channels and texts to automatically define some of the fields of the Acquisition Setup screen.

The channels indicated between the $ symbols, if corresponding to an ENUM channel, will be read at runtime and replaced with the runtime value during data acquisition. If the entered value is not corresponding to a channel, it will be used as it was entered without the $ symbols.

This function is alternative and available optionally .

###  Setup Devices

The tree list shows the data acquisition processes available. You will find some of them in the list below..

  * [TCP](../TCP/AcqMng_TCP.md)

  * [MVL](../MARVEL/AcqMng_MARVEL.md)

  * [USB/FlashCard](../FLASH_CARD/AcqMng_FLASH_CARD.md)

  * [DVR](../DVR/Acquisition_Manager_DVR.md)

  * [Real-Time](../DST/AcqMng_DST.md)

  * [WDS](../WDS/AcqMng_WDS.md)

  * [CAN](../CAN/AcqMng_CAN.md)




For further details about the configurations, see the manual about each process.

###  Acquisition Mode

This is the configuration page of Acquisition Mode.

The Devices acquisition interface can be:

  * Standard: The connection (point-point) is made directly with the device (GRX or PBE) receiver through the [FindDevs](../TCP/AcqMng_TCP.md#FindDevs)

  * WinTAX4 Telemetry Server: It allows receiving the data distributed through WTS network.

  * USB/FlashCard: The data is retrieved using an [external memory](../FLASH_CARD/AcqMng_FLASH_CARD.md)




WTS Server

Configures the server which distributes data.

Local IP Bind

The user can configure the network adapter used for WTS data acquisition. If you select the network bind "ANY", the network interface with IP address compatible with the WTS Server and Stream address will be selected automatically. Network bind information will be displayed in the log window and an error message will be displayed if the server address is not compatible with the network interface.

When Real-Time acquisition starts-up, a warning message is displayed if the Windows Firewall is turned ON. If the Real-Time stream is not available in the network at the acquisition start-up or becomes unreachable, a popup warning message will be displayed to notify the Real-Time acquisition failure.