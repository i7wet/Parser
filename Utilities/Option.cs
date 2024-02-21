﻿using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Utilities;

public static class Option
{
    public static Option<T> Some<T>(T value) => new(value, hasValue: true);

    public static Option<Unit> None { get; } = new(default, hasValue: false);

    public static Option<T> NoneIfEmpty<T>(T value) where T : IEnumerable
    {
        if (value is ICollection collection)
            return collection.Count == 0 ? None : Some(value);

        var enumerator = value.GetEnumerator();
        return enumerator.MoveNext() ? Some(value) : None;
    }
    
}
public readonly struct Option<T> : IEquatable<Option<T>>
{
    static Option<T> None { get; } = new(default!, hasValue: false);

    public Option()
    {
        HasValue = false;
        Value = default;
    }
    
    public Option(T value, bool hasValue = true)
    {
        Value = value;
        HasValue = hasValue;
    }
    
    bool HasValue { get; }

    public T SomeOr(T defaultValue)
    {
        return Value ?? defaultValue;
    }
    
    public T SomeOr(Func<T> defaultValue)
    {
        return Value ?? defaultValue();
    }
    
    [StackTraceHidden]
    [Pure]
    public T Unwrap()
    {
        if (HasValue is false)
            throw new InvalidOperationException("Option is empty");
        
        return Value;
    }
    
    public T? UnwrapOrNull()
    {
        return Value;
    }
    
    [StackTraceHidden]
    public TAs UnwrapAs<TAs>() where TAs : T 
    {
        return Value is TAs value 
            ? value 
            : throw new InvalidOperationException("Option is empty");
    }
    
    public bool IsNone => !HasValue;
    public bool IsSome => HasValue;

    T? Value { get; }
    
    public static implicit operator Option<T>(T? value)
    {
        return Option.Some(value);
    }
    
    public static implicit operator Option<T>(Option<Unit> value) => None;

    public bool Equals(Option<T> other) 
        => HasValue == other.HasValue 
           && EqualityComparer<T?>.Default.Equals(Value, other.Value);

    public override bool Equals(object? obj)
    {
        return obj is Option<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(HasValue, Value);
    }

    public void OnSome(Action<T> action)
    {
        if (HasValue)
            action(Value!);
        
    }
    
    public IEnumerable<Y> SomeOrEmpty<Y>()
    {
        if (!typeof(T).CanBeConvertedTo<IEnumerable<Y>>())
        {
            throw new Exception("method can be used only with IEnumerable<Y>");
        }
        
        if (this.IsSome)
        {
            return (IEnumerable<Y>)this.Unwrap();
        }
        
        return (IEnumerable<Y>)typeof(Enumerable).GetMethod("Empty").NullGuard()
            .GetGenericMethodDefinition().MakeGenericMethod(typeof(Y))
            .Invoke(null, null)
            .NullGuard();
    }
    
    
}