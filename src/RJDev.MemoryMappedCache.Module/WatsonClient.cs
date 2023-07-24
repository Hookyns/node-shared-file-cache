// using System;
// using System.Collections.Generic;
// using System.Text;
// using System.Threading.Tasks;
// using Microsoft.JavaScript.NodeApi;
// using WatsonTcp;
//
// namespace RJDev.MemoryMappedCache.Module;
//
// [JSExport]
// public class Client
// {
//     private readonly WatsonTcpClient _client;
//     private TaskCompletionSource<string> _taskCompletionSource = new();
//
//     public Client(int tcpPort)
//     {
//         _client = new WatsonTcpClient("127.0.0.1", tcpPort);
//         _client.Events.MessageReceived += OnMessageReceived;
//         _client.Events.StreamReceived += OnStreamReceived;
//
//         _client.Events.ServerConnected += (sender, args) => Console.WriteLine("Server connected");
//         _client.Events.ServerDisconnected += (sender, args) => Console.WriteLine("Server disconnected");
//         _client.Events.ExceptionEncountered += (sender, args) =>
//         {
//             Console.WriteLine($"Exception: {args.Exception}");
//         };
//
//         _client.Connect();
//     }
//
//     private void OnMessageReceived(object? sender, MessageReceivedEventArgs e)
//     {
//         string content = Encoding.UTF8.GetString(e.Data);
//         _taskCompletionSource.SetResult(content);
//     }
//
//     private void OnStreamReceived(object? sender, StreamReceivedEventArgs e)
//     {
//         throw new System.NotImplementedException();
//     }
//
//     public JSPromise GetFile(string fileName)
//     {
//         _taskCompletionSource = new();
//
//         return new JSPromise(async resolve =>
//         {
//             await _client.SendAsync(fileName, new Dictionary<string, object>());
//             Console.WriteLine($"Requesting file... {fileName}");
//             resolve(await _taskCompletionSource.Task);
//         });
//     }
// }