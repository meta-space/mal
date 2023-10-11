namespace csimpl;

internal class Printer
{
    internal static string Print(MalValue value)
    {
        return value switch
        {
            MalValue.List(var items) => $"({string.Join(' ', items.Select(Print))})",
            MalValue.Vector(var items) => $"[{string.Join(' ', items.Select(Print))}]",
            MalValue.HashMap(var items) => $"{{{string.Join(' ', items.SelectMany(kvp => new[] { Print(kvp.Value), Print(kvp.Key)}))}}}",
            MalValue.Number(var number) => $"{number}",
            MalValue.Atom(var symbol) => $"{symbol}",
            var oops => oops?.ToString() ?? "null value internal reader error"
        };
    }
}