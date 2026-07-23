## Commands

The menus contain all of the commands which can be executed in Setup Editor. There is also a non-configurable toolbar which displays the main commands. Finally, commands can also be executed by clicking the right-hand mouse button to open a popup menu, which shows all the possible commands for the selected element. The following section describes the commands with reference to the main menu.

### **Menu** **File**

****

| COMMAND | DESCRIPTION |
| --- | --- |
| Apply | Use this command to exit the Setup Editor environment, saving any modifications made. Modifications to context information items are saved to XML files; WinTAX4 automatically reloads the modifications on exiting Setup Editor. |
| Cancel | Use this command to exit the Setup Editor environment, cancelling any modifications made. The WinTAX4 status remains the same as before Setup Editor was opened. |
| **_Add Element File Link_** | Adds a link to an external text file. |
| **_Add VCH Library Link_** | Adds a link to a virtual channel library. |
| **_Load Master Copy_** | Use this command to load an XML file with the extension _.spec_ created using the command Save Master Copy. |
| **_Save Master Copy_** | To save all or part of the Master Copy in an XML file with the extension _.spec._ |
| Update lock | Manual Refresh of the lock state in case the editor opened in read-only mode due to simultaneous editing of the same information. When simultaneous editing is no longer available, the window is removed from the read-only mode. |
  
##### Add Element File Link

To add a link to an external text file which must be located in the WinTAX4 Libraries directory; links to text files are the elements which constitute the Element Files group. The command opens the following input window.

Library Name refers to the file which will be displayed in Setup Editor, whereas File Name is the complete path of the file which can only be entered via the Browse button. Press OK to load the link.

##### Add VCH Library Link

Use this command to add a link to a virtual channel library which must be located in the WinTAX4 Libraries directory; links to virtual channel libraries automatically become elements of the VCH Libraries group.

File Name is the complete path of the file which can only be entered via the Browse button. Press OK to load the link.

##### Load Master Copy

Use this command to load an XML file with the extension _.spec_ created using the command Save Master Copy. However, in WinTAX4 only custom groups are loaded, whereas others are ignored. In other words, only constants and user records are loaded using this command. The load is performed not by overwriting the Master loaded from file with the existing Master, but by merging the two files, adding the new values and replacing the values which are identical.

One use for this command is to save a Master copy from the Acquisition Manager environment and then load it in WinTAX4, so as to obtain the same constants or user records that you would have during acquisition.

##### Save Master Copy

To save all or part of the Master Copy in an XML file with the extension _.spec_ , which can then be loaded using the command Load Master Copy. The Save Master Copy command opens the following window:

If no item is selected from the list or if all items are selected (using the standard Windows command Ctrl+Shift) all groups with all values are saved in the file indicated in the "FileName" box. Otherwise only the selected groups are saved. The file can be saved in any directory; by default the WinTAX4 Export directory is given.

### **Menu Edit**

****

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| **_Create New Working copy_** |  | This command adds a working copy by copying it from the Master. |
| **_Sort Group A - Z_** |  | This command sorts all of the groups into ascending alphabetical order, e.g. from A to Z. |
| **_Move up_** |  | This command, which applies both to groups and group elements, can be used to move the selected element up one position. Multiple elements can be selected within a group, allowing more than one element to be moved upward. |
| **_Move down_** |  | This command, which applies both to groups and to group elements, can be used to move the selected element down one position. Multiple elements can be selected within a group, allowing more than one element to be moved downward. |
| **_Add New Group_** | Ctrl + G | This command adds a group to the working copies. |
| **_Rename Group_** |  | This command changes the name of the selected group. |
| **_Add VCH_** | Ctrl + H | This command opens the selection window, which contains all of the virtual channels present in the libraries which are linked in the VCH Libraries special group. |
| **_Add Fields_** | Ctrl + D | This command opens the Add New Field window, in which a new element can be entered. |
| **_Delete_** | Delete | This command deletes a group from a working copy or an element from a group, depending on the object selected at the time. The command does not require confirmation. A group is deleted even if it contains elements. Regarding groups, the command is only active for custom ones. Meanwhile in the case of individual elements, the command is active for all constants, user records, all VCHs and for elements contained in special groups. In default groups it is only possible to delete VCHs added to the Header group. |
  
##### Create New Working copy

This command adds a working copy by copying it from the Master. The new working copy is positioned to the right of the Master or in any case to the right of the last copy created. Group names and channel names are not indicated, since changes made to the structure of a working copy are also made to all other working copies in exactly the same way. Each new working copy is an image of the Master captured at that moment. During creation of the first working copy which is not the Master, the column header is also created.

These new working copies can also be accessed via Automation, in order to create an Excel chart showing trends in the various context information items when one of the variables is changed.

##### Add New Group

This command adds a group to the working copies. Since working copies are always aligned with each other, the group is added to all of the working copies present. When this command is selected the following window appears, in which a group name can be entered.

The created group is obviously a custom group. It is not possible to insert a group with a name which is already in use: in this case, the entry is blocked. The newly created group is obviously blank. The command can be invoked using the keyboard shortcut Ctrl+G.

##### Rename Group

This command changes the name of the selected group. The command opens the following window. Note that it is not possible to change a group name into an existing name.

##### Add VCH

This command opens the following selection window, which contains all of the virtual channels present in the libraries which are linked in the VCH Libraries special group. The VCHs can be added to all custom groups and to the header special group. A VCH is computed in the working copy to which it belongs. If the formula used to construct it contains constants which have different values in the various working copies, the computed output result will show a different value for each working copy.

##### Add Field

This command opens the following window, in which a new element can be entered.

These elements can be added to all custom groups. Both strings and numerical values can be added in the value field, without distinction.

### **Menu Comparison**

****

| COMMAND | DESCRIPTION |
| --- | --- |
| Select Data | This command opens the Databrowser for changing run comparison selections. Select new comparisons and confirm with Enter shortcut or Open command from menu or double click in the same way you open a comparison in WinTAX4 data loading Remarks: During the execution of this command, the master working copy remains the same even if you try to open a new lap from Data Browser by using the Enter shortcut, Open command from menu or double click. The only way to change it, during the comparison operations, is to execute the Clear Comparison command on Data Browser. Only in this way, at the subsequent load, you will have a new working copy. |
| **_Mode_** | Mode brings up a submenu which allows the user to select between two different comparison modes, Delta and Value. |
  
### **Options Menu**

****

| COMMAND | DESCRIPTION |
| --- | --- |
| **_Open in Excel_** | This command opens an Excel workbook in which the entire content of Setup Editor is displayed. The values are loaded into Excel with a similar layout to the original one. Each column header is also saved and, in contrast to Setup Editor, all of the names of each group and each element are repeated, not only for the Master but also for each working copy. Furthermore, any groups which may have been collapsed and hidden for space-saving purposes, are expanded and displayed on the Excel workbook. |
| **_Excel Preview_** | This command opens the standard selection window, which shows the directory WinTAX4\Utilities\ and all of the Excel workbooks contained in it. For this command to function correctly, the Excel workbooks that you wish to preview must have already been appropriately configured according to a well-defined, fixed structure. You can also open Excel files located in directories other than WinTAX4\Utilities\, even though this is the default directory. When the general Excel workbook is selected, it is opened and any matches between groups and elements present at that time in Setup Editor are checked against a fixed search key; finally, the associated fields in the Excel workbook are updated. |
| View entries values | If checked, show the groups loaded from **Element Files. The header color of these groups is cyan.** |
| **_Export in Excel_** | This command opens the standard Windows "Save As" window and shows the directory WinTAX4\Export\, although it is possible to save the file in any other directory. An Excel workbook is created with the same characteristics as the file obtained using the Open command in Excel, but with a simplified layout. |
| **_Export Group in Excel_** | This command opens the Export Group selection window. |
| **_Import Group From File_** | This command imports into Setup Editor text files formatted according to the Element File rules. |
  
##### Export Group in Excel

This command opens the group selection window.

In this window you can select one or more groups. Selecting OK opens an Excel workbook formatted according to the Excel Preview rules. Each group will be identified by a header **[GROUPNAME]** with all the group elements listed underneath in the style **[GROUPNAME].ElementName** and with their corresponding values in the next column.

If the option Only Master Working Copy is selected, then only the values for the elements of selected groups belonging to the Master are exported to the Excel file. If, on the other hand, the option All Working Copies is selected, then the selected groups of all working copies are exported.

##### Import Group from File

This command imports text files into the Setup Editor, formatted according to the Element File rules. During the import operation, new elements are added and existing elements are updated.

If the loaded file contains a group which is not present in the Master working copy, a new group is created to which all of the elements present in the file are added.

If however the group exists, new elements are added while existing values are only updated.

In short, elements belonging to identical groups are combined, while identical elements are discarded. This applies to all groups, not just custom groups.

If the Master copy already contains a constant with the same name as a new constant or a user record with the same name as a new user record, and provided they are located in different groups, the insertion of this new value is permitted. However, it is not permitted to save two constants or two user records with the same name, therefore during the save operation only the first one is kept and all others with the same name are deleted.

### **Menu View**

****

| COMMAND | DESCRIPTION |
| --- | --- |
| **_Groups Browser_** | This command displays or hides the Groups Browser bar. |
| **_Hide Group_** | Hides the selected group. The group is understood to be selected even when the selection is active on an element in the group. Multiple selections are not possible. |
| **_Expand/Collapse Group_** | Expands or collapses the selected group. The group is understood to be selected even when the selection is active on an individual element in the group. Multiple selections are not possible. The status of the individual groups is maintained whenever "Setup Editor" is reopened. |
| **_Show/Hide Group_** | This command opens the list all of the groups present in "Setup Editor". |
  
##### Show/Hide Group

This command opens the following popup menu, which lists all of the groups present in Setup Editor. Each group can be hidden or displayed by clicking the corresponding name. A ticked box indicates a displayed group, while an empty box indicates a hidden group. The status of the individual groups is maintained whenever "Setup Editor" is reopened.

### **Menu Help**

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
##### Related Issues

  * [Setup Editor](Commands.md)

  * [Elements of the window](glossary.md)

  * [Commands](Setup_Editor.md)

  * [Ole Automation](Oleautomation.md)


