## Users Management

WinTAX4 allows creating customized analysis environments for different users. A dedicated setup directory is created for each user under WintTAX4\Users. These can be copied from one PC to another and loaded into the list of users.

### Local user configurations

In general, the configurations which are local to the user are those which are related to the WinTAX4 and Data Browser screen layouts and presentation, e.g. layouts, windows, toolbars, channel browser, parameters etc. The selection of virtual channels libraries used is also user-specific.

### Creating users

  1. Select [Setup/Users/Configure Users](WTXSetupUsers.md)

  2. Select Edit/Add user

  3. Type the user name to be added. Some characters ( /:\\*?\<>| ) are not allowed in the user name. 

  4. Select a level of permissions (Beginner, Normal, Super). The default level of a new user is "Super".




##### User permissions

| Level | Archive edit | Acquisition manager/Real time |
| --- | --- | --- |
| Beginner | None | Cannot publish context info |
| Normal | Only Offset & Gain | Cannot publish context info |
| Super | All | All |
  
## 

###  Importing users

  1. Copy a user directory from another PC to WinTAX4/Users

  2. Select Setup/Users/Configure users

  3. Press F5 or select Edit refresh

  4. The users list will be refreshed to include all the subdirectories under WinTAX4/Users

  5. All the users will now be visible in the user drop down list on the toolbar


