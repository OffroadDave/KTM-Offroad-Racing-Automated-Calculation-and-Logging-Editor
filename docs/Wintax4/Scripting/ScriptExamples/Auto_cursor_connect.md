### Automatic cursor connect for all windows

Function Window_OnCursorPosChange(TimePos)

Dim oCursor

For Each oCursor In Application.Windows

oCursor.cursor = TimePos

Application.LogText(oCursor.cursor)

Next

End Function