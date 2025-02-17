namespace TicTacToe.Code;

public struct Maybe<T>
{
    private readonly T _value;
    public bool HasValue { get; }
    public T Value => HasValue ? _value : throw new InvalidOperationException("No value present");

    private Maybe(T value)
    {
        _value = value;
        HasValue = true;
    }

    public static Maybe<T> None => new Maybe<T>();
    public static Maybe<T> Some(T value) => new Maybe<T>(value);
}
