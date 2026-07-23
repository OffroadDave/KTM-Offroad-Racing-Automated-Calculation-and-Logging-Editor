### Automatic Merge

This function allows merging automatically, incoming archives of up to 4 different data loggers into a single archive.

The user selects up to 4 different folders, where data from loggers will be saved. Also, it is necessary to select a "Destination Path", where merged or completed data will be saved.

The "Automatic Merge Process" can be enabled from the Acquisition Manager window. When enabled, a process will monitor the selected folders and everytime a lap is saved in one of the configured folders, the generated ZTX will be merged with the data from the other folders, lap by lap.

To verify if one or more laps can be matched for merging, the "Automatic Merge" functions uses the "Start Time" value of the lap +/- the "Range StartTime" value. The first lap that is available in Path A or Path B (if the file in Path A is not yet available) is considered the reference lap for the start time.

The following fields are available as showed in the picture:

Path A: Select in this field the DEVICE folder related to DATA A. (eg. from Datalogger1)

Path B: Select in this field the DEVICE folder related to DATA B. (eg. from Datalogger2)

Path C: Select in this field the DEVICE folder related to DATA C. (eg. from Datalogger3) This path is optional and will be merged only if the checkbox is enabled.

Path D: Select in this field the DEVICE level folder related to DATA D. (eg. from Datalogger4) This path is optional and will be merged only if the checkbox is enabled.

Path Suffix: The "path suffix" field, if entered, is added to all merged channels of the B, C and D fields when another channel with the same name is found in Dataset A. If the suffix is not entered, a "B", "C" or "D" value is added to the channel name.

Destination Path: Select in this field the path where merged data will be saved.

Destination Device: If the user enters a value in this field, data will be merged and the user customized device will be used in substitution of the original devices of the various datasets. The value will be used also also in the folder name of the merged data. If no value is entered, all original values are merged together separated by a + symbol (eg. DataLoggerA+DataLoggerB+DataLoggerC+DataLoggerD) and this value will be used in the Device field of the data and in the folder name.

Range Start Time: This optional information is used to search laps when merging data that has a start time that is not perfectly synchronized with the other datasets. The user can enter a tolerance value that will be used to merge data that has a start value different than the reference lap. The default value for this field is 50ms.

TimeOut: The Timeout field defines for how many seconds the merge function will wait after receiving a lap in one of the configured paths before searching for a corresponding lap and proceeding with the merge. This allows the completion of acquisitions of laps before starting the merge procedure. Once the "TimeOut" counter is started, every new matching lap resets the value 

The merge function starts building the list of laps to be merged as soon as any lap is received in one the configured folders, but only merges/copies the laps when the timeout is expired and if at least the A or B laps have been received.

When the timeout expires (and the A or B laps have been received) all matching laps in the A / B / C / D folders are merged with each other and the output is copied to the destination folder or if no other matching lap is found, the lap is copied in the destination path without modifications.

If the merge operation has been completed on all paths indicated in the configuration screen, the output file will be indicated with a green icon otherwise the merge function will keep waiting for data and the lap is marked with the yellow icon visible below. (as indicated below)

### Merge Status

Opening the databrowser, it is possible to see the merge status with the above indicate arrows:

|  | The yellow arrow indicates that the merge has not been completed for all configured paths. |
| --- | --- |
|  | The green arrow indicates that the merge has been completely for all configured paths. |
  
For more information about merge behaviors and special cases please [check this page.](../../DataBrw/MergeArchive/Merging_Context_info_behaviour.md)