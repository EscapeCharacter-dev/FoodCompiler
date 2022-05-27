using FOOD.Core.Diagnostics;
using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Structure;
using FOOD.Core.Syntax.Tree;
using FOOD.Core.Syntax.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Binding;

/// <summary>
/// Performs the binding operations.
/// </summary>
public sealed class Binder : CompilationPart
{
    private readonly Lexer _lexer;

    public Binder(CompilationUnit unit, Lexer lexer) : base(unit) { _lexer = lexer; }

    public BoundTree BindExpression(ParseTree tree, ParseType? expectedImplicitType = null)
    {
        switch (tree.TreeType)
        {
            case TreeType.Literal:
                switch (tree.Token.Type)
                {
                    case TokenType.LiteralBoolean:
                        {
                            var val = (bool)tree.Token.Value!;
                            if (expectedImplicitType != null)
                            {
                                if (expectedImplicitType.Kind == TypeKind.Half
                                    || expectedImplicitType.Kind == TypeKind.Float
                                    || expectedImplicitType.Kind == TypeKind.Double)
                                {
                                    CompilationUnit.Report(new ReportedDiagnostic(
                                        DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                                    return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                                }
                                return new BoundTree(
                                    new StubTree(TreeType.Literal,
                                    new Token(tree.Token.Position, TokenType.LiteralNumber, val ? (decimal)1 : 0)),
                                    expectedImplicitType, Enumerable.Empty<BoundTree>());
                            }
                            return new BoundTree(tree, new ParseType(0, TypeKind.Boolean), Enumerable.Empty<BoundTree>());
                        }
                    case TokenType.KeywordNull:
                        {
                            if (expectedImplicitType != null)
                            {
                                if (expectedImplicitType.Kind == TypeKind.Half
                                    || expectedImplicitType.Kind == TypeKind.Float
                                    || expectedImplicitType.Kind == TypeKind.Double)
                                {
                                    CompilationUnit.Report(new ReportedDiagnostic(
                                        DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                                    return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                                }
                                return new BoundTree(
                                    new StubTree(TreeType.Literal,
                                    new Token(tree.Token.Position, TokenType.LiteralNumber, (decimal)0)),
                                    expectedImplicitType, Enumerable.Empty<BoundTree>());
                            }
                            return new BoundTree(tree, new ParseType(0, TypeKind.Pointer, new ParseType(0, TypeKind.Void)),
                                Enumerable.Empty<BoundTree>());
                        }
                    case TokenType.LiteralNumber:
                        {
                            var val = (tree.Token.Value as decimal?)!;
                            if (tree.Token.Variant)
                            {
                                if (expectedImplicitType != null)
                                    return new BoundTree(tree, expectedImplicitType, Enumerable.Empty<BoundTree>());
                                double fVal = (double)val;
                                if (fVal <= float.MaxValue && fVal >= float.MinValue)
                                    return new BoundTree(tree, new ParseType(0, TypeKind.Float), Enumerable.Empty<BoundTree>());
                                else if (fVal <= double.MaxValue && fVal >= double.MinValue)
                                    return new BoundTree(tree, new ParseType(0, TypeKind.Double), Enumerable.Empty<BoundTree>());
                                else throw new NotImplementedException();
                            }
                            if (expectedImplicitType != null)
                                return new BoundTree(tree, expectedImplicitType, Enumerable.Empty<BoundTree>());
                            var iVal = (long)val;
                            if (iVal <= int.MaxValue && iVal >= int.MinValue)
                                return new BoundTree(tree, new ParseType(0, TypeKind.Int), Enumerable.Empty<BoundTree>());
                            else if (iVal <= long.MaxValue && iVal >= long.MinValue)
                                return new BoundTree(tree, new ParseType(0, TypeKind.Long), Enumerable.Empty<BoundTree>());
                            else if ((ulong)val <= ulong.MaxValue && (ulong)val >= ulong.MinValue)
                                return new BoundTree(tree, new ParseType(0, TypeKind.ULong), Enumerable.Empty<BoundTree>());
                            else throw new NotImplementedException();
                        }
                    case TokenType.LiteralString:
                        return new BoundTree(tree,
                            new ParseType(1, TypeKind.String, null, ((string)tree.Token.Value!).Length), Enumerable.Empty<BoundTree>());
                    default: throw new NotImplementedException();
                }
            case TreeType.Sizeof: return new BoundTree(tree, new ParseType(1, TypeKind.ULong), Enumerable.Empty<BoundTree>());
            case TreeType.Lengthof:
                {
                    var uTree = (UnaryTree)tree;
                    var child = BindExpression(uTree.Child);
                    if (child.BoundType.Kind != TypeKind.String)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    return new BoundTree(tree, new ParseType(1, TypeKind.UInt), new[] { child });
                }
            case TreeType.DefaultLiteral:
                {
                    var uTree = (UnaryTree)tree;
                    var type = ((TypeTree)uTree.Child).Type;
                    if (type.Kind == TypeKind.Error)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    else if (type.Kind == TypeKind.Reference)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    else if (type.Kind == TypeKind.Pointer || type.Kind == TypeKind.Function)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_WDefaultLiteralOfPointer"], _lexer.GetPosition(tree.Token)));
                    }
                    return new BoundTree(tree, type with { QualifierField = 0b00000001 }, Enumerable.Empty<BoundTree>());
                }
            case TreeType.New:
                {
                    var type = ((TypeTree)tree.ChildrenEnumerator.ElementAt(0)).Type;
                    var decl = (StructureDeclaration)CompilationUnit.Parser.Root.GetDeclaration((string)type.Extra!)!;

                    if (decl.Members.Length < tree.ChildrenEnumerator.Count() - 1)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_invalidFunctionSignature"], _lexer.GetPosition(tree.Token),
                                $"_ctor({(string)type.Extra!})"));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }

                    var bParamList = new List<BoundTree>();
                    for (var i = 1; i < tree.ChildrenEnumerator.Count(); i++)
                        bParamList.Add(
                            BindExpression(tree.ChildrenEnumerator.ElementAt(i), decl.Members.ElementAt(i - 1).Type));
                    return new BoundTree(tree, decl.Type, bParamList);
                }
            case TreeType.Cast:
                {
                    var binary = (BinaryTree)tree;
                    var left = binary.Left;
                    var right = (TypeTree)binary.Right;
                    return new BoundTree(binary, right.Type, new[] { BindExpression(left, right.Type) });
                }
            case TreeType.UnaryPlus:
            case TreeType.UnaryMinus:
            case TreeType.LogicalNegation:
            case TreeType.BitwiseNegation:
                {
                    var sub = BindExpression(((UnaryTree)tree).Child);
                    if (sub.BoundType.Kind == TypeKind.Half || sub.BoundType.Kind == TypeKind.Float || sub.BoundType.Kind == TypeKind.Double)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    return new BoundTree(tree, sub.BoundType, new[] { sub });
                }
            case TreeType.PostfixIncrement:
            case TreeType.PostfixDecrement:
            case TreeType.PrefixIncrement:
            case TreeType.PrefixDecrement:
                {
                    var sub = BindExpression(((UnaryTree)tree).Child);
                    if (sub.BoundType.Kind == TypeKind.Half || sub.BoundType.Kind == TypeKind.Float || sub.BoundType.Kind == TypeKind.Double)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    if (sub.BoundType.Kind == TypeKind.Reference
                        || (sub.BoundType.QualifierField & 0b00000001) != 0
                        || sub.CoreTree.TreeType != TreeType.Identifier
                        && sub.CoreTree.TreeType != TreeType.MemberAccess
                        && sub.CoreTree.TreeType != TreeType.PointerMemberAccess
                        && sub.CoreTree.TreeType != TreeType.ArraySubscript
                        && sub.CoreTree.TreeType != TreeType.Dereference)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_cannotModifyConstantValue"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    return new BoundTree(tree, sub.BoundType, new[] { sub });
                }
            case TreeType.Dereference:
                {
                    var sub = BindExpression(((UnaryTree)tree).Child);
                    if (sub.BoundType.Kind != TypeKind.Pointer
                        && sub.BoundType.Kind != TypeKind.Reference)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    return new BoundTree(tree, sub.BoundType.SubType!, new[] { sub });
                }
            case TreeType.AddressOf:
                {
                    var sub = BindExpression(((UnaryTree)tree).Child);
                    if (expectedImplicitType != null && expectedImplicitType.Kind == TypeKind.Reference)
                        return new BoundTree(tree, new ParseType(0, TypeKind.Reference, sub.BoundType), new[] { sub });
                    return new BoundTree(tree, new ParseType(0, TypeKind.Pointer, sub.BoundType), new[] { sub });
                }
            case TreeType.ArraySubscript:
                {
                    var binary = (BinaryTree)tree;
                    var left = BindExpression(binary.Left);
                    var right = BindExpression(binary.Right);
                    if (left.BoundType.Kind != TypeKind.Pointer
                        && left.BoundType.Kind != TypeKind.Array
                        && left.BoundType.Kind != TypeKind.String)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    if (!right.BoundType.Kind.IsCompatibleWith(TypeKind.Int) && right.BoundType.Kind != TypeKind.Range)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    if (right.BoundType.Kind == TypeKind.Range)
                    {
                        // TODO: Slices
                        if (left.BoundType.Kind == TypeKind.String)
                            return new BoundTree(binary,
                                new ParseType(0, TypeKind.Array, new ParseType(0, TypeKind.SByte)), new[] { left, right });
                        return new BoundTree(binary, new ParseType(0, TypeKind.Array, left.BoundType.SubType!), new[] { left, right });
                    }
                    if (left.BoundType.Kind == TypeKind.String)
                        return new BoundTree(binary, new ParseType(0, TypeKind.SByte), new[] { left, right });
                    return new BoundTree(binary, left.BoundType.SubType!, new[] { left, right });
                }
            case TreeType.Range:
                {
                    var binary = (BinaryTree)tree;
                    var left = BindExpression(binary.Left);
                    var right = BindExpression(binary.Right);
                    if (!left.BoundType.Kind.IsCompatibleWith(TypeKind.Int)
                        || !right.BoundType.Kind.IsCompatibleWith(TypeKind.UInt))
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    return new BoundTree(binary, new ParseType(0, TypeKind.Range), new[] { left, right });
                }
            case TreeType.Addition:
            case TreeType.Subtraction:
            case TreeType.Multiply:
            case TreeType.Divide:
            case TreeType.Modulo:
            case TreeType.Equal:
            case TreeType.NotEqual:
            case TreeType.Lower:
            case TreeType.LowerOrEqual:
            case TreeType.Greater:
            case TreeType.GreaterOrEqual:
                {
                    var binary = (BinaryTree)tree;
                    var left = BindExpression(binary.Left);
                    var right = BindExpression(binary.Right);
                    if (left.BoundType.Kind == TypeKind.Boolean || right.BoundType.Kind == TypeKind.Boolean)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    if (!left.BoundType.Kind.IsCompatibleWith(right.BoundType.Kind))
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    var boundType = left.BoundType.Kind < right.BoundType.Kind ? right.BoundType : left.BoundType;
                    return new BoundTree(binary, boundType, new[] { left, right });
                }
            case TreeType.BitwiseAnd:
            case TreeType.BitwiseOr:
            case TreeType.BitwiseExclusiveOr:
            case TreeType.BitwiseLeftShift:
            case TreeType.BitwiseRightShift:
            case TreeType.LogicalOr:
            case TreeType.LogicalAnd:
                {
                    var binary = (BinaryTree)tree;
                    var left = BindExpression(binary.Left);
                    var right = BindExpression(binary.Right);
                    var boundType = left.BoundType.Kind < right.BoundType.Kind ? right.BoundType : left.BoundType;
                    if (boundType.Kind == TypeKind.Float || boundType.Kind == TypeKind.Double || boundType.Kind == TypeKind.Half)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    return new BoundTree(binary, boundType, new[] { left, right });
                }
            case TreeType.Conditional:
                {
                    var ternary = (TernaryTree)tree;
                    var cond = BindExpression(ternary.Left);
                    var v0 = BindExpression(ternary.Middle);
                    var v1 = BindExpression(ternary.Right);
                    if (cond.BoundType.Kind == TypeKind.Half
                        || cond.BoundType.Kind == TypeKind.Float
                        || cond.BoundType.Kind == TypeKind.Double)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    var boundType = v0.BoundType.Kind < v1.BoundType.Kind ? v1.BoundType : v0.BoundType;
                    return new BoundTree(ternary, boundType, new[] { cond, v0, v1 });
                }
            case TreeType.Compound:
                {
                    var binary = (BinaryTree)tree;
                    var left = BindExpression(binary.Left);
                    var right = BindExpression(binary.Right);
                    return new BoundTree(binary, right.BoundType, new[] { left, right });
                }
            case TreeType.Identifier:
                {
                    var ident = (string)((StubTree)tree).Token.Value!;
                    if (expectedImplicitType != null
                        && expectedImplicitType.Kind == TypeKind.Function)
                    {
                        if (CompilationUnit.Parser.Head.IsSymbolDeclared(ident)
                            && expectedImplicitType.SubType!.Kind.IsCompatibleWith(CompilationUnit.Parser.Head.GetDeclaration(ident)!.Type.Kind))
                            return new BoundTree(tree, expectedImplicitType,
                                Enumerable.Empty<BoundTree>());
                        else CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_missingSymbol"], _lexer.GetPosition(tree.Token), ident));
                    }
                    if (CompilationUnit.Parser.Head.IsSymbolDeclared(ident))
                        return new BoundTree(tree, CompilationUnit.Parser.Head.GetDeclaration(ident)!.Type,
                            Enumerable.Empty<BoundTree>());
                    else CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingSymbol"], _lexer.GetPosition(tree.Token), ident));
                    return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                }
            case TreeType.NamespacedIdentifier:
                {
                    if (tree is TernaryTree threeIdentifier)
                    {
                        var identBase = (string)threeIdentifier.Left.Token.Value!;
                        var identSecond = (string)threeIdentifier.Middle.Token.Value!;
                        var identThird = (string)threeIdentifier.Right.Token.Value!;
                        
                        if (!CompilationUnit.Driver.Module.HasExtern(identBase))
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_missingModule"], _lexer.GetPosition(tree.Token), identBase));
                            return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                        }

                        var module = CompilationUnit.Driver.Module.GetExtern(identBase)!;

                        if (!module.HasNamespace(identSecond))
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_missingNamespace"], _lexer.GetPosition(tree.Token),
                                identBase, identSecond));
                            return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                        }

                        foreach (var moduleObject in module.GetObjectsInNamespace(identSecond))
                        {
                            if (moduleObject.Root.IsSymbolDeclared(identThird))
                                return new BoundTree(
                                    tree,
                                    moduleObject.Root.GetDeclaration(identThird)!.Type,
                                    Enumerable.Empty<BoundTree>());
                            
                        }
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_missingSymbol"], _lexer.GetPosition(tree.Token),
                            $"{identBase}::{identSecond}::{identThird}"));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
 
                    }
                    else if (tree is BinaryTree twoIdentifier)
                    {
                        var identBase = (string)twoIdentifier.Left.Token.Value!;
                        var identSecond = (string)twoIdentifier.Right.Token.Value!;

                        if (!CompilationUnit.Driver.Module.HasExtern(identBase))
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_missingModule"], _lexer.GetPosition(tree.Token), identBase));
                            return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                        }

                        var module = CompilationUnit.Driver.Module.GetExtern(identBase)!;

                        if (!module.HasNamespace("_root"))
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_missingNamespace"], _lexer.GetPosition(tree.Token),
                                identBase, "(no namespace)"));
                            return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                        }

                        foreach (var moduleObject in module.GetObjectsInNamespace("_root"))
                        {
                            if (moduleObject.Root.IsSymbolDeclared(identSecond))
                                return new BoundTree(
                                    tree,
                                    moduleObject.Root.GetDeclaration(identSecond)!.Type,
                                    Enumerable.Empty<BoundTree>());
                        }
                        CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingSymbol"], _lexer.GetPosition(tree.Token),
                        $"{identBase}::{identSecond}"));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    else throw new NotImplementedException();
                }
            case TreeType.FunctionCall:
                {
                    var eTree = (ExtensibleTree)tree;
                    var funcNameTree = BindExpression(eTree.ChildrenEnumerator.ElementAt(0));
                    if (funcNameTree.CoreTree.TreeType == TreeType.Identifier)
                    {
                        var funcName = (string)funcNameTree.CoreTree.Token.Value!;
                        if (!CompilationUnit.Parser.Head.IsSymbolDeclared(funcName))
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_missingSymbol"], _lexer.GetPosition(tree.Token), funcName));
                            return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                        }
                        var decl = CompilationUnit.Parser.Head.GetDeclaration(funcName);
                        if (decl is VariableDeclaration v)
                        {
                            var parameters = ((ParseType[])v.Type.Extra!);
                            if (decl.Type.Kind != TypeKind.Function)
                            {
                                CompilationUnit.Report(new ReportedDiagnostic(
                                    DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token), funcName));
                                return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                            }
                            if (parameters.Length != eTree.ChildrenEnumerator.Count() - 1)
                            {
                                CompilationUnit.Report(new ReportedDiagnostic(
                                    DiagnosticContext.Diagnostics["_invalidFunctionSignature"], _lexer.GetPosition(tree.Token), funcName));
                                return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                            }
                            var vParam = new List<BoundTree>();
                            for (var i = 1; i < eTree.ChildrenEnumerator.Count(); i++)
                                vParam.Add(
                                    BindExpression(eTree.ChildrenEnumerator.ElementAt(i),
                                    parameters.ElementAt(i - 1)));
                            return new BoundTree(tree, decl.Type.SubType!, vParam);
                        }
                        else if (decl is not IFunctionDeclaration)
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token), funcName));
                            return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                        }
                        var fDecl = (IFunctionDeclaration)decl!;
                        if (fDecl.Parameters.Count != eTree.ChildrenEnumerator.Count() - 1)
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_invalidFunctionSignature"], _lexer.GetPosition(tree.Token), funcName));
                            return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                        }
                        var bParamList = new List<BoundTree>();
                        for (var i = 1; i < eTree.ChildrenEnumerator.Count(); i++)
                            bParamList.Add(BindExpression(eTree.ChildrenEnumerator.ElementAt(i), fDecl.Parameters.ElementAt(i - 1).Type));
                        return new BoundTree(tree, fDecl.Type, bParamList);
                    }
                    else if (funcNameTree.CoreTree.TreeType == TreeType.NamespacedIdentifier)
                    {
                        if (funcNameTree.CoreTree is TernaryTree three)
                        {
                            var moduleName = (string)three.Left.Token.Value!;
                            var nameSpace = (string)three.Middle.Token.Value!;
                            var name = (string)three.Right.Token.Value!;

                            var module = CompilationUnit.Driver.Module.GetExtern(moduleName);
                            if (module == null)
                            {
                                CompilationUnit.Report(new ReportedDiagnostic(
                                    DiagnosticContext.Diagnostics["_missingModule"], _lexer.GetPosition(tree.Token), moduleName));
                                return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                            }
                            var objectsAtModuleRoot = module.GetObjectsInNamespace(nameSpace);
                            if (objectsAtModuleRoot.Count == 0)
                            {
                                CompilationUnit.Report(new ReportedDiagnostic(
                                    DiagnosticContext.Diagnostics["_missingNamespace"], _lexer.GetPosition(tree.Token),
                                    moduleName, nameSpace));
                                return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                            }
                            IFunctionDeclaration? decl = null;
                            foreach (var moduleObject in objectsAtModuleRoot)
                            {
                                if (moduleObject.Root.IsSymbolDeclared(name))
                                    decl = (IFunctionDeclaration)moduleObject.Root.GetDeclaration(name)!;
                            }
                            if (decl == null)
                            {
                                CompilationUnit.Report(new ReportedDiagnostic(
                                    DiagnosticContext.Diagnostics["_missingSymbol"], _lexer.GetPosition(tree.Token),
                                    $"{moduleName}::{nameSpace}::{name}"));
                                return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                            }
                            if (decl.Parameters.Count != eTree.ChildrenEnumerator.Count() - 1)
                            {
                                CompilationUnit.Report(new ReportedDiagnostic(
                                    DiagnosticContext.Diagnostics["_invalidFunctionSignature"], _lexer.GetPosition(tree.Token),
                                    $"{moduleName}::{nameSpace}::{name}"));
                                return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                            }
                            var bParamList = new List<BoundTree>();
                            for (var i = 1; i < eTree.ChildrenEnumerator.Count() + 1; i++)
                                bParamList.Add(
                                    BindExpression(eTree.ChildrenEnumerator.ElementAt(i), decl.Parameters.ElementAt(i - 1).Type));
                            return new BoundTree(tree, decl.Type, bParamList);
                        }
                        else
                        {
                            var two = (funcNameTree.CoreTree as BinaryTree)!;
                            var moduleName = (string)two.Left.Token.Value!;
                            var name = (string)two.Right.Token.Value!;

                            var module = CompilationUnit.Driver.Module.GetExtern(moduleName);
                            if (module == null)
                            {
                                CompilationUnit.Report(new ReportedDiagnostic(
                                    DiagnosticContext.Diagnostics["_missingModule"], _lexer.GetPosition(tree.Token), moduleName));
                                return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                            }
                            var objectsAtModuleRoot = module.GetObjectsInNamespace("_root");
                            if (objectsAtModuleRoot.Count == 0)
                            {
                                CompilationUnit.Report(new ReportedDiagnostic(
                                    DiagnosticContext.Diagnostics["_missingNamespace"], _lexer.GetPosition(tree.Token),
                                    moduleName, "(no namespace)"));
                                return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                            }
                            IFunctionDeclaration? decl = null;
                            foreach (var moduleObject in objectsAtModuleRoot)
                            {
                                if (moduleObject.Root.IsSymbolDeclared(name))
                                    decl = (IFunctionDeclaration)moduleObject.Root.GetDeclaration(name)!;
                            }
                            if (decl == null)
                            {
                                CompilationUnit.Report(new ReportedDiagnostic(
                                    DiagnosticContext.Diagnostics["_missingSymbol"], _lexer.GetPosition(tree.Token),
                                    $"{moduleName}::{name}"));
                                return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                            }
                            var bParamList = new List<BoundTree>();
                            for (var i = 1; i < eTree.ChildrenEnumerator.Count() + 1; i++)
                                bParamList.Add(
                                    BindExpression(eTree.ChildrenEnumerator.ElementAt(i), decl.Parameters.ElementAt(i - 1).Type));
                            return new BoundTree(tree, decl.Type, bParamList);
                        }
                    }
                    else throw new NotImplementedException();
                }
            case TreeType.ArrayLiteral:
                {
                    var extensibleTree = (ExtensibleTree)tree;
                    var subType = ((TypeTree)extensibleTree.ChildrenEnumerator.ElementAt(0)).Type;
                    var type = new ParseType(subType.QualifierField, TypeKind.Array, subType, extensibleTree.ChildrenEnumerator.Count() - 1);
                    return new BoundTree(tree, type, extensibleTree.ChildrenEnumerator.Skip(1).Each(x => BindExpression(x, expectedImplicitType)));
                }
            case TreeType.Assign:
            case TreeType.SumAssign:
            case TreeType.DifferenceAssign:
            case TreeType.ProductAssign:
            case TreeType.QuotientAssign:
            case TreeType.RemainderAssign:
                {
                    var binary = (BinaryTree)tree;
                    var left = BindExpression(binary.Left);
                    var right = BindExpression(binary.Right);

                    if (left.BoundType.Kind == TypeKind.Reference
                        || (left.BoundType.QualifierField & 0b00000001) != 0
                        || left.CoreTree.TreeType != TreeType.Identifier
                        && left.CoreTree.TreeType != TreeType.MemberAccess
                        && left.CoreTree.TreeType != TreeType.PointerMemberAccess
                        && left.CoreTree.TreeType != TreeType.ArraySubscript
                        && left.CoreTree.TreeType != TreeType.Dereference)
                    {
                        if (left.CoreTree.TreeType == TreeType.Identifier)
                        {
                            var decl = CompilationUnit.Parser.Head.GetDeclaration((string)left.CoreTree.Token.Value!);
                            if (decl != null && decl.Attributes.Contains("mutable_ref"))
                                goto mutableRef;
                        }
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_cannotModifyConstantValue"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                mutableRef: return new BoundTree(tree, left.BoundType, new[] { left, right });
                }
            case TreeType.BitwiseAndAssign:
            case TreeType.BitwiseExclusiveOrAssign:
            case TreeType.BitwiseOrAssign:
            case TreeType.BitwiseLeftAssign:
            case TreeType.BitwiseRightAssign:
                {
                    var binary = (BinaryTree)tree;
                    if (binary.Left.TreeType != TreeType.Identifier)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    var ident = (string)binary.Left.Token.Value!;
                    if (!CompilationUnit.Parser.Head.IsSymbolDeclared(ident))
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_missingSymbol"], _lexer.GetPosition(tree.Token), ident));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }

                    var decl = CompilationUnit.Parser.Head.GetDeclaration(ident);
                    if (decl!.Type.Kind == TypeKind.Half || decl.Type.Kind == TypeKind.Float || decl.Type.Kind == TypeKind.Double)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    var right = BindExpression(binary.Right, decl!.Type);

                    return new BoundTree(tree, decl!.Type, new[] { right });
                }
            case TreeType.MemberAccess:
                {
                    var binary = (BinaryTree)tree;
                    var leftNotBound = binary.Left;
                    var rightNotBound = binary.Right;
                    if (CompilationUnit.Parser.Head.IsSymbolDeclared((string)leftNotBound.Token.Value!))
                    {
                        var enumName = (string)leftNotBound.Token.Value!;
                        var rawDecl = CompilationUnit.Parser.Head.GetDeclaration(enumName)!;
                        if (rawDecl.Type.Kind != TypeKind.Enum)
                            goto notEnum;
                        if ((string)rawDecl.Type.Extra! != enumName)
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                    DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(leftNotBound.Token)));
                            return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                        }
                        if (rightNotBound.TreeType != TreeType.Identifier)
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                    DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(rightNotBound.Token)));
                            return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                        }
                        var memberName = (string)rightNotBound.Token.Value!;
                        var enumDecl = (EnumDeclaration)rawDecl;
                        var member = enumDecl.FindMember(memberName);
                        if (member != null)
                            return new BoundTree(tree, enumDecl.Type, Enumerable.Empty<BoundTree>());
                        else
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_missingSymbol"],
                                _lexer.GetPosition(rightNotBound.Token),
                                (string)rightNotBound.Token.Value!));
                            return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                        }
                    }
                notEnum:
                    var left = BindExpression(binary.Left);
                    if (rightNotBound.TreeType != TreeType.MemberAccess
                        && rightNotBound.TreeType != TreeType.PointerMemberAccess
                        && rightNotBound.TreeType != TreeType.Identifier)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(rightNotBound.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    if (left.BoundType.Kind != TypeKind.Struct)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(left.CoreTree.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    var structType = left.BoundType;
                    var structure = (StructureDeclaration)CompilationUnit.Parser.Head.GetDeclaration((string)structType.Extra!)!;
                    ParseType expectedType;
                    if (rightNotBound.TreeType == TreeType.Identifier)
                        expectedType = structure.FindMember((string)rightNotBound.Token.Value!)!.Type;
                    else
                    {
                        var rightNotBoundBinary = (BinaryTree)rightNotBound;
                        var member = structure.FindMember((string)rightNotBoundBinary.Left.Token.Value!);
                        if (member == null)
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_missingSymbol"],
                                _lexer.GetPosition(rightNotBoundBinary.Left.Token),
                                (string)rightNotBoundBinary.Left.Token.Value!));
                            return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                        }
                        expectedType = member.Type;
                    }
                    var previousHead = CompilationUnit.Parser.Head;
                    CompilationUnit.Parser.Head = structure.MemberScope;
                    var right = BindExpression(binary.Right, expectedType);
                    CompilationUnit.Parser.Head = previousHead;
                    return new BoundTree(tree, expectedType, new[] { left, right });
                }
            case TreeType.PointerMemberAccess:
                {
                    var binary = (BinaryTree)tree;
                    var left = BindExpression(binary.Left);
                    var rightNotBound = binary.Right;
                    if (rightNotBound.TreeType != TreeType.MemberAccess
                        && rightNotBound.TreeType != TreeType.PointerMemberAccess
                        && rightNotBound.TreeType != TreeType.Identifier)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(rightNotBound.Token)));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    if (left.BoundType.Kind != TypeKind.Pointer
                        && left.BoundType.SubType!.Kind == TypeKind.Struct)
                    {
                        if (left.BoundType.Kind != TypeKind.Reference
                            && left.BoundType.SubType!.Kind == TypeKind.Struct)
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                    DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(left.CoreTree.Token)));
                            return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                        }
                    }
                    var structType = left.BoundType.SubType!;
                    var structure = (StructureDeclaration)CompilationUnit.Parser.Head.GetDeclaration((string)structType.Extra!)!;
                    ParseType expectedType;
                    if (rightNotBound.TreeType == TreeType.Identifier
                        && structure.MemberScope.IsSymbolDeclared((string)rightNotBound.Token.Value!))
                        expectedType = structure.FindMember((string)rightNotBound.Token.Value!)!.Type;
                    else if (!structure.MemberScope.IsSymbolDeclared((string)rightNotBound.Token.Value!))
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                               DiagnosticContext.Diagnostics["_missingSymbol"],
                               _lexer.GetPosition(rightNotBound.Token),
                               (string)rightNotBound.Token.Value!));
                        return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                    }
                    else
                    {
                        var rightNotBoundBinary = (BinaryTree)rightNotBound;
                        var member = structure.FindMember((string)rightNotBoundBinary.Left.Token.Value!);
                        if (member == null)
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_missingSymbol"],
                                _lexer.GetPosition(rightNotBoundBinary.Left.Token),
                                (string)rightNotBoundBinary.Left.Token.Value!));
                            return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
                        }
                        expectedType = member.Type;
                    }
                    var previousHead = CompilationUnit.Parser.Head;
                    CompilationUnit.Parser.Head = structure.MemberScope;
                    var right = BindExpression(binary.Right, expectedType);
                    CompilationUnit.Parser.Head = previousHead;
                    return new BoundTree(tree, expectedType, new[] { left, right });
                }

            case TreeType.Error:
                return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
            default:
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(tree.Token)));
                return new BoundTree(tree, new ParseType(0, TypeKind.Error), Enumerable.Empty<BoundTree>());
        }
    }
}
