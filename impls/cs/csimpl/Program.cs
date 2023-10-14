
namespace csimpl;

internal class Program
{
    static Mal Read()
    {
        string input = ReadLine.Read("user> ");
        return Reader.ReadStr(input);
    }

    static Mal Eval(Mal ast, Environment env)
    {
        return Evaluator.Eval(ast, env);
    }

    static string Print(Mal value)
    {
       return Printer.Print(value);
    }

    static void Main(string[] _)
    {
        ReadLine.HistoryEnabled = true;
        // Get command history
        ReadLine.GetHistory();

        var env = new Environment();
        Core.Init(env);

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