@SET CURPATH=%~dp0


:loop

dotnet build -c Debug && "%CURPATH%\Output\Server.exe"
if %ERRORLEVEL% EQU 100	goto loop

