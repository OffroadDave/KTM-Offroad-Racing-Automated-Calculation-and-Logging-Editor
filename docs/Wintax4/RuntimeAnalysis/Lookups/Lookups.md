### Editor

The lookups are [1-dimensional](../VCH/Functions/VCH_functions.md#vch_Lookup1) or [2-dimensional](../VCH/Functions/VCH_functions.md#vch_Lookup2) tables that can be used in the VCH expressions to associate some determined values to pre-determined values of one or more channels.

A lookup is identified by a file named as the lookup followed by the .dat extension. These files must be added in the WinTAX4\Libraries directory or in any subfolder of it.

Through WinTAX4 already existing lookups can be modified or new ones can be created, both 1-dimensional and 2-dimensional. To launch the lookup editor, use the Tools/Lookups command or the corresponding button on the toolbars.

The editor in the figure shows a 2-dimensional lookup .

The editor is formed by a list on the left to select the lookup to be displayed or modified, and by the editor page where the changes can be carried out.

The editor area is formed by two text boxes where the name of the lookup and an optional comment can be written, by a table and by buttons that performs operations on the table. The table is formed by rows and columns where the first row and the first column are highlighted as they contain the reference values of the channels and have a grey background. (if there is a 1-dimensional lookup with one column, no reference values are displayed in the first row), all others cells contain the output values. The cell with 0,0 index simply contains the reference to the index of the row and column selected.

To create a new lookup, use the Add Item command. The command creates in the memory a new lookup structure formed by a row and a column that can be named and a comment can be added to included. The file is saved only when the Apply command is used or when another lookup of the list is selected or when a new lookup is created. The saving always requires a confirmation.

When a lookup is edited and the rows and columns are added, the addition is carried out as follows: the row is always added after the currently selected row and the same applies to the column. The values of the first column and of the first row are automatically added using the following rule: if the row or the column selected are the last ones, then in the new row or column a value corresponding to the previous one increased by one is added, if the row or column are in any other position, the value added corresponds to the average value obtained between the previous and the next ones.

All values, included those of the reference row and column, can be anyhow edited and modified. The reference row and column must however respect the rule according to which each added value must be higher than the previous one and lower than the next one, otherwise the addition is not allowed. In all other rows there are no limits to the values that can be added.

The distinction between 1-dimensional and 2-dimensional is done on the basis of the number of columns available. When only one column is available together with the reference one, the lookup is 1-dimensional.

The editor can recognize also manually created lookups if the comment and sorting rules for rows and columns are respected.

Examples: [User Record In Lookups](../../Sample/UserRecordInLookups.md)

###  Commands of the window

#### Menu

The windows menu allows access to the following commands, divided into sub menus

##### File Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Apply | Applies the current settings to the window. |
| Cancel | Closes the window without applying the current settings. |
  
There are also grey commands, not used in lookup editor.

##### Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Copy | CTRL + C | Copy in clipboard the selected cells |
| Paste | CTRL + V | Remove all the previous cells and paste the cells saved in the selected lookup |
| Add Item |  | Adds a new lookup; the .dat file is created after the confirmation. |
| Remove Item |  | Removes the selected lookup; it does not handle multiple cancellation. The file is removed also from the disk and moved in the bin. |
  
There are also grey commands, not used in lookup editor.

#####  View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | CTRL \+ SHIFT + TAB | Enables the page of the window previous to the one currently in use. |
| Next page | CTRL \+ TAB | Enables the page of the window next to the one currently in use. |
  
##### Options Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Row | Adds a row to the lookup displayed in the position indicated by the selection. |
| Remove Row | Removes the selected row. |
| Add Column | Adds a column to the lookup displayed in the position indicated by the selection. |
| Remove Column | Removes the selected column. |
| Reload | Cancels the modifications reloading the lookup displayed by the file. |
| Copy Data to Clipboard | Copies to clipboard the rows and columns selected. |
| Paste Data to Clipboard | Copies the contents of clipboard into table. |
| Open in Excel | Opens a file in Excel with the rows and columns selected. |
  
#### Toolbar

The Channels Parameters Setup window allows access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Not enabled in this window. |
| Cancel | Similar to the Cancel command of the File menu |
| Apply | Similar to the Apply command of the File menu |
| Previous Page | Similar to the Previous page command of the View menu |
| Next Page | Similar to the Next page command of the View menu |
| Copy | Similar to Copy Data to Clipboard of the Options menu |
| Paste | Similar to Paste Data to Clipboard of the Options menu |
| Add Item | Similar to the Add Item command of the Edit menu |
| Remove Item | Similar to the Remove Item command of the Edit menu |
  
The grayed commands on toolbar are not enabled for Lookup editor.