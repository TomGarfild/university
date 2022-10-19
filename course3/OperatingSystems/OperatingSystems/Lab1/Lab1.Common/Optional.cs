namespace Lab1.Common;

public sealed class Optional<T>
{
    
    private readonly T _value;
    
    private Optional() {
        _value = default;
    }
    public static Optional<T> Empty()
    {
        return new();
    }
    
    private Optional(T value)
    {
        _value = value ?? throw new ArgumentNullException("value cannot be null");
    }
    
    public static Optional<T> Of(T value) {
        return new Optional<T>(value);
    }
    
    public static Optional<T> OfNullable(T value) {
        return value == null ? Empty() : Of(value);
    }
    
    public T Get() {
        if (_value == null) {
            throw new ArgumentNullException("No value present");
        }
        return _value;
    }
    
    public bool IsPresent() {
        return _value != null;
    }
    public Optional<U> Map<U>(Func<T, U> mapper) where U : class
    {
        if (mapper == null)
        {
            throw new ArgumentNullException("mapper cannot be null");
        }
        if (!IsPresent())
        {
            return Optional<U>.Empty();
        }

        return Optional<U>.OfNullable(mapper(_value));
    }
    
    public Optional<U> FlatMap<U>(Func<T, Optional<U>> mapper) where U : class
    {
        if (mapper == null)
        {
            throw new ArgumentNullException("mapper cannot be null");
        }
        if (!IsPresent())
        {
            return Optional<U>.Empty();
        }

        return mapper(_value) ?? throw new ArgumentNullException();
    }
    
    public T OrElse(T other) {
        return _value != null ? _value : other;
    }
    
    public T OrElseGet(Func<T> other) {
        return _value != null ? _value : other();
    }
    
    public  T OrElseThrow<TException>(Func<TException> exceptionSupplier) where TException : Exception {
        if (_value != null) {
            return _value;
        }

        throw exceptionSupplier();
    }
    
    public override bool Equals(object obj) {
        if (this == obj) {
            return true;
        }

        return obj is Optional<T> other && _value.Equals(other._value);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
    
    public override string ToString() {
        return _value != null ? $"Optional{_value}" : "Optional.empty";
    }
}