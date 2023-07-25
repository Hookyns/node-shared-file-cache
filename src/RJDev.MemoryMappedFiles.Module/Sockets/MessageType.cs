namespace RJDev.MemoryMappedFiles.Module.Sockets;

/// <summary>
/// Available message types.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Request or response for file retrieval.
    /// </summary>
    FileRetrieval,

    /// <summary>
    /// Error messageType, file not found.
    /// </summary>
    FileNotFound,

    /// <summary>
    /// SetFile - store file in cache.
    /// </summary>
    SetFile,
}