@echo off

set TARGET=%1
set BASE=%~dp0

if "%TARGET%" == "" (
	echo setup.cmd targetfolder\
	exit /B 1
)

cd %BASE%
git pull

set EXEID=launcher
set BUILDEXE=%BASE%launcher\bin\Release\%EXEID%.exe
set TARGETEXE=%TARGET%%EXEID%.exe

set MSBUILD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe
%MSBUILD% %BASE%ziphttpdTool.sln -p:Configuration=Release

if exist %TARGETEXE%.old del /F %TARGETEXE%.old
if exist %TARGETEXE% ren %TARGETEXE% %EXEID%.old
copy %BUILDEXE% %TARGETEXE%

exit /B 0
