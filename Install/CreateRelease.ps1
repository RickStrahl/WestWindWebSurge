cd "$PSScriptRoot" 

$releaseFile = "$PSScriptRoot\builds\currentrelease\WebSurgeSetup.exe"


$version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($releaseFile).FileVersion
"Raw version: " + $version
$version = $version.Trim().Replace(".0","") 
"Writing Version File for: " + $version

$finalFile = "..\..\WebSurgeReleases\WebSurgeSetup-${version}.exe"
copy $releaseFile $finalFile
cd "..\..\WebSurgeReleases"

git add -f "WebSurgeSetup-${version}.exe"
git commit -m "$version"
git push origin master

cd "$PSScriptRoot" 

pause