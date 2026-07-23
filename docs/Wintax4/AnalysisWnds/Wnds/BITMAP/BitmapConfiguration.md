### Configuration Window

The Bitmap Configuration window allows setting the look of the [Bitmap windows](BitmapFeatures.md); it is formed by the Options and Script pages.

The window moreover has a menu and a toolbar that ease the access to the configuration and management commands of the window itself.

####  Options Page

The Options page allows configuring the graphic aspect of the Bitmap windows.

### 

#####  Window Options

It allows to configure the settings of the layout of the window. Each element, except checks, can be edited by double clicking with the left button of the mouse or with the SPACE bar.

  * Custom Color: allows the setting of the window background color.

  * Window: The color is used for the background of the graphic area; if Custom Color is disabled, the window uses as background the Window color set in Color Settings configuration, in the Default Appearance page of the General Setup window (General Settings, general configuration of the WinTAX4 environment).

  * Layout: this combo permits to choose the layout mode

    * Original size: The image maintains the original size. 

    * Center: The image is centered in the window.

    * Stretch: The image is adapted to window.

    * Fit: The image is adapted to window but maintains original ratio



  * Filename. The user can select any image of type jpg, gif, png, tiff or bmp. When confirming with apply button, the image is copied in the dedicated setup directory existing for each user under WintTAX4\Users. For this reason, in configuration, is displayed only the name of the uploaded file.




####  Script

The Script page allows configuring scripts connected to the events of the [Bitmap View windows](BitmapFeatures.md) or of application, in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

####  Menu

The menu of the window allows the access to the following commands, divided into sub menu:

#####  File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the current settings of the window. |
| Cancel |  | Closes the window without applying the current settings. |
| Load |  | Opens a dialog window to select a configuration file to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file on which the current settings can saved |
  
#####  View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the one currently in use. |
  
#####  Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
### 

####  Toolbar

The window toolbar enables the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode allowing to keep visualized the configuration window while the application window are in use. |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel command of the File menu). |
| Apply | Applies the current settings to the configuration of the graphic window (similar to the Apply command of the File menu). |
| Load | Similar to the Load command of the File menu |
| Save As | Similar to the Save As command of the File menu |
| Previous page | Similar to the Previous page command of the View menu |
| Next page | Similar to the Next page command of the View menu |
  
##### Related Issues

  * [Bitmap window](BitmapFeatures.md)

  * [Elements of the window](BitmapWndItems.md)

  * [Commands](BitmapCommands.md)


