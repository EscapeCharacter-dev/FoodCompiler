using FOOD.Core.Scoping;
using FOOD.Core.Syntax.Type;
using LLVMSharp;

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

    public string[] Attributes { get; }

    public EnumDeclaration(
        string name,
        ParseType type,
        Location location,
        bool isPublic,
        (string Identifier, int Index)[] members,
        string[] attributes,
        Scope memberScope)
    {
        Name = name;
        Type = type;
        Location = location;
        IsPublic = isPublic;
        Members = members;
        MemberScope = memberScope;
        Attributes = attributes;
    }

    /// <summary>
    /// Finds a member inside the enum.
    /// </summary>
    /// <param name="ident">The name of the member.</param>
    public (string Identifier, int Index)? FindMember(string ident)
    {
        foreach (var member in Members)
        {
            if (member.Identifier == ident)
                return member;
        }
        return null;
    }

    public LLVMValueRef ValueRef { get => default(LLVMValueRef); set => new Void(); }
}
