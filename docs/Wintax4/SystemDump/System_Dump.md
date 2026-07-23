In the shipped version, WinTAX4 may have crashes that were not visible during the development cycle. To be able to fix the problems, the development team needs to know where and why the program is crashing.

WinTAX4 requires to catch every exception that lead to a crash in the main routine of its executable modules, and needs to create a record of information through the use of an API called MiniDumpWriteDump contained in a Microsoft module called dbghelp.dll. This information can help to quickly fix the problem.

The information dumps are automatically saved in the \WinTAX4\Dump directory, if \WinTAX4\ is the working folder that contains WinTAX4.exe.

When a crash situation is detected, the WinTAX4 crash manager tool can also send an e-mail (after user confirmation) to the support team with all the information needed. To configure the e-mail for system dump see [central_configuration.xml](../Installation/central_config.md#EMailServer).

The default filename format for minidump files will be:

[COMPUTER]_[COMPONENT]_[VERSION]_YYMMDD_HHMMSS.dmp

Where [COMPUTER] is the computer name in which WinTAX4 is run, and [COMPONENT] is a three letter word that identifies the WinTAX4 component that crashed. An example of dump file will be: PC1_WTX_4.0.3.1_061221_153956.dmp