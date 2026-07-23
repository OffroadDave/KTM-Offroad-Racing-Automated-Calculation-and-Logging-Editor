### Import from ASCII

The import from ASCII function is used to convert general text files into data structures type WinTAX4 (therefore in native ZTX format).

WinTAX4 is able to manage files of different formats and accepts input files, both text type, with various extensions and excel type. Conversion is performed through an assisted procedure (Wizard). Each single file is converted into a lap within a certain run. The file generated at the end of the Import phase is called ImportData.ztx.

It has the same properties as the standard files of WinTAX4, in other words files acquired by a Data Logger by Marelli (cableData.ztx in the case of “cable” data or dstData.ztx in the case of data acquired with “real-time” telemetry). In other words, the data imported are really processed as data in native format, with obvious benefits in terms of performance, reliability and security (the data converted apply the same encrypting logics as standard data).

Finally, the import function allows you to import the data in a new database and also to link the data imported to pre-existent databases. The first function proves useful when, for example, you have to convert old archives generated in a format that is not compatible with WinTAX4. It is consequently extremely useful, for example, for a new customer of the Marelli system. Using the Import ASCII function, you can convert data generated over past years with HW and SW of third parties. The import ASCII function allows you to convert data starting from a single file or from a generic directory, therefore it automates the conversion process of many files. The link function on the other hand is useful, for example, when you want to enrich the database of data generated in the vehicle (or motorbike) with data produced by emulators.

At the end of the import phase, the data processing logic is the same that you would use for any WinTAX4 datum. Just browse with the Databrowser until you reach the directory level required. Import data is consequently visible and selectable as separate laps and can be filtered just like you would do for other types of data. The filter allows you to modify the display of the Databrowser to see either just Import data or Standard data plus Import data. The Import filter shows and loads just the Import data, while if you select any one of the other filters (i.e. Cable) the data deriving from the link between Cable and Import are shown and loaded. The “Intelligent Dataloading” function enables the display, in the ABS field, of a red square by Import type laps.

WinTAX4 therefore has two separate operating modes, based on the type of selection made on the Databrowser:

  * Import filter: Imported data is loaded in WinTAX4, the names of the channels available are shown within the Channel Browser in the “Channels” list (TAB).

  * Any other filters (ALL, Cable, etc.): both the standard channels and the Import data is loaded; the Import data, pointed out with a red icon, are shown within the Channel Browser in the “Import” list (TAB).




If the channels have the same name (case 2) priority is given to the standard data. If, for example, the “RPM” channel is available in both the standard data and in the Import data, WinTAX4 will always and only use standard “RPM”. It is up to the user, in the import phase, to check if any of the names are the same and consequently if there could be any subsequent priority problems. Import data are really processed as standard data and can be used within WinTAX4 in every point of the program (e.g. VCH, analysis windows etc.)

#### 

#### Importing and converting

Once you have selected the Import from ASCII command from the Tools menu of the DataBrowser, first you will see a windows dialog to select the files, which offers, as possible extensions: *.* in the case of text files or *.xls in the case of excel files.

The default path, when the dialog is opened initially, is that set in Setup General – Directories – Import; if it has not been set, the default path of Export to ASCII is used. When you open again subsequently, the last directory used will always be indicated; the path is indeed saved in an xml configuration file, together with other data. Using the selection window you can select a single file, a group of files or a whole directory.

If you choose to select a whole path, a configuration page will appear with the preview on the first file of the folder selected; if you choose N files, the preview will be on the first one selected; in the case of a single choice, the preview is obviously that file.

The preview page, split-up into three steps, allows you to configure the import settings based on the structure of the file displayed. The settings are saved in an xml file and suggested again when you open the import tool later. The preview page is used to set the parameters used to interpret the file, for example, the choice of the line delimiter, the temporal column, the decimal separator. The choice of the settings is assisted by a list that refreshes automatically as the parameters change.

The parameters of the page have the following meaning:

  * Delimiters




Tab, semicolon, comma, space, other

As for the delimiters, you should remember that they are the same as those used by Excel. Delimiters can also be used simultaneously.

Treat consecutive delimiters as one.

With this setting, two consecutive delimiters without any digit in-between are considered as just one delimiter.

  * Select rows




Data Start at Row.

It indicates the line from which the data is imported; the lines before are ignored, which proves useful, for example, when you have to eliminate the general headers of files (many analysis tools often save the headers with information on the track, on the driver, etc.)

Channels Label Row.

It indicates the number of the line from where the converter has to extract the names of the channels. This line is highlighted in yellow and is used just if the relevant check box is selected.

If this check box is not selected, the name of the channels will be given the default settings: Channel1, Channel2, Channel3, etc. With a double click, or pressing space bar on the name of the channel, it is possible to set a custom name.

Measure Unit Row.

It indicates the number of the line from where the converter has to extract the measure unit of the channels, if present. The text in this line is highlighted in blue and is used only if the check box is selected.

  * Decimal Separator




Make sure the decimal separator, full stop or comma, matches what is used in the data and above all make sure it is not the same used to separate the columns, otherwise it will be impossible to import correctly.

  * Data Preview




The path of the file is previewed and the consequential table is based on the settings made in the configuration file or that have been edited on the window.

  * Commands




a. Load Setup...: used to load a configuration saved previously.

b. Back: grey, because it is the first step.

c. Next: moves onto the next step.

d. Finish: closes the import window, confirming all the options selected in this and in the next pages.

e. Cancel: cancels the import process.

If you select Next, you will move onto the next configuration page that will appear with the settings inherited from the previous conversion process.

This second page acts further in detail to make it easier to recognize and extract the data.

  * File Sampling Mode




The channel frequency can be selected in one of the following ways.

a. Fixed Frequency (default)

With this option, you select one of the frequencies suggested in the combo box, which are exactly those admitted by WinTAX4 (1, 2, 5, 10, 20, 100, 200, 500, 1k, 2k, 4k, 10k hertz). The default value when used for the first time is 10Hz. In this case, the samples in the first line of the file are considered at instant 0 and those following are inserted after an interval calculated based on the frequency defined by the user.

b. Timestamp

Timestamp uses the “Time column” as timeline. The values of this channel must be positive and strictly increasing. The output frequency depends on choosing Auto or Custom. Using the combo box, you can choose the format in which the values of the “time column” are formatted. The formats are SS:mmm, H:M:S.mmm, seconds, milliseconds. A brief explanation of formats will appear near the combo box.

  * Auto




In this case, the frequency is obtained from the column that the user identifies as “Time column” calculating the time that lapse between samples. The frequencies admitted by WinTAX4 are: 1, 2, 5, 10, 20, 100, 200, 500, 1k, 2k, 4k, 10k hertz. Therefore, if the frequency calculation does not produce a result that is compatible with the frequencies admitted by WinTAX4, you will set the higher frequency nearest the value calculated with the time column as the frequency. The time lapse could be different between samples; in this case the output frequency will be the higher compatible frequency calculated.

  * Frequency




In this case, the frequency is selected from a combo box; the values are: 1, 2, 5, 10, 20, 100, 200, 500, 1k, 2k, 4k, 10k hertz

When the frequency is not consistent with the time intervals between the samples of the Time column, the converter proceeds with dynamic oversampling or undersampling. Under no circumstances are made interpolations.

  * Column Data Format




These choices are applied to establish how each column is to react during conversion. After selecting each column with the mouse, you can make the following choice. The columns selected are pointed out by the fact that the text and background colors are inverted. Each choice is pointed out immediately on the preview. Each column header shows the type of choice made for it.

a. Time

It is possible to have just one “time column” per conversion process. If the File Sampling Mode is Fixed Frequency, there is no need to set it. It becomes obligatory on the other hand if you have chosen to calculate it from the data of the file (Timestamp).

b. Split

This radio button, allows the selection of the column that represents the "split channel". Only one columns can be configured as "split" in each import operation. If the "autosplit mode" is enabled, the selection of the column is mandatory. The column is also imported as a "normal" data channel.

c. Value

It is the default value for each column and it simply means that the elements of this column will be treated as data. The import function analyzes the first item in each data column and recognize the value format: 

  * If the last character of the string is the letter "h", the value is set as hexadecimal.

  * If the string contains the letter "e", the value is set as scientific.

  * In all other cases the value is set as a decimal




d. Skip column

If a column is marked “Skip”, it is not considered in the conversion process; in the preview, this fact is pointed out with grey background and text crossed out.

  * Auto Split: This checkbox enables the auto-split mode. Controls that are in this area, are enabled only when the autosplit mode is enabled. The available controls are different for each criteria. 

    * Threshold: (triggered when a range is reached.)

      1. Min: Range that trigger the split event if the is equal or below the analyzed data.

      2. Debounce: minimum time (in milliseconds) passed between 2 split events. Default: 0 (no debounce).

    * Boolean condition: (change of state of a channel with value = 0 compared against value != 0.)

      1. Trigger: Variation of the channel state that triggers the split event. (Possible values> False to true, True to False, Any Change. Default: False to True.)

      2. Debounce: minimum time (in milliseconds) passed between 2 split events. Default: 0 (no debounce).

      3. Range: Minimum change value (similar operating mode as IFEQ math function). Default: 0.

    * Counter: (configurable value variation that represent a increasing / decreasing pattern.)

       1. Trigger: Variation of the channel state that triggers the split event. Possible values> Increment, Decrement, Any change. Default: Increment.

       2. Step Size: Size of the increment that triggers the split event. Default: 1.




  * Data Preview




The path of the file is previewed and the consequential table is based on the settings made in the configuration file or that have been edited on the window.

  * Commands




a. Back: moves back to the previous step.

b. Next: moves onto the next step.

c. Finish: closes the import window, confirming all the options selected in this and in the next pages.

d. Cancel: cancels the import process.

Select Next to go to the next configuration page, which will appear with the settings inherited from previous conversions.

In this page you set the coordinates of the output Import file. The rule used by the conversion process is that for each ASCII file selected upon opening, a lap called ImportData.ztx is created.

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

c. Next: grey, because it is the last step

d. Finish: closes the import window, confirming all the options selected in this and the previous pages.

e. Cancel: it cancels the import process

Select Finish to end the operation and the ImportData.ztx file is created, which is subsequently saved in the path selected previously.