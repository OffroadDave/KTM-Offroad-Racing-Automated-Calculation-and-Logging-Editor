### Example: User Records in lookups

The User Records can be also used in combination with lookups files. In this case, there are two ways of work: 

  1. WinTAX4 Virtual Channel: **myVirtual = @Test** WinTAX4 User Records: **Test = Lookup1(“file1”, RPM)** The channel “myVirtual” extracts the content of the User Records “Test” and calculates the formula described by the string returned from the User Records (in this example 1 dimensional Lookup table, called “file1”). In this approach the User can change each run the expression of the User Records, forcing WinTAX4 to use different sets of lookup tables. For Example in the next run the formula should be **Test = Lookup1(“file2”, RPM)** ….
  2. WinTAX4 Virtual Channel **myVirtual = Lookup1(@Test, RPM)** WinTAX4 User Records **Test = “file1”** Also in this approach the User can change each run the expression of the User Record: “file1”, “file2”, “file3”… forcing WinTAX4 to use run by run different lookup files

