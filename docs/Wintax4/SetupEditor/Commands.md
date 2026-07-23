## Setup Editor Overview

_Setup Editor_ is an application used, in some licenses, to interface with the context information items of WinTAX4 data. Using this application it is possible to edit all information associated with the data, to add, edit or delete constants and user records, to perform computations using virtual channels and to import the content of files not present in the data archive. Using OLE Automation it is possible to access Setup Editor from external applications. Furthermore, the structure of Setup Editor allows for easy interaction with Excel.

Setup Editor can be accessed in the following ways:

  * Data Browser, via the _Setup Editor_ command in the right mouse button menu, via the _Edit_ _- > Setup Editor_ menu or via the shortcut CTRL+F10.

  * WinTAX4, via the _Data_ _- > Setup Editor_ menu or via the shortcut CTRL+F10.

  * In addition, the configuration of constants, user records and offset gains on Acquisition Manager occurs in a structured environment with Setup Editor.




The context information items shown in the grid correspond to the first run selected and any comparison runs associated to comparison laps if present. If opening from Data Browser, this means the first run corresponding to the first lap loaded in [Lap Info Bar](../DataBrw/Header_Lap.md) and any subsequent Comparison Run corresponding to the other laps loaded; if opening from WinTAX4, this means the run corresponding to the first lap loaded, or more generally, the first run loaded in the Working Dataset and any subsequent Comparison Run corresponding to the subsequent run in Working Dataset.

The interface shows the information in a table format that enables all of the information to be viewed on the same page, separated into groups and working copies.

Setup Editor in its standard layout consists of the main menu, a toolbar, the Groups Browser and the data grid.

Groups Browser is simply the following bar, which can be hidden from the user,

The bar indicates all of the group names, in the same appearance order of the grid. Clicking once on the name sends the cursor to the start of the indicated group. The "Groups Browser" bar contains two buttons which have different functions. The first button displays the groups in an extended drop-down list, since it is difficult to show these groups in the horizontal bar, and is therefore useful for selecting any group. The second button is used to hide the Groups Browser bar.

The grid page displays the various working copies aligned vertically in columns. The header of each working copy, which is colored in grey, shows the serial number entered at the time it was created, as well as the word "Master" in the case of the main working copy. This latter header also indicates the run number being edited. If only the Master working copy exists, no header is displayed. The grey columns are the comparison ones; the header shows the color associated to dataset, the session name from which the run is loaded and the type of comparison, delta or value. 

The commands Cancel and Apply, corresponding to the icons displayed under each working copy, enable the working copy to be deleted or saved as the Master. If it is saved as the Master, the working copy is automatically deleted. Each working copy consists of groups with a yellow title header, except for the headers of special groups which are orange; in these latter groups, any links to files which no longer exist are displayed in strikethrough text.

At the right-hand side of each group header there is an icon consisting of the symbol + or the symbol -. Clicking on the icon when the symbol is + will expand the group and all the elements will be displayed. If the symbol is -, the group will be collapsed.

The information within the various groups is displayed one item per row. The Master working copy shows both the name and value of each context information item; the other working copies show only values, in order to maintain alignment between the various working copies. The VCHs are prefixed with ° and are in italic font. User-modified values are blue, whereas red values indicate differences computed by the system (differences in the Compare, differences in the computation of VCHs).

To edit a value, simply double-click on the desired cell or press the space bar to enter the cell value editing mode. However in the case of "Element Files" groups and "VCH Libraries" groups, to edit the value of an element it is necessary to click the browse icon, which opens the file selection window.

##### Related Issues

  * [Elements of the window](glossary.md)

  * [Commands](Setup_Editor.md)

  * [Setup Device](Setupcar.md)

  * [Ole Automation](Oleautomation.md)


