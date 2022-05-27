using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Type;
using LLVMSharp;

namespace FOOD.Core.Syntax.Structure;

public struct VariableDeclaration : IDeclaration
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
        ValueRef = default;
    }

    public string Name { get; }

    public ParseType Type { get; }

    public Location Location { get; }

    public bool IsPublic { get; }

    public string[] Attributes { get; }

    public LLVMValueRef ValueRef { get; set; }
}

public struct ExternVariableDeclaration : IDeclaration
{
    public ExternVariableDeclaration(
        string name, ParseType type, Location location,
        bool isPublic, string[] attributes)
    {
        Name = name;
        Type = type;
        Location = location;
        IsPublic = isPublic;
        Attributes = attributes;
        ValueRef = default;
    }

    public string Name { get; }

    public ParseType Type { get; }

    public Location Location { get; }

    public bool IsPublic { get; }

    public string[] Attributes { get; }

    public LLVMValueRef ValueRef { get; set; }
}
