cd FilesToInstall
call PullFiles.bat
cd ..
cd
call "C:\Program Files\InstallMate 9\BinX64\tin.exe" /build:all websurge.im9
cd
call 7z a -tzip ".\WebSurge\ProductionInstall\WebSurgeSetup.zip" ".\WebSurge\ProductionInstall\WebSurgeSetup.exe"

copy ".\WebSurge\ProductionInstall\WebSurgeSetup.exe" "C:\installs\Distribution CD\Demos"
copy ".\WebSurge\ProductionInstall\WebSurgeSetup.zip" "C:\installs\Distribution CD\Demos"

pause