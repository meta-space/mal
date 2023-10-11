
namespace csimpl;

internal class Program
{
    static string Read()
    {
        return ReadLine.Read("user> ");
    }

    static string Eval(string input)
    {
        return input;
    }

    static void Print(string input)
    {
        Console.WriteLine(input);
    }

    static void Main(string[] args)
    {
        // Get command history
        ReadLine.GetHistory();

        // Add command to history
        ReadLine.AddHistory("dotnet run");

        // Clear history
        ReadLine.ClearHistory();

        while (true)
        {
            Print(Eval(Read()));
        }
    }
}