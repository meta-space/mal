using System.Text;

namespace csimpl;

internal class Printer
{
    internal static string Print(Mal value, bool isHumanReadable = false, bool isInternal = false)
    {
        return value switch
        {
            Mal.List(var items) => $"({string.Join(' ', items.Select(i => Print(i, isHumanReadable)))})",
            Mal.Vector(var items) => $"[{string.Join(' ', items.Select(i => Print(i, isHumanReadable)))}]",
            Mal.HashMap(var items) => $"{{{string.Join(' ', items.SelectMany(kvp => new[] { Print(kvp.Key), Print(kvp.Value) }))}}}",
            Mal.Number(var n) => $"{n}",
            Mal.String(var s) when !isHumanReadable => isInternal ? s : $"\"{s}\"",
            Mal.String(var s) when isHumanReadable => Escape(s),
            Mal.Symbol(var s) => $"{s}",
            Mal.Atom(var a) => $"(atom {Print(a.Value)})",
            Mal.Function(_) => "#<function>",
            var oops => oops?.ToString() ?? "null value internal reader error"
        };
    }

    internal static Mal.String PrnStr(Mal.ISequence value, string separator, bool isHumanReadable)
    {
        var str = string.Join(separator, value.Select(v => Print(v, isHumanReadable, true)));
        return new Mal.String(str);
    }

    private static string Escape(string s)
    {
        if (s.Length > 0 && s[0] == '\u029e')
        {
            return ":" + s.Substring(1);
        }

        return ToLiteral(s);
    }

    static string ToLiteral(string input)
    {
        StringBuilder literal = new StringBuilder(input.Length + 2);
        literal.Append("\\\"");
        foreach (var c in input)
        {
            switch (c)
            {
                case '\\': literal.Append(@"\\"); break;
                case '\"': literal.Append("\\\""); break;
                case '\0': literal.Append(@"\\0"); break;
                case '\a': literal.Append(@"\\a"); break;
                case '\b': literal.Append(@"\\b"); break;
                case '\f': literal.Append(@"\\f"); break;
                case '\n': literal.Append(@"\\n"); break;
                case '\r': literal.Append(@"\\r"); break;
                case '\t': literal.Append(@"\\t"); break;
                case '\v': literal.Append(@"\\v"); break;
                default:
                    // ASCII printable character
                    if (c >= 0x20 && c <= 0x7e)
                    {
                        literal.Append(c);
                        // As UTF16 escaped character
                    }
                    else
                    {
                        literal.Append(@"\u");
                        literal.Append(((int)c).ToString("x4"));
                    }
                    break;
            }
        }
        literal.Append("\\\"");
        return literal.ToString();
    }
}