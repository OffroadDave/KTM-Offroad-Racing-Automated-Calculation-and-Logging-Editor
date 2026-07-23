[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXChannels Interface Reference

A Collection is an ordered set of items that can be referred to as a unit. More...

Inherits [ITelChannels](interfaceITelChannels.md). 

[List of all members.](interfaceIWTXChannels-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Add](interfaceIWTXChannels.md#a0) ([in] BSTR Name,[out, retval] IUnknown **pVal)  
HRESULT | [Remove](interfaceIWTXChannels.md#a1) ([in] VARIANT Key)  
HRESULT | [CreateOleChannelsFromCLX](interfaceIWTXChannels.md#a2) ([in, optional] VARIANT Val)  
HRESULT | [CreateOleChannelsFromTPX](interfaceIWTXChannels.md#a3) ([in, optional] VARIANT Val)  
HRESULT | [Add2](interfaceIWTXChannels.md#a4) ([in] BSTR Name,[in] VARIANT Val,[out, retval] IDispatch **pVal)  
| HRESULT | [RemoveAll](interfaceIWTXChannels.md#a5) ([in] VARIANT *pKey) |
  
* * *

## Detailed Description

A Collection is an ordered set of items that can be referred to as a unit. 

The IWTXChannels interface is a collection of channel objects. 

* * *

## Member Function Documentation

|  | HRESULT Add | ( | [in] BSTR | _Name_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] IUnknown ** | _pVal_ |
|  | ) |  |
|  | Creates a new OLE Channel whose samples can be changed in memory |

**Parameters:**
|  | _Name_ | [in] The name of the channel to be created. |
| --- | --- |
  
**Returns:**
     The interface of the new channel. 

**Note:**
     The channel is not created if another ole channel with the same name is found.   
  
|  | HRESULT Add2 | ( | [in] BSTR | _Name_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _Val_ , |
|  |  | [out, retval] IDispatch ** | _pVal_ |
|  | ) |  |
|  | Creates a new OLE Channel whose samples can be changed in memory |

**Parameters:**
|  | _Name_ | [in] The name of the channel to be created. |
| --- | --- |
| _Val_ | [in] Reserved for future implementation |
  
**Returns:**
     The interface of the new channel. 

**Note:**
     The channel is not created if another ole channel with the same name is found. 

**Remarks:**
     This method acts like the "Add" function, but returns an interface of IDispatch* type required by applications such as matlab   
  
|  | HRESULT CreateOleChannelsFromCLX | ( | [in, optional] VARIANT | _Val_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Create OLE channels from CLX channels loaded in wintax |

**Parameters:**
|  | _Val_ | [in, optional] Optional argument, the math expression used to fill ole channels values ( e.g. "5" ) |
| --- | --- |
  
**Note:**
     The method fails if the dataset is not created using method Application.Open with 'nothing' as parameter ( new empty dataset ) and if other OLE channels are already added in current dataset. 

**Remarks:**
     Refer to [Sample: Open data in WinTAX](application_open.md) for a list of available load operations.   
  
|  | HRESULT CreateOleChannelsFromTPX | ( | [in, optional] VARIANT | _Val_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Create OLE channels from TPX channels loaded in wintax |

**Parameters:**
|  | _Val_ | [in, optional] Optional argument, the math expression used to fill ole channels values ( e.g. "5" ) |
| --- | --- |
  
**Note:**
     The method fails if the dataset is not created using method Application.Open with 'nothing' as parameter ( new empty dataset ) and if other OLE channels are already added in current dataset. 

**Remarks:**
     Refer to [Sample: Open data in WinTAX](application_open.md) for a list of available load operations.   
  
|  | HRESULT Remove | ( | [in] VARIANT | _Key_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Removes an OLE Channel from memory |

**Note:**
     OLE Channels are automatically removed when the containing [IWTXDataLoad](interfaceIWTXDataLoad.md) is reset.   
| --- | --- |
  
|  | HRESULT RemoveAll | ( | [in] VARIANT * | _pKey_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Removes all channels based on the specified type |

**Parameters:**
|  | _pKey_ | [in] Type of channels to be removed. It can be a combination of the following values 0x01 OLE Channels 0x02 Virtual Channels |
| --- | --- |
  
**Note:**
     Virtual channels are only removed within the collection and continue to be present in the WinTAX environment. 