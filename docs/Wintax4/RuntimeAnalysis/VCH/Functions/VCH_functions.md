### Math functions

####  abs(expression)

Absolute

Absolute value (or modulus) of expression. The result is the input expression without regard to its sign.

####  chavg(expression)

Average channel value.

Returns the mean of expression over the current data set. The argument can be a channel or an expression.

####  chavglap(channel)

Average channel value.

Returns the average channel value in the current lap. The argument can be a channel or an expression.

If an append of laps is loaded, chavg differs from chavglap because the first one calculates the average on the entire group of laps, while the second one calculates the average of each lap as in picture below where 5 laps are loaded. If only one lap is loaded, chavg and chavglap assume the same value.

#### chstddevlap(channel)

Standard deviation channel value

Returns the standard deviation channel value in the current lap. The argument can be a channel or an expression.

If an append of laps is loaded, chstddev differs from chstddevlap because the first one calculates the average on the entire group of laps, while the second one calculates the standard deviation of each lap. If only one lap is loaded, chstddev and chstddevlap assume the same value.

####  chavgrange(channel, min, max)

Average channel value with range

Returns the mean of expression over specified time range. The arguments can be values, constants, channels or expressions.

####  chavgrangedist(channel, min, max)

Average channel value with range

Returns the mean of expression over specified distance range. The arguments can be values, constants, channels or expressions.

####  chavgrun(channel) [(1)](VCH_functions.md#MATH_FUNCTION_REMARKS: "Remarks")

Average channel value.

Returns the average channel value in the current run.(if used in a lap report window, the returned value will be related to the lap.) The argument can be a channel or an expression.

If an append of laps of multiple runs is loaded, chavg differs from chavgrun because the first one calculates the average on the entire group of laps, while the second one calculates the average of each run. If laps of the same run are loaded, chavg and chavgrun assume the same value.

#### chstddevrun(channel) [(1)](VCH_functions.md#MATH_FUNCTION_REMARKS: "Remarks")

Standard deviation channel value.

Returns the standard deviation channel value in the current run.(if used in a lap report window, the returned value will be related to the lap.) The argument can be a channel or an expression.

If an append of laps of multiple runs is loaded, chstddev differs from chstddevrun because the first one calculates the average on the entire group of laps, while the second one calculates the average of each run. If laps of the same run are loaded, chstddev and chstddevrun assumes the same value.

####  chdelay(expression, time)

Channel delay

Returns true when expression is true but with a delay of time. The first argument can be a channel or an expression. The arguments can be constants, channels or a expressions. The second argument is the time value expressed in seconds; in case of channels or expressions, the function uses the first valid value to set the time.

####  chdistance(channel, distance)

Channel distance.

Returns the value of a channel at specified position, distance, in the lap. Both arguments can be a channel or an expression.

####  chend(channel)

End channel value.

Return the end value of the argument in input. The argument can be a channel or an expression.

####  chendlap(channel)

End channel value.

Returns the end channel value in current lap. The argument can be a channel or an expression.

####  chendrun(channel) [(1)](VCH_functions.md#MATH_FUNCTION_REMARKS: "Remarks")

End channel value.

Returns the end channel value in current run.(if used in a lap report window, the returned value will be related to the lap.) The argument can be a channel or an expression.

chFirstValueLap(channel)

First channel value in the lap

Returns the first valid value of the channel in the lap. The channel parameter can be a channel or an expression. The lookup happens on the first 3 seconds from the effective start of the lap and ignores invalid data (NoRX). If no valid data exist in the first 3 seconds, NoRX is returned.

####  chfreq(channel)

Channel frequency.

Returns sampling frequency (Hz) of channel. The argument can be a channel or an expression.

chLastValueLap(channel)

Last channel value in the lap

Returns the last valid value of the channel in the lap. The channel parameter can be a channel or an expression. The lookup happens from end to start in the 3 seconds before the effective end of the lap and excludes invalid values (NoRX). If no valid values exist in the last 3 seconds, NoRX is returned.

####  chmax(channel)

Maximum channel value.

Return the maximum value of the argument in input. The argument can be a channel or an expression.

####  chmaxlap(channel)

Maximum channel value.

Returns the maximum channel value in current lap. The argument can be a channel or an expression.

####  chmaxrange(channel, min, max)

Maximum channel value with range

Returns the maximum channel value in specified time range. The arguments can be values, constants, channels or expressions.

####  chmaxrangedist(channel, min, max)

Maximum channel value with range

Returns the maximum channel value in specified distance range. The arguments can be values, constants, channels or expressions.

####  chmaxrun(channel) [(1)](VCH_functions.md#MATH_FUNCTION_REMARKS: "Remarks")

Maximum channel value.

Returns the maximum channel value in current run.(if used in a lap report window, the returned values will be related to the lap.) The argument can be a channel or an expression.

####  chmin(channel)

Minimum channel value

Return the minimum value of the argument in input. The argument can be a channel or an expression.

####  chminlap(channel)

Minimum channel value.

Returns the minimum channel value in current lap. The argument can be a channel or an expression.

####  chminrange(channel, min, max)

Minimum channel value with range

Returns the minimum channel value in specified time range. The arguments can be values, constants, channels or expressions.

####  chminrangedist(channel, min, max)

Minimum channel value with range

Returns the minimum channel value in specified distance range. The arguments can be values, constants, channels or expressions.

####  chminrun(channel) [(1)](VCH_functions.md#MATH_FUNCTION_REMARKS: "Remarks")

Minimum channel value.

Returns the minimum channel value in current run. (if used in a lap report window, the returned value will be related to the lap.) The argument can be a channel or an expression.

####  chres(channel)

Channel resolution

Returns the channel resolution. The argument can be a channel or an expression.

####  chsamples(channel)

Channel samples

Returns the number of samples in the channel. The argument can be a channel or an expression

####  chsize(channel)

Channel size

Returns the channel size. The argument can be only a channel.

####  chstart(channel)

Start channel value

Return the start value of the argument in input. The argument can be a channel or an expression.

####  chstartlap(channel)

Start channel value

Returns the start channel value in current lap. The argument can be a channel or an expression.

####  chstartrun(channel) [(1)](VCH_functions.md#MATH_FUNCTION_REMARKS: "Remarks")

Start channel value

Returns the start channel value in current run.(if used in a lap report window, the returned value will be related to the lap.) The argument can be a channel or an expression.

####  chstdev(channel)

Standard deviation.

Returns the channel standard deviation. The argument can be a channel or an expression.

####  chtime(channel, time)

Value of channel at time.

Returns the channel value at specified time. The first argument can be a channel or an expression, the second is the value of the time.

####  chtimeshift(channel, time)

Channel time shift.

Returns the value of the channel in input shifted of time. The first argument can be a channel or an expression. The arguments can be constants, channels or a expressions. The second argument is the time value expressed in seconds; in case of channels or expressions, the function uses the first valid value to set the time.

####  chtimevalid(expression, time)

Channel time valid

Returns true when expression is true for at least time seconds. The arguments can be constants, channels or a expressions. The second argument is the time value expressed in seconds; in case of channels or expressions, the function uses the first valid value to set the time.

####  cutrange(channel, mintime, maxtime)

Cut channel range.

Returns the values of channels between the time range MinTime and MaxTime (MinTime<time<MaxTime); otherwise returns NoRx.

MinTime and MaxTime can be numbers, channels or expressions.

####  cutrangedist(channel, mindistance, maxdistance)

Cut channel range.

Returns the values of channels between the distance range MinDistance and MaxDistance (MinDistance<time<MaxDistance); otherwise returns NoRx.

MinDistance and MaxDistance can be numbers, channels or expressions.

####  deltach(channel)

Returns the difference between first and last sample of lap. The argument can be a channel or an expression.

####  deriv(channel)

Derivative of channel

Returns the derivative of the channel in input at the current time point. The derivative is calculated by deducting the value of the channel at the preceding time point: DERIV(Ch) = (Ch i \- Ch i-1 )/Dtime, where Dtime is the sampling interval expressed in seconds, defined by the configured frequency of the mathematical channel.

The argument can be a channel or an expression.

####  derivs(channel, sample)

Derivative of channel

Returns the derivative of the channel in input at the current time point. The derivative is calculated by deducting the value of the channel at the preceding time point: DERIV(Ch) = (Ch i \- Ch i-1 )/Dtime, where Dtime is the input sampling interval expressed in seconds calculated by the number of samples in input. The first argument can be a channel or an expression. The second is a number of samples.

####  derivt(channel, time)

Derivative of channel

Returns the derivative of the channel in input at the current time point. The derivative is calculated by deducting the value of the channel at the preceding time point: DERIV(Ch) = (Ch i \- Ch i-1 )/Dtime, where Dtime is the input sampling interval expressed in seconds in input. The first argument can be a channel or an expression. The second is a time value (seconds).

####  diaggt(channel, value, time)

Diagnostic greater than

Return 1 (TRUE) if channel is greater than value; otherwise returns 0 (FALSE). Holdoff time allows detection of transients, the function will remain TRUE for time, expressed in seconds, after which it will return the default value FALSE. The first argument can be a channel or an expression; the second and the third arguments only values.

####  diaglt(channel, value, time)

Diagnostic Less Than

Return 1 (TRUE) if channel is smaller than value; otherwise returns 0 (FALSE). Holdoff time allows detection of transients, the function will remain TRUE for time, expressed in seconds, after which it will return the default value FALSE. The first argument can be a channel or an expression; the second and the third arguments only values.

####  distatcond(condition, "chdistance")

Returns the total distance in the current selection of laps for which the condition is true. The first argument can be a condition or an expression. The second argument must be the name of distance channel. ChDistance must be enclosed in quotation marks. Write "" to use the default distance channel defined in General/Setup/Miscellaneous/Distance.

####  distatcondlap(condition, "chdistance")

Returns the total distance for which the condition is true in current lap. The first argument can be a condition, a channel or an expression. The second argument must be the name of distance channel. ChDistance must be enclosed in quotation marks.

Write "" to use the default distance channel defined in General/Setup/Miscellaneous/Distance.

####  distatcondrun(condition, "chdistance") [(1)](VCH_functions.md#MATH_FUNCTION_REMARKS: "Remarks")

Returns the total distance for which a condition is true in current run. (if used in a lap report window, the returned value will be related to the lap.) The first argument can be a condition, a channel or an expression. The second argument must be the name of distance channel. ChDistance must be enclosed in quotation marks.

Write "" to use the default distance channel defined in General/Setup/Miscellaneous/Distance.

####  distevent(event)

Returns the distance when an event is detected. At any given point within the data this expression will return the position of the previous occurrence of the event. The argument can be an event or an expression.

####  distincond(condition, "chdistance")

Distance travelled while a condition is true. Returns a piecewise distance counter calculated when the condition is true. It resets to zero when the condition is false. It does not reset at end of lap. This can be used to return the distance covered each time a given condition is true. The first argument can be a condition or an expression. The second argument must be the name of distance channel.

ChDistance must be enclosed in quotation marks. Write "" to use the default distance channel defined in General/Setup/Miscellaneous/Distance.

####  enum(channel, "text")

Function to find a text string value in an enumerated channel. Returns the numerical value of the Enum channel associated with the label text. This function can be combined with other logical functions (e.g. IFEQ) to create conditions and events based on a text string input. If the same label is associated with more values, the Enum function returns only the first value. The argument can be only a channel.

####  exist("object")

Function to check for the existence of an object.

The object can be a reference to a logged channel, virtual channel, constant or user record. Returns 1 if the parameter, enclosed in quotation marks, is the name of an object that exist in the data set. Returns 0 if no match is found. 

####  getbit(expression, start, n)

Get Bit

Returns n bits of expression starting form bit start, where start is in the range 0 to 31 and 0 identifies the least significant bit. The returned value is shifted according to the position start. The arguments can be values or expressions.

####  getnibble(expression, n)

Get Nibble

Returns the n th nibble of expression; n can take values from 0 to 7 where 0 returns the least significant nibble. The returned value is shifted according to the position n. The arguments can be values or expressions.

Example: GETNIBBLE(0x1CF5,1) = 15 (0xF)

####  glitch(condition, time)

Glitch filter.

Returns TRUE only when the condition is true for a given time. Condition can be a condition, a channel or an expression. Time is generally a constant value but can be also a channel or an expression; in case of variable values, the function uses the values corresponding to the instant when the condition becomes true.

####  gpsdistance(latitude1, longitude1, latitude2, longitude2)

Returns geodetic distance in meters between a pair of GPS points (latitude, longitude) on the earth’s surface, using an ellipsoidal model of the earth. The arguments can be values, channels or expressions. The input values are expressed in degrees; latitude from -90 to +90 and longitude from 0 to 360.

####  hiword(expression)

Hiword

Returns most significant word of expression and shifts the result 16 bits to the right.

####  integ(channel)

Integral of channel

Returns integral of a channel form the start to the end of the data set. The argument can be also an expression.

####  integt(channel, time)

Integral of channel from time

Returns integral of channel from time. Time is expressed in units of 0.01s. The arguments can be constants, channels or expressions; if the time argument has variable values, the function uses only the first valid value.

####  integtrap(channel)

Integral of channel

Returns integral of a channel form the start to the end of the data set using trapezoidal rule. The argument can be also an expression.

####  interp(channel, frequency)

Interpolation

Linearly interpolates channel at the specified frequency.

####  LapConstMath(LapNumber, expression)

Returns the value of the expression calculated in the selected lap. The first argument is the index of a lap in the run. The second argument can be a channel or an expression. If channel or expression returns a constant value in the lap selected, LapConstMath returns the same value; otherwise, if channel or expression is not constant, LapConstMath returns a not valid value.

####  lapsplittime(section)

Returns the time elapsed from the beginning of the lap to section. The argument is the index of the section and can be a value or an expression.

####  lookup1(file,x)

One dimensional lookup

One-dimensional lookup in file with input value x. File is a *.DAT file in the Wintax4-Libraries directory and must be enclosed in speech marks (filename) without the DAT extension.

Lookup tables can be selected from a browse list and may be edited within WinTAX4 via [Configure/Lookups](../../Lookups/Lookups.md)

Intermediate points are linearly interpolated.

Example

OUT = LOOKUP1 ("TAB", CH) where TAB is the name (without extension) of the file containing the table (e.g. TAB.DAT) and CH is the name of the input channel.

# (# = comment character)

# 1-D look-up table

# columns separated by TABs

# rows end with CR+LF

#

| 0 | 0 |
| --- | --- |
| 1000 | 1 |
| 2000 | 4 |
| 3000 | 9 |
| 4000 | 16 |
| 5000 | 25 |
| 6000 | 36 |
| 7000 | 49 |
| 8000 | 64 |
| 9000 | 81 |
| 10000 | 100 |
  
CH = 1000 -> OUT = 1.0

CH = 2500 -> OUT = 6

#### lookup1Sat(file,x)

The behavior of this VCH is the same as lookup1 (above), but in case the values are saturated, WinTAX will still elaborate data and will not show the values as NORX.

####  lookup2(file,column, row) 

Lookup two dimensional

Two-dimensional lookup in file with input value column and rows. File is a *.DAT file in the Wintax4-Libraries directory and must be enclosed in speech marks (filename) without the DAT extension.

Lookup tables can be selected from a browse list and may be edited within WinTAX4 via [Configure/Lookups](../../Lookups/Lookups.md)

Intermediate points are linearly interpolated.

Example

OUT = LOOKUP2 ("TAB", CH1, CH2) where TAB is the name of the table (TAB.DAT) without extension and CH1 and CH2 are the names of the input channels

# 2-D look-up table

# first row

# columns separated by TABs

# rows end with CR+LF

|  | 0 | 30 | 60 | 90 | 120 |
| --- | --- | --- | --- | --- | --- |
| 1000 | 1.0 | 31 | 61 | 91 | 121 |
| 3000 | 9.0 | 39 | 69 | 99 | 129 |
| 5000 | 25.0 | 55 | 85 | 115 | 145 |
| 7000 | 49.0 | 79 | 109 | 139 | 169 |
| 9000 | 81.0 | 81.0 | 111 | 141 | 201 |
| 10000 | 100.0 | 130 | 160 | 190 | 220 |
  
CH1 = 1000; CH2 = 30 -> OUT = 31

CH1 = 7000; CH2 = 90 -> OUT = 139

####  lookup2Sat(file,column, row)

The behavior of this VCH is the same as lookup2 above, but in case the values are saturated, WinTAX will still elaborate data and will not show the values as NORX.

####  loword(expression)

Low Word

Returns least significant word of expression.

####  LR_OFFSET(channel1, channel2)

Linear Regression Offset, correspond to the "B0" value. It gives the same results than plotting channel1 vs channel2 in an [XY plot](../../../AnalysisWnds/Wnds/XY/XYWndFeatures.md#XYFeatureAnalysis) with a 1st degree "Best Fit"

####  LR_R2(channel1, channel2)

Returns the R2 coefficient (fraction between 0.0 and 1.0), a measure of goodness -of- fit of Linear Regression. An R2 value of 0.0 means that knowing X does not help you predict Y. There is no linear relationship between X and Y, and the Best Fit line is a horizontal line going through the mean of all Y values. When R2 equals 1.0, all point lie exactly on a straight line with no scatter.

####  LR_SLOPE(channel1, channel2)

Linear Regression Slope, correspond to the "B1" value. It gives the same results than plotting channel2 vs channel1 in an [XY plot](../../../AnalysisWnds/Wnds/XY/XYWndFeatures.md#XYFeatureAnalysis) with a 1st degree "Best Fit"

####  mask(channel, threshold, logic, tolerance, holdtime)

it is used to generate events, includes a time-dependent hysteresis.

If the value of logic is 1, "Max", (0, "Min") the function returns

  * 0 if the channel is lower (higher) than Threshold

  * 1 if the channel is greater (less) than or equal to Threshold,




When the result is true (i.e. has exceeded the threshold) all variations within the range: [ Threshold - Tolerance ] ([ Threshold + Tolerance ]) will not be considered for a time = hold_time, counted from when the condition became true .

Thus the function results in hysteresis which is applied for a specified time.

If holdtime is set to -1 then the hysteresis time is infinite and the channel values must be below (above) the tolerance in order for the result to reset to 0.

The arguments can be constants, channels or expressions but only the channel argument is used as variable; in all the other arguments the function uses the first valid value."

Example

= mask(thrott, 60, 1 30, 0.5)

resets when thrott goes below threshold-tolerance (= 30) even if the mask time has not expired

= mask(thrott, 60, 1 40, 0.5)

resets when the mask time (=0.5s) expires because thrott is below threshold-tolerance (= 20)

####  max(expression1, expression2)

Maximum value

Returns the maximum value between expression1 and expression2 at current time.

Minimum value of a series of values

Returns the minimum value included in the expression in comparison at a current time. The first value indicates how many values will be used in the calculation.

#### maxN(n, expression1, expression2, ... ,expression n(Max 256))

Maximum value of a series of values

Returns the maximum value in the expression at a selected time. The first N value indicates how many values will be used in the expression.

####  means(channel, n)

Mean value (sample)

Returns the mean value of channel over n samples.

####  meant(channel, time)

Mean value (time)

Returns the mean value of channel over time (milliseconds).

####  mileage(condition, "chdistance")

Similar to TimeTotal but returns the piecewise counter (i.e. integral in distance) of the channel defined as ChDistance

ChDistance must be enclosed in quotation marks. Write "" to use the default distance channel defined in General/Setup/Miscellaneous/Distance.

####  min(expression1, expression2)

Minimum value

Returns the minimum value between expression1 and expression2 at current time.

#### minN(n, expression1, expression2, ... expression n (Max 256))

Minimum value of a series of values

Returns the minimum value in the expression at a selected time. The first N value indicates how many values will be used in the expression.

####  numevents(event)

Returns the progressive number of occurrences of an event within the loaded data.

Returns 0 up to first event, returns N between N th and the N+1 st

####  poly(x, b0, b1, b2, b3, b4, b5, b6)

Returns a polynomial up to 6th degree calculated on the input value X.

X apart, the other arguments are optional.

####  removenorx(channel)

Removes NoRx values from channel and replaces them with interpolated values. The argument can be also an expression. During real time analysis, the research of samples for the interpolation is executed in a time windows of 5 seconds.

#### 

####  replace(channel, x, y)

Replaces x with y in channel. The arguments can be also expressions.

####  rotl(expression, n)

Rotate expression to left by n bits.

####  rotr(expression, n)

Rotate expression to right by n bits

####  samplemax(channel, n)

Returns maximum channel value n samples behind/above.

####  samplemin(channel, n)

Returns minimum channel value n samples behind/above.

####  SAMPLES(channel)

Returns the number of samples (values <> NoRx) included in channel.

####  SAMPLET(channel, range)

Returns the number of samples up to a certain time "range" [msec] of the lap (values <> NoRx) included in channel.

####  shiftl(expression, n)

Shift left.

Shift left by n bits of expression.

####  shiftr(expression, n)

Shift right.

Shift right by n bits of expression.

####  timeatcond(condition)

Returns the total time in the current selection of laps for which the condition is true. Condition can be also an expression.

####  timeatcondlap(condition)

Returns the total time for which the condition is true in current lap. The argument can be a condition, a channel or an expression.

####  timeatcondrun(condition) [(1)](VCH_functions.md#MATH_FUNCTION_REMARKS: "Remarks")

Returns the total time for which the condition is true in the current run. (if used in a lap report window, the returned value will be related to the lap.) The first argument can be an expression.

####  timeatmax(channel)

Returns the total time in seconds where channel had its maximum value. Channel can be also an expression.

####  timeatmin(channel)

Returns the total time in seconds where channel had its minimum value. Channel can be also an expression.

####  timeevent(event)

Returns the time at which an event is detected. At any given point within the data this expression will return the time of the previous occurrence of the event. The argument can be an event or an expression.

## 

####  timeincond(condition)

Time spent in Condition.

Returns a piecewise time counter calculated when the condition is TRUE. Resets to zero when the condition is FALSE. This can be used to return the elapsed time each time a given condition is true.

#### 

####  ztime(channel, n)

Value of channel at n samples away

Returns the value of a channel with an offset of n samples. Enter a positive value to obtain a future sample or a negative value to obtain a past sample. Consequently, resulting data with the ztime function will be shifted by n samples compared to the input channel.

The arguments can be constants, channels or expressions; if the n argument has variable values, the function uses only the first valid value.

#### RUN Based Functions REMARKS:

Please note that if a virtual channel includes a "Run based operator" like: chavgrun, chendrun, chmaxrun, chminrun, chstartrun, distatcondrun, timeatcondrun, first, last, the resulting data will follow 2 different cases:

##### Graph windows:

In case of graph windows, calculations result will be related only to the loaded data. (e.g. If 2 whole runs have been loaded, both will be subjected to the calculations, if only 1 lap has been loaded, the calculations will be related to that single lap.)

To obtain results referred to a run, it is needed that all the laps composing the run are loaded. 

#####  Lap Report window:

Being a "Lap" based report, all calculations that are done using the above indicated virtual channels, will be related to the referred lap.