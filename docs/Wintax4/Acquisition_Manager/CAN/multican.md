### MultiCAN acquisition

If the license in use is enabled for MultiCAN, it will be possible to configure the following screens.

For standard CAN acquisition commands please check the [CAN acquisition page](AcqMng_CAN.md#CAN_Acquisition)

By selecting the "Peak" device, it is possible to enable the MultiCAN feature. This allows to download from different CAN devices at the same time.

By clicking on the can lines setup, it's possible to configure for each of the 4 can lines, the bitrate speed and the association between logical port / physical port.

Each CAN line can be configured according to the type of connected PEAK device:

The supported PEAK devices are:

  * PCAN-USB (USB to CAN 2.0 converter)

  * PCAN-USB FD (USB to CAN FD converter)

  * PCAN-Ethernet Gateway (Ethernet to CAN 2.0 gateway)

  * PCAN-Ethernet Gateway FD (Ethernet to CAN FD gateway)




The device selection mode can be based either on channel index or TPCanHandle ID.

| "Device Channel" Mode | Index of the device to use. Valid values range from 1 to 16. PCAN-USB and PCAN-USB FD devices have a separate indexing scheme from PCAN-Ethernet Gateways. The same channel number may be used simultaneously by one PCAN-USB and one PCAN-Gateway device. |
| --- | --- |
| "TPCanHandle" mode | In this mode, a hexadecimal numeric code must be entered to identify the device communication line (see table below). |
| 0x51 0x52  0x53  0x54  0x55  0x56  0x57  0x58  0x509 0x50A 0x50B 0x50C 0x50D 0x50E 0x50F 0x510 | PCAN-USB interface, channel 1 PCAN-USB interface, channel 2 PCAN-USB interface, channel 3 PCAN-USB interface, channel 4 PCAN-USB interface, channel 5 PCAN-USB interface, channel 6 PCAN-USB interface, channel 7 PCAN-USB interface, channel 8 PCAN-USB interface, channel 9 PCAN-USB interface, channel 10 PCAN-USB interface, channel 11 PCAN-USB interface, channel 12 PCAN-USB interface, channel 13 PCAN-USB interface, channel 14 PCAN-USB interface, channel 15 PCAN-USB interface, channel 16 |
| 0x801 0x802 0x803 0x804 0x805 0x806 0x807 0x808 0x809 0x80A 0x80B 0x80C 0x80D 0x80E 0x80F 0x810 | PCAN-LAN interface, channel 1 PCAN-LAN interface, channel 2 PCAN-LAN interface, channel 3 PCAN-LAN interface, channel 4 PCAN-LAN interface, channel 5 PCAN-LAN interface, channel 6 PCAN-LAN interface, channel 7 PCAN-LAN interface, channel 8 PCAN-LAN interface, channel 9 PCAN-LAN interface, channel 10 PCAN-LAN interface, channel 11 PCAN-LAN interface, channel 12 PCAN-LAN interface, channel 13 PCAN-LAN interface, channel 14 PCAN-LAN interface, channel 15 PCAN-LAN interface, channel 16 |
  
**Example of selection using TPCANHandle mode**: 

**Identification of connected PEAK devices**: To help identify the device corresponding to a selected channel or TPCANHandle code, the **blink** mode can be enabled. This causes the device's LED to flash.

**Note** : PCAN-Ethernet Gateway devices do not support blink mode.

**Communication Line Parameter Configuration**: Based on the selected device type, the corresponding CAN line initialization parameters are shown (2.0 / FD / Gateway).

##### **PCAN-USB** :

CAN 2.0 device initialization mode. Only the communication bit rate can be selected via the _Bitrate_ combo box.

**Note** : A PCAN-USB FD device can be configured as a PCAN-USB. In this case, it will only receive CAN 2.0 messages.

**PCAN-USB FD**:

In this mode, it is possible to select the frequency, nominal bit rate, and—by enabling the related checkbox—the data bit rate. Standard frequency and bit rate values are provided by default (selectable via combo boxes), but custom values can also be entered using the "_…"_ button.

This opens the FD parameter editor, where new user-defined configurations can be entered.

FD parameters can also be configured using a custom initialization string.

The string must follow the format defined by PEAK. If the configuration is not already available among the predefined ones, the system will prompt to create a new custom entry.

An FD configuration string can be obtained, for example, from PEAK’s PCAN-View tool or copied to the clipboard from an existing configuration.

##### PCAN-Ethernet Gateway / Gateway FD:

##### 

For this device, only the channel index or the TPCANHandle identifier can be configured. All other line parameters are retrieved from the device and can be modified using the PEAK Virtual PCAN-Gateway tool. Refer to the [CAN-Ethernet Gateway](Wintax_-_PEAK_PCAN-Ethernet_Gateway_reference_ITA.md#PCAN_Ethernet_Gateway_quick_setup) documentation for more details.

###### Import DBC files:

If in the main setup screen, in the CAN signal setup, the DBW file name has been left empty, when "Import Selected DBC" is clicked, the fieldname will be automatically filled. 

NOTE: This window allows importing both Standard DBC files and FD DBC files. 

  * Add Prefix with the CAN number: The imported channel names will be modified to include as a prefix the CAN name to which the signal is associated. (an example of the results when enabled can be seen in the picture in the "MultiCAN signal editor" below.




### MultiCAN signal Editor:

Some of the functions available in the CAN signal editor are the same for both the single CAN version and the multi CAN version and are already available in [the CAN signal editor.](AcqMng_CANEdit.md)

When new messages are imported in the multi CAN signal editor, a prefix with the channel identification will be added to all of the messages, to allow quick identification of the hardware they are associated with.

In both the "Signal name" column and the "Acquired signals" a column has been added indicating the CAN channel they are associated with.

If one of the signals in the acquired signal list is double clicked, it's possible to edit some of it's fields. An example is in the following image: The CAN channel associated to the signal is visualized in a read only box.(in the red circle)

##### New Signal Properties

##### The Add button opens the following window to add a new signal:

The window is used to add a new signal to a message that already exists even though, as already written above, by changing the CAN ID you can create a new message.

  1.      * Name: Name of the new signal.

     * Data Type: The data type can be: uByte (unsigned byte), sByte (signed byte), uWord (unsigned word), sWord (signed word), uLong (unsigned long), sLong (signed long), Float, FloatX

     * Unit: The measurement unit of the output signal

     * Comment: signal notes




##### CAN Settings

  1.      * CAN ID: Message identifier.

     * Num CAN: the signal will be assigned to this CAN channel. (only available in MultiCAN setup)

     * Type: Indicates the addressing mode of the CAN. It can be Standard or Extended. If the type is Extended, a letter "x" is appended to the message label.

     * Endianness: Refers to how bytes of a data word are ordered within memory. The user can choose between MSB (Motorola) or LSB (Intel). MSB and LSB mean respectively most and least significative bit and refers to big endian and little endian. 

     * Start bit: Offset in bits of the data sample from the packet start ( from 0 to 63 )

     * Length: Length in bits of the data sample ( from 1 to 32 )




##### Multiplexing

The multiplexing is used to vary the structure of the channels in the packet according to the value of the signal configured.

  1.      * Multiplexed by: Name of the multiplexor signal

     * Value: Multiplexing value




##### Elaboration / Formatting

  * Offset: The offset values are algebraically summed to the channel values.

  * Gain: The gain values are multiplied to the channel itself.

  * Decimals: Number of decimals in channels values.




##### Edit Signal Properties

This window allow editing the properties of one or more selected signals. The fields are the same of the New Signals Properties window.