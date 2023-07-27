using System;
using System.Buffers;
using System.IO;
using System.Net.Sockets;
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
    private Task? _receiveTask;
    private bool _disposed;

    /// <summary>
    /// Event emmiting when message is received.
    /// </summary>
    public event MessageReceivedEvent? OnMessageReceived;

    /// <summary>
    /// Event emmiting when the client disconnects.
    /// </summary>
    public event ClientDisconnectedEvent? OnDisconnectClient;

    /// <summary></summary>
    public SocketClient(TcpClient tcpClient, CancellationToken cancellationToken)
    {
        _tcpClient = tcpClient;
        _stream = tcpClient.GetStream();
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    }

    /// <summary>
    /// Start receiving data from client in the background.
    /// </summary>
    public void StartReceive()
    {
        _receiveTask = new Task(ReceiveData, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
        _receiveTask.Start();
    }

    /// <summary>
    /// Send async data to client.
    /// </summary>
    /// <param name="data"></param>
    public async Task SendAsync(byte[] data)
    {
        _stream.Write(BitConverter.GetBytes(data.Length));
        await _stream.WriteAsync(data, 0, data.Length, _cancellationTokenSource.Token).ConfigureAwait(false);

        if (_receiveTask == null)
        {
            OnMessageReceived?.Invoke(this, WaitForData());
        }
    }

    /// <summary>
    /// Send data to client and wait for response.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ReadOnlyMemory<byte> SendAndWait(byte[] data)
    {
        if (_receiveTask != null)
        {
            throw new InvalidOperationException("Cannot send data synchronously when StartReceive() was called.");
        }

        var buffer = ArrayPool<byte>.Shared.Rent(data.Length + 4);
        BitConverter.GetBytes(data.Length).CopyTo(buffer, 0);
        data.CopyTo(buffer, 4);
        _stream.Write(buffer, 0, data.Length + 4);
        // _stream.Write(BitConverter.GetBytes(data.Length));
        // _stream.Write(data, 0, data.Length);
        // _stream.Flush();
        return WaitForData();
    }

    /// <summary>
    /// Send data to client and wait for response.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal void SendRaw(byte[] data)
    {
        _stream.Write(data);
    }

    private ReadOnlyMemory<byte> WaitForData()
    {
        var messageLengthBuffer = ArrayPool<byte>.Shared.Rent(4);
        _stream.ReadExactly(messageLengthBuffer, 0, 4);

        // Reconstruct message length
        int length = BitConverter.ToInt32(messageLengthBuffer);

        // Reconstruct message data
        var messageBuffer = ArrayPool<byte>.Shared.Rent(length);
        _stream.ReadExactly(messageBuffer, 0, length);

        return messageBuffer.AsMemory(0, length);
    }

    private async void ReceiveData()
    {
        try
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var messageLengthBuffer = ArrayPool<byte>.Shared.Rent(4);
                    await _stream.ReadExactlyAsync(messageLengthBuffer, 0, 4, _cancellationTokenSource.Token)
                        .ConfigureAwait(false);

                    // Reconstruct message length
                    int length = BitConverter.ToInt32(messageLengthBuffer);

                    // Reconstruct message data
                    var messageBuffer = ArrayPool<byte>.Shared.Rent(length);
                    await _stream.ReadExactlyAsync(messageBuffer, 0, length, _cancellationTokenSource.Token)
                        .ConfigureAwait(false);

                    OnMessageReceived?.Invoke(this, messageBuffer.AsMemory(0, length));
                }
                catch (OperationCanceledException)
                {
                }
                catch (IOException)
                {
                    // TODO: Means that stream was closed. We should close the client.

                    Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to receive data from client: {e}");
                }
            }
        }
        finally
        {
            if (_tcpClient.Connected)
            {
                _tcpClient.Close();
            }

            OnDisconnectClient?.Invoke(this);
        }
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

        _receiveTask?.Dispose();

        _disposed = true;
    }
}