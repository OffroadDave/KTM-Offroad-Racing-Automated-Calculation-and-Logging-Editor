## WinTAX4 Automation Interfaces

The complete documentation is available with the WinTAX4 pack. The installer copies into the directory .\WinTAX4\Docs the file WinTAX4AutomationInterfaces.chm. The documentation is updated with the new version.

Then the main classes (and their member functions), used for the Data (and their properties) are listed in access. Every description will be provided with a working example. For further details read the documentation

### Active Scripting inside WinTAX4

### IApplication Interface Reference

The IApplication interface describes methods and properties of the WinTAX4 Application Object. The Application Object represents the entire WinTAX4 application.

The Application object contains:

\- Application-wide settings, options and methods.

\- Properties that return top-level objects, such as Windows, configurations, directories, and so on.

Note: For internal scripts use, the IApplication interface is accessible via the Application named object.

### Member Functions

The following examples show how to use some basic member functions

LogText

Current Data

Real Time

Windows

ConfVCHLibraries

ConfParameters

#### HRESULT LogText ( [in] BSTR  sLog ) 

Adds a text to the LOG window.

Sub Test_Log()

Application .LogText( "Hello" )

End Sub

#### HRESULT CurrentData ( [out, retval] IWTXDataLoads **  pVal )

Member of  IApplication that returns one array of IWTXDataLoads, collection that represents the current data loaded in the application. [Read]

Sub Test_Cdata()

Dim oData, oTime

Set oData = Application.CurrentData(0)

oTime = Application.CurrentData(0).Time

MsgBox oTime

End Sub

Note :

each item represents a comparison. A single lap selection is referenced by the first item.

in order to run this script, load a Data

In case of single lap loaded CurrentData(0) represents the Lap

In case of append lap loaded, CurrentData(0) represents the Append

In case of comparison of two lap, CurrentData(0) represents the first item (Lap or Append), CurrentData(1) represents the second item (Lap or Append)

How to obtain the Time object?

The ITelDataLoad (Inherited by IWTXDataLoad Class) interface describes methods and properties of a Data Load (Lap) object. A Data Load object represents a closed set of acquired data. It consists of a set of channels and properties. The items are:

\- HRESULT Time ([out, retval] double *pVal)

\- HRESULT Channels ([out, retval] ITelChannels **pVal)

\- HRESULT Properties ([out, retval] ITelProperties **pVal)

The Time represents the logging time of lap (or append), in seconds

Channels, Properties are a collection (array) of ITeLChannels and ItelProperties

How to obtain the samples of lap?

ITELChannels and ItelProperties are inherited by IWTXChannel (as for IteLDataLoad by IWTXDataLoad )

The Items of ITelChannel are:

\- HRESULT Name ([out, retval] BSTR *pVal)

\- HRESULT Value ([in] double Time,[out, retval] double *pVal)

\- HRESULT Values ([in] double TimeStart,[in] double TimeEnd,[out, retval] VARIANT *pVal)

\- HRESULT Properties ([out, retval] ITelProperties **pVal)

\- HRESULT Offset ([out, retval] double *pVal)

\- HRESULT Gain ([out, retval] double *pVal)

\- HRESULT SampleRate ([out, retval] long *pVal)

\- HRESULT Type ([out, retval] long *pVal)

Therefore, in order to get the samples the lap, the logical structure is the following:

Obtain the object CurrentData from Application, obtain the object Channels from CurrentData, obtain the object Channel from Channels array and finally obtain the Value from Channel

Sub GetData()

Dim oData, oChan

Set oData = Application .CurrentData(0)

Set oChan = oData.Channels( "N" )

oValue = oChan.Value(10)

MsgBox "N Value at time 10 sec= " & oValue

EndSub

How to obtain the properties of CurrentData

In the same way of the access to the sample it is also possible to extract the property of CurrentData

Sub ExportHeader()

Dim fso, f

Const ForWriting = 2

oFileName = InputBox ( "Select The File Name with path and extension" )

Set fso = CreateObject( "Scripting.FileSystemObject" )

Set f = fso.OpenTextFile(oFileName, ForWriting, True)

Dim oData

Set oData = Application .CurrentData(0)

Dim oLevel, oCons, oAbs, oTR, oMarker

Set oLevel = oData.Properties( "FuelLevel" )

Set oCons = oData.Properties( "FuelConsumption" )

Set oAbs = oData.Properties( "Abs" )

Set oTR = oData.Properties( "Run" )

Set oMarker = oData.Properties( "LapMarker" )

f.Write( "HEADER INFO:" )

f.WriteLine

f.Write( "________________________________________________" )

f.WriteLine

f.Write( "Marker =" & oMarker.Data & " " & "Abs = " & oAbs.Data & " " & "TrackRun = " & oTR.Data)

f.WriteLine

f.Write( "________________________________________________" )

f.WriteLine

f.Write( "LapLevel =  " & Round(oLevel.Data, 2))

f.WriteLine

f.Write( "LapConsumption = " & Round(oCons.Data, 2))

f.close

MsgBox ( "Export Finished" )

End Sub

The previous example introduces access to an external export of the values. The properties returned by automation are saved on a txt file indicated by user

#### HRESULT RealTime ( [out, retval] IWTXDataLoad ** pVal ) 

IWTXDataLoad current real time data loaded in the application. [Read]

The logical structure to give a sample of current RealTime Data is the same one of CurrentData

Sub Application _OnNewDataRealTime()

Dim oData, oChan, iValue, iValueOld

Set oChan = oData.Channels(Time)

Set oData = Application .CurrentDataRealTime

Set oChan = oData.Channels( " Time" )

iValue = oChan.Value(oData.Time )

iValueOld = oChan.Value(oData.Time \- 0.05) 

If iValueOld> iValue Then

If iValueOld <> "-1,#QNAN" Then

If iValue <> "-1,#QNAN" Then

Msgbox "Real-Time CloseLap" & " LapTime=" &iValueOld

EndIf

EndIf

EndIf

Next

End Sub

In order to run this script:

  1. Copy the subroutine into the configuration menu of one Real Time window.

  2. Start the Real Time acquisition process




The script reads the "Time" value channel instant after instant and compares this value against "the same value minus 0,05sec..".

If the result of the comparison is lower (valueOld > Ivalue is false) then a message box is displayed showing the lap time.

#### HRESULT Windows ( [out, retval]CollectWindows ** pVal ) 

IWindows collection represents the opened windows in the application. [Read]

All configuration files (windows, settings, user etc.) of WinTAX4 are in XML format

The customer can approach the configuration directly to carry out the modifications.

The following example shows the entering in XML configuration of one window in order to modify a parameter:

Function Window _OnCursorPosChange(TimePos)

Dim strMsg 'As String

Dim strInput 'As String

' Initialize string.

strMsg = "Weight outside range. Enter a number that is lower than 0 Or greater than 10.Press OK To enter the number again. Press Cancel To Exit"

' Prompt user for input.

strInput = InputBox( "Enter The Desired Weight ." )

' Determine if user chose "Cancel".

If strInput <> "" Then

' Test value of user input.

DoWhile strInput < 0 Or strInput > 10

If MsgBox(strMsg, vbOKCancel, "Error!" ) = vbOK Then

strInput = InputBox( "Enter a number between 0 and 10." )

Else

ExitFunction

EndIf

Loop

' Display user's correct input.

MsgBox "The Weight Selected is " & strInput & "."

EndIf

Dim oDoc

Set oDoc = CreateObject( "MSXML.DOMDocument" )

oDoc.async = False

oDoc.loadXML Window .XML

' I try the elements <Label> in document ...

Set oLabels = oDoc.getElementsByTagName( "Weight" )

ForEach Label In oLabels

If Label.Text <> "0" Then

Application .Logtext "Change the Weight " & Label.Text & strInput

Label.Text = strInput

EndIf

Next

' applies the modification to the window

Window .XML = oDoc.XML

End Function

In order to run this script:

  1. Copy the subroutine into the configuration menu of one PostProcessing window (*.gra).

  2. Load a Data

  3. Move a cursor into the graph.




The Script is carried out by CursorPosChange event, reads the XMl configuration of Window, changes the configuration and finally saves the window 

#### HRESULT ConfVCHLibraries( [out, retval]ITelConfiguration** pVal ) 

Returns current WinTAX4 VCH configuration

Sub Load_VCH()

Dim oLibraries

Dim oLib

Set oLibraries = Application .ConfVCHLibraries

ForEach oLib In oLibraries

MsgBox oLib.Name

Next

End Sub

#### HRESULT ConfParameters( [out, retval]ITelConfiguration** pVal )

Returns current WinTAX4 Parameters configuration

Sub Test_Parameters_WRITE()

Dim oCnf

Set oCnf = Application .ConfParameters

Dim oParameters

Set oParameters = oCnf.Objects

Dim oDocXml

Set oDocXml = CreateObject( "MSXML2.DOMDocument" )

oDocXml.async = False

Dim oParameter

ForEach oParameter In oParameters

oDocXml.loadXML oParameter.XML

Dim oLabels

Dim oRangeMin

Dim oRangeMax

oRangeMin= InputBox ( "Range Min =" )

oRangeMax= InputBox ( "Range Max =" ) 

Set oLabels = oDocXml.getElementsByTagName( "Name" )

Set oLabelsRangeMin = oDocXml.getElementsByTagName( "RangeMin" )

Set oLabelsRangeMax = oDocXml.getElementsByTagName( "RangeMax" )

Dim oLabel

ForEach oLabel In oLabels

oLabel.Text = "N"

Next

Dim oLabelRangeMin

ForEach oLabelRangeMin In oLabelsRangeMin

oLabelRangeMin.Text = oRangeMin

Next

Dim oLabelRangeMax

ForEach oLabelRangeMax In oLabelsRangeMax

oLabelRangeMax.Text = oRangeMax

Next

oParameter.XML = oDocXml.XML

Next

End Sub

In order to run this script:

  1. Copy into menu: General/setup/scripts

  2. Select the script into the toolbar Select Macro




The first example (Load_VCH) reads the VCH libraries currently loaded into WinTAX4.

The second example reads the XMl configuration of Parameters settings (menu WinTAX4: Setup/Channels Parameters..), changes for all channels the Name and RangeMin, RangeMax for manual scale settings

#### Jscript Sample

function Window ::OnCursorPosChange(TimePos)

{

var oData;

var oChan;

var iValues;

oData = Application .CurrentData(0);

oChan = oData.Channels( "N" );

iValues = oChan.Value( TimePos );

Application .LogText( iValues );

}

In order to run this script:

  1. Modify the Script Language setting in WinTAX4 menu: Setup/General/Miscellaneous/Application/script language

  2. Restart WinTAX4

  3. Copy the subroutine into the configuration menu of one PostProcessing window (*.gra).

  4. Load some Data

  5. Move a cursor into the graph.


