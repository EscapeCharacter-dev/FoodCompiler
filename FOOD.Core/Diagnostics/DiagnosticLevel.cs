namespace FOOD.Core.Diagnostics;

public enum DiagnosticLevel
{
    /// <summary>
    /// This is a message to the program by the compiler that contains usually nothing important
    /// or too critical. NOT a warning.
    /// </summary>
    Information,

    /// <summary>
    /// These are about possible errors or odd coding.
    /// </summary>
    Warning1,

    /// <summary>
    /// These error messages are still of interest of most programmers, but are way less critical.
    /// </summary>
    Warning2,

    /// <summary>
    /// Warnings that can of use, but shouldn't be too overlooked, unless you're writing something
    /// serious.
    /// </summary>
    Warning3,

    /// <summary>
    /// Pedantic. For serious programming.
    /// </summary>
    Warning4,

    /// <summary>
    /// An error prevents compilation in all cases, and is mostly due to mistakes found during the
    /// parsing or linking processes.
    /// </summary>
    Error,

    /// <summary>
    /// A fatal error is an internal fatal error of the compiler. This will stop the compiler. This
    /// should not happen in normal cases.
    /// </summary>
    Fatal,
}
