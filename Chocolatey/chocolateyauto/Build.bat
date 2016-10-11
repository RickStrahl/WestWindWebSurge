cd %~dp0
call choco pack
call choco install "WestwindWebSurge" -fdv  -s "C:\projects2010\WebSurge\Chocolatey\chocolateyauto"
 
pause