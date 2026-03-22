using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BitMagic.Common;

public class DocumentCache : IDisposable
{
    public static DocumentCache Instance { get; set; } = new();

    private readonly Dictionary<string, SourceFile> _files = new();

    public string[] GetFile(string filename)
    {
        if (_files.TryGetValue(filename, out var sourceFile))
            return sourceFile.GetLines();

        return [];
    }

    public async Task AddFile(string filename)
    {
        var toAdd = new SourceFile(filename);
        await toAdd.Load();
        _files.Add(filename, toAdd);
    }

    public async Task<string[]> ReadAllTextAsync(string filename)
    {
        if (_files.ContainsKey(filename))
            return _files[filename].GetLines();

        var content = await File.ReadAllTextAsync(filename);
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        SetFileContent(filename, lines);
        return lines;
    }

    public async Task WriteAllLinesAsync(string filename, IReadOnlyList<string> content)
    {
        SetFileContent(filename, content);
        await File.WriteAllLinesAsync(filename, content);
    }

    public void SetFileContent(string filename, IReadOnlyList<string> content)
    {
        if (_files.ContainsKey(filename))
        {
            _files[filename].Lines = content.ToArray();
            return;
        }

        _files.Add(filename, new SourceFile(filename, content.ToArray()));
    }

    public async Task UpdateFile(string filename)
    {
        if (_files.Remove(filename, out var removed))
        {
            removed.Dispose();
        }

        await AddFile(filename);
    }

    public bool IsInCache(string filename) => _files.ContainsKey(filename);

    public void Dispose()
    {
        foreach (var i in _files)
            i.Value.Dispose();
    }

    private sealed class SourceFile : IDisposable
    {
        public string Filename { get; }
        private string[] _lines = [];
        public string[] Lines
        {
            get => _lines;
            set
            {
                _lines = value;
                ReadTime = DateTime.Now;
            }
        }

        public DateTime ReadTime { get; private set; }
        private FileSystemWatcher? _watcher = null;
        private Timer? _debounceTimer;

        public SourceFile(string filename)
        {
            Filename = filename;
            ReadTime = DateTime.MinValue;
        }

        public SourceFile(string filename, string[] lines)
        {
            Filename = filename;
            Lines = lines;
            SetupFileWatcher();
        }

        public string[] GetLines()
        {
            if (File.GetLastWriteTime(Filename) > ReadTime)
            {
                Console.WriteLine($"File {Filename} has been modified since last read. Reloading...");
                Lines = File.ReadAllLines(Filename);
            }

            return Lines;
        }

        public async Task Load()
        {
            Lines = await File.ReadAllLinesAsync(Filename);
            SetupFileWatcher();
        }

        private void SetupFileWatcher()
        {
            var directory = Path.GetDirectoryName(Filename)!;
            var fileName = Path.GetFileName(Filename);

            _watcher = new FileSystemWatcher(directory, fileName)
            {
                NotifyFilter = NotifyFilters.LastWrite |
                               NotifyFilters.Size
            };

            _watcher.Changed += _watcher_Changed;
            _watcher.EnableRaisingEvents = true;
        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File {Filename} has changed. Reloading...");
            _debounceTimer?.Dispose();

            _debounceTimer = new Timer(_ =>
            {
                var lines = TryReadFileWithRetry(Filename);
                if (lines != null)
                {
                    Lines = lines;
                    Console.WriteLine($"File {Filename} reloaded successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to reload file {Filename} after multiple attempts.");
                }
            }, null, 100, Timeout.Infinite);
        }

        private static string[]? TryReadFileWithRetry(string path, int retries = 10, int delayMs = 50)
        {
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    return File.ReadAllLines(path);
                }
                catch (IOException)
                {
                    Thread.Sleep(delayMs);
                }
                catch (UnauthorizedAccessException)
                {
                    Thread.Sleep(delayMs);
                }
            }

            return null;
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Changed -= _watcher_Changed;
                _watcher.Dispose();
                _debounceTimer?.Dispose();
            }
        }
    }
}
