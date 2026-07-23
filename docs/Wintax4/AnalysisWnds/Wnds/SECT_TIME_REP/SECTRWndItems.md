### Elements of the window

The window is formed by two tables, the first represents in details all sections of the lap; the second shows the Ideal Time, the Rolling Lap, the Selected Lap and the difference Selected Lap - Ideal Time and Selected Lap - Rolling Lap.

In the first table each row identifies a lap; the number of laps depends on the configured mode. There are three possible modes

  * Session Mode: All laps of the current session; if the configuration lap filters are enabled, the number of laps might be lower.

  * Run Mode: All laps of the current run; if the configuration lap filters are enabled, the number of laps might be lower.

  * Selected Mode: Only the laps loaded in WinTAX4; if the configuration lap filters are enabled, the number of laps might be lower.




All laps are considered for calculations. The user can configure a filter by selecting a lap marker item from the list of available lap markers. 

Laps can be further filtered by enabling or disabling the check available in the ENABLE column and using the refresh command to reload the report.

The columns have the following meaning:

  * The grey column shows a green arrow in correspondence of the laps loaded on WinTAX4 and a yellow cup in correspondence of the best lap. If the arrow and the cup are on the same lap, only the arrow is displayed. 

  * The Enable column permits to exclude from the calculations the unselected laps. After the first load, all check are enabled. To reload the window after any deselect, use the command Refresh.

  * The Selected Lap column may have only one lap selected at a time. After the first load, the default is the best lap. By changing the Selected Lap, the two "difference" rows ("Diff. Ideal" and "Diff. Rolling") below the "Selected Lap" row are automatically recalculated. 

  * Laps information configured in the window setup environment will be shown in the subsequent columns of the report.

  * The columns coming after the information up to the LapTime column represent Sections into which the Track is divided and they were obtained by using the [Track Editor](../../../Tools/TrackEditor/Track_Editor.md). Each lap is then divided into N sections and for each lap the coverage times of each section are shown. The times of each section are called split time.

  * After the sections, the LapTime column is displayed that represents the coverage time of the vehicle lap. For each lap the sum of all sections times corresponds to the Lap Time.

  * The fore last column shows the Diff. Ideal. It contains the time difference between the Lap Time of the current time and the Ideal Time. If the Lap Time has a value lower than the Ideal Time, the difference is equal to 0.

  * Last columns shows the Diff. Rolling. It shows the time difference between the Lap Time of the current lap and the Rolling Lap. If the Lap Time has a value lower than the Ideal Time, the difference is equal to 0.




The cells forming the various sections have the following properties.

  * The split times which make the Fastest Rolling Lap are in bold.

  * The split times which make the Ideal Time are shown by a configurable color (cyan on the image).

  * The split times of the loaded laps are highlighted and they can be selected making a zoom in other graphs windows (Graph, XY, etc.)

  * The cells in yellow (color can be changed) show a comparison between the split times of a single section: the split times higher than the best one and included in the configured range are highlighted (see Percentage in [Configuration](SECTRWndSetup.md) ).

  * The grayed cells with dash are those relating to filtered laps or disabled laps.

  * The cell in column Lap Time that has the foreground and background colors reversed, representing the best lap.




The second table is always formed by five rows. 

The first one represents the Ideal Time and the second one the Rolling Lap: each row is formed by the various split times that make the two ideal laps; the last column shows the sum of all the tabled split times that are the Ideal Time and the Rolling Lap values. 

The third row represents the Selected Lap and is formed by the same split times that make the same lap in the laps list. 

The fourth and the fifth rows represent the difference from each split time of the Selected Lap with Ideal Time and with Rolling Lap; the last column shows the sum of all the differences in the table. 

##### Related Issues

  * [Section Time Report window](SECTRWndOverview.md)

  * [Configuration](SECTRWndSetup.md)

  * [Commands](SECTRWndCommands.md)


