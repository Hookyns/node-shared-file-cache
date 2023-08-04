using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace RJDev.MemoryMappedFiles.Module;

/// <summary>
/// Thread-safe cache mapping files info memory.
/// </summary>
public sealed class MemoryMappedCache : IDisposable
{
    private readonly string _workingDirectory;
    private bool _disposed;

    /// <summary>
    /// List of files in the cache.
    /// </summary>
    private readonly ConcurrentDictionary<string, FileMemoryRef> _files = new();

    /// <summary>
    /// List of files that are currently being read.
    /// </summary>
    private readonly ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _readingFiles = new();

    /// <summary>
    /// Cancelation token sources for curently writing files.
    /// </summary>
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _writingFiles = new();

    /// <summary>
    /// Locking object.
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    /// </summary>
    /// <param name="workingDirectory"></param>
    public MemoryMappedCache(string workingDirectory)
    {
        _workingDirectory = workingDirectory;
    }

    ~MemoryMappedCache()
    {
        ReleaseUnmanagedResources();
    }

    /// <summary>
    /// Load all files from working directory to the cache.
    /// </summary>
    public int LoadFiles(string[] include)
    {
        Matcher matcher = new();
        matcher.AddIncludePatterns(include);

        PatternMatchingResult result = matcher.Execute(
            new DirectoryInfoWrapper(
                new DirectoryInfo(_workingDirectory)
            )
        );

        foreach (var fileMatch in result.Files)
        {
            byte[] fileBuffer = File.ReadAllBytes(Path.Combine(_workingDirectory, fileMatch.Path));
            AddFileToCache(fileMatch.Path, fileBuffer);
        }

        // // TODO: Replace by glob matcher
        // foreach (string file in Directory.GetFiles(_workingDirectory, "*", SearchOption.AllDirectories))
        // {
        //     string fileName = Path.GetRelativePath(_workingDirectory, file).Replace('\\', '/');
        //     byte[] fileBuffer = File.ReadAllBytes(file);
        //
        //     AddFileToCache(fileName, fileBuffer);
        // }

        return _files.Count;
    }

    /// <summary>
    /// Try to get file from the cache. It returns false if file is not in the cache.
    /// </summary>
    /// <remarks>
    /// The <see cref="GetFile"/> tries to get file from or or it loads it from disk.
    /// </remarks>
    /// <param name="fileName"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    public bool TryGetFile(string fileName, [MaybeNullWhen(false)] out long file)
    {
        if (_files.TryGetValue(fileName, out FileMemoryRef fileRef))
        {
            file = fileRef.Content.ToInt64();
            return true;
        }
        file = default;
        return false;
    }

    /// <summary>
    /// Get file from the cache.
    /// </summary>
    /// <remarks>
    /// If the file is not in the cache, file will be loaded from the disk.
    /// The file will be read only once. This method is thread-safe.
    /// </remarks>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<long> GetFile(string fileName)
    {
        if (_files.TryGetValue(fileName, out FileMemoryRef file))
        {
            return file.Content.ToInt64();
        }

        if (_readingFiles.TryGetValue(fileName, out TaskCompletionSource<byte[]>? task))
        {
            await task.Task.ConfigureAwait(false);
            return _files[fileName].Content.ToInt64();
        }

        // Try to get file again. It might have been added by another thread in the meantime.
        if (_files.TryGetValue(fileName, out FileMemoryRef file2))
        {
            return file2.Content.ToInt64();
        }

        // Create completion source and add it to the dictionary;
        // so other threads can await it if they ask for the same file.
        var tcs = new TaskCompletionSource<byte[]>();

        if (!_readingFiles.TryAdd(fileName, tcs))
        {
            // It was added by another thread in the meantime; await it.
            await _readingFiles[fileName].Task.ConfigureAwait(false);

            return _files[fileName].Content.ToInt64();
        }

        try
        {
            string path = Path.GetFullPath(fileName, _workingDirectory);
            byte[] bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);

            // _files.TryAdd(fileName, bytes);
            AddFileToCache(fileName, bytes);
            _readingFiles.TryRemove(fileName, out _);
            tcs.TrySetResult(bytes);

            return _files[fileName].Content.ToInt64();
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
            _readingFiles.TryRemove(fileName, out _);
            throw;
        }
    }

    private void AddFileToCache(string fileName, byte[] content)
    {
        // // Create buffer for data (length + file content)
        // byte[] buffer = new byte[4 + content.Length];
        //
        // // Add length of the file to the buffer
        // BitConverter.GetBytes(content.Length).CopyTo(buffer, 0);
        // // Add file content to the buffer
        // content.CopyTo(buffer, 4);

        _files.TryAdd(fileName, FileMemoryRef.From(content));
    }

    // /// <summary>
    // /// Store file in the cache, also store it on the disk.
    // /// </summary>
    // /// <param name="fileName"></param>
    // /// <param name="content"></param>
    // public async Task SetFile(string fileName, byte[] content)
    // {
    //     if (_files.ContainsKey(fileName))
    //     {
    //         lock (_lock)
    //         {
    //             // Replace file in cache
    //             _files.AddOrUpdate(fileName, content, (_, _) => content);
    //             _readingFiles.TryRemove(fileName, out _);
    //         }
    //     }
    //
    //     var myTokenSource = new CancellationTokenSource();
    //
    //     _writingFiles.AddOrUpdate(fileName, myTokenSource, (_, currentToken) =>
    //     {
    //         currentToken.Cancel();
    //         return myTokenSource;
    //     });
    //
    //
    //     string path = Path.GetFullPath(fileName, _workingDirectory);
    //     await File.WriteAllBytesAsync(path, content, myTokenSource.Token).ConfigureAwait(false);
    // }

    private void ReleaseUnmanagedResources()
    {
        if (_disposed)
        {
            return;
        }

        foreach (var file in _files)
        {
           file.Value.Dispose();
        }

        _disposed = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
}