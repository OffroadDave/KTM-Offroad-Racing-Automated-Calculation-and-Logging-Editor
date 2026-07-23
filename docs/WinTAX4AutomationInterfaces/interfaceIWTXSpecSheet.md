[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# IWTXSpecSheet Interface Reference

The IWTXSpecSheet exposes methods and properties of SpecSheet Objects. More...

[List of all members.](interfaceIWTXSpecSheet-members.md)  
---  
  
## Public Member Functions  
  
HRESULT | [Count](interfaceIWTXSpecSheet.md#a0) ([out, retval] LONG *pVal)  
HRESULT | [WorkingCopy](interfaceIWTXSpecSheet.md#a1) ([in] VARIANT Item,[out, retval] [IWTXSpecWorkingCopy](interfaceIWTXSpecWorkingCopy.md) **pVal)  
HRESULT | [Refresh](interfaceIWTXSpecSheet.md#a2) (void)  
HRESULT | [RemoveWorkingCopy](interfaceIWTXSpecSheet.md#a3) (VARIANT Item)  
| HRESULT | [AddWorkingCopy](interfaceIWTXSpecSheet.md#a4) (void) |
  
* * *

## Detailed Description

The IWTXSpecSheet exposes methods and properties of SpecSheet Objects. 

The interface contains a collection of working copies which permits accessing groups and single items in a spec sheet editor. 

* * *

## Member Function Documentation

|  | HRESULT AddWorkingCopy | ( | void |  | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Creates a new Working Copy from the master working copy |

**Remarks:**
     The Working Copy is added at the end of the collections 

**Note:**
     Use WorkingCopy property to access a new Working Copy.   
| --- | --- |
  
|  | HRESULT Count | ( | [out, retval] LONG * | _pVal_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Returns the number of Working copies available in the collection. [Read] |

**Remarks:**
     Use this property to get the number of Working copies opened in the Spec Sheet Editor 

**Note:**
     The property includes Working Copies opened in comparison   
| --- | --- |
  
|  | HRESULT Refresh | ( | void |  | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Reload spec sheet dialog |

**Note:**
     This method is used to reload SpecSheet after changes in Working Copies, Groups and Items.   
| --- | --- |
  
|  | HRESULT RemoveWorkingCopy | ( | VARIANT | _Item_ | ) |  |
| --- | --- | --- | --- | --- | --- |
|  | Removes a Working Copy from Spec Sheet |

**Parameters:**
|  | _Item_ | Index of the Working Copy to remove |
| --- | --- |
  
**Note:**
      
  
|  | HRESULT WorkingCopy | ( | [in] VARIANT | _Item_ , |
| --- | --- | --- | --- |
|  |  | [out, retval] [IWTXSpecWorkingCopy](interfaceIWTXSpecWorkingCopy.md) ** | _pVal_ |
|  | ) |  |
|  | Allows accessing a single Working Copy of the collection. [Read] |

**Parameters:**
|  | _Item_ | Index of the Working Copy in the collection. |
| --- | --- |
| _pVal_ | Pointer to the Working Copy interface. |
  
**Remarks:**
     Use this method to access a single Working Copy. Depending on the language used, it is possible to access the collection in easier ways (eg. calling **WorkingCopy(0)** or **WorkingCopy("workingcopyname")** in VBScript) 

**Note:**
     This is a standard collection method. Collections are recognized and accessed in different ways depending on the language used (eg. a **For Each..Next** loop in VB as follows) 