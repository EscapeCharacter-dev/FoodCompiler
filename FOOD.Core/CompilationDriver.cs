﻿using FOOD.Core.Scoping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core;

/// <summary>
/// A compilation driver makes the steps of compilation happen to multiple files at once and builds modules.
/// </summary>
public sealed class CompilationDriver
{
    public readonly Module Module;
    private readonly List<CompilationUnit> _units = new List<CompilationUnit>();

    public CompilationDriver(string moduleName)
    {
        Module = new Module(moduleName);
    }

    public void AddSource(string source) => _units.Add(new CompilationUnit(this, source));

    public void Parse()
    {
#if DEBUG
        foreach (var unit in _units)
        {
            unit.Parse();
            Module.Objects.Add(unit.ModuleObject);
        }
#else
        Parallel.ForEach(_units, unit => unit.Parse());
        _units.ForEach(unit =>
        {
            Module.Objects.Add(unit.ModuleObject);
            unit.DisplayDiagnostics();
        });
#endif
    }

    public void DisplayDiagnostics()
    {
        foreach (var unit in _units)
            unit.DisplayDiagnostics();
    }
}
