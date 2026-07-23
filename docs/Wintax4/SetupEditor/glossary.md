## Setup Editor elements

This section explains the terminology used to describe the various functions and gives an overview of how information are organized in Setup Editor.

### **Context Information**

This term denotes all items of information which are saved along with data during laps acquisition. This information can relate to the individual lap (lap level) or to the entire run (run level). Using Setup Editor it is possible to read, edit and in some cases also add or delete information. Context information items which are added by Setup Editor are also called "fields". Note that the elements syntax must respect certain rules, e.g. they must not contain spaces, they must not contain the characters -/.,+=#@*^\ and furthermore they must not start with numeric values. When constants are used in VCH formulas, they must only be used with their names, without indications regarding the group to which they belong. Such indications must however be given in the case of Element Files. It is therefore not possible to have elements of the same type (e.g. constants) with the same name. Elements of different types (for example a constant element and a user record element) can however share the same name.

### **Virtual Channels (VCH)**

Virtual channels are channels configured in the mathematical libraries of WinTAX4. When a library is loaded into the VCH Libraries special group, all of the VCHs contained in it are available to be loaded into the Setup Editor of the custom groups and of the default Header group. The aim is to show the result of mathematical computations based on the context information numeric values.

VCH computations can also be based on Element File parameters, e.g. values read from text files whose corresponding links are stored in the Element Files group. These computations are performed using the values of information items stored in the memory at that time and a variation of the elements which constitute the VCH causes the VCH to be immediately recalculated. Setup Editor is also able to perform computations of VCHs which have logged channels in the formula.

### **Element Files**

Element files are text files created by external applications which contain suitably encoded information that can be read by Setup Editor. These files must reside in the Libraries directory of WinTAX4. Element files can be used in two ways. The first way involves creating a link to the file and saving the link in the Element Files special group. The second way is by importing the text file. In both cases, Setup Editor is able to access the information contained in the files but with one fundamental difference. In the first case, Setup Editor does not display the loaded groups and values because it refers to them via the links to the file. In the second case however, the imported values reside directly in Setup Editor but the link with the source file is lost completely. The advantage of the first case is that the system is sensitive to changes in the elements within the files, while in the second case, obviously changes to the file are lost unless the file is subsequently reloaded. On the other hand, the disadvantage of the first case is that information loaded from the file is not visible in Setup Editor. The user should select the most convenient method as required. In both cases, when it is necessary to retrieve one of these elements, for example in a VCH computation formula, the syntax to use is **Group.Value**.

The information contained in element files can consist of numeric values or string values. Only the presence of a non-numeric character differentiates the two types of information: numeric or alphanumeric, e.g. constants and user records.

The syntax of the individual elements must respect the syntax rules of VCHs, e.g. they must not contain spaces, they must not contain the characters -/.,+=#@*^\ and furthermore they must not start with numeric values.

The only accepted decimal separator character is the decimal point, irrespective of the regional settings configured on the PC. Vectors are not provided for.

File formatting must however conform to the following fixed rules.

| FILE FORMAT | DESCRIPTION |
| --- | --- |
| [GROUP NAME] | group name, always delimited by the characters [ and ] |
| Element Name = Element Value | group elements, the sign = delimits the name of the element from its value // is the comment sign; the whole line after this symbol is ignored. |
  
For example, the content of an element file may be as follows:

| FILE FORMAT | COMMENT | DESCRIPTION |
| --- | --- | --- |
| [GENERAL] | // group name | Element Value=_VAL_T1.06_ (string) |
| Event=VAL_T4.06 | // Element: _Even_. | Element Value=_VAL_T1.06_ (string) |
| EventNum=Test | // Element: _EventNum_ | Element Value=_Test_ (string) |
| Session=Day1, 30/10/06 | // Element: _Session_ | Element Value=_Day1, 30/10/06_ (string) |
| Track Length=4005 | // Element: Track Length | Element Value=_4005_ (numeric) |
  
| FILE FORMAT | COMMENT | DESCRIPTION |
| --- | --- | --- |
| [ENGINE] | // group name |  |
| Cyl head=1995 | // Element _Cyl head_ | Element Value=_1995_ (numeric) |
| Engine spec=BB4 | // Element: _Engine spec_. | Element Value=_BB4_ (string) |
| Primary=48/88 | // Element _Primary_. | Element Value=_48/88_ (string) |
  
### **Groups**

Context information items and VCHs are sorted into groups. When a lap is opened for the first time, Setup Editor loads all of the context information items and sorts them into groups. After loading it is still possible to add new groups. Groups are identified by a name which must be unique. They can also be unlimited in number, just as there is no limit to the number of context information items and VCHs which may be present within a group. The group name must also respect the syntax rules, e.g. it must not contain spaces, it must not contain the characters -/.,+=#@*^\ and furthermore it must not start with numeric values.

Groups can be divided into three categories: 1) Default Groups 2) Custom Groups 3) Special Groups.

**_Default Groups_**

Below is a summary of the groups that are created at startup of Setup Editor. The following groups cannot be deleted and contains elements which themselves cannot be deleted but only edited.

**Abs , Comment, Lap, Lap Distance, Lap Time, Marker, Meteo Bar, Meteo Hum In, Meteo Hum Out, Meteo Rain, Meteo Temp In, Meteo Temp Out, Meteo Wind Dir, Wind Speed, Run, Stored Track, Track Lap** contain lap-level information; within the group, unique information is displayed for every single lap in the run.

In addition there are four other default groups but with different characteristics.

**Offset Gain** contains the list of run-level parameters which can modify the value of the channel. Lap-level offset gains are simply ignored. It is not possible to add or delete channels because each element of the group is associated with a lap channel. The values can only be edited.

**Header** contains general information about the loaded run. This information can be edited but not deleted. In this group it is possible to add or delete VCH elements.

**Constants** is a group which contains all of the run-level constants, whereas the lap-level constants are completely ignored. Constants are numeric values associated with a name. This group is created the first time that a data archive is opened with Setup Editor in order to somehow group all of the existing constants, although in fact it is a fully custom group. Therefore, when the archive is opened on subsequent occasions, the group may not even be present because it can be deleted or renamed.

**User Record** is a group which contains all of the run-level user record values, whereas lap-level user record values are completely ignored. User records are text values associated with a name. This group is created the first time that a data archive is opened with Setup Editor in order to somehow group all of the existing user records, although in fact it is a fully custom group. Therefore, when the archive is opened on subsequent occasions, the group may not even be present because it can be deleted or renamed.

**_Custom Groups_**

Custom groups are groups which are inserted by the user, they can be unlimited in number and can also contain an unlimited number of context information items or VCHs. In fact, the default groups "Constants" and "User Record" are similar to custom groups, the only difference being that they are created the first time that a lap is opened in Setup Editor, to enable existing constants and user records to be placed. The elements present in the groups must be unique for the given type. In each group there must be only one constant with a particular name; if multiple constants are inserted with the same name, including between different groups, the system does not allow them to be saved and only saves the first constant that is found. However, it is possible for a constant and a user record with the same name to coexist.

**_Special Groups_**

There are two special groups which are called **Element Files** and **VCH Libraries**.

**Element Files** groups contain links to text files present in the library. These files contain further information which are not displayed in Setup Editor but which is however available for VCH computations. To add links to the group, use the command Add Element File Link.

The **VCH Libraries** group instead contains one or more links to the WinTAX4 virtual channel libraries; if the archive is being opened for the first time with Setup Editor, the group is still present but empty. To add links to the group, use the command Add VCH Library Link.

It is also possible to create groups of context information items from suitably formatted external files.

The WinTAX4 channel browser is able to manage groups. The individual group is not active and cannot be dragged and dropped to insert all of its constituent elements in the desired window, whereas drag & drop is permitted for individual elements or for a multiple selection of elements. It is possible to expand or collapse the list of constants by double-clicking on the group name.

### **Working Copy**

The full set of groups is called the working copy. Up to 16 working copies can be displayed simultaneously on the main page. The main working copy, which is the one for which data can be saved in the archive, is called the Master. The other working copies are for tests and modifications; if a working copy must be saved, Setup Editor allows the existing Master to be replaced with the desired working copy. Working copies are always aligned in the sense that any added/deleted elements are also added/deleted on all the working copies present in Setup Editor. Computations executed by the VCHs are performed on the working copy to which it belongs. It is therefore normal for the same VCH to produce different results in the various working copies, since the parameters which constitute the VCH may have been edited. The content of the working copies is also available via OLE/Automation, by means of appropriate addressing.

### **Run Comparison**

Using Setup Editor it is possible to make comparisons between runs. The reference run is always the one loaded in the Master working copy. It is therefore not possible to edit this reference run without exiting Setup Editor and re-entering the program after having loaded the data from another run. Consequently the comparison is made between the run indicated by the current Master working copy and one or more of the other runs. In terms of the display, the appearance is similar to the status of multiple working copies. In other words, a main column which displays the Master run, in addition to one column for each of the comparison runs. Each column is divided into groups of elements. The Master run determines which elements are displayed, in the sense that any elements present in at least one of the comparison runs but not in the Master run are not shown. Similarly, elements which are present in the Master run but not present in any of the comparison runs are still displayed and indicated with a dash symbol (-). Elements that are present in the comparison runs cannot be edited in any way. To highlight this fact, the cells of the values are shaded grey.

The comparison can be performed in two modes, Delta and Values.

In Delta mode, differences with the reference run are displayed; fields which remain unchanged are shown blank. In string fields, the message NoRx is displayed if the values are different. If the reference run contains an element which is not present in the comparison runs, a dash symbol is displayed in the comparison runs. If on the other hand the element is present in the comparison runs but not in the Master run, then the element is not shown. Changing a value of a field on the Master working copy, the difference is automatically recalculated in the comparison runs.

In Values mode, the values of the reference run are displayed; these values are displayed in red when different from the values of the same elements in the comparison run, otherwise they are shown in black as normal.

### **Excel Preview**

Excel Preview mode used to display only the requested data inside an Excel spreadsheet. In order to activate preview mode, it is necessary to create an Excel workbook with a well-defined, fixed structure.

In particular it is necessary to have created, within the Excel workbook, two worksheets called Preview and Groups, in the desired order.

The Preview sheet requires no further preparation, while the Groups sheet needs to be formatted to receive data.

The Preview sheet displays the bitmap of the track present in the Stored Track group currently saved to file. If the stored track of the Master working copy has been edited but not saved, the original track is still loaded. If there is no track associated with the stored track, the Preview sheet remains blank.

The **Groups** sheet instead displays the values of the desired group and working copy elements. In fact, this group must contain preset search keys so that only elements indexed by the keys (if any) are displayed.

To index a group, the following syntax is used:

**[GROUPNAME]** if you want to associate a value to an element of the GROUPNAME group in the Master working copy.

**[GROUPNAME].wc1** if you want to associate a value to an element of the GROUPNAME group in working copy number 1

**[GROUPNAME].wcN** if you want to associate a value to an element of the GROUPNAME group in working copy number N

The names of the elements to which you want to associate the value must be listed in the column under the group. Here it is no longer necessary to indicate which working copy is involved, since this information is in the group name. The element is indexed as follows: **[GROUPNAME].ElementName**.

If the indicated working copy and the corresponding group contains an element with the same name as the requested element, then the value is written in the same row and in the column immediately to the right of the column having that name. The value is the one which is currently saved in the requested working copy.

Therefore when preparing the Groups sheet, always ensure to keep an empty column to the right of each column containing the requests of groups to be displayed.

##### Related Issues

  * [Setup Editor](Commands.md)

  * [Commands](Setup_Editor.md)

  * [Setup Device](Setupcar.md)

  * [Ole Automation](Oleautomation.md)


