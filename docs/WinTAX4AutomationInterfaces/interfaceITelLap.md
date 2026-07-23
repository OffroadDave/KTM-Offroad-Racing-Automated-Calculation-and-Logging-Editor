[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelLap Interface Reference

The ITelLap interface represents a Telemetry lap. More...

Inherited by [IWTXLap](interfaceIWTXLap.md). 

[List of all members.](interfaceITelLap-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [FilePath](interfaceITelLap.md#a0) ([out, retval] BSTR *pVal)  
HRESULT | [LapIndex](interfaceITelLap.md#a2) ([out, retval] int *pVal)  
| HRESULT | [Properties](interfaceITelLap.md#a4) ([out, retval] [ITelProperties](interfaceITelProperties.md) **pVal) |
  
* * *

## Detailed Description

The ITelLap interface represents a Telemetry lap. 

* * *

## Member Function Documentation

|  | HRESULT FilePath | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Path of the file that contains the lap. |

**Note:**
      
| --- | --- |
  
|  | HRESULT LapIndex | ( | [out, retval] int * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Index of the lap in the archive. |

**Note:**
     Can be used in DTX archives, when a single DTX file contains more laps. In ZTX archives it is always set to 0.   
| --- | --- |
  
|  | HRESULT Properties | ( | [out, retval] [ITelProperties](interfaceITelProperties.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | [ITelProperty](interfaceITelProperty.md) collection that represents a set of generic properties associated with the lap. [Read] |

**Note:**
      
---|---