
namespace csimpl;


record MalValue{
    public record List(IList<MalValue> Items) : MalValue;
    public record Atom(string Symbol) : MalValue;
    public record Number(decimal Value) : MalValue;


    private MalValue(){} // private constructor can prevent derived cases from being defined elsewhere
}