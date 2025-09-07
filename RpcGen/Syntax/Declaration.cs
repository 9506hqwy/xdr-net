namespace RpcGen;

public class Declaration(TypeSpecifier typeSpec, string identifier, bool optional, Value? @fixed, bool variable) : IDefinition
{
    private const string VoidIdentifier = "void";

    public Value? Fixed { get; } = @fixed;

    public string Identifier { get; } = identifier;

    public bool Optional { get; } = optional;

    public TypeSpecifier Type { get; } = typeSpec;

    public bool Variable { get; } = variable;

    public static Declaration Take(TokenReader reader)
    {
        var typeSpec = TypeSpecifier.Take(reader);
        if (typeSpec.IsVoid)
        {
            return new Declaration(typeSpec, VoidIdentifier, false, null, false);
        }

        var identifier = reader.ExpectIdentifier();
        var val = identifier.Value.TrimStart('*');

        Value? @fixed = null;
        bool variable = false;
        if (reader.TryExpectAngleBracketStart(out _))
        {
            if (!reader.TryExpectAngleBracketEnd(out _))
            {
                _ = Value.Take(reader);
                _ = reader.ExpectAngleBracketEnd();
            }

            variable = true;
        }
        else if (reader.TryExpectBracketStart(out _))
        {
            @fixed = Value.Take(reader);
            _ = reader.ExpectBracketEnd();
        }

        return new Declaration(typeSpec, val, val != identifier.Value, @fixed, variable);
    }
}
