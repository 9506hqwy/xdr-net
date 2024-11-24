namespace RpcGen;

public class Value
{
    public Value(IdentifierToken token)
    {
        this.Identifier = token;
    }

    public Value(Constant constant)
    {
        this.Constant = constant;
    }

    public Constant? Constant { get; }

    public IdentifierToken? Identifier { get; }

    public bool IsFalse => this.Identifier is not null && this.Identifier.Value == "FALSE";

    public bool IsTrue => this.Identifier is not null && this.Identifier.Value == "TRUE";

    public static Value Take(TokenReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.TryExpectIdentifier(out var identifier))
        {
            return new Value(identifier!);
        }

        var constant = Constant.Take(reader);
        return new Value(constant);
    }
}
