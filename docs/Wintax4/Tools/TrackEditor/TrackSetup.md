### Track Setup

Track Setup allows to set the computation mode of the track trajectory and calculate it. There are three ways to do that, depending on Computation Mode configured in [General Setup \ Special Channels](../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgSpecialChannels):

  * Standard

  * GPS (Deg. / Min / Sec.)

  * GPS (Degree)




#### Standard (Speed / Acceleration)

This mode uses speed channel and lateral acceleration channel. The default displayed values of Speed and Lateral acceleration come from [General Setup \ Special Channels](../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgSpecialChannels).

  * Track Name: Name assigned to the track

  * Source Data: Lap that contains the channels used for track calculation. The user can select from all loaded laps. 




  * Speed (Km/h): Speed channel expressed in Km/h.

  * Lateral Acceleration (g): Lateral acceleration channel expressed in g unit.

  * Offsets: Optional values to be added to the channels for any adjustments.

  * Auto Correction: Correction necessary for calculations which will not place the start and end point of the track in the same point, not closing properly the circuit.

  * Inversion: Invert the calculated track.

  * Generate Track: This is the button which perform calculation. The circuit obtained is displayed on the right side of the main Track Editor page.




  * Keep Visible: Allows to keep displayed the window after performing calculations.

  * Open Channel Browser: Displays the Channel Browser window if it is closed.

  * Cancel: Exits from window without saving and without performing calculations.




## 

#### GPS (Deg. / Min. / Sec.)

This mode uses latitude and longitude coordinates in degrees, minutes, seconds.

  * Track Name: Name of the created track. 

  * Source Data: Lap which contains the channels used for track calculation. The user can select from all loaded laps. 



  * Latitude (deg.): Degrees latitude channel.

  * Latitude (min.): Minutes latitude channel.

  * Latitude (sec.): Seconds latitude channel.

  * Longitude (deg.): Degrees longitude channel.

  * Longitude (min.): Minutes longitude channel.

  * Longitude (sec.): Seconds longitude channel.

  * Status: Control channel that can be used to verify the quality of GPS channels. The channel also contains information about orientation (North, South, East, West).

  * Enable OK/HS bit: Allows Status channel to use control information.

  * Enable NSEW bits: Allows Status channel to use information orientation (North, South, East, West).

  * Generate GPS Track: This is the button which perform calculation. The circuit obtained is displayed on the right side of the main Track Editor page.



  * Keep Visible: Allows to keep displayed the window after performing calculations.

  * Open Channel Browser: Displays the Channel Browser window if it is closed.

  * Cancel: Exits from window without saving and without performing calculations.




#### GPS (degree)

Uses latitude and longitude coordinates.

  * Track Name: Name assigned to the track

  * Latitude (Deg.): Latitude degrees latitude channel.

  * Longitude (Deg.): Longitude degrees channel.

  * Track Mode: sets the circuit display graphical mode.

  * Monodimensional Track ‘Single Trace’: draws track with a single trace using Source Data selected.

  * Bidimensional Track ‘Double Trace’: computes two traces, inner and outer, using two laps.

  * Bidimensional Track ‘Driver Line’: draws track with driver line calculated from Source Data and medium track size.



  * Medium Track Size: Defines the width of the track line if "Bidimensional Track Driver Line" is selected.

  * Inner Line: Lap that contains the channels used for inner track calculation if "Bidimensional Track Double Trace" is selected.

  * Outer Line: Lap that contains the channels used for outer track calculation if "Bidimensional Track Double Trace" is selected.

  * Source Data: Lap that contains the channels used for track calculation. The user can select from all loaded laps. 

  * Generate GPS Track: This is the button which perform calculation. The circuit obtained is displayed on the right side of the main Track Editor page.




  * Keep Visible: Allows to keep the window displayed after performing calculations.

  * Open Channel Browser: Displays the Channel Browser window if it is closed.

  * Cancel: Exits from window without saving and without performing calculations.




##### Related Issues

  * [Track Editor](Track_Editor.md)

  * [Commands](Commands.md)

  * [Elements of the window](Elements.md)

  * [Generate Track Section](GenerateTrackSections.md)

  * [Track Designer](TrackDesigner.md)

  * [Google Earth](Google_Earth.md)


