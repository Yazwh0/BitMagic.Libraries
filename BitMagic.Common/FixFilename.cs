namespace BitMagic.Common;

public static class FixFilenameExtension
{
    public static string FixFilename(this string filename)
    {
#if OS_WINDOWS
        return char.ToLower(filename[0]) + filename[1..];
#endif
#if OS_LINUX
        return path;
#endif
    }
}
