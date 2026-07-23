[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXSpecGroup Interface Reference

The interface contains a collections of Items. More...

[List of all members.](interfaceIWTXSpecGroup-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Count](interfaceIWTXSpecGroup.md#a0) ([out, retval] LONG *pVal)  
HRESULT | [Name](interfaceIWTXSpecGroup.md#a1) ([out, retval] BSTR *pVal)  
HRESULT | [DataItem](interfaceIWTXSpecGroup.md#a3) (VARIANT Item,[out, retval] [IWTXSpecItem](interfaceIWTXSpecItem.md) **pVal)  
HRESULT | [RemoveItem](interfaceIWTXSpecGroup.md#a4) ([in] VARIANT Item)  
| HRESULT | [AddItem](interfaceIWTXSpecGroup.md#a5) ([in] BSTR Name,[out, retval] IUnknown **pVal) |
  
* * *

## Detailed Description

The interface contains a collections of Items. 

The IWTXSpecGroup exposes methods and properties of Group Objects 

* * *

## Member Function Documentation

|  | HRESULT AddItem | ( | [in] BSTR | _Name_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IUnknown ** | _pVal_ |
|  | ) |  |
|  | Creates a new Item |

**Parameters:**
|  | _Name_ | [in] The name of the item to be created. |
| --- | --- |
  
**Returns:**
     The interface of the new item. 

**Note:**
     The Item is not created if a group with the same name is found.   
  
|  | HRESULT Count | ( | [out, retval] LONG * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of items available in the collection. [Read] |

**Remarks:**
     Use this property to get the number of existing items in a group 

**Note:**
      
| --- | --- |
  
|  | HRESULT DataItem | ( | VARIANT | _Item_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] [IWTXSpecItem](interfaceIWTXSpecItem.md) ** | _pVal_ |
|  | ) |  |
|  | Allows access to a single Item of the collection. [Read] |

**Parameters:**
|  | _Item_ | Index of the item in the collection. |
| --- | --- |
| _pVal_ | Pointer to the group interface. |
  
**Remarks:**
     Use this method to access a single Item. Depending on the language used, it is possible to access the collection in easier ways (eg. calling **DataItem(0)** or **DataItem("itemname")** in VBScript) 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used (eg. a **For Each..Next** loop in VB as follows)   
  
|  | HRESULT Name | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Specifies the name of the group. [Read / Write] |

**Note:**
      
| --- | --- |
  
|  | HRESULT RemoveItem | ( | [in] VARIANT | _Item_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Removes an Item from the group |

**Parameters:**
|  | _Item_ | Index of the Item to remove |
| --- | --- |
  
**Note:**
    