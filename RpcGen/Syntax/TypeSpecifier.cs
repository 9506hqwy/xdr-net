namespace RpcGen;

public class TypeSpecifier
{
    public TypeSpecifier(IdentifierToken identifier)
    {
        this.Identifier = identifier;
    }

    public TypeSpecifier(IList<ReservedToken> reserved)
    {
        this.Reserved = reserved;
    }

    public IList<ReservedToken>? Reserved { get; }

    public IdentifierToken? Identifier { get; }

    public bool IsBool => this.IsType(ReservedType.TypeBool);

    public bool IsVoid => this.IsType(ReservedType.TypeVoid);

    public static TypeSpecifier Take(TokenReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.TryExpectIdentifier(out var identifier))
        {
            return new TypeSpecifier(identifier!);
        }

        var reserveds = new List<ReservedToken>();
        while (reader.TryExpectReservedToken(out var reserved))
        {
            reserveds.Add(reserved!);
        }

        return reserveds.Count == 0
            ? throw new Exception($"Not found identifier ({reader.Current.Position}).")
            : new TypeSpecifier(reserveds);
    }

    public bool IsType(ReservedType type) => this.Reserved?.Any(r => r.Type == type) ?? false;
}
