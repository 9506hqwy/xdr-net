namespace Xdr;

public interface IXdrUnion
{
    object Data { get; }
}

public abstract class XdrUnion<T> : IXdrUnion
{
    public XdrUnion(T value)
    {
        this.Value = value;
    }

    public object Data => this.Value!;

    public T Value { get; set; }
}
