namespace Xdr;

public interface IXdrOption
{
    object Data { get; }
}

public class XdrOption<T> : IXdrOption
{
    public XdrOption(T value)
    {
        this.Value = value;
    }

    public object Data => this.Value!;

    public T Value { get; set; }
}
