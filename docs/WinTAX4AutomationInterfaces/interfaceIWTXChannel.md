[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXChannel Interface Reference

The IWTXChannel interface is directly derived from [ITelChannel](interfaceITelChannel.md). Refer to the [ITelChannel](interfaceITelChannel.md) interface documentation for a list of methods / properties. More...

Inherits [ITelChannel](interfaceITelChannel.md). 

[List of all members.](interfaceIWTXChannel-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [SetValuesMath](interfaceIWTXChannel.md#a0) ([in] VARIANT sExpression)  
HRESULT | [ParameterOffset](interfaceIWTXChannel.md#a1) ([out, retval] double *pVal)  
HRESULT | [ParameterGain](interfaceIWTXChannel.md#a3) ([out, retval] double *pVal)  
HRESULT | [ValuesAtSampleRate](interfaceIWTXChannel.md#a5) ([in] VARIANT TimeStart,[in] VARIANT TimeEnd,[in] long lSampleRate,[in, optional] VARIANT Mode,[out, retval] VARIANT *pVal)  
HRESULT | [SamplesCount](interfaceIWTXChannel.md#a6) ([in] VARIANT TimeStart,[in] VARIANT TimeEnd,[out, retval] long *pVal)  
HRESULT | [Min](interfaceIWTXChannel.md#a7) ([in] VARIANT TimeStart,[in] VARIANT TimeEnd,[out, retval] VARIANT *pVal)  
HRESULT | [Max](interfaceIWTXChannel.md#a8) ([in] VARIANT TimeStart,[in] VARIANT TimeEnd,[out, retval] VARIANT *pVal)  
HRESULT | [Avg](interfaceIWTXChannel.md#a9) ([in] VARIANT TimeStart,[in] VARIANT TimeEnd,[out, retval] VARIANT *pVal)  
HRESULT | [AvgAbs](interfaceIWTXChannel.md#a10) ([in] VARIANT TimeStart,[in] VARIANT TimeEnd,[out, retval] VARIANT *pVal)  
HRESULT | [StdDev](interfaceIWTXChannel.md#a11) ([in] VARIANT TimeStart,[in] VARIANT TimeEnd,[out, retval] VARIANT *pVal)  
HRESULT | [PutMatlabValues](interfaceIWTXChannel.md#a12) ([in] VARIANT TimeStart,[in] VARIANT TimeEnd,[in, optional] VARIANT VariableName,[in, optional] VARIANT Matlab)  
HRESULT | [GetMatlabValues](interfaceIWTXChannel.md#a13) ([in] VARIANT TimeStart,[in] VARIANT TimeEnd,[in, optional] VARIANT VariableName,[in, optional] VARIANT Matlab)  
HRESULT | [SetTimeStartOffset](interfaceIWTXChannel.md#a14) ([in] double Val)  
HRESULT | [PutMatlabStructureWithTime](interfaceIWTXChannel.md#a15) ([in, optional] VARIANT VariableName,[in, optional] VARIANT Matlab)  
HRESULT | [GetMatlabStructureWithTime](interfaceIWTXChannel.md#a16) ([in, optional] VARIANT VariableName,[in, optional] VARIANT Matlab)  
HRESULT | [Units](interfaceIWTXChannel.md#a17) ([out, retval] BSTR *pVal)  
| Returns the channel units. [Read].   
HRESULT | [Decimals](interfaceIWTXChannel.md#a18) ([out, retval] long *pVal)  
| Returns the number of decimals used to display values of a channel. [Read].   
HRESULT | [OutputFormat](interfaceIWTXChannel.md#a19) ([out, retval] long *pVal)  
| HRESULT | [SetMeasureUnit](interfaceIWTXChannel.md#a20) ([in] BSTR newVal) |
  
* * *

## Detailed Description

The IWTXChannel interface is directly derived from [ITelChannel](interfaceITelChannel.md). Refer to the [ITelChannel](interfaceITelChannel.md) interface documentation for a list of methods / properties. 

* * *

## Member Function Documentation

|  | HRESULT Avg | ( | [in] VARIANT | _TimeStart_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _TimeEnd_ , |
|  |  | [out, retval] VARIANT * | _pVal_ |
|  | ) |  |
|  | Returns the average values of the channel in the interval taken into account. [Read] |

**Parameters:**
|  | _TimeStart_ | [in] The start time in seconds. |
| --- | --- |
| _TimeEnd_ | [in] The end time in seconds. |
  
**Note:**
      
  
|  | HRESULT AvgAbs | ( | [in] VARIANT | _TimeStart_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _TimeEnd_ , |
|  |  | [out, retval] VARIANT * | _pVal_ |
|  | ) |  |
|  | Returns the average of the absolute values of the channel in the interval taken into account. [Read] |

**Parameters:**
|  | _TimeStart_ | [in] The start time in seconds. |
| --- | --- |
| _TimeEnd_ | [in] The end time in seconds. |
  
**Note:**
      
  
|  | HRESULT GetMatlabStructureWithTime | ( | [in, optional] VARIANT | _VariableName_ , |
| --- | --- | --- | --- |
|  |  | [in, optional] VARIANT | _Matlab_ |
|  | ) |  |
|  | Fills the samples of the OLE Channel with a Structure With Time defined in MatLab |

**Parameters:**
|  | _VariableName_ | [in, optional] A matlab variable name whose values are put in the channel samples. If empty, it's used the channel name. (see [ITelChannel::Name](interfaceITelChannel.md#a0) ) |
| --- | --- |
| _Matlab_ | [in, optional] A matlab interface used to retrieve the variable. If empty, it's used the WinTAX Matlab application used in export operation (see [IApplication::MatlabApplication](interfaceIApplication.md#a58) ) |
  
**Remarks:**
     This method only works with OLE Channels (see [IWTXChannels::Add](interfaceIWTXChannels.md#a0)). The SampleRate of the channel must be already set with a valid frequency. Refer to [OLE Channels](ole_channels.md) for further information and [Structure with time data type](matlab_integration.md#sample_structure_with_time) for a short data description of structure with times   
  
|  | HRESULT GetMatlabValues | ( | [in] VARIANT | _TimeStart_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _TimeEnd_ , |
|  |  | [in, optional] VARIANT | _VariableName_ , |
|  |  | [in, optional] VARIANT | _Matlab_ |
|  | ) |  |
|  | Fills the samples of the OLE Channel with a vector defined in MatLab |

**Parameters:**
|  | _TimeStart_ | [in] The start time in seconds. |
| --- | --- |
| _TimeEnd_ | [in] The end time in seconds. |
| _VariableName_ | [in, optional] A matlab variable name whose values are put in the channel samples. If empty, it's used the channel name. (see [ITelChannel::Name](interfaceITelChannel.md#a0) ) |
| _Matlab_ | [in, optional] A matlab interface used to retrieve the variable. If empty, it's used the WinTAX Matlab application used in export operation (see [IApplication::MatlabApplication](interfaceIApplication.md#a58) ) |
  
**Remarks:**
     This method only works with OLE Channels (see [IWTXChannels::Add](interfaceIWTXChannels.md#a0)). The SampleRate of the channel must be already set with a valid frequency. Refer to [OLE Channels](ole_channels.md) for further information and [Sample Code](matlab_integration.md#sample_matlab) for how to use the method.   
  
|  | HRESULT Max | ( | [in] VARIANT | _TimeStart_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _TimeEnd_ , |
|  |  | [out, retval] VARIANT * | _pVal_ |
|  | ) |  |
|  | Returns the maximum value of channel in the interval taken into account. [Read] |

**Parameters:**
|  | _TimeStart_ | [in] The start time in seconds. |
| --- | --- |
| _TimeEnd_ | [in] The end time in seconds. |
  
**Note:**
      
  
|  | HRESULT Min | ( | [in] VARIANT | _TimeStart_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _TimeEnd_ , |
|  |  | [out, retval] VARIANT * | _pVal_ |
|  | ) |  |
|  | Returns the minimum value of channel in the interval taken into account. [Read] |

**Parameters:**
|  | _TimeStart_ | [in] The start time in seconds. |
| --- | --- |
| _TimeEnd_ | [in] The end time in seconds. |
  
**Note:**
      
  
|  | HRESULT OutputFormat | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the output format of a channel. [Read] |

**Note:**
     Output Format can return the following values: 0 Auto 1 Decimal 2 Float Numeric 3 Float Scientific 4 Hexadecimal 5 Binary 6 Ascii 7 Enum   
| --- | --- |
  
|  | HRESULT ParameterGain | ( | [out, retval] double * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the channel Gain correction value configured in WinTAX Parameters setup. [Read / Write] |

**Remarks:**
     If Parameter is already set in Parameters configuration value is changed with the newer. If not, a new Parameter is added to WinTAX Parameters configuration   
| --- | --- |
  
|  | HRESULT ParameterOffset | ( | [out, retval] double * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the channel Offset correction value configured in WinTAX Parameters setup. [Read / Write] |

**Remarks:**
     If Parameter is already set in Parameters configuration value is changed with the newer. If not, a new Parameter is added to WinTAX Parameters configuration   
| --- | --- |
  
|  | HRESULT PutMatlabStructureWithTime | ( | [in, optional] VARIANT | _VariableName_ , |
| --- | --- | --- | --- |
|  |  | [in, optional] VARIANT | _Matlab_ |
|  | ) |  |
|  | Fills a Matlab Structure With Time with the samples amd timestamp of the channel |

**Parameters:**
|  | _VariableName_ | [in, optional] A Matlab variable name whose values are put in the channel samples. If empty, it's used the channel name. (see [ITelChannel::Name](interfaceITelChannel.md#a0) ) |
| --- | --- |
| _Matlab_ | [in, optional] A Matlab interface used to retrieve the variable. If empty, it's used the WinTAX Matlab application used in export operation (see [IApplication::MatlabApplication](interfaceIApplication.md#a58) ) |
  
**Remarks:**
     This method only works with OLE Channels (see [IWTXChannels::Add](interfaceIWTXChannels.md#a0)). The SampleRate of the channel must be already set with a valid frequency. Refer to [OLE Channels](ole_channels.md) for further information and [Structure with time data type](matlab_integration.md#sample_structure_with_time) for a short data description of structure with times   
  
|  | HRESULT PutMatlabValues | ( | [in] VARIANT | _TimeStart_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _TimeEnd_ , |
|  |  | [in, optional] VARIANT | _VariableName_ , |
|  |  | [in, optional] VARIANT | _Matlab_ |
|  | ) |  |
|  | Fills a Matlab vector with the samples of the channel |

**Parameters:**
|  | _TimeStart_ | [in] The start time in seconds. |
| --- | --- |
| _TimeEnd_ | [in] The end time in seconds. |
| _VariableName_ | [in, optional] A Matlab variable name whose values are put in the channel samples. If empty, it's used the channel name. (see [ITelChannel::Name](interfaceITelChannel.md#a0) ) |
| _Matlab_ | [in, optional] A Matlab interface used to retrieve the variable. If empty, it's used the WinTAX Matlab application used in export operation (see [IApplication::MatlabApplication](interfaceIApplication.md#a58) ) |
  
**Remarks:**
     This method only works with OLE Channels (see [IWTXChannels::Add](interfaceIWTXChannels.md#a0)). The SampleRate of the channel must be already set with a valid frequency. Refer to [OLE Channels](ole_channels.md) for further information and [Sample Code](matlab_integration.md#sample_matlab) for how to use the method.   
  
|  | HRESULT SamplesCount | ( | [in] VARIANT | _TimeStart_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _TimeEnd_ , |
|  |  | [out, retval] long * | _pVal_ |
|  | ) |  |
|  | Returns the number of samples available in the interval taken into account. [Read] |

**Parameters:**
|  | _TimeStart_ | [in] The start time in seconds. |
| --- | --- |
| _TimeEnd_ | [in] The end time in seconds. |
  
**Note:**
     The following example log channel statistics when the zoom of current graph window changes 
    
    
    Function Window_OnZoomChange(vStart, vEnd)
    
    Dim [Min](interfaceIWTXChannel.md#a7)
    Dim [Max](interfaceIWTXChannel.md#a8)
    Dim [Avg](interfaceIWTXChannel.md#a9)
    Dim nSamples
    Dim sStat
    
    Set oChannel = Application.CurrentData(0).Channels("ch1" )
    
    [Min](interfaceIWTXChannel.md#a7) = oChannel.Min( vStart, vEnd )
    [Max](interfaceIWTXChannel.md#a8) = oChannel.Max( vStart, vEnd )
    [Avg](interfaceIWTXChannel.md#a9) = oChannel.Avg( vStart, vEnd )
    nSamples = oChannel.SamplesCount( vStart, vEnd )
    
    
    Application.LogText ...
    
    End Function
      
  
|  | HRESULT SetMeasureUnit | ( | [in] BSTR | _newVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Sets the channel's unit of measure. |

**Remarks:**
     This method only works with OLE Channels (see [IWTXChannels::Add](interfaceIWTXChannels.md#a0)).   
| --- | --- |
  
|  | HRESULT SetTimeStartOffset | ( | [in] double | _Val_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Set the time start offset of first sample of the lap |

**Remarks:**
     TimeStartOffset is the time offset of the first sample with respect to beacon. See the picture below 

**Note:**
     TimeStartOffset is expressed in seconds and it must be greater or equal to zero and less than channel step.   
| --- | --- |
  
|  | HRESULT SetValuesMath | ( | [in] VARIANT | _sExpression_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Fills the samples of the OLE Channel with the results of a math expression |

**Parameters:**
|  | _sExpression_ | [in] A math expression whose results are put in the channel samples |
| --- | --- |
  
**Remarks:**
     This method only works with OLE Channels (see [IWTXChannels::Add](interfaceIWTXChannels.md#a0)). The SampleRate of the channel must be already set with a valid frequency. Refer to [OLE Channels](ole_channels.md) for further information.   
  
|  | HRESULT StdDev | ( | [in] VARIANT | _TimeStart_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _TimeEnd_ , |
|  |  | [out, retval] VARIANT * | _pVal_ |
|  | ) |  |
|  | Returns the standard deviation of the channel in the interval taken into account. [Read] |

**Parameters:**
|  | _TimeStart_ | [in] The start time in seconds. |
| --- | --- |
| _TimeEnd_ | [in] The end time in seconds. |
  
**Note:**
      
  
|  | HRESULT ValuesAtSampleRate | ( | [in] VARIANT | _TimeStart_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _TimeEnd_ , |
|  |  | [in] long | _lSampleRate_ , |
|  |  | [in, optional] VARIANT | _Mode_ , |
|  |  | [out, retval] VARIANT * | _pVal_ |
|  | ) |  |
|  | Returns a set of sampled values of the channel, in a time range. [Read / Write] |

**Parameters:**
|  | _TimeStart_ | [in] The start time in seconds. |
| --- | --- |
| _TimeEnd_ | [in] The end time in seconds. |
| _lSampleRate_ | [in] sample rate used to get channel samples |
| _Mode_ | [in/optional] downsampling / upsampling mode. 0: Default, first value 1: Line interpolation 2: Average 3: Max 4: Min |
  
**Note:**
     The following example create Ole channels based on same data re-sampled with different methods 
    
    
    Dim oOleChan
    Dim oOleChanDownSampled
    Dim oOleChanUpSampled
    Dim LoadTime
    Dim fFreq
    Dim fDownFreq
    Dim fUpFreq
    
    fFreq = 10
    fDownFreq = fFreq / 2
    fUpFreq = fFreq * 2
    
    LoadTime = Application.CurrentData(0).Time
    
    Randomize()
    
    Set oOleChan = Application.CurrentData(0).Channels.Add("OLECHANNEL")
    oOleChan.SampleRate = fFreq
    ' Fills the samples with the result of an elaboration: 
    For iTime = 0 To LoadTime Step(1 / fFreq) 
      'Generate random value between 1 and 6. 
      oOleChan.Value( iTime ) = CInt(Int((6 * Rnd()) + 1)) 
    Next 
    
    
    Set oOleChanDownSampled = Application.CurrentData(0).Channels.Add("OLECHANNEL_DWS_BASE")
    oOleChanDownSampled.SampleRate = fDownFreq 
    oOleChanDownSampled.Values(0,LoadTime ) = oOleChan.ValuesAtSampleRate(0,LoadTime , fDownFreq ) 
    
    Set oOleChanDownSampled = Application.CurrentData(0).Channels.Add("OLECHANNEL_DWS_LINE")
    oOleChanDownSampled.SampleRate = fDownFreq 
    oOleChanDownSampled.Values(0,LoadTime ) = oOleChan.ValuesAtSampleRate(0,LoadTime , fDownFreq , 1 ) 
    
    Set oOleChanDownSampled = Application.CurrentData(0).Channels.Add("OLECHANNEL_DWS_AVG")
    oOleChanDownSampled.SampleRate = fDownFreq 
    oOleChanDownSampled.Values(0,LoadTime ) = oOleChan.ValuesAtSampleRate(0,LoadTime , fDownFreq , 2 ) 
    
    Set oOleChanDownSampled = Application.CurrentData(0).Channels.Add("OLECHANNEL_DWS_MAX")
    oOleChanDownSampled.SampleRate = fDownFreq 
    oOleChanDownSampled.Values(0,LoadTime ) = oOleChan.ValuesAtSampleRate(0,LoadTime , fDownFreq , 3 ) 
    
    Set oOleChanDownSampled = Application.CurrentData(0).Channels.Add("OLECHANNEL_DWS_MIN")
    oOleChanDownSampled.SampleRate = fDownFreq 
    oOleChanDownSampled.Values(0,LoadTime ) = oOleChan.ValuesAtSampleRate(0,LoadTime , fDownFreq , 4 ) 
    
    Set oOleChanUpSampled = Application.CurrentData(0).Channels.Add("OLECHANNEL_UPS_BASE")
    oOleChanUpSampled.SampleRate = fUpFreq 
    oOleChanUpSampled.Values(0,LoadTime ) = oOleChan.ValuesAtSampleRate(0,LoadTime , fUpFreq ) 
    
    
    Set oOleChanUpSampled = Application.CurrentData(0).Channels.Add("OLECHANNEL_UPS_LINE")
    oOleChanUpSampled.SampleRate = fUpFreq 
    oOleChanUpSampled.Values(0,LoadTime ) = oOleChan.ValuesAtSampleRate(0,LoadTime , fUpFreq , 1 ) 
    