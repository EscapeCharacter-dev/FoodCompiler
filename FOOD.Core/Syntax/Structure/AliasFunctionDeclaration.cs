using FOOD.Core.Scoping;
using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Type;
using LLVMSharp;
using System.Collections.Immutable;

namespace FOOD.Core.Syntax.Structure;

/// <summary>
/// Alias function decl
/// </summary>
public struct AliasFunctionDeclaration : IFunctionDeclaration
{
    public string Name { get; }

    public ParseType Type { get; }

    public Location Location { get; }

    public bool IsPublic { get; }

    public string[] Attributes { get; }

    public LLVMValueRef ValueRef { get; set; }

    public ImmutableList<VariableDeclaration> Parameters { get; }

    public bool Faillible { get; }

    public Scope Scope => new Scope(null);

    public readonly BoundTree AliasTo;

    public AliasFunctionDeclaration(
        string name,
        ParseType type,
        Location location,
        bool isPublic,
        ImmutableList<VariableDeclaration> parameters,
        string[] attributes,
        BoundTree aliasTo,
        bool faillible)
    {
        Name = name;
        Type = type;
        Location = location;
        IsPublic = isPublic;
        Parameters = parameters;
        Attributes = attributes;
        AliasTo = aliasTo;
        Faillible = faillible;
        ValueRef = default;
    }
}
