## Acquisition Manager Overview

WTX4AcqMan, called Acquisition Manager, is a WinTAX4 component used to acquire data (Cable and Real Time) from an external source and save them on PC. Acquisition Manager can be opened from WinTAX4 Acquisition menu with command Acquisition Manager.

The main window can be like in the following picture.

Acquisition Manager manages some automatic processes, called [Rx Tasks](Rx_tasks/Rx_tasks.md), started when new data (Cable and Real Time) are acquired.

When a process acquisition is running, the corresponding combo box will be disabled but still remain the possibility to modified the field from [Setup Acquisition](SESSION_STARTUP/AcqMng_Session_Startup.md) environment.

In order to download data from the logger, configure the network settings of the Ethernet adapter. See [here](NetworkSettings/NIC_config_TS11.md) for instructions.

### Configuration and Rx Tasks

Before starting to download you will need to define the session and device setups and the select some options for the acquisition processes.

In the toolbar there are two buttons used to open the [Setup Acquisition](SESSION_STARTUP/AcqMng_Session_Startup.md) and the [Rx Tasks](Rx_tasks/Rx_tasks.md). The same commands are in Acquisition menu.

In the toolbar, in the same licenses, there might be three extra combo boxes that can be used for fast definition of:

  1. Path for data acquisition (the default is taken from WinTAX4 general setup, e.g. \WinTAX4\Data\\)

  2. Session Name: the combo proposes the session name defined in Setup Acquisition and entries history.

  3. Run Name: the combo proposes the run name defined in Setup Acquisition and entries history.




In the Session Name combo and in the Run Name combo it is possible to delete entries history. To remove history the user needs to open the dropdown combo box and press the Delete key. A dialog box will appear:

By choosing yes, the combo will be emptied.

### Devices

In the main window there are some toolbars; each of them collects the commands of a given acquisition [device](SESSION_STARTUP/AcqMng_Session_Startup.md#AcqMng_SessionStartupSetupDevice).

### Find Devices on the Network

The application “Find Devices on the Network” is the module responsible for displaying all connected devices on the network. The application starts automatically at the first request of devices from applications that use them. The application automatically closes a few minutes after the last application connected has been closed. See [here](FindDev/FindDev.md) for details.