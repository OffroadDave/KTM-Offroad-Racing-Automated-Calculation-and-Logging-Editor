### System events

The following pre-defined system events are available

  * #### Tn events




A Tn event is a pre-configured event defined as the last point of maximum speed within a certain time (or distance) range when the throttle is fully open; it represents a condition of peak engine stress and within a lap and typically occurs at the end of the main straight. When searching for a Tn event WinTAX4 searches the conditions in the following order: first it applies time/distance window, then it searches for maximum speed within the selected window. Tn exists if max speed sample point corresponds to a full throttle condition, otherwise no Tn is found.

These event requires a speed channel configuration. Speed Channel is configured in [Setup/General/Special Channels](../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgSpecialChannels)

  * #### End Lap




End of Lap occurs when a lap is completed.

  * #### Acquisition trigger IN




Acquisition trigger IN occurs when Trigger Channel goes in range

  * #### Acquisition trigger OUT




Acquisition trigger OUT occurs when Trigger Channel goes out of range

  * #### Acquisition Table Changed




This system event shows the Acquisition Table changes

  * #### New Run




Occurs when a new track run starts

  * #### Start/End Map Section




This system event shows start and end of a track section

These events can be added to a library with the Options/Add... command. Although the algorithm of these events cannot be changed by the user, it is possible to set up the Graph Options. System Events can not be displayed as traces in Graphs, but only with event markers.