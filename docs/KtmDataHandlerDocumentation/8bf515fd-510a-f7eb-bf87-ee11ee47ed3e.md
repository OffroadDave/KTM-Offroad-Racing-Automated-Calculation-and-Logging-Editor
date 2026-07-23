Ktm Data Handler

# MarelliDataAccessAddLapConstant Method   
  
---  
  
Add Constant by Lap

**Namespace:** Â [KtmDataHandler.Marelli.Wintax](3e5e0a5b-20cb-dd13-03cb-f84f0dfbe896.md) **Assembly:** Â KtmDataHandler (in KtmDataHandler.dll) Version: 3.5.7.0
    
    
    public void AddLapConstant(
    	int NLap,
    	string Name,
    	double Value
    )
    
    
    Public Sub AddLapConstant ( 
    	NLap As Integer,
    	Name As String,
    	Value As Double
    )
    
    
    public:
    virtual void AddLapConstant(
    	int NLap, 
    	String^ Name, 
    	double Value
    ) sealed
    
    
    abstract AddLapConstant : 
            NLap : int * 
            Name : string * 
            Value : float -> unit 
    override AddLapConstant : 
            NLap : int * 
            Name : string * 
            Value : float -> unit 

#### Parameters

NLap
    Type:Â SystemInt32Lap number
Name
    Type:Â SystemStringConstant Name
Value
    Type:Â SystemDoubleConstant Value

#### Implements

[IMarelliDataAccessWriteAddLapConstant(Int32, String, Double)](a337cb4d-2d0e-740b-f7a1-5bbfd4bcc78f.md)

#### Reference

[MarelliDataAccess Class](2816f680-b3d8-050f-f364-48490f5b89f0.md)

[KtmDataHandler.Marelli.Wintax Namespace](3e5e0a5b-20cb-dd13-03cb-f84f0dfbe896.md)