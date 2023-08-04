using System;
using Microsoft.JavaScript.NodeApi;

namespace RJDev.MemoryMappedFiles.Tests;

public class SocketServerFixture : IDisposable
{
    private JSValueScope _scope;

    public SocketServerFixture()
    {
        Module.Module.StartCacheServer("F:/Work/packages/memory-mapped-cache/src/node/test/test-files", new[] { "**/*" }, 9876);

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
        Module.Module.StopCacheServer();
        _scope.Dispose();
    }
}