
namespace csimpl;

internal class Program
{
    static MalValue Read()
    {
        string input = ReadLine.Read("user> ");
        return Reader.ReadStr(input);
    }

    static MalValue Eval(MalValue ast, Environment env)
    {
        return Evaluator.Eval(ast, env);
    }

    static string Print(MalValue value)
    {
       return Printer.Print(value);
    }

    static void Main(string[] _)
    {
        ReadLine.HistoryEnabled = true;
        // Get command history
        ReadLine.GetHistory();

        var env = new Environment();
        env.Set("+", new MalValue.Function(args => (MalValue.Number)args[0] + (MalValue.Number)args[1]));
        env.Set("-", new MalValue.Function(args => (MalValue.Number)args[0] - (MalValue.Number)args[1]));
        env.Set("*", new MalValue.Function(args => (MalValue.Number)args[0] * (MalValue.Number)args[1]));
        env.Set("/", new MalValue.Function(args => (MalValue.Number)args[0] / (MalValue.Number)args[1]));

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