[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXExports Interface Reference

The IWTXExports exposes WinTAX database export functions available from WinTAX4 DataBrowser. More...

[List of all members.](interfaceIWTXExports-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [ExportToBIN](interfaceIWTXExports.md#a0) ([in] BSTR bsIn,[in] BSTR bsConfig,[in] BSTR bsOut)  
HRESULT | [ExportToCSV](interfaceIWTXExports.md#a1) ([in] BSTR bsIn,[in] BSTR bsConfig,[in] BSTR bsOut)  
HRESULT | [ExportToASCII](interfaceIWTXExports.md#a2) ([in] BSTR bsIn,[in] BSTR bsConfig,[in] BSTR bsOut)  
HRESULT | [ExportCHToZTX](interfaceIWTXExports.md#a3) ([in] BSTR bsIn,[in] BSTR bsConfig,[in] BSTR bsOut)  
HRESULT | [ExportToXLS](interfaceIWTXExports.md#a4) ([in] BSTR bsIn,[in] BSTR bsConfig,[in] BSTR bsOut)  
HRESULT | [ExportToMatlab](interfaceIWTXExports.md#a5) ([in] BSTR bsIn,[in] BSTR bsConfig,[in] BSTR bsOut)  
HRESULT | [ExportChangeCodeKnownLicenses](interfaceIWTXExports.md#a6) ([out, retval] VARIANT *LicenseNames)  
| HRESULT | [ExportToZTX](interfaceIWTXExports.md#a7) ([in] BSTR bsIn,[in] BSTR bsOut,[in] BSTR bsOutputLicenseName) |
  
* * *

## Detailed Description

The IWTXExports exposes WinTAX database export functions available from WinTAX4 DataBrowser. 

Example: 
    
    
    'Setup configuration file ( must exist )
    sCnfPath = Application.ExportDirectory & "autoexport.ech"
    
    'Setup output path in WinTAX Export folder
    sOutPath = Application.ExportDirectory 
    'Setup output base file name with current time 
    sOutPath = sOutPath & Year(Now) & Month(Now) & Day(Now) & Hour(Now) & Minute(Now) & Second(Now)
    
    'export data to ASCII
    sExpPath = sOutPath & ".prn"
    Application.Exports.ExportToASCII sFile, sCnfPath, sExpPath 
    
    'export data to CSV
    sExpPath = sOutPath & ".csv"
    Application.Exports.ExportToCSV sFile, sCnfPath, sExpPath
    
    'export data to BIN
    sExpPath = sOutPath & ".bin"
    Application.Exports.ExportToBIN sFile, sCnfPath, sExpPath
    
    

* * *

## Member Function Documentation

|  | HRESULT ExportChangeCodeKnownLicenses | ( | [out, retval] VARIANT * | _LicenseNames_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Get an array of license names allowed for change code. These names can be used in ExportToZTX method to force a specific change code. |

**Parameters:**
|  | _Result_ | [out, retval] Result, an array of string |
| --- | --- |
  
**Returns:**
     S_OK The operation has been performed correctly. E_FAIL Operation failed due to generic error. E_NOTIMPL Method not available for this license. 

**Remarks:**
     The function is enabled only on some licenses   
  
|  | HRESULT ExportCHToZTX | ( | [in] BSTR | _bsIn_ , |
| --- | --- | --- | --- |
|  |  | [in] BSTR | _bsConfig_ , |
|  |  | [in] BSTR | _bsOut_ |
|  | ) |  |
|  | Export data in WinTAX database file format (*.ztx) |

**Parameters:**
|  | _bsIn_ | Source WinTAX database file path (*.ztx) |
| --- | --- |
| _bsConfig_ | Source WinTAX export configuration file path (*.ech) |
| _bsOut_ | Output export directory |
  
**Note:**
     Output export directory is assumed to be the WinTAX database Run level   
  
|  | HRESULT ExportToASCII | ( | [in] BSTR | _bsIn_ , |
| --- | --- | --- | --- |
|  |  | [in] BSTR | _bsConfig_ , |
|  |  | [in] BSTR | _bsOut_ |
|  | ) |  |
|  | Export data in ASCII text as tab separated values (*.prn) |

**Parameters:**
|  | _bsIn_ | Source WinTAX database file path (*.ztx) |
| --- | --- |
| _bsConfig_ | Source WinTAX export configuration file path (*.ech) |
| _bsOut_ | Output export file path |
  
**Note:**
      
  
|  | HRESULT ExportToBIN | ( | [in] BSTR | _bsIn_ , |
| --- | --- | --- | --- |
|  |  | [in] BSTR | _bsConfig_ , |
|  |  | [in] BSTR | _bsOut_ |
|  | ) |  |
|  | Export data in binary compact (interleaved) format (*.bin) |

**Parameters:**
|  | _bsIn_ | Source WinTAX database file path (*.ztx) |
| --- | --- |
| _bsConfig_ | Source WinTAX export configuration file path (*.ech) |
| _bsOut_ | Output export file path |
  
**Note:**
     see WinTAX4 user guide for detailed explanation about the format   
  
|  | HRESULT ExportToCSV | ( | [in] BSTR | _bsIn_ , |
| --- | --- | --- | --- |
|  |  | [in] BSTR | _bsConfig_ , |
|  |  | [in] BSTR | _bsOut_ |
|  | ) |  |
|  | Export data in ASCII text as comma separated values (*.csv) |

**Parameters:**
|  | _bsIn_ | Source WinTAX database file path (*.ztx) |
| --- | --- |
| _bsConfig_ | Source WinTAX export configuration file path (*.ech) |
| _bsOut_ | Output export file path |
  
**Note:**
      
  
|  | HRESULT ExportToMatlab | ( | [in] BSTR | _bsIn_ , |
| --- | --- | --- | --- |
|  |  | [in] BSTR | _bsConfig_ , |
|  |  | [in] BSTR | _bsOut_ |
|  | ) |  |
|  | Export data in MatLab file format (*.mat) |

**Parameters:**
|  | _bsIn_ | Source WinTAX database file path (*.ztx) |
| --- | --- |
| _bsConfig_ | Source WinTAX export configuration file path (*.ech) |
| _bsOut_ | Output export file path |
  
**Note:**
      
  
|  | HRESULT ExportToXLS | ( | [in] BSTR | _bsIn_ , |
| --- | --- | --- | --- |
|  |  | [in] BSTR | _bsConfig_ , |
|  |  | [in] BSTR | _bsOut_ |
|  | ) |  |
|  | Export data in Excel sheet file format (*.xls) |

**Parameters:**
|  | _bsIn_ | Source WinTAX database file path (*.ztx) |
| --- | --- |
| _bsConfig_ | Source WinTAX export configuration file path (*.ech) |
| _bsOut_ | Output export file path |
  
**Note:**
      
  
|  | HRESULT ExportToZTX | ( | [in] BSTR | _bsIn_ , |
| --- | --- | --- | --- |
|  |  | [in] BSTR | _bsOut_ , |
|  |  | [in] BSTR | _bsOutputLicenseName_ |
|  | ) |  |
|  | Export data in WinTAX database file format (*.ztx) |

**Parameters:**
|  | _bsIn_ | Source WinTAX database file path (*.ztx) |
| --- | --- |
| _bsOut_ | Output export directory |
| _bsOutputLicenseName_ | License name for change code options. The function is enabled only on some licenses. If this parameter is empty or set to "Keep License", the source license will be maintained. If the license name is unknow or not a valid license for the change code operation, the change code operation not executed and the source license will be maintained. |
  
**Returns:**
     S_OK The operation has been performed correctly. E_FAIL Operation failed due to generic error. E_NOTIMPL Method not available for this license. 

**Note:**
     Output export directory is assumed to be the WinTAX database Run level 