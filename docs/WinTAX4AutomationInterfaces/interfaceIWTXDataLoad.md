[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXDataLoad Interface Reference

The IWTXDataLoad interface exposes methods and properties of a Data Load object. More...

Inherits [ITelDataLoad](interfaceITelDataLoad.md). 

[List of all members.](interfaceIWTXDataLoad-members.md)  
---  
  
## Public Member Functions  
  
| HRESULT | [Laps](interfaceIWTXDataLoad.md#a1) ([out, retval] [IWTXDataLoads](interfaceIWTXDataLoads.md) **pVal) |
  
* * *

## Detailed Description

The IWTXDataLoad interface exposes methods and properties of a Data Load object. 

The Data Load object represents a single lap or a set of appended laps loaded by WinTAX, and can give access to channels and other telemetry properties. Compared to TelDataX interfaces, IWTXDataLoad is alike [ITelLap](interfaceITelLap.md) interface. 

**Note:**
    **[ method Open is CURRENTLY DISABLED ]**

* * *

## Member Function Documentation

|  | HRESULT Laps | ( | [out, retval] [IWTXDataLoads](interfaceIWTXDataLoads.md) ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Laps returns an IWTXDataloads collection of the single laps that form an append. Each IWTXDataload will allow extraction only of the Properties [Read] as in the case where a single lap is loaded. If a single lap is loaded, the collection is empty. |

**Note:**
     The use of IWTXDataload in this collection is limited to the only Properties property. Refer to the page [Sample: Current Data Properties](sample_currentdataproperties.md) for a simple example.   
---|---