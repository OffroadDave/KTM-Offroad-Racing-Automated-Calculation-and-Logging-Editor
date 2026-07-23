### Condition Overview

Conditions are combinations of Boolean logic expressions (based on VCH functions) which return a value of 0 or 1. They can be used as a basis for generating events and also for filtering the data displayed in analysis windows.

To setup conditions, select Tools/Conditions.

Each Condition is defined by:

  * Name: Name of condition

  * Show: Enables/disables the visualization in the Channel Browser .

  * Expression : Boolean expression to calculate the condition; it is formed by one or more relations connected among them by a logic operator (And, Or, Xor and STOP, Condition column); each relation is composed of two parameters (usually a channel and a threshold value) connected by a Boolean operator. In the filed Expression the conditional formula is shown as a Virtual Channel and can be manually modified. Use the Reload Condition button after editing the filed expression to refresh the conditions list. If the Expression field is wrong, an error message appears instead of the condition list.




The Operators are:

  * = equal operator, check whether the actual value of the channel is equal to the configured value.

  * <> different operator, check whether the actual value of the channel is different to the configured value.

  * > greater than operator, check whether the actual value of the channel is greater than the configured value.

  * < less than operator, check whether the actual value of the channel is less than the configured value.

  * >= greater than or equal operator, check whether the actual value of the channel is greater than or equal to the configured value.

  * <= less than or equal operator, check whether the actual value of the channel is less than or equal to the configured value.




Conditions can be added to graphic windows like VCH channels

##### 

#####  Global/Local Libraries

  1. Select the library

  2. Right click and select Switch to Global (Switch to Local) or select Options/Switch to Global/Local

  3. Click OK to confirm at the prompt

  4. The selected library will now be visible within all user environments if selected as Global or will now be visible only at current user if selected as Local.




##### 

#####  Load library for all users

  1. Select Options/Load library for all users to load one or more libraries.

  2. The selected libraries will now be copied within all user 




#####  Removing Condition libraries

  1. To remove a library from the list simply right click on it and select Cut ( ctrl+X ) or type delete




Note: removing a event library from the list will not delete it from the disk

## 

### Commands of the window

#### Menu

The menu of the Condition Setup window gives access to the following commands, which are divided into sub-menus:

##### File Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Apply | Applies the current settings of the window. |
| Cancel | Closes the window without applying the current settings. |
| Load | Opens a dialog window to select a Condition library (.cnd) to be loaded. |
| Save As | Opens a dialog window to select a Condition library (.cnd) to save the current settings. |
| Import | Not used |
| Export to a new library | Exports the selected Conditions to a new library. |
  
##### Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies the configurations of the selected channels into the clipboard and removes them from the list. |
| Copy | Ctrl + C | Copies the configurations of the selected channels in the list into the clipboard . |
| Paste | Ctrl + V | Pastes the configurations of the channels available in the clipboard, thus adding them to the list. |
| Add Item |  | Adds a new item to the selected library. |
| Remove Item |  | Removes the selected items. |
| Move Up |  | Moves up by one position the selected items in the list. |
| Move Down |  | Moves down by one position the selected items in the list. |
| Find... | Ctrl \+ F | Not used. |
| Find Next | F3 | Not used. |
| Replace... | Ctrl \+ R | Not used. |
| Comment selected lines | Ctrl \+ K | Not used. |
| Uncomment selected lines | Ctrl \+ U | Not used. |
  
##### View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl \+ Shift + Tab | Enables the page of the window next to the one currently in use. |
  
##### Tools Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Sort | Sorts the libraries by name, if a library or the conditions of the library to which the selected condition belongs are selected. Libraries and conditions can be sorted alphabetically or be visualized in the order they have been added. Both alphabetical sorting orders are only executed at runtime and not saved. |
  
##### Options Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Row | Adds a line to the list of the conditional formulas. |
| Remove Row | Removes the selected line from the list of conditional formulas. |
| Reload Condition | Refreshes the condition list after the manual editing of the conditions. |
  
##### Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
#### 

#### Toolbar

The toolbar allows access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Not enabled in this window. |
| Cancel | Similar to the Cancel command of the File menu |
| Apply | Similar to the Apply command of the File menu |
| Load | Similar to the Load command of the File menu |
| Save As | Similar to the Save As command of the File menu |
| Previous Page | Similar to the Previous page command of the View menu |
| Next Page | Similar to the Next page command of the View menu |
| Cut | Similar to the Cut command of the Edit menu |
| Copy | Similar to the Copy command of the Edit menu |
| Paste | Similar to the Paste command of the Edit menu |
| Add Item | Similar to the Add Item command of the Edit menu |
| Remove Item | Similar to the Remove Item command of the Edit menu |
| Move Up | Similar to the Move Up command of the Edit menu |
| Move Down | Similar to the Move Down command of the Edit menu |
| Sort | Similar to the Sort command of the Tools menu |
| Channel Browser | Shows the pop-up menu for selecting a page from the Channel Browser window. |
  
#### 

#### Pop-up Menu

To display the pop-up menu of the Condition Setup window, click on the right hand mouse button. 

The pop-up menu of the Condition Setup window allows access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Cut | Similar to the Cut command of the Edit menu |
| Copy | Similar to the Copy command of the Edit menu |
| Paste | Similar to the Paste command of the Edit menu |
| Add Item | Similar to the Add Item command of the Edit menu |
| Remove Item | Similar to the Remove Item command of the Edit menu |
| Move Up | Similar to the Move Up command of the Edit menu |
| Move Down | Similar to the Move Down command of the Edit menu |
| Switch to Global/Local | Switches a library from Global to Local or vice versa. |
| Sort | Similar to the Sort command of the Tools menu |
| Select library for all users | Selected library will be visible to all users. More information here |