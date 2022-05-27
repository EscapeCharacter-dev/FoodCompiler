using FOOD.Core.Scoping;
using FOOD.Core.Syntax.Type;
using LLVMSharp;
using System.Collections.Immutable;

namespace FOOD.Core.Syntax.Structure;

/// <summary>
/// Extern structure decl
/// </summary>
public struct ExternFunctionDeclaration : IFunctionDeclaration
{
    public ImmutableList<VariableDeclaration> Parameters { get; }

    public bool Faillible { get; }

    public Scope Scope { get; }

    public string Name { get; }

    public ParseType Type { get; }

    public Location Location { get; }

    public bool IsPublic { get; }

    public string[] Attributes { get; }

    public LLVMValueRef ValueRef { get; set; }

    public ExternFunctionDeclaration(
        string name,
        ParseType type,
        Location location,
        bool isPublic,
        ImmutableList<VariableDeclaration> parameters,
        string[] attributes,
        bool isFaillible,
        Scope scope)
    {
        Name = name;
        Type = type;
        Location = location;
        IsPublic = isPublic;
        Parameters = parameters;
        Attributes = attributes;
        Faillible = isFaillible;
        Scope = scope;
        ValueRef = new();
    }
}
