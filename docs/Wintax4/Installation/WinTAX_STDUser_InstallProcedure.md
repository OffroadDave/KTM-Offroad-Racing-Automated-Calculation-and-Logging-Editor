## Standard User Installation:

Please remember that the following limitations / characteristics are to be considered when installing a STANDARD USER version of WinTAX:

  * WinTAX always requires to be installed with elevated privileges. There is no need for elevated privileges during the execution of WinTAX.

  * Only 1 installation instance of WinTAX is permitted. No other instance of WINTAX can be installed on the system if you install a STANDARD USER version.

  * Data is always saved in the public document folder

  * The zip installation packet is always marked with the suffix "_STDUSER".




### First time installation:

IMPORTANT: The install procedure is always required to be executed with elevated privileges:

  1. Run setup.exe from the installer package

  2. The Standard user version is always installed in the "C:\Program Files\WinTAX4\" or "C:\Program Files(x86)\WinTAX4\" folder. This version should not be installed multiple times. The installation path cannot be customized.

  3. After the installation is complete, run WinTAX4.

  4. If the license is protected with password protection, you might need to proceed [with the registration procedure on the first execution of WinTAX.](Registration.md)




### Subsequent installations:

Since WinTAX components need to be registered in the registry and this can only be done with elevated privileges, it is possible to execute only one instance of WinTAX on each computer when using the "Standard user" version. 

For this reason, the install procedure will warn you, before copying the files that you need to remove all other instances of WinTAX in the computer.

If another instance of WinTAX is already installed and you wish to transform this installation in a "Standard user" version or if you wish to update a previously installed version, the Install procedure will show you other instances of WinTAX already installed on the computer. Select the instance you want to transform/upgrade and click on "NEXT".

PLEASE REMEMBER: only the last installed version of WinTAX STANDARD USER will work correctly. If you execute a normal version of WinTAX the already installed version of WinTAX STANDARD USER will stop working correctly until it has been reinstalled.

### Folders created by the installer in the %Programfiles% directory.

%Programfiles%/Docs folder: Contains software documentation, like the WinTAX manual

%Programfiles%/WinTAX4/Drivers folder: Contains drivers for peripherals that might need to work in conjunction with WinTAX. (like Can-Devices.)

%Programfiles%/WinTAX4/Languages folder: Contains resources for localized versions of WinTAX.

%Programfiles%/WinTAX4/Samples folder: C code samples for Wintax Automation. 

### Folders created by the installer in the "C:\Users\Public\Documents\" directory:

C:\Users\Public\Documents\WinTAX4/System folder: Contains System files and libraries needed to execute and correctly run the software.

C:\Users\Public\Documents\WinTAX4/Plugin folder: Contains System files and libraries needed to execute and correctly run the software.

### Folders created by WinTAX upon the first execution:

C:\Users\Public\Documents\WinTAX4/Circuit folder: Contains circuit files

C:\Users\Public\Documents\WinTAX4/Data folder: Contains data downloaded from loggers.

C:\Users\Public\Documents\WinTAX4/Export folder: Contains data that has been exported using one of the export functions (es. Export user, Export to ZTX, Export to EXP, etc.)

C:\Users\Public\Documents\WinTAX4/Libraries folder: Contains library files like VCH, Conditions, Events, Lookups, etc.

C:\Users\Public\Documents\WinTAX4/log folder: Contains log of the program if loggins output has been enabled.

C:\Users\Public\Documents\WinTAX4/Plugins folder: Contains plugin files for various functions.

C:\Users\Public\Documents\WinTAX4/Users folder: Contains layouts, condition settings, Local Vhc, Events settings and Analisys windows.