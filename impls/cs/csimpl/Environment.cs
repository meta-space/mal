
namespace csimpl;

internal class Environment
{
    private Environment? _outer = null;
    private IDictionary<Mal.Symbol, Mal> _data = new Dictionary<Mal.Symbol, Mal>();

    public Environment(Environment? outer = null)
    {
        _outer = outer;
    }

    public Environment(Environment env, IReadOnlyList<Mal.Symbol> binds, IReadOnlyList<Mal> exprs)
    {
        _outer = env;
        for (var i = 0; i < binds.Count; i++)
        {
            Set(binds[i], exprs[i]);
        }

    }

    internal Mal Get(Mal.Symbol symbol)
    {
        var env = Find(symbol);
        if (env is null)
        {
            throw new MalLookupException("symbol lookup error " + symbol);
        }

        return env._data[symbol];
    }

    private Environment? Find(Mal.Symbol symbol)
    {
        return _data.ContainsKey(symbol) ? this : _outer?.Find(symbol);
    }

    internal void Set(Mal.Symbol symbol, Mal value)
    {
        _data[symbol] = value;
    }

    internal void Set(string symbol, Mal value)
    {
        Set(new Mal.Symbol(symbol), value);
    }

}