[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXEvent Interface Reference

An Event object contains information and the instances occurred in a dataset. More...

[List of all members.](interfaceIWTXEvent-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Name](interfaceIWTXEvent.md#a0) ([out, retval] BSTR *pVal)  
| Returns the event name [Read].   
HRESULT | [Group](interfaceIWTXEvent.md#a1) ([out, retval] BSTR *pVal)  
| Returns the group which the event belongs [Read].   
HRESULT | [Properties](interfaceIWTXEvent.md#a2) ([out, retval] IUnknown **pVal)  
HRESULT | [Instances](interfaceIWTXEvent.md#a3) ([out, retval] IUnknown **pVal)  
|  | Allows access to a collection of event instances found in current data set. |
  
* * *

## Detailed Description

An Event object contains information and the instances occurred in a dataset. 

The IWTXEvent interface exposes methods and properties of an Event Object. 

* * *

## Member Function Documentation

|  | HRESULT Properties | ( | [out, retval] IUnknown ** | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the properties associated to the event |

**Note:**
     Not implemented, Reserved for future implementation   
---|---