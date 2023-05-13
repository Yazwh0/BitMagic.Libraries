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

    public enum VariableType
    {
        Constant,
        ProcStart,
        ProcEnd,
        SegmentStart,
        Pointer,
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
        FixedStrings
    }

    public interface IAsmVariable
    {
        string Name { get; }
        int Value { get; }
        VariableType VariableType { get; }
        int Length { get; }
        bool Array { get; }
    }

    public interface IVariables
    {
        IReadOnlyDictionary<string, IAsmVariable> Values { get; }
        bool TryGetValue(string name, int lineNumber, out int result);
    }
}
