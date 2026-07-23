### Google Earth

WinTAX4 can be interfaced with Google Earth and download satellite maps of the areas of the tracks. The system is based on the use of some interfaces of Google Earth, which has to be installed on the PC in use; WinTAX4 is able to detect if it is installed or not; In case Google Earth is not installed, the command that would launch the software, will remain disabled. 

WinTAX requires Google Earth Pro 7.3.2. Other versions (7.3.1 and lower or 7.3.3 and higher) of Google Earth are not working correctly.

On windows 11, it is a requirement to install "Google Earth 7.3.2 - 32bit".

When proposed, avoid the update of Google Earth because newer versions do not work with WinTAX.

It is suggested to disable the auto update feature by disabling the below "Upgrade available" flag located on the lower left side of the Google Earth Window.:

Also, it is suggested to disable the Google Update task in the Windows Task Scheduler.

An active Internet line is obviously required to be able to use the satellite maps.

Please note, before installing this version, that it is highly suggested to remove all previously installed versions from the computer.)

Internet must be connected at all times when acquiring the images and when starting-up WinTAX4 if there are previously downloaded maps in the track editor:

If a satellite map has been previously acquired from Google Earth, the following time WinTAX4 is started, the Internet connection to a GE server is verified. If no connection is available, WinTAX4 alerts the user with the following pop-up message:

This does not mean that WinTAX4 cannot be used; the track windows and their functions are still guaranteed, but you will simply not be able to display the satellite map as their background.

#### Configuration

To use the functions of Google Earth (GE) correctly, the Setup – General – Special Channels page needs to be configured correctly. In the Track Options area you need to select, in the Computation Mode combo box, a GPS mode, either GPS (Deg. / Min. / Sec.) or GPS (Degree, recommended mode). Based on the mode selected, you will have to enter the channels required for configuration. Further configuration details are available in the Setup chapter.

Once the GPS channels have been configured correctly, you can open the Track Editor from the [Tools menu](Commands.md#TRKMenuTools) and create the track based on the GPS coordinates set and on the values of the channels of the lap loaded. Once these operations are complete, you can launch the interface with which WinTAX4 connects to GE using the button on the toolbar or the command from the menu.

Two commands are associated with GE on the Track Editor. The first is Google Earth Link, which opens the GE interface. The second is Remove Google Earth Map, which eliminates the GE map from the track, if it has been loaded.

As mentioned, to access Google Earth you need an active Internet connection, otherwise, when you access the Track Editor environment and attempt to connect to Google Earth using the Google Earth Link command, a connection error pop-up will appear.

#### WinTAX4 Google Earth Interface

Once opened, the GE interface appears as follows.

The movement and search commands are the same as the GE application. Using the mouse, you can browse the maps, clicking and dragging the images. Using the central roller wheel, you can zoom in and out. Using ESC, you can interrupt the loading process if it takes too long; if all the buttons are grey, it means that GE is still processing something. To browse, there is a compass and zoom bar on the right-hand side of the window.

The toolbar offers the following commands.

##### Close Google Earth Interface

The first button closes the interface without loading or saving any image.

##### View Circuit Area

The second button with the checkered flag, activates the search engine of GE in the attempt to display the area in which the track configured on the Track Editor is present, adapting the screen dimensions in the best way possible to the track size.

##### Reset North

The button turns the image so that North is at the top.

##### Grab displayed map

This button starts the map saving process and consequently loads it in WinTAX4. There are four levels of resolution of the image, which can be set from the Grab Resolution combo box, x1, x2, x3, x4, x8. The loading function attempts to recover as much space possible; it first and foremost attempts to centre the map to utilize as much space possible, then, based on the resolution set, the images are saved. You can keep an eye on the saving progress on the bar at the side of the resolution setting or you can interrupt the operation using the ESC key. At the end of the saving process, the GE interface closes and the image is loaded in the Track Editor, overlapping the track, as you can see in the picture that follows. The image can also be saved turned around.

When you exit the Track Editor environment with the Apply command, WinTAX4 will show the map as a background on the track windows.

Using the standard command: File / Save Track As… the user can save the track and relevant satellite map the same way as is normally possible for the tracks. Whilst saving, WinTAX4 will suggest the name of the file, path and type of file. Files ending in *.CGE (default setting suggested by WinTAX4) represent the coupled track and satellite map. Files ending in *.CRG represent just the track files. Remember that when saving as CGE, the images can only be used if the Internet connection is active when launching WinTAX4.

##### Zoom In

The image is enlarged, allowing to view more details and making the image magnified.

##### Zoom Out

The image is reduced, reducing the amount of details that are visible.

##### Related Issues

  * [Track Editor](Track_Editor.md)

  * [Elements of the window](Elements.md)

  * [Commands](Commands.md)

  * [Track Setup](TrackSetup.md)

  * [Generate Track Section](GenerateTrackSections.md)

  * [Track Designer](TrackDesigner.md)


