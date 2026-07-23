## Command line operations

##### <file.dtx>, <dstdata.ztx>, <cabledata.ztx>

Loads the specified data file (ZTX or DTX,other file formats are ignored). Specify the full path to the ZTX file.

Example:

WinTAX4 c:\WinTAX4\data\track\session\device\run_003\lap_00001234_00023\cabledata.ztx

WinTAX4 c:\WinTAX4\data\track\session\device\2405201T.DTX

##### <file.wly>

To open the specified .WLY layout file, just specify the file name to load a layout from the current user directory. To load a layout from another user specify the full path

Example:

WinTAX4 myLayout.wly

WinTAX4 c:\WinTAX4\users\another\hisLayout.wly

##### -curtime<F>

Places the analysis window cursor in the specified position (value expressed in seconds)

Example:

WinTAX4 -curtime10.5

##### -starttime<F>, -endtime<F>

Zooms in the current analysis windows to the specified limits (values expressed in seconds)

Example:

WinTAX4 -curtime10.5 -starttime0 -endtime20

##### -password

Opens the Select Password window

Example:

WinTAX4 -password

##### -FreezeDST

Creates a (user-defined) data file in the WinTAX4\work directory containing the DST data from last track marker beacon to current instant. The file is automatically loaded and displayed by WinTAX4.

Example:

WinTAX4 -FreezeDST c:\WinTAX4\data\dstData.ztx

(Saves dstData.ztx in c:\WinTAX4\data\ and loads the file to memory.)

##### -Filter[all|cable|dst]

Sets data type filter (all/cable/dst) for WinTAX4. It is used to ensure that when you load a data file of a certain type, any operation such as lap +/-, lap reports does not include other ztx file types.

Example:

WinTAX4 -filter [dst]

##### -LoadLastLayout

Opens the last layout of the current user.

WinTAX4 c:\WinTAX4\data\track\session\device\run_003\lap_00001234_00023\cabledata.ztx -LoadLastLayout

##### **-load: <DatasetNumber>;<filePath1>;< filePath2>**

Loads in the dataset “DatasetNumber” the data files specified in the arguments (FilePath1, FilePath2) .

In input the command requires the dataset number (from 0 to 9) and one or multiple Wintax file path(s) (relative or absolute).

The "DatasetNumber" must be a number between 0 and 9 – if it is not, the file in the related load command is not loaded.

The "FilePath" option must be a path that contains a Wintax filename (ZTX or DTX,other file formats are ignored) and the format can be absolute or relative (from the command line path). To use append lap mode, the paths of multiple laps need to be separated by a “;” character. (semicolon)

Multiple load parameters are permitted in the same command.

_Example:_

_Example for compare lap:_

WinTAX4 –load:2;”c:\WinTAX4\data\track\session\device\run_003\lap_00001234_000023\cabledata.ztx” -load:6;”c:\WinTAX4\data\track\session\device\run_004\lap_00001234_000056\cabledata.ztx”

_Example for append laps:_

WinTAX4 –load:2;”.\run_003\lap_00001234_00023\cabledata.ztx”;”run_004\lap_00001234_00056\cabledata.ztx”

##### 

##### **-parameter: <ParameterFilePath.txt>**

The command requires the file path (relative or absolute) of a text file to be interpreted as command/parameter text.

The parameter file must be compliant with Wintax Command line syntax.

It must contain a list of one or more commands and parameters (that must be separated by at least one space between them, or written on per line).

_Example:_

WinTAX4 –paramenter:”.\OpenDataCompare.txt”

**Parameter file “.\OpenDataCompare.txt”:**

-load:0;”.\run\lap\cabledata.ztx”

-load:1;”.\run\lap\dstData.ztx”

-load:2;”.\run\lap\FlashData.ztx”

-curtime10.5

-starttime0 –endtime20

-load:4;”.\run\lap01\cabledata.ztx”;”.\run\lap02\cabledata.ztx”

myLayout.wly

##### 

##### -QuitApp

Closes WinTAX4 application.

WinTAX4 -QuitApp

##### -setlanguage

Opens the window for selecting languages.

WinTAX4 -setlanguage

##### -ImportASCII

Import data from ASCII files.

In input the command requires the full name of ASCII file (ASCIIFileName) and the full name of configuration file (CfgFileName).

WinTAX4 -ImportASCII:ASCIIFileName;CfgFileName