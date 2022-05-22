using FOOD.Core.Syntax.Tree;
using FOOD.Core.Syntax.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Structure;

/// <summary>
/// Represents a declaration.
/// </summary>
public interface IDeclaration
{
    /// <summary>
    /// The name of the variable.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The type.
    /// </summary>
    public ParseType Type { get; }

    /// <summary>
    /// The location/location type of the variable in its scope.
    /// </summary>
    public Location Location { get; }

    /// <summary>
    /// Is the declaration public.
    /// </summary>
    public bool IsPublic { get; }

    /// <summary>
    /// The list of attributes that this declaration is affected by.
    /// </summary>
    public string[] Attributes { get; }
}
