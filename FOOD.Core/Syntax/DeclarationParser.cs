using FOOD.Core.Diagnostics;
using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Statements;
using FOOD.Core.Syntax.Structure;
using FOOD.Core.Syntax.Tree;
using FOOD.Core.Syntax.Type;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax;

public partial class Parser
{
    public IDeclaration? ParseDeclaration(bool simple = false)
    {
        if (!simple && TryParseDirective()) return null;
        var isPublic = false;
        if (Head.Parent == null && Current.Type == TokenType.KeywordPublic)
        {
            _index++;
            isPublic = true;
        }

        var structAttempt = TryParseStructure(isPublic);
        if (!simple && structAttempt != null) return structAttempt;

        var type = ParseType();
        IDeclaration decl;
        if (type.Kind == TypeKind.Error)
            return null;
        if (Current.Type != TokenType.Identifier)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_expectedIdentifier"],
                _lexer.GetPosition(Previous)
                ));
        var ident = Current;
        _index++;
        if (_head.IsSymbolDeclared((string)ident.Value!))
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_symbolAlreadyDefined"],
                _lexer.GetPosition(Previous)
                ));

        if (simple)
        {
            decl = new VariableDeclaration((string)ident.Value!, type, Location.Argument, isPublic, null);
            return decl;
        }
        if (Current.Type == TokenType.Semicolon)
        {
            _index++;
            decl = new VariableDeclaration((string)ident.Value!, type, Location.Local, isPublic, null);
            _head += decl;
            return decl;
        }
        if (Current.Type == TokenType.OpenBracket)
        {
            _index++;
            var parameters = new List<VariableDeclaration>();
            StartScope();
            while (true)
            {
                var param = ParseDeclaration(true);
                if (param == null)
                {
                    if (Current.Type == TokenType.ClosedBracket)
                        break;
                    else
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingCommaOrClosingBracket"],
                        _lexer.GetPosition(Previous)
                        ));
                        break;
                    }
                }
                _head += param;
                parameters.Add((VariableDeclaration)param);
                if (Current.Type == TokenType.ClosedBracket)
                    break;
                else if (Current.Type == TokenType.Comma)
                    _index++;
                else CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_missingCommaOrClosingBracket"],
                    _lexer.GetPosition(Previous)
                    ));
            }
            _index++;
            if (Current.Type == TokenType.Colon)
            {
                _index++;
                _head += new SimpleFunctionDeclaration(
                    (string)ident.Value!, type, Location.Static, isPublic, parameters.ToImmutableList(), null);
                var funcExpr = _binder.BindExpression(ParseExpression(), type);
                if (Current.Type != TokenType.Semicolon)
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingSemicolon"],
                        _lexer.GetPosition(Previous)
                        ));
                decl = new SimpleFunctionDeclaration(
                    (string)ident.Value!, type, Location.Static, isPublic, parameters.ToImmutableList(), funcExpr);
                _index++;
                EndScope();
                
                _head += decl;
                return decl;
            }
            else if (Current.Type == TokenType.OpenCurlyBracket)
            {
                _head += new ImperativeFunctionDeclaration(
                    (string)ident.Value!, type, Location.Static, isPublic, parameters.ToImmutableList(), null);
                var stat = ParseStatement();
                EndScope();
                decl = new ImperativeFunctionDeclaration(
                    (string)ident.Value!, type, Location.Static, isPublic, parameters.ToImmutableList(), stat);
                _head += decl;
                return decl;
            }
        }

        if (Current.Type != TokenType.Equal)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingSemicolonOrEqual"],
                _lexer.GetPosition(Previous)
                ));
        _index++;

        var expr = _binder.BindExpression(ParseExpression(), type);
        if (Current.Type != TokenType.Semicolon)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingClosingBracket"],
                _lexer.GetPosition(Previous)
                ));
        _index++;
        decl = new VariableDeclaration((string)ident.Value!, type, Location.Local, isPublic, expr);
        _head += decl;
        return decl;
    }
}
