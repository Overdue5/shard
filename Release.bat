@SET CURPATH=%~dp0

@SET EXENAME=Server

@TITLE: %EXENAME% - https://www.servuo.com

::##########

@ECHO:
@ECHO: Compile %EXENAME% for Windows
@ECHO:

dotnet build -c Release

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS

::##########

@ECHO OFF

"%CURPATH%\Output\%EXENAME%.exe"

