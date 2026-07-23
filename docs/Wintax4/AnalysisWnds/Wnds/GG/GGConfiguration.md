### Configuration Window

The G-G Diagram Window Configuration allows setting the [G-G Diagram](GGFeatures.md) window; it is formed by the Options and Script pages.

The window moreover includes a menu, a toolbar and a pop-up menu that ease the access to the configuration and management commands of the window itself.

####  Options Page

The Options page allows configuring the graphic aspect of the [G-G Diagram](GGFeatures.md) windows and it is divided into 2 sections: Window Options and Graph Options. Each configurable value can be edited by double clicking with the mouse or by pressing the space bar on the selected element.

#####  Window Options

It allows configuring the general settings of the window.

  * Custom color: enables the settings of the customized colors for the window.

  * Window: sets the background color of the window.



  * Grid: shows the settings to enable the grid in the graphic area of the window. The parameters can be modified by editing the corresponding configuration settings.




  * Enable custom grid settings: Enables the grid display with the customized settings.



  1.      1.         * Style: Sets the style of the grid.

        *           * Simple: the grid is formed by lines drawn with the styles configured in Line Style settings.

          * Cross: the grid is displayed with crosses.

          * Circular: the grid is displayed with concentric circles

        * Color: color of the grid.

        * Line style: Sets the style of the grid line (valid if the Style Simple is enabled).

        *           * Solid: continuous line

          * Dash: dashed line

          * Dot: dotted line

          * DashDot: dashed line alternated with 1 dot

          * DashDotDot: dashed line alternated with 2 dots



  * Scale: this parameters configure both scales, X and Y

  *     * Local: if it is checked, it automatically sets the scale range as minimum and maximum value calculated according on both channels available. If it is not clicked, the frequency range configured in the Min. and Max. Boxes will be used to set the scale range.

    * Min: minimum value of the scale range if Local is not enabled.

    * Max: maximum value of the scale range if Local is not enabled.

    * Layout: it is possible to choose between Outside and Inside. Outside shows the classical scale on X and Y axis out of graphic area. Inside shows scales on orthogonal axis inside the graph as displayed in following bitmap




  1.      * Color: color of the scales.




  * Mode: Switch between "All Data" and "Cursor Value". The default setting is "All Data". All Data means that the window shows the cursor and the grouped points; Cursor Value shows only cursor.

  * Cursor style: Configure the shape of the cursor.

  *     * Cross: The cursor is displayed as a little cross.

    * Ball: the cursor is displayed as a colored sphere




#####  Graphs Options

It allows configuring the settings specific for both channels to be graphically displayed.

  * Lateral acceleration (g): displays the name of the channel that identifies the X acceleration. The name of the channel can be edited or dragged from channel list and can become a math expression if the sign = comes first.

  * Longitudinal acceleration (g): displays the name of the channel that identifies the Y acceleration. The name of the channel can be edited or dragged from channel list and can become a math expression if the sign = comes first.

  * Color: Sets the graph color.

  * Style: shows the style of the channel graph. To modify the setting, edit the channel by opening the Channel Properties page where fonts and styles can be configured.




  * Enable custom font settings

  *     1.        * Family: sets the type of font.

       * Dimension: sets the size of font.

       * Scalable: enables the adapting of the font size to be proportional to the window size.

  * Line settings

  *     1.        * Style: sets the style of the graphs line

       *          * None : no line is drawn

         * Line : continuous line

         * Step : stepped line

         * Bordered : continuous line with border

       * Weight: sets the width of the line in pixel.

       * Line color: sets the line color.

       * Border Color: color for the line border

  * Marker Settings section

  *     1.        * Style: style of the markers, graphic elements used to represent the marker.

       *          * None : no markers are drawn

         * Dots : dot

         * Cross : cross

         * Rhomboid : rhomboid

         * Square : square

         * Arrow Down: arrow down

         * Arrow Up: arrow up

         * Vert Line: vertical line

         * Horz Line: horizontal line

       * Weight: size (width) of the markers in pixel.

       * Marker color: color of the markers.




####  Script

The Script page allows configuring the scripts connected to the events of the [G-G Diagram](GGFeatures.md) window or of the application, in VBScript or Jscript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

####  Menu

The [G-G Diagram](GGFeatures.md) window menu enables the access to the following commands, divided into sub menus:

#####  File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the present setting of the window. |
| Cancel |  | Closes the window without applying the present settings. |
| Load |  | Opens a dialog window to select a configuration file (.gg) to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file (.gg) on which the present settings can be saved. |
  
#####  View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl + Shift + Tab | Enables the page of the window previous to the one currently in use. |
| Next page | Ctrl + Tab | Enables the page of the window next to the one currently in use. |
  
#####  Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
#### 

####  Toolbar

The window toolbar enables the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the Keep Visible mode that allows keeping displayed the configuration window while the application windows are being used. |
| Cancel | Closes the configuration window without applying the settings (Similar to the Cancel command of the File menu). |
| Apply | Applies the present settings to the configuration of the graphic window (Similar to the Apply command of the File menu). |
| Load | Similar to the Load command of the File menu |
| Save As | Similar to the Save As command of the File menu |
| Previous page | Similar to the Previous page command of the View menu |
| Next page | Similar to the Next page command of the View menu |
  
##### Related Issues

  * [G-G Diagram window](GGFeatures.md)

  * [Elements of the window](GGWndItems.md)

  * [Functions](GGFunctions.md)

  * [Commands](GGCommands.md)


