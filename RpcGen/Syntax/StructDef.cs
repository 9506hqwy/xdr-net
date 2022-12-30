namespace RpcGen;

public class StructDef : IDefinition
{
    public StructDef(IdentifierToken identifier, IList<Declaration> body)
    {
        this.Identifier = identifier;
        this.Body = body;
    }

    public IdentifierToken Identifier { get; }

    public IList<Declaration> Body { get; }

    public static StructDef Take(TokenReader reader)
    {
        var identifier = reader.ExpectIdentifier();

        reader.ExpectBraceStart();

        var body = new List<Declaration>();
        while (!reader.Empty)
        {
            if (reader.TryExpectBraceEnd(out var _))
            {
                break;
            }

            var decl = Declaration.Take(reader);
            body.Add(decl);

            reader.ExpectSemicolon();
        }

        reader.ExpectSemicolon();

        return new StructDef(identifier, body);
    }
}
