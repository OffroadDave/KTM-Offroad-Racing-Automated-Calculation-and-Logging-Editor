Ktm Shared Utils

# IOCloneDirectory Method   
  
---  
  
Clone a Directory 

**Namespace:** Â [KtmSharedUtils.DirectoriesAndFiles](ed5eb484-6cfd-54e6-cdb8-729b58e77078.md) **Assembly:** Â KtmSharedUtils (in KtmSharedUtils.dll) Version: 1.5.10.0
    
    
    public static void CloneDirectory(
    	string root,
    	string dest,
    	int days = 0
    )
    
    
    Public Shared Sub CloneDirectory ( 
    	root As String,
    	dest As String,
    	Optional days As Integer = 0
    )
    
    
    public:
    static void CloneDirectory(
    	String^ root, 
    	String^ dest, 
    	int days = 0
    )
    
    
    static member CloneDirectory : 
            root : string * 
            dest : string * 
            ?days : int 
    (* Defaults:
            let _days = defaultArg days 0
    *)
    -> unit 
    

#### Parameters

root
    Type:Â SystemStringSource directory
dest
    Type:Â SystemStringDestination directory
days (Optional)
    Type:Â SystemInt32Number of days

#### Reference

[IO Class](125c08c1-c1de-8dc7-4d08-d82a6fda8f7f.md)

[KtmSharedUtils.DirectoriesAndFiles Namespace](ed5eb484-6cfd-54e6-cdb8-729b58e77078.md)