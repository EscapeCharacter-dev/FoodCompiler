﻿using FOOD.Core.Scoping;
using FOOD.Core.Syntax.Type;
using LLVMSharp;

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

    public string[] Attributes { get; }

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
        Scope memberScope,
        string[] attributes)
    {
        Name = name;
        Type = type;
        Location = location;
        IsPublic = isPublic;
        Members = members;
        Kind = kind;
        MemberScope = memberScope;
        Attributes = attributes;
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

    public LLVMValueRef ValueRef { get => default(LLVMValueRef); set => new Void(); }
}
