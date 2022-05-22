using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Tree;

/// <summary>
/// Represents the kind of a tree.
/// </summary>
public enum TreeType
{
    /// <summary>
    /// Used for malformed trees and such.
    /// </summary>
    Error,

    Literal,
    Identifier,
    NamespacedIdentifier,
    Type,

    PostfixIncrement,
    PostfixDecrement,
    FunctionCall,
    ArraySubscript,
    MemberAccess,
    PointerMemberAccess,

    PrefixIncrement,
    PrefixDecrement,
    UnaryPlus,
    UnaryMinus,
    LogicalNegation,
    BitwiseNegation,
    Dereference,
    AddressOf,
    Sizeof,

    Multiply,
    Divide,
    Modulo,

    Addition,
    Subtraction,

    BitwiseLeftShift,
    BitwiseRightShift,

    Lower,
    LowerOrEqual,
    Greater,
    GreaterOrEqual,

    Equal,
    NotEqual,

    BitwiseAnd,

    BitwiseExclusiveOr,

    BitwiseOr,
    
    LogicalOr,

    LogicalAnd,

    Cast,

    Conditional,

    Assign,
    SumAssign,
    DifferenceAssign,
    ProductAssign,
    QuotientAssign,
    RemainderAssign,
    BitwiseLeftAssign,
    BitwiseRightAssign,
    BitwiseAndAssign,
    BitwiseExclusiveOrAssign,
    BitwiseOrAssign,

    Compound,
    New,
    ArrayLiteral,
}
