[Main Page](index.md) | [Class Hierarchy](hierarchy.md) | [Class List](annotated.md) | [Class Members](functions.md) | [Related Pages](pages.md)

# Predefined DataLoad Properties

The following list shows the predefined properties which may be obtained from the [ITelDataLoad::Properties](interfaceITelDataLoad.md#a3) collection. The list may be extended in the future to handle new types of information in the archive. 

##  Properties

| Property | Description |
| --- | --- |
| **"FileName"** | File name (ZTX or DTX) of the archive associated with DataLoad object |
| **"FilePath"** | File path (folder) of the archive associated with DataLoad object |
| **"TrackName"** | Track name / Event name |
| **"SessionName"** | Session name |
| **"SessionComment"** | Session Comment |
| **"TrackLen"** | Length of the circuit track |
| **"RunComment"** | Comment associated with the Run (if available) |
| **"LapComment"** | Comment associated with the Lap (if available) |
| **"DeviceName"** | Device name |
| **"CarName"** | Device name (**obsolete**) |
| **"DriverName"** | Driver name. |
| **"DriverComment"** | Driver comment. |
| **"EngineName"** | Engine name |
| **"BoxNum"** | Box number |
| **"DownloadTime"** | Date/hour of download Lap to PC. |
| **"LapSize"** | Physical size of the lap in bytes (if available) |
| **"AbsFreeCountStart"** | ABS block of lap start (if available) |
| **"AbsFreeCountEnd"** | ABS block of lap end (if available) |
| **"TableIndex"** | Index of the table inside the logger memory (if available) |
| **"VerTLap"** | Lap Header format version (if available) |
| **"OpenType"** | Event that opened the lap (eg."Beacon") (if available) |
| **"CloseType"** | Event that closed the lap (eg."Beacon") (if available) |
| **"SoftwareRelease"** | Software release of the logger that generated the lap (if available) |
| **"LoggerName"** | Name of the logger that generated the lap (if available) |
| **"CarAlias"** | Car alias of the logger (if available) |
| **"AcquireTime"** | Date/hour of End Of Lap time. |
| **"TableName"** | Logging table name and version (if available) |
| **"LapMarker"** | Lap type marker. |
| **"Abs"** | Lap number within the session. |
| **"Run"** | Run number within the session. |
| **"Lap"** | Lap number within the Run. |
| **"TrackLap"** | Lap number within the session. (laps on the track). |
| **"TrackRun"** | Run number within the session. (runs on the track, TR algorithm). |
| **"T98Code"** | DTX compatibility code (T98Code) which is calculated only from the parts of the logging table which are used by WINTAX to display the data (i.e. is not affected by logging trigger/dashboard/DST map configurations etc.). It is embedded in the DTX filename when using Track Run Storage System. |
| **"FuelLevel"** | End of lap fuel level. |
| **"FuelConsumption"** | Lap fuel consumption. |
| **"TempIn"** | Weather station internal temperature sensor (°C) reading. |
| **"TempOut"** | Weather station external temperature sensor reading (°C). |
| **"WindSpeed"** | Wind speed (m/s). |
| **"WindDir"** | Wind direction (degrees). |
| **"Bar"** | Atmospheric pressure (mBar). |
| **"HumidityIn"** | Weather station internal relative humidity sensor reading (). |
| **"HumidityOut"** | Weather station external relative humidity sensor reading (). |
| **"Rain"** | Rainfall (mm). |