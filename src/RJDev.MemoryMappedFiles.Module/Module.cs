using System;
using System.Diagnostics;
using Microsoft.JavaScript.NodeApi;
using RJDev.MemoryMappedFiles.Module.Sockets;

namespace RJDev.MemoryMappedFiles.Module;

/// <summary>
/// Memory mapped cache
/// </summary>
[JSExport]
public static class Module
{
    private static SocketServer? server;

    /// <summary>
    /// Start the cache server.
    /// </summary>
    /// <param name="workingDirectory"></param>
    /// <param name="include"></param>
    /// <param name="tcpPort"></param>
    public static void StartCacheServer(string workingDirectory, string[] include, int tcpPort)
    {
#if DEBUG
        Console.WriteLine("Starting memory mapped cache...");
        Console.WriteLine($"\tport:\t\t{tcpPort} TCP");
        Console.WriteLine($"\tworking directory:\t{workingDirectory}");
#endif

        // Create cache
        var cache = new MemoryMappedCache(workingDirectory);

#if DEBUG
        var stopwatch = new Stopwatch();
        stopwatch.Start();
#endif

        // Load files
        int numberOfFiles = cache.LoadFiles(include);

#if DEBUG
        stopwatch.Stop();
        Console.WriteLine($"Loaded {numberOfFiles} files in {stopwatch.ElapsedMilliseconds:N} ms");
#endif

        // Create Socket server
        server = new SocketServer(cache, tcpPort);
        server.Start();
    }

    /// <summary>
    /// Stop cache server.
    /// </summary>
    public static void StopCacheServer()
    {
        server?.Stop();
        server?.Dispose();
    }

    /// <summary>
    /// Create client for the cache server.
    /// </summary>
    /// <param name="tcpPort"></param>
    /// <returns></returns>
    public static Client CreateClient(int tcpPort)
    {
        return new Client(tcpPort);
    }
}