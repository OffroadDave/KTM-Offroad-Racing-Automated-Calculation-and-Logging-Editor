## Script Environment

WinTAX4 provides two distinct environments to write a script, in Main Menu (Tools/Scripts...) and in the [windows](WindowsScripting.md).

The Scripts written in Tools/Script can be used as a Macro and Event based. The Macro scripts are the Subroutines that can be only manually run. The scripts Event based are automatically run by WinTAX4 whenever an event happens (i.e. new lap loaded).

In Tools/Script it is possible not only to edit a single script saved in Setup.xml file, but also to manage a set of script files (with extension vbs or js) with a system of Load/Save similar to vch libraries.

On the left of the script editor, the tree shows a General Scripts node, under which all the scripts stored in the Setup.xml configuration are listed. Every node inside General Scripts allows to move the script editor to the beginning of a single routine. Other vbs/js libraries are displayed with the same structure under the External Scripts node.

On the right, the script editor shows the file selected, focusing on the routine selected in the tree.

Two drop-down lists (module + event) allow adding predefined events to the script page: the user can select the component in the left drop-down list, and the event in the right drop-down list. After the event selection, the scripting code is added in the editor page.

The Select Macro combo shows the list of scripts now available

The scripts are carried out when the user name in the combo is selected. All scripts included into the single graph configuration can be only automatically run. All scripts functions are available for the two types of scripts.

### Scripts Editor

The script editor is a simple text editor when the client can write the source code. The editor recognizes automatically if one label is a keyword by changing the text color.

i.e.: Set oData = Application .CurrentData(0)

In the editor the standard copy/cut/paste functions are available

The list of standard VBScript functions are available with the shortcut: ctrl \+ space (also for JScript language, see previous image)

### Error Handling

The errors that can occur during the implementation are written in the History Log Messages window (WinTAX4 menu: View/Log Window). The Log Messages History window shows the number of wrong lines and the error code

### Use of Log Messages

The Log Messages window is also available for the users. It is in fact possible to write (and erase) a personal string (i.e. for source code debugging)