#!/bin/bash

TargetFramework=$1
ProjectName=$2

mv  %ProjectName%.nuspec.REMOVE %ProjectName%.nuspec 

"..\..\oqtane.framework\oqtane.package\nuget.exe" pack %ProjectName%.nuspec -Properties targetframework=%TargetFramework%;projectname=%ProjectName%
cp -f "*.nupkg" "..\..\oqtane.framework\Oqtane.Server\Packages\"