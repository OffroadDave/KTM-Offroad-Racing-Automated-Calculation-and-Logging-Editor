## Toolbar Data Header

The Data Header Toolbar visualization can be enabled/disabled trough the View/Toolbars/Data Header command of the main menu or through the right click on the toolbars area. The "Data Header" toolbar can be configured but no commands can be added to it. The main function of the Data Header is to show information to the laps loaded; when the datasets are enabled, it becomes interactive as it works also as a control to manage the data on WinTAX4.

If no lap is loaded, the toolbar is displayed as shown in the figure.

When a lap is loaded, the toolbar shows the information about the lap and depending on its configuration, it can become similar to the one shown in the figure where the pop-up menu is highlighted and can be opened by clicking with the right button on the Data Header.

If an append of a lap is loaded, a single row is anyhow displayed showing the information that vary from lap to lap (for example Abs) and that are presented one after the other separated by comma, if consecutive, or as a range separated by a colon, if not. The information LapTime, LapDistance, CronoTime and FuelConsumption are related to the currently displayed lap.

If a comparison of laps is loaded, all lap information configured are displayed. In the following figure the interactive option is set.

Each lap is marked by a number identifying its position in the Dataset (slot)

The shortcuts ALT+N (N = [1,10]) allow switching the N Dataset in the first slot.

The commands of the menu are as follows:

  * Load DataSet into Selected Position Opens the Data Browser and allows to replace the lap in the slot identified by the position of the mouse with a new lap or with a lap append.

  * Load DataSet from Bookmark Opens the list of favorite laps available in the DataSet allowing a quick insert in the slot identified by the mouse.

  * Add DataSet... allows user to select a dataset from Data Browser and stores it in the first available slot.

  * Switch to Working DataSet: selected dataset replaces the working one.

  * Remove the selected DataSet Removes the DataSet selected by the system

  * Set default AutoRX DataSet: Enables the slot of the DataSet where the lap of AutoRx must be loaded during the acquisition.

  * Compact View In case of a comparison of lap, the toolbar is zipped showing just one lap; there is however the possibility to show all laps through the arrow keys displayed on the left.




  * Interactive Data Header When this option is set, to each row a bar is added where the laps belonging to that DataSet are highlighted. Clicking with the mouse on one of these laps, the lap is loaded on WinTAX4. Moreover a red square is added on the left allowing enable/disable the DataSet on the active window. It has the same functions of the commands CTRL+RETURN and CTRL+BACKSPACE on one slot of the DataSet. When the square is red, it means that the selected window has that DataSet active.

  * Properties Opens the [configuration window of the Data Header](../WTXSetup/WndSetupDataHeader/WTXSetupDataHeader.md#WndDataHeaders).




If a toolbar requires more space than that allowed by the window, the exceeding length is cut and navigation buttons are added on the right to allow moving on all information of the Data Header, as shown in the next figure.