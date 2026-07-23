[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# Sample: Open data in WinTAX

Open in WinTAX a single lap selection on first data set 
    
    
    Sub Load_Single_Lap
    
      Application.Open "C:\WinTAX4\Data\ARCHIVE\T001\CAR001\Run_000\Lap_00000000_00001\dstdata.ztx"
    
    End sub
    

Open in WinTAX an append selection of two laps on first data set 
    
    
    Sub Load_Append
     'single lap interface
     Dim oLap
     'Laps collection
     Dim oLapsAppend
    
     'create append interface
     Set oLapsAppend = CreateObject("Wtx4DataInterfaces.TelLaps")
    
     'add first lap to selection
     Set oLap = CreateObject("Wtx4DataInterfaces.TelLap")
     oLap.FilePath = "C:\WinTAX4\Data\ARCHIVE\T001\CAR001\Run_000\Lap_00000000_00001\dstdata.ztx"
     oLapsAppend.Add oLap
    
     'add second lap to selection
     Set oLap = CreateObject("Wtx4DataInterfaces.TelLap")
     oLap.FilePath = "C:\WinTAX4\Data\ARCHIVE\T001\CAR001\Run_000\Lap_00000000_00002\dstdata.ztx"
     oLapsAppend.Add oLap
    
     'load selection
     Application.Open oLapsAppend
    
    End Sub
    

Open in WinTAX a compare selection: an append selection on first data set and a single lap on second data set 
    
    
    Sub Load_Compare
    
     'single lap interface
     Dim oLap
     'Laps collection for first data set
     Dim oLapsAppend
     'Lap Selection ( generic array )
     Dim oLapsCompare(2)
    
    
     'create interface for append seleciton 
     Set oLapsAppend = CreateObject("Wtx4DataInterfaces.TelLaps")
    
     'add first lap to selection 
     Set oLap = CreateObject("Wtx4DataInterfaces.TelLap")
     oLap.FilePath = "C:\WinTAX4\Data\ARCHIVE\T001\CAR001\Run_000\Lap_00000000_00001\dstdata.ztx"
     oLapsAppend.Add oLap
    
     'add second lap to selection
     Set oLap = CreateObject("Wtx4DataInterfaces.TelLap")
     oLap.FilePath = "C:\WinTAX4\Data\ARCHIVE\T001\CAR001\Run_000\Lap_00000000_00002\dstdata.ztx"
     oLapsAppend.Add oLap
     
     'Set first data set lap selection ( Append )
     Set oLapsCompare(0) = oLapsAppend
    
     'add lap to selection
     Set oLap = CreateObject("Wtx4DataInterfaces.TelLap")
     oLap.FilePath = "C:\WinTAX4\Data\ARCHIVE\T001\CAR001\Run_000\Lap_00000000_00003\dstdata.ztx"
    
    
     'Create second data set lap seleciton ( single lap )
     Set oLapsCompare(1) = oLap
    
    
     'load selection
     Application.Open oLapsCompare
    
    
    End Sub
    

Open in WinTAX a new lap of 12.5 sec adding a simple OleChannel for test 
    
    
    Sub Load_Empty_Lap
    
      Application.Open nothing
      Application.CurrentData(0).Time = 12.5
    
      Set oChanOLE = Application.CurrentData(0).Channels.Add("Test_Ole_Channel")
      oChanOLE.SampleRate = 100
      oChanOLE.SetValuesMath( "=$time" ) 
    
    End sub
    