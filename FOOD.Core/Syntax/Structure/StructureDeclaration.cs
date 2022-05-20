using FOOD.Core.Scoping;
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
    public readonly Scope MemberScope;

    public StructureDeclaration(
        string name,
        ParseType type,
        Location location,
        bool isPublic,
        IDeclaration[] members,
        StructureKind kind,
        Scope memberScope)
    {
        Name = name;
        Type = type;
        Location = location;
        IsPublic = isPublic;
        Members = members;
        Kind = kind;
        MemberScope = memberScope;
    }

    /// <summary>
    /// Finds a member inside the structure.
    /// </summary>
    /// <param name="ident">The name of the member.</param>
    public IDeclaration? FindMember(string ident)
    {
        foreach (var member in Members)
        {
            if (member.Name == ident)
                return member;
        }
        return null;
    }
}
