
$src = "..\WebSurge\bin\Release"
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

.\signtool.exe sign /v /n "West Wind Technologies" /tr "http://timestamp.digicert.com" /td SHA256 /fd SHA256 "$tgt\WebSurge.exe"
.\signtool.exe sign /v /n "West Wind Technologies" /tr "http://timestamp.digicert.com" /td SHA256 /fd SHA256  "$tgt\WebSurgeCli.exe"

# "Running Inno Setup..."
& "C:\Program Files (x86)\Inno Setup 6\iscc.exe" "WebSurge.iss" 

.\signtool.exe sign /v /n "West Wind Technologies" /tr "http://timestamp.digicert.com" /td SHA256 /fd SHA256 "$release\WebSurgeSetup.exe"


.\7z a -tzip "$release\WebSurgeSetup.zip" "$release\WebSurgeSetup.exe"


$version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$PSScriptRoot\builds\currentrelease\WebSurgeSetup.exe").FileVersion
$version = $version.Trim()
"Initial Version: " + $version

# Remove last two .0 version tuples if it's 0
if($version.EndsWith(".0.0")) {
    $version = $version.SubString(0,$version.Length - 4);
}
else {
    if($version.EndsWith(".0")) {    
        $version = $version.SubString(0,$version.Length - 2);
    }
}
"Truncated Version: " + $version

"Writing Version File for: " + $version
$versionFilePath = ".\builds\currentrelease\WebSurge_Version_Template.xml"
$versionFile = Get-Content -Path $versionFilePath  

$versionFile = $versionFile.Replace("{{version}}",$version).Replace("{{date}}",[System.DateTime]::Now.ToString("MMMM d, yyyy"))
$versionFile
""

out-file -filepath $versionFilePath.Replace("_Template","")  -inputobject $versionFile

get-childitem .\builds\CurrentRelease\* -include *.* | foreach-object { "{0}`t{1}`t{2:n0}`t`t{3}" -f $_.Name, $_.LastWriteTime, $_.Length, [System.Diagnostics.FileVersionInfo]::GetVersionInfo($_).FileVersion }
""
"Done..."
