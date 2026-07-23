[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITextItems Interface Reference

The [ITextItems](interfaceWintax4Lib_1_1ITextItems.md) interface allows access to single cell items inside alphanumeric windows. More...

[List of all members.](interfaceWintax4Lib_1_1ITextItems-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [CellValue](interfaceWintax4Lib_1_1ITextItems.md#a0) ([in] long lRow,[in] long lCol,[out, retval] VARIANT *pVal)  
HRESULT | [NumRows](interfaceWintax4Lib_1_1ITextItems.md#a1) ([out, retval] long *pVal)  
| HRESULT | [NumColumns](interfaceWintax4Lib_1_1ITextItems.md#a2) ([out, retval] long *pVal) |
  
* * *

## Detailed Description

The [ITextItems](interfaceWintax4Lib_1_1ITextItems.md) interface allows access to single cell items inside alphanumeric windows. 

Use this interface to extract cell values and texts. Example: 
    
    
    Dim rowText 
    For iRow = 0 To window.TextItems.NumRows-1 Step 1 
    
      rowText = "Row" & iRow & ": " 
    
      For iCol = 0 To window.TextItems.NumColumns-1 Step 1 
        rowText = rowText & " " & window.TextItems.CellValue( iRow, iCol ) 
      Next 
    
      Application.LogText rowText 
    
    Next 
    
    

* * *

## Member Function Documentation

|  | HRESULT CellValue | ( | [in] long | _lRow_ , |
| --- | --- | --- | --- |
|  |  | [in] long | _lCol_ , |
|  |  | [out, retval] VARIANT * | _pVal_ |
|  | ) |  |
|  | Returns the value corresponding to a cell in an alphanumeric window of WinTAX. |

**Parameters:**
|  | _lRow_ | [in] Row position of the cell (0 based index) |
| --- | --- |
| _lCol_ | [in] Column position of the cell (0 based index) |
  
**Returns:**
     The text corresponding to the selected cell. If lRow or lCol refer to an invalid cell, or if the window does not support cell access, the return value is an empty string. 

**Remarks:**
     Only some alphanumeric windows return CellValues: when a window is not enabled for exporting text elements, it returns 0 both in NumRows and NumColumns properties.   
  
|  | HRESULT NumColumns | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of columns available in the associated window. [Read] |

**Note:**
     The number of columns available in the associated window. This count includes also header or information cells. If the window is not alphanumeric, the return value is always 0.   
| --- | --- |
  
|  | HRESULT NumRows | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of rows available in the associated window. [Read] |

**Note:**
     The number of rows available in the associated window. This count includes also header or information cells. If the window is not alphanumeric, the return value is always 0.   
---|---