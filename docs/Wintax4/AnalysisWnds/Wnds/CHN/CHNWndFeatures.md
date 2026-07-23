### Functions

  * The [Channels](CHNWndOverview.md) window has the following functions:

  *     * View

    * Channels Selection

    * Dataset




####  View

The channels can be displayed according to the following views: All Channels, Custom and Groups. When the "All Channels" display mode is selected, all logged channels are shown. By selecting "custom mode" instead, the user can enter only the desired channels. Custom mode is the only mode that can also include all other types of channels, like for instance virtual channels, events and Ole. Moreover "custom mode" enables the drag & drop of the channels on the window. If a different mode is selected, dragging & dropping of a channel will switch a window to custom mode. If a dataset that manage groups has been loaded, it will be possible to select the logged channels belonging to a single group. 

####  Channel Selection

The channels can be in multiple selection mode by using CTRL and SHIFT in combination with the left button of the mouse, or through the SHIFT key on the keyboard and the arrow keys. The selection influences the commands "Open in Excel", "Copy Data to Clipboard" and "Properties".

####  Dataset

The window manages the comparison of datasets. A small colored square identifies the corresponding loaded dataset. Using the [Interactive Data Header](../../../Toolbars/ToolbarDataHeader.md) or the Dataset command, the window can switch on or switch off the desired dataset. 

In each column header there is a button with a triangle shape. Pressing the triangle button that corresponds to a column, each of the other columns show the difference of the values of all the channels with the corresponding ones in the delta column as shown in picture below.

.

In this picture the triangle button of column Dataset 2 is pressed and columns Dataset 1 and Dataset 3 show the difference with column Dataset 2.

If the DataSet compared have different channels, the window shows all channels by entering the value NoRx for channels that are not in common.

##### Related Issues

  * [Channels window](CHNWndOverview.md)

  * [Elements of the window](CHNWndItems.md)

  * [Configuration](CHNWndSetup.md)

  * [Commands](CHNWndCommands.md)


