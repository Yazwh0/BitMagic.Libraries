namespace BitMagic.Common;

public static class DebugConstants
{
    public static readonly uint Breakpoint          = 0b0000_0001;
    public static readonly uint SystemBreakpoint    = 0b0000_0010;
    public static readonly uint NoStop              = 0b0000_0100;
    public static readonly uint DebugAction           = 0b0000_1000;
    public static readonly uint ReadWriteFlags      = 0b1111_0000;

    // Specific breakpoints.
    public static readonly uint LoadCheck           = 0b0001_0010;  // can use bits in the RW mask because its on the stack
}
