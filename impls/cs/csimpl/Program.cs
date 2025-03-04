
namespace csimpl;

internal class Program
{
    static Mal Read()
    {
        string input = ReadLine.Read("user> ");
        input = input.TrimStart('\0');
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

        Evaluator.Eval(Reader.ReadStr(@"
                (def! not (fn* (a) 
                            (if a false true)))"), env);
        Evaluator.Eval(Reader.ReadStr(@"
                (def! load-file (fn* (f) 
                                  (eval (read-string (str ""(do "" (slurp f) "" \n nil)"")))))" ), env);
        Evaluator.Eval(Reader.ReadStr(@"
                (def! swap! (fn* (atom mutate & rest)
                                  (let* (value (deref atom))
                                     (if value (mutate value rest))))))"), env);

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