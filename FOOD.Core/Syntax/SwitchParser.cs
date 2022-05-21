using FOOD.Core.Diagnostics;
using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Statements;
using FOOD.Core.Syntax.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax;
public partial class Parser
{
    private SwitchStatement ParseSwitch()
    {
        _index++;
        if (Current.Type != TokenType.OpenBracket)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingOpeningBracket"],
                _lexer.GetPosition(Current)
                ));
        _index++;
        var expr = _binder.BindExpression(ParseExpression());
        if (expr.BoundType.Kind == TypeKind.Half
            || expr.BoundType.Kind == TypeKind.Float
            || expr.BoundType.Kind == TypeKind.Double)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_intOrBoolExpression"],
                _lexer.GetPosition(Current)
                ));
        if (Current.Type != TokenType.ClosedBracket)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingClosingBracket"],
                _lexer.GetPosition(Current)
                ));
        _index++;
        if (Current.Type != TokenType.OpenCurlyBracket)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingOpeningBracket"],
                _lexer.GetPosition(Current)
                ));
        _index++;
        var gStatements = new List<Statement>();
        var cases = new List<(BoundTree Case, int StatementIndex)>();
        var defaultPosition = 0;
        StartScope();
        while (Current.Type != TokenType.ClosedCurlyBracket)
        {
            if (Current.Type == TokenType.EndOfFile)
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_missingClosingBracket"],
                    _lexer.GetPosition(Previous)
                    ));

            if (Current.Type == TokenType.KeywordCase)
            {
                _index++;
                var caseExpression = _binder.BindExpression(ParseExpression(), expr.BoundType);
                if (Current.Type != TokenType.Colon)
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingColon"],
                        _lexer.GetPosition(Previous)
                        ));
                _index++;
                cases.Add((caseExpression, gStatements.Count + 1));
                continue;
            }

            if (Current.Type == TokenType.KeywordDefault)
            {
                _index++;
                if (Current.Type != TokenType.Colon)
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingColon"],
                        _lexer.GetPosition(Previous)
                        ));
                _index++;
                defaultPosition = gStatements.Count + 1;
                continue;
            }

            gStatements.Add(ParseStatement());
        }
        EndScope();
        _index++;
        return new SwitchStatement(cases.ToArray(), defaultPosition, gStatements.ToArray(), expr);
    }
}
