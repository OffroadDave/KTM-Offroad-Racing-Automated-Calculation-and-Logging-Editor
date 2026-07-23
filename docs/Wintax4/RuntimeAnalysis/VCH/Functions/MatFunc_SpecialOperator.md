### Special operators

####  # operator ( channel#[ABCDEFGHIJLRT] )

The # operator is used to identify the Dataset on which the channel calculation will be made.

  * Channel can be any simple channels (logged, VCH, condition, etc. ); an expression cannot precede # operator.

  * A, B, C, D, E, F, G, H, I, J are the Dataset identifiers. Letter A corresponds to Dataset number 1, letter B corresponds to Dataset 2 and so on up to letter J which corresponds to Dataset number 10.

  * L corresponds to Real Time Lap Comparison.

  * R corresponds to Reference lap, if enabled in licence.



  * T corresponds to Telemetry Dataset.




# operator allows creating VCH which can be calculated on different Dataset in post processing or in real time.

The standard syntax of WinTAX4 VCH is the name of the channel, than the # operator followed by the index of Dataset.

  * The expression VCH1 = RPM #T - RPM #L returns the difference of RPM channel between real-time live data and reference lap used for “real-time comparison”.

  * The expression VCH2 = RPM#A – RPM#B returns the difference of RPM channel in data A and B, where usually A and B are two laps compare.

  * The expression VCH3 = RPM#T – RPM#A returns the difference of RPM channel using samples of telemetry data and samples of lap loaded in Dataset A.

  * The expression VCH4 = RPM#T – RPM#R returns the difference of RPM channel using samples of telemetry data and samples of reference lap automatically loaded by WinTAX4.

  * In post processing window, if only one lap is loaded, RPM and RPM#A returns the same values; in real time window RPM and RPM#T returns the same values.


