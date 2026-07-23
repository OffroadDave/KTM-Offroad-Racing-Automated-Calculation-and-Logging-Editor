## Intelligent Data loading

An Rx-Task Auto Update Run is available in the Acquisition Manager (Fig1); the corresponding button to carry out and notify the process is available on the WinTAX4 [Rx Buttons Bar](../../Menu/View.md) toolbar (see Fig2, like Auto RX Task ).

Fig1 Rx Task settings: Auto Update Run

Fig2 WinTAX4 Rx Buttons Bar: Auto Update Run

This task has three options :

  * Auto Update

  * Select Device

  * Browse data folder




The task does not require current PC to download laps, therefore the user can select a 'Search' folder in the same way as the Auto RX Remote task. The corresponding folder is independent from the path for the auto-rx on remote.

The search for the last Run works as follows: the Auto Update Run searches in the associated folder for new laps acquired. When it finds a new lap, the task does not send a notification to WinTAX4 until the Run is closed.

The 'Closed Run' information can be set only by WinTAX4 while downloading laps in Auto Download (ADL) mode (the condition for a 'closed run' is the successful end of the Auto Download sequence or a change in the Run counter of downloaded lap. The 'Closed Run' information is saved as an additional element into the archive at Run level.

  * If the Auto Update option is enabled, and in the Working Dataset Real Time laps corresponding to the same Run are currently loaded, when the download of a new run is completed WinTAX4 will replace the actual working data-set laps loaded (containing Real Time laps) with corresponding cable laps and the button will not flash. The replacement lap is applied even if the current selection is a single lap selection, a lap comparison with other Datasets or an append of laps within the Working Dataset. All analysis windows associated to Working Dataset are updated, but current zoom is kept.

  * If the Auto Update option is enabled, in the Working Dataset Real Time laps from the same Run are not loaded, when the download of a new run is completed the button will flash for 10 seconds and by clicking on it WinTAX4 will load into the Working Dataset the last cable run downloaded in the append mode.

  * If the Auto Update option is disabled the button will start flashing when the download of a new cable run is completed and by clicking on it WinTAX4 will load the last cable run downloaded into the Working Dataset in append mode.

  * Uncheck the task flag on the root tree to interrupt the task.




The Select Device option allows the user to decide from which single device WinTAX4 receives notifications for new RUNs available. Browsing of the device alias will be based on WTS or FindDevs depending on the acquisition interface used. Anyway the user can manually enter the name of the required Device.