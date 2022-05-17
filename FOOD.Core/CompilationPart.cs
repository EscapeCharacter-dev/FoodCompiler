using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core;

/// <summary>
/// Represents a part of the compiler that requires a <see cref="CompilationUnit"/> to work.
/// </summary>
public abstract class CompilationPart
{
    /// <summary>
    /// The compilation unit.
    /// </summary>
    protected CompilationUnit CompilationUnit;

    /// <summary>
    /// Initializes a new instance of the class <see cref="CompilationPart"/>.
    /// </summary>
    /// <param name="unit"></param>
    public CompilationPart(CompilationUnit unit)
        => CompilationUnit = unit;
}
