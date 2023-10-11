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
        return input.Slice(Position, Length);
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
        string pattern = @"[\s,]*(~@|[\[\]{}()'`~^@]|""(?:\\.|[^\\\""])*""?|;.*|[^\s\[\]{}('"",;)]*)";
        var regex = new Regex(pattern);

        // Enumerate the matches in the ReadOnlySpan.
        foreach (var match in regex.EnumerateMatches(input))
        {
            // Do something with the match.
            //var value = text.Slice(match.Index, match.Length);
            //Console.WriteLine($"'{value.ToString()}'");
            _tokens.Add(new Token(match.Index, match.Length));
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
                return ReadList();
            default:
                return ReadAtom();
        }
    }

    private MalValue ReadList()
    {
        IList<MalValue> values = new List<MalValue>();
        Next(); // consume "(" token
        while (_position < _tokens.Count)
        {
            var token = Peek();
            if (token.Value(_input) != ")")
            {
                values.Add(ReadForm);
            }

        }

        return new MalValue.List(values);
    }
}