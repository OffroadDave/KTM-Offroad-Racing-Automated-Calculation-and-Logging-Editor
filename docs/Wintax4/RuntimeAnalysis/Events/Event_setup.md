### Events Setup

The event setup environment allows to create and edit expressions (like [Virtual Channels](../VCH/VCH_libraries.md)) and save them in libraries (.evn files). Event definitions can be also created from existing [diagnostic window](Diagnostics.md) configurations.

Events are organized in groups (e.g. gearbox events, diagnostic events etc.) inside the libraries. The group name appears beside the event name when it is displayed.

Events are typically defined via boolean expressions such as those used for Conditions. Complex conditions can be built up using math channels. To avoid duplicating expressions, the event trigger (status transition) can also be defined allowing a single Condition expression to be used to define an event and its negation.

WinTAX4 also provides some pre-defined [system events](System_events.md) .

Any filtering or time-masking of multiple transitions ("debounce") must be handled within the definition of the status variable itself, not within the event configuration. Some specific math channel functions (e.g. [mask](../VCH/Functions/VCH_functions.md#vch_Mask) ) be introduced to simplify the definition of conditions to be used to generate events.

This is the main page of event configuration.

#### Libraries

##### Loading Events libraries

  1. Select File/Load from the Main menu and load or create one or more Event Library files. The libraries which are currently loaded are listed on the left of the editor window. The path for the highlighted library is shown on the right. The libraries are local to the User.

  2. Use the read only flag to prevent entries in the library from being modified. Libraries which are not saved in the local Libraries directory are always read only. They are generally shared between users.




#####  Removing Events libraries

  1. To remove a library from the list simply right click on it and select Cut ( ctrl+X ) or type delete




Note: removing a Event Library from the list will not delete it from the disk

#### Defining and displaying events

##### To create an event

  * Select Tools/Events/Configure

  * Create a new event library: select File/Load and type the name of the library to be created (e.g. myLibrary). This will create a library with the given name which contains a group called Default.

  * Change the name of the group from Default into any other name, then highlight the group.

  * Select Edit/Add item or click on the item toolbar button to add either an empty event or a system event.

  * Edit the event configuration by assigning a name, the logic of the event transition and the expression on which it is based. Note that events are generated when the value of the expression changes from zero (false) to any other value (true), or vice versa depending on the logic defined.




  * Verify that the Show flag is checked. This ensures that the event is enabled and will be visible in the channel browser. Any events not to be used from within the library can be disabled by unchecking the Show flag.

  * Click on Graph Option to set any specific display options for the particular event. If the "Graph Options" check is not enabled, the event will be displayed using the default options which are defined in [Tools/Events/Default Settings](../../Menu/Tools.md#MenuTools_Event).




In the page Graph Options it is possible to configure the shape, the size and the color of the event tick markers. It is also possible to set tooltip information, special effects display and actions to be taken when the event occurs. The automatic zoom and cursor offset are used when using the event search function. In Marker channel section it is possible to configure associated channel values to be shown in tooltips.

##### To display an event

  1. Select the Events tab in the Channel Browser

  2. The list shows all the event groups of the current libraries and the events which are enabled by the Show option the event graph options

  3. To display an event just drag it from the Channel Browser into the window . The event icon will change to green.

  4. Select any graph window then the Show events toolbar button or Tools /Events/Show events. This option switches all events on/off in the current window.




Note: 

a) If event display is switched off in all or any graph windows, it does not disable event calculation, it will still be possible to use the Search event and Event report functions.

b) Dragging event on the window by holding down the Ctrl button key, the event will be displayed as if it was a channel.

##### To enable and disable events

  * All events - Press Ctrl+E or click on Tools/Events/Enable/Disable Events.

  * Single events - To enable or disable on a "per event" basis, right click in the channel browser and toggle off "Show events in all Windows". Events can also be enabled and disabled via the show flag in the "Events setup" dialog (right click on a event and then click "Edit Events").

  * Events (on a per window basis.) - events can also be enabled and disabled on a "per window basis". To customize on a per window basis, right click on a graph window and enable or disable the "Events/Show Events" item. If disabled, all events will not be calculated only in that window.




##### 

##### Time Offset from Event

This field is used for: 

  * Calculation of channel markers values.

  * Event searching through "search events"




In more detail, when the value of the channel marker is being calculated or the cursor is positioned by using the "Search events" feature, the "Time Offset from Events" is added to the Events time.

Note: Time Offset from event cannot be applied for standard positioning of the markers in the Analysis window.

#####  Global/Local Libraries

  1. Select the library

  2. Right click and select Switch to Global (or Switch to Local if already enabled.) or select "Options / Switch to Global/Local"

  3. Click OK to confirm at the prompt

  4. The selected library will now be visible within all user environments if selected as Global or will now be visible only to the current user if selected as Local.




##### 

#####  Load library for all users

  1. Select Options/Load library for all users to load one or more libraries.

  2. The selected libraries will now be copied within all user 




#####  Removing Event libraries

  1. To remove a library from the list simply right click on it and select Cut ( ctrl+X ) or type delete




Note: removing a event library from the list will not delete it from the disk

#####  Save as encrypted

Allows the creation of encrypted event channels (EVN), where formulas are hidden and protected. The channel name, trigger, options and comment remain visible, but the expression formula is replaced by ENCRYPTED (read-only). Recipients can load the encrypted files via Channel Browser but cannot view or modify the internal math.

##### Creating Encrypted Files

Use the Save As Encrypted command, available via:

Right-click menu on individual EVN libraries

The encrypted file will have a .ECR extension

An encrypted ECR file will have priority over an unencrypted Event Library

When creating an encrypted library, it is possible to include in the encrypted library a "related" lookup table.

| Command | Description |
| --- | --- |
| Browse | Allows the selection of the destination for the encrypted file. |
| Add | Adds a selected Lookup table from a list of available lookup tables. |
| Add All | Adds all lookup tables configured in WinTAX. |
| Remove | Removed the selected lookup table. |
| Remove all | Removes all (previously) added lookup tables. |
| Encrypt to license | On selected licenses it is possible to select a destination license which uses the same encryption method. |
  
##### Loading Behavior

Encrypted files are read-only. Modifications must be made to the original file and re-encrypted

Formulas used in the encrypted library are hidden wherever shown and will appear as "Encrypted". (e.g., Channel Browser tooltip).

A dedicated icon is visualized when an encrypted library is loaded.

##### Important notes:

  * Encrypted libraries cannot be edited. It's also not possible to view or edit the expressions of the channels in an encrypted library.

  * Encrypted files are not auto-loaded into Wintax (to avoid duplicates in the Channel Browser).




Linked Lookup channels are included automatically.

If User Records are present, a dialog prompts for lookup selection, when saving the encrypted file.

Default Events Setup:

With the **Tools - Events - Default settings** command, a default Event window opens. This window allows you to configure the default settings that will be used when creating a new Event. Here you can define a template to be applied to all newly created events. 

You can select a custom audio file to be used as the event sound. This sound will be played for all events created using these default settings. 

.