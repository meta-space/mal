namespace csimpl;

internal class Evaluator
{
    internal static Mal Eval(Mal ast, Environment env)
    {
        while (true)
        {
            switch (ast)
            {
                default: // i.e. not a list
                    return EvalAst(ast, env);
                case Mal.List { Items.Count: 0 }:
                    return ast;
                case Mal.List items:

                    var symbolName = (items[0] is Mal.Symbol symbol) ? symbol.Value : "___<fn*>___";
                    switch (symbolName)
                    {
                        case "def!":
                            var name = (Mal.Symbol)items[1];
                            var value = Eval(items[2], env);
                            env.Set(name, value);
                            return value;
                        case "let*":
                            var lets = (Mal.ISequence)items[1];
                            var final = items[2];
                            Environment letEnv = new(env);
                            for (int i = 0; i < lets.Count; i += 2)
                            {
                                var key = (Mal.Symbol)lets[i];
                                var val = lets[i + 1];
                                letEnv.Set(key, Eval(val, letEnv));
                            }
                            env = letEnv;
                            ast = final;
                            continue;
                        case "do":
                            var statements = items[1..^1];
                            statements.Select(s => Eval(s, env));
                            ast = statements[^1];
                            continue;
                        case "if":
                            var condition = items[1];
                            var consequent = items[2];
                            var alternative = (items.Count > 3) ? items[3] : Mal.Nil;
                            var res = Eval(condition, env);
                            var isTrue = !(res == Mal.Nil || res == Mal.False);
                            ast = isTrue ? consequent : alternative;
                            continue;
                        case "fn*":
                            var param = (Mal.List)items[1];
                            var body = items[2];
                            ast = new Mal.Closure(param, body, env, args => Mal.Nil);
                            continue;
                        default: // function call
                            var fn = EvalAst(items[0], env);
                            var args = (Mal.List)EvalAst(items[1..], env);

                            if (fn is Mal.Closure closure)
                            {
                                ast = closure.Ast;
                                var fparams = closure.Parameters.OfType<Mal.Symbol>().ToList();
                                env = new Environment(closure.Env, fparams, args);
                                continue;
                            }
                            if (fn is Mal.Function function)
                            {
                                var result = function.Op(args);
                                return result;
                            }

                            throw new MalRuntimeException("off the rails " + symbolName);
                    }

            }
        }
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
            _ => ast
        };
    }
}