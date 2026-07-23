## Excel troubleshooting

1\. Excel shows the following error message: “The requested operation required elevation or error n. -2147418113” when using COM object to open WinTAX.

This problem can happen when WinTAX and Excel do not have the same “Elevation privileges".

WinTAX requires elevated privileges to work correctly, while Excel by default can run with no elevated privileges. (when clicking on the taskbar, or double clicking on a document for example)

When Excel is used to open another program, via OLE for example, Excel passes the same privilege level to the opened program. In case of WinTAX this causes an error message. 

The problem can be corrected by executing Excel with elevated privileges, by right clicking on the Excel Icon and choosing “Run as administrator” or by using WinTAX in the "Standard user" configuration.

* * *