## Setup Data and DataSet Headers 

The Data Header window is divided into two sections: the section on the left shows the list of the configurable Headers (Data Headers and Dataset Headers), while the section on the right allows configuring the elements selected in the Headers list.

Depending on the licence, the setup of the Dataset Header might be missing.

The window has also an integrated menu and a toolbar that ease the access to the configuration and management commands of the window itself.

### Setup Data Header

This section configures the header of the data of WinTAX4

In the list it is possible to select which information must be displayed on the Data Header bar and in which order they must be presented. The Options buttons permits to modify the order of the list, to select all information, to select none of them and to select the default configuration.

Through the Add Item command new items in addition to the ones already available can be added. All elements that have been added will be marked with the prefix "User Record". After the Key/Expression field is inserted, the Data Header will show a Key/Expression corresponding value. The "user records" can also be selected and moved or deleted from the list with the Remove Item command.

If a numeric information is selected, then it is possible to set the numeric format and the number of decimals through the Output Format combo and the Decimals combo

  * The Show info name check adds the name of the information before the value corresponding to the Data Header

  * The Compact view check enables the mode that allows the Data Header to occupy the space of one single lap when there is a comparison. In this situation the arrows will appear to permit switching among the laps compared.

  * The Show interactive data header check, adds to the Data Header the bar that shows the lap or the append used for that Data Header and that allows to enable / disable the DataSet and also to select one of the laps of the append.

    * The Inline data header check places the info and the interactive data header on a single line but only if there is enough space to do it, otherwise they will appear on two lines.

    * The Show data buttons check adds to the Data Header bar the “+” and “-“ buttons; these commands allow to load next or previous lap.

    * The Display lap time in the laps bar check adds to the lap bar the value of lap time as shown in the picture below.




###  DataSet Header

In this section the header of the DataSet Panel is configured

In the list it is possible to select which information must be displayed on the Data Header bar and in which order they must be presented. The Options buttons allow to change the order of the list, to select all information, to deselect them and to select the default configuration. The User Record cannot be added and it is not possible to configure its format or the aspect of the header.

###  Menu

The menu of the Data Header window allows the access to the following commands, divided into sub menus:

#### File

| COMMAND | DESCRIPTION |
| --- | --- |
| Apply | Applies the current settings of the window. |
| Cancel | Closes the window without applying the current settings. |
  
#### Edit

| COMMAND | DESCRIPTION |
| --- | --- |
| Add User Record | Adds a new user record to the list. Used only for the Data Header. |
| Remove User Record | Removes the selected user record. Used only for the Data Header. |
  
#### Help

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
###  Toolbar

The toolbar of the Data Header window enables the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Cancel | Closes the configuration window without applying the settings. |
| Apply | Applies the current settings to the configuration and closes the window. |
| Add Item | Adds a new item to the list. Used only for the Data Header. |
| Remove Item | Removes the selected items. Used only for the Data Header. |