using System.Collections;

namespace csimpl;

record class Mal
{
    public interface ISequence : IReadOnlyList<Mal> {}

    public sealed record List(IList<Mal> Items) : Mal, ISequence
    {
        public int Count => Items.Count;
        public bool IsReadOnly => Items.IsReadOnly;

        public Mal this[int index] => Items[index];
        public List Slice(int start, int length) => new(Items.Skip(start)
                                                             .Take(length)
                                                             .ToList());

        public IEnumerator<Mal> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }

        public List Map(Func<Mal, Mal> fn)
        {
            return new(Items.Select(fn).ToList());
        }

        public bool Equals(List? obj)
        {
            //Check for null and compare run-time types.
            if ((obj is null) || GetType() != obj.GetType())
            {
                return false;
            }
            List other = (List)obj;
            return Enumerable.SequenceEqual(Items, other.Items);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Items);
        }
    }

    public record Vector(IList<Mal> Items) : Mal, ISequence
    {
        public int Count => Items.Count;
        public bool IsReadOnly => Items.IsReadOnly;

        public Mal this[int index] => Items[index];
        public Vector Slice(int start, int length) => new(Items.Skip(start)
                                                             .Take(length)
                                                             .ToList());

        public IEnumerator<Mal> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }
        public Vector Map(Func<Mal, Mal> fn)
        {
            return new(Items.Select(fn).ToList());
        }

    }

    public record HashMap(IReadOnlyDictionary<Mal, Mal> Items) : Mal, IReadOnlyDictionary<Mal, Mal>
    {
        public int Count => Items.Count;

        public bool ContainsKey(Mal key)
        {
            return Items.ContainsKey(key);
        }

        public bool TryGetValue(Mal key, out Mal value)
        {
            return Items.TryGetValue(key, out value);
        }

        public Mal this[Mal key] => Items[key];

        public IEnumerable<Mal> Keys => Items.Keys;

        public IEnumerable<Mal> Values => Items.Values;

        public IEnumerator<KeyValuePair<Mal, Mal>> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }
        public HashMap Map(Func<KeyValuePair<Mal, Mal>, Mal> keySelector, Func<KeyValuePair<Mal, Mal>, Mal> elementSelector)
        {
            return new(Items.ToDictionary(keySelector, elementSelector));
        }
    }
    public record Symbol(string Value) : Mal;

    public record Constant(string Value) : Symbol(Value);

    public record String(string Value) : Mal, IEnumerable<char>
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

    public static readonly Constant Nil = new("nil");
    public static readonly Constant True = new("true");
    public static readonly Constant False = new("false");

    public record Number(decimal Value) : Mal
    {
        public static Constant operator <(Number a, Number b)
        {
            return a.Value < b.Value ? True : False;
        }
        public static Constant operator <=(Number a, Number b)
        {
            return a.Value <= b.Value ? True : False;
        }
        public static Constant operator >(Number a, Number b)
        {
            return a.Value > b.Value ? True : False;
        }
        public static Constant operator >=(Number a, Number b)
        {
            return a.Value >= b.Value ? True : False;
        }
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

    public record Function(Func<List, Mal> Op) : Mal;

    private Mal() { } // private constructor can prevent derived cases from being defined elsewhere
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