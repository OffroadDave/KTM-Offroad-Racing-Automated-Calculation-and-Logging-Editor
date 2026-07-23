#### Configurable Text log (LogFile.xml)

The configurable text log allows saving information within a text file that can be later used to analyze problems that may occur when using the software. 

WinTAX and all modules connected to it such as DataBrowser, AcqManager and all of the acquisition processes, can save log messages within a single text file.

The generated text file in which the current log information is written is "WTX4.log". The file is saved in the "Log" directory configured in WinTAX (usually% WTX4INSTALLDIR% \ Log).

Log messages are saved in the log file during the execution of the program. If the files reaches the maximum dimension it is automatically rotated. (if the MaxfileSize parameter is configured.)

Each log file, when automatically rotated, contains a reference to the previous file as follows:

...continuous of WTX4_210604_101742.log

10:17:42:674 [WTX]: [Layout] - loading layout succeeded (in 250 msec.)

#### How to activate:

The log file can be activated and configured by editing the relative xml setup file “LogFile.xml” located in the %WTX4INSTALLDIR% \ System directory of WinTAX.

Some keys may not be present. If they are not present, the user can simply add them manually respecting the syntax of the xml format of the file.

The usage of the log file is not recommended during the normal use of the program to avoid causing possible slowdowns due to the generation and writing of messages on text files.

It is not recommended to activate keys present in the configuration file that are not documented in this section to prevent possible serious malfunctions.

| XML Item | Value | Description |
| --- | --- | --- |
| <WTX>value</WTX> | 0 or 1 | Xml item that enables the logging for the "WinTAX4" process. |
| <BRW>value</BRW> | 0 or 1 | Xml item that enables the logging for the "DataBrowser" process. |
| <ACQ>value</ACQ> | 0 or 1 | Xml item that enables the logging for the "Acquisition Manager" process. |
| <TXW>value</TXW> | 0 or 1 | Xml item that enables the logging for the "TaxView" process. |
| <TCP>value</TCP> | 0 or 1 | Xml item that enables the logging for the "TCP" process. |
| <MVL>value</MVL> | 0 or 1 | Xml item that enables the logging for the "Marvel" acquisition process. |
| <FCA>value</FCA> | 0 or 1 | Xml item that enables the logging for the "Flashcard" acquisition process. |
| <DVR>value</DVR> | 0 or 1 | Xml item that enables the logging for the "Video" acquisition process. |
| <DST>value</DST> | 0 or 1 | Xml item that enables the logging for the "Real Time" acquisition process. |
| <RCS>value</RCS> | 0 or 1 | Xml item that enables the logging for the "RCS" acquisition process. |
| <CAN>value</CAN> | 0 or 1 | Xml item that enables the logging for the "CAN" acquisition process. |
| <CAN-ETH>value</CAN-ETH> | 0 or 1 | Xml item that enables the logging for the "CAN-Ethernet" acquisition process. |
| <WDS>value</WDS> | 0 or 1 | Xml item that enables the logging for the "WDS" acquisition process. |
| <V2X>value</V2X> | 0 or 1 | Xml item that enables the logging for the "V2X" acquisition process. |
| <MaxFileSize>value</MaxFileSize> | 1048576 (default) | Maximum size of the logging file in bytes of the WTX4.log file. When the file reaches the maximum dimension, the log file is rotated. The archived file is renamed withe the following format (WTX4_YYMMDD_hhmmss.log). |
| <DebugLevel>value</DebugLevel> | 0-1-2-3 | Detail level of the generated information. The following values can be configured: 0- No messages 1- Base level: (Error messages and/or operations executed by the user during usage of the program.) 2- Detailed level: Increased amount of information regarding operations done by the processes. 3- Complete Level: All available informations are exported in the log file. |