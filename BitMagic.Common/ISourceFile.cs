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

//public interface ISourceFileOld
//{
//    string Name { get; }
//    string Path { get; }
//    int? ReferenceId { get; }
//    SourceFileType Origin { get; }
//    bool Volatile { get; }
//    Action Generate { get; }
//    bool ActualFile { get; }

//    public ISourceFile? Parent { get; }

//    string GetContent();
//}

public interface ISourceFile
{
    string Name { get; }
    string Path { get; }                    // Include name
    int? ReferenceId { get; set; }          // for VSCode
    SourceFileType Origin { get; }

    Task UpdateContent();                   // for things like dissasembly
    bool RequireUpdate { get; }

    bool ActualFile { get; }


    IReadOnlyList<ISourceFile> Parents { get; }
    IReadOnlyList<ISourceFile> Children { get; }

    IReadOnlyList<string> Content { get; }
    IReadOnlyList<ParentSourceMapReference> ParentMap { get; }  // 0 based lines, is 1:1 with the Content array
    IReadOnlyList<ChildSourceMapReference> ChildrenMap { get; }

    void MapChildren();
}
