[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelSessions Interface Reference

The ITelSessions interface is a collection of session objects. A Session is logically similar to an item of the left side list of the Data Browser. More...

Inherited by [IWTXSessions](interfaceIWTXSessions.md). 

[List of all members.](interfaceITelSessions-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Item](interfaceITelSessions.md#a1) ([in] VARIANT index,[out, retval] IDispatch **ppItem)  
| HRESULT | [Count](interfaceITelSessions.md#a2) ([out, retval] long *pVal) |
  
* * *

## Detailed Description

The ITelSessions interface is a collection of session objects. A Session is logically similar to an item of the left side list of the Data Browser. 

Each Session can contain a list of Laps. A Collection is an ordered set of items that can be referred to as a unit. 

* * *

## Member Function Documentation

|  | HRESULT Count | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of sessions available in the collection. |

**Remarks:**
     Use this property to get the number of sessions available.   
| --- | --- |
  
|  | HRESULT Item | ( | [in] VARIANT | _index_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IDispatch ** | _ppItem_ |
|  | ) |  |
|  | Allows accessing a single session of the collection. |

**Parameters:**
|  | _index_ | Index of the session in the collection. |
| --- | --- |
| _ppItem_ | Pointer to the [ITelSession](interfaceITelSession.md) interface. |
  
**Remarks:**
     Use this method to access a single session. Depending on the language used, it is possible to access the collection in easier ways 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used (eg. a **For Each..Next** loop in VBscript) 