using System;

namespace BitMagic.Common;

public class AsmVariable : IAsmVariable
{
    public string Name { get; set; } = "";
    public int Value { get; set; }
    public bool Array { get; set; } = false;
    public VariableDataType VariableDataType { get; set; }
    public int Length { get; set; } = 0;
    public bool RequiresReval { get; set; } = false;
    public Func<bool, (int Value, bool RequiresReval)> Evaluate { get; set; } = (_) => (0, true);
    public SourceFilePosition? SourceFilePosition { get; set; } = null;
    /// <summary>
    /// Segment the variable is in, if known. Used to group large numbers of variables for display.
    /// </summary>
    public string? Segment { get; set; } = null;
    /// <summary>
    /// Whether the name can be reached from outside the scope it is declared in.
    /// Labels and operand labels are private whatever this says, see <see cref="IsLabel"/>.
    /// </summary>
    public Visibility Visibility { get; set; } = Visibility.Public;
    /// <summary>
    /// Set on a label written on an instruction's operand (`lda name: $1234`). These are typed
    /// Byte / Ushort like variables, so the type alone can't identify them.
    /// </summary>
    public bool OperandLabel { get; set; } = false;
    /// <summary>
    /// Keeps the variable out of the debugger's variable lists (Globals, Locals). Set on a `.constvar`,
    /// which is typed like a variable, so the type alone can't identify it.
    /// </summary>
    public bool HideInVariables { get; set; } = false;
    /// <summary>
    /// For an `.export` alias, the full name of the variable it finally resolves to.
    /// </summary>
    public string? AliasOf { get; set; } = null;
    /// <summary>
    /// Labels (`.name:`, `.:`) and operand labels: private outside their scope unless exported.
    /// An export of a label takes its type, but is a name in its own right, so isn't one.
    /// </summary>
    public bool IsLabel => AliasOf == null && (OperandLabel || VariableDataType == VariableDataType.LabelPointer);
    public VariableType VariableType => VariableType.CompileConstant;
    public void Move(int offset)
    {
        Value += offset;
    }
}

public class DebuggerVariable : IAsmVariable
{
    public string Name { get; set; } = "";
    public string Expression { get; set; } = "";

    public int Value => 0; // this isn't used

    public VariableDataType VariableDataType { get; set; }

    public int Length { get; set; }

    public bool Array { get; set; }

    public bool RequiresReval { get => false; set { } }

    public Func<bool, (int Value, bool RequiresReval)> Evaluate { get; set; } = (_) => (0, true);
    public VariableType VariableType => VariableType.DebuggerExpression;

    public void Move(int offset)
    {
        // no-op
    }
}
