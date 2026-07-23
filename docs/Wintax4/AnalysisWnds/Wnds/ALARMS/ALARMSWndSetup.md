### Configuration Window

The [Alarms](ALARMSWndOverview.md) window has no real configuration interface since only Auto Search Alarms feature can be configured. This configuration is the same that can be edited from [Tools/Alarms/Edit](../../../Menu/Tools.md#MenuTools_AlarmsSubmenu) menu.

Only the Post Processing section is used for the configuration of this window. The other section is used to enable the display of alarms during real time analysis using [Event Report window](../EVR/EVRWndOverview.md) and [Popup Alarms](../../../WTXWorkWindows/PopupAlarms/PopupAlarms.md). See [Tools/Alarms/Edit](../../../Menu/Tools.md#MenuTools_AlarmsSubmenu) menu for further details.

#### Post Processing section

In this section the user can configure the different display mode of the Alarms Window. The window performs automatic search of alarms whenever a new lap is loaded or when an [Auto Rx](../../../Acquisition_Manager/Rx_tasks/Rx_tasks.md#AutoRx) task is received.

  * Default: The default mode, without check selected, updates Alarm Window only if window is already open. 

  * Alarms: If only Alarms check is selected, the Alarm window is automatically displayed every time a channel exceeds the alarm configured thresholds. If the window was already open, it updates the values. If there are no alarms the window is automatically closed.

  * All: If All check are selected, the window will be open (or updated if already open) and shows all configured alarm channels even if they are not in alarm. This check is disabled if Alarms are disabled too.




##### 

##### Related Issues

  * [Alarms window](ALARMSWndOverview.md)

  * [Elements of the Window](ALARMSWndItems.md)

  * [Commands](ALARMSWndCommands.md)


