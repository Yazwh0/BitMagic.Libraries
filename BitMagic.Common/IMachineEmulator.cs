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

    public interface IVariables
    {
        IReadOnlyDictionary<string, int> Values { get; }
        bool TryGetValue(string name, int lineNumber, out int result);
    }
}
