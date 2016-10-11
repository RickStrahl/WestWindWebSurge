$packageName = 'WestwindWebSurge'
$fileType = 'exe'
$url = 'http://west-wind.com/files/WebSurgeSetup.exe' 
$silentArgs = '/q'
$validExitCodes = @(0)

Install-ChocolateyPackage "$packageName" "$fileType" "$silentArgs" "$url"  -validExitCodes $validExitCodes
