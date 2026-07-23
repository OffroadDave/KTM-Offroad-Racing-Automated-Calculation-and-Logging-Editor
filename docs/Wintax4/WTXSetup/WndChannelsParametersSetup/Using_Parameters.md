### Using Parameters

#### Import modes

##### Import from CLL

Some channel parameters can be imported directly from the CLL file which is used by Axon and Sysfiles to define the channels in the logging table.

Parameters* which can be imported are

  * Units label

  * Color




* the parameters available in the CLL also depend on whether they are supported by the version of Sysfiles \- see related release notes

##### Import from CLX

Some channel parameters can be directly imported from .CLX files; The .CLX file format is used in SYSMA for defining channels in the logging table.

The type of channels that it is possible to import from CLX depends on the protection level configured and on the licence of WinTAX being used.

The following parameters* can be imported:

  * number of decimals

  * labels for units of measure

  * output format

  * minimum and maximum values used for AutoScale range

  * colors

  * list of enums




##### *Parameters available in .CLX format for import also depend on whether they are supported by the version of Sysma - see related notes.

##### Import from Data

All channel parameters can be imported directly from the logged Data.

##### Import from VCH

All Virtual channel parameters can be imported directly from the Global libraries.

#### Reset All Windows to Default

This command restores all windows to the default configurations.

A pop-up message asks if the operation has to be executed only for the active user or for all users of WinTAX4. 

Vice versa it is possible to use a local window setting as default for all other configurations. This options is available on the Windows setup page:

The figure shows the Set As Default command starting from a Graph configuration page.

##### Related Issues

  * [Channel Parameters](Channels_Parameters.md)

  * [Local Groups](LocalGroups.md)


