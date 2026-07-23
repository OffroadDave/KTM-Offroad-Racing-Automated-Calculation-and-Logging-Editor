Ktm Data Handler

# MarelliDataAccessAppendImportDataToRun Method   
  
---  
  
Append import channels to existing importData of a RUN

**Namespace:** Â [KtmDataHandler.Marelli.Wintax](3e5e0a5b-20cb-dd13-03cb-f84f0dfbe896.md) **Assembly:** Â KtmDataHandler (in KtmDataHandler.dll) Version: 3.5.7.0
    
    
    public (uint , uint , uint , string )[] AppendImportDataToRun(
    	IDataset ds,
    	string TempFolderPath = "",
    	bool BDeletePreviousImportdata = true
    )
    
    
    Public Function AppendImportDataToRun ( 
    	ds As IDataset,
    	Optional TempFolderPath As String = "",
    	Optional BDeletePreviousImportdata As Boolean = true
    ) As ( As UInteger,  As UInteger,  As UInteger,  As String)()
    
    
    public:
    virtual array<ValueTuple<unsigned int, unsigned int, unsigned int, String^>>^ AppendImportDataToRun(
    	IDataset^ ds, 
    	String^ TempFolderPath = L"", 
    	bool BDeletePreviousImportdata = true
    ) sealed
    
    
    abstract AppendImportDataToRun : 
            ds : IDataset * 
            ?TempFolderPath : string * 
            ?BDeletePreviousImportdata : bool 
    (* Defaults:
            let _TempFolderPath = defaultArg TempFolderPath ""
            let _BDeletePreviousImportdata = defaultArg BDeletePreviousImportdata true
    *)
    -> ValueTuple<uint32, uint32, uint32, string>[] 
    override AppendImportDataToRun : 
            ds : IDataset * 
            ?TempFolderPath : string * 
            ?BDeletePreviousImportdata : bool 
    (* Defaults:
            let _TempFolderPath = defaultArg TempFolderPath ""
            let _BDeletePreviousImportdata = defaultArg BDeletePreviousImportdata true
    *)
    -> ValueTuple<uint32, uint32, uint32, string>[] 

#### Parameters

ds
    Type:Â [KtmDataHandler.Models.ChannelsIDataset](3304bb37-b3dc-51b5-20b7-8b132e48da65.md)Datast to be written
TempFolderPath (Optional)
    Type:Â SystemStringTemporary folder; optional parameter
BDeletePreviousImportdata (Optional)
    Type:Â SystemBooleanDelete previous importData; if false, it is renamed

#### Return Value

Type:Â ValueTupleUInt32, UInt32, UInt32, StringWrite errors

#### Implements

[IMarelliDataAccessWriteAppendImportDataToRun(IDataset, String, Boolean)](bd618e1e-308f-1392-6f76-5ca0b84f17c1.md)

Before launching this method all the TelDataX instances previously open must have no VCH loaded; see example "Example_AppendToImportDataZTX_002.mlx"

#### Reference

[MarelliDataAccess Class](2816f680-b3d8-050f-f364-48490f5b89f0.md)

[KtmDataHandler.Marelli.Wintax Namespace](3e5e0a5b-20cb-dd13-03cb-f84f0dfbe896.md)