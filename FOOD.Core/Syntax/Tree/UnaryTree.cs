using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Tree;

/// <summary>
/// A tree with only one leaf or child.
/// </summary>
public sealed class UnaryTree : ParseTree
{
    /// <summary>
    /// The leaf.
    /// </summary>
    public readonly ParseTree Child;

    public override IEnumerable<ParseTree> ChildrenEnumerator => new[] { Child };

    /// <summary>
    /// Builds a unary tree, with only one leaf.
    /// </summary>
    /// <param name="kind">The kind.</param>
    /// <param name="token">The main token.</param>
    /// <param name="child">The leaf.</param>
    public UnaryTree(TreeType kind, Token token, ParseTree child) : base (kind, token)
    {
        Child = child;
    }
}
