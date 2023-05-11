@SET CURPATH=%~dp0


:loop

dotnet build -c Release && "%CURPATH%\Output\Server.exe"
if %ERRORLEVEL% EQU 100	goto loop

