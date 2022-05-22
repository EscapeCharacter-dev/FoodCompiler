using FOOD.Core.Diagnostics;
using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Structure;
using FOOD.Core.Syntax.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax;
public partial class Parser
{
    private IDeclaration? TryParseStructure(bool isPublic, string[] attributes)
    {
        if (Current.Type == TokenType.KeywordStruct
            || Current.Type == TokenType.KeywordRecord
            || Current.Type == TokenType.KeywordUnion)
        {
            var kind =
                Current.Type == TokenType.KeywordStruct ? StructureKind.Structure
                : Current.Type == TokenType.KeywordRecord ? StructureKind.Record : StructureKind.Union;
            _index++;
            if (Current.Type != TokenType.Identifier)
            {
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_expectedIdentifier"],
                    _lexer.GetPosition(Previous)
                    ));
                return null;
            }
            var ident = (string)Current.Value!;
            if (Head.IsSymbolDeclared(ident))
            {
                
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_symbolAlreadyDefined"],
                    _lexer.GetPosition(Previous)
                    ));
                return null;
            }
            _index++;
            if (Current.Type != TokenType.OpenCurlyBracket)
            {
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_missingOpenBracket"],
                    _lexer.GetPosition(Previous)
                    ));
                return null;
            }
            _index++;
            var members = new List<IDeclaration>();

            StartScope();
            // structure signature only
            _head += new StructureDeclaration(
                ident, new ParseType(0, TypeKind.Struct), Location.Static, isPublic, Array.Empty<IDeclaration>(), kind, Head, attributes);
            while (true)
            {
                if (Current.Type == TokenType.ClosedCurlyBracket)
                    break;

                var memberDecl = ParseDeclaration(true);
                if (memberDecl == null)
                {
                    if (Current.Type == TokenType.ClosedCurlyBracket)
                        break;
                    else
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_missingCommaOrClosingBracket"],
                            _lexer.GetPosition(Previous)
                            ));
                        _index++;
                        break;
                    }
                }
                if (Current.Type != TokenType.Semicolon)
                {
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingSemicolon"],
                        _lexer.GetPosition(Previous)
                        ));
                    break;
                }
                if (Head.IsSymbolDeclared(memberDecl.Name, false))
                {

                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_symbolAlreadyDefined"],
                        _lexer.GetPosition(Previous)
                        ));
                    return null;
                }
                _head += memberDecl;
                _index++;
                members.Add(memberDecl);
            }
            var past = Head;
            EndScope();
            _index++;
            var decl = new StructureDeclaration(
                ident, new ParseType(0, TypeKind.Struct, null, ident), Location.Static, isPublic, members.ToArray(), kind, past, attributes);
            _head += decl;
            return decl;
        }
        else if (Current.Type == TokenType.KeywordEnum)
        {
            _index++;
            if (Current.Type != TokenType.Identifier)
            {
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_expectedIdentifier"],
                    _lexer.GetPosition(Previous)
                    ));
                return null;
            }
            var ident = (string)Current.Value!;
            if (Head.IsSymbolDeclared(ident))
            {

                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_symbolAlreadyDefined"],
                    _lexer.GetPosition(Previous)
                    ));
                return null;
            }
            _index++;
            if (Current.Type != TokenType.OpenCurlyBracket)
            {
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_missingOpenBracket"],
                    _lexer.GetPosition(Previous)
                    ));
                return null;
            }
            _index++;
            var members = new List<(string Identifier, int Value)>();
            var enumIndex = 0;
            while (true)
            {
                if (Current.Type == TokenType.ClosedCurlyBracket)
                    break;

                if (Current.Type != TokenType.Identifier)
                {
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_expectedIdentifier"],
                        _lexer.GetPosition(Previous)
                        ));
                    _index++;
                    return null;
                }
                var name = (string)Current.Value!;
                _index++;
                if (Current.Type == TokenType.Equal)
                {
                    _index++;
                    if (Current.Type == TokenType.LiteralNumber)
                    {
                        var tok = Current;
                        _index++;
                        if (tok.Variant)
                        {
                            CompilationUnit.Report(new ReportedDiagnostic(
                                DiagnosticContext.Diagnostics["_enumRequiresLiteralInteger"],
                                _lexer.GetPosition(Previous)
                                ));
                            return null;
                        }
                        var val = (decimal)tok.Value!;
                        enumIndex = (int)val;
                    }
                    else
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_enumRequiresLiteralInteger"],
                            _lexer.GetPosition(Current)
                            ));
                        return null;
                    }
                }
                if (Current.Type == TokenType.ClosedCurlyBracket)
                    break;
                if (Current.Type != TokenType.Comma && Current.Type != TokenType.ClosedCurlyBracket)
                {
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingCommaOrClosingBracket"],
                        _lexer.GetPosition(Previous)
                        ));
                    _index++;
                    break;
                }
                _index++;
                members.Add((name, enumIndex++));
            }
            _index++;
            var decl =
                new EnumDeclaration(ident, new ParseType(0, TypeKind.Enum, null, ident),
                Location.Static, isPublic, members.ToArray(), attributes, Head);
            _head += decl;
            return decl;

        }
        return null;
    }
}
