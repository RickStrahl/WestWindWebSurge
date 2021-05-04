$packageName = 'westwindwebsurge'
$fileType = 'exe'
$url = 'https://github.com/RickStrahl/WestwindWebSurgeReleases/raw/master/WebSurgeSetup-1.22.exe'

$silentArgs = '/SILENT'
$validExitCodes = @(0)

Install-ChocolateyPackage "packageName" "$fileType" "$silentArgs" "$url"  -validExitCodes  $validExitCodes  -checksum "C9F3601196F3BE788ED3937F1B69C3A6B71414E0B2B10A47561B0AB7099D5D74" -checksumType "sha256"
