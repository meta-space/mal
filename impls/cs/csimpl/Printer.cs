namespace csimpl;

internal class Printer
{
    internal static string Print(MalValue value)
    {
        return value switch
        {
            MalValue.List(var items) => $"({string.Join(' ', items.Select(Print))})",
            MalValue.Vector(var items) => $"[{string.Join(' ', items.Select(Print))}]",
            MalValue.HashMap(var items) => $"{{{string.Join(' ', items.SelectMany(kvp => new[] { Print(kvp.Key), Print(kvp.Value)}))}}}",
            MalValue.Number(var n) => $"{n}",
            MalValue.String(var s) => $"{s}",
            MalValue.Symbol(var s) => $"{s}",
            MalValue.Function(var _) => "#<function>",
            var oops => oops?.ToString() ?? "null value internal reader error"
        };
    }
}