## WinTAX4 .BIN file format

## File structure

The WinTAX4 binary export file uses the extension *.bin. Each file represents one ‘lap’ of data. The file is structured in three parts:

### File header

It contains general information about the lap.

### Channel header

Describes sampling and representation information for each individual channel in a sequence of 32-byte records. A maximum of 1024+1 channels may be contained in the file. The additional channel is a status channel as described below. Data is represented as integer variables. The value of the decimal field of the channel header is used to represent floating point values up to a maximum of 3 decimal places (i.e. if decimal=3 then the integer data value must be divided by 1000 to obtain the real value).

### Data

The data are written as a series of samples from each time point. The time interval is determined by the channel with the highest sampling rate. Time points are not written explicitly and the time interval must be derived from the channel header. At each time point, channel samples are written according to the order in which they appear in the channel header. At any given time point, only those channels which are sampled are written (i.e. there is no filling in at intermediate time points). If all channels are sampled at the same rate, then each time point contains one sample from each channel.

examples: [equal sampling rates](equal_sampling_rates.md); [different sampling rates](different_sampling_rates.md)

### File contents

The contents of the file are described in the table below and two cases are shown in the following examples.

| Label | Bytes | Description |
| --- | --- | --- |
File header  
File comment | 32 | Fill with NULL chars  
N_chan | 2 | Number of channels  
Lap_length | 4 | Lap length [milliseconds]  
Reserved | 10 | Reserved  
Channel header  
Freq | 2 | [1,2,5,10,20,50,100,200,500,1000,2000,5000,10000] Hz  
Decimals | 1 | 0 £ dec £ 7  
Output format | 1 | 0=Hex 1=Hex leading zeros 2=Dec 3=Dec leading zeros 4=ASCII 5=Binary  
Type | 1 | 0x0=Byte (unsigned 1 byte) 0x1=Char (signed 1 byte) 0x2=Word (unsigned 2 bytes) 0x3=Short (signed 2 bytes) 0x4=Dword (unsigned 4 bytes) 0x5=Long (signed 4 bytes) 0xFF=Status channel (unsigned 1 byte)  
Reserved | 11 | Reserved  
Label | 16 | Channel name  
Data  
|  |
  
## 

## Status channel

One of the options in the Type field is shown as Status channel (1 byte). This feature is used by WinTAX4 to provide additional information about all of the other channels at any particular sample point, in particular it is used to flag invalid data points such as radio drop-out in telemetry data. The Status value determines how all other data points in the current sample should be interpreted.

The Status channel is optional and may be given any valid name. If not present in the table, all data points will be interpreted as being good. This channel may be used by external applications which use the export file to identify sample points which should be ignored.

The Status channel may assume the following values

| Value | Meaning |
| --- | --- |
| 0 | Valid data points |
| 1 | No-Rx – radio drop out |
| 2 | Err – invalid data points |
  
WinTAX4 will typically generate the Status channel at the maximum sampling rate found in the table so that the shortest duration No-Rx will be equivalent to one sample of the fastest channel. If a *.bin file is created where Status is generated at a different, lower sampling rate then the shortest No-Rx drop-out may include several good samples of faster channels. This situation is unlikely to occur since data created by an external application (e.g. a simulation program) is unlikely to simulate radio drop-out or error conditions.