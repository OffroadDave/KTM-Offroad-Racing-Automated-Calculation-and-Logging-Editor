### Saving WinTAX4 Channels in Matlab file (*.mat)

This script saves the "RPM" channel into the mat file "WinTAX4_RPM.mat" in C:\ drive

Sub SaveMATFiles()

Dim oData, oTime, oChannel, myVar, Math

Set oData = Application.CurrentData(0)

oTime = Application.CurrentData(0).Time

Set oChannel = oData.Channels("RPM")

myVar = oChannel.Values(0, oTime)

Set Math = CreateObject("matlab.application")

Math.Visible = False

Math.PutWorkspaceData "WTXArray", "base", myVar

Math.Execute("save('c:\WinTAX4_RPM.mat', 'WTXArray')") 

End Sub