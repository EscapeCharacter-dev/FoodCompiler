using FOOD.Core.Syntax.Tree;
using FOOD.Core.Syntax.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Binding;

/// <summary>
/// Represents a tree that was bound to a type.
/// </summary>
public sealed class BoundTree
{
    /// <summary>
    /// The tree without its binding.
    /// </summary>
    public readonly ParseTree CoreTree;

    /// <summary>
    /// The type that is bound to the tree.
    /// </summary>
    public readonly ParseType BoundType;

    public readonly IEnumerable<BoundTree> Children;

    /// <summary>
    /// Builds a bound tree.
    /// </summary>
    /// <param name="core">The unbound tree.</param>
    /// <param name="boundType">The type to bind.</param>
    public BoundTree(ParseTree core, ParseType boundType, IEnumerable<BoundTree> children)
    {
        CoreTree = core;
        BoundType = boundType;
        Children = children;
    }
}
