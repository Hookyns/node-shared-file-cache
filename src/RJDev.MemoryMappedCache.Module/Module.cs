using System;
using System.Diagnostics;
using Microsoft.JavaScript.NodeApi;

namespace RJDev.MemoryMappedCache.Module;

/// <summary>
/// Memory mapped cache
/// </summary>
[JSExport]
public static class Module
{
    private static SocketServer? server;

    /// <summary>
    /// Start the memory mapped cache server.
    /// </summary>
    /// <param name="workingDirectory"></param>
    /// <param name="tcpPort"></param>
    public static void StartCacheServer(string workingDirectory, int tcpPort)
    {
        Console.WriteLine("Starting memory mapped cache...");
        Console.WriteLine($"\tport:\t\t{tcpPort} TCP");
        Console.WriteLine($"\tworking directory:\t{workingDirectory}");
        // Console.WriteLine($"\tnode version: {GetNodeVersion()}");

        // Create cache
        var cache = new MemoryMappedCache(workingDirectory);

        Stopwatch stopwatch = new();
        stopwatch.Start();

        // Load files
        int numberOfFiles = cache.LoadFiles();

        stopwatch.Stop();

        Console.WriteLine($"Loaded {numberOfFiles} files in {stopwatch.ElapsedMilliseconds:N} ms");

        // Create Socket server
        server = new SocketServer(cache, tcpPort);
        server.Start();
    }

    public static void StopCacheServer()
    {
        server?.Stop();
    }

    public static Client CreateClient(int tcpPort)
    {
        var client = new Client(tcpPort);
        // client.GetFile("index.ts").Then((content) =>
        // {
        //     Console.WriteLine(content);
        // }, err =>
        // {
        //     Console.WriteLine(err);
        // });
        return client;
    }
}