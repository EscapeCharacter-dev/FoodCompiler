using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Statements;

/// <summary>
/// Represents a statement.
/// </summary>
public abstract class Statement
{
    /// <summary>
    /// The statements contained in this statement.
    /// </summary>
    public abstract IEnumerable<Statement> ChildrenEnumerator { get; }
}
