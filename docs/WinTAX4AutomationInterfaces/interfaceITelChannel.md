[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelChannel Interface Reference

The ITelChannel interface exposes methods and properties of a channel Object. More...

Inherited by [IWTXChannel](interfaceIWTXChannel.md). 

[List of all members.](interfaceITelChannel-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Name](interfaceITelChannel.md#a0) ([out, retval] BSTR *pVal)  
HRESULT | [Value](interfaceITelChannel.md#a2) ([in] VARIANT Time,[out, retval] double *pVal)  
HRESULT | [Values](interfaceITelChannel.md#a4) ([in] VARIANT TimeStart,[in] VARIANT TimeEnd,[out, retval] VARIANT *pVal)  
HRESULT | [Properties](interfaceITelChannel.md#a6) ([out, retval] [ITelProperties](interfaceITelProperties.md) **pVal)  
HRESULT | [Offset](interfaceITelChannel.md#a7) ([out, retval] double *pVal)  
HRESULT | [Gain](interfaceITelChannel.md#a9) ([out, retval] double *pVal)  
HRESULT | [SampleRate](interfaceITelChannel.md#a11) ([out, retval] long *pVal)  
HRESULT | [Type](interfaceITelChannel.md#a13) ([out, retval] long *pVal)  
| HRESULT | [TimeStartOffset](interfaceITelChannel.md#a14) ([out, retval] double *pVal) |
  
* * *

## Detailed Description

The ITelChannel interface exposes methods and properties of a channel Object. 

A Channel object contains information and data from a channel sampled by the acquisition system. 

* * *

## Member Function Documentation

|  | HRESULT Gain | ( | [out, retval] double * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the channel Gain correction value. [Read / Write] |

**Remarks:**
     This method works in Write mode only if the channel corresponds to a Data archive The Run/Lap replication of values is therefore hidden: only the priority values are exposed (that is, Run level gain if values are not stored at Lap level, or Lap level gain if values are stored both in Run and Lap level). When the client tool changes Offset & Gain values, the new values are always stored in the Lap level. For OLE channels and Virtual channels default value "1" is always returned.   
| --- | --- |
  
|  | HRESULT Name | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the channel name. [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Offset | ( | [out, retval] double * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the channel Offset correction value. [Read / Write] |

**Remarks:**
     This method works in Write mode only if the channel corresponds to a Data archive The Run/Lap replication of values is therefore hidden: only the priority values are exposed (that is, Run level offsets if values are not stored at Lap level, or Lap level offsets if values are stored both in Run and Lap level). When the client tool changes Offset & Gain values, the new values are always stored in the Lap level. This value is independent from the Offset value that can be set in the **Parameters** configuration. For OLE channels and Virtual channels default value "0" is always returned.   
| --- | --- |
  
|  | HRESULT Properties | ( | [out, retval] [ITelProperties](interfaceITelProperties.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | [ITelProperty](interfaceITelProperty.md) collection that represents a set of generic properties. [Read] |

**Note:**
    **[ CURRENTLY DISABLED ]**  
| --- | --- |
  
|  | HRESULT SampleRate | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The sample rate of the channel, in hertz. [Read /Write ] |

**Remarks:**
     SampleRate is always read-only unless the channel is created as an OLE Channel with the method ITelChannels::Create(). In this case SampleRate **must** be set in order to determine the number of samples stored in the channel memory. Refer to the [OLE Channels](ole_channels.md) page for further information.   
| --- | --- |
  
|  | HRESULT TimeStartOffset | ( | [out, retval] double * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The time start offset of first sample of the lap |

**Remarks:**
     TimeStartOffset is the time offset of the first sample with respect to beacon. See the picture below   
| --- | --- |
  
|  | HRESULT Type | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The Type of the channel [Read] |

**Note:**
     Channel Type can return the following values: 1 - Channel stored in ZTX/DTX archive. 2 - Virtual Channel. 3 - SlowRate Channel. 4 - Bitfield Channel. 5 - MathDLL Channel.   
| --- | --- |
  
|  | HRESULT Value | ( | [in] VARIANT | _Time_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] double * | _pVal_ |
|  | ) |  |
|  | Returns one single sampled value of the channel. [Read / Write] |

**Parameters:**
|  | _Time_ | [in] The sample time expressed in seconds. Time is relative to the object (lap or append) that contains current channel. |
| --- | --- |
  
**Remarks:**
     Value is always read-only unless the channel is created as an OLE Channel with the method ITelChannels::Create(). Refer to the [OLE Channels](ole_channels.md) page for further information.   
  
|  | HRESULT Values | ( | [in] VARIANT | _TimeStart_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _TimeEnd_ , |
|  |  | [out, retval] VARIANT * | _pVal_ |
|  | ) |  |
|  | Returns a set of sampled values of the channel, in a time range. [Read / Write] |

**Parameters:**
|  | _TimeStart_ | [in] The start time in seconds. |
| --- | --- |
| _TimeEnd_ | [in] The end time in seconds. |
  
**Remarks:**
     Values is always read-only unless the channel is created as an OLE Channel with the method ITelChannels::Create(). Refer to the [OLE Channels](ole_channels.md) page for further information. 

**Note:**
     The following example demonstrates a simple analysis based on the Values property: 
    
    
    Function Window_OnZoomChange(vStart, vEnd)
    
    Dim oData
    Dim oChan
    Dim vValues
    Dim iCount
    Dim vAvgValue
    Dim iNumValues
    
    ' Get the first comparison of current selected data
    Set oData = Application.CurrentData(0)
    
    Application.ClearLog()
    
    ' Calutate RPM average value of current zoom, and log to output window.
    Set oChan = oData.Channels( "RPM" )
    vValues = oChan.Values( vStart, vEnd )
    iNumValues = UBound( vValues ) 
    vAvgValue = 0
            
    For iCount= 0 To iNumValues
            vAvgValue = vAvgValue + vValues(iCount)
    Next 
            
    vAvgValue = vAvgValue / iNumValues
    
    Application.LogText "RPM average value is " & vAvgValue & "[" & iNumValues & " samples]"
    
    End Function
    
    