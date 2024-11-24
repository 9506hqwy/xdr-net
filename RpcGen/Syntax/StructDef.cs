namespace RpcGen;

public class StructDef(IdentifierToken identifier, IList<Declaration> body) : IDefinition
{
    public IdentifierToken Identifier { get; } = identifier;

    public IList<Declaration> Body { get; } = body;

    public static StructDef Take(TokenReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var identifier = reader.ExpectIdentifier();

        _ = reader.ExpectBraceStart();

        var body = new List<Declaration>();
        while (!reader.Empty)
        {
            if (reader.TryExpectBraceEnd(out _))
            {
                break;
            }

            var decl = Declaration.Take(reader);
            body.Add(decl);

            _ = reader.ExpectSemicolon();
        }

        _ = reader.ExpectSemicolon();

        return new StructDef(identifier, body);
    }
}
