using System;

namespace RJDev.MemoryMappedFiles.Module.Sockets.Delegates;

/// <summary>
/// Delegate for the MessageReceived event.
/// </summary>
///
public delegate void MessageReceivedEvent(SocketClient client, ReadOnlyMemory<byte> message);

/// <summary>
/// Delegate for the MessageReceived event.
/// </summary>
///
public delegate void SocketServerClientMessageReceivedEvent(SocketServerClient client, ReadOnlyMemory<byte> message);