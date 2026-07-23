[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelProperties Interface Reference

The ITelProperties interface is a collection of property objects. More...

Inherited by IWTXProperties. 

[List of all members.](interfaceITelProperties-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Item](interfaceITelProperties.md#a1) ([in] VARIANT index,[out, retval] IDispatch **ppItem)  
HRESULT | [Count](interfaceITelProperties.md#a2) ([out, retval] long *pVal)  
HRESULT | [Add](interfaceITelProperties.md#a3) ([in] BSTR Name,[out, retval] IUnknown **pVal)  
HRESULT | [Remove](interfaceITelProperties.md#a4) ([in] VARIANT Item)  
|  | Remove a property from the collection. |
  
* * *

## Detailed Description

The ITelProperties interface is a collection of property objects. 

A Collection is an ordered set of items that can be referred to as a unit. 

* * *

## Member Function Documentation

|  | HRESULT Add | ( | [in] BSTR | _Name_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IUnknown ** | _pVal_ |
|  | ) |  |
|  | Add a new property in the collection |

**Parameters:**
|  | _Name_ | of the property in the collection. |
| --- | --- |
  
**Remarks:**
     If the property that is being inserted already exists in the collection, the method returns it.   
  
|  | HRESULT Count | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of properties available in the collection. |

**Remarks:**
     Use this property to get the number of properties available.   
| --- | --- |
  
|  | HRESULT Item | ( | [in] VARIANT | _index_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IDispatch ** | _ppItem_ |
|  | ) |  |
|  | Allows accessing a single property of the collection. |

**Parameters:**
|  | _index_ | Index or name of the property in the collection. |
| --- | --- |
| _ppItem_ | Pointer to the property interface. |
  
**Remarks:**
     Use this method to access a single property. Depending on the language used, it is possible to access the collection in easier ways (eg. calling **oData.Properties(0)** or **oData.Properties("FilePath")** in VBScript hide the call to the [ITelProperties::Item()](interfaceITelProperties.md#a1) method) 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used (eg. a **For Each..Next** loop in VBscript) 