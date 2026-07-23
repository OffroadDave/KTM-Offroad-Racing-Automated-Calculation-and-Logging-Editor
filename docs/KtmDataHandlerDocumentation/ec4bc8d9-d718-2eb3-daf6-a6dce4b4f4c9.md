Ktm Data Handler

# IMultipleSourceDataAccess Interface  
  
---  
  
Interface to be able to get channels from multiple sources 

**Namespace:** Â [KtmDataHandler.Generic.Data](e0050023-9555-6e22-3483-023f8871d9f9.md) **Assembly:** Â KtmDataHandler (in KtmDataHandler.dll) Version: 3.5.7.0
    
    
    public interface IMultipleSourceDataAccess : IGettableChannels
    
    
    Public Interface IMultipleSourceDataAccess
    	Inherits IGettableChannels
    
    
    public interface class IMultipleSourceDataAccess : IGettableChannels
    
    
    type IMultipleSourceDataAccess =  
        interface
            interface IGettableChannels
        end

The IMultipleSourceDataAccess type exposes the following members.

| Â | Name | Description |
| --- | --- | --- |
|  | [SourceList](0a025dcd-068b-aaa8-8992-81b6059d3154.md) | Data Source list |
  
| Â | Name | Description |
| --- | --- | --- |
|  | [GetChannels(String, UInt32, String, Boolean)](dbff70e3-7257-fb8a-5447-447064438f6f.md) | Get the channels for the specified laps  (Inherited from [IGettableChannels](ca1a0f06-5a90-aedd-8f76-40edae340038.md).) |
|  | [GetChannels(String, Double, Double, String, Boolean)](d5a80e2f-46e8-bad3-a1f9-b35077aa7d30.md) | Get channels  (Inherited from [IGettableChannels](ca1a0f06-5a90-aedd-8f76-40edae340038.md).) |
|  | [GetChannelsNames](d82a8c11-30b5-5c1d-6b2f-e5c0ac7e141c.md) | Get the names of all the channels available in in the current DataSet. If no channel exist, NULL is returned  (Inherited from [IGettableChannels](ca1a0f06-5a90-aedd-8f76-40edae340038.md).) |
  
#### Reference

[KtmDataHandler.Generic.Data Namespace](e0050023-9555-6e22-3483-023f8871d9f9.md)