## Matlab troubleshooting

1\. Matlab shows the following error message: “The requested operation requires elevation” when using COM object / ActiveX to open or interact with WinTAX.

This problem can happen when WinTAX and Matlab do not have the same “Elevation privileges".

WinTAX requires elevated privileges to work correctly, while other programs by default can run with no elevated privileges.

When Matlab is used to open another program (or interact with another program) via ActiveX for example, Matlab passes or needs the same privilege level of the called program. In case of WinTAX this causes an error message. 

The problem can be corrected by executing Matlab with elevated privileges, by right clicking on the Matlab Icon and choosing “Run as administrator” or by using WinTAX in the "Standard user" configuration.

* * *