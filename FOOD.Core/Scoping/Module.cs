using FOOD.Core.Syntax.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Scoping;

/// <summary>
/// A module is one of the most important concepts in Food. They are required to build programs or libraries.
/// </summary>
public sealed class Module
{
    /// <summary>
    /// The name of the module. This name will also define its namespace.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The module objects that are required to build this module.
    /// </summary>
    public readonly List<ModuleObject> Objects = new List<ModuleObject>();

    public readonly List<Module> Externs = new List<Module>();

    public bool HasExtern(string moduleName)
    {
        foreach (var module in Externs)
        {
            if (module.Name == moduleName)
                return true;
        }
        return false;
    }

    public bool HasNamespace(string nameSpace)
    {
        foreach (var moduleObject in Objects)
        {
            if (moduleObject.Namespace == nameSpace)
                return true;
        }
        return false;
    }

    public Module? GetExtern(string moduleName)
    {
        foreach (var module in Externs)
        {
            if (module.Name == moduleName)
                return module;
        }
        return null;
    }

    public List<ModuleObject> GetObjectsInNamespace(string nameSpace)
    {
        var list = new List<ModuleObject>();
        foreach (var moduleObject in Objects)
        {
            if (moduleObject.Namespace == nameSpace)
                list.Add(moduleObject);
        }
        return list;
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="Module"/>.
    /// </summary>
    /// <param name="name">The name of the module & its global namespace.</param>
    /// <param name="categories">The nested namespaces that it contains.</param>
    public Module(string name)
    {
        Name = name;
    }
}
