### CAN Message Layout

The button Layout in [Can Signal Database Editor](AcqMng_CANEdit.md) opens a new window which shows the graphical layout of the selected message and is used to configure the bits position of each signal.

The grid has 64 cells with index from 0 to 63\. Every cell represents a bit and a consecutive group of bits with the same color represents the bits of a signal. A signal can have one or more bits in the message layout. The grid is 8x8 so each row represents a byte and the bits order goes from right to left and from up to down.

Each single bit or each group of bits with the same color contains the name of the signal it refers and the indication of lsb (less significant bit) and msb (most significant bit) and; the arrows indicate the correct direction of reading. The whole information regarding the symbols are listed in the following picture:

Each signal can be selected and the user can performs the following operation on selected or unselected cells:

  * Add Signal




it is possible to insert a new signal using the command Add signal on right-click popup menu on a white cell or the button Add signal at the right of the table.

The command opens the following window:

  1.      * Name: Name of the new signal

     * Start bit: Offset in bits of the data sample from the packet start ( from 0 to 63 )

     * Length: Length in bits of the data sample (from 1 to 32)

     * Endianness: Refers to how bytes of a data word are ordered within memory. The user can choose between Intel (little endian) and Motorola (big endian).

     * Multiplexed by: Name of the eventual multiplexor signal or nothing.

     * Value: Multiplexing value




  * Delete signal




When a signal is selected, you can delete it with Delete button on keyboard or using the command Delete Signal in the right-click popup menu.

The command opens the following confirm message.

  * Edit signal




When a signal is selected, you can modify it with the command Properties, in the right-click popup menu.

The command opens the following window:

The fields are the same as the New Signal window.

  1.      * Name: Name of the edited signal; you can't modify it.

     * Start bit: Offset in bits of the data sample from the packet start ( from 0 to 63 )

     * Length: Length in bits of the data sample (from 1 to 32)

     * Endianness: Refers to how bytes of a data word are ordered within memory. The user can choose between Intel (little endian) and Motorola (big endian).

     * Multiplexed by: Name of the multiplexor signal, if defined.

     * Value: Multiplexing value



  * Move signal




When a signal is selected, you can drag it with the mouse over another sequence of bits. If the bits of the two or more signals are overlapped, then a warning icon appears in each overlapped cell.

when you drag a signal with more than one bit, the behavior is different depending on the type of Endianness, as shown in the picture below where the signal VPSCheckCode_02 is Motorola and Test2 is Intel. In Motorola mode the bits decrease from lsb to msb and in the Intel mode the bits increase from lsb to msb where lsb means less significant bit and msb means most significant bit.

A signal can be configured as Multiplexor; in this case the signal is identified by a small yellow circle inside the first cell of the signal as in picture below:

A Multiplexor is a signal which arrange the other signal depending on his values; the values are shown in the list at the right of the environment.

When the signal assumes the value 0x00, one signal is selected, when assumes the value 0x01, another signal is selected, as shown in pictures below.

Value 0x00 select VPSChildSecSts signal

Value 0x01 select VPSCIDBatteryLow signal

The Multiplexor signal is configured using the property of another signal from where you can choose his multiplexor, as shown here where VPSCIDNoPresCnt will be the multiplexor and VPSCIDBatteryLow one of the multiplexed (with value 0x00):

The button Add Mux Value, on the right of the window, adds a default value for multiplexed signals when a multiplexor is already defined.

When you create a new signal, it will assume this Multiplexor Value.