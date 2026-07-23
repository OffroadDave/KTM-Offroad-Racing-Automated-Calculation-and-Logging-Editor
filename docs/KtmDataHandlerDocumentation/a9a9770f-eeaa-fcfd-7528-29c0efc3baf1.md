Ktm Data Handler

# TimeseriesRunFilter Method   
  
---  
  
Apply a filter to a given vector 

**Namespace:** Â [KtmDataHandler.Models.Channels](f6e4a6da-1ea1-65ac-cf99-9562612a3ed4.md) **Assembly:** Â KtmDataHandler (in KtmDataHandler.dll) Version: 3.5.7.0
    
    
    public double[] RunFilter(
    	double[] data,
    	ITimeseriesFilter filter = null
    )
    
    
    Public Function RunFilter ( 
    	data As Double(),
    	Optional filter As ITimeseriesFilter = Nothing
    ) As Double()
    
    
    public:
    virtual array<double>^ RunFilter(
    	array<double>^ data, 
    	ITimeseriesFilter^ filter = nullptr
    ) sealed
    
    
    abstract RunFilter : 
            data : float[] * 
            ?filter : ITimeseriesFilter 
    (* Defaults:
            let _filter = defaultArg filter null
    *)
    -> float[] 
    override RunFilter : 
            data : float[] * 
            ?filter : ITimeseriesFilter 
    (* Defaults:
            let _filter = defaultArg filter null
    *)
    -> float[] 

#### Parameters

data
    Type:Â SystemDouble[double[]] data to be filtered
filter (Optional)
    Type:Â [KtmDataHandler.Utils.IInterfacesITimeseriesFilter](c3eab5c0-253e-5ce7-36c7-93f4594c68e4.md)[ITimeseriesFilter] filter

#### Return Value

Type:Â Doublefiltered data

#### Implements

[ITimeseriesRunFilter(Double, ITimeseriesFilter)](343066b0-c4ce-a2fc-26bf-96bf317e58e5.md)

#### Reference

[Timeseries Class](077bdc1a-b695-ee88-1c86-9351271026c4.md)

[KtmDataHandler.Models.Channels Namespace](f6e4a6da-1ea1-65ac-cf99-9562612a3ed4.md)