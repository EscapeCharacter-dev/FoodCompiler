using FOOD.Core.Diagnostics;
using FOOD.Core.Scoping;
using FOOD.Core.Syntax.Lex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax;

public partial class Parser
{
    public bool TryParseDirective()
    {
        if (Current.Type == TokenType.KeywordNamespace)
        {
            _index++;
            if (Current.Type != TokenType.Identifier)
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_expectedIdentifier"],
                    _lexer.GetPosition(Previous)
                    ));
            var section = (string)Current.Value!;
            if (_head.Parent != null)
                CompilationUnit.Report(new ReportedDiagnostic(
                     DiagnosticContext.Diagnostics["_mustBeInRootScope"],
                    _lexer.GetPosition(Previous)
                    ));
            CompilationUnit.ModuleObject.Namespace = section;
            _index++;
            if (Current.Type != TokenType.Semicolon)
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_missingSemicolon"],
                    _lexer.GetPosition(Previous)
                    ));
            _index++;
            return true;
        }
        else if (Current.Type == TokenType.KeywordUsing)
        {
            _index++;
            if (Current.Type != TokenType.Identifier)
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_expectedIdentifier"],
                    _lexer.GetPosition(Previous)
                    ));
            var moduleName = (string)Current.Value!;
            if (CompilationUnit.ModuleObject.Dependencies.Contains(moduleName))
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_WDuplicateUsingDirective"],
                    _lexer.GetPosition(Current),
                    moduleName
                    ));
            else CompilationUnit.ModuleObject.Dependencies.Add(moduleName);
            _index++;
            if (Current.Type != TokenType.Semicolon)
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_missingSemicolon"],
                    _lexer.GetPosition(Previous)
                    ));
            _index++;
            return true;
        }
        else if (Current.Type == TokenType.KeywordClass)
        {
            _index++;
            if (Current.Type != TokenType.Identifier)
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_expectedIdentifier"],
                    _lexer.GetPosition(Previous)
                    ));
            var className = (string)Current.Value!;
            _index++;
            if (Current.Type != TokenType.Equal)
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_missingEqual"],
                    _lexer.GetPosition(Previous)
                    ));
            _index++;
            var aliasFor = ParseType();
            if (Current.Type != TokenType.Semicolon)
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_missingSemicolon"],
                    _lexer.GetPosition(Previous)
                    ));
            _index++;
            return true;
        }
        return false;
    }
}
