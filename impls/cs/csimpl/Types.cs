
using System.Collections;

namespace csimpl;

record class MalValue
{
    public record List(IList<MalValue> Items) : MalValue, IReadOnlyList<MalValue>
    {

        public int Count => Items.Count;
        public bool IsReadOnly => Items.IsReadOnly;

        public MalValue this[int index] => Items[index];
        public List Slice(int start, int length) => new(Items.Skip(start)
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

        public List Map(Func<MalValue, MalValue> fn)
        {
            return new (Items.Select(fn).ToList());
        }
    }

    public record Vector(IList<MalValue> Items) : MalValue, IReadOnlyList<MalValue>
    {
        public int Count => Items.Count;
        public bool IsReadOnly => Items.IsReadOnly;

        public MalValue this[int index] => Items[index];
        public Vector Slice(int start, int length) => new(Items.Skip(start)
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
        public Vector Map(Func<MalValue, MalValue> fn)
        {
            return new (Items.Select(fn).ToList());
        }
    }

    public record HashMap(IReadOnlyDictionary<MalValue, MalValue> Items) : MalValue, IReadOnlyDictionary<MalValue, MalValue>
    {
        public int Count => Items.Count;

        public bool ContainsKey(MalValue key)
        {
            return Items.ContainsKey(key);
        }

        public bool TryGetValue(MalValue key, out MalValue value)
        {
            return Items.TryGetValue(key, out value);
        }

        public MalValue this[MalValue key] => Items[key];

        public IEnumerable<MalValue> Keys => Items.Keys;

        public IEnumerable<MalValue> Values => Items.Values;

        public IEnumerator<KeyValuePair<MalValue, MalValue>> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }
        public HashMap Map(Func<KeyValuePair<MalValue, MalValue>, MalValue> keySelector, Func<KeyValuePair<MalValue, MalValue>, MalValue> elementSelector)
        {
            return new (Items.ToDictionary(keySelector, elementSelector));
        }
    }
    public record Symbol(string Value) : MalValue;

    public record Constant(string Value) : Symbol(Value);

    public record String(string Value) : MalValue, IEnumerable<char>
    {
        public IEnumerator<char> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Value).GetEnumerator();
        }
    }

    public static readonly Constant Nil = new Constant("nil");
    public static readonly Constant True = new Constant("true");
    public static readonly Constant False = new Constant("false");

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
        public static Number operator +(Number a, Number b)
        {
            return new Number(a.Value + b.Value);
        }
        public static Number operator -(Number a, Number b)
        {
            return new Number(a.Value - b.Value);
        }
        public static Number operator *(Number a, Number b)
        {
            return new Number(a.Value * b.Value);
        }
        public static Number operator /(Number a, Number b)
        {
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