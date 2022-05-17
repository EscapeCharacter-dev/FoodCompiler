using FOOD.Core.Syntax.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Scoping;

/// <summary>
/// A scope is a location where a part of a program stores its variables.
/// </summary>
public sealed class Scope
{
    /// <summary>
    /// This is all of the declarations it contains.
    /// </summary>
    public List<IDeclaration> Declarations = new List<IDeclaration>(12);

    /// <summary>
    /// A list of all of the scopes that this scope encapsulates.
    /// </summary>
    public List<Scope> Scopes = new List<Scope>(2);

    /// <summary>
    /// The scope that encapsulates this one. If this is null, then this is a headless scope.
    /// </summary>
    public readonly Scope? Parent;

    /// <summary>
    /// Initializes a new instance of the class <see cref="Scope"/>.
    /// </summary>
    /// <param name="parent"></param>
    public Scope(Scope? parent)
    {
        Parent = parent;
    }

    public static Scope operator +(Scope scope, IDeclaration decl)
    {
        scope.Declarations.Add(decl);
        return scope;
    }

    public static Scope? operator -(Scope scope) => scope.Parent;

    /// <summary>
    /// Checks if a variable is already declared in this scope or a parent scope.
    /// </summary>
    /// <param name="ident"></param>
    /// <returns></returns>
    public bool IsSymbolDeclared(string ident, bool searchInParents = true)
    {
        foreach (var decl in Declarations)
        {
            if (decl.Name == ident)
                return true;
        }
        if (Parent == null) return false;
        if (searchInParents)
            return Parent.IsSymbolDeclared(ident);
        return false;
    }

    /// <summary>
    /// Gets a declaration.
    /// </summary>
    /// <param name="ident">The identifier.</param>
    /// <returns></returns>
    public IDeclaration? GetDeclaration(string ident)
    {
        foreach (var decl in Declarations)
        {
            if (decl.Name == ident)
                return decl;
        }
        if (Parent == null) return null;
        return Parent.GetDeclaration(ident);
    }

    /// <summary>
    /// Gets the closest function to this one in the tree.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception">Do not run this function at root of file.</exception>
    public IDeclaration GetClosestFunction()
    {
        foreach (var decl in Declarations)
        {
            if (decl.GetType() == typeof(ImperativeFunctionDeclaration)
                || decl.GetType() == typeof(SimpleFunctionDeclaration))
                return decl;
        }
        if (Parent == null) throw new Exception("Cannot be run in root node");
        return Parent.GetClosestFunction();
    }
}
