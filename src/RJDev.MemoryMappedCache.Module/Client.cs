using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JavaScript.NodeApi;

namespace RJDev.MemoryMappedCache.Module;

[JSExport]
public class Client : IDisposable
{
    // private readonly WatsonTcpClient _client;
    private TaskCompletionSource<string>? _taskCompletionSource;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly SocketClient _client;

    public Client(int tcpPort)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var tcpClient = new TcpClient(IPAddress.Loopback.ToString(), tcpPort);
        // tcpClient.Connect();
        _client = new SocketClient(tcpClient, _cancellationTokenSource.Token);
        _client.OnMessageReceived += OnMessageReceived;

        // _client = new WatsonTcpClient("127.0.0.1", tcpPort);
        // _client.Events.MessageReceived += OnMessageReceived;
        // _client.Events.StreamReceived += OnStreamReceived;
        //
        // _client.Events.ServerConnected += (sender, args) => Console.WriteLine("Server connected");
        // _client.Events.ServerDisconnected += (sender, args) => Console.WriteLine("Server disconnected");
        // _client.Events.ExceptionEncountered += (sender, args) =>
        // {
        //     Console.WriteLine($"Exception: {args.Exception}");
        // };
        //
        // _client.Connect();
    }

    private void OnMessageReceived(SocketClient client, ReadOnlyMemory<byte> message)
    {
        string content = Encoding.UTF8.GetString(message.Span);
        _taskCompletionSource?.SetResult(content);
    }

    // private void OnMessageReceived(object? sender, MessageReceivedEventArgs e)
    // {
    //     string content = Encoding.UTF8.GetString(e.Data);
    //     _taskCompletionSource.SetResult(content);
    // }
    //
    // private void OnStreamReceived(object? sender, StreamReceivedEventArgs e)
    // {
    //     throw new System.NotImplementedException();
    // }

    public JSPromise GetFileAsync(string fileName)
    {
        return new JSPromise(async resolve => { resolve(await GetFileAsyncCore(fileName)); });
    }

    public JSValue GetFile(string fileName)
    {
        var data = _client.SendAndWait(Encoding.UTF8.GetBytes(fileName));
        return Encoding.UTF8.GetString(data.Span);

        // SpinWait sw = new();
        // while (!task.IsCompleted && !_cancellationTokenSource.IsCancellationRequested)
        // {
        //     sw.SpinOnce();
        // }
        //
        // if (_cancellationTokenSource.IsCancellationRequested)
        // {
        //     return JSValue.Undefined;
        // }

        // return task.Result;
    }

    private async Task<string> GetFileAsyncCore(string fileName)
    {
        if (_taskCompletionSource != null)
        {
            throw new InvalidOperationException("Previous request is not completed yet! " +
                "One client is not allowed to do parallel requests.");
        }

        try
        {
            _taskCompletionSource = new();

            await _client.SendAsync(Encoding.UTF8.GetBytes(fileName)).ConfigureAwait(false);
            Console.WriteLine($"Requesting file... {fileName}");
            return await _taskCompletionSource.Task.ConfigureAwait(false);
        }
        finally
        {
            _taskCompletionSource = null;
        }
    }

    public void Dispose()
    {
        try
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _client.Dispose();
        }
        // Supress cancellation exception
        catch (OperationCanceledException)
        {
            Console.WriteLine("Cancelled");
        }
    }
}