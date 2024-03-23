using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitMagic.Common;

public abstract class SourceFileBase : ISourceFile
{
    public string Name { get; protected set; } = "";

    public string Path { get; protected set; } = "";

    public int? ReferenceId { get; set; }

    public SourceFileType Origin { get; protected set; }

    public bool RequireUpdate { get; set; }

    public bool ActualFile { get; set; }

    public abstract bool X16File { get; }

    public abstract IReadOnlyList<string> Content { get; protected set; }


    public abstract IReadOnlyList<ISourceFile> Parents { get;  }
    public abstract IReadOnlyList<ParentSourceMapReference> ParentMap { get; }


    private readonly List<ISourceFile> _children = new();
    public IReadOnlyList<ISourceFile> Children => _children;


    private readonly List<ChildSourceMapReference> _childrenMap = new();
    public IReadOnlyList<ChildSourceMapReference> ChildrenMap => _childrenMap;

    public abstract Task UpdateContent();

    private bool _childrenMapped = false;

    /// <summary>
    /// Map all the parent -> child relationships.
    /// Can only be called once, so should be done after the whole chain is loaded.
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    public void MapChildren()
    {
        if (_childrenMapped) return;

        _childrenMapped = true;

        foreach (var p in Parents)
            p.MapChildren();

        for (var cidx = 0; cidx < Children.Count; cidx++)
        {
            var c = Children[cidx];
            var index = -1;
            for (var i = 0; i < c.Parents.Count; i++)
            {
                if (c.Parents[i].Path == Path)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                throw new System.Exception("Parent not found in child mapping!");
            }

            for (var i = 0; i < c.ParentMap.Count; i++)
            {
                if (c.ParentMap[i].relativeId != index)
                    continue;

                _childrenMap.Add(new ChildSourceMapReference(c.ParentMap[i].relativeLineNumber, i, cidx));
            }
        }

        foreach (var c in Children)
            c.MapChildren();
    }

    public void AddChild(ISourceFile child)
    {
        _children.Add(child);
    }

    public virtual int AddParent(ISourceFile parent)
    {
        throw new NotImplementedException();
    }

    public virtual void SetParentMap(int lineNumber, int parentLineNumber, int parentId)
    {
        throw new NotImplementedException();
    }
}
