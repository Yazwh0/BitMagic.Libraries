using System.Diagnostics;

namespace BitMagic.Common;

[DebuggerDisplay("[{relativeId}] {relativeLineNumber}")]
public sealed record ParentSourceMapReference(int relativeLineNumber, int relativeId); // -1 for no relative

[DebuggerDisplay("{contentLineNumber} -> [{relativeId}] {relativeLineNumber}")]
public sealed record ChildSourceMapReference(int contentLineNumber, int relativeLineNumber, int relativeId); // we only store actual references
