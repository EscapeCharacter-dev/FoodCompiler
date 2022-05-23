using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Tree;
using FOOD.Core.Syntax.Type;
using LLVMSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Generators;
public class LLVMGenerator
{
    public LLVMGenerator()
    {
        Module = LLVM.ModuleCreateWithName("Sample");
        _builder = LLVM.CreateBuilder();
    }

    private readonly LLVMBuilderRef _builder;
    public readonly LLVMModuleRef Module;

    public LLVMValueRef GenerateExpression(BoundTree tree)
    {
        switch (tree.CoreTree.TreeType)
        {
            case TreeType.Literal:
                return tree.BoundType.Kind switch
                {
                    TypeKind.SByte or TypeKind.Byte => LLVM.ConstInt(LLVM.Int8Type(), (ulong)(decimal)tree.CoreTree.Token.Value!, false),
                    TypeKind.Short or TypeKind.UShort => LLVM.ConstInt(LLVM.Int16Type(), (ulong)(decimal)tree.CoreTree.Token.Value!, false),
                    TypeKind.Int or TypeKind.UInt => LLVM.ConstInt(LLVM.Int32Type(), (ulong)(decimal)tree.CoreTree.Token.Value!, false),
                    TypeKind.Long or TypeKind.ULong or TypeKind.Pointer => LLVM.ConstInt(LLVM.Int64Type(), (ulong)(decimal)tree.CoreTree.Token.Value!, false),
                    TypeKind.Half => LLVM.ConstReal(LLVM.HalfType(), (double)(decimal)tree.CoreTree.Token.Value!),
                    TypeKind.Float => LLVM.ConstReal(LLVM.FloatType(), (double)(decimal)tree.CoreTree.Token.Value!),
                    TypeKind.Double => LLVM.ConstReal(LLVM.DoubleType(), (double)(decimal)tree.CoreTree.Token.Value!),
                    _ => throw new NotImplementedException(),
                };
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
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildICmp(_builder, LLVMIntPredicate.LLVMIntULT, left, right, "cl");
                }
            case TreeType.LowerOrEqual:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildICmp(_builder, LLVMIntPredicate.LLVMIntULE, left, right, "cle");
                }
            case TreeType.Greater:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildICmp(_builder, LLVMIntPredicate.LLVMIntUGT, left, right, "cg");
                }
            case TreeType.GreaterOrEqual:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildICmp(_builder, LLVMIntPredicate.LLVMIntUGE, left, right, "cge");
                }
            case TreeType.Equal:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildICmp(_builder, LLVMIntPredicate.LLVMIntEQ, left, right, "ceq");
                }
            case TreeType.NotEqual:
                {
                    var left = GenerateExpression(tree.Children.ElementAt(0));
                    var right = GenerateExpression(tree.Children.ElementAt(1));
                    return LLVM.BuildICmp(_builder, LLVMIntPredicate.LLVMIntNE, left, right, "cneq");
                }
            case TreeType.UnaryPlus: return GenerateExpression(tree.Children.ElementAt(0));
            default: throw new NotImplementedException();
        }
    }
}
