[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# Examples

The examples are installed in "wtx4dir\Samples", where wtx4dir is the location of your WinTAX4 installation. 

##  Visual C++ Example: WTX4AutomationSample

This example requires Microsoft Visual Studio® Version 8 in order to run. The WTX4AutomationSample demonstrates how to use both and Wtx4DataInterfaces to access a WinTAX4 data archive, and WinTax4 Automation loading prevoius selected lap and creating a simple ole channels that works in Post Processing and Real Time. 

  * The "Archive Path" textbox is filled with all archive directories configured in WinTAX4. 
  * The "Session List" area is filled with all session inside selected archive 
  * The "Lap List" area is filled with all lap present in selected session 
  * The "Channel list" area is filled with all channels present in selected lap 

When the user double clicks in the "Lap List" area, the selected lap is loaded into WinTAX. A new Ole Channel "WTX4AutomationSample_OLEChannel" is created for each data selection and real time data is set when WinTAX events are fired. 

##  Visual C++ Example: WinTAXInPlaceDemo

This example requires Microsoft Visual Studio® Version 8 in order to run. The WinTAXInPlaceDemo demonstrates how to create an OleControl that can be placed inside a WinTAX Object control window in order to perform operations and receive notification events with automation. 

  * The "Load" button allows the user to select a WinTAX4 data archive file (*.ztx) and load it in WinTAX. 
  * When the event IApplication::OnLoadData is received by the OleControl, it simply adds a string in the WinTAX Log Window 



##  Visual C# Example: WinTAXInPlaceDemoCS

This example requires Microsoft Visual Studio® Version 10 and .NET Framework 4.0 in order to run. The WinTAXInPlaceDemoCS offers exactly the same functions as the C++ example. The sample shows how to implement a C# ActiveX control to be used e.g. in WinTAX Object Control Window. This includes: 

  * Adding a reference to WinTAX Automation library (using design-time-only interop assemblies feature of .NET framework 4 so that the deployment of WinTAX interop assemblies is not required. 
  * Instantiating a class from one of WinTAX Automation interfaces (e.g. [IApplication](interfaceIApplication.md)) and calling one of its methods (e.g. [IApplication::Open](interfaceIApplication.md#a8)) 
  * Handling an event fired by WinTAX (e.g. IApplication::OnLoadData). 

