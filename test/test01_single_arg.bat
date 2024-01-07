@ECHO OFF
PUSHD "%~dp0"
REM SET CONF=netcoreapp3.1
SET CONF=net8.0-windows
SET COMM=..\bin\Debug\%CONF%\InvisiLauncher.exe %~dp0test_01.ps1
ECHO %COMM%
%COMM%
SET RC=%ERRORLEVEL%
ECHO returncode=%RC%
POPD
@ECHO ON
PAUSE