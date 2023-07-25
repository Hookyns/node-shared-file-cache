using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JavaScript.NodeApi;

namespace RJDev.MemoryMappedFiles.Module.Sockets;

/// <summary>
/// Client for the cache server.
/// </summary>
[JSExport]
public class Client : IDisposable
{
    private TaskCompletionSource<string>? _taskCompletionSource;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly SocketClient _client;

    /// <summary></summary>
    public Client(int tcpPort)
    {
        _cancellationTokenSource = new CancellationTokenSource();

        // Create TCP client
        var tcpClient = new TcpClient(IPAddress.Loopback.ToString(), tcpPort);

        // Create SocketClient wrapper
        _client = new SocketClient(tcpClient, _cancellationTokenSource.Token);
        _client.OnMessageReceived += OnMessageReceived;
    }

    private void OnMessageReceived(SocketClient client, ReadOnlyMemory<byte> message)
    {
        string content = Encoding.UTF8.GetString(message.Span);
        _taskCompletionSource?.SetResult(content);
    }

    /// <summary>
    /// Get file from cache server.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public JSPromise GetFileAsync(string fileName)
    {
        return new JSPromise(async resolve => { resolve(await GetFileAsyncCore(fileName)); });
    }

    /// <summary>
    /// Get file from cache server.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public JSValue GetFile(string fileName)
    {
        var data = _client.SendAndWait(Encoding.UTF8.GetBytes(fileName));
        return Encoding.UTF8.GetString(data.Span);
    }

    // /// <summary>
    // /// Store file in cache server.
    // /// </summary>
    // /// <param name="fileName"></param>
    // /// <returns></returns>
    // public JSPromise SetFileAsync(string fileName, string content)
    // {
    //     return new JSPromise(async resolve =>
    //     {
    //         await _client
    //             .SendAsync(MessageType.SetFile, Encoding.UTF8.GetBytes(fileName), Encoding.UTF8.GetBytes(content))
    //             .ConfigureAwait(false);
    //         resolve(JSValue.Undefined);
    //     });
    // }

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

    /// <inheritdoc />
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