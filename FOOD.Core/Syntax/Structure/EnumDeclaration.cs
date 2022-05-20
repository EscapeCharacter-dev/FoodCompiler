using FOOD.Core.Scoping;
using FOOD.Core.Syntax.Type;

namespace FOOD.Core.Syntax.Structure;

/// <summary>
/// An enum declaration.
/// </summary>
public readonly struct EnumDeclaration : IDeclaration
{
    public string Name { get; }

    public ParseType Type { get; }

    public Location Location { get; }

    public bool IsPublic { get; }

    public readonly (string Identifier, int Index)[] Members;
    public readonly Scope MemberScope;

    public EnumDeclaration(
        string name,
        ParseType type,
        Location location,
        bool isPublic,
        (string Identifier, int Index)[] members,
        Scope memberScope)
    {
        Name = name;
        Type = type;
        Location = location;
        IsPublic = isPublic;
        Members = members;
        MemberScope = memberScope;
    }
}
