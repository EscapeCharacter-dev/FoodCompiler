using FOOD.Core.Scoping;
using FOOD.Core.Syntax.Statements;
using System.Collections.Immutable;

namespace FOOD.Core.Syntax.Structure;

/// <summary>
/// An object inside a module, a compiled file.
/// </summary>
public sealed class ModuleObject
{
    /// <summary>
    /// The root scope of the module.
    /// </summary>
    public readonly Scope Root;

    /// <summary>
    /// The current namespace.
    /// </summary>
    public string Namespace = "_root";

    /// <summary>
    /// A list of dependencies (modules) that this module relies on.
    /// </summary>
    public readonly List<string> Dependencies = new List<string>();

    /// <summary>
    /// Initializes a new instance of the class <see cref="ModuleObject"/>.
    /// </summary>
    /// <param name="root">The root scope.</param>
    public ModuleObject(Scope root)
    {
        Root = root;
    }
}
