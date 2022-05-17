﻿using System;
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
    public static readonly Dictionary<string, Diagnostic> Diagnostics = new Dictionary<string, Diagnostic>()
    {
        { "_lexInvalidNumber", new Diagnostic(DiagnosticLevel.Error, "Invalid number") },
        { "_lexMissingStringTerminator", new Diagnostic(DiagnosticLevel.Error, "Missing string terminator") },
        { "_lexUnexpectedChar", new Diagnostic(DiagnosticLevel.Error, "Unexpected character '{0}'") },
        { "_missingClosingBracket", new Diagnostic(DiagnosticLevel.Error, "Missing closing bracket") },
        { "_missingOpeningBracket", new Diagnostic(DiagnosticLevel.Error, "Missing opening bracket") },
        { "_missingSemicolon", new Diagnostic(DiagnosticLevel.Error, "Missing semicolon") },
        { "_binderInvalidType", new Diagnostic(DiagnosticLevel.Error, "Unsupported type") },
        { "_invalidExpressionGrammar", new Diagnostic(DiagnosticLevel.Error, "Invalid expression grammar") },
        { "_intOrBoolExpression", new Diagnostic(DiagnosticLevel.Error, "Expression must be of integer or boolean type") },
        { "_missingKeyword", new Diagnostic(DiagnosticLevel.Error, "Expected keyword {0}") },
        { "_missingSemicolonOrEqual", new Diagnostic(DiagnosticLevel.Error, "Expected semicolon or equal sign") },
        { "_symbolAlreadyDefined", new Diagnostic(DiagnosticLevel.Error, "This symbol has already been defined") },
        { "_expectedIdentifier", new Diagnostic(DiagnosticLevel.Error, "Expected an identifier") },
        { "_invalidTypeQualifier", new Diagnostic(DiagnosticLevel.Error, "Invalid type qualifier {0}") },
        { "_missingColonInCondExpr", new Diagnostic(DiagnosticLevel.Error, "Missing colon in conditional (ternary) expression") },
        { "_missingCommaOrClosingBracket", new Diagnostic(DiagnosticLevel.Error, "Missing comma or closing bracket") },
        { "_mustBeInRootScope", new Diagnostic(DiagnosticLevel.Error, "Must be in root scope (base file level)") },
        { "_missingSymbol", new Diagnostic(DiagnosticLevel.Error, "Cannot find symbol '{0}'") },
        { "_invalidFunctionSignature", new Diagnostic(DiagnosticLevel.Error, "The function signature does not match {0}") },
        { "_commentOutOfBounds", new Diagnostic(DiagnosticLevel.Error, "The comment does not terminate") },
        { "_missingModule", new Diagnostic(DiagnosticLevel.Error, "The module {0} is not found") },
        { "_missingNamespace", new Diagnostic(DiagnosticLevel.Error, "The namespace {0}::{1} is not found") },

        { "_WDuplicateUsingDirective", new Diagnostic(DiagnosticLevel.Warning2, "The namespace {0} is referenced many times") }
    };

    /// <summary>
    /// A list of reported diagnostics.
    /// </summary>
    private readonly List<ReportedDiagnostic> _reportedDiagnostics = new List<ReportedDiagnostic>();

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