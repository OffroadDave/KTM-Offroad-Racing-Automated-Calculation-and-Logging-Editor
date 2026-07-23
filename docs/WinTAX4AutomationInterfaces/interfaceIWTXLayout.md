[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXLayout Interface Reference

The IWTXLayout interface exposes methods and properties of a WinTAX Window Layout. More...

[List of all members.](interfaceIWTXLayout-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Name](interfaceIWTXLayout.md#a0) ([out, retval] BSTR *pVal)  
| HRESULT | [Displays](interfaceIWTXLayout.md#a2) ([out, retval] [IWTXDisplays](interfaceIWTXDisplays.md) **pVal) |
  
* * *

## Detailed Description

The IWTXLayout interface exposes methods and properties of a WinTAX Window Layout. 

* * *

## Member Function Documentation

|  | HRESULT Displays | ( | [out, retval] [IWTXDisplays](interfaceIWTXDisplays.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the collection of displays configured in the layout. [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Name | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Specifies the name of the Layout ( file name ). [Read / Write] |

**Note:**
      
---|---