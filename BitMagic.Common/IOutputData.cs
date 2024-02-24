using System.Collections.Generic;

namespace BitMagic.Common
{
    public interface IOutputData
    {
        byte[] Data { get; }
        int Address { get; }
        bool RequiresReval { get; }
        List<string> RequiresRevalNames { get; }
        void ProcessParts(bool finalParse);
        void WriteToConsole(IEmulatorLogger logger);
        SourceFilePosition Source { get; }
        bool CanStep { get; }
        public IScope Scope { get; }
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
