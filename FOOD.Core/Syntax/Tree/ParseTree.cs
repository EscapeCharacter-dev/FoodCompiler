using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Tree;

/// <summary>
/// The most basic representation of a parse tree.
/// </summary>
public abstract class ParseTree
{
    /// <summary>
    /// An enumerator that represents all children branches.
    /// </summary>
    public abstract IEnumerable<ParseTree> ChildrenEnumerator { get; }

    /// <summary>
    /// The kind of a tree.
    /// </summary>
    public TreeType TreeType { get; }

    /// <summary>
    /// The main token of the tree.
    /// </summary>
    public Token Token { get; }

    /// <summary>
    /// Builds a parse tree.
    /// </summary>
    /// <param name="type">The kind of the tree.</param>
    /// <param name="token">The main token.</param>
    protected ParseTree(TreeType type, Token token)
    {
        TreeType = type;
        Token = token;
    }

    public string Print(int previousIndent = 0)
    {
        var result = $"{TreeType}({Token.Value?.ToString()})";
        if (ChildrenEnumerator.Count() > 0)
        {
            result += ':';
        }
        result += '\n';
        foreach (var child in ChildrenEnumerator)
        {
            for (int i = 0; i <= previousIndent; i++)
                result += "  ";
            result += child.Print(previousIndent + 1);
        }
        return result;
    }

    public override string ToString() => Print();

    /// <summary>
    /// Returns true if this expression contains only literals and no identifiers/func. calls, etc.
    /// </summary>
    /// <returns></returns>
    public bool IsConstant()
    {
        foreach (var children in ChildrenEnumerator)
            if (!children.IsConstant()) return false;
        if (TreeType != TreeType.Literal || Token.Type != TokenType.LiteralNumber) return false;
        return true;
    }
}
