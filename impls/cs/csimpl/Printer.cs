using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace csimpl;

internal class Printer
{
    internal static string Print(MalValue value)
    {
        return value switch
        {
            MalValue.List(var items) => $"({string.Join(' ', items.Select(Print))})",
            MalValue.Number(var number) => $"{number}",
            MalValue.Atom(var symbol) => $"{symbol}",
            var oops => oops?.ToString() ?? "null value internal reader error"
        };
    }
}