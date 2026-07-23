[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXLayouts Interface Reference

A Collection is an ordered set of items that can be referred to as a unit. More...

[List of all members.](interfaceIWTXLayouts-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Item](interfaceIWTXLayouts.md#a1) ([in] VARIANT index,[out, retval] IDispatch **ppItem)  
HRESULT | [Count](interfaceIWTXLayouts.md#a2) ([out, retval] long *pVal)  
| Returns the number of Layouts available in the collection.   
HRESULT | [Remove](interfaceIWTXLayouts.md#a3) ([in] VARIANT Key)  
| HRESULT | [Add](interfaceIWTXLayouts.md#a4) ([in] BSTR Name,[out, retval] IUnknown **pVal) |
  
* * *

## Detailed Description

A Collection is an ordered set of items that can be referred to as a unit. 

The IWTXLayouts interface is a collection of objects that reference to a single WinTAX windows layout. 

* * *

## Member Function Documentation

|  | HRESULT Add | ( | [in] BSTR | _Name_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IUnknown ** | _pVal_ |
|  | ) |  |
|  | Add a layout in the layout configuration |

**Note:**
      
| --- | --- |
  
|  | HRESULT Item | ( | [in] VARIANT | _index_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IDispatch ** | _ppItem_ |
|  | ) |  |
|  | Allows access to a single Layout of the collection. |

**Parameters:**
|  | _index_ | Index of the Layout in the collection. |
| --- | --- |
| _ppItem_ | Pointer to the Layout interface. |
  
**Remarks:**
     Use this method to access a single Layout. Depending on the language used, it is possible to access the collection in easier ways (eg. calling **Layouts(1)** in VBScript hides the call to the [IWTXLayouts::Item()](interfaceIWTXLayouts.md#a1) method) 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used Refer to [Sample: Layouts](Script_Change_Layout.md) for (eg. a **For Each..Next** loop in VB example   
  
|  | HRESULT Remove | ( | [in] VARIANT | _Key_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Removes a layout from the layout configuration |

**Note:**
      
---|---