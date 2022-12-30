namespace RpcGen;

public abstract class Token
{
    public Position Position { get; set; } = Position.Empty;

    public string Value { get; set; } = string.Empty;
}
