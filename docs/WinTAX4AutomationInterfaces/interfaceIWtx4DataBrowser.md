[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWtx4DataBrowser Interface Reference

The IWtx4DataBrowser interface exposes methods and properties of a WinTAX Data Browser object. More...

[List of all members.](interfaceIWtx4DataBrowser-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Open](interfaceIWtx4DataBrowser.md#a0) ([in] VARIANT vOptions,[in] VARIANT vData,[out, retval] long *lResult)  
HRESULT | [Caption](interfaceIWtx4DataBrowser.md#a1) ([out, retval] BSTR *pVal)  
HRESULT | [Visible](interfaceIWtx4DataBrowser.md#a3) ([out, retval] BOOL *pVal)  
HRESULT | [InitialPath](interfaceIWtx4DataBrowser.md#a5) ([out, retval] BSTR *pVal)  
HRESULT | [Session](interfaceIWtx4DataBrowser.md#a7) ([out, retval] BSTR *pVal)  
HRESULT | [IsSelectionValid](interfaceIWtx4DataBrowser.md#a8) ([out, retval] BOOL *pVal)  
HRESULT | [NumComparisons](interfaceIWtx4DataBrowser.md#a9) ([out, retval] long *pVal)  
HRESULT | [NumLaps](interfaceIWtx4DataBrowser.md#a10) ([in] long lComparison,[out, retval] long *pVal)  
HRESULT | [FileName](interfaceIWtx4DataBrowser.md#a11) ([in] long lComparison,[in] long lLap,[out, retval] BSTR *pVal)  
HRESULT | [Lap](interfaceIWtx4DataBrowser.md#a12) ([in] long lComparison,[in] long lLap,[out, retval] long *pVal)  
HRESULT | [CurrentLapFilterData](interfaceIWtx4DataBrowser.md#a22) ([out, retval] BSTR *pVal)  
HRESULT | [Position](interfaceIWtx4DataBrowser.md#a24) ([in] long lComparison,[out, retval] long *pVal)  
HRESULT | [SelectionLapFilter](interfaceIWtx4DataBrowser.md#a26) ([in] long lComparison,[out, retval] BSTR *pVal)  
HRESULT | [NumReferenceLaps](interfaceIWtx4DataBrowser.md#a27) ([out, retval] long *pVal)  
| Returns the number of reference lap [Read].   
HRESULT | [ReferencePathLap](interfaceIWtx4DataBrowser.md#a28) ([in] long lLap,[out, retval] BSTR *pVal)  
HRESULT | [ReferenceLapIndex](interfaceIWtx4DataBrowser.md#a29) ([in] long lLap,[out, retval] long *pVal)  
HRESULT | [ReferenceLapSel](interfaceIWtx4DataBrowser.md#a30) ([in] BSTR bSingleLapArray)  
| Set the reference lap selection.   
HRESULT | [VideoLoadingStatus](interfaceIWtx4DataBrowser.md#a31) ([out, retval] BOOL *pVal)  
| return the video loading flag status   
HRESULT | [AVGMode](interfaceIWtx4DataBrowser.md#a36) ([in] long lComparison,[out, retval] BOOL *pVal)  
| return the avg mode flag status   
HRESULT | [DistanceMode](interfaceIWtx4DataBrowser.md#a37) ([out, retval] BOOL *pVal)  
| return the distance mode avg calc flag status   
HRESULT | [CurrentLapSort](interfaceIWtx4DataBrowser.md#a38) ([out, retval] BSTR *pVal)  
| Returns current sort settings (XML format) [Read].   
HRESULT | [MergeImportDataEnabled](interfaceIWtx4DataBrowser.md#a39) ([out, retval] BOOL *pVal)  
| return the video loading flag status   
HRESULT | [CurrentDatabase](interfaceIWtx4DataBrowser.md#a40) ([out, retval] BSTR *pVal)  
| return the name of the current database   
HRESULT | [CurrentLapSelection](interfaceIWtx4DataBrowser.md#a41) ([out, retval] VARIANT *pVal)  
HRESULT | [AutoClearLapsSelectionEnabled](interfaceIWtx4DataBrowser.md#a42) ([out, retval] BOOL *pVal)  
| return the auto clear comparison flag status   
HRESULT | [ImportVideoHSC](interfaceIWtx4DataBrowser.md#a44) ([in] BSTR bSourceFilePath,[in] BSTR bDestRunPath,[out, retval] VARIANT *pVal)  
HRESULT | [DataBrowserPlugInReadModeEnabled](interfaceIWtx4DataBrowser.md#a45) ([out, retval] BOOL *pVal)  
| return the DataBrowser PlugIn ReadMode flag status   
HRESULT | [DataBrowserPlugInSelection](interfaceIWtx4DataBrowser.md#a46) ([out, retval] VARIANT *vVal)  
| return the DataBrowser PlugIn ReadMode selection lap information   
HRESULT | [DataBrowserPlugInSelectionProperties](interfaceIWtx4DataBrowser.md#a47) ([in] VARIANT v1,[out, retval] VARIANT *vValProperties)  
| return the DataBrowser PlugIn ReadMode selection lap properties information   
HRESULT | [DataSetEnableMode](interfaceIWtx4DataBrowser.md#a48) ([in] DWORD newVal)  
HRESULT | [EnableMergeImportData](interfaceIWtx4DataBrowser.md#a49) ([in] BOOL newVal)  
| Sets the merge import data flag status. [Write].   
HRESULT | [RTComparedCustomLapPath](interfaceIWtx4DataBrowser.md#a50) ([out, retval] BSTR *pVal)  
| Returns or sets the archive filename corresponding to custom Real Time Comparison Lap.   
HRESULT | [RTComparedMode](interfaceIWtx4DataBrowser.md#a52) ([out, retval] BYTE *pVal)  
| Returns or sets Real Time Comparison mode.   
HRESULT | [UseAliasGroupLabel](interfaceIWtx4DataBrowser.md#a54) ([in] BYTE newVal)  
|  | Sets Use Alias Group Label. |
  
* * *

## Detailed Description

The IWtx4DataBrowser interface exposes methods and properties of a WinTAX Data Browser object. 

The Application Object represents the entire DataBrowser application. The DataBrowser object contains browser settings, options and methods. 

**Note:**
    

* * *

## Member Function Documentation

|  | HRESULT Caption | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Specifies the text displayed in the data browser caption. (Read / Write) |

**Note:**
      
| --- | --- |
  
|  | HRESULT CurrentLapFilterData | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns current lap filter (XML format) [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT CurrentLapSelection | ( | [out, retval] VARIANT * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns current selection on DataBrowser lap list window [Read] |

**Remarks:**
     CurrentLapSelection property does not return the complete lap selection in case of comparison. To obtain the whole selection you must loop all comparison lap entries as follow: 
    
    
    Sub DumpDataSelection()
      If Browser.IsSelectionValid Then
        numCompare = Browser.NumComparisons
        For iCompare = 0 To numCompare - 1
          numLaps = Browser.numLaps(iCompare)
          For iLap = 0 To numLaps - 1
            Application.LogText( Browser.Filename(iCompare, iLap) )
          Next
        Next
      End If
    End Sub
    
      
  
| --- | --- |
  
|  | HRESULT DataSetEnableMode | ( | [in] DWORD | _newVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Sets Data Sets Enabling Mode of the Data Browser main window. [Write] |

**Note:**
      
| --- | --- |
  
|  | HRESULT FileName | ( | [in] long | _lComparison_ , |
| --- | --- | --- | --- |
|  |  | [in] long | _lLap_ , |
|  |  | [out, retval] BSTR * | _pVal_ |
|  | ) |  |
|  | Returns the archive filename corresponding to each lap in a comparison. [Read] |

**Parameters:**
|  | _lComparison_ | [in] Index of comparison |
| --- | --- |
| _lLap_ | [in] Index of Lap |
  
**Note:**
      
  
|  | HRESULT ImportVideoHSC | ( | [in] BSTR | _bSourceFilePath_ , |
| --- | --- | --- | --- |
|  |  | [in] BSTR | _bDestRunPath_ , |
|  |  | [out, retval] VARIANT * | _pVal_ |
|  | ) |  |
|  | Imports a Video file from h264 format to an avi file for HSC format, in a specified Run directory |

**Parameters:**
|  | _bSourceFilePath_ | [in] File path for source HSC video file to import (*.h264, *.avi) |
| --- | --- |
| _bDestRunPath_ | [in] Path for destination (Run) directory |
| _pVal_ | [out] pointer for storing result code. Could be one of following values: -1 Generic unknown error occurred, or feature is not available, 0 Operation completed successfully, 1 First parameter is an empty string, or source file doesn't exist, 2 Second parameter is an empty string, or destination path doesn't exist, 3 Failed copying or converting source video file to avi format, 4 Conversion tool file failed converting video metadata, or conversion tool file doesn't exist, 5 Failed loading video metadata, or no matching lap has been found for source video file, 6 Failed storing video header metadata in destination avi file. |
  
**Remarks:**
     This function is available only in versions with Video HSC feature. Example: 
    
    
    Sub Test_Importing_Video_HSC()
     Dim csVideoSource, csRunPath
     Dim result
     
     csVideoSource = "C:\Video\Video.h264"
     csRunPath = "C:\Wintax4\Data\Track\Session\Car\Run"
     result = Browser.ImportVideoHSC( csVideoSource, csRunPath )
     msgbox( result )
    End Sub
      
  
|  | HRESULT InitialPath | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the initial path for Data Session browsing. [Read / Write] |

**Remarks:**
     Call [Open()](interfaceIWtx4DataBrowser.md#a0) method after each change of this property in order to to apply the directory scan.   
| --- | --- |
  
|  | HRESULT IsSelectionValid | ( | [out, retval] BOOL * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the selection state. True if one or more comparisons are selected. [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Lap | ( | [in] long | _lComparison_ , |
| --- | --- | --- | --- |
|  |  | [in] long | _lLap_ , |
|  |  | [out, retval] long * | _pVal_ |
|  | ) |  |
|  | Returns the lap index (in the archive) corresponding to each lap in a comparison. [Read] |

**Parameters:**
|  | _lComparison_ | [in] Index of comparison |
| --- | --- |
| _lLap_ | [in] Index of Lap |
  
**Note:**
      
  
|  | HRESULT NumComparisons | ( | [out, retval] long * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of comparisons selected. [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT NumLaps | ( | [in] long | _lComparison_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] long * | _pVal_ |
|  | ) |  |
|  | Returns the number of laps selected in a comparison. [Read] |

**Parameters:**
|  | _lComparison_ | [in] Index of comparison |
| --- | --- |
  
|  | HRESULT Open | ( | [in] VARIANT | _vOptions_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _vData_ , |
|  |  | [out, retval] long * | _lResult_ |
|  | ) |  |
|  | Opens the DataBrowser, if not already opened |

**Remarks:**
     To make an automatic directory scan, the property [InitialPath()](interfaceIWtx4DataBrowser.md#a5) has to be set with the desired start path. 

**Parameters:**
|  | _vOptions_ | [in] Options for visualization **[not yet implemented]** |
| --- | --- |
| _vData_ | [in] Options for directory management **[not yet implemented]** |
  
|  | HRESULT Position | ( | [in] long | _lComparison_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] long * | _pVal_ |
|  | ) |  |
|  | Returns the position in the dataset list of laps selected in a comparison. [Read] |

**Parameters:**
|  | _lComparison_ | [in] Index of comparison |
| --- | --- |
  
|  | HRESULT ReferenceLapIndex | ( | [in] long | _lLap_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] long * | _pVal_ |
|  | ) |  |
|  | Returns the lap index (in the archive) corresponding to each lap in reference lap. [Read] |

**Parameters:**
|  | _lLap_ | [in] Index of Lap |
| --- | --- |
  
|  | HRESULT ReferencePathLap | ( | [in] long | _lLap_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] BSTR * | _pVal_ |
|  | ) |  |
|  | Returns the archive filename corresponding to each lap in a reference lap. [Read] |

**Parameters:**
|  | _lLap_ | [in] Index of Lap |
| --- | --- |
  
|  | HRESULT SelectionLapFilter | ( | [in] long | _lComparison_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] BSTR * | _pVal_ |
|  | ) |  |
|  | Returns lap filter of the selected lap (XML format) [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Session | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the start path of current Session list. [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT Visible | ( | [out, retval] BOOL * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns or sets the visibility of the Data Browser main window. True if the application is visible. [Read / Write] |

**Note:**
      
---|---