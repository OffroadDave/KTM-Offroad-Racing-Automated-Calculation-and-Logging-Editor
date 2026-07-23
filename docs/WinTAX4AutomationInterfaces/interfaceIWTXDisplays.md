[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXDisplays Interface Reference

[List of all members.](interfaceIWTXDisplays-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Item](interfaceIWTXDisplays.md#a1) ([in] VARIANT index,[out, retval] IDispatch **ppItem)  
HRESULT | [Count](interfaceIWTXDisplays.md#a2) ([out, retval] long *pVal)  
| Returns the number of Displays available in the collection.   
HRESULT | [Remove](interfaceIWTXDisplays.md#a3) ([in] VARIANT Key)  
| HRESULT | [Add](interfaceIWTXDisplays.md#a4) ([in] BSTR Name,[out, retval] IUnknown **pVal) |
  
* * *

## Detailed Description

The IWTXDisplays interface is a collection of objects that reference to a single WinTAX window. A Collection is an ordered set of items that can be referred to as a unit. 

* * *

## Member Function Documentation

|  | HRESULT Add | ( | [in] BSTR | _Name_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IUnknown ** | _pVal_ |
|  | ) |  |
|  | Add a display in layout configuration |

**Note:**
      
| --- | --- |
  
|  | HRESULT Item | ( | [in] VARIANT | _index_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IDispatch ** | _ppItem_ |
|  | ) |  |
|  | Allows access to a single Display of the collection. |

**Parameters:**
|  | _index_ | Index of the Display in the collection. |
| --- | --- |
| _ppItem_ | Pointer to the Display interface. |
  
**Remarks:**
     Use this method to access a single Display. Depending on the language used, it is possible to access the collection in easier ways (eg. calling **Displays(1)** in VBScript hides the call to the [IWTXDisplays::Item()](interfaceIWTXDisplays.md#a1) method) 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used Refer to [Sample: Layouts](Script_Change_Layout.md) for (eg. a **For Each..Next** loop in VB example   
  
|  | HRESULT Remove | ( | [in] VARIANT | _Key_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Removes a display from layout configuration |

**Note:**
      
---|---