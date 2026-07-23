### Import from BLF

**Importing and converting**

Once you have selected the Import from BLF command from the Tools menu of the DataBrowser, first you will see 2 windows dialogs to respectively select the BLF file and the DBC file needed to import data. The 2 windows will only allow the user to select *.BLF files in the first window and *.DBC files in the second window.

After the 2 files have been selected a loading popup will appear. As soon as the loading procedure has been completed, the popup will show a "Finished" button.

Click on the "Finished" button to proceed to the next screen.

In the first step a preview of the BLF file that has been loaded is displayed: the user will see a list of available "messages" that have been found during loading.

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

This second page acts further in detail to make it easier to recognize and extract data. All settings in this page refer and apply to BLF acquisition data selected in the first step. The BLF acquisition data currently selected can be seen in Preview of File: field, looking at the progressive number between round brackets after BLF file path.

File Sampling Mode section

This section allows customizing sampling configuration in conversion process. Settings apply to all channels.

  1. **Fixed frequency (default)** With this option, you select one of the frequencies suggested in the combo box, which are exactly those admitted by WinTAX4 (1, 2, 5, 10, 20, 50, 100, 200, 500, 1k, 2k, 4k, 10k hertz). The default value when used for the first time is 10Hz. In this case, the samples in the first line of the file are considered at instant 0 and those following are inserted after an interval calculated based on the frequency defined by the user.
  2. **Timestamp** Timestamp uses values for the Time Master channel as timeline. This setting is available only if selected data acquisition in BLF file has a Time Master channel with sampling rate compliant to WinTAX4 admitted values. The values of this channel must be positive and strictly increasing. The output frequency depends on choosing Auto or Custom. The only format admitted is seconds, as this is the only unit measure adopted in BLF format for Time Master channels.



In Timestamp mode, it is possible to choose the same sampling rate as BLF acquisition data or under sampling conversion.

  1. **Auto** The frequency is obtained from Time Master channel data series.
  2. **Frequency** User can under sample conversion, selecting an acquisition from the combo box.



**Channel Type section** This section shows if the channel currently selected is compliant with the WinTAX4 format and allows including or skipping it from the list of channels to be imported. Note that channels uncompliant with WinTAX4 can't be converted, so they are always skipped. Further, Time Master channel conversion is mandatory.

As in step 1, the "Data Preview" section displays values for first set of samples relative to the channels in the currently selected acquisition data.

**Commands:**

  1. Back: moves back to the previous step
  2. Next: moves onto the next step.
  3. Finish: closes the import window, confirming all the options selected in this and in the next pages.
  4. Cancel: cancels the import process.



Select Next to go to the next configuration page, which will appear with the settings inherited from previous conversions.

In this page you set the coordinates of the output Import file. For each acquisition data in the BLF file selected upon opening, a lap called cableData.ztx is created.

**Select Import Destination**

  1. **Import into existing data** If this is the choice made, when you press Finish, the Data Browser opens automatically in special mode. In this case the Data Browser has limited functions and is used just to "Explore" the archives where the data is to be imported (linked). The Data Browser consequently appears on a highlighted background to point out that it is in explore mode. The user shall select the lap in which the CableData.ztx file is to be added and confirm (execution of the import function) by double clicking the mouse on the lap line selected. If a number of files have been imported, the next laps will be automatically added to the laps with incremental Abs within the same run; if the existing laps are not suffice, new laps will be added, again with incremental Abs. 
  2. **Import in a folder** In this case the user will select a directory where the data is to be imported (i.e. C:\WinTAX4\Data) and manually set the names of the Track, Session and Device, which will then be used to create the full path of the archive.

