using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BitMagic.Common;

public class DocumentCache
{
    public static DocumentCache Instance { get; set; } = new();

    private readonly Dictionary<string, SourceFile> _files = new();

    public string[] GetFile(string filename)
    {
        if (_files.TryGetValue(filename, out var sourceFile))
            return sourceFile.Lines;

        return [];
    }

    public async Task AddFile(string filename)
    {
        var lines = await File.ReadAllLinesAsync(filename);

        _files.Add(filename, new SourceFile() { Filename = filename, Lines = lines });
    }

    public async Task<string[]> ReadAllTextAsync(string filename)
    {
        if (_files.ContainsKey(filename))
            return _files[filename].Lines;

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

        _files.Add(filename, new SourceFile() { Filename = filename, Lines = content.ToArray() });
    }

    public async Task UpdateFile(string filename)
    {
        _files.Remove(filename, out _);
        await AddFile(filename);
    }

    public bool IsInCache(string filename) => _files.ContainsKey(filename);

    private sealed class SourceFile
    {
        public string Filename { get; set; } = "";
        public string[] Lines { get; set; } = Array.Empty<string>();
    }
}
