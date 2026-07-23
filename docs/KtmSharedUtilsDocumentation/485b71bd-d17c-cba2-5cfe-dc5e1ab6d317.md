Ktm Shared Utils

# BinaryFileProcessorCppWriteDouble Method   
  
---  
  
Write Binary file 

**Namespace:** Â [KtmSharedUtils.FileIO](9fed5fee-370d-8bd8-8e19-87ce7136ea4b.md) **Assembly:** Â KtmSharedUtils (in KtmSharedUtils.dll) Version: 1.5.10.0
    
    
    public void WriteDouble(
    	string filePath,
    	double[] dataToWrite
    )
    
    
    Public Sub WriteDouble ( 
    	filePath As String,
    	dataToWrite As Double()
    )
    
    
    public:
    virtual void WriteDouble(
    	String^ filePath, 
    	array<double>^ dataToWrite
    ) sealed
    
    
    abstract WriteDouble : 
            filePath : string * 
            dataToWrite : float[] -> unit 
    override WriteDouble : 
            filePath : string * 
            dataToWrite : float[] -> unit 

#### Parameters

filePath
    Type:Â SystemStringFile path
dataToWrite
    Type:Â SystemDoubleArray to be written

#### Implements

[IBinaryFileProcessorCppWriteDouble(String, Double)](a680766d-0822-c8ff-3b98-2b9f1a3f36b3.md)

#### Reference

[BinaryFileProcessorCpp Class](3e308834-ba1f-481d-6d06-37a00a4ec46d.md)

[KtmSharedUtils.FileIO Namespace](9fed5fee-370d-8bd8-8e19-87ce7136ea4b.md)