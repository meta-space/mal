﻿namespace csimpl;

internal class Core
{
    private static readonly Mal.Function ToList =  new(args => new Mal.List(args.Items));
    private static readonly Mal.Function IsList =  new(args => args[0] is Mal.ISequence ? Mal.True : Mal.Nil);
    private static readonly Mal.Function IsEmpty = new(args => args is [Mal.ISequence { Count: 0}] ? Mal.True : Mal.False);
    private static readonly Mal.Function Count =   new(args => new Mal.Number(args is [Mal.ISequence seq] ? seq.Count : 0));
    private static readonly Mal.Function IsEqual = new(args => args[0] == args[1] ? Mal.True : Mal.False);
    private static readonly Mal.Function IsLess =  new(args => (args[0] is Mal.Number a && args[1] is Mal.Number b) ? a < b : Mal.False);
    private static readonly Mal.Function IsLessEqual = new( args => (args[0] is Mal.Number a && args[1] is Mal.Number b) ? a <= b : Mal.False);
    private static readonly Mal.Function IsGreater = new( args => (args[0] is Mal.Number a && args[1] is Mal.Number b) ? a > b : Mal.False);
    private static readonly Mal.Function IsGreaterEqual = new( args => (args[0] is Mal.Number a && args[1] is Mal.Number b) ? a >= b : Mal.False);
    private static readonly Mal.Function PrStr = new(args => Printer.PrnStr(args, " ", true)); 
    private static readonly Mal.Function Str = new(args =>  Printer.PrnStr(args, "", false));
    private static readonly Mal.Function Prn = new(args => {
        var str = Printer.PrnStr(args, " ", true);
        Console.WriteLine(str.Value);
        return Mal.Nil;
    });
    private static readonly Mal.Function PrintLn = new(args => {
        var str = Printer.PrnStr(args, " ", false);
        Console.WriteLine(str.Value);
        return Mal.Nil;
    });
    private static readonly Mal.Function Slurp = new(args => args[0] is Mal.String str ? new Mal.String(File.ReadAllText(str.Value)) : Mal.Nil);
    private static readonly Mal.Function ReadString = new(args => args[0] is Mal.String str ? Reader.ReadStr(str.Value) : Mal.Nil);

    private static readonly Dictionary<string, Mal.Function> Ns = new ()
    {
        { "+", new Mal.Function(args => (Mal.Number)args[0] + (Mal.Number)args[1]) },
        { "-", new Mal.Function(args => (Mal.Number)args[0] - (Mal.Number)args[1]) },
        { "*", new Mal.Function(args => (Mal.Number)args[0] * (Mal.Number)args[1]) },
        { "/", new Mal.Function(args => (Mal.Number)args[0] / (Mal.Number)args[1]) },
        { "pr-str", PrStr},
        { "str", Str },
        { "prn", Prn },
        { "println", PrintLn },
        { "list", ToList},
        { "list?", IsList },
        { "empty?", IsEmpty },
        { "count", Count },
        { "=", IsEqual },
        { "<", IsLess },
        { "<=", IsLessEqual },
        { ">", IsGreater },
        { ">=", IsGreaterEqual },
        { "slurp", Slurp },
        { "read-string", ReadString },
    };

    internal static void Init(Environment env)
    {
        foreach (var keyValuePair in Ns)
        {
           env.Set(keyValuePair.Key, keyValuePair.Value);   
        }
        // finally: eval
        env.Set("eval", new Mal.Function(args => Evaluator.Eval(args, env)));
    }
}