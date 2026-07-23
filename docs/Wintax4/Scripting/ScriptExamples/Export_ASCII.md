### Export channels to ASCII file

This script is intended for the Graph windows. It automatically exports all the channels contained in the waveform to an ASCII files (extension *.m) whenever a lap ends.

Function Application_OnNewLap(sFile)

Dim oData, oTime, oFileName, fso, oDoc, oLabels, Label, oChan, oFreq, f, iValue

Const ForWriting = 2

Set fso = CreateObject("Scripting.FileSystemObject")

Set oData = Application.CurrentData(0)

oTime = Application.CurrentData(0).Time

Set oDoc = CreateObject("MSXML.DOMDocument")

oDoc.async = False

oDoc.loadXML Window.XML

Set oLabels = oDoc.getElementsByTagName("Label")

For Each Label In oLabels

If Label.Text <> "" Then

Set oChan = oData.Channels(Label.Text)

oFreq = oChan.SampleRate

oFileName = "\Export\" & oChan.Name & ".m"

Set f = fso.OpenTextFile(oFileName, ForWriting, True)

For j = 0 To oTime Step (1/oFreq)

iValue = oChan.Value(j)

If iValue = "-1,#QNAN" Then

iValue = "Nan"

Else If oChan.Value(j) = "-1,#IND" Then

iValue = "Nan"

End If

f.WriteLine (Replace(iValue, ",", "."))

Next

End If

Next

MsgBox "Export Finished"

End Function