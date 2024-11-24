namespace RpcGen;

public class EnumDef(IdentifierToken identifier, IDictionary<IdentifierToken, Value> body) : IDefinition
{
    public IdentifierToken Identifier { get; } = identifier;

    public IDictionary<IdentifierToken, Value> Body { get; } = body;

    public static EnumDef Take(TokenReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var identifier = reader.ExpectIdentifier();

        _ = reader.ExpectBraceStart();

        var body = new Dictionary<IdentifierToken, Value>();
        while (!reader.Empty)
        {
            if (reader.TryExpectBraceEnd(out _))
            {
                break;
            }

            if (body.Count != 0)
            {
                _ = reader.ExpectComma();
            }

            var key = reader.ExpectIdentifier();

            _ = reader.ExpectEqual();

            var value = Value.Take(reader);

            body.Add(key, value);
        }

        _ = reader.ExpectSemicolon();

        return new EnumDef(identifier, body);
    }
}
