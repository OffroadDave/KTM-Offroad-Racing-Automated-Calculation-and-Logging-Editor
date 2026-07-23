### Automatic change of line weight

# Function Window_OnZoomChange(vStart, vEnd)

Dim strMsg 'As String

Dim strInput 'As String

' Initialize string

strMsg = "Weight outside range. A number lower than 0 Or greater than 10 was entered. Press OK To enter the number again. Press Cancel To Exit"

'Prompt user for input

strInput = InputBox("Enter The Desired Weight")

' Determine if user selects"Cancel"

If strInput <> "" Then

'Check value of user input.

Do While strInput < 0 Or strInput > 10

If MsgBox(strMsg, vbOKCancel, "Error!") = vbOK Then

strInput = InputBox("Enter a number between 0 and 10")

Else

Exit Function

End If

Loop

'Display user's correct input.

MsgBox("The Weight Selected is " & strInput & ".")

EndIf

Dim oDoc

Set oDoc = CreateObject("MSXML.Document")

oDoc.async = False

oDoc.loadXML Window.XML

'Loop the elements <Label> in document ...

Set oLabels = oDoc.getElementsByTagName("Weight")

ForEach Label In oLabels

If Label.Text <> "0" Then

Application.Logtext "Change the Weight" & Label.Text & strInput

Label.Text = strInput

End If

Next

'Apply the modification to the window

Window.XML = oDoc.XML

End Function