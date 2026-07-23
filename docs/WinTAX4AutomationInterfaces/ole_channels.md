[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# OLE Channels

##  How to create virtual channels via Automation Scripting

WinTAX enables the creation of temporary channels via Automation. These temporary channels can be modified through the internal scripts of WinTAX or outer applications with Automation Client characteristics. These channels, known inside WinTAX as "OLE Channels", enable to customize the analysis capacity exploiting mechanism outside WinTAX. For instance it's possible to: \- Create libraries of virtual channels written with VBScript or JScript scripting languages. \- Automatically import channels through the opening of files created by the customer. \- Carry out an analysis through commercial tools (i.e. MATLAB, EXCEL..) and show the results on WinTAX. \- Exchange information through team proprietary tools (i.e. simulation tools). \- Create special channels with advanced functions: events search, occurrences counters, etc. \- Import and show context information from the network or from remote hosts. 

##  What is an OLE Channel ?

An OLE Channel is simply a container of numerical values in memory. This object is considered as a channel inside WinTAX, so that each environment that uses math channels can automatically use OLE channels without any change. OLE channels are listed on a special page inside the Channel Browser and they can be dropped to the analysis windows. An OLE Channel requires a frequency to determine the number of samples to be stored: the calculation of the samples is as follows: Samples = (Frequency * LapTime) The samples are stored as double values arrays inside WinTAX. Please pay special attention to the quantity of OLE channels created because they are stored in memory until the data set is re-setted by a new loading of data. 

##  Limitations of OLE Channels

OLE Channels are available only in some areas of WinTAX. The current implementation has the following limitations: 1\. OLE Channels are not accessible in the Data Browser (especially for exports). 2\. OLE Channels are not accessible in TelDataX. 

##  How to create an OLE Channel

An OLE Channel is created in the [ITelDataLoad::Channels](interfaceITelDataLoad.md#a2) collection, through the [IWTXChannels::Add()](interfaceIWTXChannels.md#a0) method. The channel created can be modified in memory thus enabling to set its values and its frequency. Please pay attention to the following remarks about OLE Channels: 1\. [ITelChannel::SampleRate](interfaceITelChannel.md#a11) must always be set before having access to the values of an OLE Channel. 2\. The [IWTXChannels::Add()](interfaceIWTXChannels.md#a0) method is completed successfully only if the name required has not been already stored. To remove an OLE Channels from memory ITelChannels::Remove() must be called or a new set of data must be loaded from WinTAX: **when the CurrentData object is refreshed, all OLE Channels available are removed from memory and need to be re-created.** The following example shows the creation of a simple OLE Channel: 
    
    
    Dim oChanOLE
    Dim oLoad
    Dim iRate
    
    ' Add the channel to the main Data Set:
    Set oLoad = Application.CurrentData(0)
    Set oChanOLE = oLoad.Channels.Add( "My_Sample_Channel")
    
    ' Channel Frequency must be set in order to create the array of samples:
    iRate = 100
    oChanOLE.SampleRate = iRate
    
    ' Fills the samples with the result of an elaboration:
    For iTime = 0 To oLoad.Time Step (1 / iRate)
      oChanOLE.Value( iTime ) = iTime * iTime
    next
    

The following example shows the creation of a simple OLE Channel, filled with the results of a math expression: 
    
    
    Sub Make_Script_Channels
    
     Dim oChanScript, oLoad, sExpression
    
     Set oLoad = Application.CurrentData(0)
     Set oChanScript = oLoad.Channels.Add( "MyChanScript" )
     oChanScript.SampleRate = 100
    
     ' Set initial values in oChanScript from a VCH expression:
     sExpression = "=IFGT(DERIV(Gear),0)"
     oChanScript.SetValuesMath( sExpression )
    
     ' Change initial values in oChanScript via VBScript language...
     ...
    
     ' Another example: this OLE channel is filled in a three calculation:
     Set oChanScript2 = oLoad.Channels.Add( "MyChanScript2" )
     oChanScript2.SampleRate = 100
     oChanScript2.SetValuesMath( "=DERIV(Gear)" )
     oChanScript2.SetValuesMath( "=IFGT(MyChanScript2,0)" )
     oChanScript2.SetValuesMath( "=DIAGGT(MyChanScript2,0,100)" )
    
    End Sub
    
    

The following example shows how to create a simple OLE Channel in RealTime, filled with ramp values: 
    
    
    Dim LastTime
    
    Function Application_OnNewLoggingTable(vAcq)
     Dim oChanOLE
     Set oChanOLE = Application.CurrentDataRealTime.Channels.Add("OLE")
     oChanOLE.SampleRate = 100
     LastTime = 0
    End Function
    
    
    Sub Application_OnNewDataRealTime()
     Dim Step
     Dim SampleRate
    
     Set oChanOLE = Application.CurrentDataRealTime.Channels("OLE")
     SampleRate = oChanOLE.SampleRate 
     Step = 1 / SampleRate
     EndTime = Application.CurrentDataRealTime.Time 
    
     Do Until LastTime >= EndTime
    
      oChanOLE.Value( LastTime ) = CInt( LastTime * SampleRate )
      LastTime = LastTime + Step
    
     Loop
    
       
    End Sub
    

The following example shows how to create a simple OLE Channel in RealTime, filled with the results of a math expression: 
    
    
    Function Application_OnNewLoggingTable(vAcq)
     Dim oChanOLE
     Set oChanOLE = Application.CurrentDataRealTime.Channels.Add("OLE")
     oChanOLE.SampleRate = 100
     oChanOLE.SetValuesMath( "=$TIME" ) 
    
    End Function
    

The following example shows how to create a simple OLE Channel that works both in Post Processing and RealTime analysis: 
    
    
    Function CreateChannel( oLoad )
     Dim oChanOLE
     Set oChanOLE = oLoad.Channels.Add("OLE")
     oChanOLE.SampleRate = 100
     oChanOLE.SetValuesMath( "=$TIME" ) 
    End function
    
    
    
    Function Application_OnNewLoggingTable(vAcq)
     call CreateChannel( Application.CurrentDataRealTime )
    End Function
    
    
    Function Application_OnLoadData(bsFileName)
     For Each oLoad In Application.CurrentData
            call CreateChannel( oLoad )
     Next
    End Function
    

The following example shows how to create a simple OLE Channel in Lap Report or in Trend multi lap analysis window (The following example must be included in the script of Lap Report or Trend windows) 
    
    
    Function AddChannel( oLoad )
    
      Set oCHAN = oLoad.Channels.Add( "OLE" )
      oCHAN.SampleRate = 100
      oCHAN.SetValuesMath("=$TIME")
    
    End Function
    
    Function Window_OnLoadData(oLoad)
      AddChannel( oLoad )
      Application.LogText "...load new data..."
    End Function
    
    Function Application_OnLoadData(bsFileName)
            AddChannel( Application.CurrentData(0) )
    End Function
    