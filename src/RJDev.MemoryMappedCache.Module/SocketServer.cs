using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RJDev.MemoryMappedCache.Module;

// class Message
// {
//     public required int Size { get; init; }
//     public required Memory<byte> Data { get; init; }
// }

public class SocketServer : IDisposable
{
    private readonly MemoryMappedCache _cache;
    private readonly ConcurrentDictionary<SocketClient, bool> _clients = new();
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Task _acceptConnectionsTask;

    // private readonly object _state = new ();
    // private readonly ConcurrentDictionary<Socket, Memory<byte>> _buffers = new ();
    // private readonly WatsonTcpServer _server;
    // private readonly CancellationTokenSource _cancellationTokenSource;

    public SocketServer(MemoryMappedCache cache, int tcpPort)
    {
        _cache = cache;
        _cancellationTokenSource = new CancellationTokenSource();

        _listener = new TcpListener(IPAddress.Loopback, tcpPort);
        // _listener.Server.NoDelay = true;

        _acceptConnectionsTask =
            new Task(AcceptConnections, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);

        // _listener.BeginAcceptSocket(AcceptSocket, _state);

        // _server = new WatsonTcpServer("127.0.0.1", tcpPort);
        // _server.Events.MessageReceived += OnMessageReceived;
        // _server.Events.StreamReceived += OnStreamReceived;
        // _server.Events.ExceptionEncountered += (sender, args) =>
        // {
        //     Console.WriteLine($"Exception: {args.Exception}");
        // };
        //
        // _server.Events.ClientConnected += (sender, args) =>
        // {
        //     Console.WriteLine($"Client connected");
        // };
        // _server.Events.ClientDisconnected += (sender, args) => Console.WriteLine($"Client connected");
        // _server.Callbacks.SyncRequestReceived = OnSyncRequestReceived;
    }

    private async void AcceptConnections()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
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
            Console.WriteLine("Cancelled");
        }
    }

    // private SyncResponse OnSyncRequestReceived(SyncRequest req)
    // {
    //     string fileName = Encoding.UTF8.GetString(req.Data);
    //     return new SyncResponse(req, await _cache.GetFile(fileName));
    // }

    // private async void OnMessageReceived(object? sender, MessageReceivedEventArgs e)
    // {
    //     try
    //     {
    //         string fileName = Encoding.UTF8.GetString(e.Data);
    //         Console.WriteLine($"File '{fileName}' requested.");
    //         _server.Send(e.Client.Guid, await _cache.GetFile(fileName));
    //         Console.WriteLine($"File '{fileName}' sent.");
    //     }
    //     catch (Exception exception)
    //     {
    //         Console.WriteLine(exception);
    //     }
    // }

    private async void OnMessageReceived(SocketClient client, ReadOnlyMemory<byte> message)
    {
        try
        {
            string fileName = Encoding.UTF8.GetString(message.Span);
            Console.WriteLine($"File '{fileName}' requested.");
            await client.SendAsync(await _cache.GetFile(fileName)).ConfigureAwait(false);
            Console.WriteLine($"File '{fileName}' sent.");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            await client.SendAsync(Array.Empty<byte>()).ConfigureAwait(false);
        }
    }

    public void Start()
    {
        _listener.Start();
        _acceptConnectionsTask.Start();
        // _server.Start();

        // while (!_cancellationTokenSource.Token.IsCancellationRequested)
        // {
        //     var acceptedSocket = await _server.AcceptSocketAsync();
        //     acceptedSocket.Star
        // }
    }

    public void Stop()
    {
        try
        {
            _listener.Stop();
            _cancellationTokenSource.Cancel();
            // _server.Stop();
        }
        // Supress cancellation exception
        catch (OperationCanceledException)
        {
            Console.WriteLine("Cancelled");
        }
    }

    public void Dispose()
    {
        try
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _acceptConnectionsTask.Dispose();

            foreach ((SocketClient client, bool _) in _clients)
            {
                client.Dispose();
            }
        }
        // Supress cancellation exception
        catch (OperationCanceledException)
        {
            Console.WriteLine("Cancelled");
        }
    }
}