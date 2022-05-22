using FOOD.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Lex;

/// <summary>
/// The lexer transforms text into tokens that can be processed by the parser.
/// </summary>
public class Lexer : CompilationPart
{
    static Lexer()
    {
        if (_keywordLookup == null)
            Initialize();
    }

    /// <summary>
    /// Initialies the lexer.
    /// </summary>
    public static void Initialize()
    {
        _keywordLookup = new Dictionary<string, TokenType>
        {
            { "atomic", TokenType.KeywordAtomic },
            { "break", TokenType.KeywordBreak },
            { "bool", TokenType.KeywordBool },
            { "byte", TokenType.KeywordByte },
            { "case", TokenType.KeywordCase },
            { "char", TokenType.KeywordSByte },
            { "const", TokenType.KeywordConst },
            { "continue", TokenType.KeywordContinue },
            { "default", TokenType.KeywordDefault },
            { "do", TokenType.KeywordDo },
            { "double", TokenType.KeywordDouble },
            { "else", TokenType.KeywordElse },
            { "enum", TokenType.KeywordEnum },
            { "extern", TokenType.KeywordExtern },
            { "float", TokenType.KeywordFloat },
            { "for", TokenType.KeywordFor },
            { "function", TokenType.KeywordFunction },
            { "goto", TokenType.KeywordGoto },
            { "half", TokenType.KeywordHalf },
            { "if", TokenType.KeywordIf },
            { "int", TokenType.KeywordInt },
            { "long", TokenType.KeywordLong },
            { "namespace", TokenType.KeywordNamespace },
            { "new", TokenType.KeywordNew },
            { "null", TokenType.KeywordNull },
            { "public", TokenType.KeywordPublic },
            { "record", TokenType.KeywordRecord },
            { "restrict", TokenType.KeywordRestrict },
            { "return", TokenType.KeywordReturn },
            { "sbyte", TokenType.KeywordSByte },
            { "short", TokenType.KeywordShort },
            { "size", TokenType.KeywordULong },
            { "sizeof", TokenType.KeywordSizeof },
            { "static", TokenType.KeywordStatic },
            { "struct", TokenType.KeywordStruct },
            { "switch", TokenType.KeywordSwitch },
            { "typedef", TokenType.KeywordTypedef },
            { "uchar", TokenType.KeywordByte },
            { "union", TokenType.KeywordUnion },
            { "uint", TokenType.KeywordUInt },
            { "ulong", TokenType.KeywordULong },
            { "ushort", TokenType.KeywordUShort },
            { "using", TokenType.KeywordUsing },
            { "void", TokenType.KeywordVoid },
            { "volatile", TokenType.KeywordVolatile },
            { "while", TokenType.KeywordWhile },
            { "i8", TokenType.KeywordSByte },
            { "u8", TokenType.KeywordByte },
            { "i16", TokenType.KeywordShort },
            { "u16", TokenType.KeywordUShort },
            { "f16", TokenType.KeywordHalf },
            { "i32", TokenType.KeywordInt },
            { "u32", TokenType.KeywordUInt },
            { "f32", TokenType.KeywordFloat },
            { "i64", TokenType.KeywordLong },
            { "u64", TokenType.KeywordULong },
            { "f64", TokenType.KeywordDouble },
            { "true", TokenType.KeywordTrue },
            { "false", TokenType.KeywordFalse },
            { "start", TokenType.KeywordStart },
            { "end", TokenType.KeywordEnd }
        };
    }

    /// <summary>
    /// The input text.
    /// </summary>
    private readonly string _source;

    /// <summary>
    /// The position.
    /// </summary>
    private int _index = 0;

    /// <summary>
    /// Current character.
    /// </summary>
    private char Current => _source[_index];

    /// <summary>
    /// Builds a new lexer with a given source.
    /// </summary>
    /// <param name="source">The text source.</param>
    public Lexer(CompilationUnit unit, string source) : base(unit)
    {
        _source = source;
    }

    /// <summary>
    /// Gets the current position in the source code.
    /// </summary>
    /// <returns></returns>
    public unsafe Position GetPosition(Token tok)
    {
        var line = 0;
        var col = 0;

        fixed (char* p = _source)
        {
            var begin = p;
            var end = p + tok.Position;
            for (; begin <= end; begin++)
            {
                if (*begin == '\n')
                {
                    line++;
                    col = 0;
                }
                else col++;
            }
        }
        return new Position(col, line);
    }

    /// <summary>
    /// Skips spaces in string.
    /// </summary>
    private unsafe void SkipSpaces()
    {
        fixed (char* ptr = _source)
        {
            var ptrWithIndex = ptr + _index;
            while (
                *ptrWithIndex == ' ' || *ptrWithIndex == '\t' || *ptrWithIndex == '\n'
                || *ptrWithIndex == '\r' || *ptrWithIndex == '\v')
                ptrWithIndex++;
            _index = (int)(ptrWithIndex - ptr);
        }
    }

    /// <summary>
    /// Checks for operators <c>C</c> and <c>C=</c>.
    /// </summary>
    /// <param name="once">The result if <c>C</c>.</param>
    /// <param name="equal">The result if <c>C=</c>.</param>
    /// <returns>wEqual if equal present, noEqual if not.</returns>
    private TokenType Operator2RepeatNotAllowed(TokenType once, TokenType equal)
    {
        _index++;
        if (Current == '=')
            return equal;
        _index--;
        return once;
    }

    /// <summary>
    /// Checks for operators <c>C</c>, <c>CC</c>, <c>C=</c> and <c>C&gt;</c>.
    /// </summary>
    /// <param name="once">The result if <c>C</c>.</param>
    /// <param name="equal">The result if <c>C=</c>.</param>
    /// <param name="arrow">The result if <c>C&gt;</c>.</param>
    /// <param name="twice">The result if <c>CC</c>.</param>
    /// <returns></returns>
    private TokenType Operator2Arrow(TokenType once, TokenType equal, TokenType arrow, TokenType twice)
    {
        var c = Current;
        _index++;
        if (Current == c)
            return twice;
        if (Current == '=')
            return equal;
        if (Current == '>')
            return arrow;
        _index--;
        return once;
    }

    /// <summary>
    /// Checks for operators <c>C</c>, <c>CC</c> and <c>C=</c>.
    /// </summary>
    /// <param name="once">The result if <c>C</c>.</param>
    /// <param name="equal">The result if <c>CC</c>.</param>
    /// <param name="twice">The result if <c>C=</c>.</param>
    /// <returns></returns>
    private TokenType Operator2(TokenType once, TokenType equal, TokenType twice)
    {
        var c = Current;
        _index++;
        if (Current == c)
            return twice;
        if (Current == '=')
            return equal;
        _index--;
        return once;
    }

    /// <summary>
    /// Checks for operators <c>C</c> and <c>CC</c>.
    /// </summary>
    /// <param name="single">The result if <c>C</c>.</param>
    /// <param name="twice">The result if <c>CC</c>.</param>
    /// <returns></returns>
    private TokenType Operator2OnlyRepeat(TokenType single, TokenType twice)
    {
        var c = Current;
        _index++;
        if (Current == c)
            return twice;
        _index--;
        return single;
    }

    /// <summary>
    /// Checks for operators <c>C</c>, <c>CC</c> and <c>CCC</c>.
    /// </summary>
    /// <param name="single">The result if <c>C</c>.</param>
    /// <param name="twice">The result if <c>CC</c>.</param>
    /// <param name="third">The result if <c>CCC</c>.</param>
    /// <returns></returns>
    private TokenType Operator3OnlyRepeat(TokenType single, TokenType twice, TokenType third)
    {
        var c = Current;
        _index++;
        if (Current == c)
        {
            _index++;
            if (Current == c)
                return third;
            _index--;
            return twice;
        }
        _index--;
        return single;
    }

    /// <summary>
    /// Checks for operators <c>C</c>, <c>CC</c>, <c>C=</c> and <c>CC=</c>.
    /// </summary>
    /// <param name="once">The result if <c>C</c>.</param>
    /// <param name="equal">The result if <c>C=</c>.</param>
    /// <param name="twice">The result if <c>CC</c>.</param>
    /// <param name="twiceEqual">The result if <c>CC=</c>.</param>
    /// <returns></returns>
    private TokenType OperatorAngleBrackets(TokenType once, TokenType equal, TokenType twice, TokenType twiceEqual)
    {
        var c = Current;
        _index++;
        if (Current == c)
        {
            _index++;
            if (Current == '=')
                return twiceEqual;
            _index--;
            return twice;
        }
        if (Current == '=')
            return equal;
        _index--;
        return once;
    }

    private static Dictionary<string, TokenType> _keywordLookup;

    /// <summary>
    /// Fetches a new token.
    /// </summary>
    /// <returns>A built token. Might contain an error.</returns>
    public Token Fetch()
    {
        SkipSpaces(); // skipping spaces

        if (_index >= _source.Length)
            return new Token(_index, TokenType.EndOfFile, null);

        if (char.IsLetter(Current) || Current == '_')
        {
            var currentIndex = _index;
            while (char.IsLetterOrDigit(Current) || Current.Equals('_')) _index++;
            var s = _source[currentIndex.._index];
            var kind = _keywordLookup.GetValueOrDefault(s);
            if (s.StartsWith('_'))
            {
                if (s.Length >= 2 && char.IsUpper(s[1]))
                    kind = TokenType.LanguageAttribute;
                else if (s.StartsWith("__") && s.Length >= 3 && char.IsUpper(s[2]))
                    kind = TokenType.CompilerAttribute;
            }
            return new Token(_index, kind, s);
        }

        if (char.IsDigit(Current))
        {
            var currentIndex = _index;
            while (char.IsDigit(Current) || Current == '.') _index++;
            var subs = _source[currentIndex.._index];
            if (!decimal.TryParse(subs, NumberStyles.Number, CultureInfo.InvariantCulture, out var dResult))
            {
                var tok = new Token(_index, TokenType.Error, null);
                CompilationUnit.Report(new ReportedDiagnostic(
                    DiagnosticContext.Diagnostics["_lexInvalidNumber"],
                    GetPosition(tok), subs));
                return tok;
            }
            if (long.TryParse(subs, NumberStyles.Number, CultureInfo.InvariantCulture, out var lResult))
                return new Token(_index, TokenType.LiteralNumber, (decimal)lResult);
            if (ulong.TryParse(subs, NumberStyles.Number, CultureInfo.InvariantCulture, out var ulResult))
                return new Token(_index, TokenType.LiteralNumber, (decimal)ulResult);
            return new Token(_index, TokenType.LiteralNumber, dResult, true);
        }

        if (Current == '\'')
        {
            _index++;
            var result = 0L;
            while (Current != '\'')
            {
                result <<= 8;
                result |= Current;
                _index++;
            }
            _index++;
            return new Token(_index, TokenType.LiteralNumber, (decimal)result);
        }

        if (Current == '"')
        {
            _index++;
            var stringBase = _index;
            while (Current != '"')
            {
                _index++;
                if (_index >= _source.Length)
                {
                    var retTok = new Token(_index, TokenType.Error, null);
                    CompilationUnit.Report(
                        new ReportedDiagnostic(DiagnosticContext.Diagnostics["__lexInvalidNumber"], GetPosition(retTok))
                        );
                    return retTok;
                }
            }
            _index++;
            return new Token(_index, TokenType.LiteralString, _source[stringBase..(_index - 1)]);
        }

        TokenType type;
        switch (Current)
        {
            case '(': type = TokenType.OpenBracket; break;
            case ')': type = TokenType.ClosedBracket; break;
            case '[': type = TokenType.OpenSquareBracket; break;
            case ']': type = TokenType.ClosedSquareBracket; break;
            case '{': type = TokenType.OpenCurlyBracket; break;
            case '}': type = TokenType.ClosedCurlyBracket; break;
            case '?': type = TokenType.Interrogation; break;
            case ';': type = TokenType.Semicolon; break;
            case ',': type = TokenType.Comma; break;
            case ':': type = Operator2OnlyRepeat(TokenType.Colon, TokenType.Cube); break;
            case '+': type = Operator2(TokenType.Plus, TokenType.PlusEqual, TokenType.PlusPlus); break;
            case '&': type = Operator2(TokenType.Ampersand, TokenType.AmpersandEqual, TokenType.Ampersands); break;
            case '|': type = Operator2(TokenType.VerticalBar, TokenType.VerticalBarEqual, TokenType.VerticalBars); break;
            case '-': type = Operator2Arrow(TokenType.Minus, TokenType.MinusEqual, TokenType.ThinArrow, TokenType.MinusMinus); break;
            case '=': type = Operator2Arrow(TokenType.Equal, TokenType.Equals, TokenType.ThickArrow, TokenType.Equals); break;
            case '!': type = Operator2RepeatNotAllowed(TokenType.Exclamation, TokenType.ExclamationEqual); break;
            case '*': type = Operator2RepeatNotAllowed(TokenType.Star, TokenType.StarEqual); break;
            case '%': type = Operator2RepeatNotAllowed(TokenType.Percentage, TokenType.PercentageEqual); break;
            case '^': type = Operator2RepeatNotAllowed(TokenType.Caret, TokenType.CaretEqual); break;
            case '.': type = Operator3OnlyRepeat(TokenType.Dot, TokenType.DotDot, TokenType.DotDotDot); break;

            case '/':
                _index++;
                if (Current == '/')
                {
                    _index++;
                    while (_index < _source.Length && Current != '\n' && Current != '\0' && Current != '\r')
                        _index++;
                    return Fetch();
                }
                else if (Current == '*')
                {
                    _index++;
                    while (_index < _source.Length)
                    {
                        if (Current == '*')
                        {
                            _index++;
                            if (_index >= _source.Length)
                            {
                                CompilationUnit.Report(
                                    new ReportedDiagnostic(DiagnosticContext.Diagnostics["_commentOutOfBounds"],
                                    GetPosition(new Token(_index, TokenType.EndOfFile, null))));
                                return Fetch();
                            }
                            if (Current == '/')
                                break;
                            else continue;
                        }
                        _index++;
                    }
                    _index++;
                    return Fetch();
                }
                _index--;
                type = Operator2RepeatNotAllowed(TokenType.Slash, TokenType.SlashEqual); break;

            case '<':
                type = OperatorAngleBrackets(TokenType.ArrowRight, TokenType.ArrowRightEqual,
          TokenType.ArrowsRight, TokenType.ArrowsRightEqual); break;
            case '>':
                type = OperatorAngleBrackets(TokenType.ArrowLeft, TokenType.ArrowLeftEqual,
      TokenType.ArrowsLeft, TokenType.ArrowsLeftEqual); break;

            default: type = _index >= _source.Length ? TokenType.EndOfFile : TokenType.Error; break;
        }
        _index++;
        return new Token(_index, type, type != TokenType.EndOfFile ? _source[_index - 1] : null);
    }
}
