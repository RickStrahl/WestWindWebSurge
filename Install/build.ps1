
$src = "C:\projects2010\WebSurge\WebSurge\bin\Release"
$tgt = ".\Distribution"
$appdata = ".\DistributionAppData"
$release = ".\Builds\CurrentRelease"


remove-item -recurse -force ${tgt}
md $tgt


remove-item -recurse -force ${appData}
md $appdata
md $appdata\html

copy $src\*.exe $tgt
copy "..\WebSurgeCli\bin\Release\WebSurgeCli.exe" $tgt
copy "..\WebSurgeCli\bin\Release\Commandline.dll" $tgt
copy $src\*.dll $tgt
copy $src\*.config $tgt

ri "${tgt}\*.vshost.*"
ri "${tgt}\*.vshost.*.config"

xcopy "$env:appData\West Wind WebSurge\html\*.*" $appData\html /S /E /Y
remove-item  $AppData\html\_preview.*

If (Test-Path $AppData\html\_results.html) {
    remove-item  $AppData\html\_results.html
}

.\signtool.exe sign /v /n "West Wind Technologies" /sm /s MY /tr "http://timestamp.digicert.com" /td SHA256 /fd SHA256 "$tgt\WebSurge.exe"
.\signtool.exe sign /v /n "West Wind Technologies"  /sm /s MY   /tr "http://timestamp.digicert.com" /td SHA256 /fd SHA256  "$tgt\WebSurgeCli.exe"

# "Running Inno Setup..."
& "C:\Program Files (x86)\Inno Setup 5\iscc.exe" "WebSurge.iss" 

.\signtool.exe sign /v /n "West Wind Technologies"  /sm /s MY /tr "http://timestamp.digicert.com" /td SHA256 /fd SHA256 "$release\WebSurgeSetup.exe"


7z a -tzip "$release\WebSurgeSetup.zip" "$release\WebSurgeSetup.exe"
