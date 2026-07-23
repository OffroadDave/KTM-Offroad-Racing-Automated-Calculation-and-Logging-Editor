### Functions

The [Lap Report](REPWndOverview.md) window has the following functions:

  * Visualization of the Elements

  * Channel Selection

  * Mode

  *     * Session

    * Run

    * Selected

    * Single Lap

    * Section

  * Statistics

  * Connect

  * Alarms

  * Level

  * Dataset

  * Multisession




#### Visualization of the Elements

The header and the statistics area can be hidden from configuration.

#### 

#### Channels selection

A channel can be selected by clicking with the left button of the mouse on the info boxes. To make a multiple selection of the channels, select the channels by pressing the CTRL key. The information boxes of the selected channels are highlighted. To remove the selection of a channel, select another channel and click on the down arrow at the beginning of the header.

#### Mode

The main feature of the Lap report window is to display statistics based on different modes. Each statistic is displayed in a cell; if a statistic is not calculated, in the corresponding cell will be display a hyphen.

##### Session mode

In this mode all laps of the current session will be used to create the report. The calculation can take quite a while. You can stop it before the end using the command Stop Operations. In case of comparison, one line for each dataset is visualized and an Indication of "Dataset" is added to the columns. This mode supports up to 11 datasets: (10 datasets + reference lap)

##### Run mode

In this mode all laps of the current run will be used to create the report. The calculation can take quite a while. You can stop it before the end using the command Stop Operations. The green arrow shows the current lap. In case of comparison, one line for each dataset is visualized and an Indication of "Dataset" is added to the columns. This mode supports up to 11 datasets: (10 datasets \+ reference lap)

## 

##### Selected mode

Only laps currently loaded in memory as "append" are used to create the report in selected mode. In case of comparison, one line for each dataset is visualized and an Indication of "Dataset" is added to the columns. This mode supports up to 11 datasets: (10 datasets + reference lap)

##### Section mode

This report is created from current lap section map. Each row shows the statistics of a sector of the current map, if configured. The green arrow shows the section connected with the cursor position on a graph window. This mode supports up to 11 datasets: (10 datasets + reference lap)

In Section Mode, with a comparison of laps, the report displays a row for each lap / dataset.

##### 

##### Single lap mode

Only the first lap currently loaded in memory is used to create the report. In Single Lap the layout can be horizontal or vertical, as shown in the following pictures.

#### Statistics

For each channel that have the Statistic check enabled, the minimum, maximum, average values are marked with the selected colors. In the picture below, the Statistics check in window section of configuration is enabled, so the summary statistics area is visible in the bottom of the window.

#### Connect cursor

The command is available only when the values ​​in the cells are underlined. This happens only on selected lap at the statistics that represent an existing point on the graph, like Min, Max, Start and End. The left click on the cells connect the cursor on the corresponding time.

#### 

#### Alarms

The Lap Report window displays the states of alarm channel if the check in the configuration is enabled. The alarms display is via the background color of the cells (configurable). The user can assign a color to Out of Range (default yellow) and one for Alarm (default red). If there are no alarms, the background color is the normal background color of the cells.

The alarms are displayed on the window only if the channel is associated with an alarm and only if the statistic is compatible with the alarm.

The alarm display is based on the following rules, valid for all modes except Section where the default fields Time and Distance are excluded from computation:

Min, Max, Start, End and Event

With Min, Max, Start, End and Event, WinTAX4 checks if the channel is in alarm in the instant where the value of the statistic was calculated. The alarm is displayed using the values configured in Setup / Channel Parameters. If the alarm also has a condition configured, it is necessary to determine if the condition is TRUE or FALSE in the instant where the value has been calculated:

  * if the condition is TRUE, all alarms are confirmed
  * if the condition is FALSE, the channel alarm is not triggered.



About the condition: if the alarm has no condition configured the calculation does not consider it. If the condition is configured, it may be valid or invalid.

a. It is valid if it belongs to the conditions configured in the editor and if its expression is correct. In this case, Lap Report performs the calculation to verify where is TRUE or FALSE.

b. The condition is not valid if the name configured in the alarm does not match any existing condition or if there is a match but its expression is not correct. In this case Lap Report assumes that the condition does not exist, similarly to the case where no conditions are configured.

Avg and Median Corner

For these statistics the alarm is displayed using the values set in Setup / Channel Parameters but without using the Condition. Condition is not applicable because Avg and Median corner does not refer to instantaneous values.

StdDev and Change

For these statistics the display of alarms is not provided because values are not comparable with the channel values.

#### Level

Level permits the visualization of the percentage of the value respect to the maximum. The cell will be partially colored and the level correspond to the value of the cell compared to the maximum value.

#### 

#### Dataset

Dataset commands permit the connection between reports and datasets. With the selection of a dataset, it is possible to view different lap reports with different data loaded. The command is active when Lap Report is configured in Session, Run, Selected e Single Lap mode. If Section mode is selected, or if you use the window in multisession, the command is inactive. Only one dataset at a time can be selected. Opening one, it automatically disables the previous. When Default is selected, the report works in the standard way and using data from the working dataset.

##### Multi-Dataset in Lap Report

If the "Default" mode is selected, (NOTE: if multiple dataset have been loaded, the "Default" mode is automatically enabled) and the lap report is configured either as "Run" or "Session" and multiple laps are loaded in compare mode, in the lap report window a column "Dataset" is added on the lap report window. 

To make it easier to distinguish between datasets, each dataset will retain its own color in the window as configured in the general settings and as it is possible to view on the data header and in the data browser windows. 

Just like what happens with the colors, the visualized number in the "Slot" column, indicates the Dataset slot as it is visualized in the data header.

It's possible to visualize one dataset at a time using the same lap report window by clicking on "Dataset" and selecting the desired Dataset.

If the compared data contains one or more dataset that have a different number of laps, all laps that are missing from other datasets will be shown with a "-" symbol.

#### 

#### Multi Session mode

The Multisession Mode allows generating Lap Report for more than one session. Sessions can be selected from the Data Browser by means of mouse and CTRL button. Then Sessions can be loaded in WinTAX4 pressing “Enter”. This action has only effect into Lap Report and [Trend](../TRE/TREWndOverview.md) windows. The green arrow shows the current lap.

### Related Issues

  * [Lap Report window](REPWndOverview.md)

  * [Elements of the window](REPWndItems.md)

  * [Configuration](REPWndSetup.md)

  * [Commands](REPWndCommands.md)


