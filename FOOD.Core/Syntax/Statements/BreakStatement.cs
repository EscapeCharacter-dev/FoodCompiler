namespace FOOD.Core.Syntax.Statements;

/// <summary>
/// Represents an break statement.
/// </summary>
public sealed class BreakStatement : Statement
{
    public override IEnumerable<Statement> ChildrenEnumerator => Enumerable.Empty<Statement>();
}
