### FFT Analysis Window Overview

The frequency analysis functions are based upon the Discrete Fourier Transform (DTF), specifically the Fast Fourier Transform (FFT).

Computation is performed through RADIX-2 by Real data sequences (RFFT - R2), sampled with a fixed interval using Decimation Interval Time (DIT) and IN-PLACE computation. The same rules are followed for the Inverted Transform (IRFFT). When necessary, a PAD is performed with a value not equal to zero, but with values copied from the already existing ones.

Generally, the following points should be considered:

  * The FFT considers the sampled data as a periodic signal of period T equal to the length of the sample itself

  * The subject of the computation is a Finite and Discrete Series of finite values which represent a continuous signal

  * The intense processing of data may lead to a loss of precision.

  * Negative effects of the computation have been compensated where possible by using well-known techniques, which are widely described in the specialized literature.




From the Nyquist Sampling Theorem, a signal that represents a given frequency bandwidth W must be sampled with a frequency derived from the following equation: Fc >= 2*W (The Sampling Theorem sets a theoretical lower limit to the sampling frequency for a given band of frequencies, equal to: Fc >= 2*Fmax. If this is not respected, the signal energy, related to frequencies higher than Fc/2, will be distributed over the lower frequencies, resulting in distortion of the spectrum. The above-mentioned condition is thus necessary to completely reconstruct the signal itself, but may not be sufficient. It depends on both the technique and the instrument used to condition the signal itself, which depends in turn on the purpose of the analysis. Therefore, it may be necessary to sample at a frequency higher than the indicated lower limit). Based upon N given data samples, the FFT produces N complex frequency samples which are equal and opposite the one to the other with respect to the N/2th order, corresponding to the frequency W, or equal to N/2 real samples. The resolution in frequency is thus equal to Df = W/(N/2) = Fc/N.

Information that has been processed in this way can be presented as FFT or as a Power Spectral Density (PSD).

The Power Spectrum Density is estimated splitting the input signal into overlapped segments (50% of overlapping). The input signal is segmented into pair number of sections defined by FFT Length parameter; if required, zero padding and windowing are thus applied. Estimates are averaged to produce the PSD output.

The value of the PSD {Pi} is expressed in Decibels as 10 * log10 (Pi).A confidence interval of 95% dB is given.

WinTAX4 can also present the data as a Transfer Function (TF) between a system’s input and output. It uses the Fourier Transforms of the parameters to calculate the magnitude and the phase of the transfer function against frequency. The result is plotted as dB plot of magnitude (20*log10) and linear plot of phase (Bode's diagram).

By configuring Mode parameter as Spectrogram, the window shows a graphical representation of the frequency of the signal. A Spectrogram is a time-varying spectral representation that shows how the spectral density of a signal varies with time.

##### Windowing

Windowing is a technique used to shape the time portion of data, to minimize edge effects that result in spectral leakage in the FFT spectrum. 

WinTAX4 provides common windowing functions, Hamming, Hanning, Triangle, Blackman, Harris.

##### FFT Length

The number of FFT points used in the FFT calculation are defined by parameter “FFT Length”. 

##### FFT Definition 

The FFT Definition number is used to increase the number of FFT points on equal frequencies, in order to avoid FFT to be too short (such as less than sixteen).

FTT Length = 64 & FFT Definition = 1 makes for example the same result of FFT Length = 128 and definition = 2

##### 

##### Related Issues

  * [Analysis windows](../../AnalysisWndsOverview.md)

  * [Elements of the window](FFTWndItems.md)

  * [Functions](FFTWndFeatures.md)

  * [Configuration](FFTWndSetup.md)

  * [Commands](FFTWndCommands.md)


