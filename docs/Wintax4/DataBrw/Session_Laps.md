## Session List and Lap List

The Data Browser window is mainly dedicated to the two lists that identify the single laps or the groups of laps: on the left there is the Session list and on the right there is the lap list. The first one shows all sessions available in the WinTAX4/Data directory, the second contains the details of the Runs and of the Laps included in the selected session. Strictly connected to the list of session there is the Select Archive combo that defines the directory to be scanned to search for the sessions. The session is intended as a data structure compatible with the [format of WinTAX4 data](DataArchives_Full.md) .

###  Session List

The Session List displays the Track, Session and Device levels creating a line for each of these combinations, but only if they contain valid laps. So if in the Data directory the Track directory is available and in its turn it includes two sessions directories: Session1 and Session2, containing two Device directories each, the session list will display 4 lines, i.e. 4 sessions identified by Track/Session1/Device1, Track/Session1/Device2, Track/Session2/Devicer1 and Track/Session2/Device2 but only if they contain valid laps.

For instance the directories organized as in the following figure, create a sessions list that is visualized in the DataBrowser below.

The Sessions list is also provided with a header specifying the kind of information in the columns. The information in the list can be configured in the Menu by the [View/Data Header](Menu.md#Options) command that opens the [Data Header](Options_Configuration.md) window. The header is interactive and allows to arrange the list alphabetically or following an increasing or decreasing arrangement, on the basis of the elements of the selected column.

The first column always displays an icon that changes according to the selected level. For Track Session and Device level the icon is always the one shown in the previous figure; for lower levels the icon changes.

In the Sessions list no multiple selections are possible.

By clicking with the right button on the list a pop-up menu is displayed recalling some commands from different Menus. To see their detailed description, please read the [Commands](Menu.md) paragraph.

If the licence includes the [Directory Tree](DataBrowserOptimization.md), the list on the left can be divided into two parts thus introducing a new and more detailed possibility to select the laps going beyond the selection on the Select Archive combo.

###  Lap List

The lap list views all laps included in the Track/Session/Device directory divided into Run. The Runs are groups of laps separated among them by horizontal lines.

Using the same example of the Sessions list, the user can view the correspondence between the directories and what is displayed by the DataBrowser. The session is formed by three Runs (Run_001, Run_002, Run_003) each of which contains 10,2,4 laps respectively.

The Lap list is also provided with a header specifying the kind of information in the columns. The information in the list can be configured in the Menu by using the [View/Data Header](Menu.md#Options) command that opens the [Data Header](Options_Configuration.md) window. The header is not interactive and does not allow arrangements that would affect the division Runs of the Laps.

The first column always displays an icon that on the basis of the color gives information about the type of lap available in the directory. For instance if there is a yellow square, it means that the directory contains a cableData.ztx file. A green square means a dstData.ztx file while a light blue square means an nbtData.ztx file. If in the same directory all three types of laps are available, the display gives priority to the cableData.ztx, followed by the dstData.ztx and then by the nbtData.ztx; the square indicates that the directory includes also other types of laps.

The icon beside the squares in a single lap refers to the fastest lap, the best lap. The yellow icon refers to the best lap of all runs; the grey ones are the best lap of each run.

The list of laps is interactive and can be sorted based on Abs, Lap, Run, FilePath (default sorting method), on LapDate and on StartTime. The fields "Logging time" and "Download Time" can be sorted and is based on the date field.

Simply click on the column header to sort the list in incremental or decremental manner. An arrow in the header box indicates the sorting direction. To restore the standard sorting method of WinTAX4, select sort by "FilePath".

By clicking with the right button on the list, a pop-up menu is displayed, it recalls various commands from the different Menus. To see their detailed descriptions, read the [Commands](Menu.md) paragraph.

###### Icons in the lap list

In the lap list, the first columns of the databrowser can contain different type of icons that can be combined together to show information related to the lap that are being visualized.

On the same lap multiple of the following symbol can appear, if for example the examined lap corresponds to 2 or more categories.

| Symbol | Description |
| --- | --- |
|  | A lap with this symbol contains Cable data |
|  | A lap with this symbol contains real time data.(acquired with Real time or RCS) |
|  | A lap with this symbol contains External data. (eg. generated by TelDataZTX) |
|  | A lap with this symbol contains Flash data imported from a USB or flashcard |
|  | A lap with this symbol contains an imported lap |
|  | This lap has been configured as reference lap |
|  | Best lap |
|  | Best lap in run |
|  | a video is available with this lap |
|  | A locked video is available with this lap |
|  | The merge procedure was completed on this lap |
|  | A partial merge procedure was done on this lap |
|  | A lap with this symbol does not contain slow rate data |
|  | A lap with this symbol contains an acquisition table enabled for slow rate real time data |
|  | A lap with this symbol contains an acquisition table enabled for slow rate cable data |
|  | A lap with this symbol contains an acquisition table enabled for slow rate cable + real time data |
|  | A lap with this symbol contains Onboard events |