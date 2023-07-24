using System;
using System.Diagnostics;
// using System.Runtime.InteropServices.JavaScript;

namespace RJDev.MemoryMappedCache;

public class MemoryMappedCache
{
    // [JSImport("nodeVersion", "main.mjs")]
    // internal static partial string GetNodeVersion();

    // [JSExport]
    internal static void Start(string workingDirectory, int tcpPort)
    {
        Console.WriteLine("Starting memory mapped cache...");
        Console.WriteLine($"\tport:\t\t{tcpPort} TCP");
        Console.WriteLine($"\tworking directory:\t{workingDirectory}");
        // Console.WriteLine($"\tnode version: {GetNodeVersion()}");

        var cache = new Cache(workingDirectory);
        Stopwatch stopwatch = new();
        stopwatch.Start();
        cache.LoadFiles();
        stopwatch.Stop();

        Console.WriteLine($"Read time: {stopwatch.ElapsedMilliseconds:N} ms");
    }
}