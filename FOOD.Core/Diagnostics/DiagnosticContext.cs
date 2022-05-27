using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Diagnostics;

/// <summary>
/// Contains a list of diagnostics and how to handle them.
/// </summary>
public sealed class DiagnosticContext
{
    /// <summary>
    /// These are all of the possible diagnostics.
    /// </summary>
    public static readonly Dictionary<string, Diagnostic> Diagnostics = new()
    {
        { "_lexInvalidNumber", new Diagnostic(DiagnosticLevel.Error, "The number '{0}' could not be parsed.") },
        { "_lexMissingStringTerminator", new Diagnostic(DiagnosticLevel.Error, "Expected a string terminator, however none was found.") },
        { "_lexUnexpectedChar", new Diagnostic(DiagnosticLevel.Error, "The character '{0}' is unknown.") },
        { "_missingClosingBracket", new Diagnostic(DiagnosticLevel.Error, "Expected a closing bracket.") },
        { "_missingOpeningBracket", new Diagnostic(DiagnosticLevel.Error, "Expected an opening bracket.") },
        { "_missingSemicolon", new Diagnostic(DiagnosticLevel.Error, "Expected a semicolon.") },
        { "_binderInvalidType", new Diagnostic(DiagnosticLevel.Error, "This type is unsupported in this context, or not implemented by the type binder.") },
        { "_invalidExpressionGrammar", new Diagnostic(DiagnosticLevel.Error, "The expression syntax is invalid.") },
        { "_intOrBoolExpression", new Diagnostic(DiagnosticLevel.Error, "The expression must be of integer or boolean type.") },
        { "_missingKeyword", new Diagnostic(DiagnosticLevel.Error, "The keyword {0} was expected, but was not found.") },
        { "_missingSemicolonOrEqual", new Diagnostic(DiagnosticLevel.Error, "Expected either a semicolon (;) or an equal sign (=).") },
        { "_symbolAlreadyDefined", new Diagnostic(DiagnosticLevel.Error, "This symbol is already defined in this context.") },
        { "_expectedIdentifier", new Diagnostic(DiagnosticLevel.Error, "An identifier was expected.") },
        { "_invalidTypeQualifier", new Diagnostic(DiagnosticLevel.Error, "The type qualifier {0} is invalid or is not a type qualifier.") },
        { "_missingColonInCondExpr", new Diagnostic(DiagnosticLevel.Error, "Expected a colon (:) in this conditional expression.") },
        { "_missingCommaOrClosingBracket", new Diagnostic(DiagnosticLevel.Error, "Expected a comma (,) or a closing bracket.") },
        { "_mustBeInRootScope", new Diagnostic(DiagnosticLevel.Error, "This declaration or directive must be done at the root of the module object and not in a contained function.") },
        { "_missingSymbol", new Diagnostic(DiagnosticLevel.Error, "The symbol '{0}' couldn't be found in this context.") },
        { "_invalidFunctionSignature", new Diagnostic(DiagnosticLevel.Error, "This function signature does not match with the function signature of {0}.") },
        { "_commentOutOfBounds", new Diagnostic(DiagnosticLevel.Error, "This comment does not terminate.") },
        { "_missingModule", new Diagnostic(DiagnosticLevel.Error, "The module {0} is not referenced or found.") },
        { "_missingNamespace", new Diagnostic(DiagnosticLevel.Error, "The namespace {0}::{1} is not referenced or found.") },
        { "_enumRequiresLiteralInteger", new Diagnostic(DiagnosticLevel.Error, "The constant index value of an enum entry must be of integer type.") },
        { "_missingColon", new Diagnostic(DiagnosticLevel.Error, "Expected a colon (:).") },
        { "_userDefinedPreprocessedError", new Diagnostic(DiagnosticLevel.Error, "(User-defined) {0}") },
        { "_objectClassAlreadyDefined", new Diagnostic(DiagnosticLevel.Error, "The description for this module object has already ben set.") },
        { "_invalidUsageOfPPDirective", new Diagnostic(DiagnosticLevel.Error, "The preprocessor directive {0} is used incorrectly.") },
        { "_missingMacro", new Diagnostic(DiagnosticLevel.Error, "The macro '{0}' was not found.") },
        { "_duplicateDefault", new Diagnostic(DiagnosticLevel.Error, "A default case has already been found in this switch.") },
        { "_missingEndKeyword", new Diagnostic(DiagnosticLevel.Error, "A subswitch must contain an end clause.") },
        { "_cannotModifyConstantValue", new Diagnostic(DiagnosticLevel.Error, "A constant expression or symbol cannot be modified.") },
        { "_missingEqual", new Diagnostic(DiagnosticLevel.Error, "Expected an equal symbol (=).") },
        { "_outOfRegistersAMD64", new Diagnostic(DiagnosticLevel.Fatal, "The compiler ran out of registers.") },
        { "_canOnlyAliasFunction", new Diagnostic(DiagnosticLevel.Error, "It is impossible to alias something else than a function in a function alias.") },

        { "_WDuplicateUsingDirective", new Diagnostic(DiagnosticLevel.Warning2, "The namespace {0} is referenced many times") },
        { "_WUndefiningInexistentMacro", new Diagnostic(DiagnosticLevel.Warning3, "The macro {0} is already undefined") },
        { "_WUsedDefinedPreprocessed", new Diagnostic(DiagnosticLevel.Warning1, "(User-defined) {0}") },
        { "_WDefaultLiteralOfPointer", new Diagnostic(DiagnosticLevel.Warning2, "Getting the constant default value of a pointer") }
    };

    /// <summary>
    /// A list of reported diagnostics.
    /// </summary>
    private readonly List<ReportedDiagnostic> _reportedDiagnostics = new();

    /// <summary>
    /// Reports a diagnostic.
    /// </summary>
    /// <param name="toReport">The reported diagnostic.</param>
    public void Report(ReportedDiagnostic toReport) => _reportedDiagnostics.Add(toReport);

    /// <summary>
    /// Gets all of the reported diagnostics.
    /// </summary>
    /// <returns>The list of reported diagnostic. Immutable.</returns>
    public ImmutableList<ReportedDiagnostic> Get() => _reportedDiagnostics.ToImmutableList();
}
