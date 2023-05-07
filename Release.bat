@SET CURPATH=%~dp0

@SET EXENAME=Server

dotnet build -c Release

if %ERRORLEVEL% EQU 0 "%CURPATH%\Output\%EXENAME%.exe"

