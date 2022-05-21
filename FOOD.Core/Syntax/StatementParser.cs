using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FOOD.Core.Diagnostics;
using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Statements;
using FOOD.Core.Syntax.Structure;
using FOOD.Core.Syntax.Type;

namespace FOOD.Core.Syntax;
public partial class Parser
{
    public Statement ParseStatement()
    {
        _index++;
        var stat = Previous.Type switch
        {
            TokenType.Semicolon => ParseEmptyStatement(),
            TokenType.KeywordBreak => ParseBreakStatement(),
            TokenType.KeywordContinue => ParseContinueStatement(),
            TokenType.KeywordReturn => ParseReturnStatement(),
            TokenType.KeywordIf => ParseIfStatement(),
            TokenType.KeywordWhile => ParseWhileStatement(),
            TokenType.KeywordDo => ParseDoStatement(),
            TokenType.KeywordFor => ParseForStatement(),
            TokenType.OpenCurlyBracket => ParseGroupStatement(),
            TokenType.KeywordSwitch => ParseSwitch(),
            _ => null
        };
        if (stat == null)
        {
            _index--;
            var decl = ParseDeclaration();
            if (decl != null)
                return new DeclarationStatement(decl);
            var nExpr = ParseExpression();
            var expr = _binder.BindExpression(nExpr);
            if (Current.Type != TokenType.Semicolon)
            {
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_missingSemicolon"],
                    _lexer.GetPosition(Previous)
                    ));
            }
            _index++;
            return new ExpressionStatement(expr);
        }
        else return stat;
    }

    private Statement ParseEmptyStatement() => new EmptyStatement();

    private Statement ParseBreakStatement() => new BreakStatement();

    private Statement ParseContinueStatement() => new ContinueStatement();

    private Statement ParseReturnStatement()
    {
        if (Head.Parent == null)
            throw new Exception("Statements should not be allowed at root of file");
        var func = Head.GetClosestFunction();// getting the function signature
        var type = func.Type;
        if (Current.Type == TokenType.Semicolon)
        {
            _index++;
            return new ReturnStatement(null);
        }
        var expr = _binder.BindExpression(ParseExpression(), type);
        if (!type.Kind.IsCompatibleWith(expr.BoundType.Kind))
        {
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(expr.CoreTree.Token)));
            return new ReturnStatement(null);
        }
        if (Current.Type != TokenType.Semicolon)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingSemicolon"],
                _lexer.GetPosition(Previous)
                ));
        _index++;
        return new ReturnStatement(expr);
    }

    private Statement ParseIfStatement()
    {
        if (Current.Type != TokenType.OpenBracket)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingOpeningBracket"],
                _lexer.GetPosition(Previous)
                ));
        _index++;
        var expr = _binder.BindExpression(ParseExpression());
        if (expr.BoundType.Kind == TypeKind.Half
            || expr.BoundType.Kind == TypeKind.Float
            || expr.BoundType.Kind == TypeKind.Double)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_intOrBoolExpression"],
                _lexer.GetPosition(Previous)
                ));
        if (Current.Type != TokenType.ClosedBracket)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingClosingBracket"],
                _lexer.GetPosition(Previous)
                ));
        _index++;
        var ifTrue = ParseStatement();
        if (Current.Type == TokenType.Identifier)
        {
            if (Current.Value as string == "else")
            {
                _index++;
                var ifFalse = ParseStatement();
                return new IfStatement(expr, ifTrue, ifFalse);
            }
        }
        return new IfStatement(expr, ifTrue);
    }

    private Statement ParseWhileStatement()
    {
        if (Current.Type != TokenType.OpenBracket)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingOpeningBracket"],
                _lexer.GetPosition(Previous)
                ));
        _index++;
        var expr = _binder.BindExpression(ParseExpression());
        if (expr.BoundType.Kind == TypeKind.Half
            || expr.BoundType.Kind == TypeKind.Float
            || expr.BoundType.Kind == TypeKind.Double)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_intOrBoolExpression"],
                _lexer.GetPosition(Previous)
                ));
        if (Current.Type != TokenType.ClosedBracket)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingClosingBracket"],
                _lexer.GetPosition(Previous)
                ));
        _index++;
        return new WhileStatement(expr, ParseStatement());
    }

    private Statement ParseDoStatement()
    {
        var stat = ParseStatement();
        if (Current.Type != TokenType.KeywordWhile)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingKeyword"],
                _lexer.GetPosition(Previous), "while"
                ));
        _index++;
        if (Current.Type != TokenType.OpenBracket)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingOpeningBracket"],
                _lexer.GetPosition(Previous)
                ));
        _index++;
        var expr = _binder.BindExpression(ParseExpression());
        if (expr.BoundType.Kind == TypeKind.Half
            || expr.BoundType.Kind == TypeKind.Float
            || expr.BoundType.Kind == TypeKind.Double)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_intOrBoolExpression"],
                _lexer.GetPosition(Previous)
                ));
        if (Current.Type != TokenType.ClosedBracket)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingClosingBracket"],
                _lexer.GetPosition(Previous)
                ));
        _index++;
        if (Current.Type != TokenType.Semicolon)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingSemicolon"],
                _lexer.GetPosition(Previous)
                ));
        return new DoWhileStatement(expr, stat);
    }

    private Statement ParseForStatement()
    {
        if (Current.Type != TokenType.OpenBracket)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingOpeningBracket"],
                _lexer.GetPosition(Previous)
                ));
        _index++;
        StartScope();

        var declOrExpr =
            Current.Type != TokenType.Semicolon ?
            ParseDeclaration() ??
            (object)_binder.BindExpression(ParseExpression())
            : null;
        if (declOrExpr is not IDeclaration && Current.Type != TokenType.Semicolon)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingSemicolon"],
                _lexer.GetPosition(Previous)
                ));
        if (declOrExpr is not IDeclaration)
            _index++;
        var condition =
            Current.Type != TokenType.Semicolon ?
            _binder.BindExpression(ParseExpression())
            : null;
        if (condition != null &&
            (condition.BoundType.Kind == TypeKind.Half ||
            condition.BoundType.Kind == TypeKind.Float ||
            condition.BoundType.Kind == TypeKind.Double))
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_intOrBoolExpression"],
                _lexer.GetPosition(Previous)
                ));
        if (Current.Type != TokenType.Semicolon)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingSemicolon"],
                _lexer.GetPosition(Previous)
                ));
        _index++;
        var update = Current.Type != TokenType.ClosedBracket
            ? _binder.BindExpression(ParseExpression())
            : null;
        if (Current.Type != TokenType.ClosedBracket)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingClosingBracket"],
                _lexer.GetPosition(Previous)
                ));
        _index++;
        var stat = ParseStatement();

        EndScope();
        return new ForStatement(declOrExpr, condition, update, stat);
    }

    private Statement ParseGroupStatement()
    {
        var gStatements = new List<Statement>();
        StartScope();
        while (Current.Type != TokenType.ClosedCurlyBracket)
        {
            if (Current.Type == TokenType.EndOfFile)
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_missingClosingBracket"],
                    _lexer.GetPosition(Previous)
                    ));
            gStatements.Add(ParseStatement());
        }
        EndScope();
        _index++;
        return new GroupStatement(gStatements);
    }
}
