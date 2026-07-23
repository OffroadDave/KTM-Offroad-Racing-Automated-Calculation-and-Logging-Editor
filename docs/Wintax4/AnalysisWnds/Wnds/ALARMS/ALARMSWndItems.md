### Window Elements

The window is formed by a single area where the alarm boxes of each channel are automatically displayed. The window shows the name and the maximum and the minimum values of each channel. The values are indicated in text boxes with different background colors according to the alarm conditions. The standard colors are:

  * Green, used to indicate an ideal situation. In this case the value is within the Min and Max Limits.

  * Red, used to indicate an alarming situation. In this case the value is within the Valid Range but exceeds the Min and Max Limits.

  * Yellow, used to indicate an invalid value. In this case the value exceeds the Valid Range.




Each alarm could be configured with a condition. The condition limits the alarm trigger only for intervals in which the condition is verified; if the condition is false the alarm will not be identified. If the condition is wrong or not valid, the condition is ignored. In the Alarm Window, if the condition is valid but return False in the whole range, the window shows the maximum and the minimum values of the channel with green background color.

The alarms shown in the picture are configured by giving the following values:

RPM_L: OK Range [0,15000] - Valid Range [0,20000]

T_Oil: OK Range [10,90] - Valid Range [0,100]

T_Water: OK Range [30,70] - Valid Range [-10,90]

In some licenses the values are underlined; with a left click on a value in this situation, the layout cursor moves at the corresponding time in all open windows.

##### Related Issues

  * [Alarms window](ALARMSWndOverview.md)

  * [Configuration](ALARMSWndSetup.md)

  * [Commands](ALARMSWndCommands.md)


