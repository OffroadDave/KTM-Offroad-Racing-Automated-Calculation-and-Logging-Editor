## Central configuration

The central_configuration.xml file allows you to define certain default configurations during the installation. It is intended to be used by the person in charge of the management and distribution of WinTAX4 to the users of a team.

It is an uncompressed xml file which if present in the installation kit during the installation, gets copied to WinTAX4\System and is subsequently used by WinTAX4.

### Auto update section

You can define the Auto Update path and settings using the following XML keys. See also [Check for Updates](AutoUpdate.md).

  * Path element: used to define path of Setup.exe.

  * EnableStartUp element: 1 enables to check new versions on startup; 0 checks new versions only on request.

  * VerToUpdate element: used to toggle the check logic between new version (value 1) and different version (value 0).




Sample of auto update section:

<Central_Configuration>

<Autoupdate>

<Path>E:\software\WinTAX4\installer\</Path>

<EnableStartUp>1</EnableStartUp>

<VerToUpdate>0</VerToUpdate>

</Autoupdate>

</Central_Configuration>

###  E-mail server section

The EmailData key is used to define the parameters of an e-mail server and e-mail address used by WinTAX4 when creating an error dump output. The user can choose to send the dump file in an e-mail together with his comments. See also [System Dump](../SystemDump/System_Dump.md).

Sample of email server section:

<Central_Configuration>

<EmailData>

<SmtpServer>MySmtpSvr</SmtpServer>

<FromDomain>RacingTeam.com</FromDomain>

<To>joe.bloggs@RacingTeam.com</To>

</EmailData>

</Central_Configuration>