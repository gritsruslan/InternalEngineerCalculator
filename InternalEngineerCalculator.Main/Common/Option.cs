namespace InternalEngineerCalculator.Main.Common;

/// <summary> Custom Option datatype implementation (like in Rust) </summary>
public class Option<T>
{
    private static string NoneValueMessage => $"The value in Option<{typeof(T)}> was None!";

    private Option(T value)
    {
        if (value is null)
        {
            IsNone = true;
            _value = default!;
        }
        else
        {
            IsNone = false;
            _value = value;
        }
    }
    private Option()
    {
        IsNone = true;
        _value = default!;
    }

    private readonly T _value;

    public readonly bool IsNone;

    public bool IsSome => !IsNone;

    public static Option<T> None => new();

    public static Option<T> Some(T value) => new(value);

    public T Unwrap() =>
        IsNone ? throw new InvalidOperationException(NoneValueMessage) : _value;

    public T UnwrapOr(T defaultValue) =>
        IsNone ? defaultValue : _value;

    public bool TryUnwrap(out T value)
    {
        if (IsNone)
        {
            value = default!;
            return false;
        }

        value = _value;
        return true;
    }

    public void Expect(string message)
    {
        if (IsNone)
            throw new InvalidOperationException(NoneValueMessage);
    }

    public void Expect(Exception exception)
    {
        if (IsNone)
            throw exception;
    }

    public Option<T> OrElse(Func<Option<T>> alternative) =>
        IsNone ? alternative.Invoke() : this;

    public Option<T> And(Option<T> other) =>
        IsSome || other.IsSome ? other : throw new InvalidOperationException(NoneValueMessage);

    public Option<TU> AndThen<TU>(Func<T, Option<TU>> func) =>
        IsNone ? Option<TU>.None : func.Invoke(_value);

    public override bool Equals(object? obj)
    {
        if (obj is not Option<T> option)
            return false;

        if (IsNone)
        {
            return option.IsNone;
        }

        return !option.IsNone && _value!.Equals(option.Unwrap());
    }

    public override int GetHashCode()
    {
        if (IsNone)
            return 0;

        return _value?.GetHashCode() ?? 0;
    }

    public static implicit operator Option<T>(T value) => new(value);

    public static explicit operator T(Option<T> option) => option.Unwrap();
}