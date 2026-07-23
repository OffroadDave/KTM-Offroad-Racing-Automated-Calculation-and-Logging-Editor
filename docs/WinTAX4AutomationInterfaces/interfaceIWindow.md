[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWindow Interface Reference

The IWindow interface exposes methods and properties of a Wintax4 Analysis Window object. More...

[List of all members.](interfaceIWindow-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Caption](interfaceIWindow.md#a0) ([out, retval] BSTR *pVal)  
HRESULT | [Load](interfaceIWindow.md#a2) ([in] BSTR bFile)  
HRESULT | [Save](interfaceIWindow.md#a3) ([in] BSTR bFile)  
HRESULT | [XML](interfaceIWindow.md#a4) ([out, retval] BSTR *pVal)  
HRESULT | [Cursor](interfaceIWindow.md#a6) ([out, retval] VARIANT *pVal)  
HRESULT | [ZoomStart](interfaceIWindow.md#a8) ([out, retval] VARIANT *pVal)  
HRESULT | [ZoomEnd](interfaceIWindow.md#a10) ([out, retval] VARIANT *pVal)  
HRESULT | [Type](interfaceIWindow.md#a12) ([out, retval] VARIANT *pVal)  
HRESULT | [Zoom](interfaceIWindow.md#a13) ([in]VARIANT vStart,[in]VARIANT vEnd)  
HRESULT | [Close](interfaceIWindow.md#a14) ()  
| HRESULT | [TextItems](interfaceIWindow.md#a15) ([out, retval] ITextItems **pVal) |
  
* * *

## Detailed Description

The IWindow interface exposes methods and properties of a Wintax4 Analysis Window object. 

The Analysis window object represents a single window doing a specific analysis (eg. Graphs, X-Y, ..). The Window object contains: \- Window-wide settings, options and methods. \- Properties that return window objects, such as XML configuration. 

* * *

## Member Function Documentation

|  | HRESULT Caption | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Specifies the text displayed in a window's caption. [Read / Write] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Close | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | Closes the window |

**Note:**
      
| --- | --- |
  
|  | HRESULT Cursor | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The cursor position of the window, if the window contains a cursor. [Read / Write] |

**Remarks:**
     Cursor position is usually accepted as a double value expressed in seconds. When 'Absolute Time' mode is selected, cursor position can be also a string formatted as "HH:MM:SS.mmm".   
| --- | --- |
  
|  | HRESULT Load | ( | [in] BSTR | _bFile_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Loads a window configuration |

**Parameters:**
|  | _bFile_ | A string value of the configuration file path. |
| --- | --- |
  
|  | HRESULT Save | ( | [in] BSTR | _bFile_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Saves a window configuration |

**Parameters:**
|  | _bFile_ | A string value of the configuration file path. |
| --- | --- |
  
|  | HRESULT TextItems | ( | [out, retval] ITextItems ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | ITextItems interface that makes possible accessing single cell elements of alphanumeric windows. [Read] |

**Remarks:**
     Only some windows allow accessing single elements. These windows are Alarms, Channels, Numeric Table, Lap Report, Map Report, Events Report.   
| --- | --- |
  
|  | HRESULT Type | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Acquisition Table 28  RealTime XY 30  Circular Graphs 32  Legend 34  Event Report 35  Video 36  RealTime Section Time Report 37  Object Control 38  XYZ 3D 39  GG 40  Spectrogram 41  Steering wheel 42 |
| --- | --- |
  
|  | HRESULT XML | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Specifies the XML text associated to the window's configuration. [Read / Write] Example: |
    
    
    Dim oDoc
    Set oDoc = CreateObject("MSXML.DOMDocument")
    oDoc.async = False
    oDoc.loadXML Window.XML
    
    Dim oLabels
    Set oLabels = oDoc.getElementsByTagName("Label")
    
    For Each Label In oLabels
      Dim oChan
      Set oChan = Application.CurrentData(0).Channels( Label.Text )
      Application.LogText "Channel: " & oChan.Name & " " & oChan.SampleRate & "Hz"
    Next
      
  
| --- | --- |
  
|  | HRESULT Zoom | ( | [in] VARIANT | _vStart_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _vEnd_ |
|  | ) |  |
|  | Applies a time based zoom to current window. |

**Parameters:**
|  | _vStart_ | Zoom start position. |
| --- | --- |
| _vEnd_ | Zoom end position. |
  
**Note:**
     Applies only if the window manages some kind of zoom. (eg. on Lap Report windows it does nothing) 

**Remarks:**
     Zoom positions are usually accepted as a double value expressed in seconds. When 'Absolute Time' mode is selected, zoom positions can be also strings formatted as "HH:MM:SS.mmm".   
  
|  | HRESULT ZoomEnd | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Current zoom end position of the window. [Read / Write] |

**Remarks:**
     Zoom end position is usually accepted as a double value expressed in seconds. When 'Absolute Time' mode is selected, Zoom end position can be also a string formatted as "HH:MM:SS.mmm".   
| --- | --- |
  
|  | HRESULT ZoomStart | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Current zoom start position of the window. [Read / Write] |

**Remarks:**
     Zoom start position is usually accepted as a double value expressed in seconds. When 'Absolute Time' mode is selected, Zoom start position can be also a string formatted as "HH:MM:SS.mmm".   
---|---