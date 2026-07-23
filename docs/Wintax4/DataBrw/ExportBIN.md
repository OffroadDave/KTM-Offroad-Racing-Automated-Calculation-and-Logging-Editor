### Export to BIN

BIN is a compact (interleaved) format. For further details, see [WinTAX4 .bin format](../ExportFormats/binary_file_format.md)

#### Channel

List that contains all the channels of the lap selected.

#### Exported Channel

List that contains the channels to be exported. The elements on the list are added and removed using the two buttons with arrow or by double clicking on the name of the channel. Using the Options button you can change the arrangement of the channels or remove them from the list of exported channels. The configurable items in Export Bin are:

  * Frequency. If Equal Sampling Rate is checked, all channels assume the same frequency, otherwise you can configure on the list the frequency of each channel. The maximum frequency allowed is 2Khz. The default, when a channel is included in the exported channels list, is the frequency of the channel.

  * The number of decimals. The value can be any integer between 0 and 7 and the default is the number of decimal of the selected channel. The combo has a range that varies from 0 to 15; in the case exceeded the limit of 7, the number of decimal is displayed in brackets.

  * Number format that will be written in the header of each channel. This is a list of the possible formats:



  *     * Dec, Numeric, Scientific: the numeric format.

    * Hex: hexadecimal format.

    * Bin: binary format.

    * ASCII: text format.

  * When All Data is selected, all the linked laps are exported; when it is not selected, just a part of the linked laps are exported, namely that defined in the Time Start, Time End interval, which can be set by the user.

  * Append Selected Laps This check box appears if you choose to export more than one lap. If it is not checked, one file for each lap will be produced in input. If on the other hand it is checked, a file will be exported that is obtained from linking the laps selected.




##### Important:

The "Time Start" and "Time End" fields can contain a time value indicated in seconds and must be entered as decimal values (e.g., 0.5, 1.25). Make sure to use the correct decimal format.

On the right of the export window there are six buttons:

  * Export: This button performs the conversion. After clicking this button a message appears asking to save the changes (if it is not done before with the save button) then, at the browser prompt, enter a path and a name for the target export file. Press Save to proceed and OK to complete operation. The default directory is WinTAX4\Export. If more than one lap has been selected and you have chosen not to append them, a message appears asking you to choose whether you wish to write the names of the files manually or automatically. When you write the file names manually, you are requested to enter the name for each file selected, otherwise you are just requested to select the directory in which all the files are to be created.

  * Cancel: It cancels the current operation.

  * Load: choose an .ech export file.

  * Save: Save changes in current .ech file.

  * Save As...: Save changes in a selected or new .ech file.

  * Default: Set the default values of frequency, decimals and format.




##### Related Issues

  * [Export](Export.md)

  * ###### [Export to ASCII](ExportASCII.md)

  * ###### [Export to CSV](ExportCSV.md)

  * ###### [Export to XLS](ExportXLS.md)

  * ###### [Export to ZTX](ExportZTX.md)

  * ###### [Export Channels to ZTX](ExportZTX.md)

  * ###### [Export to EXP](ExportEXP.md)

  * ###### [Export to Channels to EXP](ExportEXP.md)

  * ###### [Export to Matlab](ExportMATLAB.md)


