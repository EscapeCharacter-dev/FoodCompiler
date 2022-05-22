using FOOD.Core.Syntax.Type;
using System.Collections.Immutable;

namespace FOOD.Core.Syntax.Structure;

public interface IFunctionDeclaration : IDeclaration
{
    public ImmutableList<VariableDeclaration> Parameters { get; }
}
