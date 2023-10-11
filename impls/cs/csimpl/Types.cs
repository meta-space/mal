
namespace csimpl;

record class MalValue
{
    public record List(IList<MalValue> Items) : MalValue;
    public record Vector(IList<MalValue> Items) : MalValue;
    public record HashMap(IDictionary<MalValue, MalValue> Items) : MalValue;
    public record Atom(string Symbol) : MalValue;
    public record Number(decimal Value) : MalValue;
    public record Error(string Message, int Position) : MalValue;

    private MalValue() { } // private constructor can prevent derived cases from being defined elsewhere
}