namespace FOOD.Core.Syntax.Statements;

/// <summary>
/// Represents a group of multiple statements.
/// </summary>
public sealed class GroupStatement : Statement
{
    public override IEnumerable<Statement> ChildrenEnumerator => Group;

    /// <summary>
    /// The group of statements.
    /// </summary>
    public readonly IEnumerable<Statement> Group;

    /// <summary>
    /// Initializes a class of type <see cref="GroupStatement"/>.
    /// </summary>
    /// <param name="group">The sub-statements.</param>
    public GroupStatement(IEnumerable<Statement> group)
    {
        Group = group;
    }
}