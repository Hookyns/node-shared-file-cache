﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RJDev.MemoryMappedCache;

public class Cache
{
    private readonly string _workingDirectory;
    private readonly ConcurrentDictionary<string, byte[]> _files = new();

    /// <summary>
    /// List of files that are currently being read.
    /// </summary>
    private readonly ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _readingFiles = new();

    /// <summary>
    /// Cancelation token sources for writing files.
    /// </summary>
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _writingFiles = new();

    /// <summary>
    /// Locking object.
    /// </summary>
    private readonly object _lock = new();

    public Cache(string workingDirectory)
    {
        _workingDirectory = workingDirectory;
    }

    /// <summary>
    /// Load all files from working directory to the cache.
    /// </summary>
    public void LoadFiles()
    {
        foreach (string file in Directory.GetFiles(_workingDirectory, "*", SearchOption.AllDirectories))
        {
            _files.TryAdd(file, File.ReadAllBytes(file));
        }
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
    public async Task<byte[]> GetFile(string fileName)
    {
        if (_files.TryGetValue(fileName, out byte[]? file))
        {
            return file;
        }

        if (_readingFiles.TryGetValue(fileName, out TaskCompletionSource<byte[]>? task))
        {
            return await task.Task;
        }

        // Try to get file again. It might have been added by another thread in the meantime.
        if (_files.TryGetValue(fileName, out byte[]? file2))
        {
            return file2;
        }

        // Create completion source and add it to the dictionary;
        // so other threads can await it if they ask for the same file.
        var tcs = new TaskCompletionSource<byte[]>();

        if (!_readingFiles.TryAdd(fileName, tcs))
        {
            // It was added by another thread in the meantime; await it.
            return await _readingFiles[fileName].Task;
        }

        try
        {
            string path = Path.GetFullPath(fileName, _workingDirectory);
            byte[] bytes = await File.ReadAllBytesAsync(path);

            _files.TryAdd(fileName, bytes);
            _readingFiles.TryRemove(fileName, out _);
            tcs.TrySetResult(bytes);

            return bytes;
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
            _readingFiles.TryRemove(fileName, out _);
            throw;
        }
    }

    /// <summary>
    /// Store file in the cache, also store it on the disk.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="content"></param>
    public async Task SetFile(string fileName, byte[] content)
    {
        if (_files.ContainsKey(fileName))
        {
            lock (_lock)
            {
                // Replace file in cache
                _files.AddOrUpdate(fileName, content, (_, _) => content);
                _readingFiles.TryRemove(fileName, out _);
            }
        }

        var myTokenSource = new CancellationTokenSource();

        _writingFiles.AddOrUpdate(fileName, myTokenSource, (_, currentToken) =>
        {
            currentToken.Cancel();
            return myTokenSource;
        });


        string path = Path.GetFullPath(fileName, _workingDirectory);
        await File.WriteAllBytesAsync(path, content, myTokenSource.Token);
    }
}