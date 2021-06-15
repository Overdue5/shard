@SET CURPATH=%~dp0

@SET EXENAME=Server



::##########

@ECHO:
@ECHO: Compile %EXENAME% for Windows
@ECHO:

dotnet build -c Debug

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS

::##########

@ECHO OFF

"%CURPATH%\Output\%EXENAME%.exe"

