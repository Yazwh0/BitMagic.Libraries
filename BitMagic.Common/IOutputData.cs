using System.Collections.Generic;

namespace BitMagic.Common
{
    public interface IOutputData : IDebuggerMapItem
    {
        byte[] Data { get; }
        uint[] DebugData { get; }
        bool RequiresReval { get; }
        List<string> RequiresRevalNames { get; }
        void ProcessParts(bool finalParse);
        void WriteToConsole(IEmulatorLogger logger);
    }

    public interface IDebuggerMapItem
    {
        int Address { get; set; } // settable for relocatable code
        bool CanStep { get; }
        public IScope Scope { get; }
        SourceFilePosition Source { get; }
    }

    public record SourceFilePosition
    {
        public string Name { get; set; } = "";
        public int LineNumber { get; set; }
        public string Source { get; set; } = "";
        public ISourceFile? SourceFile { get; set; }
        public SourceFilePosition()
        {
        }
        public override string ToString() => $"{Name}:{LineNumber}\n{Source}";
    }
}
