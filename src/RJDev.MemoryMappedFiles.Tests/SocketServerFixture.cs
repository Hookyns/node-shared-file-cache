using System;
using Microsoft.JavaScript.NodeApi;
using RJDev.MemoryMappedFiles.Module.Sockets;

namespace RJDev.MemoryMappedFiles.Tests;

public class SocketServerFixture : IDisposable
{
    public SocketServer SocketServer { get; }
    private JSValueScope _scope;

    public SocketServerFixture()
    {
        var cache = new Module.MemoryMappedCache("F:/Work/packages/rttist/dev/hookyns/transformer/src");
        SocketServer = new SocketServer(cache, 9876);
        SocketServer.Start();

        // NodejsPlatform platform = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libnode"));
        //
        // var env = JSNativeApi.CreateEnvironment(
        //     (JSNativeApi.Interop.napi_platform)platform,
        //     Console.WriteLine,
        //     null
        // );
        //
        // _scope = new(JSValueScopeType.Root, env);
        _scope = new(JSValueScopeType.Root);
    }

    public void Dispose()
    {
        SocketServer.Dispose();
        _scope.Dispose();
    }
}