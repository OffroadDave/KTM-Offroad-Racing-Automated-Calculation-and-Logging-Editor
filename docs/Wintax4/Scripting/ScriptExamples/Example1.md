### Generate an Excel report

This example creates an Excel report triggered by the loading data event

Function Application_OnLoadData(bsFileName)

Report_Excel(bsFileName)

End Function

Sub Report_Excel(bsFileName)

Set oExcel = CreateObject("Excel.Application")

oExcel.Visible = True

Set oBooks = oExcel.WorkBooks

oBooks.create("C:\Test.xls")

Set oWorksheets = oExcel.Worksheets

Set oWorksheet = oExcel.Sheets

Set FirstSheet = oWorksheet(1)

FirstSheet.Name = "Report From WTX4"

Set oCells = oExcel.Cells

Dim oChanA, oChanB, oChanC

Set oData = Application.CurrentData(0)

Set oChanA = oData.Channels("ABS")

Set oChanB = oData.Channels("LAP")

Set oChanC = oData.Channels("RPM")

oCells(2,1).value = oChanA.Value(0)

oCells(2,2).value = oChanB.Value(0)

oCells(2,3).value = oChanC.Value(0)

Set oExcel = Nothing

End Sub