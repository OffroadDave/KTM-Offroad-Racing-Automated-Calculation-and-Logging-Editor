### Import from MDF

The Import from MDF function is used to convert standard ASAM Measurement Data Format files into data structures type WinTAX4 (therefore in native ZTX format).

WinTAX4 is able to manage several versions for the MDF format: 3.00, 3.20, 3.30, 4.00, 4.10. Conversion performes through an assisted procedure (Wizard). Each single MDF data acquisition in each MDF file is converted into a lap within a certain run. The file generated at the end of the Import phase is called ImportData.ztx.

It has the same properties as the standard files of WinTAX4, in other words files acquired by a Data Logger by Marelli (cableData.ztx in the case of "cable" data or dstData.ztx in the case of data acquired with "real-time" telemetry). In other words, the data imported are really processed as data in native format, with obvious benefits in terms of performance, reliability and security (the data converted apply the same encrypting logics as standard data)

. 

Finally, the import function allows you to import the data in a new database and also to link the data imported to pre-existent databases. The first function proves useful when, for example, you have to convert old archives generated in a format that is not compatible with WinTAX4. It is consequently extremely useful, for example, for a new customer of the Marelli system. Using the Import MDF function, you can convert data generated over past years with HW and SW of third parties. The import MDF function allows you to convert data starting from a single file or from a generic directory, therefore it automates the conversion process of many files. The link function on the other hand is useful, for example, when you want to enrich the database of data generated in the vehicle (or motorbike) with data produced by emulators.

At the end of the import phase, the data processing logic is the same that you would use for any WinTAX4 datum. Just browse with the Databrowser until you reach the directory level required. Import data is consequently visible and selectable as separate laps and can be filtered just like you would do for other types of data. The filter allows you to modify the display of the Databrowser to see either just Import data or Standard data plus Import data. The Import filter shows and loads just the Import data, while if you select any one of the other filters (i.e. Cable) the data deriving from the link between Cable and Import are shown and loaded. The "Intelligent Dataloading" function enables the display, in the ABS field, of a red square by Import type laps.

WinTAX4 therefore has two separate operating modes, based on the type of selection made on the Databrowser:

  * Import filter: Import data is loaded in WinTAX4, the names of the channels available are shown within the Channel Browser in the "Channels" list (TAB).>
  * Any other filters (ALL, Cable, etc.): both the standard channels and the Import data is loaded; the Import data, pointed out with a red icon, are shown within the Channel Browser in the "Import" list (TAB).



If the channels have the same name (case 2) priority is given to the standard data. If, for example, the "RPM" channel is available in both the standard data and in the Import data, WinTAX4 will always and only use standard "RPM". It is up to the user, in the import phase, to check if any of the names are the same and consequently if there could be any subsequent priority problems. Import data is really processed as standard data and can be used within WinTAX4 in every point of the program (e.g. VCH, analysis windows etc.).

Channels from MDF files that have engineered data values not represented in a numeric format are not compliant with WinTAX, and will be automatically skipped from importing process.

**Importing and converting**

Once you have selected the Import from MDF command from the Tools menu of the DataBrowser, first you will see a windows dialog to select the files, which offers extensions compliant to MDF format: *.dat, *.mdf, *.mf3, *.mf4 for files strictly compliant to MDF format, *.* to see all files.

The default path, when the dialog is opened initially, is that set in Setup General - Directories \- Import; if it has not been set, the default path of Export to ASCII is used. When you open again subsequently, the last directory used will always be indicated; the path is indeed saved in an xml configuration file, together with other data.

A single MDF file may store several data acquisitions, each one of them corresponding to a telemetry data file. For this reason, only one MDF file at a time is allowed to be selected for import.

The preview page, split-up into three steps, allows you to configure the import settings based on the structure of the MDF file loaded. Configuration settings for the whole import operation are saved in an xml file and again suggested when you'll open the import tool the next time

In the first step a preview of the MDF file loaded is displayed: a list of data acquisitions and the list of channels belonging to the currently selected data acquisition, along with values for a set of first samples.

Data acquisition sets from MDF file are conventionally called Data Groups.

The list of data acquisitions has several columns:

  * N.: conventional progressive numeric index to identify the data acquisition. The number for the selected data acquisition is shown between round brackets after the path of loaded MDF file in Preview of File field.
  * Sorted: the sorted state for the data acquisition, as meant for MDF ASAM standards (Yes for data acquisition sorted, No for unsorted data acquisition).
  * Time Master Channel: the name for the channel designed as Master Time in the data acquisition.
  * Freq. (Hz): sampling rate for data acquisition. WinTAX4 automatically calculates a sampling rate compliant to its standards, taking into account all sampling values for the Time Master channel.
  * Address: physical address for data acquisition block on MDF file.
  * Comment: text comment associated to data acquisition in MDF file.



Selecting a different data acquisition changes the preview of channels and sampling values in the section Data Preview.

Under each channel name the type of the channel itself is displayed:

  * (Time): the channel is designed as the Time Master channel.
  * (Value): standard channel, compliant to WinTAX4; its values will be imported.
  * (Skip): channel values will not be imported, because the channel format is not compliant to WinTAX4 standards or the user has explicitly removed it from import list.



**Commands:**

  1. Load Setup...: used to load a configuration previously saved.
  2. Back: disabled as long as this is the first step.
  3. Next: moves onto the next step.
  4. Finish: closes the import window, confirming all the options selected in this and in the next pages.
  5. Cancel: cancels the import process.



If you select Next, you will move onto the next configuration page that will appear with the settings inherited from the previous conversion process.

This second page acts further in detail to make it easier to recognize and extract the data. All settings in this page refer and apply to MDF acquisition data selected in the first step. The MDF acquisition data currently selected can be seen in Preview of File: field, looking at the progressive number between round brackets after MDF file path.

File Sampling Mode section

This section allows customizing sampling configuration in conversion process. Settings apply to all channels.

  1. **Fixed frequency (default)** With this option, you select one of the frequencies suggested in the combo box, which are exactly those admitted by WinTAX4 (1, 2, 5, 10, 20, 100, 200, 500, 1k, 2k, 4k, 10k hertz). The default value when used for the first time is 10Hz. In this case, the samples in the first line of the file are considered at instant 0 and those following are inserted after an interval calculated based on the frequency defined by the user.
  2. **Timestamp** Timestamp uses values for the Time Master channel as timeline. This setting is available only if selected data acquisition in MDF file has a Time Master channel with sampling rate compliant to WinTAX4 admitted values. The values of this channel must be positive and strictly increasing. The output frequency depends on choosing Auto or Custom. The only format admitted is seconds, as this is the only unit measure adopted in MDF format for Time Master channels.



In Timestamp mode, it is possible to choose the same sampling rate as MDF acquisition data or under sampling conversion.

  1. **Auto** The frequency is obtained from Time Master channel data series.
  2. **Frequency** User can under sample conversion, selecting an acquisition from the combo box.



**Channel Type section**

This section shows if the channel currently selected is compliant with WinTAX4 and allows including or skipping it from the list of channels to be imported. Note that channels uncompliant with WinTAX4 can't be converted, so they are always (Skip). Further, Time Master channel must always be converted.

As in step 1, Data Preview section displays values for first set of samples relative to channels in the currently selected acquisition data.

**Commands:**

  1. Back: moves back to the previous step
  2. Next: moves onto the next step.
  3. Finish: closes the import window, confirming all the options selected in this and in the next pages.
  4. Cancel: cancels the import process.



Select Next to go to the next configuration page, which will appear with the settings inherited from previous conversions.

In this page you set the coordinates of the output Import file. For each acquisition data in the MDF file selected upon opening, a lap called ImportData.ztx is created.

By default only the acquisition data selected in step one will be converted. The Convert all Data Groups allows converting all acquisition data sets present in the loaded MDF file.

**Select Import Destination**

  1. **Import into existing data** If this is the choice made, when you press Finish, the Data Browser opens automatically in special mode. In this case the Data Browser has limited functions and is used just to "Explore" the archives where the data is to be imported (linked). The Data Browser consequently appears on a blue background to point out that it is in explore mode. The user shall select the lap in which the ImportData.ztx file is to be added and confirm (execution of the import function) by double clicking the mouse on the lap line selected. If a number of files have been imported, the next laps will be automatically added to the laps with incremental Abs within the same run; if the existent laps are not suffice, new laps will be added, again with incremental Abs. 
  2. **Import in a folder** In this case the user will select a directory where the data is to be imported (i.e. C:\WinTAX4\Data) and manually set the names of the Track, Session and Device, which will then be used to create the full path of the archive.



The values set in the last conversion process are saved and suggested again in the next one.

**Data Preview**

The path of the file is previewed and the consequential table is based on the settings made in the configuration file or that have been edited on the window.

**Commands**

  1. Save Setup...: used to save the current configuration
  2. Back: moves back to the previous step
  3. Next: grey, because it is the last step
  4. Finish: closes the import window, confirming all the options selected in this and the previous pages.
  5. Cancel: cancels the import process.



Select Finish to end the operation and the ImportData.ztx file is created, which is subsequently saved in the path selected previously.