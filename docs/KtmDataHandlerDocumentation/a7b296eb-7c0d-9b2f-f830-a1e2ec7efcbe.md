Ktm Data Handler

# MarelliCLXAccessGetCalibrationNames Method   
  
---  
  
Get the Calibration names

**Namespace:** Â [KtmDataHandler.Marelli.Sysma](231941d2-4c65-7ce2-a989-329d2f7fdc2e.md) **Assembly:** Â KtmDataHandler (in KtmDataHandler.dll) Version: 3.5.7.0
    
    
    public (string[] , string[] ) GetCalibrationNames()
    
    
    Public Function GetCalibrationNames As ( As String(),  As String())
    
    
    public:
    virtual ValueTuple<array<String^>^, array<String^>^> GetCalibrationNames() sealed
    
    
    abstract GetCalibrationNames : unit -> ValueTuple<string[], string[]> 
    override GetCalibrationNames : unit -> ValueTuple<string[], string[]> 

#### Return Value

Type:Â ValueTupleString, StringReturn the names of all the calibrations

#### Implements

[IGettableCalibrationsGetCalibrationNames](82030ef7-2853-39df-0334-bb85640b23d3.md)

#### Reference

[MarelliCLXAccess Class](89f6891c-0695-8d1c-0b5f-9b59d06a4822.md)

[KtmDataHandler.Marelli.Sysma Namespace](231941d2-4c65-7ce2-a989-329d2f7fdc2e.md)