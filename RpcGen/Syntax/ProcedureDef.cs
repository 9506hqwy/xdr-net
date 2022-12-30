namespace RpcGen;

public class ProcedureDef
{
    public ProcedureDef(IdentifierToken identifier, TypeSpecifier returnType, TypeSpecifier argType, Value value)
    {
        this.Identifier = identifier;
        this.ReturnType = returnType;
        this.ArgType = argType;
        this.Value = value;
    }

    public TypeSpecifier ArgType { get; }

    public IdentifierToken Identifier { get; }

    public TypeSpecifier ReturnType { get; }

    public Value Value { get; }

    public static ProcedureDef Take(TokenReader reader)
    {
        var returnType = TypeSpecifier.Take(reader);

        var identifier = reader.ExpectIdentifier();

        reader.ExpectParenStart();

        var argType = TypeSpecifier.Take(reader);

        reader.ExpectParenEnd();

        reader.ExpectEqual();

        var value = Value.Take(reader);

        return new ProcedureDef(identifier, returnType, argType, value);
    }
}
