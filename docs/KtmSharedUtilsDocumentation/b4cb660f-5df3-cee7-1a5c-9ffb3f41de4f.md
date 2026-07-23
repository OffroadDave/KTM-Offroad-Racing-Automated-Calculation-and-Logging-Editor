Ktm Shared Utils

# ICrc32AddData Method   
  
---  
  
Add data to already calculated CRC 

**Namespace:** Â [KtmSharedUtils.CRC](5ca7a439-2c1b-a241-19c7-c8b92bf729bd.md) **Assembly:** Â KtmSharedUtils (in KtmSharedUtils.dll) Version: 1.5.10.0
    
    
    void AddData(
    	Object data
    )
    
    
    Sub AddData ( 
    	data As Object
    )
    
    
    void AddData(
    	Object^ data
    )
    
    
    abstract AddData : 
            data : Object -> unit 
    

#### Parameters

data
    Type:Â SystemObjectArray of data to calculate the CRC of; Accepted data types: \- array of Int \- array of Uint \- array of Long \- array of Ulong \- array of Byte \- array of Char \- string 

#### Reference

[ICrc32 Interface](1e9a75e1-c32b-650f-6984-b54eaf457663.md)

[KtmSharedUtils.CRC Namespace](5ca7a439-2c1b-a241-19c7-c8b92bf729bd.md)