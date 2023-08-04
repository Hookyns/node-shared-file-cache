dotnet publish -r win-x64 -c Release -o publish/win-x64
xcopy /Y .\publish\win-x64\RJDev.MemoryMappedFiles.Module.node ..\node\addons\mmf.win-x64.node*

docker build -f Dockerfile-linux -t mmf-linux --platform linux/amd64 .
@REM docker run --rm -v ${PWD}/publish/linux-x64:/app mmf-linux

@REM dotnet publish -r linux-x64 -c Release -o publish/linux-x64
@REM dotnet publish -r linux-arm64 -c Release -o publish/linux-arm64
@REM dotnet publish -r osx-x64 -c Release -o publish/osx-x64
@REM xcopy /Y .\publish\linux-x64\RJDev.MemoryMappedFiles.Module.node ..\node\addons\mmf.linux-x64.node*
@REM xcopy /Y .\publish\linux-arm64\RJDev.MemoryMappedFiles.Module.node ..\node\addons\mmf.linux-arm64.node*
@REM xcopy /Y .\publish\osx-x64\RJDev.MemoryMappedFiles.Module.node ..\node\addons\mmf.osx-x64.node*