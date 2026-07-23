### Export to Matlab

Exporting is based on a binary format, where the file consists of a header, a channel header and the samples area. The channels exported are one dimensional arrays of doubles containing the values of the samples. Since the format is binary, there is no need to have Matlab installed to be able to export.

To export towards Matlab, you need to select one or more laps and start the Export to Matlab command from the Tools menu of the Data Browser. With this command, a window opens where you can select the channels to be exported.

#### Channel

List that contains all the channels of the lap selected.

#### Exported Channel

List that contains the laps to be exported. The elements on the list are added and removed using the two buttons with arrow or by double clicking on the name of the channel. On this list, the only parameter that can be edited is the frequency because the values exported will always and in any event be arrays of doubles. Using the Options button you can change the arrangement of the channels or remove them from the list of exported channels.

##### Equal sampling rate

When the check box is selected, the value in the combo box will be the frequency used for all the channels.

##### Append Selected Laps

This check box appears if you choose to export more than one lap. If it is not selected, one file for each lap will be produced in input. If on the other hand it is selected, a file will be exported that is obtained from linking the laps selected. In this second case, another check box is also enabled, namely All Data. When it is selected, all the linked laps are exported; when it is not selected, just a part of the linked laps are exported, namely that defined in the TimeStart, TimeEnd interval, which can be set by the user.

##### Important:

The "Time Start" and "Time End" fields can contain a time value indicated in seconds and must be entered as decimal values (e.g., 0.5, 1.25). Make sure to use the correct decimal format.

##### Info

It selects information that will be used to compose the name of the output file as shown in picture below.

On the right of the export window there are six buttons:

  * Export: This button performs the conversion. After clicking this button a message appears asking to save the changes (if it is not done before with the save button) then, at the browser prompt, enter a path and a name for the target export file. Press Save to proceed and OK to complete operation. The default directory is WinTAX4\Export. If more than one lap has been selected and you have chosen not to append them, a message appears asking you to choose whether you wish to write the names of the files manually or automatically. When you write the file names manually, you are requested to enter the name for each file selected, otherwise you are just requested to select the directory in which all the files are to be created.

  * Cancel: It cancels the current operation.

  * Load: choose an .ech export file.

  * Save: Save changes in current .ech file.

  * Save As...: Save changes in a selected or new .ech file.

  * Default: Set the default values of decimals and format.




#### Related Issues

  * [Export](Export.md)

  * ###### [Export to BIN](ExportBIN.md)

  * ###### [Export to ASCII](ExportASCII.md)

  * ###### [Export to CSV](ExportCSV.md)

  * ###### [Export to XLS](ExportXLS.md)

  * ###### [Export to ZTX](ExportZTX.md)

  * ###### [Export Channels to ZTX](ExportZTX.md)

  * ###### [Export to EXP](ExportEXP.md)

  * ###### [Export to Channels to Exp](ExportEXP.md)


