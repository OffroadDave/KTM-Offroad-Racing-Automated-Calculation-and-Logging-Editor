[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ICollectWindows Interface Reference

A Collection is an ordered set of items that can be referred to as a unit. More...

[List of all members.](interfaceICollectWindows-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Item](interfaceICollectWindows.md#a1) ([in] VARIANT index,[out, retval] IDispatch **ppItem)  
HRESULT | [Count](interfaceICollectWindows.md#a2) ([out, retval] long *pVal)  
| HRESULT | [Add](interfaceICollectWindows.md#a3) ([in] BSTR name,[out, retval] IDispatch **ppItem) |
  
* * *

## Detailed Description

A Collection is an ordered set of items that can be referred to as a unit. 

The ICollectWindows interface is a collection of windows. 

* * *

## Member Function Documentation

|  | HRESULT Add | ( | [in] BSTR | _name_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IDispatch ** | _ppItem_ |
|  | ) |  |
|  | Adds a new analysis window to the WinTAX frame. |

**Parameters:**
|  | _name_ | Full path name of an existing configuration (eg."c:\Wintax4\users\John\Engine.gra"). |
| --- | --- |
  
**Remarks:**
     The type of window opened depends on the extension of the file passed. The file must be in current user directory.   
  
|  | HRESULT Count | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of windows available in the collection. |

**Remarks:**
     Use this property to get the number of windows opened in WinTAX frame   
| --- | --- |
  
|  | HRESULT Item | ( | [in] VARIANT | _index_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IDispatch ** | _ppItem_ |
|  | ) |  |
|  | Allows accessing a single Window of the collection. |

**Parameters:**
|  | _index_ | Index of the Window in the collection. |
| --- | --- |
| _ppItem_ | Pointer to the Window interface. |
  
**Remarks:**
     Use this method to access a single Window. Depending on the language used, it is possible to access the collection in easier ways (eg. calling **Windows(1)** in VBScript hides the call to the [ICollectWindows::Item()](interfaceICollectWindows.md#a1) method) 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used (eg. a **For Each..Next** loop in VB as follows) 
    
    
    Dim oFilePath
    Set oFilePath = Application.CurrentData(0).Properties( "FilePath" )
    
    Dim oWindow
    For Each oWindow In Application.Windows
      oWindow.Caption = ">> " & oFilePath.Data
    Next
    