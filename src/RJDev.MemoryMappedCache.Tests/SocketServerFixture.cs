using System;
using RJDev.MemoryMappedCache.Module;

namespace RJDev.MemoryMappedCache.Tests;

public class SocketServerFixture : IDisposable
{
    public SocketServer SocketServer { get; }

    public SocketServerFixture()
    {
        var cache = new Module.MemoryMappedCache("F:/Work/packages/rttist/dev/hookyns/transformer/src");
        SocketServer = new SocketServer(cache, 9876);
        SocketServer.Start();
    }

    public void Dispose()
    {
        SocketServer.Dispose();
    }
}