using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Tree;
/// <summary>
/// A tree with three children.
/// </summary>
public sealed class TernaryTree : ParseTree
{
    /// <summary>
    /// The left branch.
    /// </summary>
    public readonly ParseTree Left;

    /// <summary>
    /// The middle branch.
    /// </summary>
    public readonly ParseTree Middle;

    /// <summary>
    /// The right branch.
    /// </summary>
    public readonly ParseTree Right;

    public override IEnumerable<ParseTree> ChildrenEnumerator => new[] { Left, Middle, Right };

    /// <summary>
    /// Builds a binary tree, with two children.
    /// </summary>
    /// <param name="kind">The kind.</param>
    /// <param name="token">The main token.</param>
    /// <param name="left">The left branch.</param>
    /// <param name="middle">The middle branch.</param>
    /// <param name="right">The right branch.</param>
    public TernaryTree(TreeType kind, Token token, ParseTree left, ParseTree middle, ParseTree right) : base(kind, token)
    {
        Left = left;
        Middle = middle;
        Right = right;
    }
}