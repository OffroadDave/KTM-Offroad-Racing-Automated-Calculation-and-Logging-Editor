[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXUser Interface Reference

The IWTXUser interface exposes properties of a user configured in WinTAX. More...

[List of all members.](interfaceIWTXUser-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Name](interfaceIWTXUser.md#a0) ([out, retval] BSTR *pVal)  
| HRESULT | [Level](interfaceIWTXUser.md#a2) ([out, retval] VARIANT *pVal) |
  
* * *

## Detailed Description

The IWTXUser interface exposes properties of a user configured in WinTAX. 

* * *

## Member Function Documentation

|  | HRESULT Level | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the level of user [Read / Write] |

**Note:**
     Could be one of the following values \- 0: Beginner user \- 1: Normal user \- 2: Super user   
| --- | --- |
  
|  | HRESULT Name | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the name of user [Read / Write] |

**Note:**
      
---|---