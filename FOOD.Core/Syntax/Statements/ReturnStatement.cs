using FOOD.Core.Syntax.Binding;

namespace FOOD.Core.Syntax.Statements;

/// <summary>
/// A return statement.
/// </summary>
public sealed class ReturnStatement : Statement
{
    public override IEnumerable<Statement> ChildrenEnumerator => Enumerable.Empty<Statement>();

    /// <summary>
    /// The expression to return.
    /// </summary>
    public readonly BoundTree? Expression;

    /// <summary>
    /// Initializes an instance of the class <see cref="ReturnStatement"/>.
    /// </summary>
    /// <param name="expression">The expression.</param>
    public ReturnStatement(BoundTree? expression)
    {
        Expression = expression;
    }
}
