namespace FOOD.Core.Diagnostics;

/// <summary>
/// An instanced diagonstic with data attached to it.
/// </summary>
public readonly struct ReportedDiagnostic
{
    /// <summary>
    /// The diagnostic.
    /// </summary>
    public readonly Diagnostic Diagnostic;

    /// <summary>
    /// The position from where the diagnostic was emitted. (0, 0) is directly from the compiler.
    /// </summary>
    public readonly Position Position;

    /// <summary>
    /// Other parameters, etc.
    /// </summary>
    private readonly object[] _miscData;

    /// <summary>
    /// Initializes a new instance of the class <see cref="ReportedDiagnostic"/>.
    /// </summary>
    /// <param name="diagonstic">The diagnostic from <see cref="DiagnosticContext.Diagnostics"/></param>
    /// <param name="position">The position where it occured.</param>
    /// <param name="miscData">Other miscellaneous data.</param>
    public ReportedDiagnostic(Diagnostic diagonstic, Position position, params object[] miscData)
    {
        Diagnostic = diagonstic;
        Position = position;
        _miscData = miscData;
    }

    /// <summary>
    /// Gets a string that consists of the interpolated diagnostic message with parameters.
    /// </summary>
    /// <returns></returns>
    public string Render() => Diagnostic.Render(_miscData);
}
