using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace csimpl;

readonly struct Token
{
    public readonly int Index;
    public readonly int Length;

    public Token(int index, int length)
    {
        Index = index;
        Length = length;
    }

    public ReadOnlySpan<char> Value(ReadOnlySpan<char> input)
    {
        return input.Slice(Index, Length).Trim();
    }

    public bool IsEqual(ReadOnlySpan<char> input, string token)
    {
        return Value(input).CompareTo(token, StringComparison.InvariantCulture) == 0;
    }
}

internal ref struct Reader
{
    private readonly ReadOnlySpan<char> _input;
    private readonly Collection<Token> _tokens = new();
    private int _position = 0;

    private Reader(ReadOnlySpan<char> input)
    {
        _input = input;
        Tokenize(_input);
    }

    Token Next()
    {
        return _tokens[_position++];
    }

    Token Peek()
    {
        return _tokens[_position];
    }

    private void Tokenize(ReadOnlySpan<char> input)
    {
        // Create a regular expression to match the word "test".
        string pattern = @"[\s,]*(~@|[\[\]{}()'`~^@]|""(?:\\.|[^\\""])*""?|;.*|[^\s\[\]{}('`"",;)]*)";
        var regex = new Regex(pattern);

        foreach (Match match in regex.Matches(input.ToString()))
        {
            var group = match.Groups[1];
            var span = group.ValueSpan;
            if (!span.IsEmpty)
            {
                var token = new Token(group.Index, group.Length);
                _tokens.Add(token);
            }
        }
    }

    public static Mal ReadStr(string input)
    {
        var reader = new Reader(input);
        return reader.ReadForm();
    }

    private Mal ReadForm()
    {
        var token = Peek();
        var input = token.Value(_input);
        return input switch
        {
            var value when value.StartsWith("(") => new Mal.List(ReadList("(", ")")),
            var value when value.StartsWith("[") => new Mal.Vector(ReadList("[", "]")),
            var value when value.StartsWith("{") => ReadHashMap("{", "}"),
            var value when value.StartsWith("\"") => ReadStringInternal(),
            var value when value.StartsWith("@") => ReaderMacroAtDeref(),
            _ => ReadSymbol()
        };
    }

    private Mal.List ReaderMacroAtDeref()
    {
        Next();
        var deref = new List<Mal> { new Mal.Symbol("deref"), ReadForm() };
        return new Mal.List(deref);
    }

    private Mal.String ReadStringInternal()
    {
        var token = Next();
        var input = token.Value(_input).ToString();
        return new Mal.String(UnEscape(input));
        return new Mal.String(input.Substring(1, input.Length-2));
    }

    private Mal.String ReadString()
    {
        var token = Next();
        var input = token.Value(_input).ToString();
        return new Mal.String(input);
        return new Mal.String(UnEscape(input));
    }

    private static string UnEscape(string input)
    {
         var value = input.Substring(1, input.Length-2)
                    //.Replace(@"\\",   "\u029e")
                    .Replace("\\\"", "\"")
                    .Replace(@"\\n",    "\n")
                    .Replace("\u029e", "\\");
         return value;
    }

    private Mal ReadSymbol()
    {
        var token = Next();
        var input = token.Value(_input);
        return input switch
        {
            "true" => Mal.True,
            "false" => Mal.False,
            "nil" => Mal.Nil,
            var value when decimal.TryParse(value, out var num) => new Mal.Number(num),
            var value => new Mal.Symbol(value.ToString())
        };
    }

    private IReadOnlyList<Mal> ReadList(string startToken, string stopToken)
    {
        var list = new List<Mal>();
        var start = Next(); // consume start-token
        var startIndex = start.Index;
        if (!start.IsEqual(_input, startToken)) throw new MalSyntaxException($"Expected {startToken} but got {start}");

        while (_position < _tokens.Count)
        {
            var token = Peek();
            if (token.IsEqual(_input, stopToken))
            {
                Next(); // consume stop-token
                return list;
            }
            else // process more items
            {
                list.Add(ReadForm());
            }
        }

        throw new MalSyntaxException($"Unmatched {startToken} at index: {startIndex}");
    }

    private Mal.HashMap ReadHashMap(string startToken, string stopToken)
    {
        var map = new Dictionary<Mal, Mal>();
        var start = Next(); // consume start-token
        var startIndex = start.Index;
        if (!start.IsEqual(_input, startToken)) throw new MalSyntaxException($"Expected {startToken} but got {start}");

        while (_position < _tokens.Count)
        {
            var token = Peek();
            if (token.IsEqual(_input, stopToken))
            {
                Next(); // consume stop-token
                return new Mal.HashMap(map);
            }
            else // process more items
            {
                // TODO: this is too broad; only string, symbol, and number values should be used as keys
                map.Add(ReadForm(), ReadForm());
            }
        }

        throw new MalSyntaxException($"Unmatched {startToken} at index: {startIndex}");
    }
}