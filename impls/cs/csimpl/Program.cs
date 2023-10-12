
namespace csimpl;

internal class Program
{
    static MalValue Read()
    {
        string input = ReadLine.Read("user> ");
        return Reader.ReadStr(input);
    }

    static MalValue Eval(MalValue input, Environment env)
    {
        return input switch
        {
            MalValue.List { Items.Count: 0 } => input,
            MalValue.List items => Apply(items, env),
            MalValue.Vector vect => EvalAst(vect, env),
            MalValue.HashMap map => EvalAst(map, env),
            //var anyOther => EvalAst(anyOther, env)
            var any => any 
        };
    }

    private static MalValue Apply(MalValue.List items, Environment env)
    {
        var atom = (MalValue.Atom)(items[0]);
        var fn = (MalValue.Function)env.Lookup(atom.Symbol);
        var result = fn.Op((MalValue.List)EvalAst(items[1..], env));
        return result;
    }

    private static MalValue EvalAst(MalValue input, Environment env)
    {
        return input switch
        {
            MalValue.Atom atom => env.Lookup(atom.Symbol),
            MalValue.List list => new MalValue.List(list.Select(i => Eval(i, env)).ToList()),
            MalValue.Vector vect => new MalValue.Vector(vect.Items.Select(i => Eval(i, env)).ToList()),
            MalValue.HashMap map => new MalValue.HashMap(map.Items.ToDictionary(kvp => kvp.Key, kvp => Eval(kvp.Value, env))),
            var any => any 
        };
    }

    static string Print(MalValue value)
    {
       return Printer.Print(value);
    }

    static void Main(string[] args)
    {
        ReadLine.HistoryEnabled = true;
        // Get command history
        ReadLine.GetHistory();

        var env = new Environment();

        while (true)
        {
            try
            {
                Console.WriteLine(Print(Eval(Read(), env)));
            }
            catch (MalSyntaxException se)
            {
                Console.WriteLine(se.Message);
            }
            catch (MalLookupException le)
            {
                Console.WriteLine(le.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Internal exception: \n{e}");
            }
        }

        Console.WriteLine("exiting");
    }
}