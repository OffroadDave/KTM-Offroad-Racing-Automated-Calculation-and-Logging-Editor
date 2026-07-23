### Example: equal sampling rates

[back](binary_file_format.md)

Ch1 = 5 Hz (byte)

Ch2 = 5 Hz (word)

Ch3 = 5 Hz (long)

Samples are ordered by channel and sampling rate. Min sampling interval = 0.2 s The example shows samples from one second of data.

DATA  
---  
Time point [s] | Data sample | Length  
0.0 | Ch1_sample1 | 1  
| Ch2_sample1 | 2  
| Ch3_sample1 | 4  
0.2 | Ch1_sample2 | 1  
| Ch2_sample2 | 2  
| Ch3_sample2 | 4  
0.4 | Ch1_sample3 | 1  
| Ch2_sample3 | 2  
| Ch3_sample3 | 4  
0.6 | Ch1_sample4 | 1  
| Ch2_sample4 | 2  
| Ch3_sample4 | 4  
0.8 | Ch1_sample5 | 1  
| Ch2_sample5 | 2  
| Ch3_sample5 | 4  
1.0 | Ch1_sample6 | 1  
| Ch2_sample6 | 2  
| Ch3_sample6 | 4