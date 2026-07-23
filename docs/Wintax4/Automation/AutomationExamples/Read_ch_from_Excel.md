### Read WinTAX4 channels from Excel

The following Excel routine allows reading WinTAX4 data

Sub ReadChannels()

Dim oWintax As Object

Dim oStartData, oTime, oFileName

Set oWintax = CreateObject("WinTAX4.Application")

oWintax.Visible = True

'Open a WinTAX4 lap...

oStartData = "C:\WinTAX4\Data\Track\Session\Device\Run\Lap\cableData.ztx"

oWintax.Open (oStartData)

Set oData = oWintax.CurrentData(0)

Dim myChannel As WTXChannel

'only for demonstration purpose, get the channel "RPM" ..

Set myChannel = oData.Channels("RPM")

'only for demonstration purpose, the first value of RPM" is..

FirstValue = myChannel.Value(0)

MsgBox FirstValue

End Sub