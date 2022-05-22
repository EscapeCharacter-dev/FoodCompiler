using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Type;

/// <summary>
/// Represents the kind of a type.
/// </summary>
public enum TypeKind
{
    /// <summary>
    /// Used to indicate syntax errors.
    /// </summary>
    Error,

    /// <summary>
    /// <b>void</b> is an incomplete type that indicates a lack of data.
    /// </summary>
    Void,

    /// <summary>
    /// <b>bool</b> is internally represented as a <see cref="Byte" />.
    /// </summary>
    Boolean,

    /// <summary>
    /// <b>sbyte</b>, <b>char</b> or <b>i8</b> is a 8-bit signed integer.
    /// </summary>
    SByte,

    /// <summary>
    /// <b>byte</b>, <b>uchar</b> or <b>u8</b> is a 8-bit unsigned integer.
    /// </summary>
    Byte,

    /// <summary>
    /// <b>short</b> or <b>i16</b> is a 16-bit signed integer.
    /// </summary>
    Short,

    /// <summary>
    /// <b>ushort</b> or <b>u16</b> is a 16-bit unsigned integer.
    /// </summary>
    UShort,

    /// <summary>
    /// <b>half</b> or <b>f16</b> is a 16-bit float.
    /// </summary>
    Half,

    /// <summary>
    /// <b>int</b> or <b>i32</b> is a 32-bit signed integer.
    /// </summary>
    Int,

    /// <summary>
    /// <b>uint</b> or <b>u32</b> is a 32-bit unsigned integer.
    /// </summary>
    UInt,

    /// <summary>
    /// An enum can have multiple fixed values. Typically represented by <b>int</b> in code.
    /// </summary>
    Enum,

    /// <summary>
    /// <b>float</b> or <b>f32</b> is a 32-bit float.
    /// </summary>
    Float,

    /// <summary>
    /// <b>long</b> or <b>i64</b> is a 64-bit signed integer.
    /// </summary>
    Long,

    /// <summary>
    /// <b>ulong</b> or <b>u64</b> is a 64-bit unsigned integer.
    /// </summary>
    ULong,

    /// <summary>
    /// <b>double</b> or <b>f64</b> is a 64-bit float.
    /// </summary>
    Double,

    /// <summary>
    /// A function types represents a callable lambda or function. Its address is stored in the variable.
    /// </summary>
    Function,

    /// <summary>
    /// A pointer points to data.
    /// </summary>
    Pointer,

    /// <summary>
    /// A reference points to a variable, but can't be null nor changed.
    /// </summary>
    Reference,

    /// <summary>
    /// An array is a group of multiple values.
    /// </summary>
    Array,

    /// <summary>
    /// A structure is a block of multiple <i>members</i>.
    /// </summary>
    Struct,
    String,
}
