using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Type;
using System.Collections.Immutable;

namespace FOOD.Core.Syntax.Structure;

public readonly struct SimpleFunctionDeclaration : IFunctionDeclaration
{
    public readonly BoundTree? Body;

    /// <summary>
    /// Optional body makes this a signature-only function (will search higher scopes for code.)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="location"></param>
    /// <param name="isPublic"></param>
    /// <param name="parameters"></param>
    /// <param name="body"></param>
    public SimpleFunctionDeclaration(
        string name,
        ParseType type,
        Location location,
        bool isPublic,
        ImmutableList<VariableDeclaration> parameters,
        BoundTree? body)
    {
        Name = name;
        Type = type;
        Location = location;
        IsPublic = isPublic;
        Parameters = parameters;
        Body = body;
    }

    public ImmutableList<VariableDeclaration> Parameters { get; }

    public string Name { get; }

    public ParseType Type { get; }

    public Location Location { get; }

    public bool IsPublic { get; }
}
