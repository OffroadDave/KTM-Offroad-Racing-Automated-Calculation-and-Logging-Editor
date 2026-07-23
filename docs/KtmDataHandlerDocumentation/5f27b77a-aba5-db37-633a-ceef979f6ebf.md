Ktm Data Handler

# TyreInformationDataAccessRead Method   
  
---  
  
Reads the tyre information files for the current event 

**Namespace:** Â [KtmDataHandler.KTM.Tyres](04a3461c-f80e-a146-22dd-708dff8b1a17.md) **Assembly:** Â KtmDataHandler (in KtmDataHandler.dll) Version: 3.5.7.0
    
    
    public IDictionary<string, TyreInformationFile> Read(
    	out List<string> errors,
    	IEnumerable<string> filteredRiders = null
    )
    
    
    Public Function Read ( 
    	<OutAttribute> ByRef errors As List(Of String),
    	Optional filteredRiders As IEnumerable(Of String) = Nothing
    ) As IDictionary(Of String, TyreInformationFile)
    
    
    public:
    IDictionary<String^, TyreInformationFile^>^ Read(
    	[OutAttribute] List<String^>^% errors, 
    	IEnumerable<String^>^ filteredRiders = nullptr
    )
    
    
    member Read : 
            errors : List<string> byref * 
            ?filteredRiders : IEnumerable<string> 
    (* Defaults:
            let _filteredRiders = defaultArg filteredRiders null
    *)
    -> IDictionary<string, TyreInformationFile> 
    

#### Parameters

errors
    Type:Â System.Collections.GenericListStringList of errors while reading files
filteredRiders (Optional)
    Type:Â System.Collections.GenericIEnumerableStringList of riders to filter for reading. If null, ignored. Default: false

#### Return Value

Type:Â IDictionaryString, [TyreInformationFile](1168e6de-eec8-024d-ccfb-badf7d2ee7a5.md)List of trye information snapshots grouped by rider

#### Reference

[TyreInformationDataAccess Class](56351907-269d-4bab-8b44-f3f1ec92e35f.md)

[KtmDataHandler.KTM.Tyres Namespace](04a3461c-f80e-a146-22dd-708dff8b1a17.md)