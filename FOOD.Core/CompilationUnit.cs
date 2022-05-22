using FOOD.Core.Diagnostics;
using FOOD.Core.Syntax;
using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Structure;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core;

/// <summary>
/// Represents the source and output of a translation unit.
/// </summary>
public sealed class CompilationUnit
{
    /// <summary>
    /// The input source of this compilation unit.
    /// </summary>
    public readonly string Input;

    /// <summary>
    /// The name of the file (used when displaying errors)
    /// </summary>
    public readonly string Name;
    
    /// <summary>
    /// The parser gives the meaning of the text to the compiler.
    /// </summary>
    public readonly Parser Parser;

    /// <summary>
    /// The diagnostic context takes care of displaying diagnostic
    /// and handling them. You have to report your diagnostics to it.
    /// </summary>
    private readonly DiagnosticContext _diagContext;

    /// <summary>
    /// The module object is the final representation of the source.
    /// It will be serialized into a .mo object, a binary module object.
    /// </summary>
    public readonly ModuleObject ModuleObject;

    /// <summary>
    /// The compilation driver is the parent object that takes care of
    /// building all of the module objects.
    /// </summary>
    public readonly CompilationDriver Driver;

    /// <summary>
    /// Initializes a new instance of the class <see cref="CompilationUnit"/>.
    /// </summary>
    /// <param name="input">The source input.</param>
    public CompilationUnit(CompilationDriver driver, string input, string name)
    {
        _diagContext = new DiagnosticContext();
        Driver = driver;
        Name = name;

        Input = input + " ";
        Parser = new Parser(this, Input);
        ModuleObject = new ModuleObject(Parser.Root);
    }

    /// <summary>
    /// Parses the source and creates the module object instance.
    /// </summary>
    public void Parse()
    {
        while (!Parser.EndReached) 
            Parser.ParseDeclaration();
    }

    /// <summary>
    /// Reports a diagnostic.
    /// </summary>
    /// <param name="toReport">The reported diagnostic.</param>
    public void Report(ReportedDiagnostic toReport) => _diagContext.Report(toReport);

    /// <summary>
    /// Displays all of the diagnostics.
    /// </summary>
    public void DisplayDiagnostics()
    {
        AnsiVTConsole.SetPalette();
        var diags = _diagContext.Get();
        if (diags.IsEmpty)
            return;
        Console.WriteLine($"Diagnostics thrown when compiling {Name}:");
        foreach (var diag in diags)
        {
            Console.Write(diag.Position + ": ");
            AnsiVTConsole.SetPalette(
                diag.Diagnostic.Level == DiagnosticLevel.Information ? Colors.LightCyan :
                diag.Diagnostic.Level >= DiagnosticLevel.Warning1 && diag.Diagnostic.Level <= DiagnosticLevel.Warning4 ? Colors.Yellow :
                diag.Diagnostic.Level == DiagnosticLevel.Error ? Colors.LightRed : Colors.White,
                diag.Diagnostic.Level == DiagnosticLevel.Fatal ? Colors.Red : Colors.Black
                );
            Console.WriteLine(diag.Render());
            Console.WriteLine();
            
            if (diag.Position.X != 0)
            {
                var line = Input.Split('\n')[diag.Position.Y];
                AnsiVTConsole.SetPalette(Colors.Purple, Colors.Black, TermAttributes.Underline);
                Console.Write(line);
                AnsiVTConsole.SetPalette(Colors.Yellow, Colors.Black);
                Console.Write('\n');
                for (int i = 0; i < diag.Position.X; i++)
                {
                    if (line[i] == '\t')
                        Console.Write("~~~~");
                    else Console.Write('~');
                }
                Console.WriteLine('^');
            }
            AnsiVTConsole.SetPalette();
        }
        for (var i = 0; i < Console.WindowWidth; i++) Console.Write('=');
        Console.WriteLine('\n');
    }
}
