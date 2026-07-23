Ktm Shared Utils

# ArrayDataTypeIsIncreasingStrictlyMonotonicallyT Method   
  
---  
  
Returns true if the elements in the are increasing strictly monotonically. 

**Namespace:** Â [KtmSharedUtils.Array](5cd2c134-d21b-c6c4-cce2-e951dfd4a402.md) **Assembly:** Â KtmSharedUtils (in KtmSharedUtils.dll) Version: 1.5.10.0
    
    
    public static bool IsIncreasingStrictlyMonotonically<T>(
    	T[] array
    )
    where T : IComparable
    
    
    
    Public Shared Function IsIncreasingStrictlyMonotonically(Of T As IComparable) ( 
    	array As T()
    ) As Boolean
    
    
    public:
    generic<typename T>
    where T : IComparable
    static bool IsIncreasingStrictlyMonotonically(
    	array<T>^ array
    )
    
    
    static member IsIncreasingStrictlyMonotonically : 
            array : 'T[] -> bool  when 'T : IComparable
    

#### Parameters

array
    Type:Â TList we are interested in.

#### Type Parameters

T
    Type of elements in the list.

#### Return Value

Type:Â BooleanTrue if all of the the elements in the list are increasing monotonically.

#### Reference

[ArrayDataType Class](f6ba2c96-ecfe-dfc2-5d0c-ad50832e5e0a.md)

[KtmSharedUtils.Array Namespace](5cd2c134-d21b-c6c4-cce2-e951dfd4a402.md)