using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax.Type;

/// <summary>
/// Represents a parsed type.
/// <br>Qualifier Field:</br>
/// <br> - 0b00000001: Constant</br>
/// <br> - 0b00000010: Volatile</br>
/// <br> - 0b00000100: Restrict</br>
/// <br> - 0b00001000: Atomic</br>
/// </summary>
public sealed record ParseType(byte QualifierField, TypeKind Kind, ParseType? SubType = null, object? Extra = null)
{
    public string Print(int indent = 0)
    {
        var result = "";
        for (int i = 0; i < indent; i++)
            result += "  ";
        result += $"{Kind}:{QualifierField}";
        if (SubType is not null)
            result += " ->\n" + SubType.Print(indent + 1);
        return result;
    }

    public override string ToString() => Print();
}
