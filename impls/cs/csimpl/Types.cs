using System.Collections;
using System.Diagnostics;

namespace csimpl;

record class Mal
{
    public Mal? Meta = null;

    public interface ISequence : IReadOnlyList<Mal>
    {
       // IReadOnlyList<Mal> Items => this;
    }

    [DebuggerDisplay("({Items})")]
    public sealed record List(IReadOnlyList<Mal> Items) : Mal, ISequence
    {

        public List(params Mal[] items) : this(items.ToList())
        {
        }

        public int Count => Items.Count;

        public Mal this[int index] => Items[index];

        public List Slice(int start, int length) => new(this.Skip(start)
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
            List other = obj;
            return Enumerable.SequenceEqual(Items, other.Items);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Items);
        }
    }

    public sealed record Vector(IReadOnlyList<Mal> Items) : Mal, ISequence
    {
        public int Count => Items.Count;

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
        
        
        public bool Equals(Vector? obj)
        {
            //Check for null and compare run-time types.
            if ((obj is null) || GetType() != obj.GetType())
            {
                return false;
            }
            Vector other = obj;
            return Enumerable.SequenceEqual(Items, other.Items);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Items);
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

    [DebuggerDisplay("Symbol: {Value}")]
    public record Symbol(string Value) : Mal;


    [DebuggerDisplay("Keyword: {}")]
    public record Keyword(string Value) : Mal;


    [DebuggerDisplay("Constant: {Value}")]
    public record Constant(string Value) : Symbol(Value);

    [DebuggerDisplay("\"{Value}\"")]
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

    internal class Ref
    {
        internal Ref(Mal value)
        {
            Value = value;
        }

        internal Mal Value { get; set; }
    }

    [DebuggerDisplay("<{Value}>")]
    public  record Atom(Ref R) : Mal
    {
        public Mal Value
        {
            get => R.Value;
            set => R.Value = value;
        } 
    }

    public static readonly Constant Nil = new("nil");
    public static readonly Constant True = new("true");
    public static readonly Constant False = new("false");

    [DebuggerDisplay("{Value}")]
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

    public record Function(Func<ISequence, Mal> Op) : Mal // Op should be called Apply
    {
    }

    public record Closure(ISequence Parameters, Mal Ast, Environment Env, Func<ISequence,Mal> Fn) : Function(Fn)
    {
    }

    private Mal() { } // private constructor can prevent derived cases from being defined elsewhere
}

public class MalRuntimeException : Exception
{
    public MalRuntimeException(string message) : base(message) { }
}

public class MalSyntaxException : Exception
{
    public MalSyntaxException(string message) : base(message) { }
}

public class MalLookupException : Exception
{
    public MalLookupException(string message) : base(message) { }
}