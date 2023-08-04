using System;
using RJDev.MemoryMappedFiles.Module;

try
{
    Module.StartCacheServer("F:/Work/packages/memory-mapped-cache/src/node/test/test-files", new[] { "**/*" }, 9876);

    Console.WriteLine("Running...");

    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}