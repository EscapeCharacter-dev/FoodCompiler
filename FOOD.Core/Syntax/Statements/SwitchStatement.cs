using FOOD.Core.Syntax.Binding;

namespace FOOD.Core.Syntax.Statements;

public sealed class SwitchStatement : Statement
{
    public readonly (BoundTree Case, int StatementIndex)[] Cases;
    public readonly int DefaultPosition;
    public readonly Statement[] Statements;
    public readonly BoundTree Value;

    public override IEnumerable<Statement> ChildrenEnumerator => Statements;

    public SwitchStatement(
        (BoundTree Case, int StatementIndex)[] cases,
        int defaultPosition,
        Statement[] statements,
        BoundTree value)
    {
        Cases = cases;
        DefaultPosition = defaultPosition;
        Statements = statements;
        Value = value;
    }
}
