using System;

namespace FOOD.Core;

/// <summary>
/// Represents a position in code.
/// </summary>
public readonly struct Position
{
    public readonly long X;
    public readonly long Y;

    /// <summary>
    /// Initializes on the stack an object of type <see cref="Position"/>.
    /// </summary>
    /// <param name="x">The column.</param>
    /// <param name="y">The row.</param>
    public Position(long x, long y)
    {
        X = x;
        Y = y;
    }

    // Converts a position into readable text (used for error messages and such.)
    public override string ToString() => $"({Y + 1}:{X})";
}