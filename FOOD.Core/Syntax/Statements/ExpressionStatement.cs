using FOOD.Core.Syntax.Binding;

namespace FOOD.Core.Syntax.Statements;

/// <summary>
/// Represents a statement that only contains an expression.
/// </summary>
public sealed class ExpressionStatement : Statement
{
    public override IEnumerable<Statement> ChildrenEnumerator => Enumerable.Empty<Statement>();

    /// <summary>
    /// The expression.
    /// </summary>
    public readonly BoundTree Expression;

    /// <summary>
    /// Initializes an instance of the class <see cref="ExpressionStatement"/>.
    /// </summary>
    /// <param name="expression">The expression.</param>
    public ExpressionStatement(BoundTree expression)
    {
        Expression = expression;
    }
}
