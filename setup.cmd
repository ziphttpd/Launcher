rem @echo off

set ZH_HOME=%1
set SCRIPTDIR=%~dp0

if "%ZH_HOME%" == "" (
	echo setup.cmd targetfolder\
	exit /B 1
)

cd %SCRIPTDIR%
git pull

set FILE=launcher.exe
set SOURCE=%SCRIPTDIR%launcher\bin\Release\%FILE%
set TARGET=%ZH_HOME%%FILE%

set MSBUILD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe
%MSBUILD% %SCRIPTDIR%ziphttpdTool.sln -p:Configuration=Release

if exist %TARGET%.old del /Y %TARGET%.old
if exist %TARGET% ren %TARGET% %FILE%.old
copy %SOURCE% %TARGET%

exit /B 0
