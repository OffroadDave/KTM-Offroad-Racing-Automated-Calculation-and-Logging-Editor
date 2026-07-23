## Admin user Installation:

Please remember that the following limitations / characteristics are to be considered when installing a ADMIN USER version of WinTAX:

  * WinTAX always requires to be installed and executed with elevated privileges.

  * Data is always saved inside the folder where WinTAX is installed. This path can be customized.




### First time installation:

  1. Run setup.exe from the installer package

  2. Choose the install path where you wish to install WinTAX4 - The install procedure will propose a default path inside the "C:\Program files\" directory or the "C:\Program files (X86)" directory, but this path can be changed.




  3. After the installation is complete, run WinTAX4.

  4. If the license is protected with password, you might need to proceed [with the registration procedure on the first execution of WinTAX.](Registration.md)

  5. If the license is protected with hardware protection, you must ensure that it is inserted in the USB port during install and execution.

  6. **Note: To run WinTAX4 correctly, the user must have administrator privileges.**




### Subsequent installations

  1. Close WinTAX4

  2. Run setup.exe from the installer package

  3. The setup procedure allows the users to install multiple instances of WinTAX4: users can select between "Install a new instance of this application" and "Maintain or update the instance of this application selected below"

  4. Install a new instance of this application:

     1. Return to Step2 of first time installation.

  5. Maintain or update the instance of this application selected below:

     1. All detected instances of WinTAX4 will be listed. Select one from the list to maintain it.

     2. Select Update/Repair then continue

  6. The install procedure might ask for a reboot at the end of installation. If this happens please make sure to reboot to prevent installation problems.




To avoid this situation, please carry out the following procedure before installation

  1.      * Open Task Manager and check if any WinTAX4 process ("Wtx...") is still running.

     * Close these processes.




### Folders created by the installer in the folder where WinTAX has been installed:

###### Note: These paths can be fully customized during the install procedure: Default 32Bit Install Path: C:\Program Files(x86)\WinTax + License Name\ Default 64Bit Install Path: C:\Program Files\WinTax + License Name\

Docs folder: Contains software documentation, like WinTAX manual

Drivers folder: Contains drivers relative to peripherals that might need to work in conjunction with WinTAX. (like Can-Devices.)

Languages folder: Contains resource files with the text that is used on the interface of WINTAX to visualize the program in other languages.

Samples folder: C code samples for Wintax Automation. 

System folder: Contains System files and libraries needed to execute and correctly run the software.

### Folders created by WinTAX upon the first execution:

Circuit folder: Contains circuit files

Data folder: Contains data downloaded from loggers.

Export folder: Contains data that has been exported using one of the export functions (es. Export user, Export to ZTX, Export to EXP, etc.)

Libraries folder: Contains library files like VCH, Conditions, Events, Lookups, etc.

log folder: Contains log of the program if loggins output has been enabled.

Plugins folder: Contains plugin files for various functions.

Users folder: Contains layouts, condition settings, Local Vhc, Events settings and windows.