## OLE Automation

Setup Editor - or more precisely the working copies, groups and individual elements - are accessible from external applications via Automation. In fact, the WinTAX4 interfaces are extended to provide direct read and write access to context information and to the various working copies.

Context information can be read or edited via Automation only by means of the COM interfaces of the WinTAX4 component.

The following example demonstrates how to access this information.

Dim oWintax As Object

Set oWintax = CreateObject("WinTAX4.Application")

Dim spec As WTXSpecSheet

Set spec = oWintax.specsheet

Dim nWorking As Long

nWorking = spec.Count

Dim working As WTXSpecWorkingCopy

If (nWorking >= 1) Then Set working = oWintax.specsheet.WorkingCopy(0)

Dim nGroup As Long

nGroup = working.Count

Dim group As WTXSpecGroup

If (nGroup >= 1) Then Set group = working.group(0)

Dim item As WTXSpecItem

Set item = group.AddItem("test")

item.Value = 47

Set item = group.DataItem(0)

group.RemoveItem ("test")

group.RemoveItem (0)

##### Related Issues

  * [Setup Editor](Commands.md)

  * [Elements of the window](glossary.md)

  * [Commands](Setup_Editor.md)

  * [Setup Device](Setupcar.md)


