namespace RpcGen;

public abstract class NumberToken : Token
{
    public abstract object GetValue<T>();
}
