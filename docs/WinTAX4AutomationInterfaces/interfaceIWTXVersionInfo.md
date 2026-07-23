[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXVersionInfo Interface Reference

The IWTXVersionInfo interface allows access to current WinTAX version information. More...

[List of all members.](interfaceIWTXVersionInfo-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [VersionNumber](interfaceIWTXVersionInfo.md#a0) ([out, retval] VARIANT *pVal)  
HRESULT | [LicenseLevel](interfaceIWTXVersionInfo.md#a1) ([out, retval] VARIANT *pVal)  
HRESULT | [ReleaseDate](interfaceIWTXVersionInfo.md#a2) ([out, retval] VARIANT *pVal)  
HRESULT | [BuildArchitecture](interfaceIWTXVersionInfo.md#a3) ([out, retval] VARIANT *pVal)  
HRESULT | [RunningPrivileges](interfaceIWTXVersionInfo.md#a4) ([out, retval] VARIANT *pVal)  
HRESULT | [PasswordProtected](interfaceIWTXVersionInfo.md#a5) ([out, retval] VARIANT *pVal)  
HRESULT | [DongleProtected](interfaceIWTXVersionInfo.md#a6) ([out, retval] VARIANT *pVal)  
| HRESULT | [ExpirationDate](interfaceIWTXVersionInfo.md#a7) ([out, retval] VARIANT *pVal) |
  
* * *

## Detailed Description

The IWTXVersionInfo interface allows access to current WinTAX version information. 

* * *

## Member Function Documentation

|  | HRESULT BuildArchitecture | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the current WinTAX build architecture. [Read] |

**Note:**
     WinTAX build architectures is returned as a code (number). Possible values are: \- 1: "x64" \- 2: "x86"   
| --- | --- |
  
|  | HRESULT DongleProtected | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns Returns TRUE if the current WinTAX is protected with dongle. [Read]. |

**Note:**
      
| --- | --- |
  
|  | HRESULT ExpirationDate | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns Returns the current WinTAX expiration date. [Read]. |

**Note:**
     The expiration date is returned as a string   
| --- | --- |
  
|  | HRESULT LicenseLevel | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the current WinTAX license level. [Read] |

**Note:**
     The license level is returned as a code (number). Possible values are: \- 1: "Pro" \- 2: "Team" \- 3: "Expert" \- 4: "Sat" \- 5: "User" \- 6: "Junior" \- 7: "Demo" \- 8: "Basic"   
| --- | --- |
  
|  | HRESULT PasswordProtected | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns Returns TRUE if the current WinTAX is protected with password. [Read]. |

**Note:**
      
| --- | --- |
  
|  | HRESULT ReleaseDate | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the current WinTAX release date. [Read] |

**Note:**
     The release date is returned as a string   
| --- | --- |
  
|  | HRESULT RunningPrivileges | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the current WinTAX running privileges. [Read] |

**Note:**
     WinTAX running privileges is returned as a code (number). Possible values are: \- 1: "standard user" \- 2: "administrator"   
| --- | --- |
  
|  | HRESULT VersionNumber | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the current WinTAX version number. [Read] |

**Note:**
     The version number is returned as a string   
---|---