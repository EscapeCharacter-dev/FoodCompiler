using FOOD.Core.Syntax.Type;

namespace FOOD.Core.Syntax.Structure;

/// <summary>
/// A structure declaration.
/// </summary>
public readonly struct StructureDeclaration : IDeclaration
{
    public string Name { get; }

    public ParseType Type { get; }

    public Location Location { get; }

    public bool IsPublic { get; }

    public readonly IDeclaration[] Members;
    public readonly StructureKind Kind;

    public StructureDeclaration(
        string name,
        ParseType type,
        Location location,
        bool isPublic,
        IDeclaration[] members,
        StructureKind kind)
    {
        Name = name;
        Type = type;
        Location = location;
        IsPublic = isPublic;
        Members = members;
        Kind = kind;
    }
}
