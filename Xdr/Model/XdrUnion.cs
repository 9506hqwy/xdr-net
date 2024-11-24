namespace Xdr;

public interface IXdrUnion
{
    object Data { get; }
}

[Serializable]
public abstract class XdrUnion<T>(T value) : IXdrUnion
{
    public object Data => this.Value!;

    public T Value { get; set; } = value;
}
