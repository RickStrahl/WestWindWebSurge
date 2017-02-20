$packageName = 'WestwindWebSurge'
$fileType = 'exe'
$url = 'https://github.com/RickStrahl/WestWindWebSurge/raw/master/InstallerFiles/Releases/WebSurgeSetup-1.01.exe' 
$silentArgs = '/q'
$validExitCodes = @(0)

Install-ChocolateyPackage "$packageName" "$fileType" "$silentArgs" "$url"  -validExitCodes $validExitCodes -checksum "9BB6D4A051C8E82490E20650AFFAE38389F14F4E06C566F8053C96D8839DBB4E" -checksumType "sha256"
