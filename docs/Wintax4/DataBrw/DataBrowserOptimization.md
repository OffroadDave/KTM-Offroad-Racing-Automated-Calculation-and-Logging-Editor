## Directory Tree

The Directory Tree is a new browsing strategy for WinTAX4 to let the user decide at which level of the archive should point the browser; this will help to speed up data browsing in case of massive archives linked or poor network connections.

The command View->Directory Tree allows showing / hiding a tree window pane on the left hand side. This tree window allows to show the archive structure and the content run based without the need to be linked at the track level.

It should be stressed the fact that to better understand the subject, the drop down list [Select Archives](Menu.md#SelectArchive) and the new function "Directory Tree" represent two different ways to select data and they are not linked the one to the other. The new and the old functions work in the same way on the list of sessions and on the list of laps, they just differ in how they have access to the lists. Both functions have been expanded so that they enable the selection without being constrained at track level, but enabling the access also to lower levels, even to the single lap. On the basis of the selection chosen, the Session and Lap lists will be up-dated accordingly.

The dropdown list is not up-dated following to a selection on the tree.

So in case of linked folders containing data from a single session the left hand session selection field will show all the information about this session independently to the archive level linked.

The selection can be made by simply clicking with the mouse on the tree or scrolling it with the arrow keys; the list will be automatically up-dated without requiring a confirmation.

By clicking with the right button on a folder it will be possible to add the selected path to favorites available to the left top drop down list menu.

Above the tree panels, a small header contains:

  * One text input field where it is possible to manually enter a root path which saves the history of browsed paths (up to twenty entries).

  * A browse directory button that opens the current directory and the fills the previous text box enabling to select directory not manually edited.



  * An OK button confirms the root path and then up-dates the tree content. Pressing the Enter key in the text input field performs the same action.

  * A clear buttons that erases the combo history.



  * A close button that hides the tree pane (see command View->Directory Tree).




Any selection change in the tree pane does not change the content of the [Select Archives](Menu.md#SelectArchive) drop down list and vice versa.

Session fields on the left panel will be shown in the following conditions of selection in the directory tree:

  * An archive folder is selected, then the Session List contains sessions found in the Track, Session, Device levels.

  * A Track, Session or Device folder is selected, then the Session List contains sessions found in the following folders.



  * Track folder is selected, then the Session List contains one (or more) session found in the following folders: one Session List for each Vehicle. The Lap List contains all the Laps for each Run.

  * Session folder is selected, then the Session List contains one (or more) session found in the following folders: one Session List for each Vehicle. The Lap List contains all the Laps for each Run.

  * Device folder is selected, then the Session List contains only one Session. The Lap List contains all the Laps for each Run.

  * A Run folder is selected and the Session List contains a single Session. The Lap List is filtered to show only the laps in the selected Run. In this case the Run selection will be filtered also in analysis windows (Previous Lap, Next Lap, Lap Reports, etc).

  * A Lap folder is selected and the Session List contains a single Session. The Lap List is filtered to show only the lap selected. In this case the Lap selection will be filtered also in analysis windows. Lap Reports will work based only on one lap. Previous Lap, Next Lap are disabled.




The Session List does not contain sessions when the selection on the Directory Tree is on a folder higher to an archive level.

###  Stop Browsing Option

it is possible to stop the data browsing in case of problems opening an archive; to this aim a button on the toolbar will be added and also a shortcut (Esc). In menu File this command is called "Stop Scanning". The stop operation will require to be confirmed by the user.