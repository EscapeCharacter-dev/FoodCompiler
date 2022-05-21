using FOOD.Core.Syntax.Binding;

namespace FOOD.Core.Syntax.Statements;

public readonly struct SwitchCase
{
    public readonly Statement[] Statements;
    public readonly BoundTree? ConditionCase;

    public SwitchCase(Statement[] statements, BoundTree? conditionCase)
    {
        Statements = statements;
        ConditionCase = conditionCase;
    }
}
