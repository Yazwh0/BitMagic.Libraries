using System;
using System.Collections.Generic;
using System.IO;

namespace BitMagic.Common;

public static class FileCache
{
    private readonly static Dictionary<string, FileData> Cache = [];
    private readonly static object LockObj = new();

    public static byte[] GetFileData(string filename)
    {
        lock (LockObj)
        {
            if (Cache.ContainsKey(filename))
                return Cache[filename].Data;

            var data = File.ReadAllBytes(filename);

            Cache.Add(filename, new FileData() { Filename = filename, Data = data, LastWriteTimeUtc = File.GetLastWriteTimeUtc(filename) });

            return data;
        }
    }

    public static DateTime GetLastWriteTimeUtc(string filename)
    {
        lock (LockObj)
        {
            if (!Cache.ContainsKey(filename))
            {
                var data = File.ReadAllBytes(filename);

                Cache.Add(filename, new FileData() { Filename = filename, Data = data, LastWriteTimeUtc = File.GetLastWriteTimeUtc(filename) });
            }

            var lastWriteTimeUtc = File.GetLastWriteTimeUtc(filename);

            if (lastWriteTimeUtc != Cache[filename].LastWriteTimeUtc)
            {
                var data = File.ReadAllBytes(filename);
                Cache[filename].Data = data;
                Cache[filename].LastWriteTimeUtc = lastWriteTimeUtc;
            }

            return Cache[filename].LastWriteTimeUtc;
        }
    }

    public static void SetFileData(string filename, byte[] data)
    {
        lock (LockObj)
        {
            File.WriteAllBytes(filename, data);

            // can be improved.
            if (Cache.ContainsKey(filename))
                Cache.Remove(filename);

            Cache.Add(filename, new FileData() { Filename = filename, Data = data, LastWriteTimeUtc = File.GetLastWriteTimeUtc(filename) });
        }
    }

    public static void Clear()
    {
        lock (LockObj)
        {
            Cache.Clear();
        }
    }
}

public class FileData
{
    public string Filename { get; set; } = "";
    public byte[] Data { get; set; } = [];
    public DateTime LastWriteTimeUtc { get; set; }
}