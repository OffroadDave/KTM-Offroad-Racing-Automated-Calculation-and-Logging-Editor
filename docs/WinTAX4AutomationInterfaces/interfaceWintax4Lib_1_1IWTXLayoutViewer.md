[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXLayoutViewer Interface Reference

The [IWTXLayoutViewer](interfaceWintax4Lib_1_1IWTXLayoutViewer.md) interface exposes methods and properties of a Wintax4 Layout Viewer window The Layout Viewer Window interface can be uset to query the current viewer settings and to get/set the acitive layout that is currently displayed. More...

[List of all members.](interfaceWintax4Lib_1_1IWTXLayoutViewer-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Visible](interfaceWintax4Lib_1_1IWTXLayoutViewer.md#a0) ([out, retval] VARIANT *pVal)  
HRESULT | [MonitorAvailable](interfaceWintax4Lib_1_1IWTXLayoutViewer.md#a2) ([out, retval] VARIANT *pVal)  
| HRESULT | [CurrentLayout](interfaceWintax4Lib_1_1IWTXLayoutViewer.md#a3) ([out, retval] BSTR *pVal) |
  
* * *

## Detailed Description

The [IWTXLayoutViewer](interfaceWintax4Lib_1_1IWTXLayoutViewer.md) interface exposes methods and properties of a Wintax4 Layout Viewer window The Layout Viewer Window interface can be uset to query the current viewer settings and to get/set the acitive layout that is currently displayed. 

* * *

## Member Function Documentation

|  | HRESULT CurrentLayout | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the current Layout displayed (or to be displayed) in the Layout Viewer [Read / Write] |

**Note:**
     Setting a new value on this property will cause the immediate switch to the new layout. The layout must exist in the 'Users' folder in order to switch correctly. Reading the value of this property returns the name of the current layout selected.   
| --- | --- |
  
|  | HRESULT MonitorAvailable | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns false(0) if this Layout Viewer is currently hidden because a monitor is not connected to the PC. [Read] |

**Note:**
     This value can be true(-1) even if the layout window is hidden by a user configuration but it can be disaplayed if wanted.   
| --- | --- |
  
|  | HRESULT Visible | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the visibility of the Layout Viewer window. True (-1) if the window is visible, else False (0). [Read / Write] |

**Note:**
      
---|---