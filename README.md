# Memory-Mapped Files
> Native `.node` addon for node.js written in C#.

Memory-mapped files offer a significant performance benefit over standard file access. 
It reduces the number of times the application must access the disk (which is slow compared to memory).
In case you use thread workers, you can share the memory-mapped files between them, so you will also reduce the number of redundant file reads.

Native file reading is much faster than reading from a node.js. 
Memory-mapped files are intended mainly for workers so it creates TCP server that holds the files; Workers transfer the files over the network so it slows it down a little, but it is still much faster than reading from a node.js.








## Publish
src/RJDev.MemoryMappedFiles.Module/publish.bat

Runtime Identifier
https://learn.microsoft.com/en-us/dotnet/core/rid-catalog