rem @echo off

set ZH_HOME=%1
set SCRIPTDIR=%~dp0

if "%ZH_HOME%" == "" (
	echo setup.cmd targetfolder\
	exit /B 1
)

cd %SCRIPTDIR%
git pull

set EXEID=launcher
set SOURCE=%SCRIPTDIR%launcher\bin\Release\%EXEID%.exe
set TARGET=%ZH_HOME%%EXEID%.exe

set MSBUILD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe
%MSBUILD% %SCRIPTDIR%ziphttpdTool.sln -p:Configuration=Release

if exist %TARGET%.old del /Y %TARGET%.old
if exist %TARGET% ren %TARGET% %TARGET%.old
copy %SOURCE% %TARGET%

exit /B 0
