using FOOD.Core.Syntax.Binding;

namespace FOOD.Core.Syntax.Statements;

/// <summary>
/// A for statement.
/// </summary>
public sealed class ForStatement : Statement
{
    public override IEnumerable<Statement> ChildrenEnumerator => new[] { While };

    /// <summary>
    /// If this expression yields true, the body is executed.
    /// </summary>
    public readonly BoundTree? Condition;

    /// <summary>
    /// This code is looped over if <see cref="Condition"/> yields true.
    /// </summary>
    public readonly Statement While;

    /// <summary>
    /// Either a declaration, null or an expression, this sets a variable at the start of the for loop.
    /// </summary>
    public readonly object? Initialization;

    /// <summary>
    /// This code is executed just before the code is looped over and the condition check.
    /// </summary>
    public readonly BoundTree? Update;

    /// <summary>
    /// Initializes an instance of the class <see cref="ForStatement"/>.
    /// </summary>
    /// <param name="initialization">The initialization.</param>
    /// <param name="condition">The condition of the for statement.</param>
    /// <param name="update">The update of the for statement.</param>
    /// <param name="statWhile">The statement.</param>
    public ForStatement(object? initialization, BoundTree? condition, BoundTree? update, Statement statWhile)
    {
        Initialization = initialization;
        Condition = condition;
        Update = update;
        While = statWhile;
    }
}
