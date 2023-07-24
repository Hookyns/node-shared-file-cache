using System;

namespace RJDev.MemoryMappedCache;

using System.Runtime.CompilerServices;

internal class Interop
{
    // [MethodImpl(MethodImplOptions.InternalCall)]
    public unsafe void Start(string workingDirectory, int port)
    {
        Console.WriteLine("Method called, {0}, {1}", workingDirectory, port);
    }

    // [MethodImpl(MethodImplOptions.InternalCall)]
    // public static unsafe extern void ResponseAddHeader(uint requestId, string name, string value);
    //
    // [MethodImpl(MethodImplOptions.InternalCall)]
    // public static unsafe extern void ResponseSendChunk(uint requestId, byte* buffer, int buffer_length);
    //
    // [MethodImpl(MethodImplOptions.InternalCall)]
    // public static unsafe extern void ResponseComplete(uint requestId, int statusCode);
    //
    // public event EventHandler<(uint RequestId, string Method, string Url, string HeadersCombined, byte[]? Body)>? OnIncomingRequest;
    //
    // // TODO: Make sure this doesn't get trimmed if AOT compiled
    // // The requestId is a uint instead of a long because otherwise the runtime fails with an error like "CANNOT HANDLE INTERP ICALL SIG VLI"
    // // For a more complete implementation you might want to use a GUID string instead.
    // private unsafe void Start(uint requestId, string method, string url, string headersCombined, byte[]? body)
    //     => OnIncomingRequest?.Invoke(this, (requestId, method, url, headersCombined, body));
}