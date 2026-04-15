namespace TicTacToe.Code;

public readonly struct Maybe<T>
{
    private readonly T _value;
    public readonly bool HasValue { get; }
    public readonly T Value => HasValue ? _value : throw new InvalidOperationException("No value present");
    public readonly Maybe<T> OrElse(Func<Maybe<T>> fallback) => HasValue ? this : fallback();

    private Maybe(T value)
    {
        _value = value;
        HasValue = true;
    }

    public static Maybe<T> None => new();
    public static Maybe<T> Some(T value) => new(value);
}
