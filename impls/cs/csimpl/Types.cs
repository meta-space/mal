
using System.Collections;
using Microsoft.VisualBasic.CompilerServices;

namespace csimpl;

record class MalValue
{
    public record List(IList<MalValue> Items) : MalValue, IReadOnlyCollection<MalValue>
    {

        public int Count => Items.Count;
        public bool IsReadOnly => Items.IsReadOnly;

        public MalValue this[int index] => Items[index];
        public List Slice(int start, int length)
  => new List(Items
         .Skip(start)
         .Take(length)
         .ToList());

        public IEnumerator<MalValue> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }
    }
    public record Vector(IList<MalValue> Items) : MalValue;
    public record HashMap(IDictionary<MalValue, MalValue> Items) : MalValue;
    public record Atom(string Symbol) : MalValue;

    public record Constant(string Name) : MalValue;

    public record Number(decimal Value) : MalValue
    {
            //public static Constant operator <(Number a, Number b) {
            //    return a.Value < b.Value ? True : False;
            //}
            //public static Constant operator <=(Number a, Number b) {
            //    return a.Value <= b.Value ? True : False;
            //}
            //public static Constant operator >(Number a, Number b) {
            //    return a.Value > b.Value ? True : False;
            //}
            //public static Constant operator >=(Number a, Number b) {
            //    return a.Value >= b.Value ? True : False;
            //}
            public static Number operator +(Number a, Number b) {
                return new Number(a.Value + b.Value);
            }
            public static Number operator -(Number a, Number b) {
                return new Number(a.Value - b.Value);
            }
            public static Number operator *(Number a, Number b) {
                return new Number(a.Value * b.Value);
            }
            public static Number operator /(Number a, Number b) {
                return new Number(a.Value / b.Value);
            }
    }

    public record Function(Func<List, MalValue> Op) : MalValue
    {
    }



    private MalValue() { } // private constructor can prevent derived cases from being defined elsewhere

}

public class MalSyntaxException : Exception
{
    public MalSyntaxException(string message) : base(message)
    {
    }
}

public class MalLookupException : Exception
{
    public MalLookupException(string message) : base(message)
    {
    }
}