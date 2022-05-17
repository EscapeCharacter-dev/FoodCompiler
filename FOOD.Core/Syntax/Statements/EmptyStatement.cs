namespace FOOD.Core.Syntax.Statements;

/// <summary>
/// Represents an empty statement (e.g.: -><c>;</c>)
/// </summary>
public sealed class EmptyStatement : Statement
{
    public override IEnumerable<Statement> ChildrenEnumerator => Enumerable.Empty<Statement>();
}
