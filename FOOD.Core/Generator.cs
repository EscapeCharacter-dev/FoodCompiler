using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Statements;
using FOOD.Core.Syntax.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core;

/// <summary>
/// This abstract class provides a frame to build code generators.
/// </summary>
public abstract class Generator<T> : CompilationPart
{
    protected Generator(CompilationUnit unit) : base(unit)
    {
        _gens = new()
        {
            { TreeType.Addition, x => GenAdd(x) },
            { TreeType.Subtraction, x => GenSub(x) },
            { TreeType.Multiply, x => GenMul(x) },
            { TreeType.Divide, x => GenDiv(x) },
            { TreeType.Modulo, x => GenMod(x) },
            { TreeType.Literal, x => GenLiteral(x) }
        };
    }

    /// <summary>
    /// This <see cref="StringBuilder"/> allows the generator to store the generated code.
    /// </summary>
    protected readonly StringBuilder _outputBuilder = new StringBuilder(4096);

    /// <summary>
    /// The output code.
    /// </summary>
    public string Output => _outputBuilder.ToString();

    /// <summary>
    /// Generates code for an expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    public T GenerateExpression(BoundTree expression) => _gens[expression.CoreTree.TreeType](expression);

    /// <summary>
    /// The generation functions.
    /// </summary>
    private readonly Dictionary<TreeType, Func<BoundTree, T>> _gens;

    /// <summary>
    /// Generates an addition.
    /// </summary>
    protected abstract T GenAdd(BoundTree expr);

    /// <summary>
    /// Generates a subtraction.
    /// </summary>
    protected abstract T GenSub(BoundTree expr);

    /// <summary>
    /// Generates a multiplication.
    /// </summary>
    protected abstract T GenMul(BoundTree expr);

    /// <summary>
    /// Generates a division.
    /// </summary>
    protected abstract T GenDiv(BoundTree expr);

    /// <summary>
    /// Generates a division and gets the remainder.
    /// </summary>
    protected abstract T GenMod(BoundTree expr);

    /// <summary>
    /// Generates a literal value.
    /// </summary>
    protected abstract T GenLiteral(BoundTree expr);
}
