### Read and Write data from Excel

Sub Excel()

Dim oBooks, oBook, oExcel

Dim oWorksheet

Dim oWorksheets

Dim FirstSheet

Dim oCells, FirstCell, FirstCellMath

Dim i

i = InputBox ("Insert Number")

'create excel object

Set oExcel = CreateObject("Excel.Application")

oExcel.Visible = True

Set oBooks = oExcel.WorkBooks

oBooks.create("C:\TestRW.xls")

Set oWorksheets = oExcel.Worksheets

Set oWorksheet = oExcel.Sheets

Set FirstSheet = oWorksheet(1)

FirstSheet.Name = "Test Read_Write"

Set oCells = oExcel.Cells

Set FirstCell = oCells(1,1)

FirstCell.Value = i

Set FirstCellMath = oCells(1,2)

Dim myMsgBox

myMsgBox = MsgBox (FirstCellMath.Value, 48)

Set oExcel = Nothing

End Sub