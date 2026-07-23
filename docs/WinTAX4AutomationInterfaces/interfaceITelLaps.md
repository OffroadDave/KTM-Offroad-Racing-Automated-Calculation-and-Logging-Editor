[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelLaps Interface Reference

The ITelLaps interface is a collection of lap objects. A Lap is logically similar to an item of the right side list of the Data Browser window. More...

Inherited by [IWTXLaps](interfaceIWTXLaps.md). 

[List of all members.](interfaceITelLaps-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Item](interfaceITelLaps.md#a1) ([in] VARIANT index,[out, retval] IDispatch **ppItem)  
| HRESULT | [Count](interfaceITelLaps.md#a2) ([out, retval] long *pVal) |
  
* * *

## Detailed Description

The ITelLaps interface is a collection of lap objects. A Lap is logically similar to an item of the right side list of the Data Browser window. 

Each Lap can contain a list of properties. A Collection is an ordered set of items that can be referred to as a unit. 

* * *

## Member Function Documentation

|  | HRESULT Count | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of laps available in the collection. |

**Remarks:**
     Use this property to get the number of laps available.   
| --- | --- |
  
|  | HRESULT Item | ( | [in] VARIANT | _index_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IDispatch ** | _ppItem_ |
|  | ) |  |
|  | Allows accessing a single lap of the collection. |

**Parameters:**
|  | _index_ | Index of the lap in the collection. |
| --- | --- |
| _ppItem_ | Pointer to the [ITelLap](interfaceITelLap.md) interface. |
  
**Remarks:**
     Use this method to access a single lap. Depending on the language used, it is possible to access the collection in easier ways 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used (eg. a **For Each..Next** loop in VBscript) 