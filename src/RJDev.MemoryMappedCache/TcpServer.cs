// using System;
// using System.Runtime.InteropServices.JavaScript;
//
// namespace RJDev.MemoryMappedCache;
//
// public partial class TcpServer
// {
//     [JSImport("nodeVersion", "main.mjs")]
//     internal static partial string GetNodeVersion();
//
//     [JSExport]
//     internal static void Run(int port)
//     {
//         Console.WriteLine($"Node version: {GetNodeVersion()}");
//         Console.WriteLine($"Running TcpServer on port: {port}");
//     }
// }