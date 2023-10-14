namespace csimpl;

internal class Printer
{
    internal static string Print(Mal value)
    {
        return value switch
        {
            Mal.List(var items) => $"({string.Join(' ', items.Select(Print))})",
            Mal.Vector(var items) => $"[{string.Join(' ', items.Select(Print))}]",
            Mal.HashMap(var items) => $"{{{string.Join(' ', items.SelectMany(kvp => new[] { Print(kvp.Key), Print(kvp.Value)}))}}}",
            Mal.Number(var n) => $"{n}",
            Mal.String(var s) => $"{s}",
            Mal.Symbol(var s) => $"{s}",
            Mal.Function(_) => "#<function>",
            var oops => oops?.ToString() ?? "null value internal reader error"
        };
    }

    internal static Mal Prn_Str(Mal value, string space, bool isHumanReadable)
    {
        Console.WriteLine(value);
       return Mal.Nil; 
    }
}