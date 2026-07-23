[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXLayoutViewers Interface Reference

A Collection is an ordered set of items that can be referred to as a unit. More...

[List of all members.](interfaceWintax4Lib_1_1IWTXLayoutViewers-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Item](interfaceWintax4Lib_1_1IWTXLayoutViewers.md#a1) ([in] VARIANT index,[out, retval] IDispatch **ppItem)  
HRESULT | [Count](interfaceWintax4Lib_1_1IWTXLayoutViewers.md#a2) ([out, retval] long *pVal)  
|  | Returns the number of Layout Viewers available in the collection. |
  
* * *

## Detailed Description

A Collection is an ordered set of items that can be referred to as a unit. 

The [IWTXLayouts](interfaceIWTXLayouts.md) interface is a collection of objects that reference to a single WinTAX windows layout. 

* * *

## Member Function Documentation

|  | HRESULT Item | ( | [in] VARIANT | _index_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IDispatch ** | _ppItem_ |
|  | ) |  |
|  | Allows access to a single Layout Viewer of the collection. |

**Parameters:**
|  | _index_ | Index of the Layout Viewer in the collection. |
| --- | --- |
| _ppItem_ | Pointer to the Layout Viewer interface. |
  
**Remarks:**
     Use this method to access a single Layout Viewer. Depending on the language used, it is possible to access the collection in easier ways (eg. calling **LayoutViewers(1)** in VBScript hides the call to the [IWTXLayoutViewers::Item()](interfaceWintax4Lib_1_1IWTXLayoutViewers.md#a1) method) 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used Refer to [Sample: Layouts](Script_Change_Layout.md) for (eg. a **For Each..Next** loop in VB example 