[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# _IApplicationEvents Interface Reference

Use this interface to respond to application and state changes. More...

[List of all members.](interfaceWintax4Lib_1_1__IApplicationEvents-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [OnCreate](interfaceWintax4Lib_1_1__IApplicationEvents.md#a0) ()  
HRESULT | [OnExitApplication](interfaceWintax4Lib_1_1__IApplicationEvents.md#a1) ()  
HRESULT | [OnLoadData](interfaceWintax4Lib_1_1__IApplicationEvents.md#a2) ([in]BSTR bsFileName)  
HRESULT | [OnSettingChange](interfaceWintax4Lib_1_1__IApplicationEvents.md#a3) ()  
HRESULT | [OnNewDataRealTime](interfaceWintax4Lib_1_1__IApplicationEvents.md#a4) ()  
HRESULT | [OnNewLap](interfaceWintax4Lib_1_1__IApplicationEvents.md#a5) ([in] BSTR sFile)  
HRESULT | [OnAutoRX](interfaceWintax4Lib_1_1__IApplicationEvents.md#a6) ([in] BSTR sFile)  
HRESULT | [OnAutoRXRemote](interfaceWintax4Lib_1_1__IApplicationEvents.md#a7) ([in] BSTR sFile)  
HRESULT | [OnAutoImportLap](interfaceWintax4Lib_1_1__IApplicationEvents.md#a8) ([in] BSTR sFile)  
HRESULT | [OnAutoCopyToHost](interfaceWintax4Lib_1_1__IApplicationEvents.md#a9) ([in]BSTR sFileSource,[in]BSTR sFileDest, LONG lResult)  
HRESULT | [OnAutoAlarmFound](interfaceWintax4Lib_1_1__IApplicationEvents.md#a10) ([in] BSTR sChan,[in] double dMin,[in] double dMax)  
HRESULT | [OnEvent](interfaceWintax4Lib_1_1__IApplicationEvents.md#a11) ([in] BSTR sChan,[in] VARIANT dTime)  
HRESULT | [OnNewLoggingTable](interfaceWintax4Lib_1_1__IApplicationEvents.md#a12) ([in] LONG lAcq)  
HRESULT | [OnBeginUpdate](interfaceWintax4Lib_1_1__IApplicationEvents.md#a13) ()  
HRESULT | [OnEndUpdate](interfaceWintax4Lib_1_1__IApplicationEvents.md#a14) ()  
HRESULT | [OnDownloadEnd](interfaceWintax4Lib_1_1__IApplicationEvents.md#a15) ([in] LONG lAcq)  
| HRESULT | [OnCursorPosChange](interfaceWintax4Lib_1_1__IApplicationEvents.md#a16) ([in]VARIANT TimePos) |
  
* * *

## Detailed Description

Use this interface to respond to application and state changes. 

The IApplicationEvents interface is a callback interface to which Wintax4 sends event notifications. 

* * *

## Member Function Documentation

|  | HRESULT OnAutoAlarmFound | ( | [in] BSTR | _sChan_ , |
| --- | --- | --- | --- |
|  |  | [in] double | _dMin_ , |
|  |  | [in] double | _dMax_ |
|  | ) |  |
|  | The OnAutoAlarmFound method receives an event notice indicating that an alarm was found during the 'Auto Search Alarms' operation. |

**Parameters:**
|  | _sChan_ | [in] Name of the alarmed channel. |
| --- | --- |
| _dMin_ | [in] Minimum value found. |
| _dMax_ | [in] Maximum value found. |
  
**Remarks:**
     The OnAutoAlarmFound event is sent only when 'Auto Search Alarms' is enabled and when the search is performed. The search is done only in post processing mode on current laps.   
  
|  | HRESULT OnAutoCopyToHost | ( | [in] BSTR | _sFileSource_ , |
| --- | --- | --- | --- |
|  |  | [in] BSTR | _sFileDest_ , |
|  |  | LONG | _lResult_ |
|  | ) |  |
|  | The OnAutoCopyToHost method receives an event notice indicating that a 'Copy to Host' operation has been completed. |

**Parameters:**
|  | _sFileSource_ | [in] Source file path of the copied file. |
| --- | --- |
| _sFileDest_ | [in] Destination file path of the copied file. |
| _lResult_ | [in] Result of the copy operation. 0 means that the copy failed. 1 means that the copy succeeded. |
  
**Remarks:**
     OnAutoCopyToHost event is sent either if the copy operation is successfull or is not. Check lResult parameter to determine if the operation was successful.   
  
|  | HRESULT OnAutoImportLap | ( | [in] BSTR | _sFile_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The OnAutoImportLap method receives an event notice indicating that a new lap has been opened by WinTAX Auto Import Lap function. |

**Parameters:**
|  | _sFile_ | [in] File path of the ZTX lap. |
| --- | --- |
  
**Remarks:**
     Notification is sent only if 'Auto Import Lap' is enabled and not paused. Scripts can process OnAutoImportLap in order to make automated post processing elaborations or automated data exports.   
  
|  | HRESULT OnAutoRX | ( | [in] BSTR | _sFile_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The OnAutoRX method receives an event notice indicating that a new lap has been closed by an acquisition process and opened by WinTAX. |

**Parameters:**
|  | _sFile_ | [in] File path of the ZTX lap. |
| --- | --- |
  
**Remarks:**
     Notification is sent only if 'Auto RX' is enabled and not paused. Scripts can process OnAutoRX in order to make automated post processing elaborations or automated data exports. Notification is sent either for Cable or DST laps.   
  
|  | HRESULT OnAutoRXRemote | ( | [in] BSTR | _sFile_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The OnAutoRXRemote method receives an event notice indicating that a new lap has been opened by WinTAX Auto RX Remote function. |

**Parameters:**
|  | _sFile_ | [in] File path of the ZTX lap. |
| --- | --- |
  
**Remarks:**
     Notification is sent only if 'Auto RX Remote' is enabled and not paused. Scripts can process OnAutoRXRemote in order to make automated post processing elaborations or automated data exports.   
  
|  | HRESULT OnBeginUpdate | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | The OnBeginUpdate method receives an event notice indicating that WinTAX is about to update windows ( e.g. with a new set of data or new setup ). |

**Note:**
      
| --- | --- |
  
|  | HRESULT OnCreate | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | The OnCreate method receives an event notice indicating that the Wintax4 application has been created. |

**Note:**
      
| --- | --- |
  
|  | HRESULT OnCursorPosChange | ( | [in] VARIANT | _TimePos_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The OnCursorPosChange method receives an event notice indicating that the current window cursor has been moved. |

**Parameters:**
|  | _TimePos_ | [in] New cursor position. |
| --- | --- |
  
**Remarks:**
     OnCursorPosChange can be used to customize some cursor operations, such as Snapping, Auto Connect and so on .. When 'Relative Time' mode is selected, cursor position is expressed in seconds. When 'Absolute Time' is selected, cursor position is a string formatted as "HH:MM:SS.mmm".   
  
|  | HRESULT OnDownloadEnd | ( | [in] LONG | _lAcq_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The OnDownloadEnd method receives an event notice indicating that the download is completed. |

**Parameters:**
|  | _lAcq_ | [in] Index of the acquisition process. 0 for TCP Acquisition 2 for MARVEL Acquisition 3 for FLASHCARD Acquisition 7 for DVR Acquisition. |
| --- | --- |
  
**Remarks:**
     This event is fired only when overall data download is completed. It is never triggered during RealTime Acquisition.   
  
|  | HRESULT OnEndUpdate | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | The OnEndUpdate method receives an event notice indicating that WinTAX has finished to update windows ( e.g. with a new set of data or new setup ). |

**Note:**
      
| --- | --- |
  
|  | HRESULT OnEvent | ( | [in] BSTR | _sChan_ , |
| --- | --- | --- | --- |
|  |  | [in] VARIANT | _dTime_ |
|  | ) |  |
|  | The OnEvent method receives an event notice indicating that an event occurred |

**Parameters:**
|  | _sChan_ | [in] Name of the event. |
| --- | --- |
| _dTime_ | [in] Time of the event occurrence, expressed in seconds (relative time). |
  
**Remarks:**
     This event is fired only during RealTime acquisition.   
  
|  | HRESULT OnExitApplication | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | The OnExitApplication method receives an event notice indicating that Wintax4 is going to close. This event is fired after the user has confirmed to close WinTAX and immediately before destroying all internal windows and objects. |

**Note:**
     After receiving this event you must release all WinTAX interfaces as they will be no longer valid.   
| --- | --- |
  
|  | HRESULT OnLoadData | ( | [in] BSTR | _bsFileName_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The OnLoadData method receives an event notice indicating that new data has been loaded in the current dataset. |

**Parameters:**
|  | _bsFileName_ | [in] File path of the first selected file of the new selection. [IApplication::CurrentData](interfaceIApplication.md#a6) can be used to reference the complete selection. |
| --- | --- |
  
**Note:**
      
  
|  | HRESULT OnNewDataRealTime | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | The OnNewDataRealTime method receives an event notice indicating that new realtime samples have been acquired. Scripts can process RealTime samples by using the [IApplication::CurrentDataRealTime()](interfaceIApplication.md#a7) object. The following example searches data during realtime acquisition : |
    
    
    Sub Application_OnNewDataRealTime()
    
            Dim oData, oChan, iValue, iValueOld
    
            Set oData = Application.CurrentDataRealTime
            Set oChan = oData.Channels("TIME")
    
            iValue = oChan.Value(oData.Time )
            iValueOld = oChan.Value(oData.Time - 0.05)
     
            If iValueOld > iValue Then
    
                            If iValueOld <> "-1,#QNAN" Then
                                    If iValue <> "-1,#QNAN" Then
                                            Msgbox "DSTCloseLap" & " LapTime=" & iValueOld
                                    End If  
                            End If
            End If
    
    End Sub
    

**Remarks:**
     Notification is sent at a maximum rate of 10 times per second and only when new data is acquired from DST process.   
| --- | --- |
  
|  | HRESULT OnNewLap | ( | [in] BSTR | _sFile_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The OnNewLap method receives an event notice indicating that a new lap has been closed by an acquisition process. |

**Parameters:**
|  | _sFile_ | [in] File path of the ZTX lap. |
| --- | --- |
  
**Remarks:**
     Notification is sent even though 'Auto RX' is disabled. Scripts can process OnNewLap in order to make automated post processing elaborations or automated data exports. Notification is sent either for Cable or DST laps.   
  
|  | HRESULT OnNewLoggingTable | ( | [in] LONG | _lAcq_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The OnNewLoggingTable method receives an event notice indicating that the Real Time logging table is changed |

**Parameters:**
|  | _lAcq_ | [in] Real Time acquisition type. Could be: **[1]** DST Acquisition |
| --- | --- |
  
**Remarks:**
     This event is fired only during RealTime acquisition.   
  
|  | HRESULT OnSettingChange | ( |  | ) |  |
| --- | --- | --- | --- | --- |
|  | The OnSettingChange method receives an event notice indicating that the application settings are changed. |

**Remarks:**
     The following settings are notified : \- Change in 'Setup->General' configuration. \- Change in 'Setup->Users->Configure' configuration. \- Change in 'Configure->Parameters' configuration. \- Change in 'Configure->Virtual Channels' configuration. \- Change in 'Configure->Alarms' configuration. \- Change in 'Configure->Conditions' configuration. \- Change in 'Configure->Events' configuration. \- Change in 'Configure->Aliases' configuration. \- Change in 'Configure->Enums' configuration.   
---|---