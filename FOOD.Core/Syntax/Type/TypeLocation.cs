using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Type;

/// <summary>
/// Represents the various locations of a variable.
/// </summary>
public enum TypeLocation
{
    /// <summary>
    /// Stored on the stack.
    /// </summary>
    Local,

    /// <summary>
    /// Stored in the binary.
    /// </summary>
    Global,

    /// <summary>
    /// Stored in a register.
    /// </summary>
    Register,

    /// <summary>
    /// Indicates that the variable is stored elsewhere
    /// in another binary that isn't publicly referenced.
    /// </summary>
    Extern,
}
