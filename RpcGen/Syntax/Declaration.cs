namespace RpcGen;

public class Declaration : IDefinition
{
    private const string VoidIdentifier = "void";

    public Declaration(TypeSpecifier typeSpec, string identifier, bool optional, Value? @fixed, bool variable)
    {
        this.Type = typeSpec;
        this.Identifier = identifier;
        this.Optional = optional;
        this.Fixed = @fixed;
        this.Variable = variable;
    }

    public Value? Fixed { get; }

    public string Identifier { get; }

    public bool Optional { get; }

    public TypeSpecifier Type { get; }

    public bool Variable { get; }

    public static Declaration Take(TokenReader reader)
    {
        var typeSpec = TypeSpecifier.Take(reader);
        if (typeSpec.IsVoid)
        {
            return new Declaration(typeSpec, Declaration.VoidIdentifier, false, null, false);
        }

        var identifier = reader.ExpectIdentifier();
        var val = identifier.Value.TrimStart('*');

        Value? @fixed = null;
        bool variable = false;
        if (reader.TryExpectAngleBracketStart(out var _))
        {
            if (!reader.TryExpectAngleBracketEnd(out var _))
            {
                Value.Take(reader);
                reader.ExpectAngleBracketEnd();
            }

            variable = true;
        }
        else if (reader.TryExpectBracketStart(out var _))
        {
            @fixed = Value.Take(reader);
            reader.ExpectBracketEnd();
        }

        return new Declaration(typeSpec, val, val != identifier.Value, @fixed, variable);
    }
}
