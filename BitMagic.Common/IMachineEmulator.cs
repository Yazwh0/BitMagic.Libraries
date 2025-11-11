using System;
using System.Collections.Generic;

namespace BitMagic.Common
{
    public interface IMachine
    {
        string Name { get; }
        int Version { get; }
        IVariables Variables { get; }
        ICpu Cpu { get; }
    }

    public enum VariableDataType
    {
        Constant,
        ProcStart,
        ProcEnd,
        SegmentStart,
        LabelPointer,
        Byte,
        Sbyte,
        Char,
        Short,
        Ushort,
        Int,
        Uint,
        Long,
        Ulong,
        String,
        FixedStrings,

        Ptr,

        BytePtr,
        SbytePtr,
        CharPtr,
        ShortPtr,
        UshortPtr,
        IntPtr,
        UintPtr,
        LongPtr,
        UlongPtr,
        StringPtr,
        FixedStringsPtr
    }

    public enum VariableType
    {
        CompileConstant,
        DebuggerExpression
    }

    public interface IAsmVariable
    {
        string Name { get; }
        int Value { get; }
        VariableDataType VariableDataType { get; }
        VariableType VariableType { get; }
        int Length { get; }
        bool Array { get; }
        bool RequiresReval { get; }
        Func<bool, (int Value, bool RequiresReval)> Evaluate { get; }
    }

    public interface IVariables
    {
        IReadOnlyDictionary<string, IAsmVariable> Values { get; }
        IList<IAsmVariable> AmbiguousVariables { get; }
        bool TryGetValue(string name, SourceFilePosition source, out IAsmVariable? result);
        bool TryGetValue(int value, SourceFilePosition source, out IAsmVariable? result);
    }
}
