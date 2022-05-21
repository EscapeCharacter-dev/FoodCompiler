namespace FOOD.Core.Syntax.Statements;

/// <summary>
/// Represents a continue statement.
/// </summary>
public sealed class ContinueStatement : Statement
{
    public override IEnumerable<Statement> ChildrenEnumerator => Enumerable.Empty<Statement>();
}
