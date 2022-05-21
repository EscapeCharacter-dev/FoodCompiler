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
    private SwitchCase ParseCase(BoundTree? condition)
    {
        var statements = new List<Statement>();
        while (Current.Type != TokenType.ClosedCurlyBracket
            && Current.Type != TokenType.KeywordStart
            && Current.Type != TokenType.KeywordEnd
            && Current.Type != TokenType.KeywordCase
            && Current.Type != TokenType.KeywordDefault)
            statements.Add(ParseStatement());
        return new SwitchCase(statements.ToArray(), condition);
    }

    private SwitchRegion ParseSwitchRegion(ParseType expectedType)
    {
        var subRegions = new List<SwitchRegion>();
        var defaultFound = false;
        var cases = new List<SwitchCase>();
        while (Current.Type != TokenType.ClosedCurlyBracket
            && Current.Type != TokenType.KeywordEnd)
        {
            if (Current.Type == TokenType.KeywordCase)
            {
                _index++;
                var caseExpr = _binder.BindExpression(ParseExpression(), expectedType);
                if (Current.Type != TokenType.Colon)
                {
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingColon"],
                        _lexer.GetPosition(Current)
                        ));
                }
                _index++;
                cases.Add(ParseCase(caseExpr));
                continue;
            }
            else if (Current.Type == TokenType.KeywordDefault)
            {
                if (defaultFound)
                {
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_duplicateDefault"],
                        _lexer.GetPosition(Current)
                        ));
                }
                _index++;
                if (Current.Type != TokenType.Colon)
                {
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingColon"],
                        _lexer.GetPosition(Current)
                        ));
                }
                _index++;
                cases.Add(ParseCase(null));
                defaultFound = true;
                continue;
            }
            else if (Current.Type == TokenType.KeywordStart)
            {
                _index++;
                if (Current.Type != TokenType.Colon)
                {
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingColon"],
                        _lexer.GetPosition(Current)
                        ));
                }
                _index++;
                var startStatements = ParseCase(null).Statements;
                var subSwitch = ParseSwitchRegion(expectedType);
                if (Current.Type != TokenType.KeywordEnd)
                {
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingEndKeyword"],
                        _lexer.GetPosition(Current)
                        ));
                }
                _index++;
                if (Current.Type != TokenType.Colon)
                {
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingColon"],
                        _lexer.GetPosition(Current)
                        ));
                }
                _index++;
                var endStatements = ParseCase(null).Statements;
                subRegions.Add(subSwitch with { CommonStart = startStatements, CommonEnd = endStatements });
            }
        }
        return new SwitchRegion(subRegions.ToArray(), cases.ToArray(), Array.Empty<Statement>(), Array.Empty<Statement>());
    }

    private SwitchStatement ParseSwitch()
    {
        if (Current.Type != TokenType.OpenBracket)
            CompilationUnit.Report(new ReportedDiagnostic(
                DiagnosticContext.Diagnostics["_missingOpeningBracket"],
                _lexer.GetPosition(Current)
                ));
        _index++;
        var expr = _binder.BindExpression(ParseExpression());
        if (!expr.BoundType.Kind.IsCompatibleWith(TypeKind.Int))
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
        StartScope();
        var region = ParseSwitchRegion(expr.BoundType);
        EndScope();
        _index++;
        return new SwitchStatement(region, expr);
    }
}
