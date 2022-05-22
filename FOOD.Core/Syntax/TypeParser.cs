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
    /// <summary>
    /// Parses a type.
    /// </summary>
    /// <returns>The parsed type.</returns>
    public ParseType ParseType()
    {
        byte qualifierField = 0b00000000;
        while (Current.Type == TokenType.KeywordConst
            || Current.Type == TokenType.KeywordVolatile
            || Current.Type == TokenType.KeywordRestrict
            || Current.Type == TokenType.KeywordAtomic)
        {
            qualifierField |= Current.Type switch
            {
                TokenType.KeywordConst => 0b00000001,
                TokenType.KeywordVolatile => 0b00000010,
                TokenType.KeywordRestrict => 0b00000100,
                TokenType.KeywordAtomic => 0b00001000,
                _ => 0b11111111,
            };
            if (qualifierField == 0b11111111)
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_invalidTypeQualifier"],
                    _lexer.GetPosition(Previous),
                    Current.Value!
                    ));
            _index++;
        }
        ParseType type;
        var kind = Current.Type switch
        {
            TokenType.KeywordVoid => TypeKind.Void,
            TokenType.KeywordBool => TypeKind.Boolean,
            TokenType.KeywordSByte => TypeKind.SByte,
            TokenType.KeywordByte => TypeKind.Byte,
            TokenType.KeywordShort => TypeKind.Short,
            TokenType.KeywordUShort => TypeKind.UShort,
            TokenType.KeywordHalf => TypeKind.Half,
            TokenType.KeywordInt => TypeKind.Int,
            TokenType.KeywordUInt => TypeKind.UInt,
            TokenType.KeywordFloat => TypeKind.Float,
            TokenType.KeywordLong => TypeKind.Long,
            TokenType.KeywordULong => TypeKind.ULong,
            TokenType.KeywordDouble => TypeKind.Double,
            TokenType.KeywordString => TypeKind.String,
            _ => TypeKind.Error
        };
        if (kind == TypeKind.Error && Current.Type == TokenType.KeywordFunction)
        {
            kind = TypeKind.Function;
            _index++;
            var subType = ParseType();
            if (Current.Type != TokenType.OpenBracket)
            {
                CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingClosingBracket"],
                        _lexer.GetPosition(Previous)
                        ));
                _index++;
                return new ParseType(qualifierField, TypeKind.Error);
            }
            _index++;
            var list = new List<ParseType>();
            while (true)
            {
                list.Add(ParseType());
                if (Current.Type != TokenType.Comma)
                {
                    if (Current.Type != TokenType.ClosedBracket)
                    {
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_missingClosingBracket"],
                            _lexer.GetPosition(Previous)
                            ));
                        _index++;
                        return new ParseType(qualifierField, TypeKind.Error);
                    }
                    else break;
                }
                _index++;
            }
            type = new ParseType(qualifierField, kind, subType, list.ToArray());
        }
        else if (kind == TypeKind.Error && Current.Type == TokenType.Identifier)
        {
            var ident = (string)Current.Value!;
            if (Head.IsSymbolDeclared(ident))
            {
                if (Head.GetDeclaration(ident)!.Type.Kind == TypeKind.Struct
                    && Head.GetDeclaration(ident)!.GetType() == typeof(StructureDeclaration))
                {
                    kind = TypeKind.Struct;
                    type = new ParseType(qualifierField, kind, null, ident);
                }
                else if (Head.GetDeclaration(ident)!.Type.Kind == TypeKind.Enum
                    && Head.GetDeclaration(ident)!.GetType() == typeof(EnumDeclaration))
                {
                    kind = TypeKind.Enum;
                    type = new ParseType(qualifierField, kind, null, ident);
                }
                else
                    type = new ParseType(qualifierField, kind);
            }
            else type = new ParseType(qualifierField, kind);
        }
        else type = new ParseType(qualifierField, kind);
        if (kind == TypeKind.Error)
            return type;
        _index++;
        while (Current.Type == TokenType.Star
            || Current.Type == TokenType.Ampersand
            || Current.Type == TokenType.OpenSquareBracket)
        {
            var tokenType = Current.Type;
            var subType = type;
            _index++;
            byte ptrQualifierField = 0b00000000;
            if (tokenType == TokenType.OpenSquareBracket)
            {
                kind = TypeKind.Array;
                var expr = _binder.BindExpression(P13());
                if (!expr.BoundType.Kind.IsCompatibleWith(TypeKind.Int))
                {
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_binderInvalidType"], _lexer.GetPosition(expr.CoreTree.Token)));
                    return new ParseType(qualifierField, TypeKind.Error);
                }
                if (Current.Type != TokenType.ClosedSquareBracket)
                {
                    CompilationUnit.Report(new ReportedDiagnostic(
                        DiagnosticContext.Diagnostics["_missingClosingBracket"],
                        _lexer.GetPosition(Previous)
                        ));
                    return new ParseType(qualifierField, TypeKind.Error);
                }
                _index++;
                type = new ParseType(ptrQualifierField, kind, subType);
            }
            else
            {
                while (Current.Type == TokenType.KeywordConst
                || Current.Type == TokenType.KeywordVolatile
                || Current.Type == TokenType.KeywordRestrict
                || Current.Type == TokenType.KeywordAtomic)
                {
                    ptrQualifierField |= Current.Type switch
                    {
                        TokenType.KeywordConst => 0b00000001,
                        TokenType.KeywordVolatile => 0b00000010,
                        TokenType.KeywordRestrict => 0b00000100,
                        TokenType.KeywordAtomic => 0b00001000,
                        _ => 0b11111111
                    };
                    if (ptrQualifierField == 0b11111111)
                        CompilationUnit.Report(new ReportedDiagnostic(
                            DiagnosticContext.Diagnostics["_invalidTypeQualifier"],
                            _lexer.GetPosition(Previous),
                            Current.Value!
                            ));
                    _index++;
                }

                type = new ParseType(
                    ptrQualifierField,
                    tokenType == TokenType.Star ? TypeKind.Pointer : TypeKind.Reference,
                    subType);
            }
        }
        return type;
    }
}
