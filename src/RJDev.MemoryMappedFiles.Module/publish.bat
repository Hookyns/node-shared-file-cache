dotnet publish -r win-x64 -c Release -o publish/win-x64
dotnet publish -r linux-x64 -c Release -o publish/linux-x64
dotnet publish -r linux-arm64 -c Release -o publish/linux-arm64
dotnet publish -r osx-x64 -c Release -o publish/osx-x64


xcopy /Y .\publish\win-x64\RJDev.MemoryMappedFiles.Module.node ..\node\addons\memory-mapped-files.win-x64.node*
xcopy /Y .\publish\linux-x64\RJDev.MemoryMappedFiles.Module.node ..\node\addons\memory-mapped-files.linux-x64.node*
xcopy /Y .\publish\linux-arm64\RJDev.MemoryMappedFiles.Module.node ..\node\addons\memory-mapped-files.linux-arm64.node*
xcopy /Y .\publish\osx-x64\RJDev.MemoryMappedFiles.Module.node ..\node\addons\memory-mapped-files.osx-x64.node*