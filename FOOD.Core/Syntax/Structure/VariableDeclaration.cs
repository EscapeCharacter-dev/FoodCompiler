using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Type;

namespace FOOD.Core.Syntax.Structure;

public readonly struct VariableDeclaration : IDeclaration
{
    public readonly BoundTree? BoundTree;

    public VariableDeclaration(string name, ParseType type, Location location, bool isPublic, BoundTree? expr)
    {
        BoundTree = expr;
        Name = name;
        Type = type;
        Location = location;
        IsPublic = isPublic;
    }

    public string Name { get; }

    public ParseType Type { get; }

    public Location Location { get; }

    public bool IsPublic { get; }
}
