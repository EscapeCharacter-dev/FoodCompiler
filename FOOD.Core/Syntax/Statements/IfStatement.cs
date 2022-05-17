using FOOD.Core.Syntax.Binding;

namespace FOOD.Core.Syntax.Statements;

/// <summary>
/// An if statement.
/// </summary>
public sealed class IfStatement : Statement
{
    public override IEnumerable<Statement> ChildrenEnumerator => new[] { IfTrue, IfFalse ?? new EmptyStatement() };

    /// <summary>
    /// If this expression yields true, then <see cref="IfTrue"/> is executed.
    /// If <see cref="IfFalse"/> is not null and the expression yields false, it is executed.
    /// </summary>
    public readonly BoundTree Condition;

    /// <summary>
    /// Code executed if <see cref="Condition"/> yields true.
    /// </summary>
    public readonly Statement IfTrue;

    /// <summary>
    /// Code executed if <see cref="Condition"/> yields false.
    /// </summary>
    public readonly Statement? IfFalse;

    /// <summary>
    /// Initializes an instance of the class <see cref="IfStatement"/>.
    /// </summary>
    /// <param name="condition">The condition of the if statement.</param>
    /// <param name="ifTrue">The code executed if the condition yields true.</param>
    /// <param name="ifFalse">The code executed if the condition yields false.</param>
    public IfStatement(BoundTree condition, Statement ifTrue, Statement? ifFalse = null)
    {
        Condition = condition;
        IfTrue = ifTrue;
        IfFalse = ifFalse;
    }
}
