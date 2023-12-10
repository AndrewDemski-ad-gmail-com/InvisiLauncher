PUSHD "%~dp0"
SET COMM=..\bin\Debug\net8.0-windows7.0\InvisiLauncher.exe %~dp0test_01.ps1
ECHO %COMM%
%COMM%
SET RC=%ERRORLEVEL%
ECHO returncode=%RC%
POPD
PAUSE