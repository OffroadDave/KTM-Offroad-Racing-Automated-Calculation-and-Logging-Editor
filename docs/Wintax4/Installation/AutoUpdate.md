## Check for Updates

Check for Updates allows updating WinTAX4 to the latest version (or to any other version that is present in the installation folder. (see below for more information.) The updates can be configured to be automatically or manually detected. The update configuration is in the [central_configuration.xml](central_config.md#AutoUpdate) file, in the directory WinTAX4\System. For the update system to work correctly, the update must be stored on a "Server" in a predetermined folder containing both the WinTAX4 installer and the "Version.xml" file which contains the ID of the new version. Updates can be automatic or manual and in both cases there are two possibilities:

  * The VerToUpdate parameter is 0: the update is carried out in any case (even if there is an older version.)



  * The VerToUpdate parameter is 1: the update is performed only if there is a newer version of the software.




### Automatic updates

The central_configuration.xml file must have the EnableStartUp element set to 1. The update window is displayed immediately after the splash screen asking whether to continue the update procedure. The following window is displayed:

Fig. 1

By clicking on OK, WinTAX4 is closed and the Setup is launched; by clicking on Cancel the update is cancelled.

### Manual updates

The manual update can be run from the Help/Check for Updates menu; the following window shows the central_configuration.xml parameters.

Fig. 2

Clicking on Check for Updates a control similar to the one of the automatic startup is carried out and the update procedure window is displayed (Fig. 1). If the central_configuration.xml file is missing from the system, when the manual update is requested the following message is displayed:

Fig. 3

In this case updates are not possible because there is no configured path.