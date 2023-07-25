using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RJDev.MemoryMappedFiles.Module.Sockets;

/// <summary>
/// Socket server.
/// </summary>
public class SocketServer : IDisposable
{
    private readonly MemoryMappedCache _cache;
    private readonly ConcurrentDictionary<SocketClient, bool> _clients = new();
    private readonly TcpListener _listener;

    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _acceptConnectionsTask;
    private bool _disposed;

    /// <summary></summary>
    public SocketServer(MemoryMappedCache cache, int tcpPort)
    {
        _cache = cache;

        _listener = new TcpListener(IPAddress.Loopback, tcpPort);
        _listener.Server.NoDelay = true;
    }

    /// <summary>
    /// Start listening for connections.
    /// </summary>
    public void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _acceptConnectionsTask = new Task(
            AcceptConnections,
            _cancellationTokenSource.Token,
            TaskCreationOptions.LongRunning
        );

        _listener.Start();
        _acceptConnectionsTask.Start();
    }

    /// <summary>
    /// Stop listening for connections.
    /// </summary>
    public void Stop()
    {
        try
        {
            _listener.Stop();
            _cancellationTokenSource?.Cancel();
        }
        // Supress cancellation exception
        catch (OperationCanceledException)
        {
        }
    }

    private async void AcceptConnections()
    {
        while (!_cancellationTokenSource!.IsCancellationRequested)
        {
            // Create TcpClient
            TcpClient tcpClient = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);

            // Create ScketClient
            var client = new SocketClient(tcpClient, _cancellationTokenSource.Token);

            // Set events
            client.OnMessageReceived += OnMessageReceived;
            client.OnDisconnectClient += OnDisconnectClient;

            _clients.TryAdd(client, true);

            // Start async receive
            client.StartReceive();
        }
    }

    private void OnDisconnectClient(SocketClient client)
    {
        try
        {
            _clients.TryRemove(client, out _);
            client.Dispose();
        }
        // Supress cancellation exception
        catch (OperationCanceledException)
        {
        }
    }

    private async void OnMessageReceived(SocketClient client, ReadOnlyMemory<byte> message)
    {
        try
        {
            string fileName = Encoding.UTF8.GetString(message.Span);
            await client.SendAsync(await _cache.GetFile(fileName)).ConfigureAwait(false);
        }
        catch (FileNotFoundException)
        {
            // TODO: Send some kind of message type; FileNotFound
            await client.SendAsync(Array.Empty<byte>()).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            await client.SendAsync(Array.Empty<byte>()).ConfigureAwait(false);
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
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _acceptConnectionsTask?.Dispose();

            foreach ((SocketClient client, bool _) in _clients)
            {
                client.Dispose();
            }
        }
        // Supress cancellation exception
        catch (OperationCanceledException)
        {
        }

        _disposed = true;
    }
}