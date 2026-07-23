## Filters

On the DataBrowser two levels of filter are available, the first at session level and the second at lap level. Both are aimed at limiting the number of sessions and laps only to those to be analyzed.

###  Session Filter

The selection of the View/Filters/Session Filters Setup command opens the configuration panel of the Session Filters on the left of the window. The filter is dynamic, i.e. the fields to be filtered are the same as displayed in the sessions list and are configured in the [Header Configuration](Options_Configuration.md#Session) page.

The filter is enabled on the text available in the corresponding columns of the list and can be integrated by using the jolly character * that indicate that the string is complete. The filter is enabled by selecting the check box of the information to be filtered and modified, possibly using the jolly characters, the text to be filtered. The non selected check box hides the text box of the filter but it does not cancel the possible content. To apply the filter, click on Apply Filter or press Enter . The header of the sessions columns indicate which filter is enabled on the column, To remove all session filter, click on Clear All or select the View/Filters/Clear All Filters command.

The View/Filters/Session Filters Setup command that opens the filters panel is also used to hide the panel itself; the filters are enabled also when the panel is hidden.

###  Lap Filter

The lap filters work only on the lap list and reduce the number of laps displayed compared to those really available in the scanned session; the filters are applied by using the [combo of the lap filter](Menu.md#SelectLapFilter) available in the Main toolbar. There are some predefined filters as shown in the next figure

the predefined filters are as follows

  * All No filter is applied and it shows all scanned laps

  * Radio It shows the Radio data; it is a filter linked to the data saved on *.dtx file and it is compatible with WinTAX2 and WinTAX3.

  * Cable It shows only the cable data, i.e. the laps identified by the cableData.ztx file

  * Real Time It shows only the real time data, i.e. the laps identified by dstData.ztx and nbtData.ztx files using to display the priority rules as described in the [laps list](Session_Laps.md#ListaLap) .

  * Flash It shows only the USB/FlashCard data, i.e. the laps identified by the flashData.ztx files

  * Import It shows only the imported data, i.e. the laps identified by the importData.ztx files

  * Video It shows only the video data.




To the above mentioned filters, custom filters can be added created with the following configuration page:

  * Filter Name: sets the name of the filter

  * Acquisition

  *     *       * Source: the laps are filtered based on the source of the data: All no filter is applied, Radio type is compatible with WinTAX2 and WinTAX3 files and it is linked to *.dtx files, Cable filters taking into account only the cable data (files cableData.ztx), Real time filters taking into account only the data acquired in real time (files dstData.ztx and nbtData.ztx), Flash filters only the flash data (files flashData.ztx), Video filters only the video data and Import filters only importData.ztx.

    * Runs

    *       * All Runs: selects all runs

      * Runs: filters so to keep all runs specified in the list of the text folder. The comma is used to list more runs or a double dot to define an interval (Example: 1,2,3,10..12)

      * Last: select the last runs



  1.      * Date / Hour

     *        * All Laps: it is by default and selects all laps of the run

       * Find all laps acquired:

       *          * Between: filters only the laps acquired during the set time range

         * During the previous: filters only the laps acquired during the last set hours.



  * Lap Markers:




The filter is applied according to the selected Markers. By default everything is selected. The [Lap Markers](Editing_context_info.md#Header) are information contained in each lap and define the type of Lap. The two buttons on the options bar enable to select all markers or to deselect them all.

###  Menu 

The menu of the window enables the access to the following commands, divided into sub menus:

#### File Menu

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Apply | Enter | Applies the current settings of the window. |
| Cancel | Esc | Closes the window without applying the current settings. |
| Load |  | Opens and loads an xml file with .lff extension containing custom filters. Possible further filters already available in the window will be cancelled when the loading a new series of filters. |
| Save |  | Saves the settings of the custom filters on a xml file with .lff extension. |
  
#### Edit Menu

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Filter | Adds a new custom filter. |
| Remove Filter | Removes the custom filter selected. Predefined filters cannot be removed. |
  
### Toolbar

The toolbar of the window enable the access to the same commands available in the menus.