$packageName = 'westwindwebsurge'
$fileType = 'exe'
$url = 'https://github.com/RickStrahl/WestwindWebSurgeReleases/raw/master/WebSurgeSetup-1.23.exe'

$silentArgs = '/SILENT'
$validExitCodes = @(0)

Install-ChocolateyPackage "packageName" "$fileType" "$silentArgs" "$url"  -validExitCodes  $validExitCodes  -checksum "6C3175267A2B7CFA14446CE8767488CB0FC4806D3EA61FA0E3EC42A25F5D1638" -checksumType "sha256"
