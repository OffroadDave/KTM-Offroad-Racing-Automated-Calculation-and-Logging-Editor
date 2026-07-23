### Example: different sampling rates

[back](binary_file_format.md)

Ch1 = 1 Hz (byte)

Ch2 = 10 Hz (word)

Ch3 = 5 Hz (long)

Samples are ordered by channel and sampling rate. Min sampling interval = 0.1 s The example shows samples from one second of data.

DATA  
---  
Time point [s] | Data sample | Length  
0.0 | Ch1_sample1 | 1  
| Ch2_sample1 | 2  
| Ch3_sample1 | 4  
0.1 | Ch2_sample2 | 2  
0.2 | Ch2_sample3 | 2  
| Ch3_sample2 | 4  
0.3 | Ch2_sample4 | 2  
0.4 | Ch2_sample5 | 2  
| Ch3_sample3 | 4  
0.5 | Ch2_sample6 | 2  
0.6 | Ch2_sample7 | 2  
| Ch3_sample4 | 4  
0.7 | Ch2_sample8 | 2  
0.8 | Ch2_sample9 | 2  
| Ch3_sample5 | 4  
0.9 | Ch2_sample10 | 2  
1.0 | Ch1_sample2 | 1  
| Ch2_sample11 | 2  
| Ch2_sample6 | 4