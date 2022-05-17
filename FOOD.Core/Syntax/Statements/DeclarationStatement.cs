using FOOD.Core.Syntax.Structure;

namespace FOOD.Core.Syntax.Statements;

public sealed class DeclarationStatement : Statement
{
    public override IEnumerable<Statement> ChildrenEnumerator => Enumerable.Empty<Statement>();

    public readonly IDeclaration Declaration;

    public DeclarationStatement(IDeclaration declaration)
    {
        Declaration = declaration;
    }
}
