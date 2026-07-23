## Ethernet download interface

To download data via cable, connect the PC directly to the data logger using a standard Ethernet network adapter. For instructions about the interface adapter installation please refer to the instructions supplied with the hardware.

### Configure the PC with a fixed IP address

#### Instructions for Windows 2000/XP

The data logger is programmed with a fixed IP address. Unless the network is set up to assign the correct address to the PC via DHCP, define a fixed IP address for the PC to allow the two devices to communicate

  1. Connect the PC to the data logger via Ethernet link. From the start menu select Start/Settings/Control Panel and double click on the Network icon.

  2. Right click on the Network connection to be used to download (more than one may be available).

  3. Click on Properties and then select Internet Protocol (TCP/IP) from the list of items.

  4. Take note of the settings found here. Configure them later when the PC has to be connected back to the network.

  5. Choose an IP address on the same subnet as the data logger.

  6. Configure the IP address of the PC as follows, e.g.:

IP address: 192.168.1.xxx




Subnet mask: 255.255.255.0

Default gateway: none

  1. To check if the IP address change was successful, use the DOS command prompt ( Start/Run/cmd ) and enter the ipconfig command. Check if the new IP has been entered correctly. If it is not correct, please repeat the configuration steps.




#### Instructions for Windows 7 and newer Windows versions

The data logger is programmed with a fixed IP address. Unless the network is set up to assign the correct address to the PC via DHCP, define a fixed IP address for the PC to allow the two devices to communicate

  1. Connect the PC to the data logger via Ethernet link. 

  2. Click on Start and then click on "Run" or press the following Key combination of the keyboard: "Windows Logo button and R"

  3. In the text field enter "ncpa.cpl" and then click ok.

  4. Right click on the Network connection to be used to download (more than one may be available).

  5. Click on Properties and then select Internet Protocol (TCP/IP) from the list of items. 




  5. Take note of the settings found here. Configure them later when the PC has to be connected back to the network.

  6. Choose an IP address on the same subnet as the data logger.

  7. Configure the IP address of the PC as follows, e.g.:

  8. IP address: 192.168.1.xxx Subnet mask: 255.255.255.0 Default gateway: none

  9. To check if the IP address change was successful, use the DOS command prompt ( Start/Run/cmd ) and enter the ipconfig command. Check if the new IP has been entered correctly. If it is not correct, please repeat the configuration steps.




###  Check the communication link with the Data Logger

  1. Run the FindDevs program.

  2. A device identification message like the following one should be available:

NameLogger_SN/DeviceAlias/(ABS, No Table) The labels in the identification string give information about the device and the current status of logging SN = logger serial no. ABS = absolute lap counter index Alias = a label which the user programs in the device from Sysma, typically the chassis number or device name The logging status is as follows:

| STATUS | DESCRIPTION | CAUSED BY |
| --- | --- | --- |
| No Table | Acquisition table is not loaded | First use, after format or clear data |
| TrigIn | Trigger In | When the logging trigger condition is true (i.e. logging active) or the trigger is not configured |
| TrigOut | Trigger Out | When the logging trigger condition is false (i.e. logging not active) |
  3. As an alternative open a prompt command and enter the ping command followed by a space and data logger IP address.

  4. To work back on the network, select the appropriate settings from Internet Protocol (TCP/IP) Properties window.




This is the TCP window properties in version earlier than Vista