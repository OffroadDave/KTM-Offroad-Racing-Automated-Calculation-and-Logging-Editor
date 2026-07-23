### Sampling rate

  * Sampling rate of the waveform is very important because the FFT function operates on a set number of samples (FFT length) 

  * If the sampling rate is low the FFT will operate on a long time duration but the resolution at high frequencies is lost

  * Conversely, if the sampling rate is high, it will operate on a short duration and so low frequencies will not be visible

  * Therefore you must decide what is the frequency range of interest and you may need to filter and downsample in order to achieve the correct sampling rate to give the best final spectrum result

  * At the same time you will need to consider the best Definition to use

  * You may find that there are not enough samples in the acquired waveform to allow the highest definition

  * The following slides aim to explain these choices


