[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelDataLoads Interface Reference

The ITelDataLoads interface is a collection of data loads, that is objects that reference to a single data selection (single lap, laps appended, etc). More...

Inherited by [IWTXDataLoads](interfaceIWTXDataLoads.md). 

[List of all members.](interfaceITelDataLoads-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Item](interfaceITelDataLoads.md#a1) ([in] VARIANT index,[out, retval] IDispatch **ppItem)  
| HRESULT | [Count](interfaceITelDataLoads.md#a2) ([out, retval] long *pVal) |
  
* * *

## Detailed Description

The ITelDataLoads interface is a collection of data loads, that is objects that reference to a single data selection (single lap, laps appended, etc). 

A Collection is an ordered set of items that can be referred to as a unit. 

* * *

## Member Function Documentation

|  | HRESULT Count | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of data loads available in the collection. |

**Remarks:**
     Use this property to get the number of data loads opened in current selection. The number of data loads is equivalent to the number of comparisons used: Single Lap Selection = 1 data load available. 3 Laps Appended in a Selection = 1 data load available. Comparison of 3 Laps = 3 data loads available.   
| --- | --- |
  
|  | HRESULT Item | ( | [in] VARIANT | _index_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IDispatch ** | _ppItem_ |
|  | ) |  |
|  | Allows accessing a single DataLoad of the collection. |

**Parameters:**
|  | _index_ | Index of the DataLoad in the collection. |
| --- | --- |
| _ppItem_ | Pointer to the DataLoad interface. |
  
**Remarks:**
     Use this method to access a single DataLoad. Depending on the language used, it is possible to access the collection in easier ways (eg. calling **Application.CurrentData(1)** in VBScript hides the call to the [ITelDataLoads::Item()](interfaceITelDataLoads.md#a1) method) 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used (eg. a **For Each..Next** loop in VB) 