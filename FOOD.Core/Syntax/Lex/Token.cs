using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Lex;

/// <summary>
/// Represents a syntax token.
/// Outputed by the lexer.
/// </summary>
public readonly struct Token
{
    /// <summary>
    /// The position of the token in text.
    /// </summary>
    public readonly long Position;

    /// <summary>
    /// The type of the token.
    /// </summary>
    public readonly TokenType Type;

    /// <summary>
    /// Optional data associated with the token.
    /// </summary>
    public readonly object? Value = null;

    /// <summary>
    /// If true, interpret the value differently. For example, a number will become a float.
    /// </summary>
    public readonly bool Variant = false;

    /// <summary>
    /// Builds a new token.
    /// </summary>
    /// <param name="position">The position of the token.</param>
    /// <param name="type">The type of the token.</param>
    /// <param name="value">The value of the token.</param>
    /// <param name="variant">If the token is a variant.</param>
    public Token(long position, TokenType type, object? value, bool variant = false)
    {
        Position = position;
        Type = type;
        Value = value;
        Variant = variant;
    }

    public override string ToString() => $"{Position} {Type} [{Value?.ToString()}]";
}
