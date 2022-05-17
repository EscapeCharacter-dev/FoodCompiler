using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Tree;

/// <summary>
/// A class that can have an unlimited amount of children.
/// </summary>
public class ExtensibleTree : ParseTree
{
    /// <summary>
    /// The child nodes.
    /// </summary>
    private readonly ParseTree[] _children;

    public override IEnumerable<ParseTree> ChildrenEnumerator => _children;

    public ExtensibleTree(TreeType kind, Token token, ParseTree[] children) : base(kind, token)
    {
        _children = children;
    }
}
