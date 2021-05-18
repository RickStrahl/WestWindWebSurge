$packageName = 'westwindwebsurge'
$fileType = 'exe'
$url = 'https://github.com/RickStrahl/WestwindWebSurgeReleases/raw/master/WebSurgeSetup-1.23.2.exe'

$silentArgs = '/SILENT'
$validExitCodes = @(0)

Install-ChocolateyPackage "packageName" "$fileType" "$silentArgs" "$url"  -validExitCodes  $validExitCodes  -checksum "EE6A7A9A8FEDFAD76A66C074AF6DC3ABE116BBF16AB2C6F24AFD617B4B897488" -checksumType "sha256"
