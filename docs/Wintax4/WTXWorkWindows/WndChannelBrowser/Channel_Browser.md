## Channel Browser

The Channel Browser is shown or hidden through the View/Channel Browser command of the main menu. The window is dockable and can be blocked in any of the four sides of the main window. The Channel Browser is a folder displaying all types of channels, logged, math, events, etc. Besides displaying a list of channels available, its function is to support all the drag & drop operations that are necessary to show the different channels on the [analysis windows](../../AnalysisWnds/AnalysisWndsOverview.md) . To this aim the window has the special feature, both in floating mode and in docking mode, of being accessible also from the modal windows of the configurations, so that the drag & drop is possible not only on the windows but also on their configurations.

The drag & drop, i.e. the dragging of the channel with the mouse on the wished place, is not the only way to add a channel in a window;

Edit box on the top of the list can be used to search for channels. The channels list is updated runtime, while editing characters, showing channels which name matches the filter string. The search is case insensitive but you can still use the Shift key to edit the uppercase letters. Filter can be deleted by keyboard or button Reset.

In the complete configuration the Channel Browser is provided with following Tabs: Channels, Info, Events, OLE, ChannelsRT, Constants, Math, Cond, Groups, Local Groups, Import, WDS and Variables. However not all tabs are available on different licenses.

#### Channels

The Tab Channels includes the logged channels. Besides the basic functions, for the logged channel clicking with the right button enables the access to a pop-up menu that includes the following commands

  * Sort If the check of the Sort command is enabled the channels are in alphabetical order, otherwise they appear according to the loading order. The status is saved and kept till the next loading.




#### Info

The Info Tab contains all information associated to the lap and the meteo data if available. Besides the basic functions, for the Info clicking with the right button enables the access to a pop-up menu that includes the following commands

  * Sort If the check of the Sort command is enabled the channels are in alphabetical order, otherwise they appear according to the loading order. The status is saved and kept till the next loading




#### Events

The Event Tab contains all configured [events](../../RuntimeAnalysis/Events/Events.md) that have the Show parameter enabled. In addition, the events are grouped on the basis of the group they belong to. The group unlike the event cannot be dragged on a window. it is also possible to drag on a window the event pressing the CTRL key; in this case it will not be considered as an event but as math channel and it will be regularly graphed.

When an event is in red, it means that it is configured to be a Hot Event, i.e. an event where the occurrences can be scrolled with the shortcuts ALT+P and ALT+N. There can be just one Hot Event so the setting of a Hot Event removes this feature to another event that previously had this feature.

Channels, that are unavailable for syntax error(s) in formula, will be displayed with a grayed icon overlapped by red cross. Moving the mouse cursor over items, a tooltip with error description will appear.

When the icons of the events are colored in green, it means that the active analysis window displays these events.

Besides the basic functions, clicking with the right button enables the access to a pop-up menu that includes some of the following commands, depending on the licenses.

  * Edit Events Opens the configuration page placed on the selected event. If no event is selected, the configuration page set on the first group of the first library is opened.

  * Enable/Disable events: Enables/disables events to be displayed the display of the events on all WinTAX4 windows. Available only for some licenses.

  * Show Event When it is enabled, displays the event on all windows managing the events. If it is not enabled, each window displaying its own event works normally. The global event is identified by the icon that shows a blue circle behind the usual symbol of the events. (Enabled only for some licenses.)

  * Hide Event Removes the event from the Channel Browser, hiding it throughout all WinTAX4. It decreases the flag of the Show configuration parameter. To show the hidden event again, the configuration flag must be increased again. It is enabled only for some licenses.

  * Set As Hot Event Sets the selected event as an Hot Event..

  * Clear Hot Event Removes the Hot Event status from the event possibly having this feature.

  * Sort If the check of the Sort command is enabled, the events are in alphabetical order, otherwise they appear according to the loading order. The status is saved and kept till the next loading.




#### OLE

The OLE tab includes the channels added to WinTAX4 from outside through [OLE Automation](../../Automation/OLE_AUTOMATION.md) or also by WinTAX4 itself through the script.

Besides the basic functions, for the logged channel clicking with the right button enables the access to a pop-up menu that includes the following commands

  * Sort If the check of the Sort command is enabled the Info are in alphabetical order, otherwise they appear according to the loading order. The status is saved and kept till the next loading




#### ChannelsRT

The ChannelsRT tab contains the channels that are acquired through Real Time; the icon is the same as the logged channels, only in green. Besides the basic functions, for the logged channel clicking with the right button enables the access to a pop-up menu that includes the following commands

  * Sort If the check of the Sort command is enabled the OLE channels are in alphabetical order, otherwise they appear according to the loading order. The status is saved and kept till the next loading.




#### Math

The Math tab contains all configured [Virtual Channels (VCH)](../../RuntimeAnalysis/VCH/VCH_libraries.md) and where the Show parameter is checked in configuration. Besides the basic functions, for the logged channel clicking with the right button enables the access to a pop-up menu that includes the following commands

  * Edit Virtual Channels Opens the configuration page placed on the selected VCH. If no VCH is selected, the configuration page set in the first library is anyhow opened.

  * Sort If the check of the Sort command is enabled the VCH are in alphabetical order, otherwise they appear according to the loading order. The status is saved and kept till the next loading.




#### Cond

The Cond Tab contains all configured [Conditions](../../RuntimeAnalysis/Conditions/Conditions.md) having the Show parameter checked in configuration. Besides the basic functions, for the logged channel clicking with the right button enables the access to a pop-up menu that includes the following commands

  * Edit Conditions Opens the configuration page placed on the selected Condition. If no Condition is selected, the configuration page set in the first library is anyhow opened.

  * Sort If the check of the Sort command is enabled the conditions are in alphabetical order, otherwise they appear according to the loading order. The status is saved and kept till the next loading.




#### Groups

The Group Tab contains a combo which allows selecting among various groups of channels into which the logged channels have been divided. The list shows all channels belonging to that group.

Besides the basic functions, for the logged channel clicking with the right button enables the access to a pop-up menu that includes the following commands.

  * Sort If the check of the Sort command is enabled the channels are in alphabetical order, otherwise they appear according to the loading order. The status is saved and kept till the next loading




#### User Record

The User Record Tab contains the user records configured in acquisition manager and then saved within acquired data. As for all other Tabs, the sort option is available by mouse right click:

  * Sort: If checked the items are displayed in alphabetical order, otherwise they appear as saved into data . Sort status is kept in memory until next lap is loaded.




#### Import

The Import Tab contains the channels [imported from ASCII](../../DataBrw/ImportAscii.md) and stored in file ImportData.ztx. As for all other Tabs, the sort option is available by mouse right click:

  * Sort: If checked the items are displayed in alphabetical order, otherwise they appear as saved into data . Sort status is kept in memory until next lap is loaded.




#### WDS

The WDS Tab contains the channels acquired from [Wireless Distribution System](../../Acquisition_Manager/WDS/AcqMng_WDS.md). The WDS channels are of four basic types: CAR, DOWN, UP and ACCESS POINT. Each type is identified by a different icon. These channels are merged with the vehicles contained in WDS Car Toolbar to obtain the list of logged channels in Channels Tab.

As for all other Tabs, the sort option is available by mouse right click:

  * Sort: If checked the items are displayed in alphabetical order but grouped by type; otherwise they appear as saved into data. Sort order is kept in memory until next lap is loaded.




#### Local Group

The Local Group Tab contains the groups configured in [Channel Parameters](../../WTXSetup/WndChannelsParametersSetup/LocalGroups.md). Dragging a Local Group on a Graph Window, all channels belonging to the group are loaded on the window.

As for all other Tabs, the sort option is available by right clicking with the mouse:

  * Sort: If checked, the items are displayed in alphabetical order, otherwise they appear as saved into data. Sort status is kept in memory until next lap is loaded.




#### Variables

The Variables Tab contains the [static](../../RuntimeAnalysis/VCHEx/Functions/statements.md#vch_StatStatic) variables. The drag and drop of Variables is allowed only on [Setup of Virtual Channels](../../RuntimeAnalysis/VCHEx/VCH_libraries.md) but not on the analysis windows.

As for all other Tabs, the sort option is available by right clicking with the mouse:

  * Sort: If checked the items are displayed in alphabetical order, otherwise they appear as saved into data. Sort status is kept in memory until next lap is loaded.




## 

### Custom Mode

In some licenses a further Tab called Custom is available allowing to group various channels in a single position.

For further details on the Custom tab see the [Setup Channel Browser](../../WTXSetup/WndSetupChannelBrowser/WTXSetupChannelBrowser.md) .

In this mode three types of arrangement are available

  * The arrangement by default places the channels groups arranged by the Setup configuration; in each group the channels are arranged according to the loading order. it is available when the Sort by Name and Sort by Type commands are not checked,

  * Sort by Name arranges the channels in alphabetical order

  * Sort by Type arranges the channels in alphabetical order but for each group, keeping the order of the group in the Setup.




### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window; the following list of priority determines what type of channel is shown:

  * Logged channels

  * Constants

  * Info Fields

  * Informations - (Header Properties - Info)

  * Ole

  * RunTime Math DLL

  * Math DLL

  * VCH (Virtual Channels)

  * Conditions

  * Events

  * Alias

  * User Record

  * Variance




If 2 or more channels that have the same name also have the same type of data, WinTAX4 will choose the first channel found when data is loaded.

### Filtering functions

Exclusion mode: 

Adding "-word" after a search term (e.g., wheel -temp) will show only channels containing "wheel" but not "temp".

Exact match mode: 

Adding a space after the search term will show only channels that exactly match the text entered.

Wildcard mode:

The "?" character matches any single character (e.g., wh?el → "wheel", "whAel"). The "*" character matches any sequence of characters (e.g., wh*el → "wheel", "whale", "whatevel").