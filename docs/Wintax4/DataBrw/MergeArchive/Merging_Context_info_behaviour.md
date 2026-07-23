### Context info merging behaviour

The following behaviors will be applied when merging Context info:

#### 1\. Merging Lap Data behaviour:

  * Comments: Comments found in the various laps during merge are concatenated and maintained in the merged lap.

  * Constants: The Information are merged into a unique file. In case of duplicates, the related suffix is added.

  * InfoField: The Information are merged into a unique file. In case of duplicates, the related suffix is added.

  * UserRecord: The Information are merged into a unique file. In case of duplicates, the related suffix is added.

  * Meteo: Information from the primary logger is maintained.

  * LapInfo (Converter): Information from the primary logger is maintained.

  * DiagsTable: Information from the primary logger is maintained.

  * SystemSetup: Information from the primary logger is maintained.

  * LapHeader: Information from the primary logger is maintained.

  * Offset&Gain: Information are merged into a unique file maintaining all information about the merged channels.

  * BitField – Information are merged into a unique file maintaining all information about the merged channels.

  * Enum – Information are merged into a unique file maintaining all information about the merged channels.




#### 2\. Merging RUN data behaviour:

  * Track (CRT / XML): The file from the primary logger is maintained.

  * Session: The file from the primary logger is maintained.

  * Car: The file from the primary logger is maintained.

  * Comment: All found comments for each lap are concatenated 

  * Constant. All Information from each lap are merged in a single file, in case of duplicates the related suffix is added.

  * Offset&Gain: Information are merged into a unique file maintaining all information about the merged channels.

  * BitField: Information are merged into a unique file maintaining all information about the merged channels.

  * Enum: Information are merged into a unique file maintaining all information about the merged channels.




#### Merge Data - Special behaviours

##### Duplicated channel name management:

If the same channel name is found in one or more "Dataloads", the channels of the "DataLogger B, C and D will be renamed adding the suffix that the user has entered in the "PATH B - Suffix Field", "PATH C - Suffix Field" or "PATH D - Suffix Field".

##### Missing laps:

If a lap can't be matched (no comparable "Start time" is detected.), it will be copied without any further modification to the destination folder. 

If this happens a message will be displayed in the log window.

#####  Laps with Different lengths

In case 2 laps have different length time, the merged lap will be created with the longest length of the laps.

All channels of the shortest lap, will be completed with NORX samples for the missing part to reach the length of the longest lap.

If a lap does not exist in the data logger or a matching lap is not found, the lap will be copied in the destination folder without any modification. If this happens a message will be displayed in the log window.