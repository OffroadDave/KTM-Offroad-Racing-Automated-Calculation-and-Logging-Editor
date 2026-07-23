### Automatic change of channel label

Function Window_OnZoomChange (vStart, vEnd)

Dim oDoc

Set oDoc = CreateObject("MSXML.DOMDocument")

oDoc.async = False

oDoc.loadXML Window.XML

'cycle all elements <Label> in the document..

Set oLabels = oDoc.getElementsByTagName("Label")

For Each Label In oLabels

If Label.Text <> "LAP" Then

Application.Logtext " Change the channel " & Label.Text & " in LAP"

Label.Text = "LAP"

End If

Next

' Apply the modification to the window..

Window.XML = oDoc.XML

End Function