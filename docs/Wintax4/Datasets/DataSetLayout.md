## Save slot in layout

In the layout the slot position for each window is saved, but not the DataSet loaded in the slot. This means that a configuration can be restored taking into account the number of DataSets and their active position on the window.

To restore this situation however the layout must be loaded after the data, otherwise the data loading is applied.

Example:

  1. A layout has four windows A, B, C and D and DataSets 1,2,3 and 4 are active.

  2. The layout is saved.

  3. At the next session of WinTAX4 to restore the situation, first load the 4 DataSets on the slots 1,2,3,4 and then load the layout otherwise the data loading operations will overwrite those written in the layout.




The information in layout can be modified only the next time the layout is saved.