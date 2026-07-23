### Commands

The main commands available in the Track Editor window can be enabled through the

  * Menu from Track Editor dialog mainframe

  *     * File

    * Track Designer

    * Track Sections

    * Google Earth

    * Tools

    * Help

  * The buttons located on the dedicated Toolbar and on the window area.

  * Pop-up menu that can be displayed by the mouse right clicking on the table on the left or on the graph, if configured, on the right.

  * Keyboard shortcuts




#### Menu File 

The File menu allows the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Apply | Applies the current settings of the Track Editor and save the configured track in the correct format. See here for a brief description of formats. |
| Cancel | Closes the window without applying the current settings to the configuration of the Track Editor |
| Load Track | Opens a Windows dialog to select a configuration file for the Track. The track formats file are *.CRT, *.CRG, *.CGE. See here for a brief description of formats. |
| Save Track As | Opens a Windows dialog to save a new configuration file. With this command the user can save the track and relevant satellite map as is normally possible for the tracks. Whilst saving, WinTAX4 will suggest the name of the file, path and type of file. See here for a brief description of formats. |
| Clear Current Track | Remove the current Track |
  
##### 

##### Track file format

The Track Editor recognizes this three track formats:

  1. the *.CRT is the file format for a Track calculated by means of speed and acceleration channels (standard mode)

  2. the *.CRG is the file format for a Track calculated by means of GPS channels (GPS mode)

  3. the *.CGE is the file format for a Track calculated by means of GPS channels but this file also contains the Google Earth map.




Files ending in *.CGE (default setting suggested by WinTAX4) represent the track and satellite map coupled together. Files ending in *.CRG represent just the track files. Remember that when saving as CGE, the images can only be used if the Internet connection is active when launching WinTAX4.

#### Menu Track Designer

The Track Designer menu allows the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Track Designer |  | Opens the [Track Designer window](TrackDesigner.md). |
  
#### 

#### Menu Track Sections

The Track Sections menu allows the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Auto Generate Track Sections... |  | Opens the [Auto Generate Track Sections](GenerateTrackSections.md#TRKAutoGenerate) window |
| Load Track Sections |  | To load a *.poi file. |
| Save Track Sections As |  | Save a *.poi file with the configured sections. |
| Add Section | + | Adds a new section at the end of the list. See [Manual Generate Track Section](GenerateTrackSections.md#TRKManualGenerate) for further details. |
| Insert Section | Insert | Splits in two parts the selected section. See [Manual Generate Track Section](GenerateTrackSections.md#TRKManualGenerate) for further details. |
| Remove Section | Delete | Removes the selected sections. See [Manual Generate Track Section](GenerateTrackSections.md#TRKManualGenerate) for further details. |
| Clear Track Sections |  | Remove all sections. See [Manual Generate Track Section](GenerateTrackSections.md#TRKManualGenerate) for further details. |
  
##### Section file (*.poi)

The section file has .poi extension. A *.poi file is a sector descriptor file, it also contains an internal link to Track when sectors have been generated. If the linked Track is available on the same path of the Track Sectors, it is loaded together with the Track Sectors. To ensure a proper association between Track Sections and Track, it is suggested to call Track Sections and Track files with same name.

#### 

#### Google Earth

The Google Earth menu allows the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Google Earth Link | Opens the [Google Earth](Google_Earth.md) interface. |
| Remove Google Earth map | Remove from preview and from configuration the Google Map. |
  
#### 

#### Tools

The menu Tools allows the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Track Setup... | Opens the [Track Setup Window](TrackSetup.md). |
| Edit Distance... | The command allows the user to set the length of the track manually. |
| Export to CRT | Exports the current track in a *.crt Track file format. |
| Export to CSV | Exports the current track in *.csv file.  If the circuit is standard type, the CSV will have the following format: X position in meters, Y position in meters, Distance in meters. If the circuit is GPS type, the CSV will have the following format: Latitude in degrees, Longitude in degrees, Distance in meters. |
| Import From CSV | Import a track from *.csv file. |
  
#### Menu Help

The Track Sections menu allows the following commands:

| COMMAND | SHORTCUT | DESCRIPTION |
| --- | --- | --- |
| Help | F1 | Opens the WinTAX4 help, if available. |
  
### 

### Toolbar and buttons

The figure below shows the Track Editor toolbar.

The [Graph](../../AnalysisWnds/Wnds/GRA/GRAWndOverview.md) window toolbar allow the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Keep Visible | Allows to keep displayed the window after performing calculations. |
| Apply | Applies the current settings of the Track Editor |
| Cancel | Closes the window without applying the current settings to the configuration of the Track Editor |
| Track Setup | Opens the [Track Setup Window](TrackSetup.md). |
| Auto Generate Track Sections | Opens the [Auto Generate Track Sections](GenerateTrackSections.md#TRKAutoGenerate) window |
| Add Section | Adds a new section at the end of the list. See [Manual Generate Track Section](GenerateTrackSections.md#TRKManualGenerate) for further details. |
| Insert Section | Splits in two parts the selected section. See [Manual Generate Track Section](GenerateTrackSections.md#TRKManualGenerate) for further details. |
| Remove Section | Removes the selected sections. See [Manual Generate Track Section](GenerateTrackSections.md#TRKManualGenerate) for further details. |
| Track Designer | Opens the [Track Designer window](TrackDesigner.md). |
| Google Earth Link | Opens the [Google Earth](Google_Earth.md) interface. |
| Remove Google Earth map | Remove the Google map from preview and from configuration. |
| Open Channel Browser | Shows the pop-up menu to select the page in the Channel Browser window . |
  
### 

### Window commands

On the window there are the following commands:

| COMMAND | DESCRIPTION |
| --- | --- |
| Track Setup | Opens the [Track Setup Window](TrackSetup.md). |
| Auto Generate Track Sections | Opens the [Auto Generate Track Sections](GenerateTrackSections.md#TRKAutoGenerate) window |
| Manual Sections Definitions | This button is displayed only when there is a graph in the preview window (Combo Preview in Graph mode or in Circuit and Graph Mode). See [Manual Generate Track Section](GenerateTrackSections.md#TRKManualGenerate) for further details. |
| Customize Graph Channels | See Customize Graph Channel |
  
###  Pop-up menu

By clicking with the right mouse button on the table section, the following pop-up menu is displayed.

| COMMAND | DESCRIPTION |
| --- | --- |
| Add Section | Adds a new section at the end of the list. See [Manual Generate Track Section](GenerateTrackSections.md#TRKManualGenerate) for further details. |
| Insert Section | Splits in two parts the selected section. See [Manual Generate Track Section](GenerateTrackSections.md#TRKManualGenerate) for further details. |
| Remove Section | Removes the selected sections. See [Manual Generate Track Section](GenerateTrackSections.md#TRKManualGenerate) for further details. |
  
By clicking with the right mouse button on the preview graph window, the following pop-up menu is displayed.

| COMMAND | DESCRIPTION |
| --- | --- |
| Zoom Out | Decreases the zoom factor. |
| Zoom In | Increases the zoom factor to magnify details around cursor position. |
| Minimum Zoom | Maximum zoom factor. |
| Maximum Zoom | Minimum zoom factor, whole data are displayed. |
| Zoom Fix Range | Displays the graphic area corresponding to a fixed interval of the X axis. |
| Manual Section Definition | See [Manual Generate Track Section](GenerateTrackSections.md#TRKManualGenerate) for further details. |
  
### 

### Keyboard Shortcut

To see the complete list of shortcuts available for the graph window, click [here](../../Shortcut/WindowsShortcuts.md#ShortcutGraph).

Below are listed those shortcuts that do not appear in any of the commands and of the toolbar buttons described before. These commands can be executed only from keyboard. All these commands are not customizable.

| SHORTCUT | DESCRIPTION |
| --- | --- |
| Esc | Closes the edit of section cells in the table. |
|  |  |
  
##### 

##### Customize Graph Channel

This button is displayed only when there is a graph in the preview window (Combo Preview in Graph mode or in Circuit and Graph Mode).

As default (Auto), in the graph preview the waveform shows the special channels configured in [General Setup](../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgSpecialChannels). The Speed channel has red color, the Lateral Acceleration is blue and the Throttle is green. 

By the "Customize Channels" radio button the user can add generic channels in to window. The buttons below the channel list are used to add, remove and move the order of channels; drag & drop of channels from Channel Browser is admitted.

##### 

##### Related Issues

  * [Track Editor](Track_Editor.md)

  * [Elements of the window](Elements.md)

  * [Track Setup](TrackSetup.md)

  * [Generate Track Section](GenerateTrackSections.md)

  * [Track Designer](TrackDesigner.md)

  * [Google Earth](Google_Earth.md)


