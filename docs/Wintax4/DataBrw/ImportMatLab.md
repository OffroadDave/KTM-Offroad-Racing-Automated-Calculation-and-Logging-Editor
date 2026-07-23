### Import from Matlab

Import from Matlab is a function used to convert files originating from Matlab. They are imported via Ole Automation, therefore Matlab must be installed, otherwise it is impossible to import them. Conversion is performed through an assisted procedure (Wizard). Each single file is converted into a lap within a certain run. The file generated at the end of the Import phase is called ImportData.ztx, the same as that used for the Import ASCII function.

It has the same properties as the standard files of WinTAX4, in other words files acquired by a Data Logger by Marelli (cableData.ztx in the case of “cable” data or dstData.ztx in the case of data acquired with “real-time” telemetry).

In other words, the data imported are really processed as data in native format, with obvious benefits in terms of performance, reliability and security (the data converted apply the same encrypting logics as standard data).

#### Databrowser

At the end of the import phase, the data processing logic is the same that you would use for any WinTAX4 datum. Just browse with the Databrowser until you reach the directory level required.

Import data is consequently visible and selectable as separate laps and can be filtered just like you would do for other types of data.

A new type of filter allows you to filter the display of the Databrowser to be able to see Import data or standard data plus Import data.

The Import filter shows and loads just the Import data, while if you select any one of the other filters (i.e. Cable) the data deriving from the link between Cable and Import are shown and loaded.

The “Intelligent Dataloading” function enables the display, in the ABS field, of a red square by Import type laps.

#### WinTAX4

WinTAX4 has two separate operating modes based on the type of selection made on the Databrowser:

1) Import Filter: Imported data is loaded in WinTAX4, the available channels names are shown within the Channel Browser in the “Channels” list (TAB).

2) Any other filter (ALL, Cable, etc.): both standard data and Import data are loaded simultaneously; imported channels are shown within the Channel Browser in the “Import” list (TAB) and imported channels are pointed out by a red icon.

If the channels have the same name (case 2) priority is given to the standard data. If, for example, the “RPM” channel is available in both the standard data and in the Import data, WinTAX4 will always and only use standard “RPM”. It is up to the user, in the import phase, to check if any of the names are the same and consequently if there could be any subsequent priority problems.

Import data is really processed as standard data and can be used within WinTAX4 in every point of the program (e.g. VCH, analysis windows etc.).

#### Importing and converting

Once you have selected the Import from Matlab command from the Tools menu of the Data Browser, first you will see a windows dialog to select the files, which offers the choice between files type *.m and *.mat, both extensions associated with the file of Matlab.

The default path, when the dialog is opened initially, is that set in Setup General – Directories – Import; if it has not been set, the default Export path is set. When you open again subsequently, the last directory used will always be indicated; the path is saved in an xml configuration file, together with other data. Using the selection window you can select a single file, a group of files or a whole directory.

If you choose to select a whole path, a configuration page will appear with the preview on the first file of the folder selected; if you choose N files, the preview will be on the first one selected; in the case of a single choice, the preview is obviously that file.

The preview page, split-up into two steps, allows you to configure some parameters. The settings are saved in an xml file and suggested again when you open the import tool later.

The parameters of the page have the following meaning:

  * Sampling Mode




The channel frequency can be selected in one of the following ways.

a. Fixed Frequency (default)

  * Global Sampling Frequency




With this option, you select one of the frequencies suggested in the combo box, which are exactly those admitted by WinTAX4 (1, 2, 5, 10, 20, 100, 200, 500, 1k, 2k, 4k, 10k hertz). The default value when used for the first time is 10Hz. In this case, the samples in the first line of the file are considered at instant 0 and those following are inserted after an interval calculated based on the frequency defined by the user.

This values is used by all channels.

  * Simple Column Sampling Frequency




The frequency can be selected for single channel using the combo in Column Data Format when one or more columns are selected. The frequencies suggested by combo are exactly those admitted by WinTAX4 (1, 2, 5, 10, 20, 100, 200, 500, 1k, 2k, 4k, 10k hertz). The default value when used for the first time is 10Hz. Also in this case, the samples in the first line of the file are considered at instant 0 and those following are inserted after an interval calculated based on the frequency defined by the user.

b. Timestamp

Timestamp uses the “Time column” as timeline. The values of this channel must be positive and strictly increasing. The output frequency depends on choosing Auto or Custom. Using the combo box, you can choose the format in which the values of the “time column” are formatted. The formats are SS:mmm, H:M:S.mmm, seconds, milliseconds.

  * Auto




In this case, the frequency is obtained from the column that the user identifies as “Time column” calculating the time that lapse between samples. The frequencies admitted by WinTAX4 are: 1, 2, 5, 10, 20, 100, 200, 500, 1k, 2k, 4k, 10k hertz. Therefore, if the frequency calculation does not produce a result that is compatible with the frequencies admitted by WinTAX4, you will set the higher frequency nearest the value calculated with the time column as the frequency. The time lapse could be different between samples; in this case the output frequency will be the higher compatible frequency calculated.

  * Frequency




In this case, the frequency is selected from a combo box; the values are: 1, 2, 5, 10, 20, 100, 200, 500, 1k, 2k, 4k, 10k hertz

When the frequency is not consistent with the time intervals between the samples of the Time column, the converter proceeds with dynamic oversampling or undersampling. Under no circumstances are made interpolations.

  * Column Data Format




These choices are applied to establish how each column is to react during conversion. After selecting each column with the mouse, you can make the following choice. The columns selected are pointed out by the fact that the text and background colors are inverted. Each choice is pointed out immediately on the preview. Each column header shows the type of choice made for it.

a. Time

It is possible to have just one “time column” per conversion process. If the File Sampling Mode is Fixed Frequency, there is no need to set it. It becomes mandatory on the other hand if you have chosen to calculate it from the data of the file (Timestamp).

b. Split

This radio button, allows the selection of the column that represents the "split channel". Only one columns can be configured as "split" in each import operation. If the "autosplit mode" is enabled, the selection of the column is mandatory. The column is also imported as a "normal" data channel.

c. Value

it is the default value for each column and it simply means that the elements of this column will be treated as data.

d. Skip column

If a column is marked “Skip column”, it is not considered in the conversion process; in the preview, this fact is pointed out with grey background and text crossed out.

  * Data Preview




The path of the file is previewed and the consequential table is based on the settings made in the configuration file or that have been edited on the window.

  * Commands




a. Load Setup: used to load a configuration saved previously.

b. Back: grey, because it is the first step.

c. Next: moves onto the next step.

d. Finish: closes the import window, confirming all the options selected in this and in the next pages.

e. Cancel: cancels the import process.

Select Next to go to the next configuration page, which will appear with the settings inherited from previous conversions.

In this page you set the coordinates of the output Import file. The rule used by the conversion process is that for each Matlab file selected upon opening, a lap called ImportData.ztx is created.

  * Select Import Destination




a. Import into existing data

If this is the choice made, when you press Finish, the Data Browser opens automatically in special mode. In this case the Data Browser has limited functions and is used just to “Explore” the archives where the data is to be imported (linked).

The Data Browser consequently appears on a blue background to point out that it is in explore mode.

The user shall select the lap in which the ImportData.ztx file is to be added and confirm (execution of the import function) by double clicking the mouse on the lap line selected.

Imported data is saved sequentially, starting from the lap selected during the procedure.

If a number of files have been imported, the next laps will be automatically added to the laps with incremental Abs within the same run; if the existent laps are not suffice, new laps will be added, again with incremental Abs.

b. Import in a folder

In this case the user will select a directory where the data is to be imported (i.e. C:\WinTAX4\Data) and manually set the names of the Track, Session and Device, which will then be used to create the full path of the archive.

Each lap maintains the original run / track run, while abs and lap will be incremented.

The values set in the last conversion process are saved and suggested again in the next one.

  * Data Preview




The path of the file is previewed and the consequential table is based on the settings made in the configuration file or that have been edited on the window.

  * Commands




a. Save Setup...: used to save the current configuration.

b. Back: moves back to the previous step

c. Next: grey, because this is the last step

d. Finish: closes the import window, confirming all the options selected in this and the previous pages.

e. Cancel: it cancels the import process

Select Finish to end the operation and the ImportData.ztx file is created, which is subsequently saved in the path selected previously.