namespace RpcGen;

public abstract class NumberToken : Token
{
#pragma warning disable CA1721
    public abstract object GetValue<T>();
#pragma warning restore CA1721
}
