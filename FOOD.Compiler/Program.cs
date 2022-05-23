using System;
using System.Diagnostics;
using System.Runtime;
using System.Text;
using FOOD.Core;
using FOOD.Core.Generators;
using FOOD.Core.Syntax;
using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Lex;

namespace FOOD.Compiler;

public static class Program
{
    public static void Main(string[] args)
    {
        //var input = File.ReadAllText("Samples/Enumerable.fd");
        //var stopwatch = new Stopwatch();
        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) break;
            var driver = new CompilationDriver("Sample");
            var unit = new CompilationUnit(driver, input, "SampleGen_x86-64");
            var generator = new LLVMGenerator();
            var expr = unit.Parser.Binder.BindExpression(unit.Parser.ParseExpression());
            var llvm = generator.GenerateExpression(expr);
            llvm.Dump();
            Console.WriteLine();
        }
        //driver.AddSource("Enumerable", input);
        //stopwatch.Restart();
        //driver.Parse();
        //stopwatch.Stop();
        //driver.DisplayDiagnostics();
        //Console.WriteLine(stopwatch.ElapsedMilliseconds + " ms");
    }
}