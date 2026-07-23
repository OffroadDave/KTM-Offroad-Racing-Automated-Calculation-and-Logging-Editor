### Automating reading of user data

The following function allows to automatically read a channel from a user-generated file (called “clientData.txt”) if found in the WinTAX4 archive. The name of the OLE Channel and its frequency are loaded from the header of the file. This file is structured as follows:

EXTERNAL_GENERATED //OLE Channel Name

100 //OLE Channel Frequency 

284.30 //Sample

285.72 //Sample

284.63 

285.13

...

...

Function Application_OnLoadData(bsFileName)

Dim oData

Set oData = Application.CurrentData(0)

Dim oFilePath

Set oFilePath = oData.Properties( "FilePath" )

Dim sFilePath

sFilePath = oFilePath.Data

sFilePath = sFilePath & "clientData.txt"

Create_OLE_Channel_From_External_File( sFilePath )

End Function

Function Create_OLE_Channel_From_External_File( FileName )

On Error Resume Next

Err.Clear

Dim objFSO, objTextFile

Dim sRead, sReadLine, sReadAll

Const ForReading = 1, ForWriting = 2, ForAppending = 8

Set objFSO = CreateObject("Scripting.FileSystemObject")

Set objTextFile = objFSO.OpenTextFile( FileName , ForReading )

sReadLine = objTextFile.ReadLine

If Len(sReadLine) = 0 Then

Exit Function

End If

Dim oChanOLE

Dim oLoad

Set oLoad = Application.CurrentData(0)

Set oChanOLE = oLoad.Channels.Add( sReadLine )

sReadLine = objTextFile.ReadLine

iRate = CInt(sReadLine)

oChanOLE.SampleRate = iRate

Dim iTime

iTime = 0

Do

sReadLine = objTextFile.ReadLine

If Len(sReadLine) > 0 Then

If (iTime < oLoad.Time) Then

oChanOLE.Value( iTime ) = CDbl( sReadLine )

iTime = iTime + (1 / iRate)

End If

End If

Loop Until (Len(sReadLine) = 0)

objTextFile.Close

End Function