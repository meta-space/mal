namespace csimpl;

internal class Environment
{
    private Environment? _outer = null;
    private IDictionary<MalValue.Symbol, MalValue> _data = new Dictionary<MalValue.Symbol, MalValue>();

    public Environment(Environment? env = null)
    {
        _outer = env;
    }

    internal MalValue Get(MalValue.Symbol symbol)
    {
        var env = Find(symbol);
        if (env is null)
        {
            throw new MalLookupException("symbol lookup error " + symbol);
        }

        return env._data[symbol];
    }

    private Environment? Find(MalValue.Symbol symbol)
    {
        return _data.ContainsKey(symbol) ? this : _outer?.Find(symbol);
    }

    internal void Set(MalValue.Symbol symbol, MalValue value)
    {
        _data[symbol] = value;
    }

    internal void Set(string symbol, MalValue value)
    {
        Set(new MalValue.Symbol(symbol), value);
    }

}