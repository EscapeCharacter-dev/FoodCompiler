using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Structure;

/// <summary>
/// The general location of the declared object.
/// </summary>
public enum Location
{
    /// <summary>
    /// Specifies that this object is found in another library.
    /// </summary>
    External,

    /// <summary>
    /// A public object can be referenced through the whole module and beyond.
    /// </summary>
    Public,

    /// <summary>
    /// The object can only be referenced inside this file.
    /// </summary>
    Static,

    /// <summary>
    /// The object is stored on either the stack or a register. Invalid for functions.
    /// </summary>
    Local,

    /// <summary>
    /// A function argument.
    /// </summary>
    Argument,
}