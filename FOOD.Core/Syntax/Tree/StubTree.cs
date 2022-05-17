using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Tree;

/// <summary>
/// A childless tree.
/// </summary>
public sealed class StubTree : ParseTree
{
    public override IEnumerable<ParseTree> ChildrenEnumerator => Enumerable.Empty<ParseTree>();

    /// <summary>
    /// Builds a stub tree, a tree without children.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="token">The main token.</param>
    public StubTree(TreeType kind, Token token) : base(kind, token) { }
}
