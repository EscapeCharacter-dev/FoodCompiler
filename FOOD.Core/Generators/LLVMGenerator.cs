using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Structure;
using FOOD.Core.Syntax.Tree;
using FOOD.Core.Syntax.Type;
using LLVMSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Generators;
public class LLVMGenerator : CompilationPart
{
    public LLVMGenerator(CompilationUnit unit) : base(unit)
    {
        Module = LLVM.ModuleCreateWithName("Sample");
        _builder = LLVM.CreateBuilder();
    }

    private readonly LLVMBuilderRef _builder;
    public readonly LLVMModuleRef Module;

    private LLVMTypeRef GetLLVMTypeFromTypeKind(ParseType type)
    {
        LLVMTypeRef? ret = type.Kind switch
        {
            TypeKind.Void => LLVM.VoidType(),
            TypeKind.Boolean => LLVM.IntType(1),
            TypeKind.Byte or TypeKind.SByte => LLVM.Int8Type(),
            TypeKind.Short or TypeKind.UShort => LLVM.Int16Type(),
            TypeKind.Int or TypeKind.UInt => LLVM.Int32Type(),
            TypeKind.Long or TypeKind.ULong => LLVM.Int64Type(),
            TypeKind.Pointer or TypeKind.Reference => LLVM.PointerType(GetLLVMTypeFromTypeKind(type.SubType!), 0),
            TypeKind.Half => LLVM.HalfType(),
            TypeKind.Float => LLVM.FloatType(),
            TypeKind.Double => LLVM.DoubleType(),
            _ => null,
        };
        if (ret == null)
        {
            if (type.Kind == TypeKind.Struct)
            {
                var elems = new List<LLVMTypeRef>();
                var decl = (StructureDeclaration)CompilationUnit.Parser.Root.GetDeclaration((string)type.Extra!)!;
                foreach (var member in decl.Members)
                    elems.Add(GetLLVMTypeFromTypeKind(member.Type));
                return LLVM.StructType(elems.ToArray(), decl.Attributes.Contains("packed"));
            }
        }
        return ret ?? throw new NotImplementedException();
    }

    public LLVMValueRef GenerateExpression(BoundTree tree)
    {
        switch (tree.CoreTree.TreeType)
        {
            case TreeType.Literal:
                {
                    var type = GetLLVMTypeFromTypeKind(tree.BoundType);
                    if (tree.BoundType.Kind.IsCompatibleWith(TypeKind.Int))
                        return LLVM.ConstInt(type, (ulong)(decimal)tree.CoreTree.Token.Value!, false);
                    else if (tree.BoundType.Kind.IsFloat())
                        return LLVM.ConstReal(type, (double)(decimal)tree.CoreTree.Token.Value!);
                    else if (tree.BoundType.Kind == TypeKind.Pointer)
                    {
                        var constant = LLVM.ConstInt(LLVM.Int64Type(), (ulong)(decimal)tree.CoreTree.Token.Value!, false);
                        return LLVM.BuildIntToPtr(_builder, constant, type, "literalptr");
                    }
                    else throw new NotImplementedException();
                }
            case TreeType.Addition:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildAdd(_builder, left, right, "add");
                }
            case TreeType.Subtraction:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildSub(_builder, left, right, "sub");
                }
            case TreeType.Multiply:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildMul(_builder, left, right, "mul");
                }
            case TreeType.Divide:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildSDiv(_builder, left, right, "div");
                }
            case TreeType.Modulo:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildSRem(_builder, left, right, "add");
                }
            case TreeType.BitwiseAnd:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildAnd(_builder, left, right, "and");
                }
            case TreeType.BitwiseOr:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildOr(_builder, left, right, "or");
                }
            case TreeType.BitwiseExclusiveOr:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildXor(_builder, left, right, "xor");
                }
            case TreeType.BitwiseLeftShift:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildShl(_builder, left, right, "lsh");
                }
            case TreeType.BitwiseRightShift:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildLShr(_builder, left, right, "rsh");
                }
            case TreeType.UnaryMinus:
                {
                    var child = GenerateExpression(tree.Children.ElementAt(0));
                    return LLVM.BuildNeg(_builder, child, "neg");
                }
            case TreeType.BitwiseNegation:
                {
                    var child = GenerateExpression(tree.Children.ElementAt(0));
                    return LLVM.BuildNeg(_builder, child, "bneg");
                }
            case TreeType.LogicalNegation:
                {
                    var child = GenerateExpression(tree.Children.ElementAt(0));
                    return LLVM.BuildICmp(_builder, LLVMIntPredicate.LLVMIntEQ, child, LLVM.ConstInt(LLVM.TypeOf(child), 0, false), "ln");
                }
            case TreeType.Lower:
            case TreeType.LowerOrEqual:
            case TreeType.Greater:
            case TreeType.GreaterOrEqual:
            case TreeType.Equal:
            case TreeType.NotEqual:
                {
                    int predicate = tree.BoundType.Kind.IsUnsigned() ? tree.CoreTree.TreeType switch
                    {
                        TreeType.Lower => (int)LLVMIntPredicate.LLVMIntULT,
                        TreeType.LowerOrEqual => (int)LLVMIntPredicate.LLVMIntULE,
                        TreeType.Equal => (int)LLVMIntPredicate.LLVMIntEQ,
                        TreeType.NotEqual => (int)LLVMIntPredicate.LLVMIntNE,
                        TreeType.GreaterOrEqual => (int)LLVMIntPredicate.LLVMIntUGE,
                        TreeType.Greater => (int)LLVMIntPredicate.LLVMIntUGT,
                        _ => throw new Exception("C# compiler bug")
                    } : tree.BoundType.Kind.IsFloat() ? tree.CoreTree.TreeType switch
                    {
                        TreeType.Lower => (int)LLVMRealPredicate.LLVMRealULT,
                        TreeType.LowerOrEqual => (int)LLVMRealPredicate.LLVMRealULE,
                        TreeType.Equal => (int)LLVMRealPredicate.LLVMRealUEQ,
                        TreeType.NotEqual => (int)LLVMRealPredicate.LLVMRealUNE,
                        TreeType.GreaterOrEqual => (int)LLVMRealPredicate.LLVMRealUGE,
                        TreeType.Greater => (int)LLVMRealPredicate.LLVMRealUGT,
                        _ => throw new Exception("C# compiler bug")
                    } : tree.CoreTree.TreeType switch
                    {
                        TreeType.Lower => (int)LLVMIntPredicate.LLVMIntSLT,
                        TreeType.LowerOrEqual => (int)LLVMIntPredicate.LLVMIntSLE,
                        TreeType.Equal => (int)LLVMIntPredicate.LLVMIntEQ,
                        TreeType.NotEqual => (int)LLVMIntPredicate.LLVMIntNE,
                        TreeType.GreaterOrEqual => (int)LLVMIntPredicate.LLVMIntSGE,
                        TreeType.Greater => (int)LLVMIntPredicate.LLVMIntSGT,
                        _ => throw new Exception("C# compiler bug")
                    };
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    if (tree.BoundType.Kind.IsFloat())
                        return LLVM.BuildFCmp(_builder, (LLVMRealPredicate)predicate, left, right, "cmp");
                    return LLVM.BuildICmp(_builder, (LLVMIntPredicate)predicate, left, right, "cmp");
                }
            case TreeType.LogicalAnd:
                {
                    var leftBase = GenerateExpression(tree.Children.ElementAt(0));
                    var rightBase = GenerateExpression(tree.Children.ElementAt(1));
                    var left = LLVM.BuildICmp(
                        _builder, LLVMIntPredicate.LLVMIntNE, leftBase,
                        LLVM.ConstInt(GetLLVMTypeFromTypeKind(tree.BoundType), 0, false), "landcmp");
                    var right = LLVM.BuildICmp(
                        _builder, LLVMIntPredicate.LLVMIntNE, rightBase,
                        LLVM.ConstInt(GetLLVMTypeFromTypeKind(tree.BoundType), 0, false), "landcmp");
                    return LLVM.BuildAnd(_builder, left, right, "land");
                }
            case TreeType.LogicalOr:
                {
                    var leftBase = GenerateExpression(tree.Children.ElementAt(0));
                    var rightBase = GenerateExpression(tree.Children.ElementAt(1));
                    var left = LLVM.BuildICmp(
                        _builder, LLVMIntPredicate.LLVMIntNE, leftBase,
                        LLVM.ConstInt(GetLLVMTypeFromTypeKind(tree.BoundType), 0, false), "lorcmp");
                    var right = LLVM.BuildICmp(
                        _builder, LLVMIntPredicate.LLVMIntNE, rightBase,
                        LLVM.ConstInt(GetLLVMTypeFromTypeKind(tree.BoundType), 0, false), "lorcmp");
                    return LLVM.BuildOr(_builder, left, right, "lor");
                }
            case TreeType.Cast:
                {
                    var childNoGen = tree.Children.ElementAt(0);
                    var child = GenerateExpression(childNoGen);
                    if (tree.BoundType.Kind == TypeKind.Void)
                        return child;
                    var to = GetLLVMTypeFromTypeKind(tree.BoundType);
                    switch (childNoGen.BoundType.Kind)
                    {
                        case TypeKind.Boolean:
                        case TypeKind.SByte:
                        case TypeKind.Byte:
                        case TypeKind.Short:
                        case TypeKind.UShort:
                        case TypeKind.Int:
                        case TypeKind.UInt:
                        case TypeKind.Long:
                        case TypeKind.ULong:
                            if (tree.BoundType.Kind.IsFloat())
                                return LLVM.BuildUIToFP(_builder, child, to, "cast");
                            else if (tree.BoundType.Kind == TypeKind.Pointer)
                                return LLVM.BuildIntToPtr(_builder, child, to, "cast");
                            else if (tree.BoundType.Kind.IsCompatibleWith(TypeKind.Int))
                                return LLVM.BuildIntCast(_builder, child, to, "cast");
                            else throw new NotImplementedException();
                        case TypeKind.Half:
                        case TypeKind.Float:
                        case TypeKind.Double:
                            if (tree.BoundType.Kind.IsCompatibleWith(TypeKind.Int))
                                return LLVM.BuildUIToFP(_builder, child, to, "cast");
                            else if (tree.BoundType.Kind.IsFloat())
                                return LLVM.BuildFPCast(_builder, child, to, "cast");
                            else throw new NotImplementedException();
                        case TypeKind.Pointer:
                            if (tree.BoundType.Kind == TypeKind.Pointer)
                                return LLVM.BuildPointerCast(_builder, child, to, "cast");
                            else if (tree.BoundType.Kind == TypeKind.Reference)
                                return LLVM.BuildPointerCast(_builder, child, to, "cast");
                            else if (tree.BoundType.Kind.IsCompatibleWith(TypeKind.Int))
                                return LLVM.BuildPtrToInt(_builder, child, to, "cast");
                            else throw new NotImplementedException();
                        default: throw new NotImplementedException();
                    }
                }
            case TreeType.UnaryPlus: return GenerateExpression(tree.Children.ElementAt(0));
            default: throw new NotImplementedException();
        }
    }
}
