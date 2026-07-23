## Real time telemetry

Acquisition process of Real Time Data.

All commands and their procedures to acquire Real Time data are described here.

##### Real time bar: list of the devices available

Each device can be started and stopped with a double click. The status colors of the icon are as follows: 

| STATUS | DESCRIPTION |
| --- | --- |
|  | Process enabled and connection with the device displayed in the combo |
|  | Process enabled and connection failed |
|  | Process not active |
|  | The stream is connected but has an incompatible license |
  
###  Configuration of the acquisition process

Logging Table: points to the directory where WinTAX4 looks for the logging table (*.DLS) to be used with the Real time data stream; with WTS server it is not used.

Real Time Data Source : selects the Real time device or stream to receive data in local master mode. For WTS the stream is selected before the Real time process is activated.

Data Acquisition : options to enable detection of the logging trigger (as defined in logging table).

| OPTIONS | DESCRIPTION |
| --- | --- |
| Acquisition trigger | Enables detection of the trigger condition. Always to be left enabled |
| Save trigger OUT laps | Enables WinTAX4 to save Real time data even when logger on device is not saving (out of trigger). This option should be left disabled except for debug purposes. |
| No-Rx Timeout (sec.) | Will close the current Real time lap if no data is received for a specified time This option is useful to ensure that data is captured when the device stops off track |
| Sudden Close  On Trigger Out | Inhibits logging post-trigger time |
| Don't save  Real Time Data | If this flag is ON, Real Time process will not save dstData.ztx file and nbtData.ztx files |
| Real-Time buffer | Allows scrolling back in the real time Graph windows for the |
  
## 

### Multi Device Acquisition

Multi Device Acquisition allows users to acquire and store one or more real time streams simultaneously.

The configurations are the same of the standard acquisition except the configuration of the Real Time Data Source:

In the screen below it is possible to select and activate up to five streams

In WinTAX each real-time window can be associated to one of five real time streams

###  Acquisition Mode

Select the method used by WinTAX to receive Real time data:

| OPTIONS | DESCRIPTION |
| --- | --- |
| Standard | Allows to connect directly to a PBE/GRX receiver |
| WinTAX4  Telemetry Server | Uses the WTS network distribution infrastructure. If enabled the user can optionally |

  * configure the server used for context information
  * automatically switch to server when connected to stream

  
  
Note: After 10 hours of consecutive acquisition, real time windows will be reset simulating a start over.