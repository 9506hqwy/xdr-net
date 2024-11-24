namespace RpcGen;

public class ProcedureDef(IdentifierToken identifier, TypeSpecifier returnType, TypeSpecifier argType, Value value)
{
    public TypeSpecifier ArgType { get; } = argType;

    public IdentifierToken Identifier { get; } = identifier;

    public TypeSpecifier ReturnType { get; } = returnType;

    public Value Value { get; } = value;

    public static ProcedureDef Take(TokenReader reader)
    {
        var returnType = TypeSpecifier.Take(reader);

        var identifier = reader.ExpectIdentifier();

        _ = reader.ExpectParenStart();

        var argType = TypeSpecifier.Take(reader);

        _ = reader.ExpectParenEnd();

        _ = reader.ExpectEqual();

        var value = Value.Take(reader);

        return new ProcedureDef(identifier, returnType, argType, value);
    }
}
