### Read and Write data from ASCII file

Sub TXT()

Const ForReading = 1

Const ForWriting = 2

Dim fso, f, ra

Set fso = CreateObject("Scripting.FileSystemObject")

'create an ASCII file...

Set f = fso.CreateTextFile("c:\testfile.txt", ForWriting, True)

f.Write "OK!!" & vbCrLf & "WinTAX4!" & vbCrLf

'read the content of the ASCII file

Set f = fso.OpenTextFile("c:\testfile.txt", ForReading)

ra = f.ReadAll

GetLine = f.Line

MsgBox ra

End Sub