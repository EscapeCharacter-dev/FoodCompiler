using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Tree;

/// <summary>
/// Represents a tree that contains a type.
/// </summary>
public sealed class TypeTree : ParseTree
{
    public override IEnumerable<ParseTree> ChildrenEnumerator => Enumerable.Empty<ParseTree>();

    public readonly ParseType Type;

    /// <summary>
    /// Builds a type tree.
    /// </summary>
    /// <param name="treeType">The type.</param>
    /// <param name="token">The main token.</param>
    /// <param name="type">The type.</param>
    public TypeTree(TreeType treeType, Token token, ParseType type) : base(treeType, token)
    {
        Type = type;
    }
}
