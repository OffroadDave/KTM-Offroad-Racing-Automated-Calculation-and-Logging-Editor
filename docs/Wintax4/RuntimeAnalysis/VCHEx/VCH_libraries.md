### Setup of Virtual Channels (VCH)

Selecting Tools/Virtual channels from Main menu, the Math Libraries Setup and editor will be open.

#### File Menu 

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | Enter | Applies the changes and close the editor |
| Cancel | Esc | Closes the editor |
| Load |  | Opens a file dialog to choose the libraries to load. See Loading VCH Libraries |
| Save As... |  | Saves the current VCH library on the disk in the Libraries directory with a new name |
| Save as Encrypted |  | Allows the library to be exported in an encrypted format. |
| Export in a new library... |  | it is similar at Save As... but loads the new library in the tree. See the description of the command. |
| Import |  | Not used |
  
#### Edit Menu 

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Copies to clipboard the configurations of the channels selected in the list of the Graphs section and it removes them from the list of the Graphs section. |
| Copy | Ctrl + C | Copies to clipboard the configurations of the channels selected in the list of the Graphs section |
| Paste | Ctrl + V | Pastes the configurations of the channels available in clipboard adding them to the list of the Graphs section |
| Add Item |  | Adds a new VCH to the selected library; see here for more details |
| Remove Item |  | Removes the selected channels; see here for more details |
| Move Up |  | Moves up by one position the selected elements |
| Move Down |  | Moves down by one position the selected elements |
| Find... | Ctrl + F | Finds a target string; see Search, Find and Replace for details |
| Find Next | F3 | Finds next occurrence; see Search, Find and Replace for details |
| Replace... | Ctrl + R | Replaces a target string with a new string; see Search, Find and Replace for details |
| Comment selected lines | Ctrl + K | Applies comment tags in the selected lines of the editor |
| Uncomment selected lines | Ctrl + U | Cancels comment tags in the selected lines of the editor |
  
#### 

#### View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use |
  
##### 

#### Tools Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Sort | Sorts libraries or virtual channels according to the selected node. |
  
##### 

#### Options Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Insert | Insert functions in the expression. See VCH Functions |
| Export CSV | Exports virtual channels in CSV format; see Import & Export in CSV format files |
| Import CSV | Imports virtual channels in CSV format; see Import & Export in CSV format files |
| Switch to Global/Local | Switches a library from Global to Local or vice versa. |
| Select library for all user | Copies the selected libraries in all users; see here for details. |
| Load library for all user | Loads and copies the selected libraries in all users; see here for details. |
  
#### 

#### Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
##### 

#####  Loading VCH libraries

  1. Select Tools/Virtual channels to open the editor

  2. Select File/Load from the editor menu and load or create one or more VCH files. The libraries which are currently loaded are listed on the left of the editor window. The path for the highlighted library is shown on the right

By default, VCH libraries are local to the User

  3. Use the read only flag to prevent entries in the library from being modified. Libraries which are not saved in the local Libraries directory are always read only. They are generally shared between users.




##### 

##### Save as encrypted

Allows the creation of encrypted virtual channels (VCH), where formulas are hidden and protected. The channel name, frequency, decimals, and comment remain visible, but the formula is replaced by ENCRYPTED (read-only). Recipients can load the encrypted files via Channel Browser but cannot view or modify the internal math.

##### Creating Encrypted Files

Use the Save As Encrypted command, available via:

Right-click menu on individual VCH libraries

The encrypted file will have a .VCR extension

An encrypted VCR file will have priority over an unencrypted Virtual channel library.

When creating an encrypted library, it is possible to include in the encrypted library a "related" lookup table.

##### 

| Command | Description |
| --- | --- |
| Browse | Allows the selection of the destination for the encrypted file. |
| Add | Adds a selected Lookup table from a list of available lookup tables. |
| Add All | Adds all lookup tables configured in WinTAX. |
| Remove | Removed the selected lookup table. |
| Remove all | Removes all (previously) added lookup tables. |
| Encrypt to license | On selected licenses it is possible to select a destination license which uses the same encryption method. |
  
##### Loading Behavior

Encrypted files are read-only. Modifications must be made to the original file and re-encrypted.

Formulas used in the encrypted library are hidden wherever shown and will appear as "Encrypted". (e.g., Channel Browser tooltip).

A dedicated icon is visualized when an encrypted library is loaded.

##### Important notes:

  * Encrypted libraries cannot be edited. It's also not possible to view or edit the expressions of the channels in an encrypted library.

  * Encrypted files are not auto-loaded into Wintax (to avoid duplicates in the Channel Browser).




Linked Lookup channels are included automatically 

#####  Export in New Library

This option allows to create a new VCH library consisting of a subset of channels from an existing library.

  1. Select Tools/Virtual channels or select the Virtual channels tab from the Channel Browser

  2. Select a subset of channels from one of the existing libraries

  3. Select File/Export in a new library

  4. Define the name for the new library. It will be created in WinTAX4\Libraries




##### Search, Find and Replace

Search, Find & Replace commands are available by clicking with mouse on respective tool bar buttons , , , and via menu Edit.

Following windows will appear, in order to set searching filter conditions:

  * Find what: insert here the target string to search for.

  * Match whole word only: enable this check in order to restrict search to strings that match exactly target string.

  * Match case: enable this check, in order to find strings that match case with target string.

  * Replace with: insert here the string that replace target string.

  * Find Next: click to find next occurrence.

  * Replace: click to replace current string found.

  * Replace All: click to replace all occurrences of target string.




Find & Replace functionality works on:

\- All libraries currently loaded, both “Global” and “Local”.

\- VCH Description and Expression.

##### 

##### Import & Export in CSV format files

VCH libraries can be imported from and exported to .CSV format files

###### Import

  1. Select Tools/Virtual channels or select the Virtual channels tab from the Channel Browser

  2. Load and select or create the destination library where the math channels must be imported to.

  3. Select Options/Import CSV

  4. Browse and select the CSV file from which to import




###### Export

  1. Select Tools/Virtual channels or select the Virtual channels tab from the Channel Browser

  2. Load and select the source library from where the math channels must be exported.

  3. Select Options/Export CSV

  4. Browse to the destination path and define the CSV output file name




##### 

#####  Global/Local Libraries

  1. Select the library

  2. Right click and select Switch to Global (Switch to Local) or select Options/Switch to Global/Local

  3. Click OK to confirm at the prompt

  4. The selected library will now be visible within all user environments if selected as Global or will now be visible only at current user if selected as Local.




##### 

#####  Select library for all users

  1. Select File/Load to load the library, by default it is loaded as Local

  2. Click on the library then right click and choose Select library for all users or select Options/Select library for all users

  3. The selected library will now be copied within all user 




#####  Load library for all users

  1. Select Options/Load library for all users to load one or more libraries.

  2. The selected libraries will now be copied within all user 




#####  Restore user default VCH libraries

  1. From the main WinTAX4 menu bar select Setup/Users/Configure user

  2. Choose the user to be restored

  3. Select Edit/Restore default user libraries




#####  Removing VCH libraries

  1. To remove a library from the list simply right click on it and select Cut ( ctrl+X ) or type delete




Note: removing a VCH library from the list will not delete it from the disk

## 

#####  Create a mathematical channel

  1. To create virtual channels select Edit/Add item

  2. A new entry with a default name will be created and the editor will show the fields needed to define the expression. Edit the default name, the name can start with a alphabetic char or with a numeric char (0-9).

  3. Click inside the Expression pane and begin typing

Note: no equals sign needs to be typed at the beginning of the expression.

Note: The expression field can contain a name of a element that starts with a decimal char..

  4. While typing an expression the editor will offer auto-complete suggestions for function and channel names.




Function arguments are shown in a tooltip when the open bracket after a function name is typed.

  6. To see a list of available functions and operators click on the functions type buttons on bottom of the window. A list of functions is shown within an help description




  6. Use the Show flag to define whether an expression is to be displayed in the [channel browser](../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md) (you may want to hide intermediate expressions).

  7. Set the frequency to Auto to generate the channel at the highest frequency of the constituent channels.




#####  Writing an expression

In addition to standard mathematical rules, a mathematical expression could be written using the [VCH Programming Language](Functions/vch_proglanguage.md). 

The expression can be written on multiple lines and each line must be terminated with a semicolon.

it is possible to use variable for saving partial results and reusing them in the same expression or in an another one.

Comment line start with "//" tag.

#### Examples

[User Record In VCH](../../Sample/UserRecordInVCH.md)