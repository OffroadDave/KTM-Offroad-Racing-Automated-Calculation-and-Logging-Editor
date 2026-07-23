[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# Sample: Current Data Properties

The [IWTXDataLoad](interfaceIWTXDataLoad.md) interface exposed by objects [IApplication::CurrentData](interfaceIApplication.md#a6) and [IApplication::CurrentDataRealTime](interfaceIApplication.md#a7) allows to get not only channel samples, but also properties of selected archives. The following example demonstrates how to get current DataLoad properties via an internal script to the WinTAX application: 
    
    
    Sub Show_Properties
    Dim oSelection
    Set oSelection = Application.CurrentData(0)
    Dim oProp
    For Each oProp In oSelection.Properties
    Application.LogText oProp.Name & " = " & oProp.Data
    Next
    End Sub
    

The following example demonstrates how to get DataLoad properties referenced by name: 
    
    
    Sub Show_Current_FilePath
    
    Dim oData
    For Each oData In Application.CurrentData
    
      Dim oFileName, oFilePath
      Set oFileName = oData.Properties( "FileName" )
      Set oFilePath = oData.Properties( "FilePath" )
     
      Application.LogText "Current File Selected : " & oFilePath.Data & oFileName.Data
    
    Next
    
    End Sub
    

The following example demonstrates how to get the current DataLoad properties for all laps in append via an internal script to the WinTAX application: 
    
    
    Sub Show_Properties_Laps_In_Append
    
    Dim oSelection
    Set oSelection = Application.CurrentData(0)
    
    Dim oLap
      For Each Lap in oSelection.Laps
         Dim oProp
         For Each oProp In Lap.Properties
           Application.LogText oProp.Name & " = " & oProp.Data
         Next
      Next
    
    End Sub
    