using System;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.IO.Hashing;
using System.Runtime.InteropServices;
using StbImageSharp;

namespace BitMagic.Common;

public class RgbaImage
{
    public int Width { get; }
    public int Height { get; }
    public PixelRgba[] Pixels { get; }

    public RgbaImage(int width, int height)
    {
        Width = width;
        Height = height;
        Pixels = new PixelRgba[width * height];
    }

    internal RgbaImage(int width, int height, PixelRgba[] pixels)
    {
        Width = width;
        Height = height;
        Pixels = pixels;
    }

    public PixelRgba this[int x, int y]
    {
        get => Pixels[y * Width + x];
        set => Pixels[y * Width + x] = value;
    }

    public Span<PixelRgba> GetRow(int y) => Pixels.AsSpan(y * Width, Width);
}

/// <summary>
/// Image loading and saving. Any image library usage should be kept in here.
/// Decoding is StbImageSharp, encoding is a minimal PNG writer over the framework's zlib as it's
/// much faster than the alternatives, which matters for the debugger views.
/// </summary>
public static class ImageFile
{
    private static readonly byte[] _pngSignature = [0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a];

    public static RgbaImage Load(string filename) => Load(File.ReadAllBytes(filename));

    public static RgbaImage Load(byte[] data)
    {
        var result = ImageResult.FromMemory(data, ColorComponents.RedGreenBlueAlpha);
        var pixels = MemoryMarshal.Cast<byte, PixelRgba>(result.Data).ToArray();

        return new RgbaImage(result.Width, result.Height, pixels);
    }

    public static void SavePng(RgbaImage image, Stream stream) => SavePng(image.Pixels, image.Width, image.Height, stream);

    // 8bit RGBA, no row filtering. Filtering was benchmarked and made it slower and larger for emulator output.
    public static void SavePng(ReadOnlySpan<PixelRgba> pixels, int width, int height, Stream stream)
    {
        if (pixels.Length < width * height)
            throw new ArgumentException("Not enough pixels for the image size", nameof(pixels));

        stream.Write(_pngSignature);

        Span<byte> header = stackalloc byte[13];
        BinaryPrimitives.WriteInt32BigEndian(header, width);
        BinaryPrimitives.WriteInt32BigEndian(header[4..], height);
        header[8] = 8;  // bit depth
        header[9] = 6;  // colour type: RGBA
        header[10] = 0; // compression
        header[11] = 0; // filter
        header[12] = 0; // interlace
        WritePngChunk(stream, "IHDR"u8, header);

        var data = MemoryMarshal.AsBytes(pixels[..(width * height)]);
        var stride = width * 4;

        using var compressed = new MemoryStream();
        using (var zlib = new ZLibStream(compressed, CompressionLevel.Optimal, true))
        {
            ReadOnlySpan<byte> filterNone = [0];
            for (var y = 0; y < height; y++)
            {
                zlib.Write(filterNone);
                zlib.Write(data.Slice(y * stride, stride));
            }
        }

        WritePngChunk(stream, "IDAT"u8, compressed.GetBuffer().AsSpan(0, (int)compressed.Length));
        WritePngChunk(stream, "IEND"u8, []);
    }

    private static void WritePngChunk(Stream stream, ReadOnlySpan<byte> type, ReadOnlySpan<byte> data)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(buffer, data.Length);
        stream.Write(buffer);
        stream.Write(type);
        stream.Write(data);

        var crc = new Crc32();
        crc.Append(type);
        crc.Append(data);
        BinaryPrimitives.WriteUInt32BigEndian(buffer, crc.GetCurrentHashAsUInt32());
        stream.Write(buffer);
    }

    public static byte[] ToPng(RgbaImage image)
    {
        using var stream = new MemoryStream();
        SavePng(image, stream);
        return stream.ToArray();
    }
}
