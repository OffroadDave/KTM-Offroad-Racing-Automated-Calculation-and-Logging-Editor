[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelSession Interface Reference

The ITelSession interface represents a Telemetry session. More...

Inherited by [IWTXSession](interfaceIWTXSession.md). 

[List of all members.](interfaceITelSession-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Laps](interfaceITelSession.md#a0) ([out, retval] [ITelLaps](interfaceITelLaps.md) **pVal)  
| HRESULT | [Properties](interfaceITelSession.md#a1) ([out, retval] [ITelProperties](interfaceITelProperties.md) **pVal) |
  
* * *

## Detailed Description

The ITelSession interface represents a Telemetry session. 

* * *

## Member Function Documentation

|  | HRESULT Laps | ( | [out, retval] [ITelLaps](interfaceITelLaps.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | [ITelLaps](interfaceITelLaps.md) collection that represents the set of laps in the session. [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Properties | ( | [out, retval] [ITelProperties](interfaceITelProperties.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | [ITelProperties](interfaceITelProperties.md) collection that represents a set of generic properties associated with the session. [Read] |

**Note:**
      
---|---