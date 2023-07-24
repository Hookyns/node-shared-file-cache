using RJDev.MemoryMappedCache.Module;

try
{
    var cache = new MemoryMappedCache("F:/Work/packages/rttist/dev/hookyns/transformer/src");
    var socketServer = new SocketServer(cache, 9876);
    socketServer.Start();

    Console.WriteLine("Running...");

    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}