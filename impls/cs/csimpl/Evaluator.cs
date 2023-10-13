namespace csimpl;

internal class Evaluator
{
    private static MalValue EvalAst(MalValue ast, Environment env)
    {
        return ast switch
        {
            MalValue.Constant constant => constant,
            MalValue.Symbol symbol => env.Get(symbol),
            MalValue.List list => list.Map(i => Eval(i, env)),
            MalValue.Vector vec => vec.Map(i => Eval(i, env)),
            MalValue.HashMap dict => dict.Map(kvp => kvp.Key, kvp => Eval(kvp.Value, env)),
            var any => any 
        };
    }

    internal static MalValue Eval(MalValue ast, Environment env)
    {
        return ast switch
        {
            MalValue.List { Items.Count: 0 } => ast,
            MalValue.List items => Apply(items, env),
            MalValue.Vector vec => EvalAst(vec, env),
            MalValue.HashMap map => EvalAst(map, env),
            var anyOther => EvalAst(anyOther, env)
        };
    }

    private static MalValue Apply(MalValue.List items, Environment env)
    {
        var symbol = (MalValue.Symbol)(items[0]);
        switch (symbol.Value) {
            case "def!":
                var name = (MalValue.Symbol)items[1];
                var value = Eval(items[2], env);
                env.Set(name, value);
                return value;
            case "let*":
                var a1 = (IReadOnlyList<MalValue>)items[1];
                var a2 = items[2];
                Environment let_env = new(env);
                for(int i=0; i<a1.Count; i+=2) {
                    var key = (MalValue.Symbol)a1[i];
                    var val = a1[i+1];
                    let_env.Set(key, Eval(val, let_env));
                }
                return Eval(a2, let_env);
            default: // function call
            var fn = (MalValue.Function)env.Get(symbol);
            var result = fn.Op((MalValue.List)EvalAst(items[1..], env));
            return result;
        }
    }
}