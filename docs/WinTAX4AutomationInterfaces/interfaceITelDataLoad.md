[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelDataLoad Interface Reference

The ITelDataLoad interface exposes methods and properties of a Data Load (Lap) object. More...

Inherited by [IWTXDataLoad](interfaceIWTXDataLoad.md). 

[List of all members.](interfaceITelDataLoad-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Time](interfaceITelDataLoad.md#a0) ([out, retval] double *pVal)  
HRESULT | [Channels](interfaceITelDataLoad.md#a2) ([out, retval] [ITelChannels](interfaceITelChannels.md) **pVal)  
HRESULT | [Properties](interfaceITelDataLoad.md#a3) ([out, retval] [ITelProperties](interfaceITelProperties.md) **pVal)  
HRESULT | [Open](interfaceITelDataLoad.md#a4) (VARIANT pVal)  
HRESULT | [Constants](interfaceITelDataLoad.md#a5) ([out, retval] [ITelProperties](interfaceITelProperties.md) **pVal)  
HRESULT | [UserRecords](interfaceITelDataLoad.md#a6) ([out, retval] [ITelProperties](interfaceITelProperties.md) **pVal)  
| HRESULT | [Save](interfaceITelDataLoad.md#a7) () |
  
* * *

## Detailed Description

The ITelDataLoad interface exposes methods and properties of a Data Load (Lap) object. 

A Data Load object represents a closed set of acquired data. It consists of a set of channels and properties. Compared to TelDataX interfaces, [IWTXDataLoad](interfaceIWTXDataLoad.md) is alike [ITelLap](interfaceITelLap.md) interface. 

**Remarks:**
     In some WinTAX distributions, this interface needs to be authorized before methods or properties can be used on a client application. See [ITelAuthorize::Authorize](interfaceITelAuthorize.md#a0) for more details. 

* * *

## Member Function Documentation

|  | HRESULT Channels | ( | [out, retval] [ITelChannels](interfaceITelChannels.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | [ITelChannels](interfaceITelChannels.md) collection that represents a set of channels. [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Constants | ( | [out, retval] [ITelProperties](interfaceITelProperties.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Constants is a [ITelProperties](interfaceITelProperties.md) collection that represents a set of dataload constants. [Read] |

**Note:**
     Refer to the page [Constants and UserRecords handling](sample_access_cst_urc.md) for a simple example on constant modification.   
| --- | --- |
  
|  | HRESULT Open | ( | VARIANT | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Open an existing archive |

**Parameters:**
|  | _pVal_ | [in] The complete file path or an array of the archives to load (eg. "\\\PC33\Data\Track\Car_05\Run_001\Lap_07\cabledata.ztx") |
| --- | --- |
  
|  | HRESULT Properties | ( | [out, retval] [ITelProperties](interfaceITelProperties.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | [ITelProperties](interfaceITelProperties.md) collection that represents a set of generic properties. [Read] |

**Note:**
     Refer to the page [Sample: Current Data Properties](sample_currentdataproperties.md) for further information on predefined properties. Also refer to the page [Sample: Current Data Properties](sample_currentdataproperties.md) for a simple example.   
| --- | --- |
  
|  | HRESULT Save | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | Save Constants and UserRecord |

**Note:**
     Method save constants and userrecord on context information xml files.   
| --- | --- |
  
|  | HRESULT Time | ( | [out, retval] double * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Logging time of Lap (or append), in seconds. [Read / Write] |

**Note:**
     Write operation is possible only if opened by WinTAX application as "empty". Refer to the page [Sample: Open data in WinTAX](application_open.md) for further information   
| --- | --- |
  
|  | HRESULT UserRecords | ( | [out, retval] [ITelProperties](interfaceITelProperties.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | UserRecord is a [ITelProperties](interfaceITelProperties.md) collection that represents a set of dataload userrecord. [Read] |

**Note:**
     Refer to the page [Constants and UserRecords handling](sample_access_cst_urc.md) for a simple example on userrecord modification.   
---|---