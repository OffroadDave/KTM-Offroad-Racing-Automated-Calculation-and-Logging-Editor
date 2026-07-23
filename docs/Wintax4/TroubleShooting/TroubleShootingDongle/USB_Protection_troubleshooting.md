## USB Protection Troubleshooting:

Please find below a series of error messages related to the hardware protection. In case you have problems, please check this section:

“Sentinel Hasp Key not found H0007”:

The dongle cannot be detected. Please verify that the “LDK Sentinel Manager” is enabled. Push (Win+R) and write “Service.msc” in the “RUN” box. This will load the Services manager. The “Sentinel LDK License Manager” needs to have the “Startup Type” parameter configured to “automatic” and the status should be “Running”. 

\- “Sentinel License Manager version is too old. H0011” (or “Run-Time version too old. H0011”)

\- “H0042 License Manager too old”

The license manager version that is installed is too old. It is suggested to update the protection drivers to the latest available release version. 

“Real-time clock has no power – H00025”:

The battery of the TIME protection that is being used is out of power. Please contact Magneti Marelli for a solution. 

“Terminal Services detected - H0027”:

The software has been started on a computer that is running on a remote desktop session. Please ensure that the software is executed on the local pc. 

“The USB KEY you are currently using is not valid for this copy of WinTAX. H0031”:

The protection that has been inserted in the USB port is not enabled to execute the software. 

Insert a compatible protection to execute the software. 

“Sentinel Key error H0033”:

The most probable cause could be the antivirus. Ensure that the Sentinel software has been added to the exceptions of the antivirus program or disable the antivirus. 

“Feature has expired - H0041”

The TIME protection that is being used has an expired date. Contact Magneti Marelli for a solution.

“No matching Feature found. H0050”:

The protection is not inserted in the USB port or it is not correctly working. Insert the protection in the USB port and check if the L.E.D. is active. 

“Hardware Key protection failure”:

Communication error between remote and local and remote license server. Ensure that security programs are not blocking the program. Deactivate them or disable them to ensure that the dongle works correctly. 

“Hasp LM is not running”

There is a communication problem with the “license server”. If the problem persists, it is suggested to reinstall the program choosing to update the drivers. 

#### Other generic problems related to the usage of a hardware protection: 

Problem 1:

The web interface http://127.0.0.1:1947/ is not visible and the software does not start. The firewall or the antivirus are probably filtering or blocking connections towards the “license manager” service. Please ensure that all firewalls and antivirus software installed on the PC are disabled. 

Problem 2:

Starting the program is extremely slow. The problem is probably caused by a version of the Sentinel driver which is old. 

Problem 3:

The USB key is not detected. When the dongle is inserted in the USB port, Windows does not detect it. The problem could be caused by a driver which is not installed correctly or by a hardware problem in the dongle. First try to reinstall the software, choosing to update the dongle driver during the installation. If this does not solve the problem try with a different dongle. 

Problem 4:

The LED on the dongle does not light up. The problem might be caused by a hardware problem of the dongle. Try the dongle on a different computer and if the problem persists, contact Magneti Marelli to evaluate if the problem could be solved by changing the dongle. 

Problem 5:

The LED is lit but opening the page http://127.0.0.1:1947/ the key does not show up: Try the dongle on a different computer to ensure that it is not broken.