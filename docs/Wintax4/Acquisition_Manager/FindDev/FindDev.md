## Find Devices on the Network

The Find Devices on the Network (or FindDevs) is the Marelli utility used to discover devices on the network. Devices means for example DataLogger, ECU, Modules, Telemetry Ground Receiver or the Telemetry Data Server application. Through “Find Devices on the Network” the PC applications such as WinTAX4 or SYSMA are able to communicate together or with hardware devices.

The application “Find Devices on the Network” is therefore the module responsible for displaying all connected devices on the network. The application starts automatically at the first request of devices from applications that use them (e.g.: WinTAX4, SYSMA). The application automatically closes after few minutes from closure of the last application connected.

The application starts minimized as a tray icons, it can be called up by double-click or right-click on it.

The application is divided into two tabs:

  * Online Devices: shows all currently connected devices on the network.

  * Historical Devices: shows the history of devices that were connected on the network.




For each device, it displays the following properties:

  * Device: device name;

  * IP Address: device IP address;

  * Alias: alias name;

  * Status: device status (for Datalogger devices: ABS counter value, Acquisition Trigger status).




When a device is detected on the network, the application shows a notification.

When a device is disconnected from the network, the application displays a notification.

### Commands

All Commands and functions to manage the application are available in menu, contextual menu integrated in the window, contextual menu in the tray icon and toolbar.

#### File

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Save As | CTRL + S | Save to a text file (*.txt) the list of displayed devices (online or historical). |
| Exit |  | Closes the application |
  
#### Edit

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Copy to Clipboard | CTRL + C | Copies to clipboard the list of displayed devices (online or historical). |
  
#### View

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Auto size columns |  | Auto fit size of the columns to the cell contents. |
| Toolbar |  | Shows/hides toolbar |
  
#### Options

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Clear historical devices |  | Clear the list of historical devices. |
| Show notifications |  | Shows/hides notifications in tray. |
| Always on top |  | Puts the application in front of all other applications. |
  
#### Help

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| About FindDevs |  | Shows the application info. |
  
### 

### **Contextual commands**

Clicking on the icon in the tray opens the following menu

The commands are the same as in the list above, except the following:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| View online devices |  | Shows the list of online devices detected on the network. |
| View historical devices |  | Shows the list of historical devices |
  
Clicking on the Online Devices windows opens the following menu

All the menu commands have already been described above.

Clicking on the Historical Devices windows opens the following menu

All the menu commands have already been described above.