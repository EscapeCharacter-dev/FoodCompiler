﻿using System;
using System.Diagnostics;
using System.Runtime;
using System.Text;
using FOOD.Core;
using FOOD.Core.Syntax;
using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Lex;

namespace FOOD.Compiler;

public static class Program
{
    public static void Main(string[] args)
    {
        var input = File.ReadAllText("Samples/Square.fd");
        var stopwatch = new Stopwatch();
        var driver = new CompilationDriver("Sample");
        driver.AddSource(input);
        stopwatch.Restart();
        driver.Parse();
        stopwatch.Stop();
        driver.DisplayDiagnostics();
        Console.WriteLine(stopwatch.ElapsedMilliseconds + " ms");
    }
}