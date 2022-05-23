namespace FOOD.Core.Syntax.Type;

/// <summary>
/// Provides extension methods for the Type <see cref="TypeKind"/>.
/// </summary>
public static partial class TypeKindExtensions
{
    /// <summary>
    /// Returns true if this type is compatible with another (direct casting).
    /// </summary>
    public static bool IsCompatibleWith(this TypeKind left, TypeKind with)
    {
        if (left == with)
            return true;

        var isIntLeft =
            left == TypeKind.SByte
            || left == TypeKind.Byte
            || left == TypeKind.Int
            || left == TypeKind.UInt
            || left == TypeKind.Long
            || left == TypeKind.ULong;
        var isIntWith =
            with == TypeKind.SByte
            || with == TypeKind.Byte
            || with == TypeKind.Int
            || with == TypeKind.UInt
            || with == TypeKind.Long
            || with == TypeKind.ULong;
        if (isIntLeft && isIntWith) return true;

        var isFloatLeft =
            left == TypeKind.Half
            || left == TypeKind.Float
            || left == TypeKind.Double;
        var isFloatWith =
            with == TypeKind.Half
            || with == TypeKind.Float
            || with == TypeKind.Double;
        if (isFloatLeft && isFloatWith) return true;
        return false;
    }

    /// <summary>
    /// Returns true if this type is unsigned.
    /// </summary>
    public static bool IsUnsigned(this TypeKind left) =>
        left == TypeKind.Byte
     || left == TypeKind.Boolean
     || left == TypeKind.UShort
     || left == TypeKind.UInt
     || left == TypeKind.ULong
     || left == TypeKind.Pointer;

    /// <summary>
    /// Returns true if this type is a floating-point type.
    /// </summary>
    public static bool IsFloat(this TypeKind left) =>
        left == TypeKind.Half
     || left == TypeKind.Float
     || left == TypeKind.Double;
}
