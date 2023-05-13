﻿namespace BitMagic.Common;

public class AsmVariable : IAsmVariable
{
    public string Name { get; set; } = "";
    public int Value { get; set; }
    public bool Array { get; set; } = false;
    public VariableType VariableType { get; set; }
    public int Length { get; set; } = 0;
}
