### VCH by Scripts

Using IWTXChannel::SetValuesMath method to put into WinTAX4 OLE Channels the math expressions.

Sub TestSetValuesMath()

Dim oChanScript, oLoad, sExpression

Set oLoad = Application.CurrentData(0)

Set oChanScript = oLoad.Channels.Add( "MyChanScript" )

oChanScript.SampleRate = 100

' Set initial values in oChanScript from a VCH expression:

sExpression = "=IFGT(DERIV(Gear),0)"

oChanScript.SetValuesMath( sExpression )

' Change initial values In oChanScript via VBScript language...

oChanScript.Value(0) = -2

oChanScript.Value(0.01) = -1

oChanScript.Value(0.02) = 0

End Sub