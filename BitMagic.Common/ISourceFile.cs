using System;

namespace BitMagic.Common;

public interface ISourceFile
{
    string Name { get; set; }
    string Path { get; set; }
    int ReferenceId { get; set; }
    string Origin { get; set; }
    bool Volatile { get; set; }
    Action Generate { get; set; }

    string GetContent();
}
