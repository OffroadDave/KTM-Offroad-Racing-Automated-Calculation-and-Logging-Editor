### Using the Windows Shell

Sub Test_Call_NOTEPAD

Dim WshShell, oExec

Set WshShell = CreateObject("WScript.Shell")

Set oExec = WshShell.Exec("notepad")

End Sub