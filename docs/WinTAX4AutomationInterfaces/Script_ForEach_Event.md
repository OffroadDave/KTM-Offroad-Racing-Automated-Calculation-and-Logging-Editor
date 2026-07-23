[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# Sample: Events

Log in WinTAX log pane all event instances on first data set when data is loaded 
    
    
    Function LogEventInstances( oEvent )
      if oEvent.Instances.Count > 0 then
        for each oInstances in oEvent.Instances
          Application.LogText "[ " & oEvent.Group & "-" & oEvent.Name & " ]" & ":" & oInstances.Description & " Lap Time: " & oInstances.Time
        next
      end if
    End Function
    
    Function Application_OnLoadData(bsFileName)
    
      for Each oEvent in Application.CurrentData(0).Events
        LogEventInstances( oEvent)
      next
    
    End Function
    