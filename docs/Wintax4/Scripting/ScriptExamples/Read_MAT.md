### Reading Matlab file (*.mat) in WinTAX4

This script reads the content of an example of a Matlab binary file (MAT), called "RPM.mat". It is a mono dimensional array of samples.

The content of "RPM.mat" is read by WinTAX4 and imported as an OLE Channel, called "Read_From_MatFile"

Sub ReadMATFiles()

Dim oData, oTime, oChannel, myVar, Math, oChanOLE

Set oData = Application.CurrentData(0)

oTime = Application.CurrentData(0).Time

oFreq = 200

Set oChanOLE = oData.Channels.Add("Read_From_MatFile")

oChanOLE.SampleRate = oFreq

Set Math = CreateObject("matlab.application")

Math.Visible = False

'read MAT file from disk...

Math.Execute("load('C:\RPM.mat')") 

'extract the content of MAT files...

Math.GetWorkspaceData "WTXArray", "base", myVar

oChanOLE.Values(0, oTime) = myVar

End Sub