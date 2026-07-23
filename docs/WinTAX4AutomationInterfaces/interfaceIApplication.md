[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IApplication Interface Reference

The IApplication interface exposes methods and properties of the Application Object of WinTAX4. More...

[List of all members.](interfaceIApplication-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Run](interfaceIApplication.md#a0) ([in]BSTR sCommand,[in, optional] VARIANT vArg1,[in, optional] VARIANT vArg2,[in, optional] VARIANT vArg3,[in, optional] VARIANT vArg4,[in, optional] VARIANT vArg5,[in, optional] VARIANT vArg6,[in, optional] VARIANT vArg7,[in, optional] VARIANT vArg8,[in, optional] VARIANT vArg9,[in, optional] VARIANT vArg10)  
HRESULT | [LogText](interfaceIApplication.md#a1) ([in]BSTR sLog)  
HRESULT | [Visible](interfaceIApplication.md#a2) ([out, retval] VARIANT *pVal)  
HRESULT | [Windows](interfaceIApplication.md#a4) ([out, retval] [ICollectWindows](interfaceICollectWindows.md) **pVal)  
HRESULT | [ClearLog](interfaceIApplication.md#a5) ()  
HRESULT | [CurrentData](interfaceIApplication.md#a6) ([out, retval] [IWTXDataLoads](interfaceIWTXDataLoads.md) **pVal)  
HRESULT | [CurrentDataRealTime](interfaceIApplication.md#a7) ([out, retval] [IWTXDataLoad](interfaceIWTXDataLoad.md) **pVal)  
HRESULT | [Open](interfaceIApplication.md#a8) ([in] VARIANT vVal,[in, optional] VARIANT vDataset)  
HRESULT | [CurrentUser](interfaceIApplication.md#a9) ([out, retval] BSTR *pVal)  
HRESULT | [ProgramDirectory](interfaceIApplication.md#a11) ([out, retval] VARIANT *pVal)  
HRESULT | [UserDirectory](interfaceIApplication.md#a13) ([out, retval] VARIANT *pVal)  
HRESULT | [AcquisitionDirectory](interfaceIApplication.md#a15) ([out, retval] VARIANT *pVal)  
HRESULT | [ExportDirectory](interfaceIApplication.md#a17) ([out, retval] VARIANT *pVal)  
HRESULT | [LibrariesDirectory](interfaceIApplication.md#a19) ([out, retval] VARIANT *pVal)  
HRESULT | [LogDirectory](interfaceIApplication.md#a21) ([out, retval] VARIANT *pVal)  
HRESULT | [CircuitDirectory](interfaceIApplication.md#a23) ([out, retval] VARIANT *pVal)  
HRESULT | [DataDirectories](interfaceIApplication.md#a25) ([out, retval] VARIANT *pVal)  
HRESULT | [HostDirectories](interfaceIApplication.md#a27) ([out, retval] VARIANT *pVal)  
HRESULT | [ImportLapDirectories](interfaceIApplication.md#a29) ([out, retval] VARIANT *pVal)  
HRESULT | [ConfParameters](interfaceIApplication.md#a31) ([out, retval] [IWTXConfiguration](interfaceIWTXConfiguration.md) **pVal)  
HRESULT | [ConfVCHLibraries](interfaceIApplication.md#a32) ([out, retval] [IWTXConfigurations](interfaceIWTXConfigurations.md) **pVal)  
HRESULT | [CurrentSession](interfaceIApplication.md#a33) ([out, retval] [IWTXSession](interfaceIWTXSession.md) **pVal)  
HRESULT | [Devices](interfaceIApplication.md#a34) ([in]LONG lAcq,[out, retval] VARIANT *pVal)  
HRESULT | [CurrentDevice](interfaceIApplication.md#a35) ([in]LONG lAcq,[out, retval] VARIANT *pVal)  
HRESULT | [AcquisitionStarted](interfaceIApplication.md#a37) ([in]LONG lAcq,[out, retval] VARIANT *pVal)  
HRESULT | [StartAcquisition](interfaceIApplication.md#a38) ([in] LONG lAcq)  
HRESULT | [StopAcquisition](interfaceIApplication.md#a39) ([in] LONG lAcq)  
HRESULT | [SpecSheet](interfaceIApplication.md#a40) ([out, retval] [IWTXSpecSheet](interfaceIWTXSpecSheet.md) **pVal)  
HRESULT | [OffsetGainMode](interfaceIApplication.md#a41) ([out, retval] LONG *pVal)  
HRESULT | [Properties](interfaceIApplication.md#a43) ([out, retval] [ITelProperties](interfaceITelProperties.md) **pVal)  
HRESULT | [Users](interfaceIApplication.md#a44) ([out, retval] [IWTXUsers](interfaceIWTXUsers.md) **pVal)  
HRESULT | [Layouts](interfaceIApplication.md#a45) ([out, retval] [IWTXLayouts](interfaceIWTXLayouts.md) **pVal)  
HRESULT | [CurrentLayout](interfaceIApplication.md#a46) ([out, retval] BSTR *pVal)  
HRESULT | [GlobalVirtualChannelsLibraries](interfaceIApplication.md#a48) ([out, retval] VARIANT *pVal)  
HRESULT | [LocalVirtualChannelsLibraries](interfaceIApplication.md#a50) ([out, retval] VARIANT *pVal)  
HRESULT | [LoadParameters](interfaceIApplication.md#a52) ([in] BSTR bsVal)  
HRESULT | [LocalEventsLibraries](interfaceIApplication.md#a53) ([out, retval] VARIANT *pVal)  
HRESULT | [LocalConditionsLibraries](interfaceIApplication.md#a55) ([out, retval] VARIANT *pVal)  
HRESULT | [RunMacro](interfaceIApplication.md#a57) ([in]BSTR sCommand,[in, optional] VARIANT vArg1,[in, optional] VARIANT vArg2,[in, optional] VARIANT vArg3,[in, optional] VARIANT vArg4,[in, optional] VARIANT vArg5,[in, optional] VARIANT vArg6,[in, optional] VARIANT vArg7,[in, optional] VARIANT vArg8,[in, optional] VARIANT vArg9,[in, optional] VARIANT vArg10)  
HRESULT | [MatlabApplication](interfaceIApplication.md#a58) ([out, retval] IDispatch **pVal)  
HRESULT | [Exports](interfaceIApplication.md#a59) ([out, retval] [IWTXExports](interfaceIWTXExports.md) **pVal)  
HRESULT | [Refresh](interfaceIApplication.md#a60) ([in] VARIANT vOpt)  
HRESULT | [CLXChNames](interfaceIApplication.md#a61) ([out, retval] VARIANT *pVal)  
| Returns the list of CLX channel names loaded in WinTAX [Read].   
HRESULT | [VersionInfo](interfaceIApplication.md#a62) ([out, retval] [IWTXVersionInfo](interfaceIWTXVersionInfo.md) **pVal)  
| Returns current WinTAX version information [Read].   
HRESULT | [OLEAutoUpdate](interfaceIApplication.md#a63) ([out, retval] VARIANT *pVal)  
HRESULT | [Load](interfaceIApplication.md#a65) ([in] VARIANT vVal)  
HRESULT | [EnableMergeImportData](interfaceIApplication.md#a66) ([out, retval] VARIANT *pVal)  
HRESULT | [LayoutViewers](interfaceIApplication.md#a68) ([out, retval] IWTXLayoutViewers **pVal)  
HRESULT | [CloseData](interfaceIApplication.md#a69) ()  
| Closes all data currently loaded in the WinTAX.   
HRESULT | [LookupsDirectory](interfaceIApplication.md#a70) ([out, retval] VARIANT *pVal)  
HRESULT | [MathDLLPlugInDirectory](interfaceIApplication.md#a72) ([out, retval] VARIANT *pVal)  
HRESULT | [DataBrowserPlugInDirectory](interfaceIApplication.md#a74) ([out, retval] VARIANT *pVal)  
| Returns or sets the current Wintax4 Data Browser PlugIn files directory.   
HRESULT | [AcqManPlugInDirectory](interfaceIApplication.md#a76) ([out, retval] VARIANT *pVal)  
| Returns or sets the current Wintax4 Acquisition Manager PlugIn files directory.   
HRESULT | [GlobalEventsLibraries](interfaceIApplication.md#a78) ([out, retval] VARIANT *pVal)  
HRESULT | [GlobalConditionsLibraries](interfaceIApplication.md#a80) ([out, retval] VARIANT *pVal)  
HRESULT | [LoadGlobalParametersLibrary](interfaceIApplication.md#a82) ([in] BSTR bsVal)  
HRESULT | [LoadLocalParametersLibrary](interfaceIApplication.md#a83) ([in] BSTR bsVal)  
HRESULT | [UserCustomDirectory](interfaceIApplication.md#a84) ([out, retval] VARIANT *pVal)  
| Returns or sets the current Wintax4 User Custom files directory.   
| HRESULT | [CreateVCRLibrary](interfaceIApplication.md#a86) ([in] BSTR path_output_vcr,[in] VARIANT array_path_vch,[in] VARIANT array_path_dat,[in] VARIANT_BOOL autodiscover_dat,[in] BSTR license_name,[out, retval] VARIANT_BOOL *Result) |
  
* * *

## Detailed Description

The IApplication interface exposes methods and properties of the Application Object of WinTAX4. 

The Application Object represents the entire Wintax4 application. The Application object contains: \- Application-wide settings, options and methods. \- Properties that return top-level objects, such as Windows, configurations, directories, and so on. 

**Note:**
     For internal scripts use, the IApplication interface is accessible via the **Application** named object. 

When WinTAX is closing all interfaces must be released because they will no longer be valid. See "OnExitApplication" event for more details. 

**Remarks:**
     In some WinTAX distributions, this interface needs to be authorized before using methods or properties on a client application. See [ITelAuthorize::Authorize](interfaceITelAuthorize.md#a0) for more details. 

* * *

## Member Function Documentation

|  | HRESULT AcquisitionDirectory | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the current Wintax4 Acquisition directory [ReadOnly] |

**Note:**
     The Acquisition directory is the folder in which downloaded laps and runs are saved. Refer to the page [Sample: Directories](sample_directories.md) for a simple example.   
| --- | --- |
  
|  | HRESULT AcquisitionStarted | ( | [in] LONG | _lAcq_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] VARIANT * | _pVal_ |
|  | ) |  |
|  | Returns the status of the acquisition process. 0 if the process is not started. 1 if the process is running. |

**Parameters:**
|  | _lAcq_ | Index of the acquisition process. 0 for TCP process 1 for DST process. |
| --- | --- |
  
**Note:**
      
  
|  | HRESULT CircuitDirectory | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the current Wintax4 Circuit directory [ReadOnly] |

**Note:**
     The Circuits directory is the folder in which CRT files are stored. Refer to the page [Sample: Directories](sample_directories.md) for a simple example.   
| --- | --- |
  
|  | HRESULT ClearLog | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | Removes all the messages from the LOG window. |

**Note:**
     Affects only the page dedicated to scripting and automation messages.   
| --- | --- |
  
|  | HRESULT ConfParameters | ( | [out, retval] [IWTXConfiguration](interfaceIWTXConfiguration.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns current Wintax4 Parameters configuration [ReadOnly] |

**Note:**
     Refer to the page [Sample: Access to Configuration objects](sample_access_configuration.md) for a simple example.   
| --- | --- |
  
|  | HRESULT ConfVCHLibraries | ( | [out, retval] [IWTXConfigurations](interfaceIWTXConfigurations.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns current Wintax4 Virtual Channel Libraries configuration [ReadOnly] |

**Note:**
     Refer to the page [Sample: Access to Configuration objects](sample_access_configuration.md) for a simple example.   
| --- | --- |
  
|  | HRESULT CreateVCRLibrary | ( | [in] BSTR | _path_output_vcr_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _array_path_vch_ , |
|  |  | [in] VARIANT | _array_path_dat_ , |
|  |  | [in] VARIANT_BOOL | _autodiscover_dat_ , |
|  |  | [in] BSTR | _license_name_ , |
|  |  | [out, retval] VARIANT_BOOL * | _Result_ |
|  | ) |  |
|  | Creates a VCR (i.e. Encrypted VCH) library and saves it at the specified path |

**Parameters:**
|  | _path_output_vcr_ | Full path of the new VCR file |
| --- | --- |
| _array_path_vch_ | Full path of the input VCH libraries. These libraries will be merged in order to obtain the list of channels to include in the VCR library. |
| _array_path_dat_ | Full path of DAT files to add to the VCR file. |
| _autodiscover_dat_ | Flag to switch on/off auto-detection of DAT files from the VCH channels included in the VCR library. Detected DAT files will be added to the VCR file. |
| _license_name_ | Name of the license to encrypt to (As shown in WinTAX). Empty string means same license as caller. |
  
**Returns:**
     TRUE if the operation completed successfully (i.e. a VCR library with at least one channel was successfully saved at the desired path), otherwise FALSE 

**Note:**
     If a VCR file with the same path as the output file already exists, it will be overwritten. 

Only unencrypted VCH files will be considered within files listed in the input libraries array. 

Input VCH libraries will be merged considering the relative order of VCH files into the array, and the relative order of channels into each library. A channel with the same name as an already inserted channel will be discarded. 

DAT files listed in input array will be added first, then DAT files referenced in output library channels. A DAT file with the same name as an already inserted file will be discarded.   
  
|  | HRESULT CurrentData | ( | [out, retval] [IWTXDataLoads](interfaceIWTXDataLoads.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | [IWTXDataLoads](interfaceIWTXDataLoads.md) collection that represents the current data loaded in the application. [ReadOnly] |

**Note:**
     Each item represents a single dataset. A single lap selection is referenced by the first item.   
| --- | --- |
  
|  | HRESULT CurrentDataRealTime | ( | [out, retval] [IWTXDataLoad](interfaceIWTXDataLoad.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | [IWTXDataLoad](interfaceIWTXDataLoad.md) current Real Time dataset. [ReadOnly] |

**Note:**
      
| --- | --- |
  
|  | HRESULT CurrentDevice | ( | [in] LONG | _lAcq_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] VARIANT * | _pVal_ |
|  | ) |  |
|  | Returns or sets current selected device for specific acquisition [Read / Write] |

**Parameters:**
|  | _lAcq_ | Index of the acquisition process. 0 for TCP process 1 for DST process. |
| --- | --- |
  
**Remarks:**
     CurrentDevice(0) (TCP) sets or returns the car name set in the Acquisition Manager TCP drop down list. CurrentDevice(1) (DST) sets or returns the device name as set in the _"Setup Acquisition - > Devices -> DST -> DST Data Source"_  
  
|  | HRESULT CurrentLayout | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the current Wintax4 Layout [Read / Write] |

**Note:**
     Setting a new value on this property will cause the immediate switch to the new layout. The layout must exist in the 'Users' folder in order to switch correctly. Reading the value of this property returns the name of the current layout selected.   
| --- | --- |
  
|  | HRESULT CurrentSession | ( | [out, retval] [IWTXSession](interfaceIWTXSession.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns current Wintax4 Session [ReadOnly] |

**Note:**
      
| --- | --- |
  
|  | HRESULT CurrentUser | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the current Wintax4 User [Read / Write] |

**Note:**
     Setting a new value on this property will cause the immediate switch to the new user. The user must exist in the 'Users' folder in order to switch correctly. Reading the value of this property returns the name of the current User selected.   
| --- | --- |
  
|  | HRESULT DataDirectories | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns current Wintax4 Data directories [ReadOnly] |

**Note:**
     The Data directories are the folders in which archives are searched / browsed. This property is returned as an array of strings. Refer to the page [Sample: Directories](sample_directories.md) for a simple example on how to recover each single folder.   
| --- | --- |
  
|  | HRESULT Devices | ( | [in] LONG | _lAcq_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] VARIANT * | _pVal_ |
|  | ) |  |
|  | Returns the list of available devices on the network for specific acquisition [ReadOnly] |

**Parameters:**
|  | _lAcq_ | Index of the acquisition process. 0 for TCP process 1 for DST process. |
| --- | --- |
  
**Remarks:**
     Devices(0) (TCP) returns an array of available device names of TS11 type. Devices(1) (DST) returns an array of available DST devices or streams.   
  
|  | HRESULT EnableMergeImportData | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the enabled status of Merge Import Data flag in Data Browser. True (-1) if the Merge Import Data is enabled, else False (0). [Read / Write] |

**Note:**
      
| --- | --- |
  
|  | HRESULT ExportDirectory | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the current Wintax4 Export directory [ReadOnly] |

**Note:**
     The Export directory is the default folder used by export functions (export BIN, CSV, Pit System, etc.) Refer to the page [Sample: Directories](sample_directories.md) for a simple example.   
| --- | --- |
  
|  | HRESULT Exports | ( | [out, retval] [IWTXExports](interfaceIWTXExports.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | [IWTXExports](interfaceIWTXExports.md) represents the WinTAX interface for exports operations. [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT GlobalConditionsLibraries | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets Global Conditions libraries [Read / Write] |

**Note:**
    (see [IApplication::GlobalVirtualChannelsLibraries](interfaceIApplication.md#a48) )   
| --- | --- |
  
|  | HRESULT GlobalEventsLibraries | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets Global Events libraries [Read / Write] |

**Note:**
    (see [IApplication::GlobalVirtualChannelsLibraries](interfaceIApplication.md#a48) )   
| --- | --- |
  
|  | HRESULT GlobalVirtualChannelsLibraries | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets Global Virtual channels libraries [Read / Write] |

**Note:**
     The following example shows how to set global libraries from external files located in WinTAX Libraries directory 
    
    
    Dim sGlobalVCHLibs(1)
    sGlobalVCHLibs(0) = Application.LibrariesDirectory & "Global_lib1.vch"
    sGlobalVCHLibs(1) = Application.LibrariesDirectory & "Global_lib2.vch"
    'Set multiple libraries selection
    Application.GlobalVirtualChannelsLibraries = sGlobalVCHLibs
    'or
    'Set Single library selection
    Application.GlobalVirtualChannelsLibraries = Application.LibrariesDirectory & "Global_lib1.vch"
      
  
| --- | --- |
  
|  | HRESULT HostDirectories | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns current Wintax4 Host directories [ReadOnly] |

**Note:**
     The Host directories are the folders in which laps are copied by 'Copy to Host' task. This property is returned as an array of strings. Refer to the page [Sample: Directories](sample_directories.md) for a simple example on how to recover each single folder.   
| --- | --- |
  
|  | HRESULT ImportLapDirectories | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns current Wintax4 ImportLap directories [ReadOnly] |

**Note:**
     The ImportLap directories are the folders from which new laps are copied by 'Auto Import Lap' task. This property is returned as an array of strings. Refer to the page [Sample: Directories](sample_directories.md) for a simple example on how to recover each single folder.   
| --- | --- |
  
|  | HRESULT Layouts | ( | [out, retval] [IWTXLayouts](interfaceIWTXLayouts.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the collection of layouts configured in layout bar for current user. [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT LayoutViewers | ( | [out, retval] IWTXLayoutViewers ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the collection of layoutsviewers currently available [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT LibrariesDirectory | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the current Wintax4 Libraries directory [ReadOnly] |

**Note:**
     The Libraries directory is the folder in which VCH, DAT, EVN, ALM, CND files are stored. Refer to the page [Sample: Directories](sample_directories.md) for a simple example.   
| --- | --- |
  
|  | HRESULT Load | ( | [in] VARIANT | _vVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Performs a refresh of WinTAX Setup configuration |

**Parameters:**
|  | _vOpt_ | [in] Full path of Setup configuration file to load |
| --- | --- |
  
|  | HRESULT LoadGlobalParametersLibrary | ( | [in] BSTR | _bsVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Load new parameter configuration in Global Parameters libraries |

**Parameters:**
|  | _bsVal_ | Full path of Parameter configuration to load |
| --- | --- |
  
|  | HRESULT LoadLocalParametersLibrary | ( | [in] BSTR | _bsVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Load new parameter configuration in Local Parameters libraries |

**Parameters:**
|  | _bsVal_ | Full path of Parameter configuration to load |
| --- | --- |
  
|  | HRESULT LoadParameters | ( | [in] BSTR | _bsVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Load new parameter configuration in WinTAX4 |

**Parameters:**
|  | _bsVal_ | Full path of Parameter configuration to load |
| --- | --- |
  
**Note:**
     The following example shows how to load a parameters setup from external file located in WinTAX Libraries directory 
    
    
    Application.LoadParameters Application.LibrariesDirectory & "StdParameter.xml"
    
      
  
|  | HRESULT LocalConditionsLibraries | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets Current WinTAX4 user Conditions libraries paths [Read / Write] |

**Parameters:**
|  | _pVal_ | Single or array of strings containing libraries path |
| --- | --- |
  
**Note:**
    (see [IApplication::LocalVirtualChannelsLibraries](interfaceIApplication.md#a50) )   
  
|  | HRESULT LocalEventsLibraries | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets Current WinTAX4 user Events libraries paths [Read / Write] |

**Parameters:**
|  | _pVal_ | Single or array of strings containing libraries path |
| --- | --- |
  
**Note:**
    (see [IApplication::LocalVirtualChannelsLibraries](interfaceIApplication.md#a50) )   
  
|  | HRESULT LocalVirtualChannelsLibraries | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets Current WinTAX4 user Virtual channels libraries paths[Read / Write] |

**Parameters:**
|  | _pVal_ | Single or array of strings containing libraries path |
| --- | --- |
  
**Note:**
     The following example shows how to set current user libraries from external files located in WinTAX Libraries directory 
    
    
    Dim sUserVCHLibs(2)
    
    sUserVCHLibs(0) = Application.LibrariesDirectory & "User_lib1.vch"
    sUserVCHLibs(1) = Application.LibrariesDirectory & "User_lib2.vch"
    sUserVCHLibs(2) = Application.LibrariesDirectory & "User_lib3.vch"
    'Set multiple libraries selection
    Application.LocalVirtualChannelsLibraries = sUserVCHLibs
    'or
    'Set Single library selection
    Application.LocalVirtualChannelsLibraries = Application.LibrariesDirectory & "User_lib1.vch"
    
      
  
|  | HRESULT LogDirectory | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the current Wintax4 Log directory [ReadOnly] |

**Note:**
     The Log directory is the folder in which log files are generated. Refer to the page [Sample: Directories](sample_directories.md) for a simple example.   
| --- | --- |
  
|  | HRESULT LogText | ( | [in] BSTR | _sLog_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Adds a text to the LOG window. |

**Parameters:**
|  | _sLog_ | [in] Text to add to the log window. |
| --- | --- |
  
|  | HRESULT LookupsDirectory | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the current Wintax4 Lookup files directory |

**Note:**
     When defined, this directory takes priority over the Libraries directory in finding DAT files. 

**Remarks:**
     The property is available only on some licenses   
| --- | --- |
  
|  | HRESULT MathDLLPlugInDirectory | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the current Wintax4 MathDLL PlugIn files directory |

**Note:**
     When defined, this directory takes priority over the PlugIns\Math directory in loading MathDLL PlugIn files. 

**Remarks:**
     The property is available only on some licenses   
| --- | --- |
  
|  | HRESULT MatlabApplication | ( | [out, retval] IDispatch ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the Matlab application interface used by WinTAX in Matlab export (e.g. Open In Matlab functions ) [Read] |

**Note:**
     Refer to [Sample Code](matlab_integration.md#sample_matlab) for a simple example on how to manage a Matlab application inside WinTAX   
| --- | --- |
  
|  | HRESULT OffsetGainMode | ( | [out, retval] LONG * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets current Offset & Gain setup mode used in WinTAX analysis [Read / Write] |

**Remarks:**
     Could be one of following values: 0 Off 1 File Table 2 Parameters settings 3 File Table and Parameters   
| --- | --- |
  
|  | HRESULT OLEAutoUpdate | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the WinTAX4 automatic update after changes made by OLE automation. True (-1) if the update is enabled, else False (0). [Read / Write] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Open | ( | [in] VARIANT | _vVal_ , |
| --- | --- | --- | --- |
|  |  | [in, optional] VARIANT | _vDataset_ |
|  | ) |  |
|  | Opens an existing archive(s) in the WinTAX |

**Parameters:**
|  | _vVal_ | [in] According to the object passed to the method, a variety of different archives and modalities may be handled: **\- Single** The complete file path of the archive to load (eg. "\\\PC33\Data\Track\Car_05\Run_001\Lap_07\cabledata.ztx") **\- Append** An array of TelLap or a TelLaps object filled with lap's complete path **\- Compare** An array of TelLaps ( compare of appends ) or a generic array of TelLap ( compare of single laps ) filled with lap's complete path **\- Empty** If param is set to nothing or empty string, a new empty lap is opened |
| --- | --- |
| _vDataset_ | [in] A set of UINT values (in the range 1 to 10) such that the i-th value represents the dataset index where the i-th Laps selection should be loaded. According to the number of comparisons, dataset index(es) may be passed as either an UINT value or an array of UINT values with as many elements as the comparison list. |
  
**Note:**
     The archive will be loaded in Wintax4 main window, and will be available via scripting by accessing the [IApplication::CurrentData](interfaceIApplication.md#a6) property. 

The array of dataset indices will be checked for a number of issues: \- If the number of indices is greater than the number of comparisons, extra indices will be discarded \- If the number of indices is lower than the number of comparisons, the array will be filled up adding indices so that each added index is the smallest possible not yet present \- If there are duplicate indices, the array will be discarded and data will be loaded in default dataset positions \- If there indices outside the admitted range (1 - 10), the array will be discarded and data will be loaded in default dataset positions. 

**Remarks:**
     Refer to [Sample: Open data in WinTAX](application_open.md) for a list of available load operations.   
  
|  | HRESULT ProgramDirectory | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the current Wintax4 program directory [ReadOnly] |

**Note:**
     The program directory is the folder in which the main executable is run. Refer to the page [Sample: Directories](sample_directories.md) for a simple example.   
| --- | --- |
  
|  | HRESULT Properties | ( | [out, retval] [ITelProperties](interfaceITelProperties.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | [ITelProperty](interfaceITelProperty.md) collection that represents a set of properties that could be used to store generic information and make it available in all user scripts. [Read / Write] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Refresh | ( | [in] VARIANT | _vOpt_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Performs a refresh of the data loaded in WinTAX |

**Parameters:**
|  | _vOpt_ | [in] Refresh mode. Could be one of following values 1 Refreshes all data loaded in WinTAX4 reloading the current data set. 2 Refreshes virtual channels reloading all the virtual channels libraries. 3 Refreshes lookup table files and update virtual channels values. |
| --- | --- |
  
|  | HRESULT Run | ( | [in] BSTR | _sCommand_ , |
| --- | --- | --- | --- |
|  |  | [in, optional] VARIANT | _vArg1_ , |
|  |  | [in, optional] VARIANT | _vArg2_ , |
|  |  | [in, optional] VARIANT | _vArg3_ , |
|  |  | [in, optional] VARIANT | _vArg4_ , |
|  |  | [in, optional] VARIANT | _vArg5_ , |
|  |  | [in, optional] VARIANT | _vArg6_ , |
|  |  | [in, optional] VARIANT | _vArg7_ , |
|  |  | [in, optional] VARIANT | _vArg8_ , |
|  |  | [in, optional] VARIANT | _vArg9_ , |
|  |  | [in, optional] VARIANT | _vArg10_ |
|  | ) |  |
|  | Executes a command available in Wintax4 main menu. |

**Parameters:**
|  | _sCommand_ | [in] The compressed text of the command to run (eg. "OpenData") |
| --- | --- |
| _vArg1_ | [in, optional] Optional argument, depending on the command to run (not used in this release) |
| _vArg2_ | [in, optional] Optional argument, depending on the command to run (not used in this release) |
| _vArg3_ | [in, optional] Optional argument, depending on the command to run (not used in this release) |
| _vArg4_ | [in, optional] Optional argument, depending on the command to run (not used in this release) |
| _vArg5_ | [in, optional] Optional argument, depending on the command to run (not used in this release) |
| _vArg6_ | [in, optional] Optional argument, depending on the command to run (not used in this release) |
| _vArg7_ | [in, optional] Optional argument, depending on the command to run (not used in this release) |
| _vArg8_ | [in, optional] Optional argument, depending on the command to run (not used in this release) |
| _vArg9_ | [in, optional] Optional argument, depending on the command to run (not used in this release) |
| _vArg10_ | [in, optional] Optional argument, depending on the command to run (not used in this release) |
  
**Remarks:**
     The command is run exactly as it was selected by the user, so it can open windows and other GUI elements depending on the status of the main application. Refer to [Run method: commands available](application_run_commands.md) for a list of available commands. 

The command is run exactly as it was selected by the user, so it can open window and other GUI elements depending on the status of the main application. Refer to [Run method: commands available](application_run_commands.md) for a list of available commands.   
  
|  | HRESULT RunMacro | ( | [in] BSTR | _sCommand_ , |
| --- | --- | --- | --- |
|  |  | [in, optional] VARIANT | _vArg1_ , |
|  |  | [in, optional] VARIANT | _vArg2_ , |
|  |  | [in, optional] VARIANT | _vArg3_ , |
|  |  | [in, optional] VARIANT | _vArg4_ , |
|  |  | [in, optional] VARIANT | _vArg5_ , |
|  |  | [in, optional] VARIANT | _vArg6_ , |
|  |  | [in, optional] VARIANT | _vArg7_ , |
|  |  | [in, optional] VARIANT | _vArg8_ , |
|  |  | [in, optional] VARIANT | _vArg9_ , |
|  |  | [in, optional] VARIANT | _vArg10_ |
|  | ) |  |
|  | Runs a macro configured in WinTAX General script site Executes a command available in Wintax4 main menu. |

**Parameters:**
|  | _sCommand_ | [in] The compressed text of the command to run (eg. "OpenData") |
| --- | --- |
| _vArg1_ | [in, optional] Optional argument, depending on the macro to run |
| _vArg2_ | [in, optional] Optional argument, depending on the macro to run |
| _vArg3_ | [in, optional] Optional argument, depending on the macro to run |
| _vArg4_ | [in, optional] Optional argument, depending on the macro to run |
| _vArg5_ | [in, optional] Optional argument, depending on the macro to run |
| _vArg6_ | [in, optional] Optional argument, depending on the macro to run |
| _vArg7_ | [in, optional] Optional argument, depending on the macro to run |
| _vArg8_ | [in, optional] Optional argument, depending on the macro to run |
| _vArg9_ | [in, optional] Optional argument, depending on the macro to run |
| _vArg10_ | [in, optional] Optional argument, depending on the macro to run |
  
**Note:**
     The following example shows how to call a function configured in General Script both from VB call and from [IApplication::RunMacro](interfaceIApplication.md#a57)
    
    
    Function DoMessageTest( string1, string2 )
      MsgBox string1 & "-" & string2
    End Function
    
    Sub CallTestFromVB
      DoMessageTest "Test", "Me"
    End Sub
    
    Sub CallTestFromApplication
      Application.RunMacro "DoMessageTest", "Test", "Me"
    End Sub
      
  
|  | HRESULT SpecSheet | ( | [out, retval] [IWTXSpecSheet](interfaceIWTXSpecSheet.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | [IWTXSpecSheet](interfaceIWTXSpecSheet.md) interface that represents the working copies loaded in the spec sheet editor. [Read] |

**Remarks:**
     SpecSheet interface is available only when SpecSheet Editor is open. 

**Note:**
     Spec sheet contains a certain number of ITelWorkingCopy.   
| --- | --- |
  
|  | HRESULT StartAcquisition | ( | [in] LONG | _lAcq_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Starts the acquisition process. |

**Parameters:**
|  | _lAcq_ | Index of the acquisition process. 0 for TCP process 1 for DST process. |
| --- | --- |
  
**Note:**
     Use [IApplication::AcquisitionStarted](interfaceIApplication.md#a37) property to check if the process is started.   
  
|  | HRESULT StopAcquisition | ( | [in] LONG | _lAcq_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Stops the acquisition process. |

**Parameters:**
|  | _lAcq_ | Index of the acquisition process. 0 for TCP process 1 for DST process. |
| --- | --- |
  
**Note:**
     Use [IApplication::AcquisitionStarted](interfaceIApplication.md#a37) property to check if the process is started.   
  
|  | HRESULT UserDirectory | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the current Wintax4 User directory [ReadOnly] |

**Note:**
     The user directory is the folder in which the **current** user configuration files are stored (eg. "c:\Wintax4\users\foo\"). Refer to [IApplication::CurrentUser](interfaceIApplication.md#a9) to change current user. Refer to the page [Sample: Directories](sample_directories.md) for a simple example.   
| --- | --- |
  
|  | HRESULT Users | ( | [out, retval] [IWTXUsers](interfaceIWTXUsers.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the collection of WinTAX users [Read] |

**Note:**
     The following example shows how to set current user as "super" user 
    
    
    Set oUser = Application.Users( Application.CurrentUser )
    oUser.Level = 2
    Application.CurrentUser = oUser.Name
      
  
| --- | --- |
  
|  | HRESULT Visible | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the visibility of the Wintax4 main window. True (-1) if the application is visible, else False (0). [Read / Write] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Windows | ( | [out, retval] [ICollectWindows](interfaceICollectWindows.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns a [ICollectWindows](interfaceICollectWindows.md) collection that represents the opened windows in the application. |

**Note:**
     The collection is empty if there are no open windows in the application frame.   
---|---