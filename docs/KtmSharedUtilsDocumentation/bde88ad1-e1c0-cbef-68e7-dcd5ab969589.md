Ktm Shared Utils

# IArrayDataTypeConvertTypeT, G Method (G)  
  
---  
  
Convert Scalar 

**Namespace:** Â [KtmSharedUtils.Array](5cd2c134-d21b-c6c4-cce2-e951dfd4a402.md) **Assembly:** Â KtmSharedUtils (in KtmSharedUtils.dll) Version: 1.5.10.0
    
    
    T ConvertType<T, G>(
    	G valIn
    )
    
    
    
    Function ConvertType(Of T, G) ( 
    	valIn As G
    ) As T
    
    
    generic<typename T, typename G>
    T ConvertType(
    	G valIn
    )
    
    
    abstract ConvertType : 
            valIn : 'G -> 'T 
    

#### Parameters

valIn
    Type:Â GValues to convert

#### Type Parameters

T
    Output element data type
G
    Input data type

#### Return Value

Type:Â TConverted values

#### Reference

[IArrayDataType Interface](4e78d5fd-0c64-63c3-26ac-609d21f5b5b6.md)

[ConvertType Overload](3df9c527-5723-e57a-c2fd-b8d3c9a5a452.md)

[KtmSharedUtils.Array Namespace](5cd2c134-d21b-c6c4-cce2-e951dfd4a402.md)