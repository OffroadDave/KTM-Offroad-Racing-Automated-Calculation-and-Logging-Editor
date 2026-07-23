[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# ITelXMLObject Interface Reference

The ITelXMLObject interface exposes methods and properties of a generic object accessible via XML-formatted data. More...

Inherited by [IWTXTelXMLObject](interfaceIWTXTelXMLObject.md). 

[List of all members.](interfaceITelXMLObject-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [XML](interfaceITelXMLObject.md#a0) ([out, retval] BSTR *pVal)  
| HRESULT | [Name](interfaceITelXMLObject.md#a2) ([out, retval] BSTR *pVal) |
  
* * *

## Detailed Description

The ITelXMLObject interface exposes methods and properties of a generic object accessible via XML-formatted data. 

* * *

## Member Function Documentation

|  | HRESULT Name | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the name of a XML Object. [Read] |

**Note:**
      
| --- | --- |
  
|  | HRESULT XML | ( | [out, retval] BSTR * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | The XML content associated to the generic object. [Read / Write] |

**Note:**
      
---|---