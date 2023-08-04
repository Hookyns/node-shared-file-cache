using System;
using System.Runtime.InteropServices;

namespace RJDev.MemoryMappedFiles.Module;

/// <summary></summary>
public struct FileMemoryRef : IDisposable
{
    private bool _disposed = false;

    /// <summary>
    /// Pointer to an unmanaged memory.
    /// </summary>
    public readonly nint Content;

    /// <summary></summary>
    private FileMemoryRef(nint content)
    {
        Content = content;
    }

    /// <summary>
    /// Create FileMemoryRef from byte array.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static FileMemoryRef From(byte[] data)
    {
        nint memory = Marshal.AllocHGlobal(4 + data.Length);
        Marshal.WriteInt32(memory, data.Length);
        Marshal.Copy(data, 0, memory + 4, data.Length);
        return new FileMemoryRef(memory);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Marshal.FreeHGlobal(Content);

        _disposed = true;
    }
}