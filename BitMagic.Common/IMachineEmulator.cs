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
        IList<IAsmVariable> AmbiguousVariables { get; }
        bool TryGetValue(string name, SourceFilePosition source, out int result);
    }
}
