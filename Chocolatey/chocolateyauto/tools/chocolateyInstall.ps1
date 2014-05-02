#NOTE: Please remove any commented lines to tidy up prior to releasing the package, including this one

$packageName = 'WestwindWebSurge' # arbitrary name for the package, used in messages
$fileType = 'exe' #only one of these: exe, msi, msu
$url = 'http://west-wind.com/files/WebSurgeSetup.exe' # download url
#$url64 = 'URL_x64_HERE' # 64bit URL here or remove - if installer decides, then use $url
$silentArgs = '/q' # "/s /S /q /Q /quiet /silent /SILENT /VERYSILENT" # try any of these to get the silent installer #msi is always /quiet
$validExitCodes = @(0) #please insert other valid exit codes here, exit codes for ms http://msdn.microsoft.com/en-us/library/aa368542(VS.85).aspx

# main helpers - these have error handling tucked into them already
# installer, will assert administrative rights
# if removing $url64, please remove from here
Install-ChocolateyPackage "$packageName" "$fileType" "$silentArgs" "$url"  -validExitCodes $validExitCodes
