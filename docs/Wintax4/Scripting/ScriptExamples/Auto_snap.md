### Automatic cursor snap

Function Window_OnCursorPosChange(TimePos)

Dim strMsg 'As String

Dim strInput 'As String

'Initialise the string

strMsg = "Number outside range. A number lower than 0 Or greater than 3 was entered. Press OK To enter the number again. Press Cancel To Exit"

'Prompt user for input..

strInput = InputBox("Enter The Round Desired.")

'check if the user selects "Cancel"

If strInput <> "" Then

'Test value of input

Do While strInput < 0 Or strInput > 3

If MsgBox(strMsg, vbOKCancel, "Error!") = vbOK Then

strInput = InputBox("Enter a number between 0 and 3.")

Else

Exit Function

End If

Loop

'Display user input

MsgBox "The Round Selected is " & strInput & "."

End If

Window.Cursor = Round (TimePos, strInput)

End Function