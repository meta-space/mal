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

    public Reader(ReadOnlySpan<char> input)
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

    public static MalValue ReadStr(string input)
    {
        var reader = new Reader(input);
        return reader.ReadForm();
    }

    private MalValue ReadForm()
    {
        var token = Peek();
        return token.Value(_input) switch
        {
            var input when input.StartsWith("(") => new MalValue.List(ReadList("(", ")")),
            var input when input.StartsWith("[") => new MalValue.Vector(ReadList("[", "]")),
            var input when input.StartsWith("{") => new MalValue.HashMap(ReadHashMap("{", "}")),
            var input when input.StartsWith("\"") =>new MalValue.String(input.ToString()),
            var input when decimal.TryParse(input, out var num) => new MalValue.Number(num),
            _ => ReadSymbol()
        };
    }

    private MalValue ReadSymbol()
    {
        var token = Next();
        return token.Value(_input) switch
        {
            "true" => MalValue.True,
            "false" => MalValue.False,
            "nil" => MalValue.Nil,
            var input => new MalValue.Symbol(input.ToString())
        }; 
    }

    private IList<MalValue> ReadList(string startToken, string stopToken)
    {
        var list = new List<MalValue>();
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

    private IReadOnlyDictionary<MalValue,MalValue> ReadHashMap(string startToken, string stopToken)
    {
        var map = new Dictionary<MalValue, MalValue>();
        var start = Next(); // consume start-token
        var startIndex = start.Index;
        if (!start.IsEqual(_input, startToken)) throw new MalSyntaxException($"Expected {startToken} but got {start}");

        while (_position < _tokens.Count)
        {
            var token = Peek();
            if (token.IsEqual(_input, stopToken))
            {
                Next(); // consume stop-token
                return map;
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