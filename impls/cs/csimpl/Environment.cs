namespace csimpl;

internal class Environment
{
    private IDictionary<string, MalValue> _env = new Dictionary<string, MalValue>
    {
        { "+", new MalValue.Function((args) => (MalValue.Number)args[0] + (MalValue.Number)args[1]) },
        { "-", new MalValue.Function((args) => (MalValue.Number)args[0] - (MalValue.Number)args[1]) },
        { "*", new MalValue.Function((args) => (MalValue.Number)args[0] * (MalValue.Number)args[1]) },
        { "/", new MalValue.Function((args) => (MalValue.Number)args[0] / (MalValue.Number)args[1]) },
    };

    internal MalValue Lookup(string symbol)
    {
        if (_env.TryGetValue(symbol, out var value))
        {
            return value;
        }

        throw new MalLookupException("symbol lookup error " + symbol);
    }

}