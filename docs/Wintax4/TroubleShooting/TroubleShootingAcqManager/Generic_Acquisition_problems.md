## Acquisition Manager Troubleshooting

1\. The "acquisition manager" shows one of the following messages when trying to connect to ECU in TCP mode:

  * [TCP] Error on receiving data: inactivity timeout reached.

  * [TCP] Request laps list failed

  * [TCP] Request laps list failed [Received nack from device]

  * [ACQ] Download laps list failed




This usually happens when there is a connectivity issue between the computer and the ECU. Usually together with these error messages also the following message:

"The Windows Firewall could be active and could cause acquisition problems. It is recommended to turn it off."

Some antivirus software (with integrated firewall) could also be blocking partially or completely a 2 way communication with the ECU. In case of problems, it

If you have a firewall enabled, and are experiencing connectivity issues to the ECU, it is highly suggested to disable the Windows Firewall and any other security software that could be causing problems before starting WinTAX.

* * *