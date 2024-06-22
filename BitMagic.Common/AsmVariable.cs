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
    public VariableType VariableType => VariableType.CompileConstant;
}

public class DebuggerVariable : IAsmVariable
{
    public string Name { get; set; } = "";
    public string Expression { get; set; } = "";

    public int Value => 0; // this isn't used

    public VariableDataType VariableDataType { get; set; }

    public int Length { get; set; }

    public bool Array { get; set; }

    public bool RequiresReval => false;

    public Func<bool, (int Value, bool RequiresReval)> Evaluate { get; set; } = (_) => (0, true);
    public VariableType VariableType => VariableType.DebuggerExpression;
}
