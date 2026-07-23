## Race Control system Acquisition

Race Control System or RCS acquisition allows the acquisition and visualization of data in Real Time:

Depending on what type of Acquisition mode is enabled on the license you might see one or more of the following acquisition modes:

#### Acquisition Modes:

###### ROUTER:

Logging Table Section:

File: points to the directory where WinTAX4 looks for the logging table (*.DLS) to be used with the Real time data stream

System Setup: This file contains the configuration needed to connect to the router and receive data.

Car:Allows the selection of the car data that will be acquired. The menu changes based on the TEP Setup that is selected.

###### TEP:

Logging Table Section: File: Needs to be configured with the path and filename of the logging table (*.DLS) to be used with the Real time data stream.

TEP Setup: it is configured with the path and destination name of the file that contains the TEP setup containing multicast addresses and configurations related to the CAR.

CAR: Allows the selection of the car data that will be acquired. The menu changes based on the TEP Setup that is selected.

###### VMS:

Logging Table Section:

File: Needs to be configured with the path and filename of the logging table (*.DLS) to be used with the Real time data stream.

OBD ID: Select the same "OnBoard ID" that has been configured on the VMS Settings

Custom Name: Can be configured as desired.

#### Shared settings:

Connection parameters - IP: this field needs to be completed with the multicast ip destination address of the RCS Router / VMS transmitting data.

Connection parameters - Port: This field needs to be completed with the port used by the RCS Router / VMS to transmit data.

Ethernet Binding: Allows the selection of a physical network card, and binds all communications on that network card. 

Don't save RCS Data: When enabled, no data will be saved at change lap.

Auto-Limit: The autolimit, if enabled in the license, creates a Lap after every X seconds as indicated in the text field.