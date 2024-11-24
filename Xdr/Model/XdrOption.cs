namespace Xdr;

public interface IXdrOption
{
    object Data { get; }
}

[Serializable]
public class XdrOption<T>(T value) : IXdrOption
{
    public object Data => this.Value!;

    public T Value { get; set; } = value;
}
