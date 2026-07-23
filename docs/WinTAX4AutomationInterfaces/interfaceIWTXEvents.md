[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXEvents Interface Reference

A Collection is an ordered set of items that can be referred to as a unit. More...

[List of all members.](interfaceIWTXEvents-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Item](interfaceIWTXEvents.md#a1) ([in] VARIANT index,[out, retval] IDispatch **ppItem)  
HRESULT | [Count](interfaceIWTXEvents.md#a2) ([out, retval] long *pVal)  
|  | Returns the number of Events available in the collection. |
  
* * *

## Detailed Description

A Collection is an ordered set of items that can be referred to as a unit. 

The IWTXEvents interface is a collection of objects that reference to a single configured Event. 

* * *

## Member Function Documentation

|  | HRESULT Item | ( | [in] VARIANT | _index_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IDispatch ** | _ppItem_ |
|  | ) |  |
|  | Allows access to a single Event of the collection. |

**Parameters:**
|  | _index_ | Index of the Event in the collection. |
| --- | --- |
| _ppItem_ | Pointer to the Event interface. |
  
**Remarks:**
     Use this method to access a single Event. Depending on the language used, it is possible to access the collection in easier ways (eg. calling **Events(1)** in VBScript hides the call to the [IWTXEvents::Item()](interfaceIWTXEvents.md#a1) method) 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used Refer to [Sample: Events](Script_ForEach_Event.md) for (eg. a **For Each..Next** loop in VB example 