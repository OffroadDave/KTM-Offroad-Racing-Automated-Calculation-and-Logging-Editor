## Assetto Corsa Plug-In installation

##### Getting Started Guide

The "Assetto Corsa Plugin” allows WinTAX to acquire and display real-time data coming from the simulation executed with "Assetto Corsa". The Plug-In is a software component that allows the broadcasting of telemetry data that can be acquired using WinTAX to receive real-time data for viewing and analysis. The software modules involved are:

  * Assetto Corsa: third party racing simulator
  * WTS Server with Assetto Corsa Plug-In: Module responsible for broadcasting real-time data acquired from Assetto Corsa to WinTAX4
  * WinTAX4: data analysis tool to acquire, display and save acquired data.



#### How to install the WTS & Assetto Corsa modules

##### WTS: First time installation

  * Run Setup.Exe from the installation package on the computer where Assetto Corsa will be executed.
  * After completion, run WTSServer (from Desktop icon)



##### WTS: Reinstallations and upgrades:

  * Close WTSServer
  * Run Setup.Exe from installation package
  * Select the "Repair" setup type.
  * After completion, run WTSServer (from Desktop icon)



### Start-up of WTS

  1. Run WTS Server from the icon on the desktop or in the "Start" menu.

  2. On the "Stream Setup" Page click on the button with the "Assetto Corsa" logo as indicated in the following picture:

  3. A windows browser folder will appear: go in WTS installation folder (C:\Program Files (x86)\WTS) 

  4. Select the module AssettoCorsaPlugin.dll.

  5. Click to open

  6. a Stream named "AssettoCorsaDSTStream" is generated.

  7. Check the box field on the left of the column name to activate the stream.

  8. The server is running. A Microsoft window popup notification will show a popup message.

  9. No more actions are required.

  10. Clicking on the minimize button will minimize WTS server in the Windows tray. The software is still running.

  11. Clicking on a "X" button will stop the program and terminate the WTS connection.




### Getting Started Guide

  7. At this point the server is started. Eventually, (depending on the version of Windows) a Microsoft pop-up notification will notify that the Server is started. (A notification similar to the one in the image below) 
  8. No more actions are required
  9. Clicking on the “_” button in the right top corner of the WTS window will minimize it in the Windows tray bar (Execution status = ON)
  10. Clicking on the “X” button in the right top corner of the WTS window will stop and terminate WTS execution



###  Setup of WTS

  1. The Default stream name proposed is AssettoCorsaDSTStream. This name will appear in WinTAX when starting the real-time acquisition. To change it:


  1. Disable the checkbox near the stream name.
  2. Double click the stream name to edit the name
  3. Edit the name following your needs
  4. Activate again the streaming check if needed. (as seen in previous chapter "Startup of WTS").


  2. The Default server name is the computer name. This name will appear in the setup of WinTAX real-time acquisition. To change it:


  1. In the second TAB (Configuration), click on STOP button
  2. Edit the name of the Server
  3. Start again the Server



The network parameters in the Configuration page (Status Port, Multicast Port, Multicast Address, Local IP bind) should be modified only by the administrator if needed. Unless you know what the settings for your network are, leave the default settings that are proposed during installation. The software has already the correct settings for almost all environments.

### WinTAX4 installation

Please follow [WinTAX installation guide](../../Installation/Installation.md#InstallationFirst).

### Start-up of WinTAX

Run WinTAX4 from the icon on the desktop or from the program menu. Now click on Acquisition and then Acquisition Manager and the following window will appear.

Right click on the toolbar and enable the "Real-Time Bar" if it is not already enabled. The "Real-Time Bar” will appear as shown in the bitmap below. Then press the "setup" Symbol highlighted below with a red arrow. 

Configure the settings like in the picture below. In the WTS Server, enter the IP address of the computer that acts as WTS server.

Finally leave the setup page using the "File - Apply" command or use the specific green check.

In the “Real-time” toolbar, select the name of the stream from the combo box list.

When the icon changes color to green, the link between WTS and WinTAX4 has already been established.

### Real Time data analysis

On the acquisition manager go to Acquisition Setup - Options. If the real time telemetry is properly started, select “Toggle Channel Browser” on the WinTAX4 main window, then select on the WinTAX4 channel browser the Tab “Channels RT”. It will be populated with the list of available real time channels. To show a channel on an analysis window you can drag and drop from the list to the specific window. Otherwise, you can also select it by double clicking on the channel or with the “Enter” key after selection. The channel search can be made easier using the browser box.

When opening a new window, the default is for Post Processing use. Once the window is open, using the keyboard shortcut "CTRL+ T" (with focus on the window) it is possible to switch between real time and post processing mode.

In case of post processing mode, the window sides are blue.

It's possible to save Window positions and sizes by clicking on File - "Save Layout As" or File - "Save Layout".

Also, a specific configuration of window and its properties can be stored by the command

File - "Save Window As" or File - "Save Window".

To edit window properties right click on the selected window and select “Properties”.