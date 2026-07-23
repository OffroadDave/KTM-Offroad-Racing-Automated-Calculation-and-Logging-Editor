### Merge Archive

This function allows merging archives of up to 4 different archives into a single archive.

This operation can be executed on various levels of data (Sessions, Run or laps) and merges each lap (from each selected path) into a single archive, containing all of the channels and information of the source data.

The requirements to correctly merge the data, is that the laps that are going to be merged have the same "Start Time" information for the various laps. 

The laps will be automatically associated with each other using the "Start time" information and the "Range Start time" (used as a tolerance value for the start time).

The "Merge Archive" function can be started from the menu "Tools" / "Merge Archive" available in the data browser menu.

To work correctly, the "Merge Archive" function will require an input of ZTX files containing "Faster file". If Faster files are not detected, an error message will be displayed. In this case the user needs to generate Faster files using the command in the DataBrowser.

#### Operating modes:

The merging operation can be executed in 3 main modes:

  * Merge Session
  * Merge Run
  * Merge Lap



####  Merge Session

This option allows merging data of an entire "session" generating a single ZTX lap file for each associated laps of the session. At least 2 paths are required to Start a Merge.

The following fields are available as showed in the picture:

Path A: Select in this field the session folder related to DATA A. (eg. from Datalogger1. )

Path B: Select in this field the session folder related to DATA B.(eg. from Datalogger2. )

Path C: Select in this field the session folder related to DATA C. (eg. from Datalogger3. ) This path is optional and will be merged only if the checkbox is enabled.

Path D: Select in this field the session folder related to DATA D. (eg. from Datalogger4. ) This path is optional and will be merged only if the checkbox is enabled.

Path Suffix: The "path suffix" field, if entered, is added to all merged channels of the B, C and D fields when another channel with the same name is found. If the suffix is not entered, a "B", "C" or "D" value is added to the channel name.

Destination Path: Select in this field the path where merged data will be saved.

Destination Device: If the user enters a value in this field, data will be merged and the user customized device will be used in substitution of the original devices of the various datasets. The value will be used also in the folder name of the merged data. If no value is entered, all original values are merged together separated by a + symbol (eg. LoggerA+LoggerB) and this value will be used in the Device field of the data and in the folder name.

Range Start Time: This information is used to search laps when merging data that has a start time that is not perfectly synchronized with the other datasets. The user can enter a tolerance value that will be used to merge data that has a start value different than the reference lap. The default value for this field is 50ms.

####  Merge Run

This option allows merging data of an entire "RUN" and generating a single ZTX file for each associated laps of the run. At least 2 paths are required to Start a Merge.

The following fields are available as showed in the picture above:

Path A: Select in this field the RUN folder related to DATA A. (eg. from Datalogger1)

Path B: Select in this field the RUN folder related to DATA B. (eg. from Datalogger2)

Path C: Select in this field the RUN folder related to DATA C. (eg. from Datalogger3) This path is optional and will be merged only if the checkbox is enabled.

Path D: Select in this field the RUN folder related to DATA D. (eg. from Datalogger4) This path is optional and will be merged only if the checkbox is enabled.

Path Suffix: The "path suffix" field, if entered, is added to all merged channels of the B, C and D fields when another channel with the same name is found. If the suffix is not entered, a "B", "C" or "D" value is added to the channel name.

Destination Path: Select in this field the path where merged data will be saved.

Destination Device: If the user enters a value in this field, data will be merged and the user customized device will be used in substitution of the original devices of the various datasets. The value will be used also in the folder name of the merged data. If no value is entered, all original values are merged together separated by a + symbol (eg. LoggerA+LoggerB) and this value will be used in the Device field of the Data and in the folder name.

Range Start Time: This information is used to search laps when merging data that has a start time that is not perfectly synchronized with the other datasets. The user can enter a tolerance value that will be used to merge data that has a start value different than the reference lap. The default value for this field is 50ms.

####  Merge Lap

This option allows merging laps and generating a single ZTX file containing all of the informations of the source laps. At least 2 paths are required to Start a Merge.

The following fields are available as showed in the picture above:

Path A: Select in this field the ZTX file related to DATA A.

Path B: Select in this field the ZTX file related to DATA B.

Path C: Select in this field the ZTX file related to DATA C. This path is optional and will be merged only if the checkbox is enabled.

Path D: Select in this field the ZTX file related to DATA D. This path is optional and will be merged only if the checkbox is enabled.

Path Suffix: The "path suffix" field, if entered, is added to all merged channels of the B, C and D fields when another channel with the same name is found. If the suffix is not entered, a "B", "C" or "D" value is added to the channel name.

Destination Path: Select in this field the path where merged data will be saved.

Destination Device: If the user enters a value in this field, data will be merged and the user customized device will be used in substitution of the original devices of the various datasets. The value will be used also in the folder name of the merged data. If no value is entered, all original values are merged together separated by a + symbol (eg. LoggerA+LoggerB) and this value will be used in the Device field of the Data and in the folder name.

Range Start Time: This information is used to search laps when merging data that has a start time that is not perfectly synchronized with the other datasets. The user can enter a tolerance value that will be used to merge data that has a start value different than the reference lap. The default value for this field is 50ms. If the difference between start times exceed the Range Start time tolerance value, a message will ask for confirmation to proceed with the merge.

#### Quick Selection using the "Directory tree" of the Data Browser

It is possible to select data for merging sessions via right click on the "Directory Tree" folders of the data browser, instead of using the "Merge Archive" window.

  * Set to "Merge Session - A": configures the selected session folder as DataLogger A.
  * Set to "Merge Session - B": configures the selected session folder as DataLogger B.
  * Set to "Merge Session - C": configures the selected session folder as DataLogger C.
  * Set to "Merge Session - D": configures the selected session folder as DataLogger D.
  * Set to Merge Session - Destination": configures the destination folder where data will be exported.
  * Open "Merge Session": shows the "Merge Archive" windows, to prosecute with the configuration
  * Execute "Merge Session": performs the merging procedure after all of the informations above have been selected. 

