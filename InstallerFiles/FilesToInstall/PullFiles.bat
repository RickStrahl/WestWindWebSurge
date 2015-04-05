copy ..\..\WebSurge\bin\release\WebSurge.exe
copy ..\..\WebSurge\bin\release\WebSurge.pdb
copy ..\..\WebSurge\bin\release\WebSurge.exe.config
copy ..\..\WebSurge\bin\release\WebSurge.Core.dll
copy ..\..\WebSurge\bin\release\WebSurge.Core.pdb
copy ..\..\WebSurge\bin\release\FiddlerCore4.dll
copy ..\..\WebSurge\bin\release\Newtonsoft.json.dll
copy ..\..\WebSurge\bin\release\Westwind.Utilities.dll
copy ..\..\WebSurge\bin\release\Westwind.RazorHosting.dll
copy ..\..\WebSurge\bin\release\System.Web.Razor.dll
copy ..\..\WebSurge\bin\release\Zed*.dll

copy ..\..\WebSurge\bin\release\certmaker.dll
copy ..\..\WebSurge\bin\release\bcmakecert.dll

copy ..\..\WebSurgeCli\bin\release\WebSurgeCli.exe
copy ..\..\WebSurgeCli\bin\release\WebSurgeCli.pdb
copy ..\..\WebSurgeCli\bin\release\WebSurgeCli.exe.config
copy ..\..\WebSurgeCli\bin\release\CommandLine.dll


xcopy "%AppData%\West Wind WebSurge\html\*.*" AppData\html /S /E /Y
REM copy "%AppData%\West Wind WebSurge\html\css\*.*" AppData\html\css
REM copy "%AppData%\West Wind WebSurge\html\css\fonts\*.*" AppData\html\css\fonts
REM copy "%AppData%\West Wind WebSurge\html\scripts\*.*" AppData\html\scripts
REM copy "%AppData%\West Wind WebSurge\html\*.*" AppData\html
del  AppData\html\_preview.*
del  AppData\html\_results.html