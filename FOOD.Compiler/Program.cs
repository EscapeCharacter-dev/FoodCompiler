using System;
using System.Diagnostics;
using System.Runtime;
using System.Text;
using FOOD.Core;
using FOOD.Core.Generators;
using FOOD.Core.Syntax;
using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Lex;
using FOOD.Core.Syntax.Structure;

namespace FOOD.Compiler;

public static class Program
{
    public static void Main(string[] args)
    {
        var input = File.ReadAllText("../cc/Lex.fd");
        var stopwatch = new Stopwatch();
        /*while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) break;
            var driver = new CompilationDriver("Sample");
            var unit = new CompilationUnit(driver, input, "SampleGen_x86-64");
            var generator = new LLVMGenerator(unit);
            var decl = unit.Parser.ParseDeclaration();
            unit.DisplayDiagnostics();
            generator.GenerateFunction((IFunctionDeclaration)decl!);
            generator.Dump();
            Console.WriteLine();
        }*/
        for (var i = 0; i < 2; i++)
        {
            var driver = new CompilationDriver("lex");
            driver.AddSource("Lex", input);
            stopwatch.Restart();
            driver.Parse();
            stopwatch.Stop();
            driver.DisplayDiagnostics();
            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds + " ms");
        }
    }
}