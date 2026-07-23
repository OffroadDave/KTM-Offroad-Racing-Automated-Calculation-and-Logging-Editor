### Filters

####  butterworth(channel, type, freqlow, freqhigh, order)

Butterworth filter is characterized by a magnitude response that is maximally flat in the pass band and monotonic overall.

The first argument can be a channel or an expression.

Type (string) may take the following values: "low", "high", "bandpass", "bandstop".

FreqLow and FreqHigh are the cutoff frequencies.

Order is the order of the filter or in other words the number of recursive inputs used to calculate the current output; the minimum order is 1, the maximum is 99.

####  FFT_BP(channel, freqlow, freqhigh)

FFT band pass filter

Returns the data channel band pass filtered between freqlow (Hz) and freqhigh (Hz). The first argument can be a channel or an expression.

####  FFT_BS(channel, freqlow, freqhigh)

FFT band stop filter

Returns the data channel filtered above and below freqlow (Hz) and freqhigh (Hz). The first argument can be a channel or an expression.

####  FFT_HP(channel, freq)

FFT high pass filter

Returns the data channel high pass filtered at freq (Hz). The first argument can be a channel or an expression.

####  FFT_LP(channel, freq)

FFT low pass filter

Returns the data channel low pass filtered at freq (Hz). The first argument can be a channel or an expression.

####  hifiltert (channel, time)

Define a period for which to filter out the dc part of a signal for a specified channel; time is given in ms. The argument channel can be also an expression; time is a value.

### Digital Filters

The digital filters of WinTAX4 are described by the recursion equation:

Y(n) = A0* X(n) + A1*X(n-1) \+ A2*X(n-2) + A3*X(n-3) + A4*X(n-4)+ ..... + B1*Y(n-1) + B2*Y(n-2) + B3*Y(n-3)

For example, the first order filter is described by the difference equation:

Y(n) = A0* X(n) + A1*X(n-1) \+ B1*Y(n-1)

The correspondent WinTAX4's formulae are: DigitalFilter1 (channel, A0, A1, B1)

####  DigitalFilter1 (channel, A0, A1, B1)

Returns the 1st order Digital filter using the equation y(n) = A0*x(n) + A1*x(n-1) + B1*y(n-1)

####  DigitalFilter2 (channel, A0, A1, A2, B1, B2)

Returns the 2nd order Digital filter using the equation y(n) = A0*x(n) + A1*x(n-1) + A2*x(n-2) + B1*y(n-1) + B2*y(n-2)

####  DigitalFilter3 (channel, A0, A1,A2, A3, B1, B2, B3)

Returns the 3rd order Digital filter using the equation y(n) = A0*x(n) + A1*x(n-1) + A2*x(n-2) + A3*x(n-3) + B1*y(n-1) \+ B2*y(n-2) + B3*y(n-3)

####  DigitalFilter4 (channel, A0, A1,A2, A3, A4, B1, B2, B3, B4)

Returns the 4th order Digital filter using the equation y(n) = A0*x(n) + A1*x(n-1) + A2*x(n-2) + A3*x(n-3) + A4*x(n-5) \+ B1*y(n-1) + B2*y(n-2) + B3*y(n-3) + B4*y(n-4)

For all 4types of digital filter, it is your responsibility to ensure the function is evaluated at the appropriate frequency for the filter coefficients. The ‘Fixed Frequency’ calculation mode is recommended to ensure that changes to the parameters sampling rates do not affect filter results.

The initial condition for Low and High Pass filters is calculated as followings:

Y(n) = A0*X(n) + A1*X(n-1) \+ A2*X(n-2) + B1*Y(n-1) + B2*Y(n-2)

Y(0) = A0*X(0) + 0 + 0 \+ 0 + 0

Y(1) = A0*X(1) + A1*X(0) \+ 0+ B1*Y(0) + 0

Y(2) = A0*X(2) + A1*X(1) \+ A2*X(0) + B1*Y(1) + B2*Y(0)

### .....

The coefficients of the recursion equation can be calculated for example in Matlab, where you can design the filter.

For example:

Cebicev LOW PASS filter defined in Matlab:

[a,b] = cheby1(4, 0.5, 0.1, 'low')

a = 0.0002 0.0007 0.0011 0.0007 0.0002

b = 1.0000 -3.5328 4.7819 -2.9328 0.6868

The correspondent filter in WinTAX4 is:

digitalfilter4(X, 0.0002, 0.0007, 0.0011, 0.0007, 0.0002, 3.5328, -4.7819, 2.9328, -0.6868)

Cebicev High PASS filter defined in Matlab:

[a,b] = cheby1(4, 0.5, 0.1, 'high')

a = 0.6066 -2.4264 3.6396 -2.4264 0.6066

b =1.0000 -3.1004 3.7156 -2.0314 0.4332

The correspondent filter in WinTAX4 is:

digitalfilter4(X, 0.6066, -2.4264, 3.6396, -2.4264, 0.6066, 3.1004, -3.7156, 2.0314, -0.4332)

### IIR Filters

####  IIR(channel, cutfreq)

Infinite Impulsive Response filter of the channel, where cutfreq is the cut-off frequency.

####  IIR_LP(channel, cutfreq)

This type of filter (Infinite Impulse Response) has time output n0 depending not only on the various input values for n <= n0, but also on the output values for n <= n0. It is therefore a recursive type of filter. Infinite Impulsive Response Low Pass filter is a recursive filter that maintains frequency content below the cut-off frequency; it is described by the following characteristic equation:

Y(n) = a0 * X(n) + b1 * Y(n-1)

Where

Y = Out, filtered signal

X = In, source signal

The coefficients of the filter are calculated as follows:

a0 = 1 – X;

b1 = X;

with X = 1/(1 + 2 * PI * cutfreq /f)

PI = ∏

f is the signal frequency

####  IIR_HP(channel, cutfreq)

This type of filter (Infinite Impulse Response) has time output n0 depending not only on the various input values for n <= n0, but also on the output values for n <= n0. It is therefore a recursive type of filter.

Infinite Impulsive Response High Pass filter is a recursive filter that maintains frequency content above the cut-off frequency; it is described by the following characteristic equation:

Y(n) = a0 * X(n) + a1* X (n-1) + b1 * Y(n-1)

Where

Y = Out, filtered signal;

X = In, source signal.

The coefficients of the filter are calculated as follows:

a0 = (1 + X)/2;

a1 = - (1 + X)/2;

b1 = X

with X = 1/(1 + 2 * PI * cutfreq /f);

f is the signal frequency

###  Saturation Amplitude Filters

####  SATAMPS(channel, positivedelta, negativedelta)

Limits the maximum change of a value (positive or negative) from one sample to the next, to a certain value; positivedelta is the maximum change per sample in positive direction and negativedelta the maximum change per sample in negative direction.

If the virtual channel is:

ch_fil = SATAMPS(ch, pos, neg)

where pos is the maximum change per sample in positive direction, neg the maximum change per sample in negative direction, the formula for SATAMPS is:

**ch_fil i = IFGE( chi, chi-1 ) * MIN( chi, (ch_fili-1 \+ pos) ) + IFLT( chi, chi-1 ) * MAX( chi, (ch_fili-1 \- neg) )**

There is the possibility to switch off either the positive or the negative part individually to use only one of both, for example by a negative value for ‘pos’ or ‘neg’:

ch_fil = SATAMPS(ch, -1, 5)

lead to a channel that has got a maximum negative slew rate of 5 / sample, without limiting the slew rate in positive direction.

####  SATAMPT(channel, positivedelta, negativedelta)

Limits the maximum allowed change per second to a value (positive or negative); positivedelta is the maximum change per second in positive direction and negativedelta the maximum change per second in negative direction.

The virtual channel:

ch_fil = SATAMPT(ch, pos, neg)

would accordingly filter ‘channel’ with a maximum delta / second.

In virtual channel pos is the maximum change per second in positive direction, neg the maximum change per second in negative direction; SATAMPT introduces the sample frequency **f sample**of the channel. The internal formula is:

**ch_fil i = IFGE( chi, chi-1 ) * MIN( chi, (ch_fili-1 \+ (pos/fsample)) ) + IFLT( chi, chi-1 ) * MAX( chi, (ch_fili-1 – (neg/fsample)) )**

There is the possibility to switch off either the positive or the negative part individually to use only one of both, for example by a negative value for ‘pos’ or ‘neg’:

ch_fil = SATAMPT(ch, 10, -1)

limit the positive slew rate to 10 / second without affecting negative behavior.

### Amplitude Filters

####  AMPLFLTT(channel, PositiveDelta, NegativeDelta)

Limits the maximum allowed change per second to a value (positive or negative) using previous filtered value; positivedelta is the maximum change per second in positive direction and negativedelta the maximum change per second in negative direction.

####  AMPLFLTS(channel, PositiveDelta, NegativeDelta)

Limits the maximum change to a value (positive or negative) from one sample to the next to a certain value using previous filtered value; positivedelta is the maximum change per sample in positive direction and negativedelta the maximum change per sample in negative direction.