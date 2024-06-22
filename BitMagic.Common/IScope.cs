namespace BitMagic.Common;

/// <summary>
/// Scoping interface, could be a proc, or higher up.
/// </summary>
public interface IScope
{
    IVariables Variables { get; }
    public string Name { get; }
    public IScope? Parent { get; }
    public bool Anonymous { get; }
}
