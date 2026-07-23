### SECU Events

SECU event means an event coming from a SECU system. An event is configured in a MES layout and it cannot be changed using WinTAX.

Basically SECU events are sets of events externally computed and then transmitted to WinTAX4. They are organized in three logical groups, distinguished by priority levels: low, medium, high.

Each group could contains several events, as usual for all other type of events.

These events can be added to a library with Options/Add... command. As explained above, the algorithm of these events cannot be changed by the WinTAX4 user, but it is possible to set up the graphical Options. SECU Events cannot be displayed as traces into graphical windows, but only with event markers.

Events can be masked so that they are not displayed:

  * in Events Report windows, by right clicking onto a row and then by choosing Mask Secu Event command from popup menu.

  * In Graph windows, by right clicking on a marker event, when the tooltip appears; a dialog which contains the list of all occurred events at this time, will be displayed. User can choose the event that should be hidden.




Masked event are not visible in all waveforms.

In order to unmask the events user has to unselect the entry from the list of masked events. This list is available in events setup dialog, one for each groups

During the analysis work, events of the same group can be hidden singly:

  * with Events Report windows, right clicking on a row where an occurrence of the event to hide is recorded, and then choosing Mask Secu Event command from popup menu.

  * with Graph windows, right clicking on a marker event when tooltip appears; a dialog with all events occurred at the same time will be displayed and user can check events to hide and then click Mask Selected button.




All occurrences of the masked events will be hidden on all windows.

Whole list of masked events is displayed in Masked Events page of the related group in the events setup dialog. Masked events can be restored removing event from the list.