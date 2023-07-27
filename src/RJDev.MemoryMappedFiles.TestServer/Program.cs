using System;
using RJDev.MemoryMappedFiles.Module;
using RJDev.MemoryMappedFiles.Module.Sockets;

try
{
    // var cache = new MemoryMappedCache("F:/Work/packages/memory-mapped-cache/src/node/test/test-files");
    // var socketServer = new SocketServer(cache, 9876);
    // socketServer.Start();
    Module.StartCacheServer("F:/Work/packages/memory-mapped-cache/src/node/test/test-files", new[] { "**/*" }, 9876);

    Console.WriteLine("Running...");

    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}