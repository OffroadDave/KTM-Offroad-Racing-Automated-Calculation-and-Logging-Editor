[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# Constants and UserRecords handling

Constants and User Records are exposed via the TelDataLoad object. The replication of values is therefore hidden: only the priority values are exposed (that is, Run level info if values are not stored at Lap level, or Lap level info if values are stored both in Run and Lap level). When the client tool changes values, the new values are always stored in the Lap level. The following example demonstrates how to add constant 
    
    
    Sub ChangeConstants
    
     Set oDataSet = Application.CurrentData(0)
    
     'Remove all constants
     For Each oCst In oDataSet.Constants
      oDataSet.Constants.Remove(oCst.Name)
     Next
    
     'Add Constant
     Set oCst = oDataSet.Constants.Add("Constant")
     oCst.Data = 12
    
     'Save
     oDataSet.Save
    
    End Sub
    

The following example demonstrates how to add user record 
    
    
    Sub ChangeUserRecords
    
     Set oDataSet = Application.CurrentData(0)
    
     'Remove all user records
     For Each oUrc In oDataSet.UserRecords
      oDataSet.UserRecords.Remove(oUrc.Name)
     Next
    
     'Add UserRecord
     Set oUrc = oDataSet.UserRecords.Add("setupdir")
     oUrc.Data = Application.LibrariesDirectory
    
     'Save
     oDataSet.Save
    
    End Sub
    