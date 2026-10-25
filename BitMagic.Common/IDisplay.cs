using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace BitMagic.Common;

public class BitImage
{
    public Memory<PixelRgba> DrawPixels { get; internal set; }
    public Memory<PixelRgba> RenderPixels { get; internal set; }
    
    private readonly PixelRgba[] _pixelsA;
    private readonly PixelRgba[] _pixelsB;

    private bool _state = false;

    public int Width { get; }
    public int Height { get; }

    public BitImage(int width, int height)
    {
        _pixelsA = new PixelRgba[height * width];
        _pixelsB = new PixelRgba[height * width];
        DrawPixels = _pixelsA;
        RenderPixels = _pixelsB;
        Width = width;
        Height = height;
    }

    public void Switch()
    {
        _state = !_state;

        if (_state)
        {
            DrawPixels = _pixelsA;
            RenderPixels = _pixelsB;
        } 
        else
        {
            DrawPixels = _pixelsB;
            RenderPixels = _pixelsA;
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct PixelRgba : IEquatable<PixelRgba>
{
    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public PixelRgba()
    {
        R = 0;
        G = 0;
        B = 0;
        A = 0;
    }

    public PixelRgba(byte r, byte g, byte b, byte a = 0xff)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    // RGB, RGBA, RRGGBB or RRGGBBAA, the leading # is optional
    public static PixelRgba ParseHex(string hex)
    {
        hex = hex.TrimStart('#');

        if (hex.Length == 3 || hex.Length == 4)
            hex = string.Concat(hex.Select(c => $"{c}{c}"));

        if (hex.Length != 6 && hex.Length != 8)
            throw new FormatException($"'{hex}' is not a valid colour, expected RGB, RGBA, RRGGBB or RRGGBBAA.");

        var value = Convert.ToUInt32(hex, 16);

        if (hex.Length == 6)
            return new PixelRgba((byte)(value >> 16), (byte)(value >> 8), (byte)value);

        return new PixelRgba((byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value);
    }

    public bool Equals(PixelRgba other) => R == other.R && G == other.G && B == other.B && A == other.A;
    public override bool Equals(object? obj) => obj is PixelRgba other && Equals(other);
    public override int GetHashCode() => R | (G << 8) | (B << 16) | (A << 24);
    public static bool operator ==(PixelRgba left, PixelRgba right) => left.Equals(right);
    public static bool operator !=(PixelRgba left, PixelRgba right) => !left.Equals(right);

    public override string ToString() => $"R:${R:X2} G:${G:X2} B:${B:X2} A:${A:X2}";
}
