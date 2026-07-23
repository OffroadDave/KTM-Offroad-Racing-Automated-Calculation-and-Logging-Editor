[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelAuthorize Interface Reference

The ITelAuthorize Interface allows to perform client application authorization necessary to activate WinTAX OLE Automation. More...

Inherited by [IWTXAuthorize](interfaceIWTXAuthorize.md). 

[List of all members.](interfaceITelAuthorize-members.md)  
---  
  
## Public Member Functions  
  
| HRESULT | [Authorize](interfaceITelAuthorize.md#a0) ([in] IUnknown *pUnk,[in] BSTR Key) |
  
* * *

## Detailed Description

The ITelAuthorize Interface allows to perform client application authorization necessary to activate WinTAX OLE Automation. 

Functionality of certain interfaces that are requiring authentication. 

* * *

## Member Function Documentation

|  | HRESULT Authorize | ( | [in] IUnknown * | _pUnk_ , |
| --- | --- | --- | --- |
|  |  | [in] BSTR | _Key_ |
|  | ) |  |
|  | Perform interface authorization |

**Parameters:**
|  | _pUnk_ | Interface to be authorized |
| --- | --- |
| _Key_ | Authorization code |
  
**Note:**
     The following example show how to perform the authorization of WinTAX interface on a client application written in VBA 
    
    
    Dim oWtx As Wintax4Lib.Application
    Dim oAuth As Wtx4DataInterfacesLib.TelAuthorize
    
    'WinTAX OLE Client Authorization code
    Const sAuthorizationCode As String = "12345678910"
    
    'Activate error handling
    On Error Resume Next
    
    'Create WinTAX Appliation
    Set oWtx = CreateObject("Wintax4.application")
    
    'Create Authirize interface
    Set oAuth = CreateObject("Wtx4DataInterfaces.TelAuthorize")
    
    'This line will throw a "Permission Denied" error ( code 70 )
    oWtx.LogText "WinTAX client interface created"
    
    'To prevent this error WinTAX Application must be registered as follow
    If Err.Number = 70 Then
        'Interface authorization via TelAuthirize object
        oAuth.Authorize oWtx, sAuthorizationCode
        oWtx.LogText "WinTAX client interface authorized"
        Err.Clear
    End If
    