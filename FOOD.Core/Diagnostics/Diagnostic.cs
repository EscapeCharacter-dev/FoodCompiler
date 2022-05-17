using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FOOD.Core.Diagnostics;

/// <summary>
/// A diagnostic.
/// </summary>
public struct Diagnostic
{
    /// <summary>
    /// The level of criticalness of the diagnostic or message, ranges from Information to Fatal Errors.
    /// </summary>
    public DiagnosticLevel Level;

    /// <summary>
    /// The message that will be displayed. Interpolation works with {0}, {1}, etc.
    /// </summary>
    public readonly string Message;

    /// <summary>
    /// Initializes a new <see cref="Diagnostic"/>. You can perform interpolation in the message with {0}, {1}, etc.
    /// </summary>
    /// <param name="defaultLevel">The default level.</param>
    /// <param name="message">The message to be displayed.</param>
    public Diagnostic(DiagnosticLevel defaultLevel, string message)
    {
        Level = defaultLevel;
        Message = message;
    }

    /// <summary>
    /// Creates a displayable string with interpolatable parameters.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The renderable string.</returns>
    public string Render(params object[] parameters)
    {
        var i = 0;
        var msg = Message;
        foreach (var parameter in parameters)
        {
            msg = Message.Replace($"{{{i}}}", parameter.ToString());
            i++;
        }
        return msg;
    }

    public override string ToString() => Render();
}
