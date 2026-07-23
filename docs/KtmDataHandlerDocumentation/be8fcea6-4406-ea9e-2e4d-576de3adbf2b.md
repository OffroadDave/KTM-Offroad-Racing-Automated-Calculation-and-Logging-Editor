Ktm Data Handler

# MarelliDataAccessGetNoObject Method   
  
---  
  
Get the basic informartion about a channel 

**Namespace:** Â [KtmDataHandler.Marelli.Wintax](3e5e0a5b-20cb-dd13-03cb-f84f0dfbe896.md) **Assembly:** Â KtmDataHandler (in KtmDataHandler.dll) Version: 3.5.7.0
    
    
    public (Object data, double tso, double sampleRate, string unit) GetNoObject(
    	string ChnName
    )
    
    
    Public Function GetNoObject ( 
    	ChnName As String
    ) As (data As Object, tso As Double, sampleRate As Double, unit As String)
    
    
    public:
    virtual ValueTuple<Object^, double, double, String^> GetNoObject(
    	String^ ChnName
    ) sealed
    
    
    abstract GetNoObject : 
            ChnName : string -> ValueTuple<Object, float, float, string> 
    override GetNoObject : 
            ChnName : string -> ValueTuple<Object, float, float, string> 

#### Parameters

ChnName
    Type:Â SystemStringChannel name

#### Return Value

Type:Â ValueTupleObject, Double, Double, String Data samples, TSO (in seconds), sampleRate (in Hz) and unit 

#### Implements

[IMarelliDataAccessReadGetNoObject(String)](741c6d41-5fd5-3bc6-674b-10d84a79ac5b.md)

#### Reference

[MarelliDataAccess Class](2816f680-b3d8-050f-f364-48490f5b89f0.md)

[KtmDataHandler.Marelli.Wintax Namespace](3e5e0a5b-20cb-dd13-03cb-f84f0dfbe896.md)