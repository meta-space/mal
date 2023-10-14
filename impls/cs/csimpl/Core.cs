namespace csimpl;

internal class Core
{
    private static readonly Mal.Function Print = new(args => { 
                Console.WriteLine(Printer.Prn_Str(args, " ", true));
                return Mal.Nil;
            });

    private static readonly Mal.Function ToList =  new(args => new Mal.List(args.Items));
    private static readonly Mal.Function IsList =  new(args => args[0] is Mal.ISequence ? Mal.True : Mal.Nil);
    private static readonly Mal.Function IsEmpty = new(args => args is [Mal.ISequence { Count: > 0}] ? Mal.True : Mal.False);
    private static readonly Mal.Function Count =   new(args => new Mal.Number(args is [Mal.ISequence seq] ? seq.Count : 0));
    private static readonly Mal.Function IsEqual = new(args => args[0] == args[1] ? Mal.True : Mal.False);
    private static readonly Mal.Function IsLess =  new(args => (args[0] is Mal.Number a && args[1] is Mal.Number b) ? a < b : Mal.False);
    private static readonly Mal.Function IsLessEqual = new( args => (args[0] is Mal.Number a && args[1] is Mal.Number b) ? a <= b : Mal.False);
    private static readonly Mal.Function IsGreater = new( args => (args[0] is Mal.Number a && args[1] is Mal.Number b) ? a > b : Mal.False);
    private static readonly Mal.Function IsGreaterEqual = new( args => (args[0] is Mal.Number a && args[1] is Mal.Number b) ? a >= b : Mal.False);

    private static readonly Dictionary<string, Mal.Function> Ns = new ()
    {
        { "+", new Mal.Function(args => (Mal.Number)args[0] + (Mal.Number)args[1]) },
        { "-", new Mal.Function(args => (Mal.Number)args[0] - (Mal.Number)args[1]) },
        { "*", new Mal.Function(args => (Mal.Number)args[0] * (Mal.Number)args[1]) },
        { "/", new Mal.Function(args => (Mal.Number)args[0] / (Mal.Number)args[1]) },
        { "prn", Print },
        { "list", ToList},
        { "list?", IsList },
        { "empty?", IsEmpty },
        { "count", Count },
        { "=", IsEqual },
        { "<", IsLess },
        { "<=", IsLessEqual },
        { ">", IsGreater },
        { ">=", IsGreaterEqual },
    };

    internal static void Init(Environment env)
    {
        foreach (var keyValuePair in Ns)
        {
           env.Set(keyValuePair.Key, keyValuePair.Value);   
        }
    }
}