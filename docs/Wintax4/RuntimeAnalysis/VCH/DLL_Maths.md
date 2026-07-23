### DLL Maths

Within the math channels environment of WinTAX4 / TelDataX it will be possible to select a series of parametric Math Client Functions in addition to the standard math functions (SIN, COS, INTEG, etc).

These functions will be available in the Virtual Channels expression of WinTAX4. In this way a maths channel may directly return the result of a Client Function or it may include it within another function. Several math channels may call the same Client Function passing different parameters in order to obtain different results. Via the Virtual Channels, the user may include his own function in every point where WinTAX4 uses a channel (virtual and logged).

#### DLL Registration

At startup, WinTAX4 checks for math DLLs in a specific folder ( \WinTAX4\PlugIns\Math\ where \WinTAX4\ is the working folder which contains WinTAX4.exe and \PlugIns\ is the folder configured in [Setup/General/Directories](../../WTXSetup/WndGeneralSetup/WTXSetupGeneralSetup.md#GeneralSetupPgDirectories) as folder type PlugIn ). For each DLL found, WinTAX4 carries out a LoadLibrary and analyses if all the functions required have been implemented. This is done via calls to GetProcAddress: if any call fails (that is, if the function name is not found in the DLL), WinTAX4 will log an error message in the Log Window, with a description of the failed operation, the DLL file name and the function name.

Not all the functions need to be implemented in the DLL, but if a DLL does not implement the minimum number of functions required, it will not be validated and therefore it will be ignored.

Here is a list of the required functions:

  * QueryMathComponentNumFunctions

  * QueryMathComponentName

  * QueryMathFunctionName

  * QueryMathFunctionDataTransferType

  * QueryMathFunctionNumParameters




Both successful and failed DLL registration results are logged in WinTAX4 [Log Window](../../WTXWorkWindows/WndLogWindow/WndLogWindow.md) .

During the registration phase, WinTAX4 calls some query functions to determine what user functions are implemented and how many parameters are needed. It calls QueryMathComponentNumFunctions, QueryMathComponentName, QueryMathFunctionName, QueryMathFunctionNumParameters, QueryMathFunctionDataTransferType, and QueryMathParameterName. Every error is logged in the log window.

During the session, WinTAX4 and TelDataX will not make further calls to GetProcAddress: functions pointers of the validated DLLs are stored during start up. Therefore, a DLL cannot be replaced during a WinTAX4 session: close WinTAX4, change the DLL file and then restart WinTAX4.

TelDataX registration sequence is similar to WinTAX4 , but it is done only when the user tool calls ITelRun::SetPropertyData with the special property PROPERTYTEXT_MathDLLEnabled set to 1. In this case TelDataX proceeds with the same registration sequence of WinTAX4. In case of conflicts, the registration stops at the first conflict found. It is possible to get the last error message via a call to ITelRun::GetPropertyData with the special property PROPERTYTEXT_MathDLLError.

When closed, WinTAX4 releases the registered DLLs. TelDataX releases the DLLs when the last ITelRun object is released.

Both WinTAX4 and TelDataX run the DLL registration loop in alphabetical order, case insensitive (eg Alpha.dll is always registered before Zone.dll).

#### Editing math channels

When the user needs to configure a math channel, WinTAX4 displays in pop-up menus and tooltips the names and descriptions of the Component, the Math Functions and the Parameters which were found during the start up phase in the DLLs.

The information displayed in the GUI is the same returned by the DLLs via the functions QueryMathComponentName, QueryMathComponentDescription, QueryMathFunctionName, QueryMathFunctionDescription, QueryMathParameterName, QueryMathParameterDescription.

In order to view the list of client functions click on Show Math DLL Functions button (highlighted in the following figure). This button will be activated only if at least one client function is available.