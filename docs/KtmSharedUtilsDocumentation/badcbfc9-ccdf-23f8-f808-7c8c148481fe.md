Ktm Shared Utils

# LogHelper Class  
  
---  
  
LogFile Management class 

SystemObjectÂ Â KtmSharedUtils.LogLogHelper

**Namespace:** Â [KtmSharedUtils.Log](8bed9340-a8fd-14c6-7b52-6c826b7ba098.md) **Assembly:** Â KtmSharedUtils (in KtmSharedUtils.dll) Version: 1.5.10.0
    
    
    public class LogHelper : ILogHelper
    
    
    Public Class LogHelper
    	Implements ILogHelper
    
    
    public ref class LogHelper : ILogHelper
    
    
    type LogHelper =  
        class
            interface ILogHelper
        end

The LogHelper type exposes the following members.

| Â | Name | Description |
| --- | --- | --- |
|  | [LogHelper](8f76a7c9-1b71-6981-0801-296f141d1d87.md) | Class constructor |
  
| Â | Name | Description |
| --- | --- | --- |
|  | [BDeepLogEnable](d86d57eb-cbe1-0cd0-f7ad-631135a58d0c.md) | Enable/disable DeepLog |
|  | [BEnableTraceView](941ad530-bd87-0e9a-03c8-a48e4d4d4a1d.md) | Enable trace view |
|  | [BLogEnable](9e3e1d08-6166-7410-0ff6-58fd02974739.md) | Enable/disble log |
|  | [LogFolderPath](c93f2e42-3f02-1ca9-edc6-ee2ce6eeb294.md) | LogFolder path (i.e. c:\KtmVMSHandlesLog) Set this before creating any object instance |
|  | [LogLevel](10832dbf-a6d8-413e-5d9a-2422ce4e2ce3.md) | Log level Set this before creating any object instance |
|  | [logLevelOld](b0c2eae2-bf00-50ef-ac0d-cd7aa0304ff6.md) | Log level Previous value |
  
| Â | Name | Description |
| --- | --- | --- |
|  | Equals | Determines whether the specified object is equal to the current object. (Inherited from Object.) |
|  | Finalize | Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection. (Inherited from Object.) |
|  | GetHashCode | Serves as the default hash function.  (Inherited from Object.) |
|  | GetType | Gets the Type of the current instance. (Inherited from Object.) |
|  | MemberwiseClone | Creates a shallow copy of the current Object. (Inherited from Object.) |
|  | [SetLogLevel](93765eb2-2505-15c7-7f96-144e04c3be80.md) | Set log level |
|  | [SetLogProperties](cf6c2223-3846-0e07-1429-12d84c087338.md) | Set log properties |
|  | ToString | Returns a string that represents the current object. (Inherited from Object.) |
|  | [Write](d9cefd42-f293-9196-7dfc-4bae4e1270f1.md) | Write log information |
|  | [WriteDeepLog](df4ebc75-8ae9-9d09-c7d9-f64bf238e44f.md) | As "Write", but to be used in case of deep log: \- all the LogLevelType are logged |
|  | [WriteTrace](89923834-58f9-9a58-790c-81afdc68d7a8.md) | Write Trace information |
  
| Â | Name | Description |
| --- | --- | --- |
|  | [Copy](5065b4fe-71c7-485f-462d-7b8b2aa03232.md) | (Defined by [Utils](d98f079c-03c9-bf35-3dc0-c99f1e75c801.md).) |
  
#### Reference

[KtmSharedUtils.Log Namespace](8bed9340-a8fd-14c6-7b52-6c826b7ba098.md)