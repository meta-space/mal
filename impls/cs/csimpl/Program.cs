
namespace csimpl;

internal class Program
{
    static MalValue Read()
    {
        string input = ReadLine.Read("user> ");
        return Reader.ReadStr(input);
    }

    static MalValue Eval(MalValue value)
    {
        return value;
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

        while (true)
        {
            Console.WriteLine(Print(Eval(Read())));
        }

        Console.WriteLine("exiting");
    }
}