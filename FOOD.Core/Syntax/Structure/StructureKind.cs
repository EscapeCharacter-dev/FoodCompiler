using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Structure;

/// <summary>
/// Used to define the kind of a structure.
/// </summary>
public enum StructureKind
{
    /// <summary>
    /// A normal structure.
    /// </summary>
    Structure,

    /// <summary>
    /// A record is a structure that has read-only members.
    /// </summary>
    Record,

    /// <summary>
    /// An union represents multiple files, that share a common address.
    /// </summary>
    Union,
}
