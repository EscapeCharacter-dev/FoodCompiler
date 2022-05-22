using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core;
public static class EnumerableExtensions
{
    public static IEnumerable<TResult> Each<TResult, TValue>(this IEnumerable<TValue> source, Func<TValue, TResult> each)
    {
        var enumerable = new TResult[source.Count()];
        var i = 0;
        foreach (var input in source)
            enumerable[i++] = each(input);
        return enumerable;
    }
}
