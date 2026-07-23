[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXUsers Interface Reference

A Collection is an ordered set of items that can be referred to as a unit. More...

[List of all members.](interfaceIWTXUsers-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Item](interfaceIWTXUsers.md#a1) ([in] VARIANT index,[out, retval] IDispatch **ppItem)  
HRESULT | [Count](interfaceIWTXUsers.md#a2) ([out, retval] long *pVal)  
HRESULT | [Add](interfaceIWTXUsers.md#a3) ([in] BSTR name,[out, retval] IUnknown **ppItem)  
| HRESULT | [Remove](interfaceIWTXUsers.md#a4) ([in] VARIANT Key) |
  
* * *

## Detailed Description

A Collection is an ordered set of items that can be referred to as a unit. 

The IWTXUsers interface is a collection of objects that reference to a WinTAX user. 

* * *

## Member Function Documentation

|  | HRESULT Add | ( | [in] BSTR | _name_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IUnknown ** | _ppItem_ |
|  | ) |  |
|  | Adds a new user to the WinTAX configuration. |

**Parameters:**
|  | _name_ | name of the user |
| --- | --- |
  
|  | HRESULT Count | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of users available in the collection. |

**Note:**
      
| --- | --- |
  
|  | HRESULT Item | ( | [in] VARIANT | _index_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IDispatch ** | _ppItem_ |
|  | ) |  |
|  | Allows access to a single User configuration of the collection. |

**Parameters:**
|  | _index_ | Index of the user in the collection. |
| --- | --- |
| _ppItem_ | Pointer to the user interface. |
  
**Remarks:**
     Use this method to access a single User. 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used   
  
|  | HRESULT Remove | ( | [in] VARIANT | _Key_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Removes an user Channel from WinTAX configuration |

**Note:**
      
---|---