using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace csimpl;

readonly struct Token
{
    public readonly int Position;
    public readonly int Length;

    public Token(int position, int length)
    {
        Position = position;
        Length = length;
    }

    public ReadOnlySpan<char> Value(ReadOnlySpan<char> input)
    {
        return input.Slice(Position, Length).Trim();
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
            foreach (Group group in match.Groups)
            {
                var span = group.ValueSpan;
                if (!span.IsEmpty && group.Name == "1")
                {
                    var token = new Token(group.Index, group.Length);
                    _tokens.Add(token);
                }
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
        switch (token.Value(_input))
        {
            case "(":
                return ReadList(")");
            case "[":
                return ReadList("]");
            case "{":
                return ReadList("}");
            default:
                return ReadAtom();
        }
    }

    private MalValue ReadAtom()
    {
        var token = Next();
        if (decimal.TryParse(token.Value(_input), out var value))
        {
            return new MalValue.Number(value);
        }
        else
        {
            return new MalValue.Atom(token.Value(_input).ToString());
        }

    }

    private MalValue ReadList(string stop)
    {
        IList<MalValue> values = new List<MalValue>();
        Next(); // consume "(" token
        while (_position < _tokens.Count)
        {
            var token = Peek();
            if (token.Value(_input).CompareTo(stop, StringComparison.InvariantCulture) == 0)
            {
                Next(); // consume ")" token
                return new MalValue.List(values);
            }
            else // process more items
            {
                values.Add(ReadForm());
            }
        }

        return new MalValue.Error($"unmatched {stop} at position", _position);
    }
}