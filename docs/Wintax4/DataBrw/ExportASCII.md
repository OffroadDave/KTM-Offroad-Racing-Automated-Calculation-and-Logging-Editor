### Export to ASCII

ASCII is a tab separated values format.

#### Channel

List that contains all the channels of the lap selected.

#### Exported Channel

List that contains the channels to be exported. The elements on the list are added and removed using the two buttons with arrow or by double clicking with the mouse on the name of the channel. Using the Options button you can change the arrangement of the channels or remove them from the list of exported channels.

The configurable items in Export ASCII are:

  * Sampling Rate. All output channels assume the same frequency configured in the Equal Sampling Rate combo.

  * The number of decimals. The value can be any integer between 0 and 15 and the default is the number of decimal of the selected channel.

  * Number Format. This is a list of available output formats:



  *     * Dec, Numeric: the numeric format.

    * Scientific format: the scientific format.

    * Hex: hexadecimal format. The number of decimal has no effect with this format.

    * Bin: binary format. The number of decimal has no effect with this format.

    * ASCII: text format. The number of decimal has no effect with this format.

  * When All Data is selected, all the linked laps are exported; when it is not selected, just a part of the linked laps are exported, namely that defined in the Time Start, Time End interval, which can be set by the user.

  * Append Selected Laps This check box appears if you choose to export more than one lap. If it is not checked, one file for each lap will be produced in input. If on the other hand it is checked, a file will be exported that is obtained from linking the laps selected.




##### Important:

The "Time Start" and "Time End" fields can contain a time value indicated in seconds and must be entered as decimal values (e.g., 0.5, 1.25). Make sure to use the correct decimal format.

Exporting in ASCII format presents some additional configurations; these settings are saved in files type *.ech but will be ignored by types of export processes which don't use it.

  * Delimiters You can choose the delimiter with which the output data will be separated. As for the delimiters, you should remember that they are the same as those used by Excel, namely Tab, comma, semicolon, space; you can also choose a generic delimiter by selecting the Custom option and writing the wording required in the box at the side; the length is always of just one digit.

  * Decimal Separator You can choose the separator used to separate decimal figures. You can choose between comma and full stop.

  * Time Format You can set the format of the Time column, created in the export function based on the set frequency. You can choose between SS:mmm and H:M:S.mmm

  * Info It selects information that will be used to compose the name of the output file as shown in picture below.




On the right of the export window there are six buttons:

  * Export: This button performs the conversion. After clicking this button a message appears asking to save the changes (if it is not done before with the save button) then, at the browser prompt, enter a path and a name for the target export file. Press Save to proceed and OK to complete operation. The default directory is WinTAX4\Export. If more than one lap has been selected and you have chosen not to append them, a message appears asking you to choose whether you wish to write the names of the files manually or automatically. When you write the file names manually, you are requested to enter the name for each file selected, otherwise you are just requested to select the directory in which all the files are to be created.

  * Cancel: It cancels the current operation.

  * Load: choose an .ech export file.

  * Save: Save changes in current .ech file.

  * Save As...: Save changes in a selected or new .ech file.

  * Default: Set the default values of decimals and format.




#### Related Issues

  * [Export](Export.md)

  * [Export to BIN](ExportBIN.md)

  * [Export to CSV](ExportCSV.md)

  * [Export to XLS](ExportXLS.md)

  * [Export to ZTX](ExportZTX.md)

  * [Export Channels to ZTX](ExportZTX.md)

  * [Export to EXP](ExportEXP.md)

  * [Export to Channels to EXP](ExportEXP.md)

  * [Export to Matlab](ExportMATLAB.md)




.