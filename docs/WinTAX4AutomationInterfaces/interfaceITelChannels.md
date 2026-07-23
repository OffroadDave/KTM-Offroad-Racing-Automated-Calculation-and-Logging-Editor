[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelChannels Interface Reference

The ITelChannels interface is a collection of objects that reference to a single channel. More...

Inherited by [IWTXChannels](interfaceIWTXChannels.md). 

[List of all members.](interfaceITelChannels-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Item](interfaceITelChannels.md#a1) ([in] VARIANT index,[out, retval] IDispatch **ppItem)  
| HRESULT | [Count](interfaceITelChannels.md#a2) ([out, retval] long *pVal) |
  
* * *

## Detailed Description

The ITelChannels interface is a collection of objects that reference to a single channel. 

A Collection is an ordered set of items that can be referred to as a unit. 

* * *

## Member Function Documentation

|  | HRESULT Count | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of channels available in the collection. |

**Remarks:**
     Use this property to get the number of channels available.   
| --- | --- |
  
|  | HRESULT Item | ( | [in] VARIANT | _index_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IDispatch ** | _ppItem_ |
|  | ) |  |
|  | Allows accessing a single channel of the collection. |

**Parameters:**
|  | _index_ | Index of the channel in the collection. |
| --- | --- |
| _ppItem_ | Pointer to the [ITelChannel](interfaceITelChannel.md) interface. |
  
**Remarks:**
     Use this method to access a single channel. Depending on the language used, it is possible to access the collection in easier ways 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used (eg. a **For Each..Next** loop in VBscript) 