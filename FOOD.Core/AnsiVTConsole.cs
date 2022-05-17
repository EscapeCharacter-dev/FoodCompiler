using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core;

/// <summary>
/// This console supports ANSI/VT100 escape codes and more.
/// </summary>
public static class AnsiVTConsole
{
    /// <summary>
    /// Writes characters on the terminal.
    /// </summary>
    /// <param name="text">The text.</param>
    public static void Write(string text)
        => Console.Write(text);

    /// <summary>
    /// Gets the integer value for that specific attribute.
    /// </summary>
    /// <param name="attribs">The attribute to convert.</param>
    /// <returns>An escape sequence starting with an escape character defining all the attributes to use.</returns>
    private static string AnsiVTGetAttribute(TermAttributes attrib)
    {
        var str = new StringBuilder("\x1B[");
        if (attrib == TermAttributes.None) return str.Append("0;").ToString();
        if (attrib.HasFlag(TermAttributes.Reversed)) str.Append("7;");
        if (attrib.HasFlag(TermAttributes.Underline)) str.Append("4;");
        if (attrib.HasFlag(TermAttributes.Blinking)) str.Append("5;");
        if (attrib.HasFlag(TermAttributes.Italic)) str.Append("3;");
        return str.ToString();
    }

    /// <summary>
    /// Converts a value from the color enum into a valid ANSI/VT100 color code.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Throws in case of an invalid color.</exception>
    private static (int Color, bool NeedBright) AnsiVTColor(Colors color)
        => color switch
        {
            Colors.Black => (30, false),
            Colors.Red => (31, false),
            Colors.Green => (32, false),
            Colors.Gold => (33, false),
            Colors.Blue => (34, false),
            Colors.Purple => (35, false),
            Colors.Cyan => (36, false),
            Colors.Gray => (37, false),

            Colors.DimGray => (30, true),
            Colors.LightRed => (31, true),
            Colors.Lime => (32, true),
            Colors.Yellow => (33, true),
            Colors.LightBlue => (34, true),
            Colors.Magenta => (35, true),
            Colors.LightCyan => (36, true),
            Colors.White => (37, true),

            _ => throw new InvalidOperationException($"{color} is not supported")
        };

    /// <summary>
    /// Sets a color/attrib. palette in the terminal.
    /// </summary>
    /// <param name="fore">The foreground color.</param>
    /// <param name="back">The background color.</param>
    /// <param name="attribs">The additional attributes.</param>
    public static void SetPalette(
        Colors fore = Colors.Gray,
        Colors back = Colors.Black,
        TermAttributes attrib = TermAttributes.None)
    {
        var ansiFore = AnsiVTColor(fore);
        var ansiBack = AnsiVTColor(back);
        var ansiAttr = AnsiVTGetAttribute(attrib);
        ansiBack.Color += 10;
        Console.Write(ansiAttr);
        Console.Write(ansiFore.NeedBright || ansiBack.NeedBright ? "1;" : "");
        Console.Write(ansiBack.Color + ";");
        Console.Write(ansiFore.Color + "m");
    }
}
