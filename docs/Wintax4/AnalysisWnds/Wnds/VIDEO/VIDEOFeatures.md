### Multimedia Window Overview

A Multimedia Window displays video or audio that has been captured from a DVR (digital video recorder) or other source, and has been synchronized with data read from ZTX files that have been downloaded from a generic data-logger connected to the DVR device. 

The following table lists all supported media formats:

| FORMAT | TYPE OF MEDIA | DESCRIPTION |
| --- | --- | --- |
| AVI | Video \+ Audio | Audio Video Interleaved |
| MOV | Video \+ Audio | QuickTime / Mov |
| 3GP | Video \+ Audio | 3GPP File Format |
| H264 | Video \+ Audio | - |
| H265 | Video \+ Audio | - |
| FLV | Video \+ Audio | Flash Video |
| MP4 | Video \+ Audio | MPEG-4 Part 14 / MPEG-4 Part 2 |
| MPEG-TS | Video \+ Audio | MPEG-2 Transport stream |
| MPEG | Video \+ Audio | MPEG-1 Systems / MPEG Program stream |
| MKV | Video \+ Audio | Matroska |
| WMV | Video \+ Audio | Dev.L wmv1 |
| AAC | Audio | Advanced Audio Coding |
| FLAC | Audio | Free Lossless Audio Codec |
| MP3 | Audio | MPEG audio layer 3 |
| WAV | Audio | Wav / Wave (Waveform audio) |
  
Note: Media files are not associated to a single lap but to the entire Run.

The synchronization permits to display the correct frames corresponding to the lap (or the laps) that has/have been loaded. When multiple cameras are available, the images are overlaid in one single video that is divided into more panes, so the video file is just one per Run. 

The channel used for synchronization is identified as PTS which stands for "Presentation Time Stamp". This channel must be configured in the General Setup, in [Video Options](../../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgVideo) page.

Media needs to be in one of the formats listed above. On the "Tools" Menu (in the DataBrowser) it's possible to select the function [Import from Video](../../../DataBrw/Import_from_Video.md) is available. This function allows the user to select a file with one of the media formats visualized above in This function permit to convert an mpg files into AVI format and put it in the VIDEO directory of the selected run.

If the data contains a media file, an icon appears in the Abs column in the Data Browser.

For the Multimedia window to function correctly, some multimedia components are needed. These components are installed during the setup.

##### Related Issues

  * [Analysis windows](../../AnalysisWndsOverview.md)

  * [Elements of the window](VIDEOWndItems.md)

  * [Functions](VIDEOFunctions.md)

  * [Configuration](VIDEOConfiguration.md)

  * [Commands](VIDEOCommand.md)


