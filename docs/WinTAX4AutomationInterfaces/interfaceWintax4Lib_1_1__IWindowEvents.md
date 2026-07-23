[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# _IWindowEvents Interface Reference

Use this interface to respond to configuration and state changes. More...

[List of all members.](interfaceWintax4Lib_1_1__IWindowEvents-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [OnCreate](interfaceWintax4Lib_1_1__IWindowEvents.md#a0) ()  
HRESULT | [OnClose](interfaceWintax4Lib_1_1__IWindowEvents.md#a1) ()  
HRESULT | [OnChange](interfaceWintax4Lib_1_1__IWindowEvents.md#a2) ()  
HRESULT | [OnCursorPosChange](interfaceWintax4Lib_1_1__IWindowEvents.md#a3) ([in]VARIANT TimePos)  
HRESULT | [OnZoomChange](interfaceWintax4Lib_1_1__IWindowEvents.md#a4) ([in]VARIANT vStart,[in]VARIANT vEnd)  
HRESULT | [OnDataRefreshStarted](interfaceWintax4Lib_1_1__IWindowEvents.md#a5) ()  
HRESULT | [OnDataRefreshFinished](interfaceWintax4Lib_1_1__IWindowEvents.md#a6) ()  
| HRESULT | [OnLoadData](interfaceWintax4Lib_1_1__IWindowEvents.md#a7) ([in] IUnknown *pVal) |
  
* * *

## Detailed Description

Use this interface to respond to configuration and state changes. 

The IWindowEvents interface is a callback interface to which Wintax4 sends event notifications. 

* * *

## Member Function Documentation

|  | HRESULT OnChange | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | The OnChange method receives an event notice indicating that the current window configuration has been changed. |

**Note:**
      
| --- | --- |
  
|  | HRESULT OnClose | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | The OnClose method receives an event notice indicating that the current window has been closed. |

**Note:**
      
| --- | --- |
  
|  | HRESULT OnCreate | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | The OnCreate method receives an event notice indicating that the current window has been created. |

**Remarks:**
     This event is only available in WinTAX within the window scripting environment. When the event is fired, the window has yet to be added to the collection of active windows and therefore cannot be accessed from external client applications via ole automation. 

**Note:**
      
| --- | --- |
  
|  | HRESULT OnCursorPosChange | ( | [in] VARIANT | _TimePos_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The OnCursorPosChange method receives an event notice indicating that the current window cursor has been moved. |

**Parameters:**
|  | _TimePos_ | [in] New cursor position. |
| --- | --- |
  
**Remarks:**
     OnCursorPosChange can be used to customize some cursor operations, such as Snapping, Auto Connect and so on .. When 'Relative Time' mode is selected, cursor position is expressed in seconds. When 'Absolute Time' is selected, cursor position is a string formatted as "HH:MM:SS.mmm". Example: 
    
    
    Function Window_OnCursorPosChange(TimePos)
    
            Application.LogText "Cursor Position is at " & Round(TimePos,1) & " seconds"
    
    End Function
      
  
|  | HRESULT OnDataRefreshFinished | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | The OnDataRefreshFinished method receives an event notice indicating that the current window has just finished to calculate a new set of results. |

**Remarks:**
     This event is only available for "Channels", "Numeric Table" and "Lap Report" windows. 

**Note:**
      
| --- | --- |
  
|  | HRESULT OnDataRefreshStarted | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | The OnDataRefreshStarted method receives an event notice indicating that the current window is about to calculate a new set of results. |

**Remarks:**
     This event is only available for "Channels", "Numeric Table" and "Lap Report" windows. 

**Note:**
      
| --- | --- |
  
|  | HRESULT OnLoadData | ( | [in] IUnknown * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The OnLoadData method receives an event notice indicating that the current window has just loaded new data to calculate a new set of results. |

**Remarks:**
     This event is only available for "Lap Report" and "Trend" windows. 

**Note:**
     Refer to the page [OLE Channels](ole_channels.md) for a simple example on how to manage an ole channel inside lap report window   
| --- | --- |
  
|  | HRESULT OnZoomChange | ( | [in] VARIANT | _vStart_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _vEnd_ |
|  | ) |  |
|  | The OnZoomChange method receives an event notice indicating that current window zoom has been changed. |

**Parameters:**
|  | _vStart_ | [in] New start zoom position. |
| --- | --- |
| _vEnd_ | [in] New end zoom position. |
  
**Remarks:**
     When 'Relative Time' mode is selected, time values are expressed in seconds. When 'Absolute Time' is selected, time values are strings formatted as "HH:MM:SS.mmm". 

This event is only available for "Graph" windows. 