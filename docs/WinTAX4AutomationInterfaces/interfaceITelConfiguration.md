[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelConfiguration Interface Reference

The ITelConfiguration interface exposes methods and properties of a WinTAX4 configuration object. More...

Inherited by [IWTXConfiguration](interfaceIWTXConfiguration.md). 

[List of all members.](interfaceITelConfiguration-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Load](interfaceITelConfiguration.md#a0) ([in, optional] BSTR bsVal)  
HRESULT | [Save](interfaceITelConfiguration.md#a1) ([in, optional] BSTR bsVal)  
HRESULT | [Objects](interfaceITelConfiguration.md#a2) ([out, retval] ITelXMLObjects **pVal)  
| HRESULT | [Name](interfaceITelConfiguration.md#a3) ([out, retval] BSTR *pVal) |
  
* * *

## Detailed Description

The ITelConfiguration interface exposes methods and properties of a WinTAX4 configuration object. 

The configuration object represents a single setup for a particular environment (Parameters, Users, Virtual Channels, etc). The Configuration object contains methods and objects that allow loading, saving and accessing the XML formatted content. 

* * *

## Member Function Documentation

|  | HRESULT Load | ( | [in, optional] BSTR | _bsVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Loads a configuration |

**Parameters:**
|  | _bsVal_ | A string value of the configuration file path. |
| --- | --- |
  
|  | HRESULT Name | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Specifies the name ( file path ) of the configuration. [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Objects | ( | [out, retval] ITelXMLObjects ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | ITelXMLObjects collection that represents the configuration sub-objects. [Read] |

**Note:**
     Refer to the page [Sample: Access to Configuration objects](sample_access_configuration.md) for a simple example on configuration access.   
| --- | --- |
  
|  | HRESULT Save | ( | [in, optional] BSTR | _bsVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Saves a configuration |

**Parameters:**
|  | _bsVal_ | A string value of the configuration file path. |
| --- | --- |