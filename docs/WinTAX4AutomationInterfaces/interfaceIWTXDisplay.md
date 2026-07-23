[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXDisplay Interface Reference

The IWTXDisplay interface exposes methods and properties of a single display configured in layout. More...

[List of all members.](interfaceIWTXDisplay-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Name](interfaceIWTXDisplay.md#a0) ([out, retval] BSTR *pVal)  
HRESULT | [Top](interfaceIWTXDisplay.md#a2) ([out, retval] VARIANT *pVal)  
HRESULT | [Left](interfaceIWTXDisplay.md#a4) ([out, retval] VARIANT *pVal)  
HRESULT | [Bottom](interfaceIWTXDisplay.md#a6) ([out, retval] VARIANT *pVal)  
| HRESULT | [Right](interfaceIWTXDisplay.md#a8) ([out, retval] VARIANT *pVal) |
  
* * *

## Detailed Description

The IWTXDisplay interface exposes methods and properties of a single display configured in layout. 

* * *

## Member Function Documentation

|  | HRESULT Bottom | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Specifies the Bottom y coordinate of the Display in WinTAX client area units( 0 - 10000 ). [Read / Write] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Left | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Specifies the Left x coordinate of the Display in WinTAX client area units( 0 - 10000 ). [Read / Write] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Name | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Specifies the name of the Display ( file name ). [Read / Write] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Right | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Specifies the Right x coordinate of the Display in WinTAX client area units( 0 - 10000 ). [Read / Write] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Top | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Specifies the Top y coordinate of the Display in WinTAX client area units( 0 - 10000 ). [Read / Write] |

**Note:**
      
---|---