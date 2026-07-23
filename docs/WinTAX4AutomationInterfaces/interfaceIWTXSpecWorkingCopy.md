[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXSpecWorkingCopy Interface Reference

The interface contains a collections of groups. More...

[List of all members.](interfaceIWTXSpecWorkingCopy-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Count](interfaceIWTXSpecWorkingCopy.md#a0) ([out, retval] LONG *pVal)  
HRESULT | [Name](interfaceIWTXSpecWorkingCopy.md#a1) ([out, retval] BSTR *pVal)  
HRESULT | [Group](interfaceIWTXSpecWorkingCopy.md#a3) (VARIANT Item,[out, retval] [IWTXSpecGroup](interfaceIWTXSpecGroup.md) **pVal)  
HRESULT | [RemoveGroup](interfaceIWTXSpecWorkingCopy.md#a4) (VARIANT Item)  
| HRESULT | [AddGroup](interfaceIWTXSpecWorkingCopy.md#a5) ([in] BSTR Name,[out, retval] IUnknown **pVal) |
  
* * *

## Detailed Description

The interface contains a collections of groups. 

The IWTXSpecWorkingCopy exposes methods and properties of Working Copy Objects. 

* * *

## Member Function Documentation

|  | HRESULT AddGroup | ( | [in] BSTR | _Name_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IUnknown ** | _pVal_ |
|  | ) |  |
|  | Create a new group |

**Parameters:**
|  | _Name_ | [in] The name of the group to be created. |
| --- | --- |
  
**Returns:**
     The interface of the new group. 

**Note:**
     The group is not created if a group with the same name is found.   
  
|  | HRESULT Count | ( | [out, retval] LONG * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of groups available in the collection. [Read] |

**Remarks:**
     Use this property to get the number of existing groups in a Working Copy 

**Note:**
     The property includes hidden groups   
| --- | --- |
  
|  | HRESULT Group | ( | VARIANT | _Item_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] [IWTXSpecGroup](interfaceIWTXSpecGroup.md) ** | _pVal_ |
|  | ) |  |
|  | Allows access to a single Group of the collection. [Read] |

**Parameters:**
|  | _Item_ | Index of the group in the collection. |
| --- | --- |
| _pVal_ | Pointer to the group interface. |
  
**Remarks:**
     Use this method to access a single Group. Depending on the language used, it is possible to access the collection in easier ways (eg. calling **Group(0)** or **Group("groupname")** in VBScript) 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used (eg. a **For Each..Next** loop in VB as follows)   
  
|  | HRESULT Name | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Specifies the name of the working copy. [Read / Write] |

**Note:**
      
| --- | --- |
  
|  | HRESULT RemoveGroup | ( | VARIANT | _Item_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Removes a group from the Working Copy |

**Parameters:**
|  | _Item_ | Index of the group to remove |
| --- | --- |
  
**Note:**
    