# HUC: Various command line tools to make your life easier
[Max Run Software](https://www.maxrunsoftware.com)

HUC is a simple to use open source command line tool for performing various tasks including...
- [FTP](#ftp-ftps-sftp)
- [FTPS](#ftp-ftps-sftp)
- [SFTP](#ftp-ftps-sftp)
- [Email](#email)
- [Delimited data conversion](#delimited-files)
- [MSSQL/MySQL/Oracle querying](#sql)
- [MSSQL/MySQL importing data into a table](#sql)
- [ZIP](#zip)
- [Windows Task Scheduler Management](#windows-task-scheduler)
- [File String Replacement](#file-replacement)
- [File Appending](#file-appending)
- [File Split](#file-split)
- [File Checksums](#file-checksum)
- [Directory Flatten](#directory-flatten)
- [Directory Remove Empty](#directory-remove-empty)
- [Web Server](#web-server)
- [SSH](#ssh)
- [Active Directory Interaction](#active-directory)
- [Google Sheets Interaction](#google-sheets)
- [Generation of public and private keys](#generate-public-and-private-keys)
- [File encryption and decryption](#file-encryption-and-decryption)
- [VMware Interaction](#vmware)
- [Can use a properties file](#using-a-properties-file)
- [Helper Utility Functions](#helper-functions)
- [Logging](#logging)

HUC is a self contained executable built on DotNet 5 and has builds available for Windows, Mac, and Linux

## Examples:
Get list of commands
```
huc
```

Get list of parameters for a command
```
huc Sql help
huc FtpPut help
huc Table help
huc <command> help
```
&nbsp;
### Email
Send an email
```
huc email -h="smtp.somerelay.org" -from="someone@aol.com" -to="grandma@aol.com" -s="Grandpa Birthday" -b="Tell Grandpa/nHAPPY BIRTHDAY!"
```

Send an email with CC and BCC and attachments
```
huc email -h="smtp.somerelay.org" -to="person1@aol.com;person2@aol.com" -cc="person3@aol.com" -bcc="person4@aol.com" -s="Some subject text" -b="Some text for body" myAttachedFile1.csv myAttachedFile2.txt
```

Send an email with text templating
```
huc email -h="smtp.somerelay.org" -to="person1@aol.com" -t1="Sandy" -t2="some other text" -s="Email for {t1}" -b="Hi {t1},\nHere is your {t2}"
```

&nbsp;
### SQL
Query Microsoft SQL server and output tab delimited data file
```
huc sql -c="Server=192.168.1.5;Database=NorthWind;User Id=testuser;Password=testpass;" -s="SELECT TOP 100 * FROM Orders" Orders100.txt
```

Query Microsoft SQL server and output multiple tab delimited data files from multiple result sets
```
huc sql -c="Server=192.168.1.5;Database=NorthWind;User Id=testuser;Password=testpass;" -s="SELECT * FROM Orders; SELECT * FROM Employees" Orders.txt Employees.txt
```

Query Microsoft SQL server with SQL script file and output tab delimited data file
```
printf "SELECT TOP 100 *\nFROM Orders" > mssqlscript.sql
huc sql -c="Server=192.168.1.5;Database=NorthWind;User Id=testuser;Password=testpass;" -f="mssqlscript.sql" OrdersFromScript.txt
```

Upload tab delimited file into a SQL server table
```
huc sqlload -c="Server=192.168.1.5;Database=NorthWind;User Id=testuser;Password=testpass;" -d=NorthWind -s=dbo -t=TempOrders Orders.txt
```

Upload tab delimited file into a SQL server table and include the file row number and a time stamp, dropping the table if it exists already
```
huc sqlload -c="Server=192.168.1.5;Database=NorthWind;User Id=testuser;Password=testpass;" -drop -rowNumberColumnName=RowNumber -currentUtcDateTimeColumnName=UploadTime -d=NorthWind -s=dbo -t=TempOrders Orders.txt
```
&nbsp;
### Delimited Files
Convert tab delimited file to csv delimited file using defaults
```
cp Orders.txt Orders.csv
huc table Orders.csv
```

Convert tab delimited file to csv delimited file using specific delimiters and excluding the header row
```
cp Orders.txt Orders.csv
huc table -hd=pipe -hq=single -he=true -dd=pipe -dq=single -de=false Orders.csv
```

Convert tab delimited file to HTML table using defaults
```
cp Orders.txt Orders.html
huc tablehtml Orders.html
```

Convert tab delimited file to HTML table embeddeding a custom CSS file and Javascript file
```
cp Orders.txt Orders.html
huc tablehtml -css=MyStyleSheet.css -js=MyJavascriptFile.js Orders.html
```

Convert tab delimited file to XML
```
cp Orders.txt Orders.xml
huc tablexml Orders.xml
```

Convert tab delimited file to JSON
```
cp Orders.txt Orders.json
huc tablejson Orders.json
```

Convert tab delimited file to fixed width file
```
huc tableFixedWidth Orders.txt 10 20 15 9 6 0 4 200
```
&nbsp;
### FTP FTPS SFTP
List files in default directory
```
huc ftplist -h=192.168.1.5 -u=testuser -p=testpass
huc ftplist -e=explicit -h=192.168.1.5 -u=testuser -p=testpass
huc ftplist -e=implicit -h=192.168.1.5 -u=testuser -p=testpass
huc ftplist -e=ssh -h=192.168.1.5 -u=testuser -p=testpass
```

Recursively list files in /home/user directory
```
huc ftplist -h=192.168.1.5 -u=testuser -p=testpass -r "/home/user"
huc ftplist -e=explicit -h=192.168.1.5 -u=testuser -p=testpass -r "/home/user"
huc ftplist -e=implicit -h=192.168.1.5 -u=testuser -p=testpass -r "/home/user"
huc ftplist -e=ssh -h=192.168.1.5 -u=testuser -p=testpass -r "/home/user"
```

Get a file from a FTP/FTPS/SFTP server
```
huc ftpget -h=192.168.1.5 -u=testuser -p=testpass remotefile.txt
huc ftpget -e=explicit -h=192.168.1.5 -u=testuser -p=testpass remotefile.txt
huc ftpget -e=implicit -h=192.168.1.5 -u=testuser -p=testpass remotefile.txt
huc ftpget -e=ssh -h=192.168.1.5 -u=testuser -p=testpass remotefile.txt
```

Put a file on a FTP/FTPS/SFTP server
```
huc ftpput -h=192.168.1.5 -u=testuser -p=testpass localfile.txt
huc ftpput -e=explicit -h=192.168.1.5 -u=testuser -p=testpass localfile.txt
huc ftpput -e=implicit -h=192.168.1.5 -u=testuser -p=testpass localfile.txt
huc ftpput -e=ssh -h=192.168.1.5 -u=testuser -p=testpass localfile.txt
```
&nbsp;
### Zip
Zipping a file
```
huc zip myOuputFile.zip someLocalFile.txt
```

Zipping multiple files
```
huc zip myOuputFile.zip *.txt *.csv
```
&nbsp;
### Windows Task Scheduler
List all tasks on scheduler
```
huc WindowsTaskSchedulerList -h="localhost" -u="administrator" -p="password" ALL
```

List a specific task MyTask on scheduler with details
```
huc WindowsTaskSchedulerList -h="localhost" -u="administrator" -p="password" -d /myTaskFolder/MyTask
```

Create a Windows Task Scheduler job to run every day at 4:15am
```
huc WindowsTaskSchedulerAdd -h="localhost" -u="administrator" -p="password" -taskUsername="system" -tw="c:\temp" -t1="DAILY 04:15" -tn="MyTask" "C:\temp\RunMe.bat"
```

Create a Windows Task Scheduler job to run every hour at 35 minutes after the hour
```
huc WindowsTaskSchedulerAdd -h="localhost" -u="administrator" -p="password" -taskUsername="system" -tw="c:\temp" -t1="HOURLY 35" -tn="MyTask" "C:\temp\RunMe.bat"
```

Create a Windows Task Scheduler job to run Monday and Wednesday at 7:12pm 
```
huc WindowsTaskSchedulerAdd -h="localhost" -u="administrator" -p="password" -taskUsername="system" -tw="c:\temp" -t1="MONDAY 19:12" -t2="WEDNESDAY 19:12" -tn="MyTask" "C:\temp\RunMe.bat"
```

Delete a Windows Task Scheduler job
```
huc WindowsTaskSchedulerRemove -h="localhost" -u="administrator" -p="password" MyTask
```
&nbsp;
### File Replacement
Replace all instances of Person with Steve in the file mydoc.txt
```
huc FileReplaceString "Person" "Steve" mydoc.txt
```
&nbsp;
### File Appending
Append files file1.txt and file2.txt to mainfile.txt
```
huc FileAppend mainfile.txt file1.txt file2.txt
```
&nbsp;
### File Split
Split a file on the new line character into 3 other files
```
huc FileSplit Orders.txt Orders1.txt Orders2.txt Orders3.txt
```
&nbsp;
### File Checksum
Generate MD5 checksum for file MyFile.zip
```
huc FileChecksum MyFile.zip
```

Generate SHA512 checksum for files *.txt
```
huc FileChecksum -t=SHA512 *.txt
```
&nbsp;
### Directory Flatten
Move all files in all subdirectories of target directory into the target directory, but don't overwrite if the file already exists
```
huc DirectoryFlatten C:\temp\MyDirectory
```

Move all files in all subdirectories of target directory into the target directory, and keep the newest file
```
huc DirectoryFlatten -c=KeepNewest C:\temp\MyDirectory
```
&nbsp;
### Directory Remove Empty
Deletes empty subdirectories recursively
```
huc DirectoryRemoveEmpty C:\temp\MyDirectory
```
&nbsp;
### Web Server
Start webserver and host files out of the current directory
```
huc WebServer .
```

Start webserver on port 80 and host files out of c:\www directory
```
huc WebServer -o=80 c:\www
```

Start webserver on port 80 and host files out of c:\www directory and require a username and password
```
huc WebServer -o=80 -u=user -p=testpass c:\www
```
&nbsp;
### SSH
Issue LS command
```
huc SSH -h=192.168.1.5 -u=testuser -p=testpass "ls"
```

Change directory and issue LS command with options
```
huc SSH -h=192.168.1.5 -u=testuser -p=testpass "cd someDirectory; ls -la;"
```
&nbsp;
### Active Directory
List all objects and their attributes to a tab delimited file
```
huc ActiveDirectoryList -h=192.168.1.5 -u=administrator -p=testpass adlist.txt
```

List various object types
```
huc ActiveDirectoryListObjects -h=192.168.1.5 -u=administrator -p=testpass ?teve*
huc ActiveDirectoryListUsers -h=192.168.1.5 -u=administrator -p=testpass
huc ActiveDirectoryListGroups -h=192.168.1.5 -u=administrator -p=testpass Group*
huc ActiveDirectoryListComputers -h=192.168.1.5 -u=administrator -p=testpass
```

List various object types and display specific LDAP fields
```
huc ActiveDirectoryListObjects -h=192.168.1.5 -u=administrator -p=testpass -pi=*Name
huc ActiveDirectoryListUsers -h=192.168.1.5 -u=administrator -p=testpass -pi=DistinguishedName,OganizationalUnit,ObjectName,ObjectGuid ?teve*
huc ActiveDirectoryListGroups -h=192.168.1.5 -u=administrator -p=testpass -pi=*Name,Object*
huc ActiveDirectoryListComputers -h=192.168.1.5 -u=administrator -p=testpass -pi=* MyComputer?
```

List additional details for an Active Directory object
```
huc ActiveDirectoryListObjectDetails -h=192.168.1.5 -u=administrator -p=testpass Administrator
huc ActiveDirectoryListObjectDetails -h=192.168.1.5 -u=administrator -p=testpass Users
huc ActiveDirectoryListObjectDetails -h=192.168.1.5 -u=administrator -p=testpass ?teve*
```

Change a user's password (note: requires LDAPS certificate to be installed on AD server or running HUC on the AD server itself)
```
huc ActiveDirectoryChangePassword -h=192.168.1.5 -u=administrator -p=testpass testuser newpassword
```

Add User
```
huc ActiveDirectoryAddUser -h=192.168.1.5 -u=administrator -p=testpass testuser
huc ActiveDirectoryAddUser -h=192.168.1.5 -u=administrator -p=testpass -firstname="steve" -lastname="foster" testuser
```

Add Group
```
huc ActiveDirectoryAddGroup -h=192.168.1.5 -u=administrator -p=testpass testgroup
huc ActiveDirectoryAddGroup -h=192.168.1.5 -u=administrator -p=testpass -gt=GlobalSecurityGroup testgroup
```

Delete User
```
huc ActiveDirectoryRemoveUser -h=192.168.1.5 -u=administrator -p=testpass testuser
```

Delete Group
```
huc ActiveDirectoryRemoveGroup -h=192.168.1.5 -u=administrator -p=testpass testgroup
```

Move User
```
huc ActiveDirectoryMoveUser -h=192.168.1.5 -u=administrator -p=testpass testuser MyNewOU
```

Move Group
```
huc ActiveDirectoryMoveGroup -h=192.168.1.5 -u=administrator -p=testpass testgroup MyNewOU
```

Add user to group
```
huc ActiveDirectoryAddUserToGroup -h=192.168.1.5 -u=administrator -p=testpass testuser MyGroup1 SomeOtherGroup
```

Remove user from group
```
huc ActiveDirectoryRemoveUserFromGroup -h=192.168.1.5 -u=administrator -p=testpass testuser MyGroup1
```

Enable user
```
huc ActiveDirectoryEnableUser -h=192.168.1.5 -u=administrator -p=testpass testuser
```

Disable user
```
huc ActiveDirectoryDisableUser -h=192.168.1.5 -u=administrator -p=testpass testuser
```

Disable users who have not logged on in the past 7 days
```
huc ActiveDirectoryDisableUsers -h=192.168.1.5 -u=administrator -p=testpass -l=7
```
&nbsp;
### Google Sheets
For setting up the Google account see...\
https://medium.com/@williamchislett/writing-to-google-sheets-api-using-net-and-a-services-account-91ee7e4a291 \
\
Clear all data from a Google Sheet tab named Sheet1 (sheet ID is in the URL)
```
huc GoogleSheetsClear -k="MyGoogleAppKey.json" -a="MyApplicationName" -id="dkjfsd328sdfuhscbjcds8hfjndsfdsfdsfe" -s="Sheet1"
```

Clear all data from the first Google Sheet tab
```
huc GoogleSheetsClear -k="MyGoogleAppKey.json" -a="MyApplicationName" -id="dkjfsd328sdfuhscbjcds8hfjndsfdsfdsfe"
```

Clear the first sheet tab and upload Orders.txt tab delimited file to it
```
huc GoogleSheetsLoad -k="MyGoogleAppKey.json" -a="MyApplicationName" -id="dkjfsd328sdfuhscbjcds8hfjndsfdsfdsfe" Orders.txt
```

Add a row to first sheet with the values "AA", blank, "CC"
```
huc GoogleSheetsAddRow -k="MyGoogleAppKey.json" -a="MyApplicationName" -id="dkjfsd328sdfuhscbjcds8hfjndsfdsfdsfe" AA null CC
```

Make the first row of data have red text, blue background, and bold
```
huc GoogleSheetsFormatCells -k="MyGoogleAppKey.json" -a="MyApplicationName" -id="dkjfsd328sdfuhscbjcds8hfjndsfdsfdsfe" -width=100 -b -fc=Red -bc=Blue 
```

Query all data from first sheet and output it to a tab delimited file MyFile.txt
```
huc GoogleSheetsQuery -k="MyGoogleAppKey.json" -a="MyApplicationName" -id="dkjfsd328sdfuhscbjcds8hfjndsfdsfdsfe" MyFile.txt
```
&nbsp;
### Generate public and private keys
Generate RSA public and private key files
```
huc GenerateKeyPair MyPublicKey.txt MyPrivateKey.txt
```

Generate RSA public and private key files with RSA length 4096
```
huc GenerateKeyPair -l=4096 MyPublicKey.txt MyPrivateKey.txt
```
&nbsp;
### File encryption and decryption
Encrypt file with password
```
huc FileEncrypt -p=password data.txt data.encrypted
```

Decrypt file with password
```
huc FileDecrypt -p=password data.encrypted dataDecrypted.txt
```

Encrypt file with public key
```
huc FileEncrypt -pk=MyPublicKey.txt data.txt data.encrypted
```

Decrypt file with private key
```
huc FileDecrypt -pk=MyPrivateKey.txt data.encrypted dataDecrypted.txt
```
&nbsp;
### VMware
Query various information in a VCenter 6.7+ infrastructure
```
huc VMwareList -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass DataCenter VM StoragePolicy
huc VMwareList -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass VM_Quick
huc VMwareList -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass VM_WithoutTools
huc VMwareList -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass VM_PoweredOff
huc VMwareList -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass VM_IsoAttached
```

Query raw JSON data from VCenter 6.7+ infrastructure
```
huc VMwareQuery -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass  /rest/vcenter/host
huc VMwareQuery -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass  /rest/vcenter/vm
huc VMwareQuery -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass  /rest/vcenter/vm/vm-1692
```

Query all infrastructure data to a JSON file
```
huc VMwareQueryJSON -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass MyDataFile.json
```

Perform various actions on a VM
```
huc VMwareVM -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass MyVM None
huc VMwareVM -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass MyVM Shutdown
huc VMwareVM -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass MyVM Reboot
huc VMwareVM -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass MyVM Standby
huc VMwareVM -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass MyVM Reset
huc VMwareVM -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass MyVM Start
huc VMwareVM -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass MyVM Stop
huc VMwareVM -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass MyVM Suspend
huc VMwareVM -h=192.168.1.5 -u=testuser@vsphere.local -p=mypass MyVM DetachISOs
```
&nbsp;
&nbsp;
## Putting it all together
Query SQL server, convert the data, sftp it, zip it, then email the data
```
huc sql -c="Server=192.168.1.5;Database=NorthWind;User Id=testuser;Password=testpass;" -s="SELECT * FROM Orders" orders.csv
huc table -hd=comma -hq=none -dd=comma -dq=none orders.csv
huc ftpput -e=ssh -h=192.168.1.5 -u=testuser -p=testpass orders.csv
huc zip orders.zip "*.csv"
huc email -h="smtp.somerelay.org" -from="me@aol.com" -to="person@aol.com" -s="Orders data" -b="Attached is the order data" "*.zip"
```
&nbsp;
## Using a properties file
When huc first runs, it attempts to generate a huc.properties file in the directory of the executable. This file contains all of the parameters for each command. You can populate this file with certain properties so you don't have to type them in every time. The huc program will first check if a parameter was supplied at the command line. If not, if will then check the properties file (commandline overrides properties file). If still not found it will attempt to use a default value for some parameters (not all, some are required to be provided).

So assuming a properties file of...
```properties
sql.connectionString=Server=192.168.1.5;Database=NorthWind;User Id=testuser;Password=testpass;
table.headerDelimiter=comma
table.headerQuoting=none
table.dataDelimiter=comma
table.dataQuoting=none
ftpput.host=192.168.1.5
ftpput.encryptionMode=SSH
ftpput.username=testuser
ftpput.password=testpass
email.host=smtp.somerelay.org
email.from=me@aol.com
```
The commands now become...
```
huc sql -s="SELECT * FROM Orders" orders.csv
huc table orders.csv
huc ftpput orders.csv
huc zip orders.zip "*.csv"
huc email -to="person@aol.com" -s="Orders data" -b="Attached is the order data" "*.zip"
```
&nbsp;
## Helper functions
Generate file with random data
```
huc GenerateRandomFile testdata.txt
huc GenerateRandomFile -l=1000000 testdata1.txt testdata2.txt testdata3.txt
```

Show current properties set in the properties file
```
huc ShowProperties
```

Show all available properties
```
huc ShowProperties -a
```

Convert Binary file to Base16
```
huc ConvertBinaryToBase16 myinputfile.txt myoutputfile.txt
```

Convert Binary file to Base64
```
huc ConvertBinaryToBase64 myinputfile.txt myoutputfile.txt
```

Get a web file
```
huc wget https://github.com/Steven-D-Foster/huc/releases/download/v1.3.0/huc-linux.zip
```

Get a web page
```
huc wget https://github.com github.txt
```

Show internet time
```
huc time
```

Show drift of local clock compared to internet time
```
huc time -d
```

Show all of the colors available for commands that take a color parameter
```
huc colors
```

Show details for a specific color
```
huc colors red
```

Test JSAS service
```
huc jsas https://192.168.0.10 MyPassword MyData
huc jsas https://192.168.0.10 MyPassword MyData NewFile.txt
```

Encrypt Password to use in huc.properties file
```
huc EncodePassword mySecretPassword
```
&nbsp;
## Logging
HUC supports various logging. At the console level HUC supports ```INFO```, ```DEBUG```, and ```TRACE``` logging levels. By default the logging level is ```INFO```. To enable ```DEBUG``` level logging at the console, specify the ```-debug``` parameter at the command line. To enable ```TRACE``` level logging, specify the ```-trace``` parameter at the command line.

HUC also supports logging to a file. To enable file logging, use the parameters ```Log.FileLevel``` and ```Log.FileName``` in the ```huc.properties``` file to specify the log level (```CRITICAL```, ```ERROR```, ```WARN```, ```INFO```, ```DEBUG```, ```TRACE```) and the filename of the file to write out to.


