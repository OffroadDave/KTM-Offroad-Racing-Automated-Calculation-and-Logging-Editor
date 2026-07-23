### Configuration Window 

The Histogram Configuration window enables to configure the appearance of the [Histogram](ISTWndOverview.md) window; as for all other windows it includes the pages Options, color Channels, Script .

The window has also a "main menu", a toolbar and a "pop-up menu" that allows configuration and management of the windows commands.

## 

#### 

#### Loading priority

WinTAX4 allows having more than one channel with the same name. After a lap has been loaded (see [Selection and Loading](../../../DataBrw/SelectLoadLap.md)) only one of these channels with the same name is visualized on the analysis window. [Read more here](../../../WTXWorkWindows/WndChannelBrowser/Channel_Browser.md#Loading_priority)

#### 

####  Options

The Options page enables to configure the aspect of the [Histogram](ISTWndOverview.md) window and it is divided in 2 sections: Window and Graph.

### 

#####  Window Options

It allows configuring the general setting of the window.

  * Custom color: allows defining local colors.

  * Window: sets the background color of the window.

  * Graph: sets the background color of the graphic area.

  * Border: sets the borders color of the areas of the window.

  * Grid: allows defining local grid




  1.      1.         * Enable: enables the display of the grid with the customized settings.

        * Style: sets the style of the grid

        *           * Simple: the grid is formed by lines drawn with the styles configured in Line Style settings.

          * Cross: the grid is formed by crosses indicating the intersection of the vertical and horizontal divisions.

        * Color: color of the grid

        * Line style: sets the style of the line of the grid (valid if the Style Simple is set)

        *           * Solid: continuous line

          * Dash: dashed line

          * Dot: dotted line

          * DashDot: dashed line alternated with 1 dot

          * DashDotDot: dashed line alternated with 2 dots



  * Display Mode: stile types for bars; Bar, Solid Bar 3D, Line, Table.

  * X Axis Mode: sets the base for the x-axis, Distance or Time.

  * Y Axis Unit: sets the display mode for bars values: Percent, Values or mixed.

  * Layout: sets the layout of the window: vertical or horizontal.

  * Type: sets the calculous layout: standard or cumulative.

  * Show Label: Show or hides the labels. 

  * Bin Labelling: allow to configure the labelling of X Axis




  * Enable custom labelling: If unchecked, the window uses an automatic labelling system based on space occupation. If checked, you can select the number of labels to display.




#####  Graphs Options

It allows configuring the specific settings for the channel to be displayed. 

  * Label: displays the name of the channel. The name of the channel can be edited and can become a math expression if a = sign comes first.

  * Display:

  *     * Color: sets the color of the columns of the histogram; the colors of each column can be modified manually.

    * Style: sets the style for line. Color, line weight, line type (line, dashed line), Markers styles




  1.      * Unit: show/hide measurement unit



  * Mode: Allows choosing between the "Value" and "Range" mode.




###### Range Mode:

In the "Range" mode it is possible to define the number of divisions, therefore the number of bins, by setting the "Division" field. In this case each "Bin" has a value width that depends on the Scale (automatic or manual) and the “Division” set. We will therefore have that Single Bin Size = (Max scale - Min scale)/Divisions. 

Example: RPM channel, Divisions 20, Scale 0-20,000. Each Bin will have a width equal to (20,000-0)/20 = 1000. 

The "Division" field can take on values between a minimum of 1 and a maximum of 256.

###### Value Mode:

The "Value" mode is suitable for representing discrete values, therefore it is used to represent channels that have precise values, not ranges of values. In this case each "Bins" represents a precise value. 

The number of "Bins" represented depends on the scale (automatic or manual) and the Step values set. We will therefore have that Number of Bins displayed = (Max Scale \- Min Scale)/Step. 

Example: GEAR channel, Step=1, Scale 0-8. Number of Bins = (8-0)/1+1 = 9. The graph will represent the % or time of distribution of these values: 0,1,2,3,4,5,6,7,8

  * Step: step is the length of the single bin and can be configured only in Value mode.

  * Division: quantity of divisions according to how the distribution of the number of samples are calculated. it is possible to calculate it only in Range mode. The minimum number of division is 1 and the maximum could be 32 or 256, depending on the active licence.

  * Scale X

  *     * Local: Auto/Manual mode for x-axis.

    * Min: sets the minimum value of the X manual scale.

    * Max: sets the maximum value of the X manual scale.

    * Color Mode: permits to configure the color of bins with the following window.




In all cases except the "Solid" setting, the configuration of the channels values occurs in the same way: if Local is checked, the intervals are automatically recalculated from the minimum and the maximum values configured; if Local is unchecked, only the number of intervals and the correspondents color are created.

The opening mode of a window by default is Solid.

  * Color Mode

  *     1.        *          * Manual: If Manual is selected, the window generate a group of default colors.

         * Gradient: If Gradation is selected, the button Color is used to choose the base color of gradation.

         * Blend: If Blend is selected, the buttons Start color and End color are used to choose the start and the end colors of blend.

         * Solid: If Solid is selected, this window is empty and bins use a unique color, settings in Color option.

       * Reverse button: This button reverses the order of the colors.

       * Color: If Gradation is selected, the button is used to choose the base color of gradation.

       * Start Color: The start color in blend mode.

       * End Color: The end color in blend mode.




Scale Y

  * Local: Auto/Manual mode for y-axis.

  * Min: sets the minimum value of the Y manual scale.

  * Max: sets the maximum value of the Y manual scale.



  * Condition: The "Condition" field allows to set a predefined condition. The data displayed is calculated by applying this condition. The graphing and calculation of the values in percentage or time of each histogram is then carried out considering the condition; the data of the configured channel corresponding to an instant where the condition is false are not considered. The percentage is however calculated on the totality of the data.




## 

####  Page Color Channel

The Color Channel page enables to configure the aspect of the channel showed as color strips in histogram bands.

### 

  * Color Channel: Enable the configuration of the color channel. When unchecked, the histogram doesn't show any colored strips. 

  * Label: displays the name of the channel. The name of the channel can be edited and can become a math expression if a = sign comes first.

  * Mode: Allows choosing between the "Value" and "Range" mode.



  1.      * ###### Range Mode:

In the "Range" mode it is possible to define the number of divisions, therefore the number of bins, by setting the "Division" field. In this case each "Bin" has a value width that depends on the Scale (automatic or manual) and the “Division” set. We will therefore have that Single Bin Size = (Max scale - Min scale)/Divisions. 

Example: RPM channel, Divisions 20, Scale 0-20,000. Each Bin will have a width equal to (20,000-0)/20 = 1000. 

The "Division" field can take on values between a minimum of 1 and a maximum of 256.

  2.      * ###### Value Mode:

The "Value" mode is suitable for representing discrete values, therefore it is used to represent channels that have precise values, not ranges of values. In this case each "Bins" represents a precise value. 

The number of "Bins" represented depends on the scale (automatic or manual) and the Step values set. We will therefore have that Number of Bins displayed = (Max Scale - Min Scale)/Step. 

Example: GEAR channel, Step=1, Scale 0-8. Number of Bins = (8-0)/1+1 = 9. The graph will represent the % or time of distribution of these values: 0,1,2,3,4,5,6,7,8.




  * Step: the length of each interval in Values mode.

  * Division: quantity of ranges according to which the number of colored strips are calculated [1: 10]

  * Auto Scale: Auto/Manual mode for color channel.

  * Min: sets the minimum value of the color channel manual scale.

  * Max: sets the maximum value of the color channel manual scale.



  * Color Mode: Opens the page Sectors Configuration.

  *     * Color Mode: In the rows of the start-end-color grid it is always possible to manually configure the colors. Otherwise color can be sets with Color Mode combo. If Manual is selected, the window generate a group of default colors.




The opening mode of a window by default is Manual.

  * Color Mode

  *     1.        *          * Manual: If Manual is selected, the window generate a group of default colors.

         * Gradient: If Gradation is selected, the button Color is used to choose the base color of gradation.

         * Blend: If Blend is selected, the buttons Start color and End color are used to choose the start and the end colors of blend.

       * Reverse button: This button reverses the order of the colors.

       * Color: If Gradation is selected, the button is used to choose the base color of gradation.

       * Start Color: The start color in blend mode.

       * End Color: The end color in blend mode.




#### Script

The Script page allows configuring scripts linked to the events of the [Histogram](ISTWndOverview.md) window or application in VBScript or JScript. See the [Windows Scripting](../../../Scripting/WindowsScripting.md) section.

####  Menu

The window Menu allows the access to the following commands, divided into sub menus:

#####  File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | A | Applies the present settings of the window. |
| Cancel |  | Closes the window without applying the present settings |
| Load |  | Opens a dialog window to select a configuration file to be loaded. |
| Save As |  | Opens a dialog window to select a configuration file on which the present settings can be saved. |
  
#####  Edit Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Cut | Ctrl + X | Enabled only on Script page. Copies to clipboard the selected text and removes it from script edit box. |
| Copy | Ctrl + C | Enabled only on Script page. Copies to clipboard the selected text. |
| Paste | Ctrl + V | Enabled only on Script page. Pastes the text from the clipboard in the script edit box. |
  
#####  View Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Previous page | Ctrl \+ Shift + Tab | Enables the page of the window previous to the current one. |
| Next page | Ctrl \+ Tab | Enables the page of the window next to the current one. |
  
##### Help Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Launches the WinTAX4 help, if available. |
  
## 

####  Toolbar

The toolbar of the window enables the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Enables the "Keep Visible" mode that allows keeping the configuration window on the screen while the application windows are being used. |
| Cancel | Closes the configuration window without applying the settings (similar to the Cancel command of the File menu). |
| Apply | Applies the present settings to the configuration of the graphic window (similar to the Apply command of the File menu). |
| Load | Similar to the Load command of the File menu |
| Save As | Similar to the Save As command of the File menu |
| Previous page | Similar to the Previous page command of the View menu |
| Next page | Similar to the Next page command of the View menu |
| Cut | Similar to the Cut command of the Edit menu |
| Copy | Similar to the Copy command of the Edit menu |
| Paste | Similar to the Paste command of the Edit menu |
| Channel Browser | Shows the pop-up menu to select the page in the Channel Browser window. |
  
#### 

####  Pop-up Menu

The pop-up menu of the window can be displayed by clicking with the right button of the mouse on the Options page.

The pop-up menu allows the access to the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Cut | Similar to the Cut command of the Edit menu |
| Copy | Similar to the Copy command of the Edit menu |
| Paste | Similar to the Paste command of the Edit menu |
| Add Graph | Similar to the Add Graph command of the Graph menu |
| Remove Graph | Similar to the Remove Graph command of the Graph menu |
| Move Up | Similar to the Move Up command of the Graph menu |
| Move Down | Similar to the Move Down command of the Graph menu |
| Restore Default | Sets in the channel the settings configured in the Channel Parameters |
| Set As Default | Configures the settings of the channel in the Channel Parameters |
| Edit... | Enables to edit the selected parameter (similar to the double click) |
  
##### Related Issues

  * [Histogram window](ISTWndOverview.md)

  * [Elements of the window](ISTWndItems.md)

  * [Commands](ISTWndCommands.md)


