using System;
using System.Buffers;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RJDev.MemoryMappedCache.Module;

public class SocketClient : IDisposable
{
    public delegate void MessageReceivedEvent(SocketClient client, ReadOnlyMemory<byte> message);
    public delegate void DisconnectClientEvent(SocketClient client);

    private readonly TcpClient _tcpClient;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly NetworkStream _stream;
    private Task? _receiveTask;
    public event MessageReceivedEvent? OnMessageReceived;
    public event DisconnectClientEvent? OnDisconnectClient;

    public SocketClient(TcpClient tcpClient, CancellationToken cancellationToken)
    {
        _tcpClient = tcpClient;
        _stream = tcpClient.GetStream();
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    }

    public void StartReceive()
    {
        _receiveTask = new Task(ReceiveData, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
        _receiveTask.Start();
    }

    public async Task SendAsync(byte[] data)
    {
        _stream.Write(BitConverter.GetBytes(data.Length));
        await _stream.WriteAsync(data, 0, data.Length, _cancellationTokenSource.Token).ConfigureAwait(false);

        if (_receiveTask == null)
        {
            OnMessageReceived?.Invoke(this, WaitForData());
        }
    }

    public ReadOnlyMemory<byte> SendAndWait(byte[] data)
    {
        _stream.Write(BitConverter.GetBytes(data.Length));
        _stream.Write(data, 0, data.Length);
        return WaitForData();
    }

    public ReadOnlyMemory<byte> WaitForData()
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
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                var messageLengthBuffer = ArrayPool<byte>.Shared.Rent(4);
                await _stream.ReadExactlyAsync(messageLengthBuffer, 0, 4, _cancellationTokenSource.Token).ConfigureAwait(false);

                // Reconstruct message length
                int length = BitConverter.ToInt32(messageLengthBuffer);

                // Reconstruct message data
                var messageBuffer = ArrayPool<byte>.Shared.Rent(length);
                await _stream.ReadExactlyAsync(messageBuffer, 0, length, _cancellationTokenSource.Token).ConfigureAwait(false);

                // int totalRead = 0;
                // while (totalRead < length)
                // {
                //     int read = await _stream.ReadAsync(messageBuffer, totalRead, length - totalRead, _cancellationToken);
                //     if (read != 0)
                //     {
                //         totalRead += read;
                //     }
                // }

                OnMessageReceived?.Invoke(this, messageBuffer.AsMemory(0, length));
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Canceled");
            }
            catch (IOException)
            {
                try
                {
                    _cancellationTokenSource.Cancel();
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Canceled");
                }
                finally
                {
                    OnDisconnectClient?.Invoke(this);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public void Dispose()
    {
        _tcpClient.Client.Disconnect(false);
        _tcpClient.Dispose();
        _receiveTask?.Dispose();
    }
}