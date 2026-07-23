### Selecting a sampling rate

  1. Decide the highest frequency of interest

  2. The sampling rate must be at least 2x the highest frequency of interest

  3. Check the lowest frequency of interest

  4. If it is out of range (too low) for the given waveform you must filter and downsample to achieve a sampling rate that will provide sufficient resolution at low frequencies




The table below shows how min and max frequencies are affected by sample rate and FFT length

The graph below shows the effect of different sampling rates on the same waveform from 5 to 1000 Hz

  * At 5Hz there is good resolution of the 0.5 and 0.6Hz components but the 4 Hz and 500Hz are out of range

  * At 1000 Hz the high frequency component is visible but there is little resolution at 4 Hz and below 2 Hz there is nothing useful




So, depending on which features of the waveform are of interest, different sampling rates may be needed

This can be easily achieved using maths channels.

Downsampling

In order to avoid spectral “artefacts” when downsampling it is good practice to first low-pass filter the waveform, e.g. using Wintax math functions to create a 100Hz channel from the original