### Export to ZTX

Export to ZTX simply copies the selected laps to the path indicated. It replicates the directory levels of the ZTX archive. 

### Export Channels to ZTX

In various situations it could be necessary to extract individual data files or groups of data files from a WinTAX4 archive. Channels to ZTX creates new ZTX files containing only the selected channels. 

#### Channel

List that contains all the channels of the selected lap.

#### Exported Channel

List that contains the channels to be exported. The elements on the list are added and removed using the two arrow buttons or by double clicking on the name of the channel. Using the Options button you can change the order of the channels or remove them from the list of exported channels.

The channel list displays the channel name, frequency, size, the number of decimal places and the format type for each channel. These values are normally fixed and unalterable. However in some licenses the value of the frequency can be changed for each channel as shown in the picture below.

The configurable items in Export CSV are:

  * When All Data is selected, all the linked laps are exported; when it is not selected, just a part of the linked laps are exported, namely that defined in the Time Start, Time End interval, which can be set by the user.

  * Append Selected Laps This check box appears if you choose to export more than one lap. If it is not checked, one file for each lap will be produced in input. If on the other hand it is checked, a file will be exported that is obtained from linking the laps selected.

  * Generate Folder Structure: If checked, replicates the levels of the ZTX archive; if unchecked creates only the lap level directory.




##### Important:

The "Time Start" and "Time End" fields can contain a time value indicated in seconds and must be entered as decimal values (e.g., 0.5, 1.25). Make sure to use the correct decimal format.

On the right of the export window there are five buttons:

  * Export: This button performs the conversion. After clicking it a message appears asking to save the changes (if it has not been done before with the save button) then, at the browser prompt, enter a path and a name for the target export file. Press Save to proceed and OK to complete operation. The default directory is WinTAX4\Export. If more than one lap has been selected and you have chosen not to append them, a message appears asking you to choose whether you wish to write the names of the files manually or automatically. When you write the file names manually, you are requested to enter the name for each file selected, otherwise you are just requested to select the directory in which all the files are to be created.

  * Cancel: Quit the export window.

  * Load: choose an .ech export file.

  * Save: Save changes in current .ech file.

  * Save As...: Save changes in a selected or new .ech file.




License optional features:

Some of the following features are available only using some licenses:

Export to licence code: Some licenses (e.g. Expert licenses) are enabled to change license code during data export (ex. In the above picture, if "License 1" is selected before exporting, all of the exported data will be exported with "license 1" code.)

Note on protection levels: Some licenses manage the protection levels for each channel. In general, the channel protection level is maintained during export. In some licenses, however, during the export the channel level is forced to be without protection; in this situation all channels in the exported file will be visible using any licence.

Export all protected channels: If this checkbox is enabled, WinTAX will export hidden channels together with all other channels. When this flag is enabled it's possible to export data even if the are no visible channels (only hidden channels have been added to the export list) ready for export.

#### Related Issues

  * [Export](Export.md)

  * ###### [Export to BIN](ExportBIN.md)

  * ###### [Export to ASCII](ExportASCII.md)

  * ###### [Export to CSV](ExportCSV.md)

  * ###### [Export to XLS](ExportXLS.md)

  * ###### [Export to EXP](ExportEXP.md)

  * ###### [Export to Channels to EXP](ExportEXP.md)

  * ###### [Export to Matlab](ExportMATLAB.md)


