using System;
using System.Buffers;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RJDev.MemoryMappedFiles.Module.Sockets.Delegates;

namespace RJDev.MemoryMappedFiles.Module.Sockets;

/// <summary>
/// Socket client.
/// </summary>
public class SocketClient : IDisposable
{
    private readonly TcpClient _tcpClient;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly NetworkStream _stream;
    private bool _disposed;

    /// <summary>
    /// Event emmiting when message is received.
    /// </summary>
    public event MessageReceivedEvent? OnMessageReceived;

    /// <summary></summary>
    public SocketClient(TcpClient tcpClient, CancellationToken cancellationToken)
    {
        _tcpClient = tcpClient;
        _stream = tcpClient.GetStream();
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    }

    /// <summary>
    /// Send async data to client.
    /// </summary>
    /// <param name="data"></param>
    public async Task SendAsync(byte[] data)
    {
        _stream.Write(BitConverter.GetBytes(data.Length));
        await _stream.WriteAsync(data, 0, data.Length, _cancellationTokenSource.Token).ConfigureAwait(false);
        OnMessageReceived?.Invoke(this, WaitForData());
    }

    /// <summary>
    /// Send data to client and wait for response.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ReadOnlyMemory<byte> SendAndWait(byte[] data)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(data.Length + 4);
        BitConverter.GetBytes(data.Length).CopyTo(buffer, 0);
        data.CopyTo(buffer, 4);
        _stream.Write(buffer, 0, data.Length + 4);
        // _stream.Write(BitConverter.GetBytes(data.Length));
        // _stream.Write(data, 0, data.Length);
        // _stream.Flush();
        return WaitForData();
    }

    private ReadOnlyMemory<byte> WaitForData()
    {
        var pointerBuffer = ArrayPool<byte>.Shared.Rent(sizeof(long));
        _stream.ReadExactly(pointerBuffer, 0, sizeof(long));
        nint pointer = new(BitConverter.ToInt64(pointerBuffer));

        int length = Marshal.ReadInt32(pointer);
        var messageBuffer = ArrayPool<byte>.Shared.Rent(length);
        Marshal.Copy(pointer + 4, messageBuffer, 0, length);

        return messageBuffer.AsMemory(0, length);

        // var messageLengthBuffer = ArrayPool<byte>.Shared.Rent(4);
        // _stream.ReadExactly(messageLengthBuffer, 0, 4);
        //
        // // Reconstruct message length
        // int length = BitConverter.ToInt32(messageLengthBuffer);
        //
        // // Reconstruct message data
        // var messageBuffer = ArrayPool<byte>.Shared.Rent(length);
        // _stream.ReadExactly(messageBuffer, 0, length);
        //
        // return messageBuffer.AsMemory(0, length);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            _cancellationTokenSource.Cancel();
        }
        catch (OperationCanceledException)
        {
        }

        if (_tcpClient.Client.Connected)
        {
            _tcpClient.Client.Disconnect(false);
        }

        // try
        // {
            _tcpClient.Dispose();
        // }
        // catch
        // {
        // }

        _disposed = true;
    }
}