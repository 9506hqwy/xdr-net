namespace RpcGen;

public class ConstantDef : IDefinition
{
    public ConstantDef(IdentifierToken identifier, Constant constant)
    {
        this.Identifier = identifier;
        this.Constant = constant;
    }

    public IdentifierToken Identifier { get; }

    public Constant Constant { get; }

    public static ConstantDef Take(TokenReader reader)
    {
        var identifier = reader.ExpectIdentifier();

        reader.ExpectEqual();

        var constant = Constant.Take(reader);

        reader.ExpectSemicolon();

        return new ConstantDef(identifier, constant);
    }
}
