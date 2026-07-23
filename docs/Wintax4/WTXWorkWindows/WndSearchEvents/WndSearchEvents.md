## Search Events

This dockable window allows to search for individual or multiple events.

In the definition of each event it is possible to set up a zoom range that is automatically applied when the event is searched. A time offset parameter is used to place the cursor and set the zoom range at a predefined time before or after the event. It is also possible to set up a specific layout which will be opened when the event is detected. By default the current layout is maintained.

Above the buttons there is an info line indicating how many occurrences the event has, the currently pointed one and the moment associated with it. The window is also provided with a combo allowing to choose if the occurrences of the event must be searched following a chronological order or event by event.

If the "by event" is selected, the buttons of the window have the following functions:

  * Home




Moves the cursor to the first occurrence of the selected event if it is checked; if it is not checked the command is active on the first checked event next to the selected one.

  * Previous




Moves the cursor to the previous occurrence of the selected event. If no other occurrences of the same event are available, the last previously checked one is selected. It stops at the first occurrence of the first checked event. If previous is pressed when a non checked event is selected, the last occurrence of the last checked event previous to the selected one is chosen; if none is available, the last occurrence of the last checked event is chosen.

  * Next




Moves the cursor to the next occurrence of the selected event. If no occurrences for the same event are available, the first occurrence of the next checked occurrence is selected. It stops at the last occurrence of the last checked event. If it is pressed when a non checked event is selected, the first occurrence of the first checked next to the selected one is chosen; if no next checked events are available, the first occurrence of the first checked event is chosen.

  * End




Moves the cursor to the last occurrence of the selected event if it is checked; if it is not checked the command is active on the first checked item following to the selected one.

  * GoTo




If pressed when a checked event is selected, the cursor moves to the indicated occurrence.

  * Step




Step for the Prev and Next command. It allows to scroll quicker the occurrences when many of them are available.

  * Auto




Moves the cursor to the first occurrence of the event Configured as Auto Search in the Search tab in Tools/Events/Default Settings

  * Restore




If in the configuration of an event the opening of a certain layout is programmed whenever an occurrence takes place, this button is enabled so that the default configuration, previous to the one taking place on the event is restored.

If chronologically is selected, the buttons of the window have the following functions:

  * Home




Moves the cursor to the occurrence with the shortest time instant among all the checked ones.

  * Previous




Among all the checked events, moves to the occurrence coming previously, no matter which event it belongs to.

  * Next




Among all the checked events, moves to the occurrence coming next, no matterwich event it belongs to.

  * End




Moves the cursor to the occurrence with the highest instant among all the checked ones.

  * GoTo




Disabled

  * Step




Disabled

  * Auto




Same functions as by event

  * Restore




Same functions as by event