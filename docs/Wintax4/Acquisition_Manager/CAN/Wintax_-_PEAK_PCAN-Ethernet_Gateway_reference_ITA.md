### PCAN Ethernet Gateway quick setup

The PEAK PCAN-Ethernet gatewat allows the connection to a CAN network using a virtual adapter not fisically connected to the computer but available on the ethernet network. This page will describe the main operations that the user needs to do in order to start the acquisition with WinTAX using this device.

###### Installation of the PEAK driver on the local PC

PEAK Drivers can be automatically installed by WinTAX, but the user can also manually install / upgrade the drivers by downloading it from the website and automatically launching the setup.exe. and enabling the Virtual PCAN-Gateway. 

###### Configuration of the PCAN-Ethernet Gateway

To configure the interface the user will need to connect to the device default address: <http://192.168.1.10>

Default user: admin

Default password: admin

In the lan section configure the following fields so they are compatible with your network configuration: IP of the device + subnet mask + gateway / DHCP

In the CAN section, configure the can lines opening baudrate. 

Routing section: Configure the routing rules:

For each phisical CAN, you will need to add 2 routing rules:

· 1 from the gateway to the destination PC.

· 1 from the PC to the local port / phisical channel

In these windows it's possible to define if the type of communication is going to be of TCP or UDP type and the listening and transmission ports.

###### Configuration of the Virtual PCAN-Gateway on the computer where Wintax4 will be used:

Open the configuration tool for the Virtual PCAN-Gateway.

For each virtual CAN port that needs to be received on the PC, 2 routing rules need to be configured:

Input routing for a CAN channel (ports and type of connection need to be equal to the ones configured on the PCAN-Ethernet gateway.) 

Output routing for each CAN channel: (ports and type of connection need to be of the same type of the rules that have been configured on the PCAN-Ethernet gateway device:

How to test the configuration:

To ensure that the settings have been configured correctly, it's possible to use the CANView tool, which is available on all computer where the drivers have been installed, opening the "virtual" ports that have been detected automatically.

NOTE: on the PCAN ethernet gateway devices, it is not possible to configure the baud rate for opening the line (as it's possible to see below, the selection box is disabled), because this setting is already configured in the web configuration interface of the device.

STEP5 – Configurazione di WinTax4

The configuration of the CAN acquisition corresponds exactly to the CAN-USB configuration. The only thing that is required to be done differently from the USB mode, is to select as device the item "“Peak Ethernet Gateway”.

When this mode is selected, it's not possible to define the line opening baud rate setting.

During the acquisition activation phase, in the log window a list of the settings received from the device will be visualized:

Troubleshooting

During the development phase, it has been noticed that in some situations, specially when communication parameters have been changed on the device, it seems that data reception stops working.

To resolve this problem, we noticed that clicking on the button “Reload” available on the “Virtual PCAN-Gateway” this problem is always solved: