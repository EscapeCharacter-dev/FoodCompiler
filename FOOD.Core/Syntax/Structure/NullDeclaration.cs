using FOOD.Core.Syntax.Type;
using LLVMSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Structure;
public struct NullDeclaration : IDeclaration
{
    public string Name => "";

    public ParseType Type => new ParseType(0, TypeKind.Error);

    public Location Location => Location.Static;

    public bool IsPublic => false;

    public string[] Attributes => Array.Empty<string>();

    public LLVMValueRef ValueRef { get; set; }

    public NullDeclaration() { ValueRef = new LLVMValueRef(); }
}
