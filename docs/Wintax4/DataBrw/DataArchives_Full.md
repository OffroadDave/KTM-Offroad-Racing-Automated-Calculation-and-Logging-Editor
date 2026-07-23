## Data Archives

The basic format of WinTAX4 data is ZTX. WinTAX4 archives and it is compatible with the old formats of DTX data available in WinTAX2 and WinTAX3.

### ZTX Archive

The ZTX archive is a well-defined hierarchy of directories containing data files and context information about data acquisition sessions. Context information are stored in XML files created by WinTAX4. The archive can contain other data and files not handled by WinTAX4 as long as there are no naming conflicts (e.g. the archive can contain User data in various format. This data can be loaded in WinTAX4 via [Automation](../Scripting/ScriptExamples/Automated_read_userData.md)).

The data structure is based on six levels of directories.

The basic directory is set in Setup/General and corresponds to the Data directory.

Then, following, it's possible to find in a nested format the following folders: 

  1. Data

  2. Track

  3. Session

  4. Device

  5. Run

  6. Lap




### Run Directory

The RUN directory contains data and information for a single run/outing.

It is named with a well-defined name format as explained in the appropriate section below (e.g. RUN_TrackRun).

It contains the following items:

  1. One or more LAP directories with well-defined names.

  2. A Session Record file named Session.xml

  3. A Device Record file named Car.xml

  4. A Comments Record file named RunComments.xml

  5. A Lap Header Record file named RunLapHeaders.xml

  6. A Offset & Gain file named RunOffset.xml

  7. A User Record file named RunUserRecords.xml

  8. A constant file named RunConstants.xml




### Lap Directory

The Lap directory contains information about a single lap. It is named with a well-defined format as explained below.

It contains the following items:

  1. One or more ZTX files containing raw and binary data (e.g. dstData.ztx, cableData.ztx, nbtData.ztx). (It is also possible to double click a ZTX file to start WinTAX and load the file.)

  2. Lap Header Record file named LapHeader.xml .

  3. Constants Record file named Constants.xml .

  4. Offsets Record file named Offsets.xml.

  5. User Record file named UserRecords.xml.

  6. Meteo Record file named Meteo.xml .

  7. Comments Record file named Comments.xml .




### Lap Directory Name

The Lap Directory name follows some simple rules in its construction. The base model of the lap directory name is:

Lap_Abs_SN 

where:

  * Lap is a fixed tag to identify the object

  * Abs is the abs value extract from Lap Header and is a number of minimum 8 digits eventually filled with zeros if shortest.

  * SN is the serial number extract from Lap Header and is a number of minimum 5 digits eventually filled with zeros if shortest.




If the folder is already present, a zero-based counter of three characters is added:

Lap_Abs_SN~nnn

If the lap is marked as Trigger Out, a zero-based counter of three characters is always added and the separator is #:

Lap_Abs#SN~nnn

If the lap is marked as Auto Limit, a zero-based counter of three characters is always added and the separator is @:

Lap_Abs@SN~nnn