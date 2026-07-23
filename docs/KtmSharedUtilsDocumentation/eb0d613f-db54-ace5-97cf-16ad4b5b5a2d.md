Ktm Shared Utils

# CustomListTAddRange Method   
  
---  
  
Add a IPropertyList; if the element is already in, an exception is thrown 

**Namespace:** Â [KtmSharedUtils.DataType](a5c7d275-ad03-28ef-ded6-5ccae76ad4c9.md) **Assembly:** Â KtmSharedUtils (in KtmSharedUtils.dll) Version: 1.5.10.0
    
    
    public void AddRange(
    	ICustomList<T> inC,
    	bool ReplaceFlag = false
    )
    
    
    Public Sub AddRange ( 
    	inC As ICustomList(Of T),
    	Optional ReplaceFlag As Boolean = false
    )
    
    
    public:
    virtual void AddRange(
    	ICustomList<T>^ inC, 
    	bool ReplaceFlag = false
    ) sealed
    
    
    abstract AddRange : 
            inC : ICustomList<'T> * 
            ?ReplaceFlag : bool 
    (* Defaults:
            let _ReplaceFlag = defaultArg ReplaceFlag false
    *)
    -> unit 
    override AddRange : 
            inC : ICustomList<'T> * 
            ?ReplaceFlag : bool 
    (* Defaults:
            let _ReplaceFlag = defaultArg ReplaceFlag false
    *)
    -> unit 

#### Parameters

inC
    Type:Â [KtmSharedUtils.DataTypeICustomList](41964ee6-b57e-9294-2ca1-9f38f7af5ebe.md)[T](926ae6f2-532f-4393-c430-6da17ddeb5cd.md)Object to add
ReplaceFlag (Optional)
    Type:Â SystemBooleanIf TRUE, an exist element with the same name is replaced

#### Implements

[ICustomListTAddRange(ICustomListT, Boolean)](3e7fd2d7-2237-e16c-b33d-1ef81b0abb4c.md)

#### Reference

[CustomListT Class](926ae6f2-532f-4393-c430-6da17ddeb5cd.md)

[KtmSharedUtils.DataType Namespace](a5c7d275-ad03-28ef-ded6-5ccae76ad4c9.md)