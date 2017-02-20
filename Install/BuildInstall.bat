cd FilesToInstall
call PullFiles.bat

"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin\signtool.exe" sign /v /n "West Wind Technologies" /sm /s MY /tr "http://timestamp.digicert.com" /td SHA256 /fd SHA256 ".\WebSurge.exe"
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin\signtool.exe" sign /v /n "West Wind Technologies"  /sm /s MY   /tr "http://timestamp.digicert.com" /td SHA256 /fd SHA256  ".\WebSurgeCli.exe"

cd ..
cd
"C:\Program Files\InstallMate 9\BinX64\tin.exe" /build:all websurge.im9

call "C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin\signtool.exe"  sign /v /n "West Wind Technologies"  /sm /s MY /tr "http://timestamp.digicert.com" /td SHA256 /fd SHA256 ".\WebSurge\ProductionInstall\WebSurgeSetup.exe"

cd
7z a -tzip ".\WebSurge\ProductionInstall\WebSurgeSetup.zip" ".\WebSurge\ProductionInstall\WebSurgeSetup.exe"

copy ".\WebSurge\ProductionInstall\WebSurgeSetup.exe" "C:\installs\Distribution CD\Demos"
copy ".\WebSurge\ProductionInstall\WebSurgeSetup.zip" "C:\installs\Distribution CD\Demos"

pause	