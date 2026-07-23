## High Speed Camera - Video Acquisition

The "High speed camera" video acquisition function can be used to download a set of laps and videos data from a supported HSC device. 

It's possible to use this feature clicking on the Acquisition - Acquisition manager menu. (on enabled licenses).

### Commands 

| Command | Description |
| --- | --- |
|  | Activate TCP (not connected, error, connected) |
|  | Download video and data |
|  | Download only video files |
  
#### Start a TCP connection to the HSC device. Upon load, the combo box of the hardware list () in the acquisition manager screen will be automatically updated with the list of the devices available on the network.

After choosing a supported hardware from the hardware list and clicking on the "Activate TCP" button, the 2 acquisition icons indicated above will become enabled and colored.

#### Start a DATA + HSC VIDEO downloading session

To start a DATA + HSC VIDEO acquisition session, press the button: Activate TCP ( )

When the icon show a connected status (), the hardware is connected and it is possible to start downloading data.

The "Download Data" () button allows the selection with the green arrow allows opening the "Download Data" window to allow selection and receive both telemetry data and video data at the same time.

Download only HSC VIDEO + DATA: Each lap that has to be downloaded needs to be selected. (in the following example, all data and all videos will be downloaded.)

| Icon | Description |
| --- | --- |
| No icon | There is no video connected to this lap |
|  | There is a video connected to this lap |
|  | The video that is connected to this lap is protected against deleting. |
  
Each lap for which you want to download video needs to have the checkbox in the "E" column enabled and needs to be selected. If the "E" checkbox of a lap is selected, but the lap is not selected, when the "OK" button is pressed, nothing is downloaded.

#### Start a "HSC VIDEO" download session

To download only videos files from the hardware, press the button: Activate TCP ( )

When the icon becomes green (), the hardware is connected and it is possible to start downloading data.

The "media" icon () allows the download only of "video" files. After clicking it, the following window appears.

| Icon | Explanation |
| --- | --- |
| No icon | The video has not yet been downloaded. |
|  | This video has already been downloaded and is stored on the drive. |
|  | This video has been locked and cannot be deleted from the device. |
  
On all versions of WinTAX enabled to download HSC videos, the DataBrowser will display near a lap that is associated to a video a icon.

####  HSC data Transfer

After clicking OK the data transfer starts. (both in video only and in data & video.)

The status of the transfer is visualized by a progress bar like in the picture below.