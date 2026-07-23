Ktm Data Handler

# IDataAccessRead Interface  
  
---  
  
Intrface to read data from the DataAccess 

**Namespace:** Â [KtmDataHandler.DataStorage](22a271a7-da74-151a-08a8-af093318f17b.md) **Assembly:** Â KtmDataHandler (in KtmDataHandler.dll) Version: 3.5.7.0
    
    
    public interface IDataAccessRead : IGettableChannels, 
    	IGettableLapMarks
    
    
    Public Interface IDataAccessRead
    	Inherits IGettableChannels, IGettableLapMarks
    
    
    public interface class IDataAccessRead : IGettableChannels, 
    	IGettableLapMarks
    
    
    type IDataAccessRead =  
        interface
            interface IGettableChannels
            interface IGettableLapMarks
        end

The IDataAccessRead type exposes the following members.

| Â | Name | Description |
| --- | --- | --- |
|  | [GetChannels(String, UInt32, String, Boolean)](dbff70e3-7257-fb8a-5447-447064438f6f.md) | Get the channels for the specified laps  (Inherited from [IGettableChannels](ca1a0f06-5a90-aedd-8f76-40edae340038.md).) |
|  | [GetChannels(String, Double, Double, String, Boolean)](d5a80e2f-46e8-bad3-a1f9-b35077aa7d30.md) | Get channels  (Inherited from [IGettableChannels](ca1a0f06-5a90-aedd-8f76-40edae340038.md).) |
|  | [GetChannelsNames](d82a8c11-30b5-5c1d-6b2f-e5c0ac7e141c.md) | Get the names of all the channels available in in the current DataSet. If no channel exist, NULL is returned  (Inherited from [IGettableChannels](ca1a0f06-5a90-aedd-8f76-40edae340038.md).) |
|  | [GetLapMarks](75ef8cf6-05c2-764b-ca66-199381a6cfd9.md) | Return the Lap Marks Array  (Inherited from [IGettableLapMarks](7c95423d-1dc6-348e-87fa-47e244f8c969.md).) |
  
#### Reference

[KtmDataHandler.DataStorage Namespace](22a271a7-da74-151a-08a8-af093318f17b.md)