## Introduction

### Scripting: Interfaces for WinTAX4 Automation

WinTAX4 provides powerful possibilities for interfacing with external applications using the Automation Server technology (formerly OLE Automation Server). Automation is a protocol which allows an application to make its own objects available for use in other applications, programming tools or via scripting languages.

In this way WinTAX4 can be run and controlled by any program which has the characteristics of Automation controller. Some examples of applications which make great use of Automation are Microsoft Excel, Access, Project and many others written in Visual Basic or Visual C++.

It is possible, for example, to open a WinTAX4 window from an Excel spreadsheet, analyze information via Matlab or run print or copy commands directly from an application written in Visual Basic.

### Active Scripting inside WinTAX4

WinTAX4 contains an engine to run scripts written in standard interpreted languages (VBScript and JScript). Select the script language to be used from General/Setup/Miscellaneous/Application. Scripts are custom applications (macro functions) that can be manually launched from the toolbars or can be enabled as a result of system events (acquisition of a new lap, loading of data, alarm or condition found in the data).

The scripts written by the customer can be used to search for data or conditions, to launch WinTAX4 environment commands, to enter additional information automatically calculated into the archives or to transfer information from external applications to WinTAX4 and vice versa.

All WinTAX4 objects exposed through OLE Automation can be viewed and managed through scripting. Moreover, through scripting, external applications can be called to obtain information, analyze data and, in general, use other functions that are not present in WinTAX4.

WinTAX4 contains a compiler for two different languages:

  * VBScript

  * JScripts




To modify the supported language run Setup / General from main menu, select General Options page in General Setup window, in section Application select language in Script Language field

The default is VBScript