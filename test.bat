SETLOCAL
SET host=localhost
SET username=testadmin
SET password=mySecretAdmin1

huc ActiveDirectoryAddUser -h=%host% -u=%username% -p=%password% -fn=Test1 -ln=Doe -dn="John1 Doe" -ea="jd1@aol.com" jd1
huc ActiveDirectoryChangePassword -h=%host% -u=%username% -p=%password% jd1 myNew29Password
huc ActiveDirectoryAddGroup -h=%host% -u=%username% -p=%password% TestGroup1
huc ActiveDirectoryAddOU -h=%host% -u=%username% -p=%password% OU1
huc ActiveDirectoryAddOU -h=%host% -u=%username% -p=%password% -pou=OU1 OU2
huc ActiveDirectoryMoveGroup -h=%host% -u=%username% -p=%password% TestGroup1 OU1
huc ActiveDirectoryMoveUser -h=%host% -u=%username% -p=%password% jd1 OU2
huc ActiveDirectoryAddUserToGroup -h=%host% -u=%username% -p=%password% jd1 TestGroup1

huc ActiveDirectoryListUsers -h=%host% -u=%username% -p=%password% j?1
huc ActiveDirectoryListGroups -h=%host% -u=%username% -p=%password% Test*1

huc ActiveDirectoryRemoveUserFromGroup -h=%host% -u=%username% -p=%password% jd1 TestGroup1
huc ActiveDirectoryRemoveUser -h=%host% -u=%username% -p=%password% jd1
huc ActiveDirectoryRemoveGroup -h=%host% -u=%username% -p=%password% TestGroup1
huc ActiveDirectoryRemoveOU -h=%host% -u=%username% -p=%password% OU2
huc ActiveDirectoryRemoveOU -h=%host% -u=%username% -p=%password% OU1








