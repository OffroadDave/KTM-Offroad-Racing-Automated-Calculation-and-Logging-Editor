### Window Elements

The Symbols Led window is composed by a grid area where each cell of the grid shows the configured bitmap (or led) calculated by conditions.

#### Information Area

This area displays the messages associated to the verified condition at the current position. The window manages one or more channels and works with single lap or with compare of laps. In the case of comparison a small colored square to the left side of the channel will be shown to identify the lap related.

The window will be divided in several sections, accordingly to the number of configured and valid channels. The layout modes for channels distribution are Horizontal, Vertical and Automatic. In the picture above the layout is Auto, in the picture below is Horizontal.

If one or more channels are configured in setup page but it is not available within the current loaded dataset, it will not be displayed in window. Each cell has a fixed size that depend on the selected value of the configuration parameter Size that can be Small or Normal (default).

The length of the text displayed depends on the bitmap size. The eventual overflowing text is truncated. Hovering with the mouse on one of the icons will show a tooltip that displays the full name of the channel and its actual value.

#### Bitmap

In the following table it is possible to see the whole list of the images used in the window. 

| Name | OK | Alarm | Name | OK | Alarm |
| --- | --- | --- | --- | --- | --- |
| Generic |  |  | Brake |  |  |
| Temperature |  |  | Fuel |  |  |
| Oil Pressure |  |  | Tire pressure |  |  |
| Oil Level |  |  | Engine |  |  |
| Battery |  |  | Power Steering |  |  |
| ABS |  |  | Light |  |  |
  
##### Related Issues

  * [Symbols Led window](SymbolsLedFeatures.md)

  * [Configuration](SymbolsLedConfiguration.md)

  * [Commands](SymbolsLedCommands.md)


