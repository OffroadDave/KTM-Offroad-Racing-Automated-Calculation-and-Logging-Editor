Ktm Data Handler

# TimeseriesGetDataAtTime Method   
  
---  
  
Return the Timeseries value at a specific time value. Interpolation method is used as specified. 

**Namespace:** Â [KtmDataHandler.Models.Channels](f6e4a6da-1ea1-65ac-cf99-9562612a3ed4.md) **Assembly:** Â KtmDataHandler (in KtmDataHandler.dll) Version: 3.5.7.0
    
    
    public double[] GetDataAtTime(
    	double[] TimePoints,
    	InterpMethod method
    )
    
    
    Public Function GetDataAtTime ( 
    	TimePoints As Double(),
    	method As InterpMethod
    ) As Double()
    
    
    public:
    virtual array<double>^ GetDataAtTime(
    	array<double>^ TimePoints, 
    	InterpMethod method
    ) sealed
    
    
    abstract GetDataAtTime : 
            TimePoints : float[] * 
            method : InterpMethod -> float[] 
    override GetDataAtTime : 
            TimePoints : float[] * 
            method : InterpMethod -> float[] 

#### Parameters

TimePoints
    Type:Â SystemDouble[sec] - Time value array
method
    Type:Â [KtmDataHandler.Models.ChannelsInterpMethod](80deb342-cb3d-a625-191d-7f58e14c1cfb.md)Interpolation method

#### Return Value

Type:Â Double Timeseries Data at "time = TimePoint" 

#### Implements

[ITimeseriesGetDataAtTime(Double, InterpMethod)](b04fae04-56a3-c863-d131-842f960eebcb.md)

#### Reference

[Timeseries Class](077bdc1a-b695-ee88-1c86-9351271026c4.md)

[KtmDataHandler.Models.Channels Namespace](f6e4a6da-1ea1-65ac-cf99-9562612a3ed4.md)