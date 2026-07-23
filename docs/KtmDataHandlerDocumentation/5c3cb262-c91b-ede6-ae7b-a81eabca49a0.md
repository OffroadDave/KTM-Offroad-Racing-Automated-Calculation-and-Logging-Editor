Ktm Data Handler

# DatasetGetElement Method   
  
---  
  
Get object 

**Namespace:** Â [KtmDataHandler.Models.Channels](f6e4a6da-1ea1-65ac-cf99-9562612a3ed4.md) **Assembly:** Â KtmDataHandler (in KtmDataHandler.dll) Version: 3.5.7.0
    
    
    public ITimeseries GetElement(
    	string NameToGet
    )
    
    
    Public Function GetElement ( 
    	NameToGet As String
    ) As ITimeseries
    
    
    public:
    virtual ITimeseries^ GetElement(
    	String^ NameToGet
    ) sealed
    
    
    abstract GetElement : 
            NameToGet : string -> ITimeseries 
    override GetElement : 
            NameToGet : string -> ITimeseries 

#### Parameters

NameToGet
    Type:Â SystemStringTimeseries name to return

#### Return Value

Type:Â [ITimeseries](c50f5c88-77d2-9ae1-4b9c-436993647371.md)Matching Timeseries

#### Implements

[IDatasetGetElement(String)](17a15e2d-f666-a2a8-dc93-f5d15aa7831a.md)

#### Reference

[Dataset Class](b9fd123e-ba88-6c11-f1a7-372cc997d262.md)

[KtmDataHandler.Models.Channels Namespace](f6e4a6da-1ea1-65ac-cf99-9562612a3ed4.md)