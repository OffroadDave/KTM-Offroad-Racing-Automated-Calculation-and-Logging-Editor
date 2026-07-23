[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# Matlab integration

##  Sample Code

The following example demonstrates how to create WinTAX analysis based on the Matlab application 
    
    
    Sub  CreateMATLABSession
    
    Set Math = Application.MatLabApplication
    
    Math.Execute( "clear" )
    Math.Execute("t = 0:.001:10;")
    Math.Execute("x = sin(2*pi*50*t) + sin(2*pi*120*t);")
    Math.Execute("y = x + 2*randn(size(t));")
    Math.Execute("z=fft(y);")
    
    Application.Open Nothing 
    Application.CurrentData(0).Time = 10 
    Set oChanOLE = Application.CurrentData(0).Channels.Add("t") 
    oChanOLE.SampleRate = 1000
    oChanOLE.GetMatLabValues 0, 10, "t" , Math 
    
    Set oChanOLE = Application.CurrentData(0).Channels.Add("x") 
    oChanOLE.SampleRate = 1000
    oChanOLE.GetMatLabValues 0, 10, "x" , Math 
    
    Set oChanOLE = Application.CurrentData(0).Channels.Add("y") 
    oChanOLE.SampleRate = 1000
    oChanOLE.GetMatLabValues 0, 10, "y" , Math 
    
    Set oChanOLE = Application.CurrentData(0).Channels.Add("z") 
    oChanOLE.SampleRate = 1000
    oChanOLE.GetMatLabValues 0, 10, "z" , Math 
    
    
    End Sub
    
    

##  Structure with time data type

Structures With time is a Matlab variable format that allow to write data from Simulink to Matlab workspace. It's consists of a structure with three fields: time, signals, and blockName. 

  * The time field contains a vector of simulation time steps. 
  * The blockName field contains the name of the Workspace block. 
  * The signal fields contains a structure with three fields: values, dimensions, and label. 
    * The value field contains the array of signal values. 
    * The dimensions field specifies the dimensions of the array of values. 
    * The label field contains the label of the input line. 

Note: WinTAX can manage structures with times with only one dimension. The following example demonstrates how to create a WinTAX analysis based on a Simulink simulation made of logged data previously written by WinTAX in Matlab workspace 
    
    
    Sub RPMSimulation
    
     Set Math = Application.MatLabApplication
    
    'Get RPM from WinTAX data
     Set oWTXChannel = Application.CurrentData(0).Channels("RPM")
    
    'Put RPM in structure with time ( values + stimestamp for simulation )
     oWTXChannel.PutMatlabStructureWithTime "WinTAX_RPM"
    
    'Perform simulation on simulink model "ElabRPM.mdl" variables:
    'In WinTAX_RPM
    'Out Simulink_RPM
     Math.Execute( "sim c:\ElabRPM.mdl" )
    
    'Create a new ole channel to retrieve simulated data
     Set oSimOleChannel = Application.CurrentData(0).Channels.Add("OLE_RPM_SIM")
     oSimOleChannel.SampleRate = 500
    
    'Fill OLE_RPM_SIM with samples of structure with time Simulink_RPM.
     oSimOleChannel.GetMatlabStructureWithTime "Simulink_RPM"
    
    End Sub
    