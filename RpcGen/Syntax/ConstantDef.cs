namespace RpcGen;

public class ConstantDef(IdentifierToken identifier, Constant constant) : IDefinition
{
    public IdentifierToken Identifier { get; } = identifier;

    public Constant Constant { get; } = constant;

    public static ConstantDef Take(TokenReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var identifier = reader.ExpectIdentifier();

        _ = reader.ExpectEqual();

        var constant = Constant.Take(reader);

        _ = reader.ExpectSemicolon();

        return new ConstantDef(identifier, constant);
    }
}
