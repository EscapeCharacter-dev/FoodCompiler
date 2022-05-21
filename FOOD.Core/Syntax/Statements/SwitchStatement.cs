using FOOD.Core.Syntax.Binding;

namespace FOOD.Core.Syntax.Statements;

public sealed class SwitchStatement : Statement
{
    public readonly SwitchRegion BaseRegion;
    public readonly BoundTree Value;

    public override IEnumerable<Statement> ChildrenEnumerator => BaseRegion.Statements;

    public SwitchStatement(SwitchRegion baseRegion, BoundTree value)
    {
        BaseRegion = baseRegion;
        Value = value;
    }
}
