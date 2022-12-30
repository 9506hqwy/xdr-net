namespace RpcGen;

public class EnumDef : IDefinition
{
    public EnumDef(IdentifierToken identifier, IDictionary<IdentifierToken, Value> body)
    {
        this.Identifier = identifier;
        this.Body = body;
    }

    public IdentifierToken Identifier { get; }

    public IDictionary<IdentifierToken, Value> Body { get; }

    public static EnumDef Take(TokenReader reader)
    {
        var identifier = reader.ExpectIdentifier();

        reader.ExpectBraceStart();

        var body = new Dictionary<IdentifierToken, Value>();
        while (!reader.Empty)
        {
            if (reader.TryExpectBraceEnd(out var _))
            {
                break;
            }

            if (body.Count != 0)
            {
                reader.ExpectComma();
            }

            var key = reader.ExpectIdentifier();

            reader.ExpectEqual();

            var value = Value.Take(reader);

            body.Add(key, value);
        }

        reader.ExpectSemicolon();

        return new EnumDef(identifier, body);
    }
}
