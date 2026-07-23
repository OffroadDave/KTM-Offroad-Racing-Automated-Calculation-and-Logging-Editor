[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelConfigurations Interface Reference

The ITelConfigurations interface is a collection of generic [ITelConfiguration](interfaceITelConfiguration.md) objects, used to reference multiple libraries of configurations such as Math Libraries. More...

Inherited by [IWTXConfigurations](interfaceIWTXConfigurations.md). 

[List of all members.](interfaceITelConfigurations-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Item](interfaceITelConfigurations.md#a1) ([in] VARIANT index,[out, retval] IDispatch **ppItem)  
| HRESULT | [Count](interfaceITelConfigurations.md#a2) ([out, retval] long *pVal) |
  
* * *

## Detailed Description

The ITelConfigurations interface is a collection of generic [ITelConfiguration](interfaceITelConfiguration.md) objects, used to reference multiple libraries of configurations such as Math Libraries. 

A Collection is an ordered set of items that can be referred to as a unit. 

* * *

## Member Function Documentation

|  | HRESULT Count | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of configurations available in the collection. |

**Remarks:**
     Use this property to get the number of configurations available.   
| --- | --- |
  
|  | HRESULT Item | ( | [in] VARIANT | _index_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IDispatch ** | _ppItem_ |
|  | ) |  |
|  | Allows accessing a single configuration of the collection. |

**Parameters:**
|  | _index_ | Index of the configuration in the collection. |
| --- | --- |
| _ppItem_ | Pointer to the [ITelConfiguration](interfaceITelConfiguration.md) interface. |
  
**Remarks:**
     Use this method to access a single configuration. Depending on the language used, it is possible to access the collection in easier ways 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used (eg. a **For Each..Next** loop in VBscript) 