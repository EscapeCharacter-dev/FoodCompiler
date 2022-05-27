using FOOD.Core.Scoping;
using FOOD.Core.Syntax.Statements;
using FOOD.Core.Syntax.Type;
using LLVMSharp;
using System.Collections.Immutable;

namespace FOOD.Core.Syntax.Structure;

public struct ImperativeFunctionDeclaration : IFunctionDeclaration
{
    public readonly Statement? Body;

    /// <summary>
    /// Optional body makes this a signature-only function (will search higher scopes for code.)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="location"></param>
    /// <param name="isPublic"></param>
    /// <param name="parameters"></param>
    /// <param name="body"></param>
    public ImperativeFunctionDeclaration(
        string name,
        ParseType type,
        Location location,
        bool isPublic,
        ImmutableList<VariableDeclaration> parameters,
        string[] attributes,
        Statement? body,
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
        Body = body;
        Scope = scope;
        ValueRef = default(LLVMValueRef);
    }
    public ImmutableList<VariableDeclaration> Parameters { get; }

    public string Name { get; }

    public ParseType Type { get; }

    public Location Location { get; }

    public bool IsPublic { get; }

    public bool Faillible { get; }

    public string[] Attributes { get; }

    public Scope Scope { get; }

    public LLVMValueRef ValueRef { get; set; }

    public override string ToString() => $"[{Type}]{Name}:\n" + Body;
}
