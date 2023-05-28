using System;

namespace BitMagic.Common;

public enum SourceFileOrigin
{
    FileSystem,
    Static,
    Decompiled,
    Intermediary,
}

public interface ISourceFile
{
    string Name { get; }
    string Path { get; }
    int? ReferenceId { get; }
    SourceFileOrigin Origin { get; }
    bool Volatile { get; }
    Action Generate { get; }
    bool ActualFile { get; }

    public ISourceFile? Parent { get; }

    string GetContent();
}
