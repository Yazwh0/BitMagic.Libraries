using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitMagic.Common;

public enum SourceFileType
{
    FileSystem,
    Static,
    Decompiled,
    Intermediary,
    Binary,
    MemoryMap
}

public interface ISourceFile
{
    string Name { get; }
    string Path { get; }                    // Include name
    int? ReferenceId { get; set; }          // for VSCode
    SourceFileType Origin { get; }

    Task UpdateContent();                   // for things like dissasembly
    bool RequireUpdate { get; }

    bool ActualFile { get; }                // On the host PC
    bool X16File { get; }                   // File to be put on the SDCard

    IReadOnlyList<ISourceFile> Parents { get; }
    IReadOnlyList<ISourceFile> Children { get; }

    IReadOnlyList<string> Content { get; }
    IReadOnlyList<ParentSourceMapReference> ParentMap { get; }  // 0 based lines, is 1:1 with the Content array
    IReadOnlyList<ChildSourceMapReference> ChildrenMap { get; }

    void MapChildren();
    void AddChild(ISourceFile parent);

    int AddParent(ISourceFile parent);
    void SetParentMap(int lineNumber, int parentLineNumber, int parentId);
}
