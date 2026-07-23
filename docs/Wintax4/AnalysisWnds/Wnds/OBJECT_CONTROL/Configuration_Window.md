### Configuration Window

The Object Control Configuration window allows setting the [Object Control](OBJWndOverview.md) target.

The window also includes a menu, a toolbar and a pop-up menu that ease the access to the configuration and management commands of the window itself.

#### 

####  Control Object

  * Insert Control: Opens the Insert Object Window to select the ActiveX control to insert




Users can select the type of object to insert:

  * Create New: Displays the documents registered on the user's system.

  * Create from File: Displays a Browse button that let user navigate to a file from which to create an object to insert. THIS FEATURE IS NOT CURRENTLY SUPPORTED. A WARNING MESSAGE IS DISPLAYED ON USER CONFIRMATION.

  * Create Control: Displays the controls registered on the user's system. Only controls registered with the “Insertable” registry key are displayed. To add a new object to the list use the Add Control button (ONLY REGISTRATION OF UNMANAGED COM DLLs IS CURRENTLY SUPPORTED. COM ASSEMBLIES SHOULD BE REGISTERED PRIOR TO USING THE WINDOW).



  * Clear: Delete the selected ActiveX control




####  Menu

The [Control Object](OBJWndOverview.md) window menu enables the access to the following commands, divided into sub menus:

#####  File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the present setting of the window. |
| Cancel |  | Closes the window without applying the present settings. |
| Load |  | Opens a dialog window to select a configuration file (.ocw) to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file (.ocw) on which the present settings can be saved. |
  
#### 

####  Toolbar

The window toolbar enables the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode that allows keeping displayed the configuration window while the application windows are being used. |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel command of the File menu). |
| Apply | Applies the present settings to the configuration of the graphic window (similar to the Apply command of the File menu). |
| Load | Similar to the Load command of the File menu. |
| Save As | Similar to the Save As command of the File menu. |
  
##### 

##### Related Issues

  * [Object Control window](OBJWndOverview.md)

  * [Commands](Commands.md)


