using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Lex;

/// <summary>
/// The type of a token.
/// </summary>
public enum TokenType
{
    Identifier,
    /// <summary>
    /// The end of the text.
    /// </summary>
    EndOfFile,

    /// <summary>
    /// Used in case of a syntax error.
    /// </summary>
    Error,

    LiteralNumber,
    LiteralString,

    Equal,
    Equals,
    ThickArrow,

    Exclamation,
    ExclamationEqual,
    Interrogation,

    Plus,
    PlusPlus,
    PlusEqual,

    Minus,
    MinusMinus,
    MinusEqual,
    ThinArrow,

    Star,
    StarEqual,

    Slash,
    SlashEqual,

    Percentage,
    PercentageEqual,

    Caret,
    CaretEqual,

    Ampersand,
    Ampersands,
    AmpersandEqual,

    VerticalBar,
    VerticalBars,
    VerticalBarEqual,

    ArrowRight,
    ArrowsRight,
    ArrowRightEqual,
    ArrowsRightEqual,

    ArrowLeft,
    ArrowsLeft,
    ArrowLeftEqual,
    ArrowsLeftEqual,

    Dot,
    DotDot,
    Comma,
    Colon,
    Semicolon,
    Squiggle,

    OpenBracket,
    ClosedBracket,
    OpenSquareBracket,
    ClosedSquareBracket,
    OpenCurlyBracket,
    ClosedCurlyBracket,

    LanguageAttribute,
    CompilerAttribute,
    KeywordAtomic,
    KeywordBreak,
    KeywordBool,
    KeywordByte,
    KeywordCase,
    KeywordConst,
    KeywordContinue,
    KeywordDefault,
    KeywordDo,
    KeywordDouble,
    KeywordDynamic,
    KeywordElse,
    KeywordEnum,
    KeywordExtern,
    KeywordFloat,
    KeywordFor,
    KeywordFunction,
    KeywordGoto,
    KeywordHalf,
    KeywordIf,
    KeywordInt,
    KeywordLong,
    KeywordNamespace,
    KeywordPublic,
    KeywordRecord,
    KeywordRestrict,
    KeywordReturn,
    KeywordSByte,
    KeywordShort,
    KeywordSizeof,
    KeywordStatic,
    KeywordStruct,
    KeywordSwitch,
    KeywordTypedef,
    KeywordUnion,
    KeywordUInt,
    KeywordULong,
    KeywordUShort,
    KeywordVoid,
    KeywordVolatile,
    KeywordWhile,
    KeywordTrue,
    KeywordFalse,
    KeywordUsing,
    Cube,
    KeywordNew,
    LiteralBoolean,
    KeywordNull,
    DotDotDot,
    KeywordStart,
    KeywordEnd,
}
