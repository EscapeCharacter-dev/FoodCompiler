using FOOD.Core.Scoping;

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
    /// The scope of the contained statements.
    /// </summary>
    public readonly Scope Scope;

    /// <summary>
    /// Initializes a class of type <see cref="GroupStatement"/>.
    /// </summary>
    /// <param name="group">The sub-statements.</param>
    /// <param name="scope">The scope that contains all the child statements.</param>
    public GroupStatement(IEnumerable<Statement> group, Scope scope)
    {
        Group = group;
        Scope = scope;
    }
}