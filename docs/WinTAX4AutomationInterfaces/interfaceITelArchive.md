[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelArchive Interface Reference

The ITelArchive interface represents a Telemetry data archive. More...

Inherited by [IWTXArchive](interfaceIWTXArchive.md). 

[List of all members.](interfaceITelArchive-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Open](interfaceITelArchive.md#a0) (BSTR bsPath, VARIANT vOpt)  
HRESULT | [Properties](interfaceITelArchive.md#a1) ([out, retval] [ITelProperties](interfaceITelProperties.md) **pVal)  
| HRESULT | [Sessions](interfaceITelArchive.md#a2) ([out, retval] [ITelSessions](interfaceITelSessions.md) **pVal) |
  
* * *

## Detailed Description

The ITelArchive interface represents a Telemetry data archive. 

**Remarks:**
     In some WinTAX distributions, this interface needs to be authorized before using methods or properties on a client application. See [ITelAuthorize::Authorize](interfaceITelAuthorize.md#a0) for more details.

* * *

## Member Function Documentation

|  | HRESULT Open | ( | BSTR | _bsPath_ , |
| --- | --- | --- | --- |
|  |  | VARIANT | _vOpt_ |
|  | ) |  |
|  | Opens an archive, allowing to access laps and channels through a hierarchy of internal objects (laps, channels). |

**Parameters:**
|  | _bsPath_ | Path to an archive folder used for |
| --- | --- |
| _vOpt_ | Optional flag for future implementation. Not yet used. |
  
**Remarks:**
      
  
|  | HRESULT Properties | ( | [out, retval] [ITelProperties](interfaceITelProperties.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | [ITelProperty](interfaceITelProperty.md) collection that represents a set of generic properties. [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Sessions | ( | [out, retval] [ITelSessions](interfaceITelSessions.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | [ITelSessions](interfaceITelSessions.md) collection that represents a set of sessions. [Read] |

**Remarks:**
     After a [ITelArchive::Open](interfaceITelArchive.md#a0) call, navigate through the Sessions collection to search for a specific session, lap, and channels.   
---|---