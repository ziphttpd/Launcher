set TARGET=%1
set BASE=%~dp0
set MSBUILD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe
%MSBUILD% %BASE%ziphttpdTool.sln -p:Configuration=Release
copy %BASE%launcher\bin\Release\launcher.exe %TARGET%
