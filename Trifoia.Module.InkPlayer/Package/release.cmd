@echo off
set TargetFramework=%1
set ProjectName=%2

del "*.nupkg"
REN  %ProjectName%.nuspec.REMOVE %ProjectName%.nuspec 
"..\..\oqtane.framework\oqtane.package\nuget.exe" pack %ProjectName%.nuspec -Properties targetframework=%TargetFramework%;projectname=%ProjectName%
XCOPY "*.nupkg" "..\..\oqtane.framework\Oqtane.Server\Packages\" /Y