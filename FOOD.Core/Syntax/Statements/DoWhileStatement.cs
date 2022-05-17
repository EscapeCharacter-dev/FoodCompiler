using FOOD.Core.Syntax.Binding;

namespace FOOD.Core.Syntax.Statements;

/// <summary>
/// A do while statement.
/// </summary>
public sealed class DoWhileStatement : Statement
{
    public override IEnumerable<Statement> ChildrenEnumerator => new[] { IfTrue };

    /// <summary>
    /// While this expression yields true, <see cref="IfTrue"/> is executed.
    /// </summary>
    public readonly BoundTree Condition;

    /// <summary>
    /// Code executed while <see cref="Condition"/> yields true.
    /// </summary>
    public readonly Statement IfTrue;

    /// <summary>
    /// Initializes an instance of the class <see cref="DoWhileStatement"/>.
    /// </summary>
    /// <param name="condition">The condition of the while statement.</param>
    /// <param name="ifTrue">The code executed while the condition yields true.</param>
    public DoWhileStatement(BoundTree condition, Statement ifTrue)
    {
        Condition = condition;
        IfTrue = ifTrue;
    }
}
