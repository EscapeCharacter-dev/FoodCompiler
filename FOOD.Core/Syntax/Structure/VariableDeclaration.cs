using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Type;

namespace FOOD.Core.Syntax.Structure;

public readonly struct VariableDeclaration : IDeclaration
{
    public readonly BoundTree? BoundTree;

    public VariableDeclaration(
        string name, ParseType type, Location location,
        bool isPublic, BoundTree? expr, string[] attributes)
    {
        BoundTree = expr;
        Name = name;
        Type = type;
        Location = location;
        IsPublic = isPublic;
        Attributes = attributes;
    }

    public string Name { get; }

    public ParseType Type { get; }

    public Location Location { get; }

    public bool IsPublic { get; }

    public string[] Attributes { get; }
}
