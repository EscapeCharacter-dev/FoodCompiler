using FOOD.Core.Scoping;
using FOOD.Core.Syntax.Type;
using System.Collections.Immutable;

namespace FOOD.Core.Syntax.Structure;

/// <summary>
/// A function declaration.
/// </summary>
public interface IFunctionDeclaration : IDeclaration
{
    /// <summary>
    /// The parameters of the function.
    /// </summary>
    public ImmutableList<VariableDeclaration> Parameters { get; }

    /// <summary>
    /// Whether the function can fail
    /// </summary>
    public bool Faillible { get; }

    /// <summary>
    /// The scope for the body of the function.
    /// </summary>
    public Scope Scope { get; }
}
