## rFactor Plug-In installation

##### Getting Started Guide

The "rFactor Plug-In” allows WinTAX to acquire and display real-time data and parameters coming from the simulated car. The Plug-In is a software component that can be used to configure rFactor to send real-time data and WinTAX to receive the same data for viewing and analysis. The software modules involved are:

  * rFactor: third party car modelling system
  * WTSrFactor Plug-In: management core of real-time data
  * WTS: WinTAX Telemetry Server. Module responsible for sending real-time car modelling data to WinTAX4
  * WinTAX4: data analysis tool

Data directed to WinTAX flows in a schema similar to the following sample:

#### How to install the WTS & WTSrFactorPlugIn modules

##### WTS: First time installation

  * Run Setup.Exe from installation package
  * Select Setup Type = Complete
  * After completion, run WTSServer (from Desktop icon)



##### WTS: Reinstallations and upgrades:

  * Close WTSServer
  * Run Setup.Exe from installation package
  * Select the "Repair" setup type.
  * After completion, run WTSServer (from Desktop icon)



######  WTSrFactorPlugIn Module installation

The installation of WTS generates in the WTS installation directory the following files:

  * WTSrFactorPlugIn.dll

  * WTSrFactor2PlugIn.dll

  * rFactor2PlugIn64Engine.dll




These files must be manually copied in the rFactor PlugIns folder.

  * rFactor: Copy the file “C:\Program Files\WTS\WTSrFactorPlugIn.dll” in rFactor PlugIns folder (usually C:\Program Files\rFactor\PlugIns”) 

  * rFactor2: Copy the file “C:\Program Files\WTS\ rFactor2PlugIn64Engine.dll” in rFactor2 64bit PlugIns folder (usually “C:\Program Files (x86)\Steam\steamapps\common\rFactor 2\Bin64\Plugins”) 




### Start-up of WTS

  1. Run WTS Server from the icon on the desktop or in the "Start" menu.

  2. On Stream Setup Page click on the "+" button to “Add rFactor Source”

  3. A windows browser folder will appear: go in WTS installation folder (C:\Program Files\WTS) 

  4. Select the module WTSrFactorPlugIn.dll if you want to use rFactor or select the module WTSrFactor2Plugin.dll if you want to use rFactor2.

  5. Click to open

  6. a Stream named rFactorDSTStream is created

  7. Check the box field on the left of the column name to activate the stream.

  8. The server is running. A Microsoft window popup notificaton will show a popup message.

  9. No more actions are required.

  10. Click on the minimize button will minimize WTS server in the Windows tray. The software is still running.

  11. Clicking on a "X" button will stop the program and terminate the WTS connection.

  12. 


  5. check the box field on the left of name column for activating the stream




### Getting Started Guide

  7. At this point the server is started. A Microsoft pop-up notification will notify that the Server is started. (check image below) 
  8. No more actions are required
  9. Clicking on the “_” button in the right top corner of the WTS window will minimize it in the Windows tray bar (Execution status = ON)
  10. Clicking on the “X” button in the right top corner of the WTS window will stop and terminate WTS execution



###  Setup of WTS

  1. The Default stream name proposed is rFactorDSTStream. This name will appear in WinTAX when starting the real-time acquisition. To change it:


  1. Leave the green check on the stream checkbox
  2. Double click the stream name to edit the name
  3. Edit the name following your needs
  4. Activate again the streaming check if needed. (as seen in previous chapter "Startup of WTS").


  2. The Default server name is the computer name. This name will appear in the setup of WinTAX real-time acquisition. To change it:


  1. In the second TAB (Configuration), click on STOP button
  2. Edit the name of the Server
  3. Start again the Server



To change the list of channels exported by the WTSrFactor Plug-In you have to modify a system “rFactorChannels.xml” file. It is located in C:\Program Files\WTS\System\ directory. The XML is a plain text file that can be opened using standard Window editors such as Notepad. Before changing this file the WTS Server must be stopped.

The list of channels should be modified only by the administrator.

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

On the acquisition manager go to Acquisition Setup - Options. If the real time telemetry is properly started, select “Toggle Channel Browser” on the WinTAX4 mainframe, then select on the WinTAX4 channel browser the Tab “Channels RT”. It will be populated with the list of available real time channels. To show a channel on an analysis window you can drag and drop from the list to the specific window. Otherwise you can also select it by double click on the channel or with the “Enter” key after selection. The channel search can be made easier using the browser box. The list of WinTAX4 analysis windows available for Real Time use is:

  1. .GRA window(graph window)
  2. .CHN window(channel window)
  3. .BGR window (bargraph window)
  4. .CGR window (circular graph window)



When opening a new window the default is for Post Processing use. Once the window is open, using the keyboard shortcut "CTRL+ T" (with focus on the window) it is possible to switch between real time and post processing mode.

In case of post processing mode the window side is blue.

It's possible to save Window positions and sizes by clicking on File - "Save Layout As" or File - "Save Layout".

Also a specific configuration of window and its properties can be stored by the command

File - "Save Window As" or File - "Save Window".

To edit window properties right click on the selected window and select “Properties”.

### Appendix A: list of channels exported by WTSrFactorPlugIn (rFactor)

The WTSrFactorPlugIn module will export the following list of channels: 

####  Formulas for channels calculation

These Channels are calculated by WTSrFactorPlugIn:

  * GyroPitchAngle=at2(-MidRow_mOriY_Z, sqrt((TopRow_mOriX_Z^2) \+ (BotRow_mOriZ_Z^2)))
  * GyroRollAngle=at2(MidRow_mOriY_X, sqrt((TopRow_mOriX_X^2) \+ (BotRow_mOriZ_X^2)))
  * GyroYawAngle = asn(BotRow_mOriZ_X)
  * Damper_Velocity_FL = derivative of suspension pos FL
  * Damper_Velocity_FR = derivative of suspension pos FR
  * Damper_Velocity_RL = derivative of suspension pos RL
  * Damper_Velocity_RR = derivative of suspension pos RR
  * GForceLat = LocalRotAccelX / 9.80665
  * GForceLong = LocalAccelZ / 9.80665
  * LapDistance = integral of speed



###  Appendix B: additional channels exported by WTSrFactor2PlugIn (rFactor2)

The WTSrFactor2Plugin module will export these additional channels: