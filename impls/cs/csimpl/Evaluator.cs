using static csimpl.Mal;

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
                        case "def!": {
                            var name = (Mal.Symbol)items[1];
                            var value = Eval(items[2], env);
                            env.Set(name, value);
                            return value;
                        }
                        case "defmacro!": {
                            var name = (Mal.Symbol)items[1];
                            var value = Eval(items[2], env) with { Meta = True };
                            env.Set(name, value);
                            return value;
                        }
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
                        case "quote":
                            return items[1];
                        case "quasiquote":
                            ast = Quasiquote(items[1]);
                            continue;
                        case "do":
                            var statements = items[1..].Select(s => Eval(s, env)).ToList();
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
                            var param = (Mal.ISequence)items[1];
                            var body = items[2];
                            ast = new Mal.Closure(param, body, env, args => Eval(body, new Environment(env, param?.OfType<Symbol>().ToList() ?? new List<Symbol>(), args)));
                            continue;
                        default: // function call
                            var fn = Eval(items[0], env);
                            bool isMacro = fn?.Meta == Mal.True;

                            if (isMacro && fn is Mal.Closure fun)
                            {
                                var xargs = new Mal.List(items[1..].ToList());
                                //var fparams = fun.Parameters.OfType<Mal.Symbol>().ToList();
                                //env = new Environment(fun.Env, fparams, args);
                                //ast = Eval(fun.Ast, env);
                                //var xargs = (Mal.ISequence)EvalAst(items[1..], env);

                                ast = fun.Op(xargs);
                                continue;
                            }
                            
                            var args = (Mal.ISequence)EvalAst(items[1..], env);
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
                            

                            throw new MalRuntimeException("malformed function invocation " + fn);
                    }
            }
        }
    }


    //private Mal QQ_Expand(Mal ast)
    //{
    //    if (StartsWith(ast, "unquote")) // backquote
    //    {
    //        return ast[1];
    //    }

    //    if (StartsWith(ast, "splice-unquote")) // comma-at-sign ,@
    //    {
    //        throw new MalRuntimeException("mal-formed splice-unquote");
    //    }

    //    if (StartsWith(ast, "backquote"))
    //    {
    //        return QQ_Expand(QQ_Expand(ast[1]));
    //    }

    //    if (ast is Mal.List list and 
    //            list is [Mal.Symbol car, Mal.List cdr])
    //    {
    //        return new Mal.List(new Mal.Symbol("append"), QQ_Expand(car), QQ_Expand(cdr));
    //    }
    //    else
    //}


    // eval
    private static bool StartsWith(Mal ast, string sym)
    {
        if (ast is Mal.List && !(ast is Mal.Vector))
        {
            var list = (Mal.List)ast;
            if (list.Count == 2 && list[0] is Mal.Symbol)
            {
                Mal.Symbol a0 = (Mal.Symbol)list[0];
                return a0.Value == sym;
            }
        }
        return false;
    }

    private static Mal QQ_Loop(Mal.List ast)
    {
        var acc = new Mal.List();
        for (int i = ast.Count - 1; 0 <= i; i -= 1)
        {
            Mal elt = ast[i];
            if (StartsWith(elt, "splice-unquote"))
            {
                acc = new Mal.List(new Mal.Symbol("concat"), ((Mal.List)elt)[1], acc);
            }
            else
            {
                acc = new Mal.List(new Mal.Symbol("cons"), Quasiquote(elt), acc);
            }
        }
        return acc;
    }

    private static Mal Quasiquote(Mal ast)
    {
        //  Check Vector subclass before List.
        if (ast is Mal.Vector)
        {
            return new Mal.List(new Mal.Symbol("vec"), QQ_Loop(((Mal.List)ast)));
        }
        else if (StartsWith(ast, "unquote"))
        {
            return ((Mal.List)ast)[1];
        }
        else if (ast is Mal.List)
        {
            return QQ_Loop((Mal.List)ast);
        }
        else if (ast is Mal.Symbol || ast is Mal.HashMap)
        {
            return new Mal.List(new Mal.Symbol("quote"), ast);
        }
        else
        {
            return ast;
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