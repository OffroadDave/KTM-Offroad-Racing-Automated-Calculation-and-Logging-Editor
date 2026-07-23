## Setup Device

A simplified version of Setup Editor is present in the Acquisition Manager setup, for the purpose of editing constants, user records and offset & gain values. Note that by default the following 5 groups are given:

### **VCH Libraries**

The **VCH Libraries** group has the same characteristics as the VCH Libraries special group on WinTAX4. The only difference is that in this simplified environment, it is not possible to use virtual channels. The obvious reason being that this is a pre-acquisition setup, therefore the channels are not available. To add links to the group, use the Add Element File Link command. This group cannot be deleted.

### **Element Files**

Just as in WinTAX4, the **Element Files** group contains links to text files present in the library. This group cannot be deleted.

### **Constants**

The Constants group is a custom group and can therefore be freely edited or deleted. It is proposed the first time that data is read using Setup Device, so as to give the user an indication of where to save the constants.

### **User Record**

The User Record group is a custom group and can therefore be freely edited or deleted. It is proposed the first time that data is read using Setup Device, so as to give the user an indication of where to save the user records.

### **Offset Gain**

By contrast, the Offset Gain group is a fixed group which cannot be deleted, and which must contain the indications of the channels with addition or multiplication constants to be applied to the channel itself.

The interface appears as shown in the following screenshot; in the case of configuration via wizard, the Setup interface is not used.

The Setup Device commands are essentially the same as those found in the extended version on WinTAX4, with a few exceptions.

**_Add Offset & Gain field_**

This command is active when the Offset Gain fixed field is selected. The command does not exist in Setup Editor which is opened from WinTAX4 or from Data Browser. Selecting it opens the following window, in which it is possible to configure the offset and gain values to be associated with the name of a channel.

**_Load & Save Master Copy_**

These commands function in the same way as the equivalent commands on WinTAX4; the only difference is that they are located in the Options menu instead of in the File menu. The Load and Save commands relate to the selected vehicle.

**_Add VCH_**

It is not possible to add VCH in Setup Device. Since it is not possible to load data in this phase, it is not possible to obtain results on the virtual channels: however, it is possible to select links to external files (VCH and TXT) which will subsequently be saved in the archives.

##### Related Issues

  * [Setup Editor](Commands.md)

  * [Elements of the window](glossary.md)

  * [Commands](Setup_Editor.md)

  * [Ole Automation](Oleautomation.md)


