using FOOD.Core.Diagnostics;
using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax;
public partial class Parser
{
    private ParseTree P0()
    {
        if (Current.Type == TokenType.LiteralNumber ||
            Current.Type == TokenType.LiteralString)
        {
            var c = Current;
            _index++;
            return new StubTree(TreeType.Literal, c);
        }
        else if (Current.Type == TokenType.OpenBracket)
        {
            _index++;
            var tree = ParseExpression();
            if (Current.Type != TokenType.ClosedBracket)
            {
                _index++;
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_missingClosingBracket"],
                    _lexer.GetPosition(Previous)
                    ));
                return new StubTree(TreeType.Error, Current);
            }
            _index++;
            return tree;
        }
        else if (Current.Type == TokenType.KeywordTrue || Current.Type == TokenType.KeywordFalse)
        {
            var c = Current;
            _index++;
            if (Current.Type == TokenType.KeywordTrue)
                return new StubTree(TreeType.Literal, new Token(c.Position, TokenType.LiteralBoolean, true));
            else return new StubTree(TreeType.Literal, new Token(c.Position, TokenType.LiteralBoolean, false));
        }
        else if (Current.Type == TokenType.KeywordNull)
        {
            _index++;
            return new StubTree(TreeType.Literal,
                new Token(Previous.Position, TokenType.KeywordNull, null));
        }
        else if (Current.Type == TokenType.Identifier)
        {
            var identBase = Current;
            _index++;
            if (Current.Type == TokenType.Cube)
            {
                _index++;
                if (Current.Type != TokenType.Identifier)
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_expectedIdentifier"],
                        _lexer.GetPosition(Previous)
                        ));
                var identSecond = Current;
                _index++;
                if (Current.Type == TokenType.Cube)
                {
                    _index++;
                    if (Current.Type != TokenType.Identifier)
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_expectedIdentifier"],
                            _lexer.GetPosition(Previous)
                            ));
                    var identThird = Current;
                    _index++;
                    return new TernaryTree(
                        TreeType.NamespacedIdentifier, identBase,
                        new StubTree(TreeType.Identifier, identBase),
                        new StubTree(TreeType.Identifier, identSecond),
                        new StubTree(TreeType.Identifier, identThird));
                }
                return new BinaryTree(
                        TreeType.NamespacedIdentifier, identBase,
                        new StubTree(TreeType.Identifier, identBase),
                        new StubTree(TreeType.Identifier, identSecond));
            }
            return new StubTree(TreeType.Identifier, identBase);
        }
        else
        {
            _index++;
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_invalidExpressionGrammar"],
                _lexer.GetPosition(Previous)
                ));
            return new StubTree(TreeType.Error, Current);
        }
    }

    private ParseTree P1()
    {
        var left = P0();
        while (Current.Type.IsPartOf(
                TokenType.PlusPlus,
                TokenType.MinusMinus,
                TokenType.OpenSquareBracket,
                TokenType.OpenBracket))
        {
            switch (Current.Type)
            {
                case TokenType.PlusPlus: _index++; left = new UnaryTree(TreeType.PostfixIncrement, Current, left); break;
                case TokenType.MinusMinus: _index++; left = new UnaryTree(TreeType.PostfixDecrement, Current, left); break;
                case TokenType.OpenSquareBracket:
                    {
                        var tok = Current;
                        _index++;
                        var right = ParseExpression();
                        if (Current.Type != TokenType.ClosedSquareBracket)
                        {
                            _index++;
                            CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_missingClosingBracket"],
                                _lexer.GetPosition(Previous)
                                ));
                            return new StubTree(TreeType.Error, Current);
                        }
                        _index++;
                        left = new BinaryTree(TreeType.ArraySubscript, tok, left, right);
                        break;
                    }
                case TokenType.OpenBracket:
                    {
                        _index++;
                        var children = new List<ParseTree>(4);
                        children.Add(left);
                        while (Current.Type != TokenType.ClosedBracket)
                        {
                            children.Add(P15());
                            if (Current.Type != TokenType.Comma)
                            {
                                if (Current.Type != TokenType.ClosedBracket)
                                {
                                    _index++;
                                    CompilationUnit.Report(new ReportedDiagnostic(
                                        DiagnosticContext.Diagnostics["_missingClosingBracket"],
                                        _lexer.GetPosition(Previous)
                                    ));
                                    left = new StubTree(TreeType.Error, Current);
                                    return left;
                                }
                                break;
                            }
                            _index++;
                        }
                        _index++;
                        left = new ExtensibleTree(TreeType.FunctionCall, left.Token, children.ToArray());
                        break;
                    }
            }
        }
        return left;
    }

    private ParseTree PDots()
    {
        var left = P1();
        while (Current.Type == TokenType.Dot || Current.Type == TokenType.ThinArrow)
        {
            var op = Current;
            var kind = Current.Type == TokenType.Dot ? TreeType.MemberAccess : TreeType.PointerMemberAccess;
            _index++;
            var right = PDots();
            left = new BinaryTree(kind, op, left, right);
        }
        return left;
    }

    private ParseTree P2()
    {
        TreeType kind = TreeType.Error;
        var token = Current;
        switch (Current.Type)
        {
            case TokenType.PlusPlus: kind = TreeType.PrefixIncrement; break;
            case TokenType.MinusMinus: kind = TreeType.PrefixDecrement; break;
            case TokenType.Plus: kind = TreeType.UnaryPlus; break;
            case TokenType.Minus: kind = TreeType.UnaryMinus; break;
            case TokenType.Exclamation: kind = TreeType.LogicalNegation; break;
            case TokenType.Squiggle: kind = TreeType.BitwiseNegation; break;
            case TokenType.Star: kind = TreeType.Dereference; break;
            case TokenType.Ampersand: kind = TreeType.AddressOf; break;
            case TokenType.KeywordSizeof:
                {
                    _index++;
                    var type = ParseType();
                    kind = TreeType.Sizeof;
                    return new UnaryTree(TreeType.Sizeof, token, new TypeTree(TreeType.Type, token, type));
                }
            case TokenType.KeywordNew:
                {
                    _index++;
                    var type = ParseType();
                    kind = TreeType.New;
                    if (Current.Type != TokenType.OpenBracket)
                    {
                        _index++;
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_missing¸OpeningBracket"],
                            _lexer.GetPosition(Previous)
                            ));
                        return new StubTree(TreeType.Error, Current);
                    }
                    _index++;
                    var children = new List<ParseTree>(4);
                    while (Current.Type != TokenType.ClosedBracket)
                    {
                        children.Add(P15());
                        if (Current.Type != TokenType.Comma)
                        {
                            if (Current.Type != TokenType.ClosedBracket)
                            {
                                _index++;
                                CompilationUnit.Report(new ReportedDiagnostic(
                                    DiagnosticContext.Diagnostics["_missingClosingBracket"],
                                    _lexer.GetPosition(Previous)
                                ));
                                return new StubTree(TreeType.Error, Current);
                            }
                            break;
                        }
                        _index++;
                    }
                    _index++;
                    return new ExtensibleTree(kind, token,
                        new[] { new TypeTree(TreeType.Type, token, type) }.Union(children).ToArray());
                }
            case TokenType.OpenBracket:
                {
                    _index++;
                    var previous = _index - 1;
                    var type = ParseType();
                    if (type.Kind == Type.TypeKind.Error)
                    {
                        _index = previous;
                        break;
                    }
                    if (Current.Type != TokenType.ClosedBracket)
                    {
                        _index++;
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_missingClosingBracket"],
                            _lexer.GetPosition(Previous)
                            ));
                        return new StubTree(TreeType.Error, Current);
                    }
                    _index++;
                    kind = TreeType.Cast;
                    return new BinaryTree(kind, token, P2(), new TypeTree(TreeType.Type, token, type));
                }
        }
        if (kind == TreeType.Error)
            return PDots();
        _index++;
        var tree = P2();
        return new UnaryTree(kind, token, tree);
    }

    private ParseTree P3()
    {
        var left = P2();
        while (Current.Type == TokenType.Star || Current.Type == TokenType.Slash || Current.Type == TokenType.Percentage)
        {
            var op = Current;
            var kind =
                Current.Type == TokenType.Star ? TreeType.Multiply :
                Current.Type == TokenType.Slash ? TreeType.Divide : TreeType.Modulo;
            _index++;
            var right = P3();
            left = new BinaryTree(kind, op, left, right);
        }
        return left;
    }

    private ParseTree P4()
    {
        var left = P3();
        while (Current.Type == TokenType.Plus || Current.Type == TokenType.Minus)
        {
            var op = Current;
            var kind =
                Current.Type == TokenType.Plus ? TreeType.Addition : TreeType.Subtraction;
            _index++;
            var right = P4();
            left = new BinaryTree(kind, op, left, right);
        }
        return left;
    }

    private ParseTree P5()
    {
        var left = P4();
        while (Current.Type == TokenType.ArrowsLeft || Current.Type == TokenType.ArrowsRight)
        {
            var op = Current;
            var kind =
                Current.Type == TokenType.ArrowsLeft ? TreeType.BitwiseLeftShift : TreeType.BitwiseRightShift;
            _index++;
            var right = P5();
            left = new BinaryTree(kind, op, left, right);
        }
        return left;
    }

    private ParseTree P6()
    {
        var left = P5();
        while (Current.Type == TokenType.ArrowLeft || Current.Type == TokenType.ArrowRight
            || Current.Type == TokenType.ArrowLeftEqual || Current.Type == TokenType.ArrowRightEqual)
        {
            var op = Current;
            var kind =
                Current.Type == TokenType.ArrowLeft ? TreeType.Lower :
                Current.Type == TokenType.ArrowLeftEqual ? TreeType.LowerOrEqual :
                Current.Type == TokenType.ArrowRight ? TreeType.Greater : TreeType.GreaterOrEqual;
            _index++;
            var right = P6();
            left = new BinaryTree(kind, op, left, right);
        }
        return left;
    }

    private ParseTree P7()
    {
        var left = P6();
        while (Current.Type == TokenType.Equals || Current.Type == TokenType.ExclamationEqual)
        {
            var op = Current;
            var kind =
                Current.Type == TokenType.Equals ? TreeType.Equal : TreeType.NotEqual;
            _index++;
            var right = P7();
            left = new BinaryTree(kind, op, left, right);
        }
        return left;
    }

    private ParseTree P8()
    {
        var left = P7();
        while (Current.Type == TokenType.Ampersand)
        {
            var op = Current;
            _index++;
            var right = P8();
            left = new BinaryTree(TreeType.BitwiseAnd, op, left, right);
        }
        return left;
    }

    private ParseTree P9()
    {
        var left = P8();
        while (Current.Type == TokenType.Caret)
        {
            var op = Current;
            _index++;
            var right = P9();
            left = new BinaryTree(TreeType.BitwiseExclusiveOr, op, left, right);
        }
        return left;
    }

    private ParseTree P10()
    {
        var left = P9();
        while (Current.Type == TokenType.VerticalBar)
        {
            var op = Current;
            _index++;
            var right = P10();
            left = new BinaryTree(TreeType.BitwiseOr, op, left, right);
        }
        return left;
    }

    private ParseTree P11()
    {
        var left = P10();
        while (Current.Type == TokenType.Ampersands)
        {
            var op = Current;
            _index++;
            var right = P11();
            left = new BinaryTree(TreeType.LogicalAnd, op, left, right);
        }
        return left;
    }

    private ParseTree P12()
    {
        var left = P11();
        while (Current.Type == TokenType.VerticalBars)
        {
            var op = Current;
            _index++;
            var right = P12();
            left = new BinaryTree(TreeType.LogicalOr, op, left, right);
        }
        return left;
    }

    private ParseTree P13()
    {
        var left = P12();
        while (Current.Type == TokenType.ThickArrow)
        {
            var op = Current;
            _index++;
            var right = new TypeTree(TreeType.Type, op, ParseType());
            left = new BinaryTree(TreeType.Cast, op, left, right);
        }
        return left;
    }

    private ParseTree P14()
    {
        var left = P13();
        while (Current.Type == TokenType.Interrogation)
        {
            var op = Current;
            _index++;
            var mid = P14();
            if (Current.Type != TokenType.Colon)
            {
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_missingColonInCondExpr"],
                    _lexer.GetPosition(Previous)
                    ));
                return left;
            }
            _index++;
            var right = P14();
            left = new TernaryTree(TreeType.Conditional, op, left, mid, right);
        }
        return left;
    }

    private ParseTree P15()
    {
        var left = P14();
        while (Current.Type == TokenType.Equal || Current.Type == TokenType.PlusEqual
            || Current.Type == TokenType.MinusEqual || Current.Type == TokenType.StarEqual
            || Current.Type == TokenType.SlashEqual || Current.Type == TokenType.PercentageEqual
            || Current.Type == TokenType.ArrowsLeftEqual || Current.Type == TokenType.ArrowsRightEqual
            || Current.Type == TokenType.AmpersandEqual || Current.Type == TokenType.VerticalBarEqual
            || Current.Type == TokenType.CaretEqual)
        {
            var op = Current;
            var kind =
                Current.Type == TokenType.Equal ? TreeType.Assign :
                Current.Type == TokenType.PlusEqual ? TreeType.SumAssign :
                Current.Type == TokenType.MinusEqual ? TreeType.DifferenceAssign :
                Current.Type == TokenType.StarEqual ? TreeType.ProductAssign :
                Current.Type == TokenType.SlashEqual ? TreeType.QuotientAssign :
                Current.Type == TokenType.PercentageEqual ? TreeType.RemainderAssign :
                Current.Type == TokenType.ArrowsLeftEqual ? TreeType.BitwiseLeftAssign :
                Current.Type == TokenType.ArrowsRightEqual ? TreeType.BitwiseRightAssign :
                Current.Type == TokenType.AmpersandEqual ? TreeType.BitwiseAndAssign :
                Current.Type == TokenType.VerticalBarEqual ? TreeType.BitwiseOrAssign :
                TreeType.BitwiseExclusiveOrAssign;
            _index++;
            var right = P15();
            left = new BinaryTree(kind, op, left, right);
        }
        return left;
    }

    private ParseTree P16()
    {
        var left = P15();
        while (Current.Type == TokenType.Comma)
        {
            var op = Current;
            _index++;
            var right = P16();
            left = new BinaryTree(TreeType.Compound, op, left, right);
        }
        return left;
    }

    /// <summary>
    /// Parses an expression.
    /// </summary>
    /// <returns>The parsed expression.</returns>
    public ParseTree ParseExpression()
    {
        return P16();
    }
}
