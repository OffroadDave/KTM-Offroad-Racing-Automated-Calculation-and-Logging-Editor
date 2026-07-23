### CAN Signal Database Editor

This environment permits to configure the acquired signals. This is the main editor page and can be opened with the Edit button in CAN Signals Database section in Setup page of Setup Acquisition Windows.

In the window there are two areas, the Signal Database and the Acquired Signals. In the Signal Database area all messages and signals in database are listed, while in the Acquired signals area the only listed signals are those selected for acquisition by user. The two areas are separated by a button with whom the signals are added to the list of acquisition.

#### Signal Database

All messages are listed in the left tree with the Name and the CAN ID in bracket. There are four buttons below the list to sort items: the messages can be sorted alphabetically by their names or with the CAN ID in bracket.

Each message is a group of signals. it is possible to add a new message pushing Add button. The Add button usually add a new signals at selected message; if there are not selected message, a new message will be create. You create a new message even if the user changes the CAN ID and adds a value that does not exist. If the user changes the CAN ID and writes one already exists, the signal is added to the message indicated by the CAN ID.

At the top of the list there is an edit box that can be used to search for messages. The messages list is updated in real time, while editing characters, showing channels with names that match the filter string. The search is case sensitive. Filter can be deleted by keyboard or button Reset.

The signals members of the selected message are displayed in the right list. Signals with a white icon are not selected for the acquisition, those with a green icon, however, are selected for acquisition. If the icon is red, it means that there are overlapping bits on the map (see button layout to solve the problem).

In the right table are displayed the following information:

  * Name: Name of the signal (64 characters)

  * Data Type: The data type can be: uByte (unsigned byte), sByte (signed byte), uWord (unsigned word), sWord (signed word), uLong (unsigned long), sLong (signed long), Float, FloatX

  * Endianness: Intel = little endian, Motorola = big endian.

  * Start bit: Offset in bits of the data sample from the packet start ( from 0 to 63 )

  * Length: Length in bits of the data sample (from 1 to 32)

  * Multiplexed: Signal multiplexor

  * Comment: Signal notes




At the top of the list there is an edit box that can be used to search for signals. The signals list is updated runtime, while editing characters, showing channels which name matches the filter string. The search is case sensitive. Filter can be deleted by keyboard or button Reset.

Below the signals table there are some buttons which performs the following commands:

  * Layout:




See the [CAN Message Layout](AcqMng_CANLayout.md) window

  * Add




Opens the New Signal Properties window. 

  * Remove




The command removes the selected signals. A message box asks for delete confirmation .

  * Edit




Opens the Edit Signal Properties window. 

##### New Signal Properties

##### The Add button opens the following window to add a new signal:

The window is used to add a new signal to a message that already exists even though, as already written above, by changing the CAN ID you can create a new message.

  1.      * Name: Name of the new signal.

     * Data Type: The data type can be: uByte (unsigned byte), sByte (signed byte), uWord (unsigned word), sWord (signed word), uLong (unsigned long), sLong (signed long), Float, FloatX

     * Unit: The measurement unit of the output signal

     * Comment: signal notes




##### CAN Settings

  1.      * CAN ID: Message identifier.

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