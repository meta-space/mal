namespace csimpl;

internal class Evaluator
{
    internal static Mal Eval(Mal ast, Environment env)
    {
        return ast switch
        {
            Mal.List { Items.Count: 0 } => ast,
            Mal.List items => Apply(items, env),
            Mal.Vector vec => EvalAst(vec, env),
            Mal.HashMap map => EvalAst(map, env),
            var anyOther => EvalAst(anyOther, env)
        };
    }

    private static Mal EvalAst(Mal ast, Environment env)
    {
        return ast switch
        {
            Mal.Constant constant => constant,
            Mal.Symbol symbol => env.Get(symbol),
            Mal.List list => list.Map(i => Eval(i, env)),
            Mal.Vector vec => vec.Map(i => Eval(i, env)),
            Mal.HashMap dict => dict.Map(kvp => kvp.Key, kvp => Eval(kvp.Value, env)),
            var any => any 
        };
    }

    private static Mal Apply(Mal.List ast, Environment env)
    {
        if (ast[0] is Mal.List list)
        {
            var lambda = Apply(list, env) as Mal.Function;
            return lambda.Op((Mal.List)EvalAst(ast[1..], env));
        }

        var symbol = (Mal.Symbol)(ast[0]);
        switch (symbol.Value) {
            case "def!":
                var name = (Mal.Symbol)ast[1];
                var value = Eval(ast[2], env);
                env.Set(name, value);
                return value;
            case "let*":
                var lets = (Mal.ISequence)ast[1];
                var final = ast[2];
                Environment letEnv = new(env);
                for(int i=0; i<lets.Count; i+=2) {
                    var key = (Mal.Symbol)lets[i];
                    var val = lets[i+1];
                    letEnv.Set(key, Eval(val, letEnv));
                }
                return Eval(final, letEnv);
            case "do":
                var dos = (Mal.ISequence)ast[1..];
                return dos.Select(s => Eval(s, env)).Last();
            case "if":
                var condition = ast[1];
                var consequent = ast[2];
                var alternative = (ast.Count > 3) ? ast[3] : Mal.Nil;
                var res = Eval(condition, env);
                var isTrue = !(res == Mal.Nil || res == Mal.False);
                return Eval(isTrue ? consequent : alternative, env); 
            case "fn*":
                var binds = (Mal.List)ast[1];
                var body = ast[2];
                return new Mal.Function(args =>  Eval(body, new Environment(env, binds.OfType<Mal.Symbol>().ToList(), args)));
                
            default: // function call
                var fn = (Mal.Function)env.Get(symbol);
                var result = fn.Op((Mal.List)EvalAst(ast[1..], env));
                return result;
        }
    }
}