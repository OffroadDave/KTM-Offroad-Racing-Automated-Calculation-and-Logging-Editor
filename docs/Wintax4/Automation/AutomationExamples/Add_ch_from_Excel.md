### Add WinTAX4 channels from Excel

The following Excel routine allows to put data in WinTAX4 via OLE Channels

Sub AddChannels()

Dim oWintax As Object

Dim oStartData, oTime, oFileName

Set oWintax = CreateObject("WinTAX4.Application")

oWintax.Visible = True

'Open a WinTAX4 lap...

oStartData = "C:\WinTAX4\Data\Track\Session\Device\Run\Lap\cableData.ztx"

oWintax.Open (oStartData)

Set oData = oWintax.CurrentData(0)

oTime = oData.Time

Dim oChanOLE As WTXChannel

Dim oFreq, oLBuffer As Integer

oFreq = 100

oLBuffer = oFreq*oTime

ReDim buffer(oLBuffer)

Set oChanOLE = oData.Channels.Add("ExcelTest")

oChanOLE.SampleRate = oFreq

For i = 0 To Round(oLBuffer, 0)

buffer(i) = i

Next

oChanOLE.Values(0, oTime) = buffer

End Sub